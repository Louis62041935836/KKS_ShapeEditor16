using System;

namespace KKShapeEditor
{
	// Token: 0x02000028 RID: 40
	public class LayerRenameUndoEntry : IUndoEntry
	{
		// Token: 0x060001FC RID: 508 RVA: 0x000116F3 File Offset: 0x0000F8F3
		public LayerRenameUndoEntry(DeformLayer layer, string before, string after)
		{
			this._layer = layer;
			this._before = before;
			this._after = after;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00011710 File Offset: 0x0000F910
		public void Undo(UndoContext ctx)
		{
			this._layer.Name = this._before;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00011723 File Offset: 0x0000F923
		public void Redo(UndoContext ctx)
		{
			this._layer.Name = this._after;
		}

		// Token: 0x040000EA RID: 234
		private readonly DeformLayer _layer;

		// Token: 0x040000EB RID: 235
		private readonly string _before;

		// Token: 0x040000EC RID: 236
		private readonly string _after;
	}
}
