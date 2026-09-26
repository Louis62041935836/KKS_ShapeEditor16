using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000036 RID: 54
	public class InflateTool : IDeformTool
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600025B RID: 603 RVA: 0x000141F6 File Offset: 0x000123F6
		// (set) Token: 0x0600025C RID: 604 RVA: 0x000141FE File Offset: 0x000123FE
		public float Amount { get; set; }

		// Token: 0x0600025D RID: 605 RVA: 0x00014208 File Offset: 0x00012408
		public void Apply(ShapeDeformer deformer, DeformLayer layer, BrushResult brushResult, Vector3[] vertices, Vector3[] normals, Camera camera)
		{
			if (layer == null || brushResult == null || normals == null)
			{
				return;
			}
			Vector3[] deltas = layer.Deltas;
			float amount = this.Amount;
			if (Mathf.Approximately(amount, 0f))
			{
				return;
			}
			Transform transform = null;
			if (deformer != null && deformer.DisplayTransform != null)
			{
				transform = deformer.DisplayTransform;
			}
			foreach (KeyValuePair<int, float> keyValuePair in brushResult.AffectedVertices)
			{
				int key = keyValuePair.Key;
				float value = keyValuePair.Value;
				if (key >= 0 && key < deltas.Length && key < normals.Length)
				{
					Vector3 normalized = normals[key].normalized;
					Vector3 vector = ((transform != null) ? (transform.TransformDirection(normalized) * (amount * value)) : (normalized * (amount * value)));
					Vector3 vector2;
					if (deformer != null)
					{
						deformer.WorldDeltaToBindDelta(key, vector, out vector2);
					}
					else
					{
						vector2 = vector;
					}
					deltas[key] += vector2;
				}
			}
			layer.Dirty = true;
		}
	}
}
