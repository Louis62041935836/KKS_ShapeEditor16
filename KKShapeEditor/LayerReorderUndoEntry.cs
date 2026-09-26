using System;

namespace KKShapeEditor
{
	// Token: 0x02000030 RID: 48
	public class LayerReorderUndoEntry : IUndoEntry
	{
		// Token: 0x06000217 RID: 535 RVA: 0x00011D8E File Offset: 0x0000FF8E
		public LayerReorderUndoEntry(DeformData data, bool movedUp)
		{
			this._data = data;
			this._movedUp = movedUp;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x00011DA4 File Offset: 0x0000FFA4
		public void Undo(UndoContext ctx)
		{
			int activeLayerIndex = this._data.ActiveLayerIndex;
			if (this._movedUp)
			{
				this._data.MoveLayerDown(activeLayerIndex);
				return;
			}
			this._data.MoveLayerUp(activeLayerIndex);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00011DE0 File Offset: 0x0000FFE0
		public void Redo(UndoContext ctx)
		{
			int activeLayerIndex = this._data.ActiveLayerIndex;
			if (this._movedUp)
			{
				this._data.MoveLayerUp(activeLayerIndex);
				return;
			}
			this._data.MoveLayerDown(activeLayerIndex);
		}

		// Token: 0x04000102 RID: 258
		private readonly DeformData _data;

		// Token: 0x04000103 RID: 259
		private readonly bool _movedUp;
	}
}
