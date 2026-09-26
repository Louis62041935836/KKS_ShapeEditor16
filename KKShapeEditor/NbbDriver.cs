using System;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000011 RID: 17
	public class NbbDriver
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00006B60 File Offset: 0x00004D60
		// (set) Token: 0x060000AF RID: 175 RVA: 0x00006B68 File Offset: 0x00004D68
		public string SourceRendererPath { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00006B71 File Offset: 0x00004D71
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x00006B79 File Offset: 0x00004D79
		public string SourceShapeName { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00006B82 File Offset: 0x00004D82
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x00006B8A File Offset: 0x00004D8A
		public float InputMin { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00006B93 File Offset: 0x00004D93
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x00006B9B File Offset: 0x00004D9B
		public float InputMax { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00006BA4 File Offset: 0x00004DA4
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00006BAC File Offset: 0x00004DAC
		public string TargetRendererPath { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00006BB5 File Offset: 0x00004DB5
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00006BBD File Offset: 0x00004DBD
		public string TargetLayerId { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00006BC6 File Offset: 0x00004DC6
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00006BCE File Offset: 0x00004DCE
		public bool Enabled { get; set; }

		// Token: 0x060000BC RID: 188 RVA: 0x00006BD8 File Offset: 0x00004DD8
		public NbbDriver()
		{
			this.SourceRendererPath = "";
			this.SourceShapeName = "";
			this.InputMin = 0f;
			this.InputMax = 100f;
			this.TargetRendererPath = "";
			this.TargetLayerId = "";
			this.Enabled = true;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00006C3C File Offset: 0x00004E3C
		internal bool TryResolveSource(Transform root, out SkinnedMeshRenderer smr, out int shapeIndex)
		{
			smr = null;
			shapeIndex = -1;
			if (root == null || string.IsNullOrEmpty(this.SourceShapeName))
			{
				return false;
			}
			SkinnedMeshRenderer skinnedMeshRenderer = ShapeEditorController.FindRendererByPath(root, this.SourceRendererPath) as SkinnedMeshRenderer;
			if (skinnedMeshRenderer == null)
			{
				return false;
			}
			Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
			if (sharedMesh == null)
			{
				return false;
			}
			int instanceID = sharedMesh.GetInstanceID();
			if (instanceID != this._cachedMeshId || this._cachedShapeName != this.SourceShapeName)
			{
				this._cachedShapeIndex = (this.SourceShapeName.StartsWith("kkse_") ? (-1) : sharedMesh.GetBlendShapeIndex(this.SourceShapeName));
				this._cachedMeshId = instanceID;
				this._cachedShapeName = this.SourceShapeName;
			}
			if (this._cachedShapeIndex < 0)
			{
				return false;
			}
			smr = skinnedMeshRenderer;
			shapeIndex = this._cachedShapeIndex;
			return true;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00006D0C File Offset: 0x00004F0C
		internal bool TryReadSourceWeight(Transform root, out float rawWeight)
		{
			rawWeight = 0f;
			SkinnedMeshRenderer skinnedMeshRenderer;
			int num;
			if (!this.TryResolveSource(root, out skinnedMeshRenderer, out num))
			{
				return false;
			}
			rawWeight = skinnedMeshRenderer.GetBlendShapeWeight(num);
			return true;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00006D3C File Offset: 0x00004F3C
		public NbbDriver Clone()
		{
			return new NbbDriver
			{
				SourceRendererPath = this.SourceRendererPath,
				SourceShapeName = this.SourceShapeName,
				InputMin = this.InputMin,
				InputMax = this.InputMax,
				TargetRendererPath = this.TargetRendererPath,
				TargetLayerId = this.TargetLayerId,
				Enabled = this.Enabled
			};
		}

		// Token: 0x0400003E RID: 62
		private int _cachedShapeIndex = -1;

		// Token: 0x0400003F RID: 63
		private int _cachedMeshId;

		// Token: 0x04000040 RID: 64
		private string _cachedShapeName;
	}
}
