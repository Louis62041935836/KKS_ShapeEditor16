using System;
using System.Collections.Generic;
using UnityEngine;
using AIChara;

namespace KKShapeEditor
{
	// Token: 0x02000006 RID: 6
	internal static class CharacterSwitchCarryover
	{
		// Token: 0x06000014 RID: 20 RVA: 0x0000261C File Offset: 0x0000081C
		public static CharacterSwitchCarryover.Snapshot CaptureSnapshot(ShapeEditorController controller)
		{
			if (controller == null)
			{
				return null;
			}
			if (!ShapeEditorPlugin.IsStudio)
			{
				return null;
			}
			if (!ShapeEditorPlugin.EnableCharacterSwitchCarryover.Value)
			{
				return null;
			}
			ChaControl chaControl = controller.ChaControl;
			if (chaControl == null)
			{
				return null;
			}
			Transform rootTransform = controller.RootTransform;
			if (rootTransform == null)
			{
				return null;
			}
			HashSet<string> hashSet = new HashSet<string>();
			List<Renderer> renderers = ShapeEditorController.CollectCharacterRenderers(chaControl, rootTransform);
			foreach (Renderer renderer in renderers)
			{
				if (renderer != null)
				{
					hashSet.Add(ShapeEditorController.GetRelativePath(rootTransform, renderer.transform));
				}
			}
			if (hashSet.Count == 0)
			{
				return null;
			}
			Dictionary<string, DeformData> allDeformData = controller.GetAllDeformData();
			Dictionary<string, DeformData> dictionary = new Dictionary<string, DeformData>();
			foreach (KeyValuePair<string, DeformData> keyValuePair in allDeformData)
			{
				if (hashSet.Contains(keyValuePair.Key))
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			if (dictionary.Count == 0)
			{
				return null;
			}
			CharacterSwitchCarryover.Snapshot snapshot = new CharacterSwitchCarryover.Snapshot
			{
				DeformDataByPath = dictionary,
				SubLevelsByPath = new Dictionary<string, int>(),
				SubFacesByPath = new Dictionary<string, List<int[]>>(),
				SubSmoothByPath = new Dictionary<string, bool[]>(),
				TriangleHashByPath = new Dictionary<string, int>(),
				VertexCountByPath = new Dictionary<string, int>(),
				Generation = controller.GetReloadGeneration()
			};
			Dictionary<string, int> pendingSubLevels = controller.GetPendingSubLevels();
			Dictionary<string, List<int[]>> pendingSubFaces = controller.GetPendingSubFaces();
			Dictionary<string, bool[]> pendingSubSmooth = controller.GetPendingSubSmooth();
			if (pendingSubLevels != null)
			{
				foreach (KeyValuePair<string, int> keyValuePair2 in pendingSubLevels)
				{
					if (dictionary.ContainsKey(keyValuePair2.Key))
					{
						snapshot.SubLevelsByPath[keyValuePair2.Key] = keyValuePair2.Value;
					}
				}
			}
			if (pendingSubFaces != null)
			{
				foreach (KeyValuePair<string, List<int[]>> keyValuePair3 in pendingSubFaces)
				{
					if (dictionary.ContainsKey(keyValuePair3.Key))
					{
						snapshot.SubFacesByPath[keyValuePair3.Key] = keyValuePair3.Value;
					}
				}
			}
			if (pendingSubSmooth != null)
			{
				foreach (KeyValuePair<string, bool[]> keyValuePair4 in pendingSubSmooth)
				{
					if (dictionary.ContainsKey(keyValuePair4.Key))
					{
						snapshot.SubSmoothByPath[keyValuePair4.Key] = keyValuePair4.Value;
					}
				}
			}
			foreach (string text in dictionary.Keys)
			{
				Renderer renderer = ShapeEditorController.FindRendererByPath(rootTransform, text);
				if (!(renderer == null))
				{
					Mesh mesh = MeshHelper.GetMesh(renderer);
					if (!(mesh == null))
					{
						try
						{
							snapshot.TriangleHashByPath[text] = CharacterSwitchCarryover.ComputeTriangleHash(mesh.triangles);
						}
						catch
						{
						}
						snapshot.VertexCountByPath[text] = mesh.vertexCount;
					}
				}
			}
			return snapshot;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002938 File Offset: 0x00000B38
		public static int ComputeTriangleHash(int[] triangles)
		{
			if (triangles == null)
			{
				return 0;
			}
			int num = 17;
			for (int i = 0; i < triangles.Length; i++)
			{
				num = num * 31 + triangles[i];
			}
			return num;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002968 File Offset: 0x00000B68
		public static bool ValidateTopology(string path, ShapeEditorController newController, CharacterSwitchCarryover.Snapshot snapshot)
		{
			if (snapshot == null || newController == null || string.IsNullOrEmpty(path))
			{
				return false;
			}
			DeformData deformData;
			if (!snapshot.DeformDataByPath.TryGetValue(path, out deformData) || deformData == null)
			{
				return false;
			}
			int num;
			if (deformData.Layers != null && deformData.Layers.Count > 0 && deformData.Layers[0].Deltas != null && deformData.Layers[0].Deltas.Length != 0)
			{
				num = deformData.Layers[0].Deltas.Length;
			}
			else if (deformData.DeletedFaces.Count <= 0 || snapshot.VertexCountByPath == null || !snapshot.VertexCountByPath.TryGetValue(path, out num) || num <= 0)
			{
				return false;
			}
			Transform rootTransform = newController.RootTransform;
			if (rootTransform == null)
			{
				return false;
			}
			Renderer renderer = ShapeEditorController.FindRendererByPath(rootTransform, path);
			if (renderer == null)
			{
				return false;
			}
			Mesh mesh = MeshHelper.GetMesh(renderer);
			if (mesh == null)
			{
				return false;
			}
			if (mesh.vertexCount != num)
			{
				return false;
			}
			int num2;
			snapshot.SubLevelsByPath.TryGetValue(path, out num2);
			if (MeshHelper.GetSubdivisionLevel(renderer) != num2)
			{
				return false;
			}
			int num3;
			if (!snapshot.TriangleHashByPath.TryGetValue(path, out num3))
			{
				return false;
			}
			int num4;
			try
			{
				num4 = CharacterSwitchCarryover.ComputeTriangleHash(mesh.triangles);
			}
			catch
			{
				return false;
			}
			return num4 == num3;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002AC4 File Offset: 0x00000CC4
		public static void ApplySnapshotIfTopologyMatches(ShapeEditorController controller, CharacterSwitchCarryover.Snapshot snapshot)
		{
			if (controller == null || snapshot == null)
			{
				return;
			}
			if (snapshot.Generation > controller.GetReloadGeneration())
			{
				return;
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			foreach (KeyValuePair<string, DeformData> keyValuePair in snapshot.DeformDataByPath)
			{
				string key = keyValuePair.Key;
				if (!CharacterSwitchCarryover.ValidateTopology(key, controller, snapshot))
				{
					list.Add(key);
				}
				else
				{
					controller.GetAllDeformData()[key] = keyValuePair.Value;
					if (keyValuePair.Value.DeletedFaces.Count > 0)
					{
						keyValuePair.Value.DeletedFacesDirty = true;
					}
					int num;
					if (snapshot.SubLevelsByPath.TryGetValue(key, out num))
					{
						controller.SetPendingSubLevel(key, num);
					}
					else
					{
						controller.RemovePendingSubLevel(key);
					}
					List<int[]> list3;
					if (snapshot.SubFacesByPath.TryGetValue(key, out list3))
					{
						controller.SetPendingSubFaces(key, list3);
					}
					else
					{
						controller.RemovePendingSubFaces(key);
					}
					bool[] array;
					if (snapshot.SubSmoothByPath != null && snapshot.SubSmoothByPath.TryGetValue(key, out array))
					{
						controller.SetPendingSubSmooth(key, array);
					}
					else
					{
						controller.RemovePendingSubSmooth(key);
					}
					controller.ReinitDeformerForPath(key);
					list2.Add(key);
				}
			}
			if (list.Count > 0)
			{
				ShapeEditorPlugin.Logger.LogInfo(string.Format(L.CarryoverSkippedPathsFormat, string.Join(", ", list.ToArray())));
			}
			if (list2.Count > 0)
			{
				controller.NotifyDataChanged();
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002C58 File Offset: 0x00000E58
		public static void LogTopologyDiagnostic(ShapeEditorController controller, CharacterSwitchCarryover.Snapshot snapshot, string path)
		{
			if (controller == null || snapshot == null || string.IsNullOrEmpty(path))
			{
				ShapeEditorPlugin.Logger.LogInfo("LogTopologyDiagnostic: null arg or empty path");
				return;
			}
			DeformData deformData;
			snapshot.DeformDataByPath.TryGetValue(path, out deformData);
			int num = ((deformData != null && deformData.Layers != null && deformData.Layers.Count > 0 && deformData.Layers[0].Deltas != null) ? deformData.Layers[0].Deltas.Length : (-1));
			int num2;
			snapshot.SubLevelsByPath.TryGetValue(path, out num2);
			int num3;
			snapshot.TriangleHashByPath.TryGetValue(path, out num3);
			int num4 = -1;
			int num5 = -1;
			string text = "<unread>";
			Transform rootTransform = controller.RootTransform;
			if (rootTransform != null)
			{
				Renderer renderer = ShapeEditorController.FindRendererByPath(rootTransform, path);
				if (renderer != null)
				{
					Mesh mesh = MeshHelper.GetMesh(renderer);
					if (mesh != null)
					{
						num4 = mesh.vertexCount;
						num5 = MeshHelper.GetSubdivisionLevel(renderer);
						try
						{
							text = CharacterSwitchCarryover.ComputeTriangleHash(mesh.triangles).ToString();
						}
						catch (Exception ex)
						{
							text = "<exception: " + ex.GetType().Name + ">";
						}
					}
				}
			}
			ShapeEditorPlugin.Logger.LogInfo(string.Format("LogTopologyDiagnostic path={0} | snap: verts={1} level={2} hash={3} | new: verts={4} level={5} hash={6}", new object[] { path, num, num2, num3, num4, num5, text }));
		}

		// Token: 0x02000052 RID: 82
		internal sealed class Snapshot
		{
			// Token: 0x0400045C RID: 1116
			public Dictionary<string, DeformData> DeformDataByPath;

			// Token: 0x0400045D RID: 1117
			public Dictionary<string, int> SubLevelsByPath;

			// Token: 0x0400045E RID: 1118
			public Dictionary<string, List<int[]>> SubFacesByPath;

			// Token: 0x0400045F RID: 1119
			public Dictionary<string, bool[]> SubSmoothByPath;

			// Token: 0x04000460 RID: 1120
			public Dictionary<string, int> TriangleHashByPath;

			// Token: 0x04000461 RID: 1121
			public Dictionary<string, int> VertexCountByPath;

			// Token: 0x04000462 RID: 1122
			public int Generation;
		}
	}
}
