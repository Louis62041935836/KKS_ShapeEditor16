using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000031 RID: 49
	public static class WeightRemapper
	{
		// Token: 0x0600021A RID: 538 RVA: 0x00011E1C File Offset: 0x0001001C
		public static BoneWeight[] ComputeRemappedWeights(Vector3[] clothBindVerts, Vector3[] clothDeltas, Transform[] clothBones, Vector3[] bodyVerts, BoneWeight[] bodyWeights, BoneWeight[] clothOrigWeights, Transform[] bodyBones, int[] bodyTriangles)
		{
			if (clothBindVerts == null || clothDeltas == null || clothBones == null || bodyVerts == null || bodyWeights == null || bodyBones == null)
			{
				return null;
			}
			if (clothBindVerts.Length != clothDeltas.Length)
			{
				return null;
			}
			Dictionary<int, int> dictionary = WeightRemapper.BuildBoneIndexMap(bodyBones, clothBones);
			Bounds bounds = WeightRemapper.ComputeBounds(bodyVerts);
			SpatialHashGrid spatialHashGrid = new SpatialHashGrid(bodyVerts, bounds);
			float num = WeightRemapper.ComputeAverageEdgeLength(bodyVerts, bodyTriangles) * 3f;
			int num2 = clothBindVerts.Length;
			BoneWeight[] array = new BoneWeight[num2];
			int[] array2 = new int[4];
			float[] array3 = new float[4];
			BoneWeight[] array4 = new BoneWeight[4];
			for (int i = 0; i < num2; i++)
			{
				Vector3 vector = clothBindVerts[i] + clothDeltas[i];
				int num3 = WeightRemapper.FindKNearest(spatialHashGrid, vector, num, array2, array3);
				if (num3 == 0)
				{
					if (clothOrigWeights != null && i < clothOrigWeights.Length)
					{
						array[i] = clothOrigWeights[i];
					}
				}
				else
				{
					for (int j = 0; j < num3; j++)
					{
						array4[j] = WeightRemapper.RemapBoneWeight(bodyWeights[array2[j]], dictionary);
					}
					array[i] = WeightRemapper.BlendBoneWeights(array4, array3, num3);
				}
			}
			return array;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00011F3C File Offset: 0x0001013C
		private static Dictionary<int, int> BuildBoneIndexMap(Transform[] bodyBones, Transform[] clothBones)
		{
			Dictionary<Transform, int> dictionary = new Dictionary<Transform, int>(clothBones.Length);
			for (int i = 0; i < clothBones.Length; i++)
			{
				if (clothBones[i] != null)
				{
					dictionary[clothBones[i]] = i;
				}
			}
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>(bodyBones.Length);
			for (int j = 0; j < bodyBones.Length; j++)
			{
				int num;
				if (bodyBones[j] != null && dictionary.TryGetValue(bodyBones[j], out num))
				{
					dictionary2[j] = num;
				}
			}
			return dictionary2;
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00011FB0 File Offset: 0x000101B0
		private static BoneWeight BlendBoneWeights(BoneWeight[] weights, float[] distSq, int count)
		{
			if (count == 1)
			{
				return weights[0];
			}
			float[] array = new float[count];
			float num = 0f;
			for (int i = 0; i < count; i++)
			{
				float num2 = Mathf.Sqrt(distSq[i]);
				if (num2 < 1E-08f)
				{
					return weights[i];
				}
				array[i] = 1f / num2;
				num += array[i];
			}
			float num3 = 1f / num;
			for (int j = 0; j < count; j++)
			{
				array[j] *= num3;
			}
			Dictionary<int, float> dictionary = new Dictionary<int, float>(count * 4);
			for (int k = 0; k < count; k++)
			{
				float num4 = array[k];
				BoneWeight boneWeight = weights[k];
				if (boneWeight.weight0 > 0f)
				{
					WeightRemapper.AddWeight(dictionary, boneWeight.boneIndex0, boneWeight.weight0 * num4);
				}
				if (boneWeight.weight1 > 0f)
				{
					WeightRemapper.AddWeight(dictionary, boneWeight.boneIndex1, boneWeight.weight1 * num4);
				}
				if (boneWeight.weight2 > 0f)
				{
					WeightRemapper.AddWeight(dictionary, boneWeight.boneIndex2, boneWeight.weight2 * num4);
				}
				if (boneWeight.weight3 > 0f)
				{
					WeightRemapper.AddWeight(dictionary, boneWeight.boneIndex3, boneWeight.weight3 * num4);
				}
			}
			int[] array2 = new int[4];
			float[] array3 = new float[4];
			foreach (KeyValuePair<int, float> keyValuePair in dictionary)
			{
				int num5 = -1;
				float num6 = keyValuePair.Value;
				for (int l = 0; l < 4; l++)
				{
					if (array3[l] < num6)
					{
						num6 = array3[l];
						num5 = l;
					}
				}
				if (num5 >= 0)
				{
					array2[num5] = keyValuePair.Key;
					array3[num5] = keyValuePair.Value;
				}
			}
			float num7 = array3[0] + array3[1] + array3[2] + array3[3];
			if (num7 > 0f)
			{
				float num8 = 1f / num7;
				array3[0] *= num8;
				array3[1] *= num8;
				array3[2] *= num8;
				array3[3] *= num8;
			}
			BoneWeight boneWeight2 = default(BoneWeight);
			boneWeight2.boneIndex0 = array2[0];
			boneWeight2.weight0 = array3[0];
			boneWeight2.boneIndex1 = array2[1];
			boneWeight2.weight1 = array3[1];
			boneWeight2.boneIndex2 = array2[2];
			boneWeight2.weight2 = array3[2];
			boneWeight2.boneIndex3 = array2[3];
			boneWeight2.weight3 = array3[3];
			return boneWeight2;
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0001225C File Offset: 0x0001045C
		private static void AddWeight(Dictionary<int, float> accum, int boneIdx, float weight)
		{
			float num;
			if (accum.TryGetValue(boneIdx, out num))
			{
				accum[boneIdx] = num + weight;
				return;
			}
			accum[boneIdx] = weight;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00012288 File Offset: 0x00010488
		private static BoneWeight RemapBoneWeight(BoneWeight bodyBw, Dictionary<int, int> boneMap)
		{
			int[] array = new int[4];
			float[] array2 = new float[4];
			float num = 0f;
			int num2 = 0;
			float num3 = bodyBw.weight0;
			int num4;
			if (num3 > 0f && boneMap.TryGetValue(bodyBw.boneIndex0, out num4))
			{
				array[num2] = num4;
				array2[num2] = num3;
				num += num3;
				num2++;
			}
			num3 = bodyBw.weight1;
			int num5;
			if (num3 > 0f && boneMap.TryGetValue(bodyBw.boneIndex1, out num5))
			{
				array[num2] = num5;
				array2[num2] = num3;
				num += num3;
				num2++;
			}
			num3 = bodyBw.weight2;
			int num6;
			if (num3 > 0f && boneMap.TryGetValue(bodyBw.boneIndex2, out num6))
			{
				array[num2] = num6;
				array2[num2] = num3;
				num += num3;
				num2++;
			}
			num3 = bodyBw.weight3;
			int num7;
			if (num3 > 0f && boneMap.TryGetValue(bodyBw.boneIndex3, out num7))
			{
				array[num2] = num7;
				array2[num2] = num3;
				num += num3;
				num2++;
			}
			if (num2 == 0)
			{
				return default(BoneWeight);
			}
			float num8 = ((num > 0f) ? (1f / num) : 0f);
			BoneWeight boneWeight = default(BoneWeight);
			if (num2 > 0)
			{
				boneWeight.boneIndex0 = array[0];
				boneWeight.weight0 = array2[0] * num8;
			}
			if (num2 > 1)
			{
				boneWeight.boneIndex1 = array[1];
				boneWeight.weight1 = array2[1] * num8;
			}
			if (num2 > 2)
			{
				boneWeight.boneIndex2 = array[2];
				boneWeight.weight2 = array2[2] * num8;
			}
			if (num2 > 3)
			{
				boneWeight.boneIndex3 = array[3];
				boneWeight.weight3 = array2[3] * num8;
			}
			return boneWeight;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00012420 File Offset: 0x00010620
		private static int FindKNearest(SpatialHashGrid grid, Vector3 queryPos, float radius, int[] outIndices, float[] outDistSq)
		{
			int count = 0;
			grid.FindVerticesInRadius(queryPos, radius, delegate(int idx, float dSq)
			{
				int num = ((count < 4) ? count : 4);
				int num2 = 0;
				while (num2 < count && num2 < 4)
				{
					if (dSq < outDistSq[num2])
					{
						num = num2;
						break;
					}
					num2++;
				}
				if (num >= 4)
				{
					return;
				}
				for (int i = ((count < 4) ? count : 3); i > num; i--)
				{
					outIndices[i] = outIndices[i - 1];
					outDistSq[i] = outDistSq[i - 1];
				}
				outIndices[num] = idx;
				outDistSq[num] = dSq;
				if (count < 4)
				{
					int count2 = count;
					count = count2 + 1;
				}
			});
			return count;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00012464 File Offset: 0x00010664
		private static float ComputeAverageEdgeLength(Vector3[] vertices, int[] triangles)
		{
			if (vertices == null || triangles == null || triangles.Length < 3)
			{
				return 0.1f;
			}
			double num = 0.0;
			int num2 = 0;
			for (int i = 0; i < triangles.Length; i += 3)
			{
				int num3 = triangles[i];
				int num4 = triangles[i + 1];
				int num5 = triangles[i + 2];
				num += (double)Vector3.Distance(vertices[num3], vertices[num4]);
				num += (double)Vector3.Distance(vertices[num4], vertices[num5]);
				num += (double)Vector3.Distance(vertices[num5], vertices[num3]);
				num2 += 3;
			}
			if (num2 <= 0)
			{
				return 0.1f;
			}
			return (float)(num / (double)num2);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0001250C File Offset: 0x0001070C
		private static Bounds ComputeBounds(Vector3[] vertices)
		{
			if (vertices == null || vertices.Length == 0)
			{
				return new Bounds(Vector3.zero, Vector3.zero);
			}
			Vector3 vector = vertices[0];
			Vector3 vector2 = vertices[0];
			for (int i = 1; i < vertices.Length; i++)
			{
				Vector3 vector3 = vertices[i];
				if (vector3.x < vector.x)
				{
					vector.x = vector3.x;
				}
				if (vector3.y < vector.y)
				{
					vector.y = vector3.y;
				}
				if (vector3.z < vector.z)
				{
					vector.z = vector3.z;
				}
				if (vector3.x > vector2.x)
				{
					vector2.x = vector3.x;
				}
				if (vector3.y > vector2.y)
				{
					vector2.y = vector3.y;
				}
				if (vector3.z > vector2.z)
				{
					vector2.z = vector3.z;
				}
			}
			Bounds bounds = default(Bounds);
			bounds.SetMinMax(vector, vector2);
			return bounds;
		}

		// Token: 0x04000104 RID: 260
		private const int K = 4;
	}
}
