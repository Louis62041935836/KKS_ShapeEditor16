using System;

namespace KKShapeEditor
{
	// Token: 0x02000018 RID: 24
	public class PsdDriver
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x000087CC File Offset: 0x000069CC
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x000087D4 File Offset: 0x000069D4
		public string SourceBonePath { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x000087DD File Offset: 0x000069DD
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x000087E5 File Offset: 0x000069E5
		public PsdChannel Channel { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x000087EE File Offset: 0x000069EE
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x000087F6 File Offset: 0x000069F6
		public float InputMin { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x000087FF File Offset: 0x000069FF
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x00008807 File Offset: 0x00006A07
		public float InputMax { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00008810 File Offset: 0x00006A10
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00008818 File Offset: 0x00006A18
		public string TargetRendererPath { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00008821 File Offset: 0x00006A21
		// (set) Token: 0x060000EB RID: 235 RVA: 0x00008829 File Offset: 0x00006A29
		public string TargetLayerId { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00008832 File Offset: 0x00006A32
		// (set) Token: 0x060000ED RID: 237 RVA: 0x0000883A File Offset: 0x00006A3A
		public bool Enabled { get; set; }

		// Token: 0x060000EE RID: 238 RVA: 0x00008843 File Offset: 0x00006A43
		public PsdDriver()
		{
			this.SourceBonePath = "";
			this.Channel = PsdChannel.RotationX;
			this.TargetRendererPath = "";
			this.TargetLayerId = "";
			this.Enabled = true;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000887C File Offset: 0x00006A7C
		public PsdDriver Clone()
		{
			return new PsdDriver
			{
				SourceBonePath = this.SourceBonePath,
				Channel = this.Channel,
				InputMin = this.InputMin,
				InputMax = this.InputMax,
				TargetRendererPath = this.TargetRendererPath,
				TargetLayerId = this.TargetLayerId,
				Enabled = this.Enabled
			};
		}
	}
}
