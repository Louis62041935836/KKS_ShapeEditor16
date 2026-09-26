using System;

namespace KKShapeEditor
{
	// Token: 0x0200002F RID: 47
	public class FaceRestoreUndoEntry : IUndoEntry
	{
		// Token: 0x06000214 RID: 532 RVA: 0x00011CCA File Offset: 0x0000FECA
		public FaceRestoreUndoEntry(DeformData data, int[] removedIndices)
		{
			this._data = data;
			this._removedIndices = removedIndices;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00011CE0 File Offset: 0x0000FEE0
		public void Undo(UndoContext ctx)
		{
			if (this._data == null || this._removedIndices == null)
			{
				return;
			}
			for (int i = 0; i < this._removedIndices.Length; i++)
			{
				this._data.DeletedFaces.Add(this._removedIndices[i]);
			}
			this._data.DeletedFacesDirty = true;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00011D38 File Offset: 0x0000FF38
		public void Redo(UndoContext ctx)
		{
			if (this._data == null || this._removedIndices == null)
			{
				return;
			}
			for (int i = 0; i < this._removedIndices.Length; i++)
			{
				this._data.DeletedFaces.Remove(this._removedIndices[i]);
			}
			this._data.DeletedFacesDirty = true;
		}

		// Token: 0x04000100 RID: 256
		private readonly DeformData _data;

		// Token: 0x04000101 RID: 257
		private readonly int[] _removedIndices;
	}
}
