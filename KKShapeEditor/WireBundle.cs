using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200004C RID: 76
	internal class WireBundle
	{
		// Token: 0x04000439 RID: 1081
		public Vector3[] Verts;

		// Token: 0x0400043A RID: 1082
		public int[] Tris;

		// Token: 0x0400043B RID: 1083
		public int MeshId;

		// Token: 0x0400043C RID: 1084
		public WireEdge[] Edges;

		// Token: 0x0400043D RID: 1085
		public Mesh LineMesh;

		// Token: 0x0400043E RID: 1086
		public int[] LineIndexBuffer;

		// Token: 0x0400043F RID: 1087
		public int[] VisibleLineIndices;

		// Token: 0x04000440 RID: 1088
		public int PrevVisibleCount = -1;

		// Token: 0x04000441 RID: 1089
		public Color32[] Colors;

		// Token: 0x04000442 RID: 1090
		public bool ColorsDirty = true;

		// Token: 0x04000443 RID: 1091
		public List<int> SelectedFaceVerts;

		// Token: 0x04000444 RID: 1092
		public bool[] TriFacing;

		// Token: 0x04000445 RID: 1093
		public int LastDrawVertsHash;

		// Token: 0x04000446 RID: 1094
		public int LastDrawCamHash;

		// Token: 0x04000447 RID: 1095
		public bool EverDrawn;

		// Token: 0x04000448 RID: 1096
		public bool ColorsUploaded;

		// Token: 0x04000449 RID: 1097
		public int LastRefreshRebakeFrame = -1;

		// Token: 0x0400044A RID: 1098
		public int LastRefreshSig;

		// Token: 0x0400044B RID: 1099
		public bool RefreshValid;
	}
}
