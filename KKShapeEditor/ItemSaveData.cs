using System;
using System.Collections.Generic;

namespace KKShapeEditor
{
	// Token: 0x0200001F RID: 31
	public class ItemSaveData
	{
		// Token: 0x040000CE RID: 206
		public Dictionary<string, DeformData> DeformDataMap;

		// Token: 0x040000CF RID: 207
		public Dictionary<string, int> SubdividedMeshes;

		// Token: 0x040000D0 RID: 208
		public Dictionary<string, List<int[]>> SubdividedFaces;

		// Token: 0x040000D1 RID: 209
		public Dictionary<string, bool[]> SubdividedSmooth;
	}
}
