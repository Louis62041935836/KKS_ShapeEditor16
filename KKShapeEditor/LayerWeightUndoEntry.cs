using System;

namespace KKShapeEditor
{
	// Token: 0x0200002C RID: 44
	public class LayerWeightUndoEntry : IUndoEntry
	{
		// Token: 0x0600020B RID: 523 RVA: 0x00011AC5 File Offset: 0x0000FCC5
		public LayerWeightUndoEntry(DeformLayer layer, float before, float after)
		{
			this._layer = layer;
			this._before = before;
			this._after = after;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00011AE2 File Offset: 0x0000FCE2
		public void Undo(UndoContext ctx)
		{
			this._layer.Weight = this._before;
			this._layer.Dirty = true;
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00011B01 File Offset: 0x0000FD01
		public void Redo(UndoContext ctx)
		{
			this._layer.Weight = this._after;
			this._layer.Dirty = true;
		}

		// Token: 0x040000F6 RID: 246
		private readonly DeformLayer _layer;

		// Token: 0x040000F7 RID: 247
		private readonly float _before;

		// Token: 0x040000F8 RID: 248
		private readonly float _after;
	}
}
