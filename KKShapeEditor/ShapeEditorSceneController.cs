using System;
using System.Collections;
using System.Collections.Generic;
using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200001E RID: 30
	public class ShapeEditorSceneController : SceneCustomFunctionController
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x0000E32C File Offset: 0x0000C52C
		protected override void OnSceneSave()
		{
			Dictionary<int, ItemSaveData> dictionary = new Dictionary<int, ItemSaveData>();
			Dictionary<string, ItemSaveData> dictionary2 = new Dictionary<string, ItemSaveData>();
			global::Studio.Studio instance = Singleton<global::Studio.Studio>.Instance;
			if (instance == null)
			{
				return;
			}
			foreach (KeyValuePair<int, ObjectCtrlInfo> keyValuePair in instance.dicObjectCtrl)
			{
				OCIItem ociitem = keyValuePair.Value as OCIItem;
				if (ociitem != null && !(ociitem.objectItem == null))
				{
					ItemShapeController component = ociitem.objectItem.GetComponent<ItemShapeController>();
					if (!(component == null))
					{
						Dictionary<string, DeformData> allDeformData = component.GetAllDeformData();
						bool flag = false;
						bool flag2 = false;
						foreach (DeformData deformData in allDeformData.Values)
						{
							if (deformData.HasLayers)
							{
								flag = true;
							}
							if (deformData.DeletedFaces.Count > 0)
							{
								flag2 = true;
							}
							if (flag && flag2)
							{
								break;
							}
						}
						Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
						Dictionary<string, List<int[]>> dictionary4 = new Dictionary<string, List<int[]>>();
						Dictionary<string, bool[]> dictionary5 = new Dictionary<string, bool[]>();
						foreach (Renderer renderer in component.GetAllRenderers())
						{
							int subdivisionLevel = MeshHelper.GetSubdivisionLevel(renderer);
							if (subdivisionLevel > 0)
							{
								string relativePath = ShapeEditorController.GetRelativePath(component.RootTransform, renderer.transform);
								dictionary3[relativePath] = subdivisionLevel;
								List<int[]> subdivisionFaces = MeshHelper.GetSubdivisionFaces(renderer);
								if (subdivisionFaces != null)
								{
									dictionary4[relativePath] = subdivisionFaces;
								}
								List<bool> subdivisionSmooth = MeshHelper.GetSubdivisionSmooth(renderer);
								if (subdivisionSmooth != null && subdivisionSmooth.Count > 0)
								{
									dictionary5[relativePath] = subdivisionSmooth.ToArray();
								}
							}
						}
						if (flag || flag2 || dictionary3.Count != 0)
						{
							ItemSaveData itemSaveData = new ItemSaveData
							{
								DeformDataMap = ((flag || flag2) ? allDeformData : null),
								SubdividedMeshes = ((dictionary3.Count > 0) ? dictionary3 : null),
								SubdividedFaces = ((dictionary4.Count > 0) ? dictionary4 : null),
								SubdividedSmooth = ((dictionary5.Count > 0) ? dictionary5 : null)
							};
							if (ociitem.treeNodeObject != null && !ociitem.treeNodeObject.enableCopy)
							{
								string hierarchyPath = ShapeEditorSceneController.GetHierarchyPath(ociitem.objectItem.transform);
								dictionary2[hierarchyPath] = itemSaveData;
							}
							else
							{
								dictionary[keyValuePair.Key] = itemSaveData;
							}
						}
					}
				}
			}
			PluginData pluginData = new PluginData();
			if (dictionary.Count > 0)
			{
				pluginData.data["items"] = ShapeSerializer.SerializeItemDict(dictionary);
			}
			if (dictionary2.Count > 0)
			{
				pluginData.data["mapItems"] = ShapeSerializer.SerializeMapItemDict(dictionary2);
			}
			base.SetExtendedData((pluginData.data.Count > 0) ? pluginData : null);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000E64C File Offset: 0x0000C84C
		protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
		{
			if ((int)operation == 2)
			{
				ItemShapeController[] array = UnityEngine.Object.FindObjectsOfType<ItemShapeController>();
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
				MeshHelper.PurgeDestroyed();
			}
			if ((int)operation == 2)
			{
				return;
			}
			PluginData extendedData = base.GetExtendedData();
			if (extendedData == null)
			{
				return;
			}
			object obj;
			if (extendedData.data.TryGetValue("items", out obj))
			{
				byte[] array2 = obj as byte[];
				if (array2 != null)
				{
					Dictionary<int, ItemSaveData> dictionary = ShapeSerializer.DeserializeItemDict(array2);
					if (dictionary != null)
					{
						foreach (KeyValuePair<int, ItemSaveData> keyValuePair in dictionary)
						{
							ObjectCtrlInfo objectCtrlInfo;
							if (loadedItems.TryGetValue(keyValuePair.Key, out objectCtrlInfo))
							{
								ShapeEditorSceneController.RestoreItem(objectCtrlInfo as OCIItem, keyValuePair.Value);
							}
						}
					}
				}
			}
			object obj2;
			if (extendedData.data.TryGetValue("mapItems", out obj2))
			{
				byte[] array3 = obj2 as byte[];
				if (array3 != null)
				{
					Dictionary<string, ItemSaveData> dictionary2 = ShapeSerializer.DeserializeMapItemDict(array3);
					if (dictionary2 != null && dictionary2.Count > 0)
					{
						base.StartCoroutine(this.RestoreMapItemsDelayed(dictionary2));
					}
				}
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000E76C File Offset: 0x0000C96C
		private static void RestoreItem(OCIItem ociItem, ItemSaveData saveData)
		{
			if (ociItem == null || ociItem.objectItem == null || saveData == null)
			{
				return;
			}
			ItemShapeController itemShapeController = ociItem.objectItem.GetComponent<ItemShapeController>();
			if (itemShapeController == null)
			{
				itemShapeController = ociItem.objectItem.AddComponent<ItemShapeController>();
			}
			if (saveData.SubdividedMeshes != null && saveData.SubdividedMeshes.Count > 0)
			{
				Dictionary<string, List<int[]>> dictionary = saveData.SubdividedFaces ?? new Dictionary<string, List<int[]>>();
				Dictionary<string, bool[]> dictionary2 = saveData.SubdividedSmooth ?? new Dictionary<string, bool[]>();
				HashSet<int> hashSet = new HashSet<int>();
				foreach (KeyValuePair<string, int> keyValuePair in saveData.SubdividedMeshes)
				{
					Renderer renderer = ShapeEditorController.FindRendererByPath(itemShapeController.RootTransform, keyValuePair.Key);
					if (!(renderer == null))
					{
						MeshHelper.CloneMeshIfShared(renderer);
						Mesh mesh = MeshHelper.GetMesh(renderer);
						if (!(mesh == null))
						{
							int instanceID = mesh.GetInstanceID();
							if (!hashSet.Contains(instanceID))
							{
								List<int[]> list;
								dictionary.TryGetValue(keyValuePair.Key, out list);
								if (list != null && list.Count > 0)
								{
									bool[] array;
									dictionary2.TryGetValue(keyValuePair.Key, out array);
									List<bool> list2 = ((array != null) ? new List<bool>(array) : null);
									List<List<int[]>> list3;
									MeshHelper.SubdivideReplay(mesh, list, list2, out list3, null);
									DeformData deformData;
									if (saveData.DeformDataMap != null && saveData.DeformDataMap.TryGetValue(keyValuePair.Key, out deformData) && deformData.DeletedFaces.Count > 0)
									{
										deformData.DeletedFacesDirty = true;
									}
								}
								hashSet.Add(mesh.GetInstanceID());
							}
						}
					}
				}
			}
			itemShapeController.LoadData(saveData.DeformDataMap);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000E934 File Offset: 0x0000CB34
		private IEnumerator RestoreMapItemsDelayed(Dictionary<string, ItemSaveData> mapItemData)
		{
			yield return null;
			
			if (mapItemData == null || mapItemData.Count == 0)
			{
				yield break;
			}
			
			global::Studio.Studio instance = Singleton<global::Studio.Studio>.Instance;
			if (instance == null)
			{
				yield break;
			}
			
			foreach (KeyValuePair<string, ItemSaveData> kvp in mapItemData)
			{
				foreach (KeyValuePair<int, ObjectCtrlInfo> itemKvp in instance.dicObjectCtrl)
				{
					OCIItem ociItem = itemKvp.Value as OCIItem;
					if (ociItem != null && ociItem.objectItem != null)
					{
						string hierarchyPath = ShapeEditorSceneController.GetHierarchyPath(ociItem.objectItem.transform);
						if (hierarchyPath == kvp.Key)
						{
							ShapeEditorSceneController.RestoreItem(ociItem, kvp.Value);
							break;
						}
					}
				}
				yield return null;
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000E944 File Offset: 0x0000CB44
		private static string GetHierarchyPath(Transform t)
		{
			List<string> list = new List<string>();
			while (t != null)
			{
				list.Add(t.name);
				t = t.parent;
			}
			list.Reverse();
			return string.Join("/", list.ToArray());
		}
	}
}
