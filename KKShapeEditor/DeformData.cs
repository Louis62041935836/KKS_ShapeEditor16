using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200000B RID: 11
	public class DeformData
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00003279 File Offset: 0x00001479
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00003281 File Offset: 0x00001481
		public string RendererPath { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000033 RID: 51 RVA: 0x0000328A File Offset: 0x0000148A
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00003292 File Offset: 0x00001492
		public List<DeformLayer> Layers { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000035 RID: 53 RVA: 0x0000329B File Offset: 0x0000149B
		// (set) Token: 0x06000036 RID: 54 RVA: 0x000032A3 File Offset: 0x000014A3
		public int ActiveLayerIndex { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000032AC File Offset: 0x000014AC
		// (set) Token: 0x06000038 RID: 56 RVA: 0x000032B4 File Offset: 0x000014B4
		public bool WeightRemapped { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000039 RID: 57 RVA: 0x000032BD File Offset: 0x000014BD
		// (set) Token: 0x0600003A RID: 58 RVA: 0x000032C5 File Offset: 0x000014C5
		public HashSet<int> DeletedFaces { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003B RID: 59 RVA: 0x000032CE File Offset: 0x000014CE
		// (set) Token: 0x0600003C RID: 60 RVA: 0x000032D6 File Offset: 0x000014D6
		public bool DeletedFacesDirty { get; set; }

		// Token: 0x0600003D RID: 61 RVA: 0x000032DF File Offset: 0x000014DF
		public DeformData(string rendererPath)
		{
			this.RendererPath = rendererPath;
			this.Layers = new List<DeformLayer>();
			this.ActiveLayerIndex = -1;
			this.DeletedFaces = new HashSet<int>();
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000330B File Offset: 0x0000150B
		public bool AddDeletedFace(int faceIdx)
		{
			if (this.DeletedFaces.Add(faceIdx))
			{
				this.DeletedFacesDirty = true;
				return true;
			}
			return false;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003325 File Offset: 0x00001525
		public bool RemoveDeletedFace(int faceIdx)
		{
			if (this.DeletedFaces.Remove(faceIdx))
			{
				this.DeletedFacesDirty = true;
				return true;
			}
			return false;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x0000333F File Offset: 0x0000153F
		public void ClearDeletedFaces()
		{
			if (this.DeletedFaces.Count == 0)
			{
				return;
			}
			this.DeletedFaces.Clear();
			this.DeletedFacesDirty = true;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003364 File Offset: 0x00001564
		public void UnionDeletedFaces(IEnumerable<int> indices)
		{
			if (indices == null)
			{
				return;
			}
			int count = this.DeletedFaces.Count;
			foreach (int num in indices)
			{
				this.DeletedFaces.Add(num);
			}
			if (this.DeletedFaces.Count != count)
			{
				this.DeletedFacesDirty = true;
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000033D8 File Offset: 0x000015D8
		public void ExceptDeletedFaces(IEnumerable<int> indices)
		{
			if (indices == null || this.DeletedFaces.Count == 0)
			{
				return;
			}
			int count = this.DeletedFaces.Count;
			foreach (int num in indices)
			{
				this.DeletedFaces.Remove(num);
			}
			if (this.DeletedFaces.Count != count)
			{
				this.DeletedFacesDirty = true;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00003458 File Offset: 0x00001658
		public DeformLayer ActiveLayer
		{
			get
			{
				if (this.ActiveLayerIndex >= 0 && this.ActiveLayerIndex < this.Layers.Count)
				{
					return this.Layers[this.ActiveLayerIndex];
				}
				return null;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003489 File Offset: 0x00001689
		public bool HasLayers
		{
			get
			{
				return this.Layers.Count > 0;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00003499 File Offset: 0x00001699
		public bool CanSculpt
		{
			get
			{
				return this.ActiveLayerIndex >= 0 && this.ActiveLayerIndex < this.Layers.Count;
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000034B9 File Offset: 0x000016B9
		public void SetActiveLayer(int index)
		{
			if (index < -1 || index >= this.Layers.Count)
			{
				return;
			}
			this.ActiveLayerIndex = index;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000034D8 File Offset: 0x000016D8
		public Vector3[] ComputeFinalDelta()
		{
			if (this.Layers.Count == 0)
			{
				return null;
			}
			int num = this.Layers[0].Deltas.Length;
			if (this._finalDeltas == null || this._finalDeltas.Length != num)
			{
				this._finalDeltas = new Vector3[num];
			}
			for (int i = 0; i < num; i++)
			{
				this._finalDeltas[i] = Vector3.zero;
			}
			for (int j = 0; j < this.Layers.Count; j++)
			{
				DeformLayer deformLayer = this.Layers[j];
				float weight = deformLayer.Weight;
				if (weight > 0f && deformLayer.Deltas.Length == num)
				{
					Vector3[] deltas = deformLayer.Deltas;
					for (int k = 0; k < num; k++)
					{
						Vector3[] finalDeltas = this._finalDeltas;
						int num2 = k;
						finalDeltas[num2].x = finalDeltas[num2].x + deltas[k].x * weight;
						Vector3[] finalDeltas2 = this._finalDeltas;
						int num3 = k;
						finalDeltas2[num3].y = finalDeltas2[num3].y + deltas[k].y * weight;
						Vector3[] finalDeltas3 = this._finalDeltas;
						int num4 = k;
						finalDeltas3[num4].z = finalDeltas3[num4].z + deltas[k].z * weight;
					}
				}
			}
			for (int l = 0; l < this.Layers.Count; l++)
			{
				this.Layers[l].Dirty = false;
			}
			return this._finalDeltas;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003650 File Offset: 0x00001850
		public bool AnyDirty()
		{
			for (int i = 0; i < this.Layers.Count; i++)
			{
				if (this.Layers[i].Dirty)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000368C File Offset: 0x0000188C
		public DeformLayer AddLayer(int vertexCount)
		{
			DeformLayer deformLayer = new DeformLayer(this.GenerateUniqueLayerName(), vertexCount);
			this.Layers.Add(deformLayer);
			this.ActiveLayerIndex = this.Layers.Count - 1;
			return deformLayer;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000036C8 File Offset: 0x000018C8
		public string GenerateUniqueLayerName()
		{
			string text = L.LayerDefaultNameFmt.Replace("{0}", "");
			int num = 0;
			for (int i = 0; i < this.Layers.Count; i++)
			{
				string name = this.Layers[i].Name;
				int num2;
				if (name != null && name.Length > text.Length && name.StartsWith(text) && int.TryParse(name.Substring(text.Length), out num2) && num2 > num)
				{
					num = num2;
				}
			}
			return string.Format(L.LayerDefaultNameFmt, Mathf.Max(num + 1, this.Layers.Count + 1));
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003770 File Offset: 0x00001970
		public void SetLayerWeight(int index, float weight)
		{
			if (index < 0 || index >= this.Layers.Count)
			{
				return;
			}
			float num = Mathf.Clamp01(weight);
			if (Mathf.Approximately(this.Layers[index].Weight, num))
			{
				return;
			}
			this.Layers[index].Weight = num;
			this.Layers[index].Dirty = true;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000037D5 File Offset: 0x000019D5
		public void RenameLayer(int index, string newName)
		{
			if (index < 0 || index >= this.Layers.Count)
			{
				return;
			}
			this.Layers[index].Name = newName;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000037FC File Offset: 0x000019FC
		public void RemoveLayer(int index)
		{
			if (index < 0 || index >= this.Layers.Count)
			{
				return;
			}
			this.Layers.RemoveAt(index);
			if (this.Layers.Count == 0)
			{
				this.ActiveLayerIndex = -1;
				return;
			}
			if (this.ActiveLayerIndex >= this.Layers.Count)
			{
				this.ActiveLayerIndex = this.Layers.Count - 1;
				return;
			}
			if (this.ActiveLayerIndex > index)
			{
				int activeLayerIndex = this.ActiveLayerIndex;
				this.ActiveLayerIndex = activeLayerIndex - 1;
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003880 File Offset: 0x00001A80
		public void MoveLayerUp(int index)
		{
			if (index <= 0 || index >= this.Layers.Count)
			{
				return;
			}
			DeformLayer deformLayer = this.Layers[index];
			this.Layers[index] = this.Layers[index - 1];
			this.Layers[index - 1] = deformLayer;
			if (this.ActiveLayerIndex == index)
			{
				int num = this.ActiveLayerIndex;
				this.ActiveLayerIndex = num - 1;
				return;
			}
			if (this.ActiveLayerIndex == index - 1)
			{
				int num = this.ActiveLayerIndex;
				this.ActiveLayerIndex = num + 1;
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000390C File Offset: 0x00001B0C
		public void MoveLayerDown(int index)
		{
			if (index < 0 || index >= this.Layers.Count - 1)
			{
				return;
			}
			DeformLayer deformLayer = this.Layers[index];
			this.Layers[index] = this.Layers[index + 1];
			this.Layers[index + 1] = deformLayer;
			if (this.ActiveLayerIndex == index)
			{
				int num = this.ActiveLayerIndex;
				this.ActiveLayerIndex = num + 1;
				return;
			}
			if (this.ActiveLayerIndex == index + 1)
			{
				int num = this.ActiveLayerIndex;
				this.ActiveLayerIndex = num - 1;
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x0000399C File Offset: 0x00001B9C
		public void ResetAllLayers(int newVertexCount)
		{
			for (int i = 0; i < this.Layers.Count; i++)
			{
				this.Layers[i].Reset(newVertexCount);
			}
			this._finalDeltas = null;
		}

		// Token: 0x04000020 RID: 32
		private Vector3[] _finalDeltas;
	}
}
