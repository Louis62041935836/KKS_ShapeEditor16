using System;
using System.Collections.Generic;

namespace KKShapeEditor
{
	// Token: 0x02000029 RID: 41
	public class MultiDeltaUndoEntry : IUndoEntry
	{
		// Token: 0x060001FF RID: 511 RVA: 0x00011736 File Offset: 0x0000F936
		public MultiDeltaUndoEntry()
		{
			this._children = new List<IUndoEntry>();
			this._affectedDeformers = new List<ShapeDeformer>();
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000200 RID: 512 RVA: 0x00011754 File Offset: 0x0000F954
		public int ChildCount
		{
			get
			{
				return this._children.Count;
			}
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00011761 File Offset: 0x0000F961
		public void Add(IUndoEntry child)
		{
			if (child != null)
			{
				this._children.Add(child);
			}
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00011774 File Offset: 0x0000F974
		public void TrackDeformer(ShapeDeformer deformer)
		{
			if (deformer == null)
			{
				return;
			}
			for (int i = 0; i < this._affectedDeformers.Count; i++)
			{
				if (this._affectedDeformers[i] == deformer)
				{
					return;
				}
			}
			this._affectedDeformers.Add(deformer);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x000117C4 File Offset: 0x0000F9C4
		public void Undo(UndoContext ctx)
		{
			for (int i = this._children.Count - 1; i >= 0; i--)
			{
				this._children[i].Undo(ctx);
			}
			for (int j = 0; j < this._affectedDeformers.Count; j++)
			{
				if (this._affectedDeformers[j] != null)
				{
					this._affectedDeformers[j].InvalidateDeltaCache();
				}
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00011838 File Offset: 0x0000FA38
		public void Redo(UndoContext ctx)
		{
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].Redo(ctx);
			}
			for (int j = 0; j < this._affectedDeformers.Count; j++)
			{
				if (this._affectedDeformers[j] != null)
				{
					this._affectedDeformers[j].InvalidateDeltaCache();
				}
			}
		}

		// Token: 0x040000ED RID: 237
		private readonly List<IUndoEntry> _children;

		// Token: 0x040000EE RID: 238
		private readonly List<ShapeDeformer> _affectedDeformers;
	}
}
