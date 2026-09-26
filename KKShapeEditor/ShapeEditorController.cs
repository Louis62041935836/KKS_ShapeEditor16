using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensibleSaveFormat;
using KKAPI.Chara; 
using AIChara;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200001C RID: 28
	public class ShapeEditorController : CharaCustomFunctionController
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000130 RID: 304 RVA: 0x0000B130 File Offset: 0x00009330
		// (remove) Token: 0x06000131 RID: 305 RVA: 0x0000B168 File Offset: 0x00009368
		public event Action OnDataChanged;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000132 RID: 306 RVA: 0x0000B1A0 File Offset: 0x000093A0
		// (remove) Token: 0x06000133 RID: 307 RVA: 0x0000B1D8 File Offset: 0x000093D8
		public event Action OnCorruptionDetected;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000134 RID: 308 RVA: 0x0000B210 File Offset: 0x00009410
		// (remove) Token: 0x06000135 RID: 309 RVA: 0x0000B248 File Offset: 0x00009448
		public event Action OnRenderersChanged;

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000136 RID: 310 RVA: 0x0000B27D File Offset: 0x0000947D
		public Dictionary<string, CorruptionReason> CorruptionState
		{
			get
			{
				return this._corruptionState;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000137 RID: 311 RVA: 0x0000B285 File Offset: 0x00009485
		public Transform RootTransform
		{
			get
			{
				ChaControl chaControl = this.ChaControl;
				return chaControl != null ? chaControl.transform : base.transform;
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000B298 File Offset: 0x00009498
		public DeformData GetOrCreateDeformData(Renderer renderer)
		{
			if (renderer == null || this.RootTransform == null)
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
			if (!this._activeClothingKeys.Contains(relativePath) && this.IsClothingRangePath(relativePath))
			{
				this.MasterFor(this.ActiveCoordType)[relativePath] = deformData;
				this._activeClothingKeys.Add(relativePath);
			}
			return deformData;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000B32C File Offset: 0x0000952C
		public DeformData GetDeformData(Renderer renderer)
		{
			if (renderer == null || this.RootTransform == null)
			{
				return null;
			}
			string relativePath = ShapeEditorController.GetRelativePath(this.RootTransform, renderer.transform);
			DeformData deformData;
			this._deformDataMap.TryGetValue(relativePath, out deformData);
			return deformData;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000B374 File Offset: 0x00009574
		public Dictionary<string, DeformData> GetAllDeformData()
		{
			return this._deformDataMap;
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600013B RID: 315 RVA: 0x0000B37C File Offset: 0x0000957C
		public List<PsdDriver> Drivers
		{
			get
			{
				return this._psdDrivers;
			}
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000B384 File Offset: 0x00009584
		public bool HasDriverForTarget(string rendererPath, string layerId)
		{
			if (string.IsNullOrEmpty(layerId))
			{
				return false;
			}
			string text = rendererPath ?? "";
			for (int i = 0; i < this._psdDrivers.Count; i++)
			{
				PsdDriver psdDriver = this._psdDrivers[i];
				if (psdDriver.TargetLayerId == layerId && (psdDriver.TargetRendererPath ?? "") == text)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000B3F1 File Offset: 0x000095F1
		public bool TryAddDriver(PsdDriver driver)
		{
			if (driver == null)
			{
				return false;
			}
			if (this.HasDriverForTarget(driver.TargetRendererPath, driver.TargetLayerId))
			{
				return false;
			}
			this._psdDrivers.Add(driver);
			return true;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000B41B File Offset: 0x0000961B
		public void RemoveDriver(PsdDriver driver)
		{
			if (driver == null)
			{
				return;
			}
			this._psdDrivers.Remove(driver);
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600013F RID: 319 RVA: 0x0000B42E File Offset: 0x0000962E
		public List<CsbDriver> CsbDrivers
		{
			get
			{
				return this._csbDrivers;
			}
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000B438 File Offset: 0x00009638
		public bool HasCsbDriverForTarget(string rendererPath, string layerId)
		{
			if (string.IsNullOrEmpty(layerId))
			{
				return false;
			}
			string text = rendererPath ?? "";
			for (int i = 0; i < this._csbDrivers.Count; i++)
			{
				CsbDriver csbDriver = this._csbDrivers[i];
				if (csbDriver.TargetLayerId == layerId && (csbDriver.TargetRendererPath ?? "") == text)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000B4A5 File Offset: 0x000096A5
		public bool TryAddCsbDriver(CsbDriver driver)
		{
			if (driver == null)
			{
				return false;
			}
			if (this.HasCsbDriverForTarget(driver.TargetRendererPath, driver.TargetLayerId))
			{
				return false;
			}
			this._csbDrivers.Add(driver);
			return true;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000B4CF File Offset: 0x000096CF
		public void RemoveCsbDriver(CsbDriver driver)
		{
			if (driver == null)
			{
				return;
			}
			this._csbDrivers.Remove(driver);
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000143 RID: 323 RVA: 0x0000B4E2 File Offset: 0x000096E2
		public List<NbbDriver> NbbDrivers
		{
			get
			{
				return this._nbbDrivers;
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000B4EC File Offset: 0x000096EC
		public bool HasNbbDriverForTarget(string rendererPath, string layerId)
		{
			if (string.IsNullOrEmpty(layerId))
			{
				return false;
			}
			string text = rendererPath ?? "";
			for (int i = 0; i < this._nbbDrivers.Count; i++)
			{
				NbbDriver nbbDriver = this._nbbDrivers[i];
				if (nbbDriver.TargetLayerId == layerId && (nbbDriver.TargetRendererPath ?? "") == text)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000B559 File Offset: 0x00009759
		public bool TryAddNbbDriver(NbbDriver driver)
		{
			if (driver == null)
			{
				return false;
			}
			if (this.HasNbbDriverForTarget(driver.TargetRendererPath, driver.TargetLayerId))
			{
				return false;
			}
			this._nbbDrivers.Add(driver);
			return true;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000B583 File Offset: 0x00009783
		public void RemoveNbbDriver(NbbDriver driver)
		{
			if (driver == null)
			{
				return;
			}
			this._nbbDrivers.Remove(driver);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000B598 File Offset: 0x00009798
		public void InstallImportedDrivers(string targetPath, HashSet<string> importedLayerIds, List<PsdDriver> psd, List<CsbDriver> csb, List<NbbDriver> nbb, bool preserveCoordinateScope)
		{
			string text = targetPath ?? "";
			if (importedLayerIds == null)
			{
				importedLayerIds = new HashSet<string>();
			}
			for (int i = this._psdDrivers.Count - 1; i >= 0; i--)
			{
				PsdDriver psdDriver = this._psdDrivers[i];
				if (psdDriver != null && (psdDriver.TargetRendererPath ?? "") == text && importedLayerIds.Contains(psdDriver.TargetLayerId ?? ""))
				{
					this._psdDrivers.RemoveAt(i);
				}
			}
			for (int j = this._csbDrivers.Count - 1; j >= 0; j--)
			{
				CsbDriver csbDriver = this._csbDrivers[j];
				if (csbDriver != null && (csbDriver.TargetRendererPath ?? "") == text && importedLayerIds.Contains(csbDriver.TargetLayerId ?? ""))
				{
					this._csbDrivers.RemoveAt(j);
				}
			}
			for (int k = this._nbbDrivers.Count - 1; k >= 0; k--)
			{
				NbbDriver nbbDriver = this._nbbDrivers[k];
				if (nbbDriver != null && (nbbDriver.TargetRendererPath ?? "") == text && importedLayerIds.Contains(nbbDriver.TargetLayerId ?? ""))
				{
					this._nbbDrivers.RemoveAt(k);
				}
			}
			if (psd != null)
			{
				for (int l = 0; l < psd.Count; l++)
				{
					PsdDriver psdDriver2 = psd[l];
					if (psdDriver2 != null)
					{
						PsdDriver psdDriver3 = psdDriver2.Clone();
						psdDriver3.TargetRendererPath = text;
						this._psdDrivers.Add(psdDriver3);
					}
				}
			}
			if (csb != null)
			{
				int num = -1;
				for (int m = 0; m < csb.Count; m++)
				{
					CsbDriver csbDriver2 = csb[m];
					if (csbDriver2 != null)
					{
						int num2;
						if (!preserveCoordinateScope)
						{
							num2 = -1;
						}
						else if (csbDriver2.CoordinateScope < 0)
						{
							num2 = -1;
						}
						else
						{
							num2 = num;
						}
						CsbDriver csbDriver3 = csbDriver2.Clone();
						csbDriver3.TargetRendererPath = text;
						csbDriver3.CoordinateScope = num2;
						this._csbDrivers.Add(csbDriver3);
					}
				}
			}
			if (nbb != null)
			{
				for (int n = 0; n < nbb.Count; n++)
				{
					NbbDriver nbbDriver2 = nbb[n];
					if (nbbDriver2 != null)
					{
						NbbDriver nbbDriver3 = nbbDriver2.Clone();
						nbbDriver3.TargetRendererPath = text;
						this._nbbDrivers.Add(nbbDriver3);
					}
				}
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000148 RID: 328 RVA: 0x0000B80C File Offset: 0x00009A0C
		public IDictionary<string, int> SubdivisionLevels
		{
			get
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				if (this.RootTransform == null)
				{
					return dictionary;
				}
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

		// Token: 0x06000149 RID: 329 RVA: 0x0000B89C File Offset: 0x00009A9C
		public void RestoreSingleRendererAndQueueSubdivide(string path, int targetLevel, List<int[]> facesPerLevel = null, List<bool> smoothPerLevel = null)
		{
			if (this.RootTransform == null)
			{
				return;
			}
			Renderer renderer = ShapeEditorController.FindRendererByPath(this.RootTransform, path);
			if (renderer == null)
			{
				return;
			}
			string text = path ?? "";
			int subdivisionLevel = MeshHelper.GetSubdivisionLevel(renderer);
			bool flag = facesPerLevel != null && targetLevel > 0;
			if (targetLevel == 0)
			{
				if (this._pendingSubLevels != null)
				{
					this._pendingSubLevels.Remove(text);
				}
				if (this._pendingSubFaces != null)
				{
					this._pendingSubFaces.Remove(text);
				}
				if (this._pendingSubSmooth != null)
				{
					this._pendingSubSmooth.Remove(text);
				}
			}
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
				if (this._pendingSubLevels == null)
				{
					this._pendingSubLevels = new Dictionary<string, int>();
				}
				this._pendingSubLevels[text] = targetLevel;
				if (this._pendingSubFaces == null)
				{
					this._pendingSubFaces = new Dictionary<string, List<int[]>>();
				}
				List<int[]> list = new List<int[]>(targetLevel);
				for (int i = 0; i < targetLevel; i++)
				{
					int[] array = ((facesPerLevel != null && i < facesPerLevel.Count) ? facesPerLevel[i] : null);
					list.Add(array);
				}
				this._pendingSubFaces[text] = list;
				if (this._pendingSubSmooth == null)
				{
					this._pendingSubSmooth = new Dictionary<string, bool[]>();
				}
				bool[] array2 = new bool[targetLevel];
				for (int j = 0; j < targetLevel; j++)
				{
					array2[j] = MeshHelper.SmoothAt(smoothPerLevel, j);
				}
				this._pendingSubSmooth[text] = array2;
				ShapeEditorPlugin.Instance.StartCoroutine(this.DelayedRestoreCoroutine());
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000BA2A File Offset: 0x00009C2A
		internal Dictionary<string, int> GetPendingSubLevels()
		{
			return this._pendingSubLevels;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000BA32 File Offset: 0x00009C32
		internal Dictionary<string, List<int[]>> GetPendingSubFaces()
		{
			return this._pendingSubFaces;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000BA3A File Offset: 0x00009C3A
		internal Dictionary<string, bool[]> GetPendingSubSmooth()
		{
			return this._pendingSubSmooth;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000BA42 File Offset: 0x00009C42
		internal void SetPendingSubLevel(string path, int level)
		{
			if (this._pendingSubLevels == null)
			{
				this._pendingSubLevels = new Dictionary<string, int>();
			}
			this._pendingSubLevels[path] = level;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000BA64 File Offset: 0x00009C64
		internal void SetPendingSubFaces(string path, List<int[]> faces)
		{
			if (this._pendingSubFaces == null)
			{
				this._pendingSubFaces = new Dictionary<string, List<int[]>>();
			}
			this._pendingSubFaces[path] = faces;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000BA86 File Offset: 0x00009C86
		internal void SetPendingSubSmooth(string path, bool[] smooth)
		{
			if (this._pendingSubSmooth == null)
			{
				this._pendingSubSmooth = new Dictionary<string, bool[]>();
			}
			this._pendingSubSmooth[path] = smooth;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000BAA8 File Offset: 0x00009CA8
		internal void RemovePendingSubLevel(string path)
		{
			if (this._pendingSubLevels != null)
			{
				this._pendingSubLevels.Remove(path);
			}
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000BABF File Offset: 0x00009CBF
		internal void RemovePendingSubFaces(string path)
		{
			if (this._pendingSubFaces != null)
			{
				this._pendingSubFaces.Remove(path);
			}
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000BAD6 File Offset: 0x00009CD6
		internal void RemovePendingSubSmooth(string path)
		{
			if (this._pendingSubSmooth != null)
			{
				this._pendingSubSmooth.Remove(path);
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000BAED File Offset: 0x00009CED
		internal int GetReloadGeneration()
		{
			return this._reloadGeneration;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000BAF5 File Offset: 0x00009CF5
		internal void NotifyDataChanged()
		{
			Action onDataChanged = this.OnDataChanged;
			if (onDataChanged == null)
			{
				return;
			}
			onDataChanged();
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000BB08 File Offset: 0x00009D08
		internal void ReinitDeformerForPath(string path)
		{
			if (this.RootTransform == null || string.IsNullOrEmpty(path))
			{
				return;
			}
			DeformData deformData;
			if (!this._deformDataMap.TryGetValue(path, out deformData) || deformData == null)
			{
				return;
			}
			if (!deformData.HasLayers && (deformData.DeletedFaces == null || deformData.DeletedFaces.Count == 0))
			{
				return;
			}
			Renderer renderer = ShapeEditorController.FindRendererByPath(this.RootTransform, path);
			if (renderer == null)
			{
				return;
			}
			this.InitDeformerForRenderer(renderer, deformData);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000BB80 File Offset: 0x00009D80
		private void InitDeformerForRenderer(Renderer renderer, DeformData data)
		{
			ShapeDeformer shapeDeformer = renderer.GetComponent<ShapeDeformer>();
			if (shapeDeformer == null)
			{
				shapeDeformer = renderer.gameObject.AddComponent<ShapeDeformer>();
			}
			shapeDeformer.StudioMode = false;
			shapeDeformer.DeformData = data;
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
				string relativePath = ShapeEditorController.GetRelativePath(this.RootTransform, renderer.transform);
				if (!string.IsNullOrEmpty(relativePath) || this._deformDataMap.ContainsKey(""))
				{
					this._deformDataMap.Remove(relativePath);
				}
				this.DropActiveClothingEntry(relativePath);
				return;
			}
			if (data.WeightRemapped && skinnedMeshRenderer != null)
			{
				this.RecomputeRemappedWeights(shapeDeformer, skinnedMeshRenderer);
			}
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000BC68 File Offset: 0x00009E68
		public SkinnedMeshRenderer GetBodySmr()
		{
			if (this._chaControl == null || this._chaControl.objBody == null)
			{
				return null;
			}
			SkinnedMeshRenderer componentInChildren = this._chaControl.objBody.GetComponentInChildren<SkinnedMeshRenderer>();
			if (componentInChildren == null || componentInChildren.sharedMesh == null)
			{
				return null;
			}
			return componentInChildren;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000BCC3 File Offset: 0x00009EC3
		public List<Renderer> GetAllRenderers()
		{
			return ShapeEditorController.CollectCharacterRenderers(this._chaControl, this.RootTransform);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000BCD8 File Offset: 0x00009ED8
		public void BindWindow(ShapeEditorWindow window, Action onCorruption, ref ShapeEditorController subscribed)
		{
			if (window == null)
			{
				return;
			}
			List<string> list = new List<string>();
			window.Renderers = ShapeEditorController.CollectCharacterRenderers(this._chaControl, this.RootTransform, list);
			window.RendererCategories = list;
			List<string> list2 = new List<string>(window.Renderers.Count);
			Transform rootTransform = this.RootTransform;
			for (int i = 0; i < window.Renderers.Count; i++)
			{
				Renderer renderer = window.Renderers[i];
				list2.Add((renderer != null) ? ShapeEditorController.GetRelativePath(rootTransform, renderer.transform) : null);
			}
			window.RendererPaths = list2;
			window.CorruptionState = this.CorruptionState;
			if (subscribed != this)
			{
				if (subscribed != null)
				{
					subscribed.OnCorruptionDetected -= onCorruption;
					subscribed.OnRenderersChanged -= onCorruption;
				}
				this.OnCorruptionDetected += onCorruption;
				this.OnRenderersChanged += onCorruption;
				subscribed = this;
			}
			window.SanitizeSelectionAgainstRenderers();
			if (window.PresetState != null)
			{
				window.PresetState.ResetForOwnerSwitch();
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000BDCB File Offset: 0x00009FCB
		public static void UnbindCorruptionSubscription(Action onCorruption, ref ShapeEditorController subscribed)
		{
			if (subscribed == null)
			{
				return;
			}
			subscribed.OnCorruptionDetected -= onCorruption;
			subscribed.OnRenderersChanged -= onCorruption;
			subscribed = null;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000BDEC File Offset: 0x00009FEC
		private HashSet<string> ComputeCoordinatePathSet()
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (this.RootTransform == null || this._chaControl == null)
			{
				return hashSet;
			}
			foreach (Renderer renderer in ShapeEditorController.CollectClothingAccessoryRenderers(this._chaControl))
			{
				if (!(renderer == null))
				{
					hashSet.Add(ShapeEditorController.GetRelativePath(this.RootTransform, renderer.transform));
				}
			}
			return hashSet;
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600015C RID: 348 RVA: 0x0000BE84 File Offset: 0x0000A084
		private int ActiveCoordType
		{
			get
			{
				if (!(this._chaControl != null))
				{
					return 0;
				}
				return 0;
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000BEA8 File Offset: 0x0000A0A8
		private Dictionary<string, DeformData> MasterFor(int coord)
		{
			Dictionary<string, DeformData> dictionary;
			if (!this._clothingDeformByCoord.TryGetValue(coord, out dictionary))
			{
				dictionary = new Dictionary<string, DeformData>();
				this._clothingDeformByCoord[coord] = dictionary;
			}
			return dictionary;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000BEDC File Offset: 0x0000A0DC
		private void DropActiveClothingEntry(string path)
		{
			this._activeClothingKeys.Remove(path);
			Dictionary<string, DeformData> dictionary;
			if (this._clothingDeformByCoord.TryGetValue(this.ActiveCoordType, out dictionary))
			{
				dictionary.Remove(path);
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000BF14 File Offset: 0x0000A114
		private bool IsClothingRangePath(string path)
		{
			if (this._chaControl == null || this.RootTransform == null)
			{
				return false;
			}
			foreach (Renderer renderer in ShapeEditorController.CollectClothingAccessoryRenderers(this._chaControl))
			{
				if (!(renderer == null) && ShapeEditorController.GetRelativePath(this.RootTransform, renderer.transform) == path)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000BFAC File Offset: 0x0000A1AC
		private void MirrorClothingIntoMap(int coord)
		{
			foreach (string text in this._activeClothingKeys)
			{
				this._deformDataMap.Remove(text);
			}
			this._activeClothingKeys.Clear();
			Dictionary<string, DeformData> dictionary;
			if (this._clothingDeformByCoord.TryGetValue(coord, out dictionary))
			{
				foreach (KeyValuePair<string, DeformData> keyValuePair in dictionary)
				{
					this._deformDataMap[keyValuePair.Key] = keyValuePair.Value;
					this._activeClothingKeys.Add(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000C084 File Offset: 0x0000A284
		private void SyncActiveClothingToMaster()
		{
			HashSet<string> hashSet = this.ComputeCoordinatePathSet();
			if (hashSet.Count == 0)
			{
				return;
			}
			Dictionary<string, DeformData> dictionary = this.MasterFor(this.ActiveCoordType);
			foreach (string text in this._deformDataMap.Keys)
			{
				if (hashSet.Contains(text))
				{
					dictionary[text] = this._deformDataMap[text];
					this._activeClothingKeys.Add(text);
				}
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000C11C File Offset: 0x0000A31C
		private void SwapClothingCoordinate(int oldCoord, int newCoord)
		{
			Dictionary<string, DeformData> dictionary = this.MasterFor(oldCoord);
			foreach (string text in this._activeClothingKeys)
			{
				DeformData deformData;
				if (this._deformDataMap.TryGetValue(text, out deformData))
				{
					dictionary[text] = deformData;
				}
			}
			this.MirrorClothingIntoMap(newCoord);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000C190 File Offset: 0x0000A390
		private void LoadLayersIntoModel(Dictionary<string, DeformData> loaded, byte version)
		{
			this._deformDataMap = new Dictionary<string, DeformData>();
			this._clothingDeformByCoord.Clear();
			this._activeClothingKeys.Clear();
			this._pendingFlatPromotion = false;
			if (loaded == null)
			{
				return;
			}
			if (version >= 6)
			{
				foreach (KeyValuePair<string, DeformData> keyValuePair in loaded)
				{
					int num;
					string text;
					if (ShapeSerializer.TryParseClothingKey(keyValuePair.Key, out num, out text))
					{
						keyValuePair.Value.RendererPath = text;
						this.MasterFor(num)[text] = keyValuePair.Value;
					}
					else
					{
						this._deformDataMap[keyValuePair.Key] = keyValuePair.Value;
					}
				}
				this.MirrorClothingIntoMap(this.ActiveCoordType);
				return;
			}
			foreach (KeyValuePair<string, DeformData> keyValuePair2 in loaded)
			{
				this._deformDataMap[keyValuePair2.Key] = keyValuePair2.Value;
			}
			this._pendingFlatPromotion = true;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000C2D6 File Offset: 0x0000A4D6
		private void HandleTimelineSync()
		{
			TimelineRegistry.SyncController(this);
		}

		protected override void OnReload(KKAPI.GameMode currentGameMode, bool maintainState)
		{
			base.OnReload(currentGameMode, maintainState);

			this._chaControl = base.GetComponent<ChaControl>();

			if (this._chaControl == null)
			{
				ShapeEditorPlugin.Logger.LogWarning(
					"ShapeEditorController: ChaControl was not found on the controller GameObject");
				return;
			}

			this.OnDataChanged -= this.HandleTimelineSync;
			this.OnDataChanged += this.HandleTimelineSync;

			if (!maintainState)
			{
				this.OnCharaReload(false);
			}
		}

		protected override void OnCardBeingSaved(KKAPI.GameMode currentGameMode)
		{
			this.OnCharaCardSaving();
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000C300 File Offset: 0x0000A500
		protected override void Update()
		{
			CsbEvaluator.Evaluate(this);
			NbbEvaluator.Evaluate(this);
			PsdEvaluator.Evaluate(this);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000C378 File Offset: 0x0000A578
		private void OnCoordinateTypeChanged(int oldCoord, int newCoord)
		{
			int num = ((this._pendingSubLevels != null) ? this._pendingSubLevels.Count : 0);
			ShapeEditorPlugin.Logger.LogInfo(string.Concat(new string[]
			{
				"OnCoordinateTypeChanged entry ",
				oldCoord.ToString(),
				"→",
				newCoord.ToString(),
				": map=",
				this._deformDataMap.Count.ToString(),
				", masterCoords=",
				this._clothingDeformByCoord.Count.ToString(),
				", pendingSub=",
				num.ToString()
			}));
			if (this._deformDataMap.Count == 0 && this._clothingDeformByCoord.Count == 0 && num == 0)
			{
				return;
			}
			this.SwapClothingCoordinate(oldCoord, newCoord);
			ShapeEditorPlugin.Logger.LogInfo(string.Concat(new string[]
			{
				"Coordinate type changed ",
				oldCoord.ToString(),
				"→",
				newCoord.ToString(),
				": tracked coords=",
				this._clothingDeformByCoord.Count.ToString(),
				", active clothing entries=",
				this._activeClothingKeys.Count.ToString()
			}));
			if (this.RootTransform != null)
			{
				this.CleanupDeformers();
			}
			if (this._pendingSubLevels != null && this._pendingSubLevels.Count > 0)
			{
				ShapeEditorPlugin.Instance.StartCoroutine(this.DelayedRestoreCoroutine());
			}
			else
			{
				ShapeEditorPlugin.Instance.StartCoroutine(this.DelayedAttachCoroutine());
			}
			Action onDataChanged = this.OnDataChanged;
			if (onDataChanged == null)
			{
				return;
			}
			onDataChanged();
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000C520 File Offset: 0x0000A720
		public void OnCharaReload(bool maintainState)
		{
			if (maintainState)
			{
				return;
			}
			this._reloadGeneration++;
			CharacterSwitchCarryover.Snapshot snapshot = CharacterSwitchCarryover.CaptureSnapshot(this);
			if (snapshot != null)
			{
				this._pendingCarryoverSnapshot = snapshot;
			}
			this._pendingSubLevels = null;
			this._pendingSubFaces = null;
			this._pendingSubSmooth = null;
			this._psdDrivers.Clear();
			this._csbDrivers.Clear();
			this._nbbDrivers.Clear();
			byte b = 0;
			Dictionary<string, DeformData> dictionary = null;
			PluginData extendedData = base.GetExtendedData();
			if (extendedData != null)
			{
				object obj;
				if (extendedData.data.TryGetValue("layers", out obj))
				{
					byte[] array = obj as byte[];
					if (array != null)
					{
						dictionary = ShapeSerializer.DeserializeAllLayers(array, out b);
					}
				}
				object obj2;
				if (extendedData.data.TryGetValue("subdivision", out obj2))
				{
					byte[] array2 = obj2 as byte[];
					if (array2 != null)
					{
						ShapeSerializer.DeserializeSubdivisionInfo(array2, out this._pendingSubLevels, out this._pendingSubFaces, out this._pendingSubSmooth);
					}
				}
				object obj3;
				if (extendedData.data.TryGetValue("psd", out obj3))
				{
					byte[] array3 = obj3 as byte[];
					if (array3 != null)
					{
						this._psdDrivers = ShapeSerializer.DeserializeDrivers(array3);
					}
				}
				object obj4;
				if (extendedData.data.TryGetValue("csb", out obj4))
				{
					byte[] array4 = obj4 as byte[];
					if (array4 != null)
					{
						this._csbDrivers = ShapeSerializer.DeserializeCsb(array4);
					}
				}
				object obj5;
				if (extendedData.data.TryGetValue("nbb", out obj5))
				{
					byte[] array5 = obj5 as byte[];
					if (array5 != null)
					{
						this._nbbDrivers = ShapeSerializer.DeserializeNbb(array5);
					}
				}
			}
			this.LoadLayersIntoModel(dictionary, b);
			if (this.RootTransform != null)
			{
				this.CleanupDeformers();
			}
			if (this._pendingSubLevels != null && this._pendingSubLevels.Count > 0)
			{
				ShapeEditorPlugin.Instance.StartCoroutine(this.DelayedRestoreCoroutine());
			}
			else
			{
				ShapeEditorPlugin.Instance.StartCoroutine(this.DelayedAttachCoroutine());
			}
			Action onDataChanged = this.OnDataChanged;
			if (onDataChanged == null)
			{
				return;
			}
			onDataChanged();
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000C6EC File Offset: 0x0000A8EC
		public void OnCharaCardSaving()
		{
			PluginData pluginData = new PluginData();
			this.SyncActiveClothingToMaster();
			int activeCoordType = this.ActiveCoordType;
			Dictionary<string, DeformData> dictionary = new Dictionary<string, DeformData>();
			foreach (KeyValuePair<int, Dictionary<string, DeformData>> keyValuePair in this._clothingDeformByCoord)
			{
				bool flag = keyValuePair.Key == activeCoordType;
				foreach (KeyValuePair<string, DeformData> keyValuePair2 in keyValuePair.Value)
				{
					DeformData value = keyValuePair2.Value;
					if (value != null && (!flag || !this._corruptionState.ContainsKey(keyValuePair2.Key)) && (value.HasLayers || value.DeletedFaces.Count > 0))
					{
						dictionary[ShapeSerializer.MakeClothingKey(keyValuePair.Key, keyValuePair2.Key)] = value;
					}
				}
			}
			foreach (KeyValuePair<string, DeformData> keyValuePair3 in this._deformDataMap)
			{
				if (!this._activeClothingKeys.Contains(keyValuePair3.Key) && !this._corruptionState.ContainsKey(keyValuePair3.Key) && (keyValuePair3.Value.HasLayers || keyValuePair3.Value.DeletedFaces.Count > 0))
				{
					dictionary[keyValuePair3.Key] = keyValuePair3.Value;
				}
			}
			if (dictionary.Count > 0)
			{
				pluginData.data["layers"] = ShapeSerializer.SerializeAllLayers(dictionary);
			}
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			Dictionary<string, List<int[]>> dictionary3 = new Dictionary<string, List<int[]>>();
			Dictionary<string, bool[]> dictionary4 = new Dictionary<string, bool[]>();
			foreach (Renderer renderer in this.GetAllRenderers())
			{
				int subdivisionLevel = MeshHelper.GetSubdivisionLevel(renderer);
				if (subdivisionLevel > 0)
				{
					string relativePath = ShapeEditorController.GetRelativePath(this.RootTransform, renderer.transform);
					if (!this._corruptionState.ContainsKey(relativePath))
					{
						dictionary2[relativePath] = subdivisionLevel;
						List<int[]> subdivisionFaces = MeshHelper.GetSubdivisionFaces(renderer);
						if (subdivisionFaces != null)
						{
							dictionary3[relativePath] = subdivisionFaces;
						}
						List<bool> subdivisionSmooth = MeshHelper.GetSubdivisionSmooth(renderer);
						if (subdivisionSmooth != null && subdivisionSmooth.Count > 0)
						{
							dictionary4[relativePath] = subdivisionSmooth.ToArray();
						}
					}
				}
			}
			if (dictionary2.Count > 0)
			{
				pluginData.data["subdivision"] = ShapeSerializer.SerializeSubdivisionInfo(dictionary2, dictionary3, dictionary4);
			}
			for (int i = this._psdDrivers.Count - 1; i >= 0; i--)
			{
				if (!this.LayerExistsAnywhere(this._psdDrivers[i].TargetLayerId))
				{
					this._psdDrivers.RemoveAt(i);
				}
			}
			byte[] array = ShapeSerializer.SerializeDrivers(this._psdDrivers);
			if (array != null)
			{
				pluginData.data["psd"] = array;
			}
			for (int j = this._csbDrivers.Count - 1; j >= 0; j--)
			{
				if (!this.LayerExistsAnywhere(this._csbDrivers[j].TargetLayerId))
				{
					this._csbDrivers.RemoveAt(j);
				}
			}
			byte[] array2 = ShapeSerializer.SerializeCsb(this._csbDrivers);
			if (array2 != null)
			{
				pluginData.data["csb"] = array2;
			}
			for (int k = this._nbbDrivers.Count - 1; k >= 0; k--)
			{
				if (!this.LayerExistsAnywhere(this._nbbDrivers[k].TargetLayerId))
				{
					this._nbbDrivers.RemoveAt(k);
				}
			}
			byte[] array3 = ShapeSerializer.SerializeNbb(this._nbbDrivers);
			if (array3 != null)
			{
				pluginData.data["nbb"] = array3;
			}
			base.SetExtendedData((pluginData.data.Count > 0) ? pluginData : null);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000CAF4 File Offset: 0x0000ACF4
		private bool LayerExistsAnywhere(string layerId)
		{
			if (string.IsNullOrEmpty(layerId))
			{
				return false;
			}
			foreach (KeyValuePair<int, Dictionary<string, DeformData>> keyValuePair in this._clothingDeformByCoord)
			{
				foreach (KeyValuePair<string, DeformData> keyValuePair2 in keyValuePair.Value)
				{
					if (PsdEvaluator.FindLayerById(keyValuePair.Value, keyValuePair2.Key, layerId) != null)
					{
						return true;
					}
				}
			}
			foreach (KeyValuePair<string, DeformData> keyValuePair3 in this._deformDataMap)
			{
				if (PsdEvaluator.FindLayerById(this._deformDataMap, keyValuePair3.Key, layerId) != null)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000CBFC File Offset: 0x0000ADFC
		protected override void OnCoordinateBeingSaved(ChaFileCoordinate coordinate)
		{
			if (coordinate == null)
			{
				return;
			}
			List<Renderer> list = ShapeEditorController.CollectClothingAccessoryRenderers(this._chaControl);
			HashSet<string> hashSet = new HashSet<string>();
			if (this.RootTransform != null)
			{
				foreach (Renderer renderer in list)
				{
					hashSet.Add(ShapeEditorController.GetRelativePath(this.RootTransform, renderer.transform));
				}
			}
			ShapeEditorPlugin.Logger.LogInfo(string.Format("OnCoordinateBeingSaved fired: map entries={0}, coordinate range paths={1}", this._deformDataMap.Count, hashSet.Count));
			if (hashSet.Count == 0 && this._deformDataMap.Count > 0)
			{
				ShapeEditorPlugin.Logger.LogWarning("OnCoordinateBeingSaved: clothing/accessory renderer set is empty; skipping write to avoid clobbering existing data");
				return;
			}
			PluginData pluginData = new PluginData();
			Dictionary<string, DeformData> dictionary = new Dictionary<string, DeformData>();
			foreach (KeyValuePair<string, DeformData> keyValuePair in this._deformDataMap)
			{
				if (!this._corruptionState.ContainsKey(keyValuePair.Key) && hashSet.Contains(keyValuePair.Key) && (keyValuePair.Value.HasLayers || keyValuePair.Value.DeletedFaces.Count > 0))
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			if (dictionary.Count > 0)
			{
				pluginData.data["layers"] = ShapeSerializer.SerializeAllLayers(dictionary);
			}
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			Dictionary<string, List<int[]>> dictionary3 = new Dictionary<string, List<int[]>>();
			Dictionary<string, bool[]> dictionary4 = new Dictionary<string, bool[]>();
			foreach (Renderer renderer2 in list)
			{
				int subdivisionLevel = MeshHelper.GetSubdivisionLevel(renderer2);
				if (subdivisionLevel > 0)
				{
					string relativePath = ShapeEditorController.GetRelativePath(this.RootTransform, renderer2.transform);
					if (!this._corruptionState.ContainsKey(relativePath))
					{
						dictionary2[relativePath] = subdivisionLevel;
						List<int[]> subdivisionFaces = MeshHelper.GetSubdivisionFaces(renderer2);
						if (subdivisionFaces != null)
						{
							dictionary3[relativePath] = subdivisionFaces;
						}
						List<bool> subdivisionSmooth = MeshHelper.GetSubdivisionSmooth(renderer2);
						if (subdivisionSmooth != null && subdivisionSmooth.Count > 0)
						{
							dictionary4[relativePath] = subdivisionSmooth.ToArray();
						}
					}
				}
			}
			if (dictionary2.Count > 0)
			{
				pluginData.data["subdivision"] = ShapeSerializer.SerializeSubdivisionInfo(dictionary2, dictionary3, dictionary4);
			}
			base.SetCoordinateExtendedData(coordinate, (pluginData.data.Count > 0) ? pluginData : null);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000CEA4 File Offset: 0x0000B0A4
		protected override void OnCoordinateBeingLoaded(ChaFileCoordinate coordinate)
		{
			if (coordinate == null)
			{
				return;
			}
			int activeCoordType = this.ActiveCoordType;
			HashSet<string> hashSet = this.ComputeCoordinatePathSet();
			ShapeEditorPlugin.Logger.LogInfo(string.Format("OnCoordinateBeingLoaded fired (coord {0}): map entries={1}, coordinate range paths={2}", activeCoordType, this._deformDataMap.Count, hashSet.Count));
			List<string> list = new List<string>();
			foreach (string text in this._deformDataMap.Keys)
			{
				if (this._activeClothingKeys.Contains(text) || hashSet.Contains(text))
				{
					list.Add(text);
				}
			}
			foreach (string text2 in list)
			{
				this._deformDataMap.Remove(text2);
			}
			this._activeClothingKeys.Clear();
			this._clothingDeformByCoord[activeCoordType] = new Dictionary<string, DeformData>();
			PluginData coordinateExtendedData = base.GetCoordinateExtendedData(coordinate);
			if (coordinateExtendedData != null)
			{
				object obj;
				if (coordinateExtendedData.data.TryGetValue("layers", out obj))
				{
					byte[] array = obj as byte[];
					if (array != null)
					{
						Dictionary<string, DeformData> dictionary = ShapeSerializer.DeserializeAllLayers(array);
						if (dictionary != null)
						{
							Dictionary<string, DeformData> dictionary2 = this.MasterFor(activeCoordType);
							foreach (KeyValuePair<string, DeformData> keyValuePair in dictionary)
							{
								dictionary2[keyValuePair.Key] = keyValuePair.Value;
								this._deformDataMap[keyValuePair.Key] = keyValuePair.Value;
								this._activeClothingKeys.Add(keyValuePair.Key);
							}
						}
					}
				}
				object obj2;
				if (coordinateExtendedData.data.TryGetValue("subdivision", out obj2))
				{
					byte[] array2 = obj2 as byte[];
					if (array2 != null)
					{
						Dictionary<string, int> dictionary3;
						Dictionary<string, List<int[]>> dictionary4;
						Dictionary<string, bool[]> dictionary5;
						ShapeSerializer.DeserializeSubdivisionInfo(array2, out dictionary3, out dictionary4, out dictionary5);
						if (dictionary3 != null && dictionary3.Count > 0)
						{
							if (this._pendingSubLevels == null)
							{
								this._pendingSubLevels = new Dictionary<string, int>();
							}
							if (this._pendingSubFaces == null)
							{
								this._pendingSubFaces = new Dictionary<string, List<int[]>>();
							}
							if (this._pendingSubSmooth == null)
							{
								this._pendingSubSmooth = new Dictionary<string, bool[]>();
							}
							foreach (KeyValuePair<string, int> keyValuePair2 in dictionary3)
							{
								this._pendingSubLevels[keyValuePair2.Key] = keyValuePair2.Value;
							}
							if (dictionary4 != null)
							{
								foreach (KeyValuePair<string, List<int[]>> keyValuePair3 in dictionary4)
								{
									this._pendingSubFaces[keyValuePair3.Key] = keyValuePair3.Value;
								}
							}
							if (dictionary5 != null)
							{
								foreach (KeyValuePair<string, bool[]> keyValuePair4 in dictionary5)
								{
									this._pendingSubSmooth[keyValuePair4.Key] = keyValuePair4.Value;
								}
							}
						}
					}
				}
			}
			if (this.RootTransform != null)
			{
				this.CleanupDeformers();
			}
			if (this._pendingSubLevels != null && this._pendingSubLevels.Count > 0)
			{
				ShapeEditorPlugin.Instance.StartCoroutine(this.DelayedRestoreCoroutine());
			}
			else
			{
				ShapeEditorPlugin.Instance.StartCoroutine(this.DelayedAttachCoroutine());
			}
			Action onDataChanged = this.OnDataChanged;
			if (onDataChanged == null)
			{
				return;
			}
			onDataChanged();
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000D26C File Offset: 0x0000B46C
		private IEnumerator DelayedRestoreCoroutine()
		{
			yield return null;
			this.ReplayPendingSubdivision();
			this.AttachDeformers();
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000D27C File Offset: 0x0000B47C
		private void ReplayPendingSubdivision()
		{
			if (this._pendingSubLevels == null)
			{
				return;
			}
			Dictionary<string, List<int[]>> dictionary = this._pendingSubFaces ?? new Dictionary<string, List<int[]>>();
			Dictionary<string, bool[]> dictionary2 = this._pendingSubSmooth ?? new Dictionary<string, bool[]>();
			HashSet<int> hashSet = new HashSet<int>();
			foreach (KeyValuePair<string, int> keyValuePair in this._pendingSubLevels)
			{
				Renderer renderer = ShapeEditorController.FindRendererByPath(this.RootTransform, keyValuePair.Key);
				if (!(renderer == null))
				{
					MeshHelper.CloneMeshIfShared(renderer);
					Mesh mesh = MeshHelper.GetMesh(renderer);
					if (!(mesh == null))
					{
						int instanceID = mesh.GetInstanceID();
						if (!hashSet.Contains(instanceID))
						{
							if (MeshHelper.GetSubdivisionLevel(renderer) >= keyValuePair.Value)
							{
								hashSet.Add(instanceID);
							}
							else
							{
								List<int[]> list;
								dictionary.TryGetValue(keyValuePair.Key, out list);
								if (list != null && list.Count > 0)
								{
									bool[] array;
									dictionary2.TryGetValue(keyValuePair.Key, out array);
									List<bool> list2 = ((array != null) ? new List<bool>(array) : null);
									Matrix4x4[] array2 = ((array != null && Array.IndexOf<bool>(array, true) >= 0) ? MeshHelper.BuildBodySkinMatrices(renderer, mesh) : null);
									List<List<int[]>> list3;
									MeshHelper.SubdivideReplay(mesh, list, list2, out list3, array2);
									DeformData deformData;
									if (this._deformDataMap.TryGetValue(keyValuePair.Key, out deformData) && deformData.DeletedFaces.Count > 0)
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
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000D428 File Offset: 0x0000B628
		private IEnumerator DelayedAttachCoroutine()
		{
			yield return null;
			this.AttachDeformers();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000D438 File Offset: 0x0000B638
		private void AttachDeformers()
		{
			if (this.RootTransform == null)
			{
				return;
			}
			if (this._pendingFlatPromotion)
			{
				this._pendingFlatPromotion = false;
				this.SyncActiveClothingToMaster();
			}
			this._corruptionState.Clear();
			List<string> list = new List<string>();
			foreach (string text in this._deformDataMap.Keys.ToArray<string>())
			{
				Renderer renderer = ShapeEditorController.FindRendererByPath(this.RootTransform, text);
				if (!(renderer == null) && ControllerResolver.Resolve(renderer.transform).IsOnItem)
				{
					list.Add(text);
				}
			}
			if (list.Count > 0)
			{
				foreach (string text2 in list)
				{
					this._deformDataMap.Remove(text2);
				}
				ShapeEditorPlugin.Logger.LogWarning("Removed " + list.Count.ToString() + " stale entries written by older versions to character card while items were parented: " + string.Join(", ", list.ToArray()));
			}
			foreach (string text3 in this._deformDataMap.Keys.ToArray<string>())
			{
				DeformData deformData;
				if (this._deformDataMap.TryGetValue(text3, out deformData) && (deformData.HasLayers || (deformData.DeletedFaces != null && deformData.DeletedFaces.Count != 0)))
				{
					Renderer renderer2 = ShapeEditorController.FindRendererByPath(this.RootTransform, text3);
					if (!(renderer2 == null))
					{
						Mesh mesh = MeshHelper.GetMesh(renderer2);
						if (mesh != null && deformData.Layers.Count > 0 && deformData.Layers[0].Deltas.Length != mesh.vertexCount && (this._pendingSubLevels == null || !this._pendingSubLevels.ContainsKey(text3) || this._pendingSubFaces == null || !this._pendingSubFaces.ContainsKey(text3) || this._pendingSubFaces[text3] == null || this._pendingSubFaces[text3].Count <= 0))
						{
							this._corruptionState[text3] = CorruptionReason.MissingSubdivisionIntent;
						}
						else
						{
							this.InitDeformerForRenderer(renderer2, deformData);
						}
					}
				}
			}
			if (this._corruptionState.Count > 0)
			{
				ShapeEditorPlugin.Logger.LogWarning("Corrupted DeformData entries detected (MissingSubdivisionIntent): " + string.Join(", ", new List<string>(this._corruptionState.Keys).ToArray()));
				Action onCorruptionDetected = this.OnCorruptionDetected;
				if (onCorruptionDetected == null)
				{
					return;
				}
				onCorruptionDetected();
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000D6E4 File Offset: 0x0000B8E4
		public void ResetLayersForPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			this._deformDataMap.Remove(path);
			this._corruptionState.Remove(path);
			this.DropActiveClothingEntry(path);
			Action onDataChanged = this.OnDataChanged;
			if (onDataChanged == null)
			{
				return;
			}
			onDataChanged();
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000D720 File Offset: 0x0000B920
		private void RecomputeRemappedWeights(ShapeDeformer deformer, SkinnedMeshRenderer clothSmr)
		{
			SkinnedMeshRenderer bodySmr = this.GetBodySmr();
			if (bodySmr == null || clothSmr.sharedMesh == null)
			{
				return;
			}
			Mesh sharedMesh = bodySmr.sharedMesh;
			Mesh sharedMesh2 = clothSmr.sharedMesh;
			Vector3[] array = deformer.BindVertices ?? sharedMesh2.vertices;
			Vector3[] array2 = ((deformer.DeformData != null) ? deformer.DeformData.ComputeFinalDelta() : null);
			if (array2 == null || array2.Length != array.Length)
			{
				return;
			}
			BoneWeight[] array3 = WeightRemapper.ComputeRemappedWeights(array, array2, clothSmr.bones, sharedMesh.vertices, sharedMesh.boneWeights, deformer.OriginalBoneWeights, bodySmr.bones, sharedMesh.triangles);
			if (array3 != null)
			{
				deformer.RemappedBoneWeights = array3;
			}
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000D7CC File Offset: 0x0000B9CC
		private void CleanupDeformers()
		{
			if (this.RootTransform == null)
			{
				return;
			}
			ShapeDeformer[] componentsInChildren = this.RootTransform.GetComponentsInChildren<ShapeDeformer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i]);
			}
			MeshHelper.PurgeDestroyed();
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000D810 File Offset: 0x0000BA10
		public static List<Renderer> CollectCharacterRenderers(ChaControl chaCtrl, Transform root)
		{
			return ShapeEditorController.CollectCharacterRenderers(chaCtrl, root, null);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000D81C File Offset: 0x0000BA1C
		public static List<Renderer> CollectCharacterRenderers(ChaControl chaCtrl, Transform root, List<string> categoriesOut)
		{
			List<Renderer> list = new List<Renderer>();
			if (categoriesOut != null)
			{
				categoriesOut.Clear();
			}
			if (chaCtrl == null && root != null)
			{
				chaCtrl = root.GetComponent<ChaControl>();
			}
			if (chaCtrl == null)
			{
				return list;
			}
			HashSet<Renderer> hashSet = new HashSet<Renderer>();
			GameObject[] objClothes = chaCtrl.objClothes;
			if (objClothes != null)
			{
				for (int i = 0; i < objClothes.Length; i++)
				{
					ShapeEditorController.CollectRenderersFromGameObject(objClothes[i], list, hashSet, RendererCategory.ClothesCategoryKey(i), categoriesOut);
				}
			}
			GameObject[] objAccessory = chaCtrl.objAccessory;
			if (objAccessory != null)
			{
				for (int j = 0; j < objAccessory.Length; j++)
				{
					ShapeEditorController.CollectRenderersFromGameObject(objAccessory[j], list, hashSet, RendererCategory.AccessoryCategoryKey(j), categoriesOut);
				}
			}
			GameObject[] objHair = chaCtrl.objHair;
			if (objHair != null)
			{
				for (int k = 0; k < objHair.Length; k++)
				{
					ShapeEditorController.CollectRenderersFromGameObject(objHair[k], list, hashSet, RendererCategory.HairCategoryKey(k), categoriesOut);
				}
			}
			ShapeEditorController.CollectRenderersFromGameObject(chaCtrl.objBody, list, hashSet, "Body", categoriesOut);
			ShapeEditorController.CollectRenderersFromGameObject(chaCtrl.objHead, list, hashSet, "Head", categoriesOut);
			return list;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000D904 File Offset: 0x0000BB04
		public static List<Renderer> CollectClothingAccessoryRenderers(ChaControl chaCtrl)
		{
			List<Renderer> list = new List<Renderer>();
			if (chaCtrl == null)
			{
				return list;
			}
			HashSet<Renderer> hashSet = new HashSet<Renderer>();
			ShapeEditorController.CollectRenderersFromArray(chaCtrl.objClothes, list, hashSet);
			ShapeEditorController.CollectRenderersFromArray(chaCtrl.objAccessory, list, hashSet);
			return list;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000D944 File Offset: 0x0000BB44
		public static HashSet<string> CollectBodyHeadRendererPaths(ChaControl chaCtrl, Transform root)
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (chaCtrl == null || root == null)
			{
				return hashSet;
			}
			List<Renderer> list = new List<Renderer>();
			HashSet<Renderer> hashSet2 = new HashSet<Renderer>();
			ShapeEditorController.CollectRenderersFromGameObject(chaCtrl.objBody, list, hashSet2);
			ShapeEditorController.CollectRenderersFromGameObject(chaCtrl.objHead, list, hashSet2);
			foreach (Renderer renderer in list)
			{
				if (!(renderer == null))
				{
					hashSet.Add(ShapeEditorController.GetRelativePath(root, renderer.transform));
				}
			}
			return hashSet;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000D9EC File Offset: 0x0000BBEC
		private static void CollectRenderersFromArray(GameObject[] roots, List<Renderer> result, HashSet<Renderer> seen)
		{
			if (roots == null)
			{
				return;
			}
			for (int i = 0; i < roots.Length; i++)
			{
				ShapeEditorController.CollectRenderersFromGameObject(roots[i], result, seen);
			}
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000DA17 File Offset: 0x0000BC17
		private static void CollectRenderersFromGameObject(GameObject go, List<Renderer> result, HashSet<Renderer> seen)
		{
			ShapeEditorController.CollectRenderersFromGameObject(go, result, seen, null, null);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000DA24 File Offset: 0x0000BC24
		private static void CollectRenderersFromGameObject(GameObject go, List<Renderer> result, HashSet<Renderer> seen, string category, List<string> categoriesOut)
		{
			if (go == null)
			{
				return;
			}
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in go.GetComponentsInChildren<SkinnedMeshRenderer>(true))
			{
				if (seen.Add(skinnedMeshRenderer))
				{
					result.Add(skinnedMeshRenderer);
					if (categoriesOut != null)
					{
						categoriesOut.Add(category);
					}
				}
			}
			foreach (MeshRenderer meshRenderer in go.GetComponentsInChildren<MeshRenderer>(true))
			{
				if (!(meshRenderer.GetComponent<SkinnedMeshRenderer>() != null) && !meshRenderer.name.EndsWith("_kkse"))
				{
					MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
					if (!(component == null) && !(component.sharedMesh == null) && component.sharedMesh.isReadable && seen.Add(meshRenderer))
					{
						result.Add(meshRenderer);
						if (categoriesOut != null)
						{
							categoriesOut.Add(category);
						}
					}
				}
			}
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000DAFC File Offset: 0x0000BCFC
		public static string GetRelativePath(Transform root, Transform target)
		{
			if (root == null || target == null)
			{
				if (!(target != null))
				{
					return "";
				}
				return target.name;
			}
			else
			{
				if (target == root)
				{
					return "";
				}
				List<string> list = new List<string>();
				Transform transform = target;
				while (transform != null && transform != root)
				{
					list.Add(transform.name);
					transform = transform.parent;
				}
				if (transform != root)
				{
					return target.name;
				}
				list.Reverse();
				return string.Join("/", list.ToArray());
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000DB98 File Offset: 0x0000BD98
		public static Renderer FindRendererByPath(Transform root, string path)
		{
			if (root == null)
			{
				return null;
			}
			Transform transform = (string.IsNullOrEmpty(path) ? root : root.Find(path));
			if (transform == null)
			{
				return null;
			}
			SkinnedMeshRenderer component = transform.GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				return component;
			}
			return transform.GetComponent<MeshRenderer>();
		}

		// Token: 0x040000A2 RID: 162
		private Dictionary<string, DeformData> _deformDataMap = new Dictionary<string, DeformData>();

		private ChaControl _chaControl;

		public new ChaControl ChaControl
		{
			get
			{
				if (this._chaControl == null)
				{
					this._chaControl = base.GetComponent<ChaControl>();
				}

				return this._chaControl;
			}
			set
			{
				this._chaControl = value;
			}
		}

		// Token: 0x040000A3 RID: 163
		private Dictionary<int, Dictionary<string, DeformData>> _clothingDeformByCoord = new Dictionary<int, Dictionary<string, DeformData>>();

		// Token: 0x040000A4 RID: 164
		private HashSet<string> _activeClothingKeys = new HashSet<string>();

		// Token: 0x040000A5 RID: 165
		private bool _pendingFlatPromotion;

		// Token: 0x040000A6 RID: 166
		private Dictionary<string, int> _pendingSubLevels;

		// Token: 0x040000A7 RID: 167
		private Dictionary<string, List<int[]>> _pendingSubFaces;

		// Token: 0x040000A8 RID: 168
		private Dictionary<string, bool[]> _pendingSubSmooth;

		// Token: 0x040000A9 RID: 169
		private Dictionary<string, CorruptionReason> _corruptionState = new Dictionary<string, CorruptionReason>();

		// Token: 0x040000AB RID: 171
		private List<PsdDriver> _psdDrivers = new List<PsdDriver>();

		// Token: 0x040000AC RID: 172
		private List<CsbDriver> _csbDrivers = new List<CsbDriver>();

		// Token: 0x040000AD RID: 173
		private List<NbbDriver> _nbbDrivers = new List<NbbDriver>();

		// Token: 0x040000AE RID: 174
		private int _reloadGeneration;

		// Token: 0x040000AF RID: 175
		private CharacterSwitchCarryover.Snapshot _pendingCarryoverSnapshot;
	}
}
