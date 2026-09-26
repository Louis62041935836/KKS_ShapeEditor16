using System;
using System.Diagnostics;

namespace KKShapeEditor.Profiling
{
	// Token: 0x0200004D RID: 77
	public static class ShapeEditorProfiler
	{
		// Token: 0x06000459 RID: 1113 RVA: 0x0002D134 File Offset: 0x0002B334
		[Conditional("DEBUG")]
		public static void BeginSample(string name)
		{
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0002D136 File Offset: 0x0002B336
		[Conditional("DEBUG")]
		public static void EndSample()
		{
		}
	}
}
