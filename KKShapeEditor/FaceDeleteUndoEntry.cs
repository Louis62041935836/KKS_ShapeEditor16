using System;

namespace KKShapeEditor
{
	// Token: 0x0200002E RID: 46
	public class FaceDeleteUndoEntry : IUndoEntry
	{
		// Token: 0x06000211 RID: 529 RVA: 0x00011C05 File Offset: 0x0000FE05
		public FaceDeleteUndoEntry(DeformData data, int[] addedIndices)
		{
			this._data = data;
			this._addedIndices = addedIndices;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00011C1C File Offset: 0x0000FE1C
		public void Undo(UndoContext ctx)
		{
			if (this._data == null || this._addedIndices == null)
			{
				return;
			}
			for (int i = 0; i < this._addedIndices.Length; i++)
			{
				this._data.DeletedFaces.Remove(this._addedIndices[i]);
			}
			this._data.DeletedFacesDirty = true;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00011C74 File Offset: 0x0000FE74
		public void Redo(UndoContext ctx)
		{
			if (this._data == null || this._addedIndices == null)
			{
				return;
			}
			for (int i = 0; i < this._addedIndices.Length; i++)
			{
				this._data.DeletedFaces.Add(this._addedIndices[i]);
			}
			this._data.DeletedFacesDirty = true;
		}

		// Token: 0x040000FE RID: 254
		private readonly DeformData _data;

		// Token: 0x040000FF RID: 255
		private readonly int[] _addedIndices;
	}
}
