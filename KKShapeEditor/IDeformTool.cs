using System;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000035 RID: 53
	public interface IDeformTool
	{
		// Token: 0x0600025A RID: 602
		void Apply(ShapeDeformer deformer, DeformLayer layer, BrushResult brushResult, Vector3[] vertices, Vector3[] normals, Camera camera);
	}
}
