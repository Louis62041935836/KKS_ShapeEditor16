using System;
using System.Collections.Generic;

namespace KKShapeEditor
{
	// Token: 0x02000026 RID: 38
	public class UndoStack
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001EF RID: 495 RVA: 0x00011458 File Offset: 0x0000F658
		public int UndoCount
		{
			get
			{
				return this._undoList.Count;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x00011465 File Offset: 0x0000F665
		public int RedoCount
		{
			get
			{
				return this._redoList.Count;
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00011472 File Offset: 0x0000F672
		public UndoStack(int maxSteps)
		{
			this._maxSteps = maxSteps;
			this._undoList = new List<IUndoEntry>(maxSteps);
			this._redoList = new List<IUndoEntry>();
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00011498 File Offset: 0x0000F698
		public void Push(IUndoEntry entry)
		{
			this._redoList.Clear();
			if (this._undoList.Count >= this._maxSteps)
			{
				this._undoList.RemoveAt(0);
			}
			this._undoList.Add(entry);
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x000114D0 File Offset: 0x0000F6D0
		public bool CanUndo
		{
			get
			{
				return this._undoList.Count > 0;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x000114E0 File Offset: 0x0000F6E0
		public bool CanRedo
		{
			get
			{
				return this._redoList.Count > 0;
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x000114F0 File Offset: 0x0000F6F0
		public void Undo(UndoContext ctx)
		{
			if (this._undoList.Count == 0)
			{
				return;
			}
			int num = this._undoList.Count - 1;
			IUndoEntry undoEntry = this._undoList[num];
			this._undoList.RemoveAt(num);
			undoEntry.Undo(ctx);
			this._redoList.Add(undoEntry);
			if (ctx.Deformer != null)
			{
				ctx.Deformer.InvalidateDeltaCache();
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00011560 File Offset: 0x0000F760
		public void Redo(UndoContext ctx)
		{
			if (this._redoList.Count == 0)
			{
				return;
			}
			int num = this._redoList.Count - 1;
			IUndoEntry undoEntry = this._redoList[num];
			this._redoList.RemoveAt(num);
			undoEntry.Redo(ctx);
			this._undoList.Add(undoEntry);
			if (ctx.Deformer != null)
			{
				ctx.Deformer.InvalidateDeltaCache();
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x000115CE File Offset: 0x0000F7CE
		public void Clear()
		{
			this._undoList.Clear();
			this._redoList.Clear();
		}

		// Token: 0x040000E2 RID: 226
		private readonly List<IUndoEntry> _undoList;

		// Token: 0x040000E3 RID: 227
		private readonly List<IUndoEntry> _redoList;

		// Token: 0x040000E4 RID: 228
		private readonly int _maxSteps;
	}
}
