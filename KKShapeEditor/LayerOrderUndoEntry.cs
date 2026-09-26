using System;

namespace KKShapeEditor
{
	// Token: 0x0200002D RID: 45
	public class LayerOrderUndoEntry : IUndoEntry
	{
		// Token: 0x0600020E RID: 526 RVA: 0x00011B20 File Offset: 0x0000FD20
		public LayerOrderUndoEntry(DeformData data, DeformLayer[] before, DeformLayer[] after, int beforeActive, int afterActive)
		{
			this._data = data;
			this._before = before;
			this._after = after;
			this._beforeActive = beforeActive;
			this._afterActive = afterActive;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00011B50 File Offset: 0x0000FD50
		public void Undo(UndoContext ctx)
		{
			this._data.Layers.Clear();
			for (int i = 0; i < this._before.Length; i++)
			{
				this._data.Layers.Add(this._before[i]);
			}
			this._data.ActiveLayerIndex = this._beforeActive;
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00011BAC File Offset: 0x0000FDAC
		public void Redo(UndoContext ctx)
		{
			this._data.Layers.Clear();
			for (int i = 0; i < this._after.Length; i++)
			{
				this._data.Layers.Add(this._after[i]);
			}
			this._data.ActiveLayerIndex = this._afterActive;
		}

		// Token: 0x040000F9 RID: 249
		private readonly DeformData _data;

		// Token: 0x040000FA RID: 250
		private readonly DeformLayer[] _before;

		// Token: 0x040000FB RID: 251
		private readonly DeformLayer[] _after;

		// Token: 0x040000FC RID: 252
		private readonly int _beforeActive;

		// Token: 0x040000FD RID: 253
		private readonly int _afterActive;
	}
}
