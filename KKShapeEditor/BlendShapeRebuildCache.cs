using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000004 RID: 4
	internal static class BlendShapeRebuildCache
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002067 File Offset: 0x00000267
		public static int FrameCount
		{
			get
			{
				return BlendShapeRebuildCache._frameCount;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x0000206E File Offset: 0x0000026E
		public static List<BlendShapeRebuildCache.Frame> Frames
		{
			get
			{
				return BlendShapeRebuildCache._frames;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002075 File Offset: 0x00000275
		public static Vector3[] GetZeroNormals(int vertCount)
		{
			if (BlendShapeRebuildCache._zeroNormals == null || BlendShapeRebuildCache._zeroNormals.Length != vertCount)
			{
				BlendShapeRebuildCache._zeroNormals = new Vector3[vertCount];
			}
			return BlendShapeRebuildCache._zeroNormals;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002098 File Offset: 0x00000298
		public static Vector3[] GetZeroTangents(int vertCount)
		{
			if (BlendShapeRebuildCache._zeroTangents == null || BlendShapeRebuildCache._zeroTangents.Length != vertCount)
			{
				BlendShapeRebuildCache._zeroTangents = new Vector3[vertCount];
			}
			return BlendShapeRebuildCache._zeroTangents;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020BC File Offset: 0x000002BC
		public static void Cache(Mesh mesh)
		{
			BlendShapeRebuildCache._frameCount = 0;
			if (mesh == null)
			{
				return;
			}
			int vertexCount = mesh.vertexCount;
			if (vertexCount <= 0)
			{
				return;
			}
			int blendShapeCount = mesh.blendShapeCount;
			for (int i = 0; i < blendShapeCount; i++)
			{
				string blendShapeName = mesh.GetBlendShapeName(i);
				if (blendShapeName != null && !blendShapeName.StartsWith("kkse_"))
				{
					int blendShapeFrameCount = mesh.GetBlendShapeFrameCount(i);
					for (int j = 0; j < blendShapeFrameCount; j++)
					{
						BlendShapeRebuildCache.Frame frame;
						if (BlendShapeRebuildCache._frameCount < BlendShapeRebuildCache._frames.Count)
						{
							frame = BlendShapeRebuildCache._frames[BlendShapeRebuildCache._frameCount];
						}
						else
						{
							frame = new BlendShapeRebuildCache.Frame();
							BlendShapeRebuildCache._frames.Add(frame);
						}
						if (frame.DeltaVertices == null || frame.DeltaVertices.Length != vertexCount)
						{
							frame.DeltaVertices = new Vector3[vertexCount];
						}
						if (frame.DeltaNormals == null || frame.DeltaNormals.Length != vertexCount)
						{
							frame.DeltaNormals = new Vector3[vertexCount];
						}
						if (frame.DeltaTangents == null || frame.DeltaTangents.Length != vertexCount)
						{
							frame.DeltaTangents = new Vector3[vertexCount];
						}
						frame.Name = blendShapeName;
						frame.Weight = mesh.GetBlendShapeFrameWeight(i, j);
						mesh.GetBlendShapeFrameVertices(i, j, frame.DeltaVertices, frame.DeltaNormals, frame.DeltaTangents);
						BlendShapeRebuildCache._frameCount++;
					}
				}
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000221C File Offset: 0x0000041C
		public static void Restore(Mesh mesh)
		{
			if (mesh == null)
			{
				return;
			}
			for (int i = 0; i < BlendShapeRebuildCache._frameCount; i++)
			{
				BlendShapeRebuildCache.Frame frame = BlendShapeRebuildCache._frames[i];
				mesh.AddBlendShapeFrame(frame.Name, frame.Weight, frame.DeltaVertices, frame.DeltaNormals, frame.DeltaTangents);
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002274 File Offset: 0x00000474
		public static void CacheSmrWeights(SkinnedMeshRenderer smr, Mesh mesh)
		{
			BlendShapeRebuildCache._smrWeightCount = 0;
			if (smr == null || mesh == null)
			{
				return;
			}
			int blendShapeCount = mesh.blendShapeCount;
			for (int i = 0; i < blendShapeCount; i++)
			{
				string blendShapeName = mesh.GetBlendShapeName(i);
				if (blendShapeName != null && !blendShapeName.StartsWith("kkse_"))
				{
					float blendShapeWeight = smr.GetBlendShapeWeight(i);
					if (blendShapeWeight <= -0.0001f || blendShapeWeight >= 0.0001f)
					{
						if (BlendShapeRebuildCache._smrWeightCount < BlendShapeRebuildCache._smrWeights.Count)
						{
							BlendShapeRebuildCache._smrWeights[BlendShapeRebuildCache._smrWeightCount] = new BlendShapeRebuildCache.WeightSlot
							{
								Name = blendShapeName,
								Weight = blendShapeWeight
							};
						}
						else
						{
							BlendShapeRebuildCache._smrWeights.Add(new BlendShapeRebuildCache.WeightSlot
							{
								Name = blendShapeName,
								Weight = blendShapeWeight
							});
						}
						BlendShapeRebuildCache._smrWeightCount++;
					}
				}
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002358 File Offset: 0x00000558
		public static void RestoreSmrWeights(SkinnedMeshRenderer smr, Mesh mesh)
		{
			if (smr == null || mesh == null)
			{
				return;
			}
			for (int i = 0; i < BlendShapeRebuildCache._smrWeightCount; i++)
			{
				BlendShapeRebuildCache.WeightSlot weightSlot = BlendShapeRebuildCache._smrWeights[i];
				int blendShapeIndex = mesh.GetBlendShapeIndex(weightSlot.Name);
				if (blendShapeIndex >= 0)
				{
					smr.SetBlendShapeWeight(blendShapeIndex, weightSlot.Weight);
				}
			}
		}

		// Token: 0x04000002 RID: 2
		internal const string KksePrefix = "kkse_";

		// Token: 0x04000003 RID: 3
		private static Vector3[] _zeroNormals;

		// Token: 0x04000004 RID: 4
		private static Vector3[] _zeroTangents;

		// Token: 0x04000005 RID: 5
		private static readonly List<BlendShapeRebuildCache.Frame> _frames = new List<BlendShapeRebuildCache.Frame>();

		// Token: 0x04000006 RID: 6
		private static int _frameCount;

		// Token: 0x04000007 RID: 7
		private static readonly List<BlendShapeRebuildCache.WeightSlot> _smrWeights = new List<BlendShapeRebuildCache.WeightSlot>();

		// Token: 0x04000008 RID: 8
		private static int _smrWeightCount;

		// Token: 0x0200004F RID: 79
		internal class Frame
		{
			// Token: 0x0400044E RID: 1102
			public string Name;

			// Token: 0x0400044F RID: 1103
			public float Weight;

			// Token: 0x04000450 RID: 1104
			public Vector3[] DeltaVertices;

			// Token: 0x04000451 RID: 1105
			public Vector3[] DeltaNormals;

			// Token: 0x04000452 RID: 1106
			public Vector3[] DeltaTangents;
		}

		// Token: 0x02000050 RID: 80
		private struct WeightSlot
		{
			// Token: 0x04000453 RID: 1107
			public string Name;

			// Token: 0x04000454 RID: 1108
			public float Weight;
		}
	}
}
