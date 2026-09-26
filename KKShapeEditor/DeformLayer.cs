using System;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200000C RID: 12
	public class DeformLayer
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000051 RID: 81 RVA: 0x000039D8 File Offset: 0x00001BD8
		// (set) Token: 0x06000052 RID: 82 RVA: 0x000039E0 File Offset: 0x00001BE0
		public string Id { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000039E9 File Offset: 0x00001BE9
		// (set) Token: 0x06000054 RID: 84 RVA: 0x000039F1 File Offset: 0x00001BF1
		public string Name { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000039FA File Offset: 0x00001BFA
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00003A02 File Offset: 0x00001C02
		public Vector3[] Deltas { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00003A0B File Offset: 0x00001C0B
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00003A13 File Offset: 0x00001C13
		public float Weight { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00003A1C File Offset: 0x00001C1C
		// (set) Token: 0x0600005A RID: 90 RVA: 0x00003A24 File Offset: 0x00001C24
		public bool Dirty { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00003A2D File Offset: 0x00001C2D
		// (set) Token: 0x0600005C RID: 92 RVA: 0x00003A35 File Offset: 0x00001C35
		public int LastTimelineWriteFrame { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00003A3E File Offset: 0x00001C3E
		// (set) Token: 0x0600005E RID: 94 RVA: 0x00003A46 File Offset: 0x00001C46
		public int LastCsbWriteFrame { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00003A4F File Offset: 0x00001C4F
		// (set) Token: 0x06000060 RID: 96 RVA: 0x00003A57 File Offset: 0x00001C57
		public int LastNbbWriteFrame { get; set; }

		// Token: 0x06000061 RID: 97 RVA: 0x00003A60 File Offset: 0x00001C60
		public DeformLayer(string name, int vertexCount)
		{
			this.Id = Guid.NewGuid().ToString("N");
			this.Name = name;
			this.Deltas = new Vector3[vertexCount];
			this.Weight = 1f;
			this.Dirty = false;
			this.LastTimelineWriteFrame = -1;
			this.LastCsbWriteFrame = -1;
			this.LastNbbWriteFrame = -1;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003AC5 File Offset: 0x00001CC5
		public void Reset(int newVertexCount)
		{
			this.Deltas = new Vector3[newVertexCount];
			this.Dirty = true;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003ADC File Offset: 0x00001CDC
		public bool IsEmpty()
		{
			for (int i = 0; i < this.Deltas.Length; i++)
			{
				if (this.Deltas[i].sqrMagnitude > 0f)
				{
					return false;
				}
			}
			return true;
		}
	}
}
