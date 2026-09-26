using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000038 RID: 56
	public class PinchTool : IDeformTool
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600026F RID: 623 RVA: 0x000145E7 File Offset: 0x000127E7
		// (set) Token: 0x06000270 RID: 624 RVA: 0x000145EF File Offset: 0x000127EF
		public float Amount { get; set; }

		// Token: 0x06000271 RID: 625 RVA: 0x000145F8 File Offset: 0x000127F8
		public void Apply(ShapeDeformer deformer, DeformLayer layer, BrushResult brushResult, Vector3[] vertices, Vector3[] normals, Camera camera)
		{
			if (layer == null || brushResult == null || vertices == null)
			{
				return;
			}
			float amount = this.Amount;
			if (Mathf.Approximately(amount, 0f))
			{
				return;
			}
			Vector3[] deltas = layer.Deltas;
			Vector3 hitPoint = brushResult.HitPoint;
			Vector3 vector = brushResult.HitNormal;
			float magnitude = vector.magnitude;
			if (magnitude < 1E-06f)
			{
				return;
			}
			vector /= magnitude;
			Transform transform = ((deformer != null) ? deformer.DisplayTransform : null);
			bool flag = this.Mode == PinchTool.PinchMode.Crease;
			foreach (KeyValuePair<int, float> keyValuePair in brushResult.AffectedVertices)
			{
				int key = keyValuePair.Key;
				float value = keyValuePair.Value;
				if (key >= 0 && key < deltas.Length && key < vertices.Length)
				{
					Vector3 vector2 = ((transform != null) ? transform.TransformPoint(vertices[key]) : vertices[key]);
					Vector3 vector3 = hitPoint - vector2;
					vector3 -= Vector3.Dot(vector3, vector) * vector;
					float magnitude2 = vector3.magnitude;
					if (magnitude2 >= 1E-06f)
					{
						Vector3 vector4 = vector3 / magnitude2;
						float num = amount * value;
						Vector3 vector5 = vector4 * num;
						if (flag)
						{
							Vector3 vector6 = -vector * num;
							vector5 = 0.5f * vector5 + 0.5f * vector6;
						}
						Vector3 vector7;
						if (deformer != null)
						{
							deformer.WorldDeltaToBindDelta(key, vector5, out vector7);
						}
						else
						{
							vector7 = vector5;
						}
						deltas[key] += vector7;
					}
				}
			}
			layer.Dirty = true;
		}

		// Token: 0x04000130 RID: 304
		public PinchTool.PinchMode Mode;

		// Token: 0x04000132 RID: 306
		private const float CreasePinchFactor = 0.5f;

		// Token: 0x02000068 RID: 104
		public enum PinchMode
		{
			// Token: 0x040004BE RID: 1214
			Pinch,
			// Token: 0x040004BF RID: 1215
			Crease
		}
	}
}
