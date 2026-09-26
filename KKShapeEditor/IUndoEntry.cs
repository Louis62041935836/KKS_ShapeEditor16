using System;

namespace KKShapeEditor
{
	// Token: 0x02000025 RID: 37
	public interface IUndoEntry
	{
		// Token: 0x060001ED RID: 493
		void Undo(UndoContext ctx);

		// Token: 0x060001EE RID: 494
		void Redo(UndoContext ctx);
	}
}
