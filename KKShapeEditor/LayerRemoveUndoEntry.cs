using System;

namespace KKShapeEditor
{
	// Token: 0x0200002B RID: 43
	public class LayerRemoveUndoEntry : IUndoEntry
	{
		// Token: 0x06000208 RID: 520 RVA: 0x00011A15 File Offset: 0x0000FC15
		public LayerRemoveUndoEntry(DeformData data, DeformLayer layer, int index, int prevActiveIndex)
		{
			this._data = data;
			this._layer = layer;
			this._index = index;
			this._prevActiveIndex = prevActiveIndex;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00011A3C File Offset: 0x0000FC3C
		public void Undo(UndoContext ctx)
		{
			int num = ((this._index <= this._data.Layers.Count) ? this._index : this._data.Layers.Count);
			this._data.Layers.Insert(num, this._layer);
			this._data.ActiveLayerIndex = this._prevActiveIndex;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00011AA2 File Offset: 0x0000FCA2
		public void Redo(UndoContext ctx)
		{
			this._data.RemoveLayer(this._data.Layers.IndexOf(this._layer));
		}

		// Token: 0x040000F2 RID: 242
		private readonly DeformData _data;

		// Token: 0x040000F3 RID: 243
		private readonly DeformLayer _layer;

		// Token: 0x040000F4 RID: 244
		private readonly int _index;

		// Token: 0x040000F5 RID: 245
		private readonly int _prevActiveIndex;
	}
}
