using System;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000027 RID: 39
	public class DeltaUndoEntry : IUndoEntry
	{
		// Token: 0x060001F8 RID: 504 RVA: 0x000115E6 File Offset: 0x0000F7E6
		public DeltaUndoEntry(DeformLayer layer, int[] indices, Vector3[] before, Vector3[] after, ShapeDeformer deformer)
		{
			this._layer = layer;
			this._indices = indices;
			this._before = before;
			this._after = after;
			this._deformer = deformer;
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x00011613 File Offset: 0x0000F813
		public ShapeDeformer Deformer
		{
			get
			{
				return this._deformer;
			}
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0001161C File Offset: 0x0000F81C
		public void Undo(UndoContext ctx)
		{
			Vector3[] deltas = this._layer.Deltas;
			for (int i = 0; i < this._indices.Length; i++)
			{
				deltas[this._indices[i]] = this._before[i];
			}
			this._layer.Dirty = true;
			if (this._deformer != null)
			{
				this._deformer.InvalidateDeltaCache();
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00011688 File Offset: 0x0000F888
		public void Redo(UndoContext ctx)
		{
			Vector3[] deltas = this._layer.Deltas;
			for (int i = 0; i < this._indices.Length; i++)
			{
				deltas[this._indices[i]] = this._after[i];
			}
			this._layer.Dirty = true;
			if (this._deformer != null)
			{
				this._deformer.InvalidateDeltaCache();
			}
		}

		// Token: 0x040000E5 RID: 229
		private readonly DeformLayer _layer;

		// Token: 0x040000E6 RID: 230
		private readonly int[] _indices;

		// Token: 0x040000E7 RID: 231
		private readonly Vector3[] _before;

		// Token: 0x040000E8 RID: 232
		private readonly Vector3[] _after;

		// Token: 0x040000E9 RID: 233
		private readonly ShapeDeformer _deformer;
	}
}
