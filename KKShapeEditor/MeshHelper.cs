using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000010 RID: 16
	public static class MeshHelper
	{
		// Token: 0x0600007C RID: 124 RVA: 0x00004484 File Offset: 0x00002684
		public static void CloneMeshIfShared(SkinnedMeshRenderer renderer)
		{
			if (renderer == null || renderer.sharedMesh == null)
			{
				return;
			}
			Mesh mesh = MeshHelper.CloneMeshCore(renderer.sharedMesh);
			if (mesh != null)
			{
				renderer.sharedMesh = mesh;
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000044C8 File Offset: 0x000026C8
		public static void CloneMeshIfShared(MeshFilter meshFilter)
		{
			if (meshFilter == null || meshFilter.sharedMesh == null)
			{
				return;
			}
			Mesh mesh = MeshHelper.CloneMeshCore(meshFilter.sharedMesh);
			if (mesh != null)
			{
				meshFilter.sharedMesh = mesh;
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000450C File Offset: 0x0000270C
		private static Mesh CloneMeshCore(Mesh originalMesh)
		{
			int instanceID = originalMesh.GetInstanceID();
			if (MeshHelper._clonedMeshIds.Contains(instanceID))
			{
				return null;
			}
			Mesh mesh = UnityEngine.Object.Instantiate<Mesh>(originalMesh);
			mesh.name = originalMesh.name + "_kkse";
			int instanceID2 = mesh.GetInstanceID();
			MeshHelper._clonedMeshIds.Add(instanceID2);
			MeshHelper._originalMeshes[instanceID2] = originalMesh;
			return mesh;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000456C File Offset: 0x0000276C
		public static bool HasOriginal(Renderer r)
		{
			Mesh mesh = MeshHelper.GetMesh(r);
			return mesh != null && MeshHelper._originalMeshes.ContainsKey(mesh.GetInstanceID());
		}

		// Token: 0x06000080 RID: 128 RVA: 0x0000459B File Offset: 0x0000279B
		public static void MarkAsAlreadyCloned(Mesh mesh)
		{
			if (mesh == null)
			{
				return;
			}
			MeshHelper._clonedMeshIds.Add(mesh.GetInstanceID());
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000045B8 File Offset: 0x000027B8
		public static bool RestoreOriginal(SkinnedMeshRenderer smr)
		{
			if (smr == null || smr.sharedMesh == null)
			{
				return false;
			}
			Mesh mesh;
			if (!MeshHelper.RestoreOriginalCore(smr.sharedMesh, out mesh))
			{
				return false;
			}
			smr.sharedMesh = mesh;
			return true;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000045F8 File Offset: 0x000027F8
		public static bool RestoreOriginal(MeshFilter mf)
		{
			if (mf == null || mf.sharedMesh == null)
			{
				return false;
			}
			Mesh mesh;
			if (!MeshHelper.RestoreOriginalCore(mf.sharedMesh, out mesh))
			{
				return false;
			}
			mf.sharedMesh = mesh;
			return true;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00004638 File Offset: 0x00002838
		public static bool RestoreOriginal(Renderer renderer)
		{
			if (renderer == null)
			{
				return false;
			}
			SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
			if (skinnedMeshRenderer != null)
			{
				return MeshHelper.RestoreOriginal(skinnedMeshRenderer);
			}
			MeshFilter component = renderer.GetComponent<MeshFilter>();
			return component != null && MeshHelper.RestoreOriginal(component);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00004680 File Offset: 0x00002880
		private static bool RestoreOriginalCore(Mesh cloned, out Mesh original)
		{
			int instanceID = cloned.GetInstanceID();
			if (!MeshHelper._originalMeshes.TryGetValue(instanceID, out original))
			{
				return false;
			}
			MeshHelper._clonedMeshIds.Remove(instanceID);
			MeshHelper._originalMeshes.Remove(instanceID);
			MeshHelper._subdivisionLevels.Remove(instanceID);
			MeshHelper._subdivisionFaces.Remove(instanceID);
			MeshHelper._subdivisionSmooth.Remove(instanceID);
			UnityEngine.Object.DestroyImmediate(cloned);
			return true;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000046E8 File Offset: 0x000028E8
		public static void PurgeDestroyed()
		{
			List<int> list = null;
			foreach (KeyValuePair<int, Mesh> keyValuePair in MeshHelper._originalMeshes)
			{
				if (keyValuePair.Value == null || !MeshHelper._clonedMeshIds.Contains(keyValuePair.Key))
				{
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(keyValuePair.Key);
				}
			}
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				int num = list[i];
				MeshHelper._clonedMeshIds.Remove(num);
				MeshHelper._originalMeshes.Remove(num);
				MeshHelper._subdivisionLevels.Remove(num);
				MeshHelper._subdivisionFaces.Remove(num);
				MeshHelper._subdivisionSmooth.Remove(num);
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000047CC File Offset: 0x000029CC
		public static Mesh GetMesh(Renderer r)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = r as SkinnedMeshRenderer;
			if (skinnedMeshRenderer != null)
			{
				return skinnedMeshRenderer.sharedMesh;
			}
			MeshFilter component = r.GetComponent<MeshFilter>();
			if (!(component != null))
			{
				return null;
			}
			return component.sharedMesh;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004808 File Offset: 0x00002A08
		public static void CloneMeshIfShared(Renderer renderer)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
			if (skinnedMeshRenderer != null)
			{
				MeshHelper.CloneMeshIfShared(skinnedMeshRenderer);
				return;
			}
			MeshFilter component = renderer.GetComponent<MeshFilter>();
			if (component != null)
			{
				MeshHelper.CloneMeshIfShared(component);
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004844 File Offset: 0x00002A44
		public static void CloneAndReplaySubdivision(Renderer renderer, List<int[]> facesPerLevel)
		{
			if (renderer == null)
			{
				return;
			}
			MeshHelper.CloneMeshIfShared(renderer);
			Mesh mesh = MeshHelper.GetMesh(renderer);
			if (mesh != null && facesPerLevel != null && facesPerLevel.Count > 0)
			{
				MeshHelper.SubdivideReplay(mesh, facesPerLevel);
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00004888 File Offset: 0x00002A88
		public static List<SubmeshInfo> GetSubmeshInfo(Mesh mesh)
		{
			List<SubmeshInfo> list = new List<SubmeshInfo>();
			if (mesh == null || !mesh.isReadable)
			{
				return list;
			}
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				int[] triangles = mesh.GetTriangles(i);
				HashSet<int> hashSet = new HashSet<int>();
				for (int j = 0; j < triangles.Length; j++)
				{
					hashSet.Add(triangles[j]);
				}
				list.Add(new SubmeshInfo
				{
					TriangleCount = triangles.Length / 3,
					VertexCount = hashSet.Count
				});
			}
			return list;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004914 File Offset: 0x00002B14
		public static int GetSubdivisionLevel(Renderer r)
		{
			Mesh mesh = MeshHelper.GetMesh(r);
			if (mesh == null)
			{
				return 0;
			}
			int num;
			if (!MeshHelper._subdivisionLevels.TryGetValue(mesh.GetInstanceID(), out num))
			{
				return 0;
			}
			return num;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000494C File Offset: 0x00002B4C
		public static List<int[]> GetSubdivisionFaces(Renderer r)
		{
			Mesh mesh = MeshHelper.GetMesh(r);
			if (mesh == null)
			{
				return null;
			}
			List<int[]> list;
			if (!MeshHelper._subdivisionFaces.TryGetValue(mesh.GetInstanceID(), out list))
			{
				return null;
			}
			return list;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004984 File Offset: 0x00002B84
		public static List<bool> GetSubdivisionSmooth(Renderer r)
		{
			Mesh mesh = MeshHelper.GetMesh(r);
			if (mesh == null)
			{
				return null;
			}
			List<bool> list;
			if (!MeshHelper._subdivisionSmooth.TryGetValue(mesh.GetInstanceID(), out list))
			{
				return null;
			}
			return list;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000049BC File Offset: 0x00002BBC
		public static int GetTotalFaceCount(Mesh mesh)
		{
			if (mesh == null || !mesh.isReadable)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				num += mesh.GetTriangles(i).Length / 3;
			}
			return num;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00004A00 File Offset: 0x00002C00
		public static bool Subdivide(Mesh mesh, int levels = 1)
		{
			List<List<int[]>> list;
			return MeshHelper.Subdivide(mesh, levels, null, false, out list, null);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004A1C File Offset: 0x00002C1C
		public static bool Subdivide(Mesh mesh, int levels, HashSet<int> selectedFaces)
		{
			List<List<int[]>> list;
			return MeshHelper.Subdivide(mesh, levels, selectedFaces, false, out list, null);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004A35 File Offset: 0x00002C35
		public static bool Subdivide(Mesh mesh, int levels, HashSet<int> selectedFaces, out List<List<int[]>> facesIndexMapPerLevel)
		{
			return MeshHelper.Subdivide(mesh, levels, selectedFaces, false, out facesIndexMapPerLevel, null);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00004A44 File Offset: 0x00002C44
		public static bool Subdivide(Mesh mesh, int levels, HashSet<int> selectedFaces, bool smooth, out List<List<int[]>> facesIndexMapPerLevel, Matrix4x4[] skinMatrices = null)
		{
			facesIndexMapPerLevel = new List<List<int[]>>();
			if (mesh == null)
			{
				return false;
			}
			int num = 0;
			int num2 = 0;
			HashSet<int> hashSet;
			List<int[]> list;
			while (num2 < levels && MeshHelper.SubdivideOnce(mesh, selectedFaces, smooth, out hashSet, out list, skinMatrices))
			{
				num++;
				selectedFaces = hashSet;
				facesIndexMapPerLevel.Add(list);
				num2++;
			}
			if (num > 0)
			{
				int instanceID = mesh.GetInstanceID();
				int num3;
				MeshHelper._subdivisionLevels.TryGetValue(instanceID, out num3);
				MeshHelper._subdivisionLevels[instanceID] = num3 + num;
			}
			return num == levels;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00004AC4 File Offset: 0x00002CC4
		public static void AppendSubdivisionFaces(Mesh mesh, int[] faces, int levels = 1)
		{
			if (mesh == null || levels <= 0)
			{
				return;
			}
			int instanceID = mesh.GetInstanceID();
			List<int[]> list;
			if (!MeshHelper._subdivisionFaces.TryGetValue(instanceID, out list))
			{
				list = new List<int[]>();
				MeshHelper._subdivisionFaces[instanceID] = list;
			}
			list.Add(faces);
			for (int i = 1; i < levels; i++)
			{
				list.Add(new int[0]);
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00004B28 File Offset: 0x00002D28
		public static void SetSubdivisionFaces(Mesh mesh, List<int[]> facesList)
		{
			if (mesh == null)
			{
				return;
			}
			int instanceID = mesh.GetInstanceID();
			if (facesList != null && facesList.Count > 0)
			{
				MeshHelper._subdivisionFaces[instanceID] = facesList;
				return;
			}
			MeshHelper._subdivisionFaces.Remove(instanceID);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004B6C File Offset: 0x00002D6C
		public static void AppendSubdivisionSmooth(Mesh mesh, bool smooth, int levels = 1)
		{
			if (mesh == null || levels <= 0)
			{
				return;
			}
			int instanceID = mesh.GetInstanceID();
			List<bool> list;
			if (!MeshHelper._subdivisionSmooth.TryGetValue(instanceID, out list))
			{
				list = new List<bool>();
				MeshHelper._subdivisionSmooth[instanceID] = list;
			}
			for (int i = 0; i < levels; i++)
			{
				list.Add(smooth);
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004BC4 File Offset: 0x00002DC4
		public static void SetSubdivisionSmooth(Mesh mesh, List<bool> smoothList)
		{
			if (mesh == null)
			{
				return;
			}
			int instanceID = mesh.GetInstanceID();
			if (smoothList != null && smoothList.Count > 0)
			{
				MeshHelper._subdivisionSmooth[instanceID] = smoothList;
				return;
			}
			MeshHelper._subdivisionSmooth.Remove(instanceID);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004C07 File Offset: 0x00002E07
		internal static bool SmoothAt(IList<bool> smooth, int level)
		{
			return smooth != null && level < smooth.Count && smooth[level];
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004C20 File Offset: 0x00002E20
		public static bool SubdivideReplay(Mesh mesh, List<int[]> facesPerLevel)
		{
			List<List<int[]>> list;
			return MeshHelper.SubdivideReplay(mesh, facesPerLevel, null, out list, null);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004C38 File Offset: 0x00002E38
		public static bool SubdivideReplay(Mesh mesh, List<int[]> facesPerLevel, out List<List<int[]>> facesIndexMapPerLevel)
		{
			return MeshHelper.SubdivideReplay(mesh, facesPerLevel, null, out facesIndexMapPerLevel, null);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004C44 File Offset: 0x00002E44
		public static bool SubdivideReplay(Mesh mesh, List<int[]> facesPerLevel, List<bool> smoothPerLevel, out List<List<int[]>> facesIndexMapPerLevel, Matrix4x4[] skinMatrices = null)
		{
			facesIndexMapPerLevel = new List<List<int[]>>();
			bool flag;
			try
			{
				if (mesh == null || facesPerLevel == null || facesPerLevel.Count == 0)
				{
					flag = false;
				}
				else
				{
					int num = 0;
					HashSet<int> hashSet = null;
					for (int i = 0; i < facesPerLevel.Count; i++)
					{
						int[] array = facesPerLevel[i];
						HashSet<int> hashSet2;
						if (array != null && array.Length != 0)
						{
							hashSet2 = new HashSet<int>();
							for (int j = 0; j < array.Length; j++)
							{
								hashSet2.Add(array[j]);
							}
						}
						else if (array != null && array.Length == 0)
						{
							hashSet2 = hashSet;
						}
						else
						{
							hashSet2 = null;
						}
						bool flag2 = MeshHelper.SmoothAt(smoothPerLevel, i);
						HashSet<int> hashSet3;
						List<int[]> list;
						if (!MeshHelper.SubdivideOnce(mesh, hashSet2, flag2, out hashSet3, out list, flag2 ? skinMatrices : null))
						{
							break;
						}
						num++;
						hashSet = hashSet3;
						facesIndexMapPerLevel.Add(list);
					}
					if (num > 0)
					{
						int instanceID = mesh.GetInstanceID();
						int num2;
						MeshHelper._subdivisionLevels.TryGetValue(instanceID, out num2);
						MeshHelper._subdivisionLevels[instanceID] = num2 + num;
						MeshHelper._subdivisionFaces[instanceID] = new List<int[]>(facesPerLevel);
						List<bool> list2 = new List<bool>(facesPerLevel.Count);
						for (int k = 0; k < facesPerLevel.Count; k++)
						{
							list2.Add(MeshHelper.SmoothAt(smoothPerLevel, k));
						}
						MeshHelper._subdivisionSmooth[instanceID] = list2;
					}
					flag = num == facesPerLevel.Count;
				}
			}
			finally
			{
			}
			return flag;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004DB4 File Offset: 0x00002FB4
		private static bool SubdivideOnce(Mesh mesh, HashSet<int> selectedFaces, bool smooth, out HashSet<int> outputSelectedFaces, out List<int[]> facesIndexMap, Matrix4x4[] skinMatrices = null)
		{
			outputSelectedFaces = null;
			facesIndexMap = null;
			Vector3[] vertices = mesh.vertices;
			if (vertices.Length == 0)
			{
				return false;
			}
			Vector3[] normals = mesh.normals;
			Vector2[] uv = mesh.uv;
			Vector2[] uv2 = mesh.uv2;
			Vector4[] tangents = mesh.tangents;
			BoneWeight[] boneWeights = mesh.boneWeights;
			Color32[] colors = mesh.colors32;
			Matrix4x4[] bindposes = mesh.bindposes;
			bool flag = normals != null && normals.Length == vertices.Length;
			bool flag2 = uv != null && uv.Length == vertices.Length;
			bool flag3 = uv2 != null && uv2.Length == vertices.Length;
			bool flag4 = tangents != null && tangents.Length == vertices.Length;
			bool flag5 = boneWeights != null && boneWeights.Length == vertices.Length;
			bool flag6 = colors != null && colors.Length == vertices.Length;
			List<Vector3> list = new List<Vector3>(vertices);
			List<Vector3> list2 = (flag ? new List<Vector3>(normals) : null);
			List<Vector2> list3 = (flag2 ? new List<Vector2>(uv) : null);
			List<Vector2> list4 = (flag3 ? new List<Vector2>(uv2) : null);
			List<Vector4> list5 = (flag4 ? new List<Vector4>(tangents) : null);
			List<BoneWeight> list6 = (flag5 ? new List<BoneWeight>(boneWeights) : null);
			List<Color32> list7 = (flag6 ? new List<Color32>(colors) : null);
			int blendShapeCount = mesh.blendShapeCount;
			List<MeshHelper.BlendShapeFrameSnapshot> list8 = null;
			if (blendShapeCount > 0)
			{
				list8 = new List<MeshHelper.BlendShapeFrameSnapshot>();
				for (int i = 0; i < blendShapeCount; i++)
				{
					string blendShapeName = mesh.GetBlendShapeName(i);
					int blendShapeFrameCount = mesh.GetBlendShapeFrameCount(i);
					for (int j = 0; j < blendShapeFrameCount; j++)
					{
						Vector3[] array = new Vector3[vertices.Length];
						Vector3[] array2 = new Vector3[vertices.Length];
						Vector3[] array3 = new Vector3[vertices.Length];
						mesh.GetBlendShapeFrameVertices(i, j, array, array2, array3);
						list8.Add(new MeshHelper.BlendShapeFrameSnapshot
						{
							Name = blendShapeName,
							Weight = mesh.GetBlendShapeFrameWeight(i, j),
							DeltaV = array,
							DeltaN = array2,
							DeltaT = array3
						});
					}
				}
			}
			List<int> list9 = ((list8 != null || skinMatrices != null) ? new List<int>() : null);
			List<int> list10 = ((smooth && ((list8 != null && list8.Count > 0) || skinMatrices != null)) ? new List<int>() : null);
			Dictionary<long, int> dictionary = new Dictionary<long, int>();
			int subMeshCount = mesh.subMeshCount;
			List<int>[] array4 = new List<int>[subMeshCount];
			int[][] array5 = new int[subMeshCount][];
			for (int k = 0; k < subMeshCount; k++)
			{
				array5[k] = mesh.GetTriangles(k);
			}
			Dictionary<long, MeshHelper.EdgeOpposites> dictionary2 = null;
			int[] array6 = null;
			if (smooth)
			{
				array6 = new int[vertices.Length];
				Dictionary<long, int> dictionary3 = new Dictionary<long, int>(vertices.Length);
				for (int l = 0; l < vertices.Length; l++)
				{
					long num = MeshHelper.QuantKey(vertices[l], 10000f);
					int num2;
					if (!dictionary3.TryGetValue(num, out num2))
					{
						num2 = l;
						dictionary3[num] = l;
					}
					array6[l] = num2;
				}
				dictionary2 = new Dictionary<long, MeshHelper.EdgeOpposites>();
				int num3 = 0;
				for (int m = 0; m < subMeshCount; m++)
				{
					int[] array7 = array5[m];
					for (int n = 0; n < array7.Length; n += 3)
					{
						if (selectedFaces == null || selectedFaces.Contains(num3))
						{
							int num4 = array6[array7[n]];
							int num5 = array6[array7[n + 1]];
							int num6 = array6[array7[n + 2]];
							MeshHelper.AccumulateOpposite(dictionary2, num4, num5, num6);
							MeshHelper.AccumulateOpposite(dictionary2, num5, num6, num4);
							MeshHelper.AccumulateOpposite(dictionary2, num6, num4, num5);
						}
						num3++;
					}
				}
			}
			if (selectedFaces != null)
			{
				int num7 = 0;
				for (int num8 = 0; num8 < subMeshCount; num8++)
				{
					int[] array8 = array5[num8];
					for (int num9 = 0; num9 < array8.Length; num9 += 3)
					{
						if (selectedFaces.Contains(num7))
						{
							MeshHelper.GetOrCreateMidpoint(array8[num9], array8[num9 + 1], dictionary, list, list2, list3, list4, list5, list6, list7, vertices, normals, uv, uv2, tangents, boneWeights, colors, flag, flag2, flag3, flag4, flag5, flag6, list9, list10, smooth, dictionary2, array6);
							MeshHelper.GetOrCreateMidpoint(array8[num9 + 1], array8[num9 + 2], dictionary, list, list2, list3, list4, list5, list6, list7, vertices, normals, uv, uv2, tangents, boneWeights, colors, flag, flag2, flag3, flag4, flag5, flag6, list9, list10, smooth, dictionary2, array6);
							MeshHelper.GetOrCreateMidpoint(array8[num9 + 2], array8[num9], dictionary, list, list2, list3, list4, list5, list6, list7, vertices, normals, uv, uv2, tangents, boneWeights, colors, flag, flag2, flag3, flag4, flag5, flag6, list9, list10, smooth, dictionary2, array6);
						}
						num7++;
					}
				}
			}
			if (selectedFaces != null)
			{
				MeshHelper.PropagateSeamMidpoints(mesh, vertices, dictionary, list, list2, list3, list4, list5, list6, list7, normals, uv, uv2, tangents, boneWeights, colors, flag, flag2, flag3, flag4, flag5, flag6, list9, list10, smooth, dictionary2, array6);
			}
			int num10 = 0;
			int num11 = 0;
			HashSet<int> hashSet = ((selectedFaces != null) ? new HashSet<int>() : null);
			List<int[]> list11 = new List<int[]>();
			for (int num12 = 0; num12 < subMeshCount; num12++)
			{
				int[] array9 = array5[num12];
				List<int> list12 = new List<int>(array9.Length * 4);
				for (int num13 = 0; num13 < array9.Length; num13 += 3)
				{
					int num14 = array9[num13];
					int num15 = array9[num13 + 1];
					int num16 = array9[num13 + 2];
					int num17 = num11;
					if (selectedFaces != null && !selectedFaces.Contains(num10))
					{
						long num18 = MeshHelper.MakeEdgeKey(num14, num15);
						long num19 = MeshHelper.MakeEdgeKey(num15, num16);
						long num20 = MeshHelper.MakeEdgeKey(num16, num14);
						int num21;
						bool flag7 = dictionary.TryGetValue(num18, out num21);
						int num22;
						bool flag8 = dictionary.TryGetValue(num19, out num22);
						int num23;
						bool flag9 = dictionary.TryGetValue(num20, out num23);
						int num24;
						if ((flag7 ? 1 : 0) + (flag8 ? 1 : 0) + (flag9 ? 1 : 0) == 0)
						{
							list12.Add(num14);
							list12.Add(num15);
							list12.Add(num16);
							num24 = 1;
							num11++;
						}
						else
						{
							num24 = MeshHelper.EmitBorderTriangles(list12, num14, num15, num16, flag7, flag8, flag9, num21, num22, num23);
							num11 += num24;
						}
						int[] array10 = new int[num24];
						for (int num25 = 0; num25 < num24; num25++)
						{
							array10[num25] = num17 + num25;
						}
						list11.Add(array10);
						num10++;
					}
					else
					{
						int orCreateMidpoint = MeshHelper.GetOrCreateMidpoint(num14, num15, dictionary, list, list2, list3, list4, list5, list6, list7, vertices, normals, uv, uv2, tangents, boneWeights, colors, flag, flag2, flag3, flag4, flag5, flag6, list9, list10, smooth, dictionary2, array6);
						int orCreateMidpoint2 = MeshHelper.GetOrCreateMidpoint(num15, num16, dictionary, list, list2, list3, list4, list5, list6, list7, vertices, normals, uv, uv2, tangents, boneWeights, colors, flag, flag2, flag3, flag4, flag5, flag6, list9, list10, smooth, dictionary2, array6);
						int orCreateMidpoint3 = MeshHelper.GetOrCreateMidpoint(num16, num14, dictionary, list, list2, list3, list4, list5, list6, list7, vertices, normals, uv, uv2, tangents, boneWeights, colors, flag, flag2, flag3, flag4, flag5, flag6, list9, list10, smooth, dictionary2, array6);
						list12.Add(num14);
						list12.Add(orCreateMidpoint);
						list12.Add(orCreateMidpoint3);
						list12.Add(orCreateMidpoint);
						list12.Add(num15);
						list12.Add(orCreateMidpoint2);
						list12.Add(orCreateMidpoint3);
						list12.Add(orCreateMidpoint2);
						list12.Add(num16);
						list12.Add(orCreateMidpoint);
						list12.Add(orCreateMidpoint2);
						list12.Add(orCreateMidpoint3);
						if (hashSet != null)
						{
							hashSet.Add(num11);
							hashSet.Add(num11 + 1);
							hashSet.Add(num11 + 2);
							hashSet.Add(num11 + 3);
						}
						list11.Add(new int[]
						{
							num11,
							num11 + 1,
							num11 + 2,
							num11 + 3
						});
						num10++;
						num11 += 4;
					}
				}
				array4[num12] = list12;
			}
			outputSelectedFaces = hashSet;
			facesIndexMap = list11;
			if (skinMatrices != null && flag5 && list6 != null && list9 != null)
			{
				int num26 = vertices.Length;
				Vector3[] array11 = new Vector3[num26];
				for (int num27 = 0; num27 < num26; num27++)
				{
					array11[num27] = MeshHelper.SkinPoint(num27, boneWeights, vertices, skinMatrices);
				}
				int num28 = list9.Count / 2;
				for (int num29 = 0; num29 < num28; num29++)
				{
					int num30 = list9[2 * num29];
					int num31 = list9[2 * num29 + 1];
					int num32 = num26 + num29;
					if (num32 < list6.Count)
					{
						Matrix4x4 matrix4x = MeshHelper.CombineSkinMatrix(list6[num32], skinMatrices);
						if (Mathf.Abs(matrix4x.determinant) >= 1E-12f)
						{
							Vector3 vector;
							if (list10 != null && list10[6 * num29] >= 0)
							{
								int num33 = list10[6 * num29];
								int num34 = list10[6 * num29 + 1];
								int num35 = list10[6 * num29 + 2];
								int num36 = list10[6 * num29 + 3];
								int num37 = list10[6 * num29 + 4];
								int num38 = list10[6 * num29 + 5];
								vector = MeshHelper.ButterflyCombine(array11[num30], array11[num31], array11[num33], array11[num34], array11[num35], array11[num36], array11[num37], array11[num38]);
							}
							else
							{
								vector = (array11[num30] + array11[num31]) * 0.5f;
							}
							Vector3 vector2 = matrix4x.inverse.MultiplyPoint3x4(vector);
							if (!float.IsNaN(vector2.x) && !float.IsNaN(vector2.y) && !float.IsNaN(vector2.z) && !float.IsInfinity(vector2.x) && !float.IsInfinity(vector2.y) && !float.IsInfinity(vector2.z))
							{
								list[num32] = vector2;
							}
						}
					}
				}
			}
			int count = list.Count;
			bool flag10;
			try
			{
				mesh.Clear();
				if (count > 65535)
				{
					mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
				}
				mesh.vertices = list.ToArray();
				if (list2 != null)
				{
					mesh.normals = list2.ToArray();
				}
				if (list3 != null)
				{
					mesh.uv = list3.ToArray();
				}
				if (list4 != null)
				{
					mesh.uv2 = list4.ToArray();
				}
				if (list5 != null)
				{
					mesh.tangents = list5.ToArray();
				}
				if (list6 != null)
				{
					mesh.boneWeights = list6.ToArray();
				}
				if (list7 != null)
				{
					mesh.colors32 = list7.ToArray();
				}
				if (bindposes != null && bindposes.Length != 0)
				{
					mesh.bindposes = bindposes;
				}
				mesh.subMeshCount = subMeshCount;
				for (int num39 = 0; num39 < subMeshCount; num39++)
				{
					mesh.SetTriangles(array4[num39].ToArray(), num39);
				}
				mesh.RecalculateBounds();
				if (list2 == null)
				{
					mesh.RecalculateNormals();
				}
				if (list8 != null && list8.Count > 0)
				{
					int num40 = vertices.Length;
					int num41 = ((list9 != null) ? (list9.Count / 2) : 0);
					int num42 = num40 + num41;
					for (int num43 = 0; num43 < list8.Count; num43++)
					{
						MeshHelper.BlendShapeFrameSnapshot blendShapeFrameSnapshot = list8[num43];
						Vector3[] array12 = new Vector3[num42];
						Vector3[] array13 = new Vector3[num42];
						Vector3[] array14 = new Vector3[num42];
						Array.Copy(blendShapeFrameSnapshot.DeltaV, array12, num40);
						Array.Copy(blendShapeFrameSnapshot.DeltaN, array13, num40);
						Array.Copy(blendShapeFrameSnapshot.DeltaT, array14, num40);
						for (int num44 = 0; num44 < num41; num44++)
						{
							int num45 = list9[2 * num44];
							int num46 = list9[2 * num44 + 1];
							int num47 = num40 + num44;
							if (list10 != null && list10[6 * num44] >= 0)
							{
								int num48 = list10[6 * num44];
								int num49 = list10[6 * num44 + 1];
								int num50 = list10[6 * num44 + 2];
								int num51 = list10[6 * num44 + 3];
								int num52 = list10[6 * num44 + 4];
								int num53 = list10[6 * num44 + 5];
								array12[num47] = MeshHelper.ButterflyCombine(blendShapeFrameSnapshot.DeltaV[num45], blendShapeFrameSnapshot.DeltaV[num46], blendShapeFrameSnapshot.DeltaV[num48], blendShapeFrameSnapshot.DeltaV[num49], blendShapeFrameSnapshot.DeltaV[num50], blendShapeFrameSnapshot.DeltaV[num51], blendShapeFrameSnapshot.DeltaV[num52], blendShapeFrameSnapshot.DeltaV[num53]);
							}
							else
							{
								array12[num47] = (blendShapeFrameSnapshot.DeltaV[num45] + blendShapeFrameSnapshot.DeltaV[num46]) * 0.5f;
							}
							array13[num47] = (blendShapeFrameSnapshot.DeltaN[num45] + blendShapeFrameSnapshot.DeltaN[num46]) * 0.5f;
							array14[num47] = (blendShapeFrameSnapshot.DeltaT[num45] + blendShapeFrameSnapshot.DeltaT[num46]) * 0.5f;
						}
						mesh.AddBlendShapeFrame(blendShapeFrameSnapshot.Name, blendShapeFrameSnapshot.Weight, array12, array13, array14);
					}
				}
				flag10 = true;
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogError("Subdivide failed during mesh write: " + ex.Message);
				flag10 = false;
			}
			return flag10;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005B14 File Offset: 0x00003D14
		private static int EmitBorderTriangles(List<int> tris, int v0, int v1, int v2, bool has01, bool has12, bool has20, int m01, int m12, int m20)
		{
			int num = (has01 ? 1 : 0) + (has12 ? 1 : 0) + (has20 ? 1 : 0);
			if (num == 3)
			{
				tris.Add(v0);
				tris.Add(m01);
				tris.Add(m20);
				tris.Add(m01);
				tris.Add(v1);
				tris.Add(m12);
				tris.Add(m20);
				tris.Add(m12);
				tris.Add(v2);
				tris.Add(m01);
				tris.Add(m12);
				tris.Add(m20);
				return 4;
			}
			if (num == 1)
			{
				if (has01)
				{
					tris.Add(v0);
					tris.Add(m01);
					tris.Add(v2);
					tris.Add(m01);
					tris.Add(v1);
					tris.Add(v2);
				}
				else if (has12)
				{
					tris.Add(v0);
					tris.Add(v1);
					tris.Add(m12);
					tris.Add(v0);
					tris.Add(m12);
					tris.Add(v2);
				}
				else
				{
					tris.Add(v0);
					tris.Add(v1);
					tris.Add(m20);
					tris.Add(m20);
					tris.Add(v1);
					tris.Add(v2);
				}
				return 2;
			}
			if (has01 && has12)
			{
				tris.Add(v0);
				tris.Add(m01);
				tris.Add(m12);
				tris.Add(v0);
				tris.Add(m12);
				tris.Add(v2);
				tris.Add(m01);
				tris.Add(v1);
				tris.Add(m12);
				return 3;
			}
			if (has12 && has20)
			{
				tris.Add(v0);
				tris.Add(v1);
				tris.Add(m12);
				tris.Add(v0);
				tris.Add(m12);
				tris.Add(m20);
				tris.Add(m12);
				tris.Add(v2);
				tris.Add(m20);
				return 3;
			}
			tris.Add(v0);
			tris.Add(m01);
			tris.Add(m20);
			tris.Add(m01);
			tris.Add(v1);
			tris.Add(v2);
			tris.Add(m01);
			tris.Add(v2);
			tris.Add(m20);
			return 3;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005D10 File Offset: 0x00003F10
		private static long MakeEdgeKey(int a, int b)
		{
			int lo = Math.Min(a, b);
			int hi = Math.Max(a, b);
			return ((long)hi << 32) | (uint)lo;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005D38 File Offset: 0x00003F38
		private static long QuantKey(Vector3 v, float q)
		{
			long num = (long)Mathf.Round(v.x * q) + 100000L;
			long num2 = (long)Mathf.Round(v.y * q) + 100000L;
			long num3 = (long)Mathf.Round(v.z * q) + 100000L;
			if (num < 0L)
			{
				num = 0L;
			}
			else if (num > 262143L)
			{
				num = 262143L;
			}
			if (num2 < 0L)
			{
				num2 = 0L;
			}
			else if (num2 > 262143L)
			{
				num2 = 262143L;
			}
			if (num3 < 0L)
			{
				num3 = 0L;
			}
			else if (num3 > 262143L)
			{
				num3 = 262143L;
			}
			return (num << 36) | (num2 << 18) | num3;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005DE0 File Offset: 0x00003FE0
		private static void PropagateSeamMidpoints(Mesh mesh, Vector3[] verts, Dictionary<long, int> edgeCache, List<Vector3> newVerts, List<Vector3> newNormals, List<Vector2> newUVs, List<Vector2> newUV2s, List<Vector4> newTangents, List<BoneWeight> newBoneWeights, List<Color32> newColors, Vector3[] normals, Vector2[] uvs, Vector2[] uv2s, Vector4[] tangents, BoneWeight[] boneWeights, Color32[] colors, bool hasNormals, bool hasUVs, bool hasUV2s, bool hasTangents, bool hasBoneWeights, bool hasColors, List<int> midpointPairs, List<int> midpointStencil, bool smooth, Dictionary<long, MeshHelper.EdgeOpposites> oppMap, int[] canonical)
		{
			Dictionary<long, List<int>> dictionary = new Dictionary<long, List<int>>();
			for (int i = 0; i < verts.Length; i++)
			{
				long num = MeshHelper.QuantKey(verts[i], 10000f);
				List<int> list;
				if (!dictionary.TryGetValue(num, out list))
				{
					list = new List<int>(2);
					dictionary[num] = list;
				}
				list.Add(i);
			}
			HashSet<long> hashSet = new HashSet<long>();
			int subMeshCount = mesh.subMeshCount;
			for (int j = 0; j < subMeshCount; j++)
			{
				int[] triangles = mesh.GetTriangles(j);
				for (int k = 0; k < triangles.Length; k += 3)
				{
					hashSet.Add(MeshHelper.MakeEdgeKey(triangles[k], triangles[k + 1]));
					hashSet.Add(MeshHelper.MakeEdgeKey(triangles[k + 1], triangles[k + 2]));
					hashSet.Add(MeshHelper.MakeEdgeKey(triangles[k + 2], triangles[k]));
				}
			}
			HashSet<long> hashSet2 = new HashSet<long>();
			List<long> list2 = new List<long>(edgeCache.Keys);
			for (int l = 0; l < list2.Count; l++)
			{
				long num2 = list2[l];
				int num3 = (int)(num2 >> 32);
				int num4 = unchecked((int)(num2 & 0xFFFFFFFFL));
				List<int> list3;
				dictionary.TryGetValue(MeshHelper.QuantKey(verts[num3], 10000f), out list3);
				List<int> list4;
				dictionary.TryGetValue(MeshHelper.QuantKey(verts[num4], 10000f), out list4);
				if (list3 != null && list4 != null && (list3.Count > 1 || list4.Count > 1))
				{
					for (int m = 0; m < list3.Count; m++)
					{
						for (int n = 0; n < list4.Count; n++)
						{
							int num5 = list3[m];
							int num6 = list4[n];
							if (num5 != num6)
							{
								long num7 = MeshHelper.MakeEdgeKey(num5, num6);
								if (num7 != num2 && hashSet.Contains(num7) && !edgeCache.ContainsKey(num7))
								{
									hashSet2.Add(num7);
								}
							}
						}
					}
				}
			}
			foreach (long num8 in hashSet2)
			{
				int num9 = (int)(num8 >> 32);
				int num10 = (int)(uint)num8;
				MeshHelper.GetOrCreateMidpoint(num9, num10, edgeCache, newVerts, newNormals, newUVs, newUV2s, newTangents, newBoneWeights, newColors, verts, normals, uvs, uv2s, tangents, boneWeights, colors, hasNormals, hasUVs, hasUV2s, hasTangents, hasBoneWeights, hasColors, midpointPairs, midpointStencil, smooth, oppMap, canonical);
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006060 File Offset: 0x00004260
		private static void AccumulateOpposite(Dictionary<long, MeshHelper.EdgeOpposites> map, int a, int b, int opp)
		{
			long num = MeshHelper.MakeEdgeKey(a, b);
			MeshHelper.EdgeOpposites edgeOpposites;
			if (!map.TryGetValue(num, out edgeOpposites))
			{
				edgeOpposites.count = 1;
				edgeOpposites.c0 = opp;
				edgeOpposites.c1 = 0;
			}
			else if (edgeOpposites.count == 1)
			{
				edgeOpposites.count = 2;
				edgeOpposites.c1 = opp;
			}
			else
			{
				edgeOpposites.count++;
			}
			map[num] = edgeOpposites;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000060CC File Offset: 0x000042CC
		private static bool TryGetWing(Dictionary<long, MeshHelper.EdgeOpposites> map, int u, int v, int exclude, out int wing)
		{
			wing = 0;
			MeshHelper.EdgeOpposites edgeOpposites;
			if (!map.TryGetValue(MeshHelper.MakeEdgeKey(u, v), out edgeOpposites) || edgeOpposites.count != 2)
			{
				return false;
			}
			if (edgeOpposites.c0 == exclude)
			{
				wing = edgeOpposites.c1;
				return true;
			}
			if (edgeOpposites.c1 == exclude)
			{
				wing = edgeOpposites.c0;
				return true;
			}
			return false;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006124 File Offset: 0x00004324
		private static int GetOrCreateMidpoint(int a, int b, Dictionary<long, int> cache, List<Vector3> newVerts, List<Vector3> newNormals, List<Vector2> newUVs, List<Vector2> newUV2s, List<Vector4> newTangents, List<BoneWeight> newBoneWeights, List<Color32> newColors, Vector3[] verts, Vector3[] normals, Vector2[] uvs, Vector2[] uv2s, Vector4[] tangents, BoneWeight[] boneWeights, Color32[] colors, bool hasNormals, bool hasUVs, bool hasUV2s, bool hasTangents, bool hasBoneWeights, bool hasColors, List<int> midpointPairs, List<int> midpointStencil, bool smooth, Dictionary<long, MeshHelper.EdgeOpposites> oppMap, int[] canonical)
		{
			long num = MeshHelper.MakeEdgeKey(a, b);
			int count;
			if (cache.TryGetValue(num, out count))
			{
				return count;
			}
			if (a >= 0 && a < verts.Length && b >= 0 && b < verts.Length)
			{
				count = newVerts.Count;
				MeshHelper.EdgeOpposites edgeOpposites = default(MeshHelper.EdgeOpposites);
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = ((canonical != null) ? canonical[a] : a);
				int num7 = ((canonical != null) ? canonical[b] : b);
				bool flag = smooth && oppMap != null && oppMap.TryGetValue(MeshHelper.MakeEdgeKey(num6, num7), out edgeOpposites) && edgeOpposites.count == 2 && MeshHelper.TryGetWing(oppMap, num6, edgeOpposites.c0, num7, out num2) && MeshHelper.TryGetWing(oppMap, num7, edgeOpposites.c0, num6, out num3) && MeshHelper.TryGetWing(oppMap, num6, edgeOpposites.c1, num7, out num4) && MeshHelper.TryGetWing(oppMap, num7, edgeOpposites.c1, num6, out num5);
				if (flag)
				{
					newVerts.Add(MeshHelper.ButterflyCombine(verts[a], verts[b], verts[edgeOpposites.c0], verts[edgeOpposites.c1], verts[num2], verts[num3], verts[num4], verts[num5]));
				}
				else
				{
					newVerts.Add((verts[a] + verts[b]) * 0.5f);
				}
				if (hasNormals)
				{
					newNormals.Add(((normals[a] + normals[b]) * 0.5f).normalized);
				}
				if (hasUVs)
				{
					newUVs.Add((uvs[a] + uvs[b]) * 0.5f);
				}
				if (hasUV2s)
				{
					newUV2s.Add((uv2s[a] + uv2s[b]) * 0.5f);
				}
				if (hasTangents)
				{
					Vector4 vector = (tangents[a] + tangents[b]) * 0.5f;
					Vector3 normalized = new Vector3(vector.x, vector.y, vector.z).normalized;
					newTangents.Add(new Vector4(normalized.x, normalized.y, normalized.z, tangents[a].w));
				}
				if (hasBoneWeights)
				{
					newBoneWeights.Add(MeshHelper.LerpBoneWeight(boneWeights[a], boneWeights[b]));
				}
				if (hasColors)
				{
					newColors.Add(MeshHelper.LerpColor32(colors[a], colors[b]));
				}
				if (midpointPairs != null)
				{
					midpointPairs.Add(a);
					midpointPairs.Add(b);
				}
				if (midpointStencil != null)
				{
					if (flag)
					{
						midpointStencil.Add(edgeOpposites.c0);
						midpointStencil.Add(edgeOpposites.c1);
						midpointStencil.Add(num2);
						midpointStencil.Add(num3);
						midpointStencil.Add(num4);
						midpointStencil.Add(num5);
					}
					else
					{
						midpointStencil.Add(-1);
						midpointStencil.Add(-1);
						midpointStencil.Add(-1);
						midpointStencil.Add(-1);
						midpointStencil.Add(-1);
						midpointStencil.Add(-1);
					}
				}
				cache[num] = count;
				return count;
			}
			ShapeEditorPlugin.Logger.LogWarning(string.Format("Subdivide: vertex index out of range (a={0}, b={1}, verts={2}), skipping midpoint", a, b, verts.Length));
			if (a < verts.Length && a >= 0)
			{
				return a;
			}
			if (b >= verts.Length || b < 0)
			{
				return 0;
			}
			return b;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000064A8 File Offset: 0x000046A8
		private static Color32 LerpColor32(Color32 a, Color32 b)
		{
			return new Color32(
				(byte)((a.r + b.r) / 2),
				(byte)((a.g + b.g) / 2),
				(byte)((a.b + b.b) / 2),
				(byte)((a.a + b.a) / 2));
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000064FC File Offset: 0x000046FC
		private static BoneWeight LerpBoneWeight(BoneWeight a, BoneWeight b)
		{
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			MeshHelper.AddSlot(dictionary, a.boneIndex0, a.weight0 * 0.5f);
			MeshHelper.AddSlot(dictionary, a.boneIndex1, a.weight1 * 0.5f);
			MeshHelper.AddSlot(dictionary, a.boneIndex2, a.weight2 * 0.5f);
			MeshHelper.AddSlot(dictionary, a.boneIndex3, a.weight3 * 0.5f);
			MeshHelper.AddSlot(dictionary, b.boneIndex0, b.weight0 * 0.5f);
			MeshHelper.AddSlot(dictionary, b.boneIndex1, b.weight1 * 0.5f);
			MeshHelper.AddSlot(dictionary, b.boneIndex2, b.weight2 * 0.5f);
			MeshHelper.AddSlot(dictionary, b.boneIndex3, b.weight3 * 0.5f);
			List<KeyValuePair<int, float>> list = new List<KeyValuePair<int, float>>(dictionary);
			list.Sort((KeyValuePair<int, float> x, KeyValuePair<int, float> y) => y.Value.CompareTo(x.Value));
			BoneWeight boneWeight = default(BoneWeight);
			float num = 0f;
			if (list.Count > 0)
			{
				boneWeight.boneIndex0 = list[0].Key;
				boneWeight.weight0 = list[0].Value;
				num += boneWeight.weight0;
			}
			if (list.Count > 1)
			{
				boneWeight.boneIndex1 = list[1].Key;
				boneWeight.weight1 = list[1].Value;
				num += boneWeight.weight1;
			}
			if (list.Count > 2)
			{
				boneWeight.boneIndex2 = list[2].Key;
				boneWeight.weight2 = list[2].Value;
				num += boneWeight.weight2;
			}
			if (list.Count > 3)
			{
				boneWeight.boneIndex3 = list[3].Key;
				boneWeight.weight3 = list[3].Value;
				num += boneWeight.weight3;
			}
			if (num > 0f)
			{
				float num2 = 1f / num;
				boneWeight.weight0 *= num2;
				boneWeight.weight1 *= num2;
				boneWeight.weight2 *= num2;
				boneWeight.weight3 *= num2;
			}
			return boneWeight;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006768 File Offset: 0x00004968
		private static Vector3 ButterflyCombine(Vector3 a, Vector3 b, Vector3 c0, Vector3 c1, Vector3 e, Vector3 f, Vector3 g, Vector3 h)
		{
			return (a + b) * 0.5f + (c0 + c1) * 0.125f - (e + f + g + h) * 0.0625f;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000067C4 File Offset: 0x000049C4
		private static Matrix4x4 CombineSkinMatrix(BoneWeight w, Matrix4x4[] skin)
		{
			Matrix4x4 matrix4x = default(Matrix4x4);
			MeshHelper.AccumulateSkin(ref matrix4x, skin, w.boneIndex0, w.weight0);
			MeshHelper.AccumulateSkin(ref matrix4x, skin, w.boneIndex1, w.weight1);
			MeshHelper.AccumulateSkin(ref matrix4x, skin, w.boneIndex2, w.weight2);
			MeshHelper.AccumulateSkin(ref matrix4x, skin, w.boneIndex3, w.weight3);
			return matrix4x;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00006834 File Offset: 0x00004A34
		private static void AccumulateSkin(ref Matrix4x4 acc, Matrix4x4[] skin, int idx, float weight)
		{
			if (weight <= 0f || idx < 0 || idx >= skin.Length)
			{
				return;
			}
			Matrix4x4 matrix4x = skin[idx];
			for (int i = 0; i < 16; i++)
			{
				ref Matrix4x4 ptr = ref acc;
				int num = i;
				ptr[num] += matrix4x[i] * weight;
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00006888 File Offset: 0x00004A88
		private static Vector3 SkinPoint(int i, BoneWeight[] bw, Vector3[] v, Matrix4x4[] skin)
		{
			return MeshHelper.CombineSkinMatrix(bw[i], skin).MultiplyPoint3x4(v[i]);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000068B4 File Offset: 0x00004AB4
		public static Matrix4x4[] BuildBodySkinMatrices(Renderer renderer, Mesh mesh)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
			if (skinnedMeshRenderer == null || mesh == null)
			{
				return null;
			}
			if (!MeshHelper.IsCharacterBody(renderer))
			{
				return null;
			}
			Matrix4x4[] bindposes = mesh.bindposes;
			if (bindposes == null || bindposes.Length == 0)
			{
				return null;
			}
			Transform[] bones = skinnedMeshRenderer.bones;
			int num = Mathf.Min((bones != null) ? bones.Length : 0, bindposes.Length);
			if (num <= 0)
			{
				return null;
			}
			Matrix4x4[] array = new Matrix4x4[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = ((bones[i] != null) ? bones[i].localToWorldMatrix : Matrix4x4.identity) * bindposes[i];
			}
			return array;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00006961 File Offset: 0x00004B61
		public static bool IsCharacterBody(Renderer renderer)
		{
			return renderer != null && renderer.name != null && renderer.name.StartsWith("o_body");
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006988 File Offset: 0x00004B88
		private static void AddSlot(Dictionary<int, float> dict, int boneIndex, float weight)
		{
			if (weight < 0.0001f)
			{
				return;
			}
			float num;
			if (dict.TryGetValue(boneIndex, out num))
			{
				dict[boneIndex] = num + weight;
				return;
			}
			dict[boneIndex] = weight;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000069BC File Offset: 0x00004BBC
		public static void TransformFaceMaskThroughSubdivision(HashSet<int> faceMask, List<int[]> facesIndexMap)
		{
			if (faceMask == null || faceMask.Count == 0 || facesIndexMap == null)
			{
				return;
			}
			List<int> list = new List<int>(faceMask);
			faceMask.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				int num = list[i];
				if (num >= 0 && num < facesIndexMap.Count)
				{
					int[] array = facesIndexMap[num];
					if (array != null)
					{
						for (int j = 0; j < array.Length; j++)
						{
							faceMask.Add(array[j]);
						}
					}
				}
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00006A34 File Offset: 0x00004C34
		public static void ResetLayersForNewVertexCount(DeformData deformData, int newVertexCount)
		{
			if (deformData == null)
			{
				return;
			}
			bool flag = false;
			foreach (DeformLayer deformLayer in deformData.Layers)
			{
				if (deformLayer.Deltas != null && deformLayer.Deltas.Length != 0)
				{
					for (int i = 0; i < deformLayer.Deltas.Length; i++)
					{
						if (deformLayer.Deltas[i] != Vector3.zero)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
			foreach (DeformLayer deformLayer2 in deformData.Layers)
			{
				deformLayer2.Deltas = new Vector3[newVertexCount];
				deformLayer2.Dirty = true;
			}
			if (flag)
			{
				ShapeEditorPlugin.Logger.LogWarning("Subdivision changed vertex count — all layer deltas have been reset");
			}
		}

		// Token: 0x04000031 RID: 49
		private static readonly HashSet<int> _clonedMeshIds = new HashSet<int>();

		// Token: 0x04000032 RID: 50
		private static readonly Dictionary<int, Mesh> _originalMeshes = new Dictionary<int, Mesh>();

		// Token: 0x04000033 RID: 51
		private static readonly Dictionary<int, int> _subdivisionLevels = new Dictionary<int, int>();

		// Token: 0x04000034 RID: 52
		private static readonly Dictionary<int, List<int[]>> _subdivisionFaces = new Dictionary<int, List<int[]>>();

		// Token: 0x04000035 RID: 53
		private static readonly Dictionary<int, List<bool>> _subdivisionSmooth = new Dictionary<int, List<bool>>();

		// Token: 0x04000036 RID: 54
		public const string BodyRendererPrefix = "o_body";

		// Token: 0x02000056 RID: 86
		private struct EdgeOpposites
		{
			// Token: 0x0400046B RID: 1131
			public int count;

			// Token: 0x0400046C RID: 1132
			public int c0;

			// Token: 0x0400046D RID: 1133
			public int c1;
		}

		// Token: 0x02000057 RID: 87
		private class BlendShapeFrameSnapshot
		{
			// Token: 0x0400046E RID: 1134
			public string Name;

			// Token: 0x0400046F RID: 1135
			public float Weight;

			// Token: 0x04000470 RID: 1136
			public Vector3[] DeltaV;

			// Token: 0x04000471 RID: 1137
			public Vector3[] DeltaN;

			// Token: 0x04000472 RID: 1138
			public Vector3[] DeltaT;
		}
	}
}
