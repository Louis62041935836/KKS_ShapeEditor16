using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200000D RID: 13
	public class ItemShapeController : MonoBehaviour
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000064 RID: 100 RVA: 0x00003B18 File Offset: 0x00001D18
		// (remove) Token: 0x06000065 RID: 101 RVA: 0x00003B50 File Offset: 0x00001D50
		public event Action OnDataChanged;

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00003B85 File Offset: 0x00001D85
		public Transform RootTransform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003B8D File Offset: 0x00001D8D
		private void Awake()
		{
			this.OnDataChanged += this.HandleTimelineSync;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003BA1 File Offset: 0x00001DA1
		private void HandleTimelineSync()
		{
			TimelineRegistry.SyncController(this);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003BAC File Offset: 0x00001DAC
		public DeformData GetOrCreateDeformData(Renderer renderer)
		{
			if (renderer == null)
			{
				return null;
			}
			string relativePath = ShapeEditorController.GetRelativePath(this.RootTransform, renderer.transform);
			DeformData deformData;
			if (!this._deformDataMap.TryGetValue(relativePath, out deformData))
			{
				deformData = new DeformData(relativePath);
				this._deformDataMap[relativePath] = deformData;
			}
			return deformData;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003BFC File Offset: 0x00001DFC
		public DeformData GetDeformData(Renderer renderer)
		{
			if (renderer == null)
			{
				return null;
			}
			string relativePath = ShapeEditorController.GetRelativePath(this.RootTransform, renderer.transform);
			DeformData deformData;
			this._deformDataMap.TryGetValue(relativePath, out deformData);
			return deformData;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003C36 File Offset: 0x00001E36
		public Dictionary<string, DeformData> GetAllDeformData()
		{
			return this._deformDataMap;
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00003C40 File Offset: 0x00001E40
		public IDictionary<string, int> SubdivisionLevels
		{
			get
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (Renderer renderer in this.GetAllRenderers())
				{
					if (!(renderer == null))
					{
						int subdivisionLevel = MeshHelper.GetSubdivisionLevel(renderer);
						if (subdivisionLevel > 0)
						{
							dictionary[ShapeEditorController.GetRelativePath(this.RootTransform, renderer.transform)] = subdivisionLevel;
						}
					}
				}
				return dictionary;
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003CC0 File Offset: 0x00001EC0
		public void RestoreSingleRendererAndQueueSubdivide(string path, int targetLevel, List<int[]> facesPerLevel = null, List<bool> smoothPerLevel = null)
		{
			Renderer renderer = ShapeEditorController.FindRendererByPath(this.RootTransform, path);
			if (renderer == null)
			{
				return;
			}
			int subdivisionLevel = MeshHelper.GetSubdivisionLevel(renderer);
			bool flag = facesPerLevel != null && targetLevel > 0;
			if (!flag && subdivisionLevel == targetLevel)
			{
				return;
			}
			if (subdivisionLevel > 0 && (targetLevel < subdivisionLevel || flag))
			{
				MeshHelper.RestoreOriginal(renderer);
				ShapeDeformer.DestroyAttached(renderer);
			}
			if (targetLevel > 0)
			{
				MeshHelper.CloneMeshIfShared(renderer);
				Mesh mesh = MeshHelper.GetMesh(renderer);
				if (mesh != null)
				{
					int num = targetLevel - MeshHelper.GetSubdivisionLevel(renderer);
					if (num > 0)
					{
						List<int[]> list = new List<int[]>(num);
						List<bool> list2 = new List<bool>(num);
						int num2 = ((facesPerLevel != null) ? (facesPerLevel.Count - num) : (-1));
						for (int i = 0; i < num; i++)
						{
							int num3 = num2 + i;
							int[] array = ((facesPerLevel != null && num3 >= 0 && num3 < facesPerLevel.Count) ? facesPerLevel[num3] : null);
							list.Add(array);
							list2.Add(MeshHelper.SmoothAt(smoothPerLevel, num3));
						}
						List<List<int[]>> list3;
						MeshHelper.SubdivideReplay(mesh, list, list2, out list3, null);
						DeformData deformData;
						if (this._deformDataMap.TryGetValue(path, out deformData) && deformData.DeletedFaces.Count > 0)
						{
							deformData.DeletedFacesDirty = true;
						}
					}
				}
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003DF1 File Offset: 0x00001FF1
		public void ReattachAll()
		{
			this.AttachDeformers();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003DF9 File Offset: 0x00001FF9
		public List<Renderer> GetAllRenderers()
		{
			return ItemShapeController.CollectItemRenderers(base.transform);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003E06 File Offset: 0x00002006
		public void LoadData(Dictionary<string, DeformData> deformData)
		{
			this._deformDataMap = deformData ?? new Dictionary<string, DeformData>();
			this.AttachDeformers();
			this.EnsureDeformersAttachedDeferred();
			Action onDataChanged = this.OnDataChanged;
			if (onDataChanged == null)
			{
				return;
			}
			onDataChanged();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003E34 File Offset: 0x00002034
		private int CountUnattachedDataPaths()
		{
			int num = 0;
			foreach (KeyValuePair<string, DeformData> keyValuePair in this._deformDataMap)
			{
				DeformData value = keyValuePair.Value;
				if (value != null && (value.HasLayers || (value.DeletedFaces != null && value.DeletedFaces.Count != 0)))
				{
					Renderer renderer = ShapeEditorController.FindRendererByPath(this.RootTransform, keyValuePair.Key);
					if (renderer == null || renderer.GetComponent<ShapeDeformer>() == null)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003EDC File Offset: 0x000020DC
		private void EnsureDeformersAttachedDeferred()
		{
			if (this.CountUnattachedDataPaths() == 0)
			{
				return;
			}
			if (this._deferredAttachCo != null)
			{
				base.StopCoroutine(this._deferredAttachCo);
			}
			this._deferredAttachCo = base.StartCoroutine(this.DeferredAttachCoroutine());
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00003F0D File Offset: 0x0000210D
		private IEnumerator DeferredAttachCoroutine()
		{
			yield return null;
			this.AttachDeformers();
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00003F1C File Offset: 0x0000211C
		internal void NotifyDataChanged()
		{
			Action onDataChanged = this.OnDataChanged;
			if (onDataChanged == null)
			{
				return;
			}
			onDataChanged();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00003F30 File Offset: 0x00002130
		private void AttachDeformers()
		{
			foreach (string text in new List<string>(this._deformDataMap.Keys))
			{
				DeformData deformData;
				if (this._deformDataMap.TryGetValue(text, out deformData) && (deformData.HasLayers || (deformData.DeletedFaces != null && deformData.DeletedFaces.Count != 0)))
				{
					Renderer renderer = ShapeEditorController.FindRendererByPath(this.RootTransform, text);
					if (!(renderer == null))
					{
						ShapeDeformer shapeDeformer = renderer.GetComponent<ShapeDeformer>();
						if (shapeDeformer == null)
						{
							shapeDeformer = renderer.gameObject.AddComponent<ShapeDeformer>();
						}
						shapeDeformer.StudioMode = true;
						shapeDeformer.DeformData = deformData;
						SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
						bool flag;
						if (skinnedMeshRenderer != null)
						{
							flag = shapeDeformer.Init(skinnedMeshRenderer);
						}
						else
						{
							MeshFilter component = renderer.GetComponent<MeshFilter>();
							MeshRenderer meshRenderer = renderer as MeshRenderer;
							flag = component != null && meshRenderer != null && shapeDeformer.Init(component, meshRenderer);
						}
						if (!flag)
						{
							UnityEngine.Object.DestroyImmediate(shapeDeformer);
							this._deformDataMap.Remove(text);
						}
					}
				}
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004084 File Offset: 0x00002284
		public static List<Renderer> CollectItemRenderers(Transform root)
		{
			List<Renderer> list = new List<Renderer>();
			if (root == null)
			{
				return list;
			}
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
			{
				if (!skinnedMeshRenderer.name.EndsWith("_kkse"))
				{
					list.Add(skinnedMeshRenderer);
				}
			}
			foreach (MeshRenderer meshRenderer in root.GetComponentsInChildren<MeshRenderer>(true))
			{
				if (!(meshRenderer.GetComponent<SkinnedMeshRenderer>() != null) && !meshRenderer.name.EndsWith("_kkse"))
				{
					MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
					if (component != null && ItemShapeController.IsMeshActuallyStripped(component.sharedMesh))
					{
						ShapeEditorPlugin.Logger.LogInfo("CollectItemRenderers: skipping CPU-stripped mesh '" + meshRenderer.name + "'");
					}
					else
					{
						list.Add(meshRenderer);
					}
				}
			}
			return list;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00004164 File Offset: 0x00002364
		private static bool IsMeshActuallyStripped(Mesh mesh)
		{
			if (mesh == null)
			{
				return false;
			}
			if (mesh.vertexCount <= 0)
			{
				return true;
			}
			if (!mesh.isReadable)
			{
				return true;
			}
			bool flag;
			try
			{
				flag = mesh.vertices.Length == 0;
			}
			catch
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000041B8 File Offset: 0x000023B8
		private void OnDestroy()
		{
			this.OnDataChanged -= this.HandleTimelineSync;
			TimelineRegistry.UnregisterController(this);
			ShapeDeformer[] componentsInChildren = base.GetComponentsInChildren<ShapeDeformer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i]);
			}
		}

		// Token: 0x04000029 RID: 41
		private Dictionary<string, DeformData> _deformDataMap = new Dictionary<string, DeformData>();

		// Token: 0x0400002B RID: 43
		private Coroutine _deferredAttachCo;
	}
}
