using System;
using System.Collections.Generic;

namespace KKShapeEditor
{
	// Token: 0x02000014 RID: 20
	public class PresetEntry
	{
		// Token: 0x060000CE RID: 206 RVA: 0x00007568 File Offset: 0x00005768
		public PresetEntry()
		{
			this.RendererPath = "";
			this.FacesPerLevel = new List<int[]>();
			this.SmoothPerLevel = new bool[0];
			this.Layers = new List<DeformLayer>();
			this.PsdDrivers = new List<PsdDriver>();
			this.CsbDrivers = new List<CsbDriver>();
			this.NbbDrivers = new List<NbbDriver>();
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000075CC File Offset: 0x000057CC
		public HashSet<string> CollectLayerIds()
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (this.Layers != null)
			{
				for (int i = 0; i < this.Layers.Count; i++)
				{
					DeformLayer deformLayer = this.Layers[i];
					if (deformLayer != null && !string.IsNullOrEmpty(deformLayer.Id))
					{
						hashSet.Add(deformLayer.Id);
					}
				}
			}
			return hashSet;
		}

		// Token: 0x04000042 RID: 66
		public string RendererPath;

		// Token: 0x04000043 RID: 67
		public int SubdivLevel;

		// Token: 0x04000044 RID: 68
		public int VertexCount;

		// Token: 0x04000045 RID: 69
		public List<int[]> FacesPerLevel;

		// Token: 0x04000046 RID: 70
		public bool[] SmoothPerLevel;

		// Token: 0x04000047 RID: 71
		public List<DeformLayer> Layers;

		// Token: 0x04000048 RID: 72
		public int[] DeletedFaces;

		// Token: 0x04000049 RID: 73
		public List<PsdDriver> PsdDrivers;

		// Token: 0x0400004A RID: 74
		public List<CsbDriver> CsbDrivers;

		// Token: 0x0400004B RID: 75
		public List<NbbDriver> NbbDrivers;
	}
}
