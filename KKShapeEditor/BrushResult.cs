using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000034 RID: 52
	public class BrushResult
	{
		// Token: 0x04000125 RID: 293
		public Vector3 HitPoint;

		// Token: 0x04000126 RID: 294
		public Vector3 HitNormal;

		// Token: 0x04000127 RID: 295
		public Dictionary<int, float> AffectedVertices;
	}
}
