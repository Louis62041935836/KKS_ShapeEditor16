using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200000E RID: 14
	public static class LayerMirror
	{
		// Token: 0x0600007A RID: 122 RVA: 0x00004210 File Offset: 0x00002410
		public static DeformLayer MirrorLayerIntoNew(ShapeDeformer deformer, Transform rendererXform, Transform objectRoot, DeformData data, DeformLayer source)
		{
			if (deformer == null || data == null || source == null)
			{
				return null;
			}
			if (rendererXform == null)
			{
				return null;
			}
			if (source.Deltas == null)
			{
				return null;
			}
			Vector3[] bindVertices = deformer.BindVertices;
			if (bindVertices == null || bindVertices.Length == 0)
			{
				return null;
			}
			Transform transform = ((objectRoot != null) ? objectRoot : rendererXform);
			Vector3[] deltas = source.Deltas;
			int num = deltas.Length;
			int num2 = bindVertices.Length;
			SpatialHashGrid spatialHashGrid = new SpatialHashGrid(bindVertices);
			DeformLayer deformLayer = data.AddLayer(num);
			Vector3[] deltas2 = deformLayer.Deltas;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = deltas[i];
				if (vector.sqrMagnitude > 0f && i < num2)
				{
					num3++;
					Vector3 vector2 = rendererXform.TransformPoint(bindVertices[i]);
					Vector3 vector3 = transform.InverseTransformPoint(vector2);
					vector3.x = -vector3.x;
					Vector3 vector4 = transform.TransformPoint(vector3);
					Vector3 vector5 = rendererXform.InverseTransformPoint(vector4);
					LayerMirror._found.Clear();
					spatialHashGrid.FindVerticesInRadius(vector5, 0.0002f, LayerMirror._collect);
					if (LayerMirror._found.Count != 0)
					{
						num4++;
						Vector3 vector6;
						deformer.BindDeltaToWorld(i, vector, out vector6);
						Vector3 vector7 = transform.InverseTransformVector(vector6);
						vector7.x = -vector7.x;
						Vector3 vector8 = transform.TransformVector(vector7);
						for (int j = 0; j < LayerMirror._found.Count; j++)
						{
							int num5 = LayerMirror._found[j];
							if (num5 >= 0 && num5 < num)
							{
								Vector3 vector9;
								deformer.WorldDeltaToBindDelta(num5, vector8, out vector9);
								deltas2[num5] = vector9;
							}
						}
					}
				}
			}
			if (num3 > 0 && num4 == 0)
			{
				ShapeEditorPlugin.Logger.LogWarning(string.Concat(new string[]
				{
					"[LayerMirror] '",
					source.Name,
					"': none of ",
					num3.ToString(),
					" sculpted vertices found a symmetric partner — is the mesh left-right symmetric and the character near neutral pose?"
				}));
			}
			else
			{
				ShapeEditorPlugin.Logger.LogInfo(string.Concat(new string[]
				{
					"[LayerMirror] '",
					source.Name,
					"': ",
					num4.ToString(),
					"/",
					num3.ToString(),
					" sculpted vertices mirrored across the ObjectRoot X plane."
				}));
			}
			deformLayer.Dirty = true;
			return deformLayer;
		}

		// Token: 0x0400002C RID: 44
		private const float ColocTolerance = 0.0002f;

		// Token: 0x0400002D RID: 45
		private static readonly List<int> _found = new List<int>(8);

		// Token: 0x0400002E RID: 46
		private static readonly Action<int, float> _collect = delegate(int idx, float distSq)
		{
			LayerMirror._found.Add(idx);
		};
	}
}
