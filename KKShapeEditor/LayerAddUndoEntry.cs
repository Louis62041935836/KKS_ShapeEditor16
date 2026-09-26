using System;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200002A RID: 42
	public class LayerAddUndoEntry : IUndoEntry
	{
		// Token: 0x06000205 RID: 517 RVA: 0x000118A8 File Offset: 0x0000FAA8
		public LayerAddUndoEntry(DeformData data, DeformLayer layer, int index)
		{
			this._data = data;
			this._layer = layer;
			this._index = index;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x000118C8 File Offset: 0x0000FAC8
		public void Undo(UndoContext ctx)
		{
			this._data.Layers.Remove(this._layer);
			if (this._data.Layers.Count == 0)
			{
				this._data.ActiveLayerIndex = -1;
				return;
			}
			if (this._data.ActiveLayerIndex >= this._data.Layers.Count)
			{
				this._data.ActiveLayerIndex = this._data.Layers.Count - 1;
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00011948 File Offset: 0x0000FB48
		public void Redo(UndoContext ctx)
		{
			if (this._layer == null)
			{
				return;
			}
			for (int i = 0; i < this._data.Layers.Count; i++)
			{
				if (this._data.Layers[i].Name == this._layer.Name)
				{
					Debug.LogWarning("LayerAddUndoEntry.Redo skipped: a layer named '" + this._layer.Name + "' already exists (likely re-added manually after Undo).");
					return;
				}
			}
			int num = ((this._index <= this._data.Layers.Count) ? this._index : this._data.Layers.Count);
			this._data.Layers.Insert(num, this._layer);
			this._data.ActiveLayerIndex = num;
		}

		// Token: 0x040000EF RID: 239
		private readonly DeformData _data;

		// Token: 0x040000F0 RID: 240
		private readonly DeformLayer _layer;

		// Token: 0x040000F1 RID: 241
		private readonly int _index;
	}
}
