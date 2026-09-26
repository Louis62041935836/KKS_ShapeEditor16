using System;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000049 RID: 73
	internal static class Theme
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600042B RID: 1067 RVA: 0x0002BB7F File Offset: 0x00029D7F
		public static Texture2D PanelBorderTex
		{
			get
			{
				if (!(Theme._panelBorder != null))
				{
					return Theme._panelBorder = Theme.MakeTex(new Color(0.1f, 0.1f, 0.1f, 1f));
				}
				return Theme._panelBorder;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x0600042C RID: 1068 RVA: 0x0002BBB8 File Offset: 0x00029DB8
		public static Texture2D RowBgEvenTex
		{
			get
			{
				if (!(Theme._rowBgEven != null))
				{
					return Theme._rowBgEven = Theme.MakeTex(new Color(0.2f, 0.2f, 0.2f, 1f));
				}
				return Theme._rowBgEven;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x0600042D RID: 1069 RVA: 0x0002BBF1 File Offset: 0x00029DF1
		public static Texture2D RowBgOddTex
		{
			get
			{
				if (!(Theme._rowBgOdd != null))
				{
					return Theme._rowBgOdd = Theme.MakeTex(new Color(0.24f, 0.24f, 0.24f, 1f));
				}
				return Theme._rowBgOdd;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600042E RID: 1070 RVA: 0x0002BC2C File Offset: 0x00029E2C
		public static GUIStyle WindowStyle
		{
			get
			{
				if (Theme._windowStyle == null)
				{
					if (Theme._windowBg == null)
					{
						Theme._windowBg = Theme.MakeTex(new Color(0.15f, 0.15f, 0.15f, 0.92f));
					}
					Theme._windowStyle = new GUIStyle(GUI.skin.window);
					Theme._windowStyle.normal.background = Theme._windowBg;
					Theme._windowStyle.onNormal.background = Theme._windowBg;
					Theme._windowStyle.stretchHeight = false;
				}
				return Theme._windowStyle;
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x0002BCBC File Offset: 0x00029EBC
		public static GUIStyle PanelStyle
		{
			get
			{
				if (Theme._panelStyle == null)
				{
					if (Theme._panelBg == null)
					{
						Theme._panelBg = Theme.MakeTex(new Color(0.2f, 0.2f, 0.2f, 1f));
					}
					Theme._panelStyle = new GUIStyle();
					Theme._panelStyle.normal.background = Theme._panelBg;
					Theme._panelStyle.padding = new RectOffset(4, 4, 4, 4);
				}
				return Theme._panelStyle;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000430 RID: 1072 RVA: 0x0002BD38 File Offset: 0x00029F38
		public static GUIStyle PanelHeaderStyle
		{
			get
			{
				if (Theme._panelHeaderStyle == null)
				{
					if (Theme._panelHeaderBg == null)
					{
						Theme._panelHeaderBg = Theme.MakeTex(new Color(0.13f, 0.13f, 0.13f, 1f));
					}
					Theme._panelHeaderStyle = new GUIStyle(GUI.skin.label);
					Theme._panelHeaderStyle.normal.background = Theme._panelHeaderBg;
					Theme._panelHeaderStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f);
					Theme._panelHeaderStyle.fontStyle = (FontStyle)1;
					Theme._panelHeaderStyle.alignment = (TextAnchor)3;
					Theme._panelHeaderStyle.padding = new RectOffset(6, 4, 1, 1);
					Theme._panelHeaderStyle.fixedHeight = 18f;
				}
				return Theme._panelHeaderStyle;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x0002BE07 File Offset: 0x0002A007
		public static GUIStyle RowLabelStyle
		{
			get
			{
				if (Theme._rowLabelStyle == null)
				{
					Theme._rowLabelStyle = new GUIStyle(GUI.skin.label);
					Theme._rowLabelStyle.alignment = (TextAnchor)3;
				}
				return Theme._rowLabelStyle;
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x0002BE34 File Offset: 0x0002A034
		public static GUIStyle RowEvenStyle
		{
			get
			{
				if (Theme._rowEvenStyle == null)
				{
					Theme._rowEvenStyle = new GUIStyle();
					Theme._rowEvenStyle.normal.background = Theme.RowBgEvenTex;
					Theme._rowEvenStyle.padding = new RectOffset(2, 2, 1, 1);
				}
				return Theme._rowEvenStyle;
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x0002BE73 File Offset: 0x0002A073
		public static GUIStyle RowOddStyle
		{
			get
			{
				if (Theme._rowOddStyle == null)
				{
					Theme._rowOddStyle = new GUIStyle();
					Theme._rowOddStyle.normal.background = Theme.RowBgOddTex;
					Theme._rowOddStyle.padding = new RectOffset(2, 2, 1, 1);
				}
				return Theme._rowOddStyle;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x0002BEB2 File Offset: 0x0002A0B2
		public static GUIStyle SeparatorStyle
		{
			get
			{
				if (Theme._separatorStyle == null)
				{
					Theme._separatorStyle = new GUIStyle();
					Theme._separatorStyle.normal.background = Theme.PanelBorderTex;
					Theme._separatorStyle.margin = new RectOffset(0, 0, 2, 2);
				}
				return Theme._separatorStyle;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000435 RID: 1077 RVA: 0x0002BEF4 File Offset: 0x0002A0F4
		public static GUIStyle DimHintStyle
		{
			get
			{
				if (Theme._dimHintStyle == null)
				{
					Theme._dimHintStyle = new GUIStyle(GUI.skin.label);
					Theme._dimHintStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
				}
				return Theme._dimHintStyle;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x0002BF44 File Offset: 0x0002A144
		public static GUIStyle BannerStyle
		{
			get
			{
				if (Theme._bannerStyle == null)
				{
					Theme._bannerStyle = new GUIStyle(GUI.skin.box);
					Theme._bannerStyle.normal.textColor = new Color(1f, 0.4f, 0.4f);
					Theme._bannerStyle.fontStyle = (FontStyle)1;
					Theme._bannerStyle.wordWrap = true;
				}
				return Theme._bannerStyle;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000437 RID: 1079 RVA: 0x0002BFAA File Offset: 0x0002A1AA
		public static GUIStyle HelpLabelStyle
		{
			get
			{
				if (Theme._helpLabelStyle == null)
				{
					Theme._helpLabelStyle = new GUIStyle(GUI.skin.label);
					Theme._helpLabelStyle.wordWrap = true;
				}
				return Theme._helpLabelStyle;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x0002BFD7 File Offset: 0x0002A1D7
		public static Texture2D SliderTrackBgTex
		{
			get
			{
				if (!(Theme._sliderTrackBg != null))
				{
					return Theme._sliderTrackBg = Theme.MakeTex(new Color(0.18f, 0.18f, 0.18f, 1f));
				}
				return Theme._sliderTrackBg;
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000439 RID: 1081 RVA: 0x0002C010 File Offset: 0x0002A210
		public static Texture2D SliderFillTex
		{
			get
			{
				if (!(Theme._sliderFill != null))
				{
					return Theme._sliderFill = Theme.MakeTex(new Color(0.35f, 0.52f, 0.78f, 1f));
				}
				return Theme._sliderFill;
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x0002C04C File Offset: 0x0002A24C
		public static GUIStyle SliderLabelLeft
		{
			get
			{
				if (Theme._sliderLabelLeft == null)
				{
					Theme._sliderLabelLeft = new GUIStyle(GUI.skin.label);
					Theme._sliderLabelLeft.alignment = (TextAnchor)3;
					Theme._sliderLabelLeft.normal.textColor = new Color(0.92f, 0.92f, 0.92f);
					Theme._sliderLabelLeft.padding = new RectOffset(8, 4, 0, 0);
					Theme._sliderLabelLeft.clipping = (TextClipping)1;
				}
				return Theme._sliderLabelLeft;
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x0600043B RID: 1083 RVA: 0x0002C0C8 File Offset: 0x0002A2C8
		public static GUIStyle SliderValueRight
		{
			get
			{
				if (Theme._sliderValueRight == null)
				{
					Theme._sliderValueRight = new GUIStyle(GUI.skin.label);
					Theme._sliderValueRight.alignment = (TextAnchor)5;
					Theme._sliderValueRight.normal.textColor = new Color(0.92f, 0.92f, 0.92f);
					Theme._sliderValueRight.padding = new RectOffset(4, 8, 0, 0);
				}
				return Theme._sliderValueRight;
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x0002C136 File Offset: 0x0002A336
		public static GUIStyle RowTextFieldStyle
		{
			get
			{
				if (Theme._rowTextFieldStyle == null)
				{
					Theme._rowTextFieldStyle = new GUIStyle(GUI.skin.textField);
					Theme._rowTextFieldStyle.fixedHeight = 22f;
					Theme._rowTextFieldStyle.alignment = (TextAnchor)3;
				}
				return Theme._rowTextFieldStyle;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x0600043D RID: 1085 RVA: 0x0002C174 File Offset: 0x0002A374
		public static GUIStyle RowButtonStyle
		{
			get
			{
				if (Theme._rowButtonStyle == null)
				{
					Theme._rowButtonStyle = new GUIStyle(GUI.skin.button);
					Theme._rowButtonStyle.alignment = (TextAnchor)4;
					Theme._rowButtonStyle.padding = new RectOffset(4, 4, 2, 2);
					Theme._rowButtonStyle.stretchWidth = true;
				}
				return Theme._rowButtonStyle;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x0002C1CC File Offset: 0x0002A3CC
		public static GUIStyle FlatButtonStyle
		{
			get
			{
				if (Theme._flatButtonStyle == null)
				{
					if (Theme._btnNormal == null)
					{
						Theme._btnNormal = Theme.MakeTex(new Color(0.28f, 0.28f, 0.28f, 1f));
					}
					if (Theme._btnHover == null)
					{
						Theme._btnHover = Theme.MakeTex(new Color(0.34f, 0.34f, 0.34f, 1f));
					}
					if (Theme._btnActive == null)
					{
						Theme._btnActive = Theme.MakeTex(new Color(0.4f, 0.4f, 0.4f, 1f));
					}
					if (Theme._btnOnNormal == null)
					{
						Theme._btnOnNormal = Theme.MakeTex(new Color(0.35f, 0.52f, 0.78f, 1f));
					}
					if (Theme._btnOnHover == null)
					{
						Theme._btnOnHover = Theme.MakeTex(new Color(0.42f, 0.58f, 0.83f, 1f));
					}
					Theme._flatButtonStyle = new GUIStyle(GUI.skin.button);
					Theme._flatButtonStyle.name = "Button";
					Theme._flatButtonStyle.normal.background = Theme._btnNormal;
					Theme._flatButtonStyle.hover.background = Theme._btnHover;
					Theme._flatButtonStyle.active.background = Theme._btnActive;
					Theme._flatButtonStyle.focused.background = Theme._btnNormal;
					Theme._flatButtonStyle.onNormal.background = Theme._btnOnNormal;
					Theme._flatButtonStyle.onHover.background = Theme._btnOnHover;
					Theme._flatButtonStyle.onActive.background = Theme._btnOnHover;
					Theme._flatButtonStyle.onFocused.background = Theme._btnOnNormal;
					Color color;
					color = new Color(0.92f, 0.92f, 0.92f);
					Color color2;
					color2 = new Color(1f, 1f, 1f);
					Theme._flatButtonStyle.normal.textColor = color;
					Theme._flatButtonStyle.hover.textColor = color2;
					Theme._flatButtonStyle.active.textColor = color2;
					Theme._flatButtonStyle.focused.textColor = color;
					Theme._flatButtonStyle.onNormal.textColor = color2;
					Theme._flatButtonStyle.onHover.textColor = color2;
					Theme._flatButtonStyle.onActive.textColor = color2;
					Theme._flatButtonStyle.onFocused.textColor = color2;
					Theme._flatButtonStyle.border = new RectOffset(0, 0, 0, 0);
					Theme._flatButtonStyle.padding = new RectOffset(4, 4, 2, 2);
					Theme._flatButtonStyle.alignment = (TextAnchor)4;
				}
				return Theme._flatButtonStyle;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600043F RID: 1087 RVA: 0x0002C478 File Offset: 0x0002A678
		public static GUIStyle FlatScrollbarStyle
		{
			get
			{
				if (Theme._flatScrollbarStyle == null)
				{
					if (Theme._scrollbarTrack == null)
					{
						Theme._scrollbarTrack = Theme.MakeTex(new Color(0.16f, 0.16f, 0.16f, 1f));
					}
					Theme._flatScrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
					Theme._flatScrollbarStyle.normal.background = Theme._scrollbarTrack;
					Theme._flatScrollbarStyle.hover.background = Theme._scrollbarTrack;
					Theme._flatScrollbarStyle.active.background = Theme._scrollbarTrack;
					Theme._flatScrollbarStyle.focused.background = Theme._scrollbarTrack;
					Theme._flatScrollbarStyle.border = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarStyle.padding = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarStyle.margin = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarStyle.fixedWidth = 12f;
				}
				return Theme._flatScrollbarStyle;
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x0002C570 File Offset: 0x0002A770
		public static GUIStyle FlatScrollbarThumbStyle
		{
			get
			{
				if (Theme._flatScrollbarThumbStyle == null)
				{
					if (Theme._scrollbarThumb == null)
					{
						Theme._scrollbarThumb = Theme.MakeTex(new Color(0.4f, 0.4f, 0.4f, 1f));
					}
					if (Theme._scrollbarThumbHover == null)
					{
						Theme._scrollbarThumbHover = Theme.MakeTex(new Color(0.55f, 0.55f, 0.55f, 1f));
					}
					Theme._flatScrollbarThumbStyle = new GUIStyle(GUI.skin.verticalScrollbarThumb);
					Theme._flatScrollbarThumbStyle.normal.background = Theme._scrollbarThumb;
					Theme._flatScrollbarThumbStyle.hover.background = Theme._scrollbarThumbHover;
					Theme._flatScrollbarThumbStyle.active.background = Theme._scrollbarThumbHover;
					Theme._flatScrollbarThumbStyle.focused.background = Theme._scrollbarThumbHover;
					Theme._flatScrollbarThumbStyle.border = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarThumbStyle.padding = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarThumbStyle.margin = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarThumbStyle.fixedWidth = 12f;
				}
				return Theme._flatScrollbarThumbStyle;
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000441 RID: 1089 RVA: 0x0002C698 File Offset: 0x0002A898
		public static GUIStyle FlatScrollbarHorizontalStyle
		{
			get
			{
				if (Theme._flatScrollbarHorizontalStyle == null)
				{
					if (Theme._scrollbarTrack == null)
					{
						Theme._scrollbarTrack = Theme.MakeTex(new Color(0.16f, 0.16f, 0.16f, 1f));
					}
					Theme._flatScrollbarHorizontalStyle = new GUIStyle(GUI.skin.horizontalScrollbar);
					Theme._flatScrollbarHorizontalStyle.normal.background = Theme._scrollbarTrack;
					Theme._flatScrollbarHorizontalStyle.hover.background = Theme._scrollbarTrack;
					Theme._flatScrollbarHorizontalStyle.active.background = Theme._scrollbarTrack;
					Theme._flatScrollbarHorizontalStyle.focused.background = Theme._scrollbarTrack;
					Theme._flatScrollbarHorizontalStyle.border = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarHorizontalStyle.padding = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarHorizontalStyle.margin = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarHorizontalStyle.fixedHeight = 12f;
				}
				return Theme._flatScrollbarHorizontalStyle;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x0002C790 File Offset: 0x0002A990
		public static GUIStyle FlatScrollbarHorizontalThumbStyle
		{
			get
			{
				if (Theme._flatScrollbarHorizontalThumbStyle == null)
				{
					if (Theme._scrollbarThumb == null)
					{
						Theme._scrollbarThumb = Theme.MakeTex(new Color(0.4f, 0.4f, 0.4f, 1f));
					}
					if (Theme._scrollbarThumbHover == null)
					{
						Theme._scrollbarThumbHover = Theme.MakeTex(new Color(0.55f, 0.55f, 0.55f, 1f));
					}
					Theme._flatScrollbarHorizontalThumbStyle = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
					Theme._flatScrollbarHorizontalThumbStyle.normal.background = Theme._scrollbarThumb;
					Theme._flatScrollbarHorizontalThumbStyle.hover.background = Theme._scrollbarThumbHover;
					Theme._flatScrollbarHorizontalThumbStyle.active.background = Theme._scrollbarThumbHover;
					Theme._flatScrollbarHorizontalThumbStyle.focused.background = Theme._scrollbarThumbHover;
					Theme._flatScrollbarHorizontalThumbStyle.border = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarHorizontalThumbStyle.padding = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarHorizontalThumbStyle.margin = new RectOffset(0, 0, 0, 0);
					Theme._flatScrollbarHorizontalThumbStyle.fixedHeight = 12f;
				}
				return Theme._flatScrollbarHorizontalThumbStyle;
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000443 RID: 1091 RVA: 0x0002C8B8 File Offset: 0x0002AAB8
		public static GUIStyle FlatScrollbarButtonStyle
		{
			get
			{
				if (Theme._flatScrollbarButtonStyle == null)
				{
					Theme._flatScrollbarButtonStyle = new GUIStyle();
					Theme._flatScrollbarButtonStyle.fixedHeight = 0f;
					Theme._flatScrollbarButtonStyle.fixedWidth = 0f;
				}
				return Theme._flatScrollbarButtonStyle;
			}
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x0002C8F0 File Offset: 0x0002AAF0
		public static void Destroy()
		{
			Theme.DestroyTex(ref Theme._windowBg);
			Theme.DestroyTex(ref Theme._panelBg);
			Theme.DestroyTex(ref Theme._panelHeaderBg);
			Theme.DestroyTex(ref Theme._panelBorder);
			Theme.DestroyTex(ref Theme._rowBgEven);
			Theme.DestroyTex(ref Theme._rowBgOdd);
			Theme.DestroyTex(ref Theme._sliderTrackBg);
			Theme.DestroyTex(ref Theme._sliderFill);
			Theme.DestroyTex(ref Theme._btnNormal);
			Theme.DestroyTex(ref Theme._btnHover);
			Theme.DestroyTex(ref Theme._btnActive);
			Theme.DestroyTex(ref Theme._btnOnNormal);
			Theme.DestroyTex(ref Theme._btnOnHover);
			Theme.DestroyTex(ref Theme._scrollbarTrack);
			Theme.DestroyTex(ref Theme._scrollbarThumb);
			Theme.DestroyTex(ref Theme._scrollbarThumbHover);
			Theme._windowStyle = null;
			Theme._panelStyle = null;
			Theme._panelHeaderStyle = null;
			Theme._rowLabelStyle = null;
			Theme._rowEvenStyle = null;
			Theme._rowOddStyle = null;
			Theme._separatorStyle = null;
			Theme._dimHintStyle = null;
			Theme._bannerStyle = null;
			Theme._helpLabelStyle = null;
			Theme._sliderLabelLeft = null;
			Theme._sliderValueRight = null;
			Theme._rowTextFieldStyle = null;
			Theme._rowButtonStyle = null;
			Theme._flatButtonStyle = null;
			Theme._flatScrollbarStyle = null;
			Theme._flatScrollbarThumbStyle = null;
			Theme._flatScrollbarHorizontalStyle = null;
			Theme._flatScrollbarHorizontalThumbStyle = null;
			Theme._flatScrollbarButtonStyle = null;
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x0002CA15 File Offset: 0x0002AC15
		private static Texture2D MakeTex(Color c)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.SetPixel(0, 0, c);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x0002CA2D File Offset: 0x0002AC2D
		private static void DestroyTex(ref Texture2D t)
		{
			if (t != null)
			{
				UnityEngine.Object.Destroy(t);
			}
			t = null;
		}

		// Token: 0x0400040C RID: 1036
		private static Texture2D _windowBg;

		// Token: 0x0400040D RID: 1037
		private static Texture2D _panelBg;

		// Token: 0x0400040E RID: 1038
		private static Texture2D _panelHeaderBg;

		// Token: 0x0400040F RID: 1039
		private static Texture2D _panelBorder;

		// Token: 0x04000410 RID: 1040
		private static Texture2D _rowBgEven;

		// Token: 0x04000411 RID: 1041
		private static Texture2D _rowBgOdd;

		// Token: 0x04000412 RID: 1042
		private static Texture2D _sliderTrackBg;

		// Token: 0x04000413 RID: 1043
		private static Texture2D _sliderFill;

		// Token: 0x04000414 RID: 1044
		private static Texture2D _btnNormal;

		// Token: 0x04000415 RID: 1045
		private static Texture2D _btnHover;

		// Token: 0x04000416 RID: 1046
		private static Texture2D _btnActive;

		// Token: 0x04000417 RID: 1047
		private static Texture2D _btnOnNormal;

		// Token: 0x04000418 RID: 1048
		private static Texture2D _btnOnHover;

		// Token: 0x04000419 RID: 1049
		private static Texture2D _scrollbarTrack;

		// Token: 0x0400041A RID: 1050
		private static Texture2D _scrollbarThumb;

		// Token: 0x0400041B RID: 1051
		private static Texture2D _scrollbarThumbHover;

		// Token: 0x0400041C RID: 1052
		private static GUIStyle _windowStyle;

		// Token: 0x0400041D RID: 1053
		private static GUIStyle _panelStyle;

		// Token: 0x0400041E RID: 1054
		private static GUIStyle _panelHeaderStyle;

		// Token: 0x0400041F RID: 1055
		private static GUIStyle _rowLabelStyle;

		// Token: 0x04000420 RID: 1056
		private static GUIStyle _rowEvenStyle;

		// Token: 0x04000421 RID: 1057
		private static GUIStyle _rowOddStyle;

		// Token: 0x04000422 RID: 1058
		private static GUIStyle _separatorStyle;

		// Token: 0x04000423 RID: 1059
		private static GUIStyle _dimHintStyle;

		// Token: 0x04000424 RID: 1060
		private static GUIStyle _bannerStyle;

		// Token: 0x04000425 RID: 1061
		private static GUIStyle _helpLabelStyle;

		// Token: 0x04000426 RID: 1062
		private static GUIStyle _sliderLabelLeft;

		// Token: 0x04000427 RID: 1063
		private static GUIStyle _sliderValueRight;

		// Token: 0x04000428 RID: 1064
		private static GUIStyle _rowTextFieldStyle;

		// Token: 0x04000429 RID: 1065
		private static GUIStyle _rowButtonStyle;

		// Token: 0x0400042A RID: 1066
		private static GUIStyle _flatButtonStyle;

		// Token: 0x0400042B RID: 1067
		private static GUIStyle _flatScrollbarStyle;

		// Token: 0x0400042C RID: 1068
		private static GUIStyle _flatScrollbarThumbStyle;

		// Token: 0x0400042D RID: 1069
		private static GUIStyle _flatScrollbarHorizontalStyle;

		// Token: 0x0400042E RID: 1070
		private static GUIStyle _flatScrollbarHorizontalThumbStyle;

		// Token: 0x0400042F RID: 1071
		private static GUIStyle _flatScrollbarButtonStyle;
	}
}
