using System;
using KKAPI.Utilities;

namespace KKShapeEditor
{
	// Token: 0x02000022 RID: 34
	internal static class TimelineCompat
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001DA RID: 474 RVA: 0x00010AD2 File Offset: 0x0000ECD2
		// (set) Token: 0x060001DB RID: 475 RVA: 0x00010AD9 File Offset: 0x0000ECD9
		public static bool IsAvailable { get; private set; }

		// Token: 0x060001DC RID: 476 RVA: 0x00010AE4 File Offset: 0x0000ECE4
		public static void Init()
		{
			if (!ShapeEditorPlugin.IsStudio)
			{
				TimelineCompat.IsAvailable = false;
				return;
			}
			try
			{
				TimelineCompat.IsAvailable = TimelineCompatibility.IsTimelineAvailable();
			}
			catch (Exception ex)
			{
				TimelineCompat.IsAvailable = false;
				ShapeEditorPlugin.Logger.LogWarning("Timeline detection failed: " + ex.GetType().Name + ": " + ex.Message);
			}
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00010B50 File Offset: 0x0000ED50
		public static void RefreshList()
		{
			if (!TimelineCompat.IsAvailable)
			{
				return;
			}
			TimelineCompatibility.RefreshInterpolablesList();
		}

		// Token: 0x040000DA RID: 218
		public const string Owner = "KKShapeEditor";
	}
}
