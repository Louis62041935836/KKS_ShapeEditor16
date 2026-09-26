using System;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200004A RID: 74
	internal static class UI
	{
		// Token: 0x06000447 RID: 1095 RVA: 0x0002CA43 File Offset: 0x0002AC43
		public static void BeginPanel(string title)
		{
			GUILayout.BeginVertical(Theme.PanelStyle, Array.Empty<GUILayoutOption>());
			if (!string.IsNullOrEmpty(title))
			{
				GUILayout.Label(title, Theme.PanelHeaderStyle, Array.Empty<GUILayoutOption>());
			}
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x0002CA6C File Offset: 0x0002AC6C
		public static void EndPanel()
		{
			GUILayout.EndVertical();
			if (Event.current.type == EventType.Repaint)
			{
				UI.DrawBorder(GUILayoutUtility.GetLastRect());
			}
			GUILayout.Space(4f);
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x0002CA94 File Offset: 0x0002AC94
		private static void DrawBorder(Rect r)
		{
			Texture2D panelBorderTex = Theme.PanelBorderTex;
			GUI.DrawTexture(new Rect(r.x, r.y, r.width, 1f), panelBorderTex);
			GUI.DrawTexture(new Rect(r.x, r.yMax - 1f, r.width, 1f), panelBorderTex);
			GUI.DrawTexture(new Rect(r.x, r.y, 1f, r.height), panelBorderTex);
			GUI.DrawTexture(new Rect(r.xMax - 1f, r.y, 1f, r.height), panelBorderTex);
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x0002CB47 File Offset: 0x0002AD47
		public static void DrawToggleBox(Rect box, bool on)
		{
			GUI.DrawTexture(box, on ? Theme.SliderFillTex : Theme.SliderTrackBgTex);
			UI.DrawBorder(box);
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x0002CB64 File Offset: 0x0002AD64
		public static Rect[] SplitRow(int n, float height = 22f, float gap = 2f)
		{
			Rect rect = GUILayoutUtility.GetRect(0f, height, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(height)
			});
			float num = (rect.width - gap * (float)(n - 1)) / (float)n;
			for (int i = 0; i < n; i++)
			{
				UI._splitCells[i] = new Rect(rect.x + (float)i * (num + gap), rect.y, num, height);
			}
			return UI._splitCells;
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x0002CBDF File Offset: 0x0002ADDF
		public static void Row(string label, Action drawField)
		{
			UI.Row(label, 120f, drawField);
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x0002CBED File Offset: 0x0002ADED
		public static void Row(string label, float labelWidth, Action drawField)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(label, Theme.RowLabelStyle, new GUILayoutOption[] { GUILayout.Width(labelWidth) });
			if (drawField != null)
			{
				drawField();
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x0002CC21 File Offset: 0x0002AE21
		public static void BeginRowBg(int index)
		{
			GUILayout.BeginHorizontal(((index & 1) == 0) ? Theme.RowEvenStyle : Theme.RowOddStyle, Array.Empty<GUILayoutOption>());
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x0002CC3E File Offset: 0x0002AE3E
		public static void EndRowBg()
		{
			GUILayout.EndHorizontal();
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x0002CC45 File Offset: 0x0002AE45
		public static void Separator()
		{
			GUILayout.Box(GUIContent.none, Theme.SeparatorStyle, new GUILayoutOption[]
			{
				GUILayout.Height(1f),
				GUILayout.ExpandWidth(true)
			});
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x0002CC72 File Offset: 0x0002AE72
		public static void ResetSliderIds()
		{
			UI._sliderIdCounter = 0;
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x0002CC7C File Offset: 0x0002AE7C
		public static float Slider(string label, float value, float min, float max, string format = "F3", string tooltip = null)
		{
			Rect rect = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
			float num = max - min;
			float num2 = ((num > 0f) ? Mathf.Clamp01((value - min) / num) : 0f);
			int controlID = GUIUtility.GetControlID(20829 + UI._sliderIdCounter++, FocusType.Passive, rect);
			Event current = Event.current;
			if (GUI.enabled)
			{
				switch (current.GetTypeForControl(controlID))
				{
				case EventType.MouseDown:
					if (current.button == 0 && rect.Contains(current.mousePosition))
					{
						GUIUtility.hotControl = controlID;
						value = UI.ComputeValue(current.mousePosition.x, rect, min, max);
						num2 = ((num > 0f) ? Mathf.Clamp01((value - min) / num) : 0f);
						GUI.changed = true;
						current.Use();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlID)
					{
						GUIUtility.hotControl = 0;
						current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						value = UI.ComputeValue(current.mousePosition.x, rect, min, max);
						num2 = ((num > 0f) ? Mathf.Clamp01((value - min) / num) : 0f);
						GUI.changed = true;
						current.Use();
					}
					break;
				}
			}
			if (current.type == EventType.Repaint)
			{
				GUI.DrawTexture(rect, Theme.SliderTrackBgTex);
				if (num2 > 0f)
				{
					GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * num2, rect.height), Theme.SliderFillTex);
				}
				UI.DrawBorder(rect);
				if (string.IsNullOrEmpty(tooltip))
				{
					GUI.Label(rect, label, Theme.SliderLabelLeft);
				}
				else
				{
					UI._labelContent.text = label;
					UI._labelContent.tooltip = tooltip;
					GUI.Label(rect, UI._labelContent, Theme.SliderLabelLeft);
				}
				GUI.Label(rect, value.ToString(format), Theme.SliderValueRight);
			}
			return Mathf.Clamp(value, min, max);
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0002CE88 File Offset: 0x0002B088
		private static float ComputeValue(float mouseX, Rect rect, float min, float max)
		{
			float num = Mathf.Clamp01((mouseX - rect.x) / rect.width);
			return Mathf.Lerp(min, max, num);
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0002CEB4 File Offset: 0x0002B0B4
		public static bool Checkbox(bool value, string label, string tooltip = null)
		{
			Rect rect = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(22f)
			});
			Rect rect2;
			rect2 = new Rect(rect.x + 3f, rect.y + (rect.height - 14f) * 0.5f, 14f, 14f);
			Rect rect3;
			rect3 = new Rect(rect2.xMax + 4f, rect.y, rect.width - 21f, rect.height);
			Event current = Event.current;
			bool flag = value;
			if (GUI.enabled && current.type == EventType.MouseDown && current.button == 0 && rect.Contains(current.mousePosition))
			{
				flag = !value;
				GUI.changed = true;
				current.Use();
			}
			if (current.type == EventType.Repaint)
			{
				Color color = GUI.color;
				if (!GUI.enabled)
				{
					GUI.color = new Color(color.r, color.g, color.b, color.a * 0.5f);
				}
				UI.DrawToggleBox(rect2, value);
				if (!string.IsNullOrEmpty(label) || !string.IsNullOrEmpty(tooltip))
				{
					UI._labelContent.text = label ?? string.Empty;
					UI._labelContent.tooltip = tooltip ?? string.Empty;
					GUI.Label(rect3, UI._labelContent, Theme.RowLabelStyle);
				}
				GUI.color = color;
			}
			return flag;
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x0002D034 File Offset: 0x0002B234
		public static UI.VirtualScrollRange BeginVirtualScroll(ref Vector2 scroll, float viewportHeight, int count, float rowHeight)
		{
			scroll = GUILayout.BeginScrollView(scroll, new GUILayoutOption[] { GUILayout.Height(viewportHeight) });
			UI.VirtualScrollRange virtualScrollRange;
			if (count <= 0 || rowHeight <= 0f)
			{
				virtualScrollRange.First = 0;
				virtualScrollRange.Last = 0;
			}
			else
			{
				int num = Mathf.Clamp(Mathf.FloorToInt(scroll.y / rowHeight), 0, count);
				int num2 = Mathf.CeilToInt(viewportHeight / rowHeight) + 1;
				virtualScrollRange.First = num;
				virtualScrollRange.Last = Mathf.Min(count, num + num2);
			}
			if (virtualScrollRange.First > 0)
			{
				GUILayout.Space((float)virtualScrollRange.First * rowHeight);
			}
			return virtualScrollRange;
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x0002D0D0 File Offset: 0x0002B2D0
		public static void EndVirtualScroll(UI.VirtualScrollRange range, int count, float rowHeight)
		{
			int num = count - range.Last;
			if (num > 0 && rowHeight > 0f)
			{
				GUILayout.Space((float)num * rowHeight);
			}
			GUILayout.EndScrollView();
		}

		// Token: 0x04000430 RID: 1072
		public const float DefaultLabelWidth = 120f;

		// Token: 0x04000431 RID: 1073
		private const float BorderThickness = 1f;

		// Token: 0x04000432 RID: 1074
		private static readonly Rect[] _splitCells = new Rect[8];

		// Token: 0x04000433 RID: 1075
		private static int _sliderIdCounter;

		// Token: 0x04000434 RID: 1076
		private static readonly GUIContent _labelContent = new GUIContent();

		// Token: 0x0200007E RID: 126
		public struct FlatSkinScope : IDisposable
		{
			// Token: 0x060004BA RID: 1210 RVA: 0x0002E1F0 File Offset: 0x0002C3F0
			public static UI.FlatSkinScope Begin()
			{
				UI.FlatSkinScope flatSkinScope = default(UI.FlatSkinScope);
				flatSkinScope._prevButton = GUI.skin.button;
				GUI.skin.button = Theme.FlatButtonStyle;
				flatSkinScope._customStyles = GUI.skin.customStyles;
				flatSkinScope._btnNamedIdx = -1;
				flatSkinScope._prevNamedBtn = null;
				if (flatSkinScope._customStyles != null)
				{
					for (int i = 0; i < flatSkinScope._customStyles.Length; i++)
					{
						if (flatSkinScope._customStyles[i] != null && flatSkinScope._customStyles[i].name == "Button")
						{
							flatSkinScope._btnNamedIdx = i;
							flatSkinScope._prevNamedBtn = flatSkinScope._customStyles[i];
							flatSkinScope._customStyles[i] = Theme.FlatButtonStyle;
							break;
						}
					}
				}
				flatSkinScope._prevVScroll = GUI.skin.verticalScrollbar;
				flatSkinScope._prevVScrollThumb = GUI.skin.verticalScrollbarThumb;
				flatSkinScope._prevVScrollUp = GUI.skin.verticalScrollbarUpButton;
				flatSkinScope._prevVScrollDown = GUI.skin.verticalScrollbarDownButton;
				flatSkinScope._prevHScroll = GUI.skin.horizontalScrollbar;
				flatSkinScope._prevHScrollThumb = GUI.skin.horizontalScrollbarThumb;
				flatSkinScope._prevHScrollLeft = GUI.skin.horizontalScrollbarLeftButton;
				flatSkinScope._prevHScrollRight = GUI.skin.horizontalScrollbarRightButton;
				GUI.skin.verticalScrollbar = Theme.FlatScrollbarStyle;
				GUI.skin.verticalScrollbarThumb = Theme.FlatScrollbarThumbStyle;
				GUI.skin.verticalScrollbarUpButton = Theme.FlatScrollbarButtonStyle;
				GUI.skin.verticalScrollbarDownButton = Theme.FlatScrollbarButtonStyle;
				GUI.skin.horizontalScrollbar = Theme.FlatScrollbarHorizontalStyle;
				GUI.skin.horizontalScrollbarThumb = Theme.FlatScrollbarHorizontalThumbStyle;
				GUI.skin.horizontalScrollbarLeftButton = Theme.FlatScrollbarButtonStyle;
				GUI.skin.horizontalScrollbarRightButton = Theme.FlatScrollbarButtonStyle;
				return flatSkinScope;
			}

			// Token: 0x060004BB RID: 1211 RVA: 0x0002E3AC File Offset: 0x0002C5AC
			public void Dispose()
			{
				if (this._prevButton != null)
				{
					GUI.skin.button = this._prevButton;
				}
				if (this._btnNamedIdx >= 0 && this._customStyles != null)
				{
					this._customStyles[this._btnNamedIdx] = this._prevNamedBtn;
				}
				if (this._prevVScroll != null)
				{
					GUI.skin.verticalScrollbar = this._prevVScroll;
				}
				if (this._prevVScrollThumb != null)
				{
					GUI.skin.verticalScrollbarThumb = this._prevVScrollThumb;
				}
				if (this._prevVScrollUp != null)
				{
					GUI.skin.verticalScrollbarUpButton = this._prevVScrollUp;
				}
				if (this._prevVScrollDown != null)
				{
					GUI.skin.verticalScrollbarDownButton = this._prevVScrollDown;
				}
				if (this._prevHScroll != null)
				{
					GUI.skin.horizontalScrollbar = this._prevHScroll;
				}
				if (this._prevHScrollThumb != null)
				{
					GUI.skin.horizontalScrollbarThumb = this._prevHScrollThumb;
				}
				if (this._prevHScrollLeft != null)
				{
					GUI.skin.horizontalScrollbarLeftButton = this._prevHScrollLeft;
				}
				if (this._prevHScrollRight != null)
				{
					GUI.skin.horizontalScrollbarRightButton = this._prevHScrollRight;
				}
			}

			// Token: 0x0400051D RID: 1309
			private GUIStyle _prevButton;

			// Token: 0x0400051E RID: 1310
			private GUIStyle[] _customStyles;

			// Token: 0x0400051F RID: 1311
			private int _btnNamedIdx;

			// Token: 0x04000520 RID: 1312
			private GUIStyle _prevNamedBtn;

			// Token: 0x04000521 RID: 1313
			private GUIStyle _prevVScroll;

			// Token: 0x04000522 RID: 1314
			private GUIStyle _prevVScrollThumb;

			// Token: 0x04000523 RID: 1315
			private GUIStyle _prevVScrollUp;

			// Token: 0x04000524 RID: 1316
			private GUIStyle _prevVScrollDown;

			// Token: 0x04000525 RID: 1317
			private GUIStyle _prevHScroll;

			// Token: 0x04000526 RID: 1318
			private GUIStyle _prevHScrollThumb;

			// Token: 0x04000527 RID: 1319
			private GUIStyle _prevHScrollLeft;

			// Token: 0x04000528 RID: 1320
			private GUIStyle _prevHScrollRight;
		}

		// Token: 0x0200007F RID: 127
		public struct VirtualScrollRange
		{
			// Token: 0x04000529 RID: 1321
			public int First;

			// Token: 0x0400052A RID: 1322
			public int Last;
		}
	}
}
