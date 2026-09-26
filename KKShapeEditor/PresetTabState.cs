using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000045 RID: 69
	public class PresetTabState
	{
		// Token: 0x06000346 RID: 838 RVA: 0x0001E258 File Offset: 0x0001C458
		public void ResetForOwnerSwitch()
		{
			this.Mode = PresetTabState.TabMode.Idle;
			this.RowChecked.Clear();
			this.LoadedBundle = null;
			this.Scroll = Vector2.zero;
			this.PreserveExistingLayers = false;
			this.PreserveCoordinateScope = true;
		}

		// Token: 0x040002FA RID: 762
		public PresetTabState.TabMode Mode;

		// Token: 0x040002FB RID: 763
		public Dictionary<int, bool> RowChecked = new Dictionary<int, bool>();

		// Token: 0x040002FC RID: 764
		public PresetBundle LoadedBundle;

		// Token: 0x040002FD RID: 765
		public bool PreserveExistingLayers;

		// Token: 0x040002FE RID: 766
		public bool PreserveCoordinateScope = true;

		// Token: 0x040002FF RID: 767
		public Vector2 Scroll;

		// Token: 0x02000074 RID: 116
		public enum TabMode
		{
			// Token: 0x040004F5 RID: 1269
			Idle,
			// Token: 0x040004F6 RID: 1270
			ImportPreview
		}
	}
}
