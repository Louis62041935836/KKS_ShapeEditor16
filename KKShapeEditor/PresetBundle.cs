using System;
using System.Collections.Generic;

namespace KKShapeEditor
{
	// Token: 0x02000015 RID: 21
	public class PresetBundle
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x00007628 File Offset: 0x00005828
		public PresetBundle()
		{
			this.Entries = new List<PresetEntry>();
		}

		// Token: 0x0400004C RID: 76
		public int FormatVersion;

		// Token: 0x0400004D RID: 77
		public List<PresetEntry> Entries;
	}
}
