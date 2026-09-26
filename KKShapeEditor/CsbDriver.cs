using System;

namespace KKShapeEditor
{
	// Token: 0x02000009 RID: 9
	public class CsbDriver
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002EAD File Offset: 0x000010AD
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002EB5 File Offset: 0x000010B5
		public int ClothingKind { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002EBE File Offset: 0x000010BE
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002EC6 File Offset: 0x000010C6
		public float[] StateWeights { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002ECF File Offset: 0x000010CF
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002ED7 File Offset: 0x000010D7
		public float UnequippedWeight { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002EE0 File Offset: 0x000010E0
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002EE8 File Offset: 0x000010E8
		public string TargetRendererPath { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002EF1 File Offset: 0x000010F1
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002EF9 File Offset: 0x000010F9
		public string TargetLayerId { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002F02 File Offset: 0x00001102
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00002F0A File Offset: 0x0000110A
		public bool Enabled { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002F13 File Offset: 0x00001113
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00002F1B File Offset: 0x0000111B
		public int CoordinateScope { get; set; }

		// Token: 0x06000029 RID: 41 RVA: 0x00002F24 File Offset: 0x00001124
		public CsbDriver()
		{
			this.ClothingKind = 0;
			this.StateWeights = new float[4];
			this.StateWeights[0] = 1f;
			this.UnequippedWeight = 0f;
			this.TargetRendererPath = "";
			this.TargetLayerId = "";
			this.Enabled = true;
			this.CoordinateScope = -1;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002F88 File Offset: 0x00001188
		public CsbDriver Clone()
		{
			CsbDriver csbDriver = new CsbDriver();
			csbDriver.ClothingKind = this.ClothingKind;
			csbDriver.StateWeights = this.StateWeights;
			csbDriver.UnequippedWeight = this.UnequippedWeight;
			csbDriver.TargetRendererPath = this.TargetRendererPath;
			csbDriver.TargetLayerId = this.TargetLayerId;
			csbDriver.Enabled = this.Enabled;
			csbDriver.CoordinateScope = this.CoordinateScope;
			csbDriver.NormalizeStateWeights();
			return csbDriver;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002FF4 File Offset: 0x000011F4
		public void NormalizeStateWeights()
		{
			float[] stateWeights = this.StateWeights;
			float[] array = new float[4];
			if (stateWeights != null)
			{
				int num = ((stateWeights.Length < 4) ? stateWeights.Length : 4);
				for (int i = 0; i < num; i++)
				{
					array[i] = CsbDriver.SanitizeWeight(stateWeights[i]);
				}
			}
			this.StateWeights = array;
			this.UnequippedWeight = CsbDriver.SanitizeWeight(this.UnequippedWeight);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000304E File Offset: 0x0000124E
		public static float SanitizeWeight(float w)
		{
			if (float.IsNaN(w) || float.IsInfinity(w))
			{
				return 0f;
			}
			if (w < 0f)
			{
				return 0f;
			}
			if (w > 1f)
			{
				return 1f;
			}
			return w;
		}

		// Token: 0x04000011 RID: 17
		public const int MaxStates = 4;
	}
}
