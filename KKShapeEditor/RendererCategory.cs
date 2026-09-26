using System;
using System.Globalization;

namespace KKShapeEditor
{
	// Token: 0x0200001A RID: 26
	public static class RendererCategory
	{
		// Token: 0x060000F6 RID: 246 RVA: 0x00008BAC File Offset: 0x00006DAC
		public static string ClothesCategoryKey(int index)
		{
			switch (index)
			{
			case 0:
				return "Top";
			case 1:
				return "Bottom";
			case 2:
				return "Bra";
			case 3:
				return "Underwear";
			case 4:
				return "Gloves";
			case 5:
				return "Pantyhose";
			case 6:
				return "Legwear";
			case 7:
				return "ShoesInner";
			case 8:
				return "ShoesOuter";
			default:
				return "Clothes";
			}
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00008C24 File Offset: 0x00006E24
		public static string HairCategoryKey(int index)
		{
			switch (index)
			{
			case 0:
				return "HairBack";
			case 1:
				return "HairFront";
			case 2:
				return "HairSide";
			case 3:
				return "HairOption";
			default:
				return "Hair";
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00008C68 File Offset: 0x00006E68
		public static string AccessoryCategoryKey(int index)
		{
			return "AccSlot" + (index + 1).ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x04000061 RID: 97
		public const string Top = "Top";

		// Token: 0x04000062 RID: 98
		public const string Bottom = "Bottom";

		// Token: 0x04000063 RID: 99
		public const string Bra = "Bra";

		// Token: 0x04000064 RID: 100
		public const string Underwear = "Underwear";

		// Token: 0x04000065 RID: 101
		public const string Gloves = "Gloves";

		// Token: 0x04000066 RID: 102
		public const string Pantyhose = "Pantyhose";

		// Token: 0x04000067 RID: 103
		public const string Legwear = "Legwear";

		// Token: 0x04000068 RID: 104
		public const string ShoesInner = "ShoesInner";

		// Token: 0x04000069 RID: 105
		public const string ShoesOuter = "ShoesOuter";

		// Token: 0x0400006A RID: 106
		public const string HairBack = "HairBack";

		// Token: 0x0400006B RID: 107
		public const string HairFront = "HairFront";

		// Token: 0x0400006C RID: 108
		public const string HairSide = "HairSide";

		// Token: 0x0400006D RID: 109
		public const string HairOption = "HairOption";

		// Token: 0x0400006E RID: 110
		public const string Body = "Body";

		// Token: 0x0400006F RID: 111
		public const string Head = "Head";

		// Token: 0x04000070 RID: 112
		public const string Clothes = "Clothes";

		// Token: 0x04000071 RID: 113
		public const string Accessory = "Accessory";

		// Token: 0x04000072 RID: 114
		public const string Hair = "Hair";

		// Token: 0x04000073 RID: 115
		public const string Item = "Item";

		// Token: 0x04000074 RID: 116
		public const string AccSlotPrefix = "AccSlot";
	}
}
