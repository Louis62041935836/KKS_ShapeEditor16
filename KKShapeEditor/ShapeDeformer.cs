using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200001B RID: 27
	public class ShapeDeformer : MonoBehaviour
	{
		// Token: 0x060000F9 RID: 249 RVA: 0x00008C90 File Offset: 0x00006E90
		public static void DestroyAttached(Renderer renderer)
		{
			if (renderer == null)
			{
				return;
			}
			ShapeDeformer component = renderer.GetComponent<ShapeDeformer>();
			if (component != null)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00008CBD File Offset: 0x00006EBD
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00008CC5 File Offset: 0x00006EC5
		public DeformData DeformData { get; set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00008CCE File Offset: 0x00006ECE
		private string SourceName
		{
			get
			{
				if (this._smr != null)
				{
					return this._smr.name;
				}
				if (this._sourceMeshFilter != null)
				{
					return this._sourceMeshFilter.name;
				}
				return "?";
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00008D0C File Offset: 0x00006F0C
		private void LogDeltaLengthMismatchOnce(string siteLabel, int deltaLen, int vertCount)
		{
			if (this._loggedDeltaMismatch)
			{
				return;
			}
			this._loggedDeltaMismatch = true;
			ShapeEditorPlugin.Logger.LogWarning(string.Format("{0}: delta length mismatch (deltaLen={1}, vertCount={2}, mesh='{3}')", new object[] { siteLabel, deltaLen, vertCount, this.SourceName }));
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00008D62 File Offset: 0x00006F62
		// (set) Token: 0x060000FF RID: 255 RVA: 0x00008D6A File Offset: 0x00006F6A
		public bool StudioMode
		{
			get
			{
				return this._studioMode;
			}
			set
			{
				this._studioMode = value;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00008D73 File Offset: 0x00006F73
		public Mesh DisplayMesh
		{
			get
			{
				return this._displayMeshRef;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00008D7C File Offset: 0x00006F7C
		public Transform DisplayTransform
		{
			get
			{
				if (this._smr != null)
				{
					return this._smr.transform;
				}
				if (this._sourceMeshRenderer != null)
				{
					return this._sourceMeshRenderer.transform;
				}
				if (this._sourceMeshFilter != null)
				{
					return this._sourceMeshFilter.transform;
				}
				return null;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00008DD8 File Offset: 0x00006FD8
		public bool IsEditMode
		{
			get
			{
				return this._editMode;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00008DE0 File Offset: 0x00006FE0
		// (set) Token: 0x06000104 RID: 260 RVA: 0x00008E08 File Offset: 0x00007008
		public BoneWeight[] RemappedBoneWeights
		{
			get
			{
				if (!this._isRemapped || !(this._clonedSharedMesh != null))
				{
					return null;
				}
				return this._clonedSharedMesh.boneWeights;
			}
			set
			{
				if (value != null && this._clonedSharedMesh != null)
				{
					this._clonedSharedMesh.boneWeights = value;
					this._activeBoneWeights = value;
					this._isRemapped = true;
				}
				else if (value == null)
				{
					if (this._isRemapped && this._clonedSharedMesh != null && this._pristineBoneWeights != null)
					{
						this._clonedSharedMesh.boneWeights = this._pristineBoneWeights;
					}
					this._activeBoneWeights = this._pristineBoneWeights;
					this._isRemapped = false;
				}
				this.InvalidateDeltaCache();
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000105 RID: 261 RVA: 0x00008E8D File Offset: 0x0000708D
		public bool HasRemappedWeights
		{
			get
			{
				return this._isRemapped;
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00008E98 File Offset: 0x00007098
		public void ClearRemappedWeights()
		{
			if (this._isRemapped && this._clonedSharedMesh != null && this._pristineBoneWeights != null)
			{
				this._clonedSharedMesh.boneWeights = this._pristineBoneWeights;
			}
			this._activeBoneWeights = this._pristineBoneWeights;
			this._isRemapped = false;
			this.InvalidateDeltaCache();
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00008EED File Offset: 0x000070ED
		public Vector3[] BindVertices
		{
			get
			{
				if (this._isStatic)
				{
					return this._restVertices;
				}
				if (!(this._clonedSharedMesh != null))
				{
					return null;
				}
				return this._clonedSharedMesh.vertices;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000108 RID: 264 RVA: 0x00008F19 File Offset: 0x00007119
		public BoneWeight[] OriginalBoneWeights
		{
			get
			{
				return this._pristineBoneWeights;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000109 RID: 265 RVA: 0x00008F21 File Offset: 0x00007121
		public Transform[] SmrBones
		{
			get
			{
				return this._smrBones;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600010A RID: 266 RVA: 0x00008F29 File Offset: 0x00007129
		public int LastVertsHash
		{
			get
			{
				return this._lastVertsHash;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600010B RID: 267 RVA: 0x00008F31 File Offset: 0x00007131
		public int LastRebakeFrame
		{
			get
			{
				return this._lastRebakeFrame;
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00008F39 File Offset: 0x00007139
		public void InvalidateDeltaCache()
		{
			this._cachedFinalDeltas = null;
			this._forceSyncOnce = true;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00008F4C File Offset: 0x0000714C
		public bool Init(SkinnedMeshRenderer smr)
		{
			int num = ((smr.sharedMesh != null) ? smr.sharedMesh.GetInstanceID() : 0);
			if (this._smr == smr && !this._isStatic && this._sourceMeshId == num)
			{
				return true;
			}
			if (this._smr == smr && this._sourceMeshId != num)
			{
				this._cachedFinalDeltas = null;
			}
			this.CleanupPreviousInitState();
			this._smr = smr;
			this._isStatic = false;
			this._sourceMeshFilter = null;
			this._sourceMeshRenderer = null;
			this._restVertices = null;
			this._staticWorkingVerts = null;
			MeshHelper.CloneMeshIfShared(smr);
			this._clonedSharedMesh = smr.sharedMesh;
			if (!ShapeDeformer.CapabilityProbe(this._clonedSharedMesh, smr.name))
			{
				this._clonedSharedMesh = null;
				this._smr = null;
				return false;
			}
			this._clonedSharedMesh.MarkDynamic();
			this._sourceMeshId = this._clonedSharedMesh.GetInstanceID();
			this._pristineBoneWeights = this._clonedSharedMesh.boneWeights;
			this._activeBoneWeights = this._pristineBoneWeights;
			Color[] colors = this._clonedSharedMesh.colors;
			this._restColors = ((colors != null && colors.Length != 0) ? colors : null);
			if (smr.sharedMesh != null)
			{
				this._bindPoses = smr.sharedMesh.bindposes;
				this._smrBones = smr.bones;
				if (this._smrBones != null && this._smrBones.Length != 0)
				{
					this._boneMatrices = new Matrix4x4[this._smrBones.Length];
				}
				this._boneMatricesFrame = -1;
			}
			this._isRemapped = false;
			this._lastInPlaceVertCount = this._clonedSharedMesh.vertexCount;
			this._hasProducedDisplayMesh = false;
			return true;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000090E8 File Offset: 0x000072E8
		public bool Init(MeshFilter sourceMf, MeshRenderer sourceMr)
		{
			Mesh mesh = ((sourceMf != null) ? sourceMf.sharedMesh : null);
			if (mesh == null || !mesh.isReadable)
			{
				if (mesh != null && ShapeDeformer._loggedProbeFailureMeshIds.Add(mesh.GetInstanceID()))
				{
					ShapeEditorPlugin.Logger.LogWarning(string.Concat(new string[] { "static mesh '", mesh.name, "' on renderer '", sourceMf.name, "' is not readable (CPU vertex data stripped) — cannot edit, skipping" }));
				}
				return false;
			}
			int num = ((sourceMf.sharedMesh != null) ? sourceMf.sharedMesh.GetInstanceID() : 0);
			if (this._isStatic && this._sourceMeshFilter == sourceMf && this._sourceMeshId == num)
			{
				return true;
			}
			if (this._sourceMeshFilter == sourceMf && this._sourceMeshId != num)
			{
				this._cachedFinalDeltas = null;
			}
			this.CleanupPreviousInitState();
			this._isStatic = true;
			this._sourceMeshFilter = sourceMf;
			this._sourceMeshRenderer = sourceMr;
			this._smr = null;
			MeshHelper.CloneMeshIfShared(sourceMf);
			this._clonedSharedMesh = sourceMf.sharedMesh;
			if (!ShapeDeformer.CapabilityProbe(this._clonedSharedMesh, sourceMf.name))
			{
				this._clonedSharedMesh = null;
				this._isStatic = false;
				this._sourceMeshFilter = null;
				this._sourceMeshRenderer = null;
				return false;
			}
			this._clonedSharedMesh.MarkDynamic();
			this._sourceMeshId = this._clonedSharedMesh.GetInstanceID();
			this._restVertices = this._clonedSharedMesh.vertices;
			Color[] colors = this._clonedSharedMesh.colors;
			this._restColors = ((colors != null && colors.Length != 0) ? colors : null);
			this._pristineBoneWeights = null;
			this._activeBoneWeights = null;
			this._isRemapped = false;
			this._lastInPlaceVertCount = this._clonedSharedMesh.vertexCount;
			this._hasProducedDisplayMesh = false;
			return true;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000092B0 File Offset: 0x000074B0
		private static bool CapabilityProbe(Mesh mesh, string rendererName)
		{
			if (mesh == null)
			{
				return false;
			}
			int vertexCount = mesh.vertexCount;
			if (vertexCount <= 0)
			{
				return false;
			}
			if (mesh.vertices.Length != vertexCount)
			{
				int instanceID = mesh.GetInstanceID();
				if (ShapeDeformer._loggedProbeFailureMeshIds.Add(instanceID))
				{
					ShapeEditorPlugin.Logger.LogWarning(string.Concat(new string[] { "mesh '", mesh.name, "' on renderer '", rendererName, "' cannot be edited — vertices stripped" }));
				}
				return false;
			}
			return true;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00009334 File Offset: 0x00007534
		public void EnterEditMode(Material editMaterial)
		{
			this._editMode = true;
			this._editMaterial = editMaterial;
			if (this._clonedSharedMesh != null)
			{
				Color[] colors = this._clonedSharedMesh.colors;
				this._restColors = ((colors != null && colors.Length != 0) ? colors : null);
			}
			this._editMaterialApplied = false;
			if (editMaterial == null)
			{
				return;
			}
			Renderer sourceRenderer = this.SourceRenderer;
			if (sourceRenderer == null)
			{
				return;
			}
			Material[] sharedMaterials = sourceRenderer.sharedMaterials;
			if (sharedMaterials == null)
			{
				return;
			}
			this._originalMaterials = (Material[])sharedMaterials.Clone();
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000093B8 File Offset: 0x000075B8
		public void ExitEditMode()
		{
			this._editMode = false;
			this._editMaterial = null;
			this.RestoreOriginalMaterials();
			this._editMaterialApplied = false;
			if (this._clonedSharedMesh != null)
			{
				this._clonedSharedMesh.colors = this._restColors;
			}
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000093F4 File Offset: 0x000075F4
		public void SetEditColors(Color[] colors)
		{
			if (this._clonedSharedMesh != null && colors != null && colors.Length == this._clonedSharedMesh.vertexCount)
			{
				this._clonedSharedMesh.colors = colors;
			}
			if (!this._editMaterialApplied && colors != null && this._editMaterial != null && this._originalMaterials != null)
			{
				Renderer sourceRenderer = this.SourceRenderer;
				if (sourceRenderer != null)
				{
					Material[] array = new Material[this._originalMaterials.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = this._editMaterial;
					}
					sourceRenderer.sharedMaterials = array;
					this._editMaterialApplied = true;
				}
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00009494 File Offset: 0x00007694
		public static int HashMatrix4x4(Matrix4x4 m)
		{
			return (((((((((((((((17 * 31 + m.m00.GetHashCode()) * 31 + m.m01.GetHashCode()) * 31 + m.m02.GetHashCode()) * 31 + m.m03.GetHashCode()) * 31 + m.m10.GetHashCode()) * 31 + m.m11.GetHashCode()) * 31 + m.m12.GetHashCode()) * 31 + m.m13.GetHashCode()) * 31 + m.m20.GetHashCode()) * 31 + m.m21.GetHashCode()) * 31 + m.m22.GetHashCode()) * 31 + m.m23.GetHashCode()) * 31 + m.m30.GetHashCode()) * 31 + m.m31.GetHashCode()) * 31 + m.m32.GetHashCode()) * 31 + m.m33.GetHashCode();
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000095A4 File Offset: 0x000077A4
		private static int ComputeVertsHash(Vector3[] verts)
		{
			if (verts == null)
			{
				return 0;
			}
			int num = 17;
			int num2 = verts.Length;
			for (int i = 0; i < num2; i++)
			{
				num = num * 31 + verts[i].x.GetHashCode();
				num = num * 31 + verts[i].y.GetHashCode();
				num = num * 31 + verts[i].z.GetHashCode();
			}
			return num;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00009610 File Offset: 0x00007810
		private void LateUpdate()
		{
			if (this.DetectMeshReplacement())
			{
				return;
			}
			if (this._clonedSharedMesh == null)
			{
				return;
			}
			bool flag = this._clonedSharedMesh.vertexCount != this._lastInPlaceVertCount;
			bool flag2 = this.DeformData != null && this.DeformData.AnyDirty();
			bool flag3 = this.DeformData != null && this.DeformData.HasLayers;
			bool flag4 = !this._hasProducedDisplayMesh && flag3;
			bool flag5 = !flag3 && this._hasProducedDisplayMesh;
			bool flag6 = this.DeformData != null && this.DeformData.DeletedFacesDirty;
			if (!flag2 && !flag && !flag4 && !flag5 && !flag6 && !this._forceSyncOnce)
			{
				return;
			}
			this._forceSyncOnce = false;
			if (this._isStatic)
			{
				this.SyncStaticInPlace();
				return;
			}
			this.SyncBlendShapesFromLayers();
		}

		// Token: 0x06000116 RID: 278 RVA: 0x000096E0 File Offset: 0x000078E0
		private void EnsureOrigTrianglesSnapshot()
		{
			if (this._clonedSharedMesh == null)
			{
				return;
			}
			int subMeshCount = this._clonedSharedMesh.subMeshCount;
			if (this._origTrianglesPerSubmesh != null && this._origTrianglesPerSubmesh.Length == subMeshCount)
			{
				return;
			}
			this._origTrianglesPerSubmesh = new int[subMeshCount][];
			for (int i = 0; i < subMeshCount; i++)
			{
				this._origTrianglesPerSubmesh[i] = this._clonedSharedMesh.GetTriangles(i);
			}
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00009748 File Offset: 0x00007948
		public int[] GetOriginalTriangles(int submesh)
		{
			this.EnsureOrigTrianglesSnapshot();
			if (this._origTrianglesPerSubmesh == null || submesh < 0 || submesh >= this._origTrianglesPerSubmesh.Length)
			{
				return null;
			}
			return this._origTrianglesPerSubmesh[submesh];
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00009771 File Offset: 0x00007971
		public int OriginalSubmeshCount
		{
			get
			{
				if (!(this._clonedSharedMesh != null))
				{
					return 0;
				}
				return this._clonedSharedMesh.subMeshCount;
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00009790 File Offset: 0x00007990
		public void RestoreFullTriangles()
		{
			if (this._clonedSharedMesh == null || this._origTrianglesPerSubmesh == null)
			{
				return;
			}
			int subMeshCount = this._clonedSharedMesh.subMeshCount;
			if (this._origTrianglesPerSubmesh.Length != subMeshCount)
			{
				return;
			}
			for (int i = 0; i < subMeshCount; i++)
			{
				this._clonedSharedMesh.SetTriangles(this._origTrianglesPerSubmesh[i], i);
			}
			this._origTrianglesPerSubmesh = null;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000097F4 File Offset: 0x000079F4
		private void RewriteTrianglesWithMask(Mesh mesh)
		{
			if (mesh == null || this.DeformData == null)
			{
				return;
			}
			this.EnsureOrigTrianglesSnapshot();
			if (this._origTrianglesPerSubmesh == null)
			{
				return;
			}
			HashSet<int> deletedFaces = this.DeformData.DeletedFaces;
			if (deletedFaces == null || deletedFaces.Count == 0)
			{
				if (this.DeformData.DeletedFacesDirty)
				{
					for (int i = 0; i < this._origTrianglesPerSubmesh.Length; i++)
					{
						mesh.SetTriangles(this._origTrianglesPerSubmesh[i], i);
					}
					this.DeformData.DeletedFacesDirty = false;
				}
				this._vertexLive = null;
				return;
			}
			int num = 0;
			for (int j = 0; j < this._origTrianglesPerSubmesh.Length; j++)
			{
				num += this._origTrianglesPerSubmesh[j].Length / 3;
			}
			bool flag = false;
			int num2 = 0;
			foreach (int num3 in deletedFaces)
			{
				if (num3 < 0 || num3 >= num)
				{
					flag = true;
					num2 = num3;
					break;
				}
			}
			if (flag)
			{
				if (!this._loggedDeletedFaceMismatch)
				{
					this._loggedDeletedFaceMismatch = true;
					ShapeEditorPlugin.Logger.LogWarning("RewriteTrianglesWithMask: deleted face index out of range " + string.Format("(value={0}, totalFaces={1}, mesh='{2}') — clearing face mask", num2, num, this.SourceName));
				}
				deletedFaces.Clear();
				this.DeformData.DeletedFacesDirty = false;
				for (int k = 0; k < this._origTrianglesPerSubmesh.Length; k++)
				{
					mesh.SetTriangles(this._origTrianglesPerSubmesh[k], k);
				}
				this._vertexLive = null;
				return;
			}
			int vertexCount = mesh.vertexCount;
			if (this._vertexLive == null || this._vertexLive.Length != vertexCount)
			{
				this._vertexLive = new bool[vertexCount];
			}
			else
			{
				Array.Clear(this._vertexLive, 0, vertexCount);
			}
			int num4 = 0;
			for (int l = 0; l < this._origTrianglesPerSubmesh.Length; l++)
			{
				int[] array = this._origTrianglesPerSubmesh[l];
				int num5 = array.Length / 3;
				int num6 = array.Length;
				if (this._triRewriteBuf == null || this._triRewriteBuf.Length < num6)
				{
					this._triRewriteBuf = new int[num6];
				}
				int num7 = 0;
				for (int m = 0; m < num5; m++)
				{
					if (!deletedFaces.Contains(num4 + m))
					{
						int num8 = m * 3;
						int num9 = array[num8];
						int num10 = array[num8 + 1];
						int num11 = array[num8 + 2];
						this._triRewriteBuf[num7++] = num9;
						this._triRewriteBuf[num7++] = num10;
						this._triRewriteBuf[num7++] = num11;
						if (num9 >= 0 && num9 < vertexCount)
						{
							this._vertexLive[num9] = true;
						}
						if (num10 >= 0 && num10 < vertexCount)
						{
							this._vertexLive[num10] = true;
						}
						if (num11 >= 0 && num11 < vertexCount)
						{
							this._vertexLive[num11] = true;
						}
					}
				}
				int[] array2 = new int[num7];
				Array.Copy(this._triRewriteBuf, array2, num7);
				mesh.SetTriangles(array2, l);
				num4 += num5;
			}
			this.DeformData.DeletedFacesDirty = false;
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00009B04 File Offset: 0x00007D04
		public bool[] LiveVertexMask
		{
			get
			{
				return this._vertexLive;
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00009B0C File Offset: 0x00007D0C
		private bool DetectMeshReplacement()
		{
			if (this._smr != null && !this._isStatic && this._smr.sharedMesh != null && this._sourceMeshId != 0 && this._smr.sharedMesh.GetInstanceID() != this._sourceMeshId)
			{
				if (this.DropIfBare())
				{
					return true;
				}
				ShapeEditorPlugin.Logger.LogInfo(string.Format("ShapeDeformer: SMR mesh replaced (stored={0}, current={1}), adopting", this._sourceMeshId, this._smr.sharedMesh.GetInstanceID()));
				if (this.AdoptReplacedMesh(this._smr.sharedMesh))
				{
					Action onMeshReplaced = this.OnMeshReplaced;
					if (onMeshReplaced != null)
					{
						onMeshReplaced();
					}
				}
				return true;
			}
			else
			{
				if (!this._isStatic || !(this._sourceMeshFilter != null) || !(this._sourceMeshFilter.sharedMesh != null) || this._sourceMeshId == 0 || this._sourceMeshFilter.sharedMesh.GetInstanceID() == this._sourceMeshId)
				{
					return false;
				}
				if (this.DropIfBare())
				{
					return true;
				}
				ShapeEditorPlugin.Logger.LogInfo("ShapeDeformer: static mesh replaced, adopting");
				if (this.AdoptReplacedMesh(this._sourceMeshFilter.sharedMesh))
				{
					Action onMeshReplaced2 = this.OnMeshReplaced;
					if (onMeshReplaced2 != null)
					{
						onMeshReplaced2();
					}
				}
				return true;
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00009C5C File Offset: 0x00007E5C
		private bool DropIfBare()
		{
			if (this.DeformData != null && (this.DeformData.HasLayers || (this.DeformData.DeletedFaces != null && this.DeformData.DeletedFaces.Count > 0)))
			{
				return false;
			}
			ShapeEditorPlugin.Logger.LogInfo("ShapeDeformer: dropping bare deformer on mesh replacement (no layers/face mask to adopt)");
			UnityEngine.Object.Destroy(this);
			return true;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00009CC4 File Offset: 0x00007EC4
		private bool AdoptReplacedMesh(Mesh newMesh)
		{
			if (newMesh == null)
			{
				return false;
			}
			string text = ((this._smr != null) ? this._smr.name : ((this._sourceMeshFilter != null) ? this._sourceMeshFilter.name : "?"));
			if (!ShapeDeformer.CapabilityProbe(newMesh, text))
			{
				return false;
			}
			if (this._clonedSharedMesh != null && this._clonedSharedMesh != newMesh)
			{
				ShapeDeformer.ClearKkseFramesIfAny(this._clonedSharedMesh, null);
			}
			this._clonedSharedMesh = newMesh;
			newMesh.MarkDynamic();
			this._sourceMeshId = newMesh.GetInstanceID();
			MeshHelper.MarkAsAlreadyCloned(newMesh);
			this._cachedFinalDeltas = null;
			this._hasProducedDisplayMesh = false;
			this._lastInPlaceVertCount = newMesh.vertexCount;
			this._loggedDeltaMismatch = false;
			this._isRemapped = false;
			this._forcePosedRebake = true;
			this._origTrianglesPerSubmesh = null;
			this._vertexLive = null;
			if (this.DeformData != null && this.DeformData.DeletedFaces.Count > 0)
			{
				this.DeformData.DeletedFacesDirty = true;
			}
			Color[] colors = newMesh.colors;
			this._restColors = ((colors != null && colors.Length != 0) ? colors : null);
			if (this._isStatic)
			{
				this._restVertices = newMesh.vertices;
				this._staticWorkingVerts = null;
				this._pristineBoneWeights = null;
				this._activeBoneWeights = null;
			}
			else if (this._smr != null)
			{
				this._pristineBoneWeights = newMesh.boneWeights;
				this._activeBoneWeights = this._pristineBoneWeights;
				this._bindPoses = newMesh.bindposes;
				this._smrBones = this._smr.bones;
				if (this._smrBones != null && this._smrBones.Length != 0 && (this._boneMatrices == null || this._boneMatrices.Length != this._smrBones.Length))
				{
					this._boneMatrices = new Matrix4x4[this._smrBones.Length];
				}
				this._boneMatricesFrame = -1;
			}
			return true;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00009EA0 File Offset: 0x000080A0
		private void SyncStaticInPlace()
		{
			if (this._clonedSharedMesh == null)
			{
				return;
			}
			try
			{
				int vertexCount = this._clonedSharedMesh.vertexCount;
				bool flag = vertexCount != this._lastInPlaceVertCount;
				this._lastInPlaceVertCount = vertexCount;
				if (this._restVertices == null || this._restVertices.Length != vertexCount || flag)
				{
					this._restVertices = this._clonedSharedMesh.vertices;
					this._loggedDeltaMismatch = false;
				}
				if (flag)
				{
					this._origTrianglesPerSubmesh = null;
				}
				if (this.DeformData == null || !this.DeformData.HasLayers)
				{
					if (this._hasProducedDisplayMesh)
					{
						this._clonedSharedMesh.vertices = this._restVertices;
						this._clonedSharedMesh.RecalculateBounds();
						this._hasProducedDisplayMesh = false;
						this._lastVertsHash = ShapeDeformer.ComputeVertsHash(this._restVertices);
						this._cachedFinalDeltas = null;
					}
					this.RewriteTrianglesWithMask(this._clonedSharedMesh);
					return;
				}
				Vector3[] array = this.DeformData.ComputeFinalDelta();
				if (array == null || array.Length != vertexCount)
				{
					if (array != null)
					{
						this.LogDeltaLengthMismatchOnce("SyncStaticInPlace", array.Length, vertexCount);
					}
					return;
				}
				if (this._staticWorkingVerts == null || this._staticWorkingVerts.Length != vertexCount)
				{
					this._staticWorkingVerts = new Vector3[vertexCount];
				}
				for (int i = 0; i < vertexCount; i++)
				{
					this._staticWorkingVerts[i].x = this._restVertices[i].x + array[i].x;
					this._staticWorkingVerts[i].y = this._restVertices[i].y + array[i].y;
					this._staticWorkingVerts[i].z = this._restVertices[i].z + array[i].z;
				}
				this._cachedFinalDeltas = array;
				this._clonedSharedMesh.vertices = this._staticWorkingVerts;
				this._clonedSharedMesh.RecalculateBounds();
				this._lastVertsHash = ShapeDeformer.ComputeVertsHash(this._staticWorkingVerts);
				this._hasProducedDisplayMesh = true;
				this.RewriteTrianglesWithMask(this._clonedSharedMesh);
			}
			finally
			{
			}
			Action onDeformationApplied = this.OnDeformationApplied;
			if (onDeformationApplied == null)
			{
				return;
			}
			onDeformationApplied();
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000A0E8 File Offset: 0x000082E8
		private void SyncBlendShapesFromLayers()
		{
			if (this._clonedSharedMesh == null)
			{
				return;
			}
			try
			{
				int vertexCount = this._clonedSharedMesh.vertexCount;
				bool flag = vertexCount != this._lastInPlaceVertCount;
				this._lastInPlaceVertCount = vertexCount;
				if (flag)
				{
					this._origTrianglesPerSubmesh = null;
				}
				List<DeformLayer> list = ((this.DeformData != null) ? this.DeformData.Layers : null);
				bool flag2 = list != null && list.Count > 0;
				if (flag2)
				{
					for (int i = 0; i < list.Count; i++)
					{
						Vector3[] deltas = list[i].Deltas;
						if (deltas == null || deltas.Length != vertexCount)
						{
							this.LogDeltaLengthMismatchOnce("SyncBlendShapesFromLayers", (deltas != null) ? deltas.Length : 0, vertexCount);
							for (int j = 0; j < list.Count; j++)
							{
								list[j].Dirty = false;
							}
							return;
						}
					}
				}
				BlendShapeRebuildCache.CacheSmrWeights(this._smr, this._clonedSharedMesh);
				BlendShapeRebuildCache.Cache(this._clonedSharedMesh);
				this._clonedSharedMesh.ClearBlendShapes();
				BlendShapeRebuildCache.Restore(this._clonedSharedMesh);
				if (flag2)
				{
					Vector3[] zeroNormals = BlendShapeRebuildCache.GetZeroNormals(vertexCount);
					Vector3[] zeroTangents = BlendShapeRebuildCache.GetZeroTangents(vertexCount);
					for (int k = 0; k < list.Count; k++)
					{
						DeformLayer deformLayer = list[k];
						this._clonedSharedMesh.AddBlendShapeFrame("kkse_" + deformLayer.Id, 100f, deformLayer.Deltas, zeroNormals, zeroTangents);
					}
				}
				this._clonedSharedMesh.RecalculateBounds();
				if (this._smr != null)
				{
					this._smr.sharedMesh = null;
					this._smr.sharedMesh = this._clonedSharedMesh;
					if (flag2)
					{
						for (int l = 0; l < list.Count; l++)
						{
							DeformLayer deformLayer2 = list[l];
							int blendShapeIndex = this._clonedSharedMesh.GetBlendShapeIndex("kkse_" + deformLayer2.Id);
							if (blendShapeIndex >= 0)
							{
								this._smr.SetBlendShapeWeight(blendShapeIndex, deformLayer2.Weight * 100f);
							}
						}
					}
					BlendShapeRebuildCache.RestoreSmrWeights(this._smr, this._clonedSharedMesh);
				}
				if (flag2)
				{
					this._cachedFinalDeltas = this.DeformData.ComputeFinalDelta();
					this._lastVertsHash = ShapeDeformer.ComputeVertsHash(this._cachedFinalDeltas);
				}
				else
				{
					this._cachedFinalDeltas = null;
					this._lastVertsHash = 0;
				}
				this._hasProducedDisplayMesh = flag2;
				this.RewriteTrianglesWithMask(this._clonedSharedMesh);
			}
			finally
			{
			}
			Action onDeformationApplied = this.OnDeformationApplied;
			if (onDeformationApplied == null)
			{
				return;
			}
			onDeformationApplied();
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000A370 File Offset: 0x00008570
		public Mesh RequestPosedMesh()
		{
			int frameCount = Time.frameCount;
			if (this._displayMeshRef != null && this._posedMeshFrame == frameCount)
			{
				return this._displayMeshRef;
			}
			if (this._isStatic)
			{
				this._displayMeshRef = this._clonedSharedMesh;
				this._posedMeshFrame = frameCount;
				return this._displayMeshRef;
			}
			if (this._smr == null)
			{
				return null;
			}
			bool flag = this._lastVertsHash != this._lastBakedVertsHash;
			bool flag2 = Time.unscaledTime - this._lastBakeTime >= 0.05f;
			bool flag3 = this._posedMesh != null && this._clonedSharedMesh != null && this._posedMesh.vertexCount != this._clonedSharedMesh.vertexCount;
			if (this._posedMesh != null && !this._forcePosedRebake && !flag && !flag3 && !flag2)
			{
				this._displayMeshRef = this._posedMesh;
				this._posedMeshFrame = frameCount;
				return this._displayMeshRef;
			}
			if (this._posedMesh == null)
			{
				this._posedMesh = new Mesh();
			}
			try
			{
				bool enabled = this._smr.enabled;
				if (!enabled)
				{
					this._smr.enabled = true;
				}
				this._smr.BakeMesh(this._posedMesh);
				if (!enabled)
				{
					this._smr.enabled = false;
				}
				this.UndoBakedScale(this._posedMesh, this._smr.transform.lossyScale);
				this._posedMesh.RecalculateBounds();
			}
			finally
			{
			}
			this._lastBakedVertsHash = this._lastVertsHash;
			this._lastBakeTime = Time.unscaledTime;
			this._lastRebakeFrame = frameCount;
			this._forcePosedRebake = false;
			this._displayMeshRef = this._posedMesh;
			this._posedMeshFrame = frameCount;
			return this._displayMeshRef;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000A53C File Offset: 0x0000873C
		private void UndoBakedScale(Mesh mesh, Vector3 scale)
		{
			if (Mathf.Abs(scale.x - 1f) < 0.001f && Mathf.Abs(scale.y - 1f) < 0.001f && Mathf.Abs(scale.z - 1f) < 0.001f)
			{
				return;
			}
			mesh.GetVertices(this._bakeVertsBuffer);
			int count = this._bakeVertsBuffer.Count;
			for (int i = 0; i < count; i++)
			{
				Vector3 vector = this._bakeVertsBuffer[i];
				vector.x /= scale.x;
				vector.y /= scale.y;
				vector.z /= scale.z;
				this._bakeVertsBuffer[i] = vector;
			}
			if (count > 0)
			{
				mesh.SetVertices(this._bakeVertsBuffer);
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000A614 File Offset: 0x00008814
		private void ComputeBoneMatrices()
		{
			if (this._smrBones == null || this._bindPoses == null || this._boneMatrices == null)
			{
				return;
			}
			Matrix4x4 matrix4x = ((this._smr != null && !this._studioMode) ? this._smr.transform.worldToLocalMatrix : Matrix4x4.identity);
			int num = this._smrBones.Length;
			if (this._bindPoses.Length < num)
			{
				num = this._bindPoses.Length;
			}
			for (int i = 0; i < num; i++)
			{
				if (this._smrBones[i] != null)
				{
					this._boneMatrices[i] = matrix4x * this._smrBones[i].localToWorldMatrix * this._bindPoses[i];
				}
				else
				{
					this._boneMatrices[i] = this._bindPoses[i];
				}
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000A6EC File Offset: 0x000088EC
		public bool WorldDeltaToBindDelta(int vertexIdx, Vector3 worldDisp, out Vector3 bindDelta)
		{
			Transform displayTransform = this.DisplayTransform;
			if (!this._isRemapped && this._clonedSharedMesh != null && this._activeBoneWeights != null && this._activeBoneWeights.Length != this._clonedSharedMesh.vertexCount)
			{
				this._pristineBoneWeights = this._clonedSharedMesh.boneWeights;
				this._activeBoneWeights = this._pristineBoneWeights;
			}
			if (this._activeBoneWeights == null || this._boneMatrices == null || vertexIdx < 0 || vertexIdx >= this._activeBoneWeights.Length)
			{
				bindDelta = ((displayTransform != null) ? displayTransform.InverseTransformVector(worldDisp) : worldDisp);
				return false;
			}
			if (this._boneMatricesFrame != Time.frameCount)
			{
				this.ComputeBoneMatrices();
				this._boneMatricesFrame = Time.frameCount;
			}
			BoneWeight boneWeight = this._activeBoneWeights[vertexIdx];
			Matrix4x4 zero = Matrix4x4.zero;
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex0, boneWeight.weight0);
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex1, boneWeight.weight1);
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex2, boneWeight.weight2);
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex3, boneWeight.weight3);
			Vector3 vector = (this._studioMode ? worldDisp : ((displayTransform != null) ? displayTransform.InverseTransformVector(worldDisp) : worldDisp));
			bindDelta = zero.inverse.MultiplyVector(vector);
			return true;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000A84C File Offset: 0x00008A4C
		public bool BindDeltaToWorld(int vertexIdx, Vector3 bindDelta, out Vector3 worldDisp)
		{
			Transform displayTransform = this.DisplayTransform;
			if (!this._isRemapped && this._clonedSharedMesh != null && this._activeBoneWeights != null && this._activeBoneWeights.Length != this._clonedSharedMesh.vertexCount)
			{
				this._pristineBoneWeights = this._clonedSharedMesh.boneWeights;
				this._activeBoneWeights = this._pristineBoneWeights;
			}
			if (this._activeBoneWeights == null || this._boneMatrices == null || vertexIdx < 0 || vertexIdx >= this._activeBoneWeights.Length)
			{
				worldDisp = ((displayTransform != null) ? displayTransform.TransformVector(bindDelta) : bindDelta);
				return false;
			}
			if (this._boneMatricesFrame != Time.frameCount)
			{
				this.ComputeBoneMatrices();
				this._boneMatricesFrame = Time.frameCount;
			}
			BoneWeight boneWeight = this._activeBoneWeights[vertexIdx];
			Matrix4x4 zero = Matrix4x4.zero;
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex0, boneWeight.weight0);
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex1, boneWeight.weight1);
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex2, boneWeight.weight2);
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex3, boneWeight.weight3);
			Vector3 vector = zero.MultiplyVector(bindDelta);
			worldDisp = (this._studioMode ? vector : ((displayTransform != null) ? displayTransform.TransformVector(vector) : vector));
			return true;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000A9A4 File Offset: 0x00008BA4
		public void LogSkinningDiagnostic(int vertexIdx, Vector3 worldDispSample, Transform xform)
		{
			if (this._activeBoneWeights == null || this._boneMatrices == null)
			{
				ShapeEditorPlugin.Logger.LogInfo(string.Format("[Diag] v={0}: no bone data (static / no rig)", vertexIdx));
				return;
			}
			if (vertexIdx < 0 || vertexIdx >= this._activeBoneWeights.Length)
			{
				ShapeEditorPlugin.Logger.LogInfo(string.Format("[Diag] v={0}: out of range ({1})", vertexIdx, this._activeBoneWeights.Length));
				return;
			}
			BoneWeight boneWeight = this._activeBoneWeights[vertexIdx];
			Matrix4x4 zero = Matrix4x4.zero;
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex0, boneWeight.weight0);
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex1, boneWeight.weight1);
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex2, boneWeight.weight2);
			this.AccumulateBoneMatrix(ref zero, boneWeight.boneIndex3, boneWeight.weight3);
			Vector3 vector = zero.MultiplyVector(Vector3.right);
			Vector3 vector2 = zero.MultiplyVector(Vector3.up);
			Vector3 vector3 = zero.MultiplyVector(Vector3.forward);
			Vector3 vector4 = ((xform != null) ? xform.InverseTransformVector(worldDispSample) : worldDispSample);
			Vector3 vector5 = zero.MultiplyVector(vector4);
			float magnitude = vector4.magnitude;
			float num = ((magnitude > 1E-06f) ? (vector5.magnitude / magnitude) : 0f);
			float num2 = Vector3.Angle(vector5, vector4);
			ShapeEditorPlugin.Logger.LogInfo(string.Concat(new string[]
			{
				string.Format("[Diag] v={0} renderer={1} studio={2}\n", vertexIdx, (this._smr != null) ? this._smr.name : "?", this._studioMode),
				string.Format("  bones: {0}({1:F2}) {2}({3:F2}) {4}({5:F2}) {6}({7:F2})\n", new object[]
				{
					this.BoneName(boneWeight.boneIndex0),
					boneWeight.weight0,
					this.BoneName(boneWeight.boneIndex1),
					boneWeight.weight1,
					this.BoneName(boneWeight.boneIndex2),
					boneWeight.weight2,
					this.BoneName(boneWeight.boneIndex3),
					boneWeight.weight3
				}),
				string.Format("  M*X={0} len={1:F3}\n", vector, vector.magnitude),
				string.Format("  M*Y={0} len={1:F3}\n", vector2, vector2.magnitude),
				string.Format("  M*Z={0} len={1:F3}\n", vector3, vector3.magnitude),
				string.Format("  worldDisp={0} len={1:F4}\n", worldDispSample, worldDispSample.magnitude),
				string.Format("  storedDelta(smrLocal)={0} len={1:F4}\n", vector4, magnitude),
				string.Format("  applied(M*delta)={0} len={1:F4}\n", vector5, vector5.magnitude),
				string.Format("  applied/stored: ratio={0:F3} angleDeg={1:F1}", num, num2)
			}));
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000ACAC File Offset: 0x00008EAC
		private string BoneName(int boneIdx)
		{
			if (this._smrBones == null || boneIdx < 0 || boneIdx >= this._smrBones.Length)
			{
				return "?";
			}
			if (!(this._smrBones[boneIdx] != null))
			{
				return "?";
			}
			return this._smrBones[boneIdx].name;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000ACFC File Offset: 0x00008EFC
		private void AccumulateBoneMatrix(ref Matrix4x4 M, int boneIdx, float w)
		{
			if (w <= 0f || this._boneMatrices == null || boneIdx < 0 || boneIdx >= this._boneMatrices.Length)
			{
				return;
			}
			Matrix4x4 matrix4x = this._boneMatrices[boneIdx];
			M.m00 += matrix4x.m00 * w;
			M.m01 += matrix4x.m01 * w;
			M.m02 += matrix4x.m02 * w;
			M.m03 += matrix4x.m03 * w;
			M.m10 += matrix4x.m10 * w;
			M.m11 += matrix4x.m11 * w;
			M.m12 += matrix4x.m12 * w;
			M.m13 += matrix4x.m13 * w;
			M.m20 += matrix4x.m20 * w;
			M.m21 += matrix4x.m21 * w;
			M.m22 += matrix4x.m22 * w;
			M.m23 += matrix4x.m23 * w;
			M.m30 += matrix4x.m30 * w;
			M.m31 += matrix4x.m31 * w;
			M.m32 += matrix4x.m32 * w;
			M.m33 += matrix4x.m33 * w;
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000129 RID: 297 RVA: 0x0000AE56 File Offset: 0x00009056
		private Renderer SourceRenderer
		{
			get
			{
				if (!(this._smr != null))
				{
					return this._sourceMeshRenderer;
				}
				return this._smr;
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000AE74 File Offset: 0x00009074
		private void RestoreOriginalMaterials()
		{
			if (this._originalMaterials == null)
			{
				return;
			}
			Renderer sourceRenderer = this.SourceRenderer;
			if (sourceRenderer != null)
			{
				sourceRenderer.sharedMaterials = this._originalMaterials;
			}
			this._originalMaterials = null;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000AEB0 File Offset: 0x000090B0
		private void CleanupPreviousInitState()
		{
			this.RestoreOriginalMaterials();
			this._editMaterialApplied = false;
			if (this._smr != null && !this._isStatic && this._clonedSharedMesh != null)
			{
				ShapeDeformer.ClearKkseFramesIfAny(this._clonedSharedMesh, this._smr);
			}
			this._clonedSharedMesh = null;
			this._pristineBoneWeights = null;
			this._activeBoneWeights = null;
			this._isRemapped = false;
			this._lastInPlaceVertCount = 0;
			if (this._posedMesh != null)
			{
				UnityEngine.Object.DestroyImmediate(this._posedMesh);
			}
			this._posedMesh = null;
			this._posedMeshFrame = -1;
			this._displayMeshRef = null;
			this._hasProducedDisplayMesh = false;
			this._restColors = null;
			this._restVertices = null;
			this._staticWorkingVerts = null;
			this._bindPoses = null;
			this._smrBones = null;
			this._boneMatrices = null;
			this._boneMatricesFrame = -1;
			this._cachedFinalDeltas = null;
			this._lastVertsHash = 0;
			this._loggedDeltaMismatch = false;
			this._loggedDeletedFaceMismatch = false;
			this._origTrianglesPerSubmesh = null;
			this._vertexLive = null;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000AFB4 File Offset: 0x000091B4
		private static void ClearKkseFramesIfAny(Mesh mesh, SkinnedMeshRenderer smr)
		{
			if (mesh == null)
			{
				return;
			}
			bool flag = false;
			int blendShapeCount = mesh.blendShapeCount;
			for (int i = 0; i < blendShapeCount; i++)
			{
				string blendShapeName = mesh.GetBlendShapeName(i);
				if (blendShapeName != null && blendShapeName.StartsWith("kkse_"))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			if (smr != null)
			{
				BlendShapeRebuildCache.CacheSmrWeights(smr, mesh);
			}
			BlendShapeRebuildCache.Cache(mesh);
			mesh.ClearBlendShapes();
			BlendShapeRebuildCache.Restore(mesh);
			if (smr != null)
			{
				smr.sharedMesh = null;
				smr.sharedMesh = mesh;
				BlendShapeRebuildCache.RestoreSmrWeights(smr, mesh);
			}
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000B040 File Offset: 0x00009240
		private void OnDestroy()
		{
			this.RestoreOriginalMaterials();
			this._editMaterialApplied = false;
			this._editMode = false;
			this._editMaterial = null;
			this._hasProducedDisplayMesh = false;
			this._isRemapped = false;
			if (this._smr != null && !this._isStatic && this._clonedSharedMesh != null)
			{
				ShapeDeformer.ClearKkseFramesIfAny(this._clonedSharedMesh, this._smr);
			}
			if (this._posedMesh != null)
			{
				UnityEngine.Object.DestroyImmediate(this._posedMesh);
			}
			this._posedMesh = null;
			this._displayMeshRef = null;
			this._clonedSharedMesh = null;
			this._pristineBoneWeights = null;
			this._activeBoneWeights = null;
			this._lastInPlaceVertCount = 0;
		}

		// Token: 0x04000075 RID: 117
		public const string DisplayGoSuffix = "_kkse";

		// Token: 0x04000076 RID: 118
		private static readonly HashSet<int> _loggedProbeFailureMeshIds = new HashSet<int>();

		// Token: 0x04000077 RID: 119
		private SkinnedMeshRenderer _smr;

		// Token: 0x04000078 RID: 120
		private MeshFilter _sourceMeshFilter;

		// Token: 0x04000079 RID: 121
		private MeshRenderer _sourceMeshRenderer;

		// Token: 0x0400007A RID: 122
		private bool _isStatic;

		// Token: 0x0400007B RID: 123
		private bool _studioMode;

		// Token: 0x0400007C RID: 124
		private Mesh _clonedSharedMesh;

		// Token: 0x0400007D RID: 125
		private BoneWeight[] _pristineBoneWeights;

		// Token: 0x0400007E RID: 126
		private Color[] _restColors;

		// Token: 0x0400007F RID: 127
		private int _sourceMeshId;

		// Token: 0x04000080 RID: 128
		private int _lastInPlaceVertCount;

		// Token: 0x04000081 RID: 129
		private Vector3[] _restVertices;

		// Token: 0x04000082 RID: 130
		private Vector3[] _staticWorkingVerts;

		// Token: 0x04000084 RID: 132
		private Vector3[] _cachedFinalDeltas;

		// Token: 0x04000085 RID: 133
		private bool _forceSyncOnce;

		// Token: 0x04000086 RID: 134
		private bool _hasProducedDisplayMesh;

		// Token: 0x04000087 RID: 135
		private int _lastVertsHash;

		// Token: 0x04000088 RID: 136
		private bool _editMode;

		// Token: 0x04000089 RID: 137
		private Material _editMaterial;

		// Token: 0x0400008A RID: 138
		private Material[] _originalMaterials;

		// Token: 0x0400008B RID: 139
		private bool _editMaterialApplied;

		// Token: 0x0400008C RID: 140
		private Mesh _posedMesh;

		// Token: 0x0400008D RID: 141
		private int _posedMeshFrame = -1;

		// Token: 0x0400008E RID: 142
		private Mesh _displayMeshRef;

		// Token: 0x0400008F RID: 143
		private int _lastBakedVertsHash;

		// Token: 0x04000090 RID: 144
		private float _lastBakeTime = -1f;

		// Token: 0x04000091 RID: 145
		private int _lastRebakeFrame = -1;

		// Token: 0x04000092 RID: 146
		private bool _forcePosedRebake;

		// Token: 0x04000093 RID: 147
		private const float PosedMeshRebakeInterval = 0.05f;

		// Token: 0x04000094 RID: 148
		private readonly List<Vector3> _bakeVertsBuffer = new List<Vector3>();

		// Token: 0x04000095 RID: 149
		private BoneWeight[] _activeBoneWeights;

		// Token: 0x04000096 RID: 150
		private Matrix4x4[] _bindPoses;

		// Token: 0x04000097 RID: 151
		private Transform[] _smrBones;

		// Token: 0x04000098 RID: 152
		private Matrix4x4[] _boneMatrices;

		// Token: 0x04000099 RID: 153
		private int _boneMatricesFrame = -1;

		// Token: 0x0400009A RID: 154
		private bool _isRemapped;

		// Token: 0x0400009B RID: 155
		private bool _loggedDeltaMismatch;

		// Token: 0x0400009C RID: 156
		private bool _loggedDeletedFaceMismatch;

		// Token: 0x0400009D RID: 157
		private int[] _triRewriteBuf;

		// Token: 0x0400009E RID: 158
		private bool[] _vertexLive;

		// Token: 0x0400009F RID: 159
		private int[][] _origTrianglesPerSubmesh;

		// Token: 0x040000A0 RID: 160
		public Action OnDeformationApplied;

		// Token: 0x040000A1 RID: 161
		public Action OnMeshReplaced;
	}
}
