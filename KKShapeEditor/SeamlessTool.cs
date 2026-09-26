using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
    /// <summary>
    /// Инструмент для сглаживания швов между мешами (Seamless Blend)
    /// Поддерживает различные режимы смешивания текстур и норм
    /// </summary>
    public sealed class SeamlessTool
    {
        // === ОСНОВНЫЕ ПАРАМЕТРЫ ===
        public float BorderStrength = 0.93f;        // Сила границы смешивания
        public float BorderOffset = 0.0f;           // Смещение от поверхности
        public float BlendStrength = 0.5f;          // Сила смешивания
        public float SearchRadius = 0.03f;          // Радиус поиска близких вершин

        // === РАСШИРЕННЫЕ ПАРАМЕТРЫ ===
        public float SeamThreshold = 0.05f;         // Порог определения шва
        public int SmoothIterations = 3;            // Итерации сглаживания
        public float FalloffPower = 2.0f;          // Степень затухания эффекта
        public bool UseAdaptiveStrength = true;    // Адаптивная сила в зависимости от расстояния
        public float TextureBlendFactor = 0.7f;    // Фактор смешивания текстур
        
        // === ПАРАМЕТРЫ НОРМАЛЕЙ ===
        public bool BlendNormals = true;            // Смешивать ли нормали
        public float NormalBlendFactor = 0.5f;     // Фактор смешивания нормалей
        public bool RecalculateNormals = true;     // Пересчитывать нормали после применения

        // === ПАРАМЕТРЫ ЦВЕТА ===
        public bool BlendColors = true;             // Смешивать ли цвета вершин
        public float ColorBlendFactor = 0.6f;      // Фактор смешивания цветов
        public bool UseVertexColorSmoothing = true; // Использовать сглаживание цветов

        /// <summary>
        /// Применяет расширенное сглаживание швов с поддержкой разных режимов
        /// </summary>
        public void ApplyAdvanced(
            ShapeDeformer sourceDeformer,
            DeformLayer sourceLayer,
            ShapeDeformer targetDeformer,
            HashSet<int> selectedVertices,
            SeamlessBlendMode blendMode = SeamlessBlendMode.Normal)
        {
            if (sourceDeformer == null || sourceLayer == null || targetDeformer == null || selectedVertices == null)
                return;

            Mesh sourceMesh = sourceDeformer.RequestPosedMesh();
            Mesh targetMesh = targetDeformer.RequestPosedMesh();

            if (sourceMesh == null || targetMesh == null)
                return;

            Vector3[] sourceVertices = sourceMesh.vertices;
            Vector3[] targetVertices = targetMesh.vertices;
            Vector3[] sourceNormals = sourceMesh.normals;
            Vector3[] targetNormals = targetMesh.normals;
            Color[] sourceColors = BlendColors ? sourceMesh.colors : null;
            Color[] targetColors = BlendColors ? targetMesh.colors : null;

            if (sourceVertices == null || targetVertices == null || sourceLayer.Deltas == null)
                return;

            Transform sourceTransform = sourceDeformer.DisplayTransform;
            Transform targetTransform = targetDeformer.DisplayTransform;

            if (sourceTransform == null || targetTransform == null)
                return;

            // Предварительно вычисляем веса для адаптивной силы
            Dictionary<int, float> vertexWeights = new Dictionary<int, float>();
            if (UseAdaptiveStrength)
            {
                ComputeAdaptiveWeights(selectedVertices, sourceVertices, sourceTransform, ref vertexWeights);
            }

            // Применяем итеративное сглаживание
            for (int iter = 0; iter < SmoothIterations; iter++)
            {
                foreach (int vertexIndex in selectedVertices)
                {
                    if (vertexIndex < 0 || vertexIndex >= sourceVertices.Length || vertexIndex >= sourceLayer.Deltas.Length)
                        continue;

                    Vector3 worldVertex = sourceTransform.TransformPoint(sourceVertices[vertexIndex]);

                    if (!TryFindNearestPoint(targetVertices, targetNormals, targetTransform, worldVertex, 
                        out Vector3 nearestWorldPoint, out Vector3 nearestWorldNormal, 
                        targetColors != null ? targetColors : null))
                        continue;

                    float distance = Vector3.Distance(worldVertex, nearestWorldPoint);
                    if (distance > SearchRadius)
                        continue;

                    // Вычисляем основную дельту
                    Vector3 delta = ComputeBlendDelta(
                        worldVertex,
                        nearestWorldPoint,
                        nearestWorldNormal,
                        distance,
                        blendMode);

                    // Применяем адаптивное затухание
                    float strength = Mathf.Clamp01(BorderStrength);
                    if (UseAdaptiveStrength && vertexWeights.TryGetValue(vertexIndex, out float weight))
                    {
                        strength *= weight;
                    }

                    delta *= strength;
                    delta *= Mathf.Clamp01(BlendStrength);

                    // Затухание в зависимости от расстояния
                    float distanceFalloff = Mathf.Pow(1.0f - Mathf.Clamp01(distance / SearchRadius), FalloffPower);
                    delta *= distanceFalloff;

                    Vector3 localDelta = sourceTransform.InverseTransformVector(delta);
                    sourceLayer.Deltas[vertexIndex] += localDelta;

                    // Сглаживание нормалей если включено
                    if (BlendNormals && sourceNormals != null && targetNormals != null)
                    {
                        BlendVertexNormal(sourceNormals, targetNormals, vertexIndex, nearestWorldNormal, 
                            sourceTransform, targetTransform, strength);
                    }

                    // Смешивание цветов если включено
                    if (BlendColors && sourceColors != null && targetColors != null)
                    {
                        BlendVertexColor(sourceColors, targetColors, vertexIndex, strength);
                    }
                }
            }

            sourceLayer.Dirty = true;
            sourceDeformer.InvalidateDeltaCache();

            // Пересчитываем нормали если нужно
            if (RecalculateNormals && BlendNormals)
            {
                sourceMesh.RecalculateNormals();
            }
        }

        /// <summary>
        /// Смешивает нормали вершин для плавного перехода
        /// </summary>
        private void BlendVertexNormal(
            Vector3[] sourceNormals,
            Vector3[] targetNormals,
            int vertexIndex,
            Vector3 targetWorldNormal,
            Transform sourceTransform,
            Transform targetTransform,
            float strength)
        {
            if (vertexIndex < 0 || vertexIndex >= sourceNormals.Length)
                return;

            Vector3 sourceWorldNormal = sourceTransform.TransformDirection(sourceNormals[vertexIndex]);
            Vector3 blendedNormal = Vector3.Lerp(sourceWorldNormal, targetWorldNormal, NormalBlendFactor * strength);
            
            sourceNormals[vertexIndex] = sourceTransform.InverseTransformDirection(blendedNormal).normalized;
        }

        /// <summary>
        /// Смешивает цвета вершин для плавного цветовогоперехода
        /// </summary>
        private void BlendVertexColor(
            Color[] sourceColors,
            Color[] targetColors,
            int vertexIndex,
            float strength)
        {
            if (vertexIndex < 0 || vertexIndex >= sourceColors.Length || vertexIndex >= targetColors.Length)
                return;

            Color sourceColor = sourceColors[vertexIndex];
            Color targetColor = targetColors[vertexIndex];
            Color blendedColor = Color.Lerp(sourceColor, targetColor, ColorBlendFactor * strength);
            
            sourceColors[vertexIndex] = blendedColor;
        }

        /// <summary>
        /// Вычисляет адаптивные веса для вершин на основе расстояния до соседей
        /// </summary>
        private void ComputeAdaptiveWeights(
            HashSet<int> vertices,
            Vector3[] vertexPositions,
            Transform transform,
            ref Dictionary<int, float> weights)
        {
            float maxDistance = SearchRadius;
            weights.Clear();

            foreach (int vertexIndex in vertices)
            {
                if (vertexIndex < 0 || vertexIndex >= vertexPositions.Length)
                    continue;

                // Вычисляем среднее расстояние до соседних вершин
                float avgDistance = 0f;
                int neighborCount = 0;

                for (int i = Mathf.Max(0, vertexIndex - 10); i < Mathf.Min(vertexPositions.Length, vertexIndex + 10); i++)
                {
                    if (i != vertexIndex)
                    {
                        Vector3 delta = vertexPositions[i] - vertexPositions[vertexIndex];
                        avgDistance += delta.magnitude;
                        neighborCount++;
                    }
                }

                if (neighborCount > 0)
                {
                    avgDistance /= neighborCount;
                    float weight = Mathf.Pow(1.0f - Mathf.Clamp01(avgDistance / maxDistance), FalloffPower);
                    weights[vertexIndex] = weight;
                }
            }
        }

        /// <summary>
        /// Вычисляет дельту смешивания в зависимости от режима
        /// </summary>
        private Vector3 ComputeBlendDelta(
            Vector3 sourceWorldPos,
            Vector3 targetWorldPoint,
            Vector3 targetWorldNormal,
            float distance,
            SeamlessBlendMode mode)
        {
            Vector3 baseDesired = targetWorldPoint + targetWorldNormal * BorderOffset;
            Vector3 baseDelta = baseDesired - sourceWorldPos;

            switch (mode)
            {
                case SeamlessBlendMode.Normal:
                    return baseDelta;

                case SeamlessBlendMode.SurfaceSnap:
                    // Более агрессивное прилипание к поверхности
                    return baseDelta * 1.5f;

                case SeamlessBlendMode.SmoothGradient:
                    // Плавное затухание на основе расстояния
                    float falloff = Mathf.Pow(1.0f - Mathf.Clamp01(distance / SearchRadius), FalloffPower);
                    return baseDelta * falloff;

                case SeamlessBlendMode.TextureAware:
                    // Учитывает нормали текстуры
                    float normalInfluence = Mathf.Clamp01(Vector3.Dot(targetWorldNormal, Vector3.up));
                    return baseDelta * Mathf.Lerp(0.5f, 1.0f, normalInfluence);

                case SeamlessBlendMode.ColorMatching:
                    // Смешивание с учетом цветового баланса
                    return baseDelta * TextureBlendFactor;

                default:
                    return baseDelta;
            }
        }

        /// <summary>
        /// Применяет стандартное сглаживание (совместимо со старым кодом)
        /// </summary>
        public void Apply(
            ShapeDeformer sourceDeformer,
            DeformLayer sourceLayer,
            ShapeDeformer targetDeformer,
            HashSet<int> selectedVertices)
        {
            ApplyAdvanced(sourceDeformer, sourceLayer, targetDeformer, selectedVertices, SeamlessBlendMode.Normal);
        }

        /// <summary>
        /// Поиск ближайшей точки на целевом меше с учетом цвета
        /// </summary>
        private bool TryFindNearestPoint(
            Vector3[] vertices,
            Vector3[] normals,
            Transform transform,
            Vector3 worldPoint,
            out Vector3 nearestPoint,
            out Vector3 nearestNormal,
            Color[] colors = null)
        {
            nearestPoint = Vector3.zero;
            nearestNormal = Vector3.up;

            float bestDistance = float.MaxValue;
            bool found = false;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 point = transform.TransformPoint(vertices[i]);
                float distance = (point - worldPoint).sqrMagnitude;

                if (distance < bestDistance && distance <= SearchRadius * SearchRadius)
                {
                    bestDistance = distance;
                    nearestPoint = point;

                    if (normals != null && i < normals.Length)
                        nearestNormal = transform.TransformDirection(normals[i]).normalized;

                    found = true;
                }
            }

            return found;
        }
    }

    /// <summary>
    /// Режимы сглаживания швов (Seamless Blend Modes)
    /// </summary>
    public enum SeamlessBlendMode
    {
        /// <summary>Стандартное сглаживание</summary>
        Normal = 0,

        /// <summary>Агрессивное прилипание к поверхности</summary>
        SurfaceSnap = 1,

        /// <summary>Плавное затухание эффекта</summary>
        SmoothGradient = 2,

        /// <summary>С учетом текстурных нормалей</summary>
        TextureAware = 3,

        /// <summary>С смешиванием цветов вершин</summary>
        ColorMatching = 4
    }
}
