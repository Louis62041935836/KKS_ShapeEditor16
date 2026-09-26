using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000037 RID: 55
	public class MoveTool : IDeformTool
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600025F RID: 607 RVA: 0x00014348 File Offset: 0x00012548
		// (set) Token: 0x06000260 RID: 608 RVA: 0x00014350 File Offset: 0x00012550
		public Vector2 MouseDelta { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000261 RID: 609 RVA: 0x00014359 File Offset: 0x00012559
		// (set) Token: 0x06000262 RID: 610 RVA: 0x00014361 File Offset: 0x00012561
		public float DragDelta { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000263 RID: 611 RVA: 0x0001436A File Offset: 0x0001256A
		// (set) Token: 0x06000264 RID: 612 RVA: 0x00014372 File Offset: 0x00012572
		public bool UseViewPlane { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000265 RID: 613 RVA: 0x0001437B File Offset: 0x0001257B
		// (set) Token: 0x06000266 RID: 614 RVA: 0x00014383 File Offset: 0x00012583
		public Transform RendererTransform { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000267 RID: 615 RVA: 0x0001438C File Offset: 0x0001258C
		// (set) Token: 0x06000268 RID: 616 RVA: 0x00014394 File Offset: 0x00012594
		public ShapeDeformer Deformer { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000269 RID: 617 RVA: 0x0001439D File Offset: 0x0001259D
		// (set) Token: 0x0600026A RID: 618 RVA: 0x000143A5 File Offset: 0x000125A5
		public int MirrorAxis
		{
			get
			{
				return this._mirrorAxis;
			}
			set
			{
				this._mirrorAxis = value;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600026B RID: 619 RVA: 0x000143AE File Offset: 0x000125AE
		// (set) Token: 0x0600026C RID: 620 RVA: 0x000143B6 File Offset: 0x000125B6
		public Transform MirrorRoot { get; set; }

		// Token: 0x0600026D RID: 621 RVA: 0x000143C0 File Offset: 0x000125C0
		public void Apply(ShapeDeformer deformer, DeformLayer layer, BrushResult brushResult, Vector3[] vertices, Vector3[] normals, Camera camera)
		{
			if (layer == null || brushResult == null)
			{
				return;
			}
			Vector3[] deltas = layer.Deltas;
			Vector3 vector;
			if (this.UseViewPlane && camera != null)
			{
				float x = this.MouseDelta.x;
				float y = this.MouseDelta.y;
				if (Mathf.Approximately(x, 0f) && Mathf.Approximately(y, 0f))
				{
					return;
				}
				vector = camera.transform.right * x + camera.transform.up * y;
			}
			else
			{
				float dragDelta = this.DragDelta;
				if (Mathf.Approximately(dragDelta, 0f))
				{
					return;
				}
				vector = brushResult.HitNormal.normalized * dragDelta;
			}
			if (this._mirrorAxis >= 0 && this._mirrorAxis <= 2)
			{
				Transform transform = ((this.MirrorRoot != null) ? this.MirrorRoot : this.RendererTransform);
				if (transform != null)
				{
					Vector3 vector2 = transform.InverseTransformVector(vector);
					if (this._mirrorAxis == 0)
					{
						vector2.x = -vector2.x;
					}
					else if (this._mirrorAxis == 1)
					{
						vector2.y = -vector2.y;
					}
					else
					{
						vector2.z = -vector2.z;
					}
					vector = transform.TransformVector(vector2);
				}
			}
			ShapeDeformer shapeDeformer = deformer ?? this.Deformer;
			foreach (KeyValuePair<int, float> keyValuePair in brushResult.AffectedVertices)
			{
				int key = keyValuePair.Key;
				float value = keyValuePair.Value;
				if (key >= 0 && key < deltas.Length)
				{
					Vector3 vector3;
					if (shapeDeformer != null)
					{
						shapeDeformer.WorldDeltaToBindDelta(key, vector, out vector3);
					}
					else
					{
						vector3 = ((this.RendererTransform != null) ? this.RendererTransform.InverseTransformVector(vector) : vector);
					}
					deltas[key] += vector3 * value;
				}
			}
			layer.Dirty = true;
		}

		// Token: 0x0400012E RID: 302
		private int _mirrorAxis = -1;
	}
}
