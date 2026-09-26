using System;
using System.Collections.Generic;
using System.Globalization;
using KKAPI.Studio;
using KKAPI.Utilities;
using UnityEngine;
using AIChara;

namespace KKShapeEditor
{
	// Token: 0x02000046 RID: 70
	public class ShapeEditorWindow
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000348 RID: 840 RVA: 0x0001E2A6 File Offset: 0x0001C4A6
		// (set) Token: 0x06000349 RID: 841 RVA: 0x0001E2AD File Offset: 0x0001C4AD
		public static bool IsMouseOverUI { get; private set; }

		// Token: 0x0600034A RID: 842 RVA: 0x0001E2B5 File Offset: 0x0001C4B5
		public static bool IsScreenPointOverUI(Vector3 inputMousePosition)
		{
			return ShapeEditorWindow._activeInstance != null && ShapeEditorWindow._activeInstance.ContainsScreenPoint(inputMousePosition);
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600034B RID: 843 RVA: 0x0001E2CB File Offset: 0x0001C4CB
		// (set) Token: 0x0600034C RID: 844 RVA: 0x0001E2D3 File Offset: 0x0001C4D3
		public bool ShowMeshHighlight { get; set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600034D RID: 845 RVA: 0x0001E2DC File Offset: 0x0001C4DC
		// (set) Token: 0x0600034E RID: 846 RVA: 0x0001E2E4 File Offset: 0x0001C4E4
		public bool ShowMeshWireframe { get; set; } = true;

		public class SeamlessCharacterOption
		{
			public string Name;
			public AIChara.ChaControl Character;
		}

		public float SeamlessBlendDistance { get; set; } = 0.03f;
		public float SeamlessBorderStrength { get; set; } = 0.67f;
		public float SeamlessBorderOffset { get; set; } = 0.0f;
		public float SeamlessBlendStrength { get; set; } = 1.0f;
		public bool SeamlessEnabled { get; set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600034F RID: 847 RVA: 0x0001E2ED File Offset: 0x0001C4ED
		// (set) Token: 0x06000350 RID: 848 RVA: 0x0001E2F5 File Offset: 0x0001C4F5
		public ShapeEditorWindow.OpMode OperationMode { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000351 RID: 849 RVA: 0x0001E2FE File Offset: 0x0001C4FE
		// (set) Token: 0x06000352 RID: 850 RVA: 0x0001E306 File Offset: 0x0001C506
		public ShapeEditorWindow.BrushToolType SelectedBrushTool { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000353 RID: 851 RVA: 0x0001E30F File Offset: 0x0001C50F
		// (set) Token: 0x06000354 RID: 852 RVA: 0x0001E317 File Offset: 0x0001C517
		public float BrushRadius { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000355 RID: 853 RVA: 0x0001E320 File Offset: 0x0001C520
		// (set) Token: 0x06000356 RID: 854 RVA: 0x0001E328 File Offset: 0x0001C528
		public float BrushStrength { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000357 RID: 855 RVA: 0x0001E331 File Offset: 0x0001C531
		// (set) Token: 0x06000358 RID: 856 RVA: 0x0001E339 File Offset: 0x0001C539
		public FalloffMode BrushFalloff { get; set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000359 RID: 857 RVA: 0x0001E342 File Offset: 0x0001C542
		// (set) Token: 0x0600035A RID: 858 RVA: 0x0001E34A File Offset: 0x0001C54A
		public bool SymmetryEnabled { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600035B RID: 859 RVA: 0x0001E353 File Offset: 0x0001C553
		// (set) Token: 0x0600035C RID: 860 RVA: 0x0001E35B File Offset: 0x0001C55B
		public int SymmetryAxisIndex { get; set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600035D RID: 861 RVA: 0x0001E364 File Offset: 0x0001C564
		// (set) Token: 0x0600035E RID: 862 RVA: 0x0001E36C File Offset: 0x0001C56C
		public float SymmetryCenter { get; set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600035F RID: 863 RVA: 0x0001E375 File Offset: 0x0001C575
		// (set) Token: 0x06000360 RID: 864 RVA: 0x0001E37D File Offset: 0x0001C57D
		public bool SymmetryCenterSet { get; set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000361 RID: 865 RVA: 0x0001E386 File Offset: 0x0001C586
		// (set) Token: 0x06000362 RID: 866 RVA: 0x0001E38E File Offset: 0x0001C58E
		public bool DeferSetSymmetryCenter { get; set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000363 RID: 867 RVA: 0x0001E397 File Offset: 0x0001C597
		// (set) Token: 0x06000364 RID: 868 RVA: 0x0001E39F File Offset: 0x0001C59F
		public bool DeferClearSymmetryCenter { get; set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000365 RID: 869 RVA: 0x0001E3A8 File Offset: 0x0001C5A8
		// (set) Token: 0x06000366 RID: 870 RVA: 0x0001E3B0 File Offset: 0x0001C5B0
		public int GizmoModeIndex { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000367 RID: 871 RVA: 0x0001E3B9 File Offset: 0x0001C5B9
		// (set) Token: 0x06000368 RID: 872 RVA: 0x0001E3C1 File Offset: 0x0001C5C1
		public int GizmoSpaceIndex { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000369 RID: 873 RVA: 0x0001E3CA File Offset: 0x0001C5CA
		// (set) Token: 0x0600036A RID: 874 RVA: 0x0001E3D2 File Offset: 0x0001C5D2
		public bool GizmoSoftSelection { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600036B RID: 875 RVA: 0x0001E3DB File Offset: 0x0001C5DB
		// (set) Token: 0x0600036C RID: 876 RVA: 0x0001E3E3 File Offset: 0x0001C5E3
		public int SoftSelectModeIndex { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600036D RID: 877 RVA: 0x0001E3EC File Offset: 0x0001C5EC
		// (set) Token: 0x0600036E RID: 878 RVA: 0x0001E3F4 File Offset: 0x0001C5F4
		public float GizmoSoftRadius { get; set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600036F RID: 879 RVA: 0x0001E3FD File Offset: 0x0001C5FD
		// (set) Token: 0x06000370 RID: 880 RVA: 0x0001E405 File Offset: 0x0001C605
		public FalloffMode GizmoFalloff { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000371 RID: 881 RVA: 0x0001E40E File Offset: 0x0001C60E
		// (set) Token: 0x06000372 RID: 882 RVA: 0x0001E416 File Offset: 0x0001C616
		public ShapeEditorWindow.GizmoSelectMethod GizmoSelection { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000373 RID: 883 RVA: 0x0001E41F File Offset: 0x0001C61F
		// (set) Token: 0x06000374 RID: 884 RVA: 0x0001E427 File Offset: 0x0001C627
		public ShapeEditorWindow.SelectMode SelectionMode { get; set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000375 RID: 885 RVA: 0x0001E430 File Offset: 0x0001C630
		// (set) Token: 0x06000376 RID: 886 RVA: 0x0001E438 File Offset: 0x0001C638
		public int SubdivideLevel { get; set; } = 1;

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000377 RID: 887 RVA: 0x0001E441 File Offset: 0x0001C641
		// (set) Token: 0x06000378 RID: 888 RVA: 0x0001E449 File Offset: 0x0001C649
		public bool SubdivideSmooth { get; set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000379 RID: 889 RVA: 0x0001E452 File Offset: 0x0001C652
		// (set) Token: 0x0600037A RID: 890 RVA: 0x0001E45A File Offset: 0x0001C65A
		public bool IsEditMode { get; private set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600037B RID: 891 RVA: 0x0001E463 File Offset: 0x0001C663
		// (set) Token: 0x0600037C RID: 892 RVA: 0x0001E46B File Offset: 0x0001C66B
		public int SelectedRendererIndex
		{
			get
			{
				return this.PrimaryRendererIndex;
			}
			set
			{
				this.PrimaryRendererIndex = value;
				this.SelectedRendererIndices.Clear();
				if (value >= 0)
				{
					this.SelectedRendererIndices.Add(value);
				}
			}
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0001E490 File Offset: 0x0001C690
		public ShapeEditorWindow(int windowId, Rect initialRect)
		{
			this._windowId = windowId;
			this._windowRect = initialRect;
			this.BrushRadius = ShapeEditorPlugin.DefaultBrushRadius.Value;
			this.BrushStrength = ShapeEditorPlugin.DefaultBrushStrength.Value;
			this.GizmoSoftRadius = 0.1f;
			float value = ShapeEditorPlugin.WindowWidth.Value;
			this._userWidth = ((value <= 0f) ? 0f : Mathf.Max(350f, value));
			float value2 = ShapeEditorPlugin.WindowHeightCap.Value;
			this._userHeightCap = ((value2 <= 0f) ? 0f : Mathf.Max(200f, value2));
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600037E RID: 894 RVA: 0x0001E696 File Offset: 0x0001C896
		// (set) Token: 0x0600037F RID: 895 RVA: 0x0001E69E File Offset: 0x0001C89E
		public bool Visible
		{
			get
			{
				return this._visible;
			}
			set
			{
				this._visible = value;
			}
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0001E6A7 File Offset: 0x0001C8A7
		public void Toggle()
		{
			this._visible = !this._visible;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0001E6B8 File Offset: 0x0001C8B8
		public void SetEditMode(bool active)
		{
			this.IsEditMode = active;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0001E6C4 File Offset: 0x0001C8C4
		public void DrawGUI()
		{
			if (!this._visible)
			{
				return;
			}
			try
			{
				ShapeEditorWindow._activeInstance = this;				
				GUILayoutOption widthOption = ((this._userWidth > 0f) ? GUILayout.Width(this._userWidth) : GUILayout.MinWidth(350f));
				GUILayoutOption heightOption = ((this._userHeightCap > 0f) ? GUILayout.Height(this._userHeightCap) : GUILayout.MinHeight(200f));
				this._windowRect = GUILayout.Window(this._windowId, this._windowRect, new GUI.WindowFunction(this.DrawWindow), "KKShapeEditor", Theme.WindowStyle, new GUILayoutOption[] { widthOption, heightOption });
				if (Event.current != null && Event.current.type == EventType.Layout)
				{
					this._settledWindowHeight = this._windowRect.height;

					if (!this._resizing)
					{
						float num = this._settledWindowHeight - this._lastViewportHeight;
						if (num > 1f)
						{
							this._chromeHeight = num;
						}
					}
				}
				if (this._showHelp)
				{
					this._helpWindowRect.x = this._windowRect.xMax;
					this._helpWindowRect.y = this._windowRect.y;
					this._helpWindowRect.width = 300f;
					this._helpWindowRect = GUILayout.Window(this._windowId + 1, this._helpWindowRect, new GUI.WindowFunction(this.DrawHelpWindow), L.HelpTitle, Theme.WindowStyle, new GUILayoutOption[]
					{
						GUILayout.Width(300f),
						GUILayout.MinHeight(500f)
					});
				}
				if (this._showResetConfirm)
				{
					this._resetConfirmRect.x = this._windowRect.x + (this._windowRect.width - this._resetConfirmRect.width) * 0.5f;
					this._resetConfirmRect.y = this._windowRect.y + (this._settledWindowHeight - this._resetConfirmRect.height) * 0.5f;
					this._resetConfirmRect = GUI.ModalWindow(this._windowId + 2, this._resetConfirmRect, new GUI.WindowFunction(this.DrawResetConfirmPopup), L.ResetCorruptedConfirmTitle, Theme.WindowStyle);
				}
				ShapeEditorWindow.IsMouseOverUI = this.ContainsScreenPoint(Input.mousePosition) || GUIUtility.hotControl != 0;
				if (ShapeEditorWindow.IsMouseOverUI)
				{
					Input.ResetInputAxes();
				}
			}
			finally
			{
			}
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0001E944 File Offset: 0x0001CB44
		public bool ContainsScreenPoint(Vector3 inputMousePosition)
		{
			if (this._resizing)
			{
				return true;
			}
			Vector2 vector;
			vector = new Vector2(inputMousePosition.x, (float)Screen.height - inputMousePosition.y);
			float num = ((this._settledWindowHeight > 0f) ? this._settledWindowHeight : this._windowRect.height);
			Rect rect;
			rect = new Rect(this._windowRect.x, this._windowRect.y, this._windowRect.width, num);
			return rect.Contains(vector) || (this._showHelp && this._helpWindowRect.Contains(vector)) || (this._showResetConfirm && this._resetConfirmRect.Contains(vector));
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000384 RID: 900 RVA: 0x0001E9FC File Offset: 0x0001CBFC
		private static int PresetTabIndex
		{
			get
			{
				if (L.TabNames == null)
				{
					return 2;
				}
				return L.TabNames.Length;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000385 RID: 901 RVA: 0x0001EA0E File Offset: 0x0001CC0E
		private static int PsdTabIndex
		{
			get
			{
				return ShapeEditorWindow.PresetTabIndex + 1;
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000386 RID: 902 RVA: 0x0001EA17 File Offset: 0x0001CC17
		private static int CsbTabIndex
		{
			get
			{
				return ShapeEditorWindow.PresetTabIndex + 2;
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000387 RID: 903 RVA: 0x0001EA20 File Offset: 0x0001CC20
		private static int NbbTabIndex
		{
			get
			{
				return ShapeEditorWindow.PresetTabIndex + 3;
			}
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0001EA2C File Offset: 0x0001CC2C
		private string[] GetTabNames()
		{
			if (L.TabNames == null)
			{
				return L.TabNames;
			}
			if (!ShapeEditorPlugin.IsStudio)
			{
				if (this._tabNamesMakerCache == null || this._tabNamesMakerBaseRef != L.TabNames)
				{
					this._tabNamesMakerBaseRef = L.TabNames;
					string[] array = new string[L.TabNames.Length + 4];
					for (int i = 0; i < L.TabNames.Length; i++)
					{
						array[i] = L.TabNames[i];
					}
					array[L.TabNames.Length] = L.PresetTabLabel;
					array[L.TabNames.Length + 1] = L.PsdTabLabel;
					array[L.TabNames.Length + 2] = L.CsbTabLabel;
					array[L.TabNames.Length + 3] = L.NbbTabLabel;
					this._tabNamesMakerCache = array;
				}
				return this._tabNamesMakerCache;
			}
			if (this._tabNamesStudioCache == null || this._tabNamesStudioBaseRef != L.TabNames)
			{
				this._tabNamesStudioBaseRef = L.TabNames;
				string[] array2 = new string[L.TabNames.Length + 5];
				for (int j = 0; j < L.TabNames.Length; j++)
				{
					array2[j] = L.TabNames[j];
				}
				array2[L.TabNames.Length] = L.PresetTabLabel;
				array2[L.TabNames.Length + 1] = L.PsdTabLabel;
				array2[L.TabNames.Length + 2] = L.CsbTabLabel;
				array2[L.TabNames.Length + 3] = L.NbbTabLabel;
				array2[L.TabNames.Length + 4] = L.SettingsTabLabel;
				this._tabNamesStudioCache = array2;
			}
			return this._tabNamesStudioCache;
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0001EB90 File Offset: 0x0001CD90
		private void DrawWindow(int id)
		{
			UI.FlatSkinScope flatSkinScope = UI.FlatSkinScope.Begin();
			bool flag = false;
			Rect rect = default(Rect);
			try
			{
				UI.ResetSliderIds();
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				string[] tabNames = this.GetTabNames();
				this._tabIndex = GUILayout.Toolbar(this._tabIndex, tabNames, Array.Empty<GUILayoutOption>());
				if (GUILayout.Button("?", new GUILayoutOption[] { GUILayout.Width(25f) }))
				{
					this._showHelp = !this._showHelp;
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(2f);
				if (this._tabIndex != this._lastTabIndexForScroll)
				{
					this._windowScroll = Vector2.zero;
					this._lastTabIndexForScroll = this._tabIndex;
				}
				float num = this.ComputeViewportHeight();
				this._lastViewportHeight = num;
				this._windowScroll = GUILayout.BeginScrollView(this._windowScroll,new GUILayoutOption[] { GUILayout.Height(num) });
				flag = true;
				int presetTabIndex = ShapeEditorWindow.PresetTabIndex;
				int psdTabIndex = ShapeEditorWindow.PsdTabIndex;
				int csbTabIndex = ShapeEditorWindow.CsbTabIndex;
				int nbbTabIndex = ShapeEditorWindow.NbbTabIndex;
				int num2 = (ShapeEditorPlugin.IsStudio ? (tabNames.Length - 1) : (-1));
				if (this._tabIndex == 0)
				{
					try
					{
						this.DrawShapeTab();
						goto IL_018B;
					}
					finally
					{
					}
				}
				if (this._tabIndex == 1)
				{
					try
					{
						this.DrawSubdivideTab();
						goto IL_018B;
					}
					finally
					{
					}
				}
				if (this._tabIndex == presetTabIndex)
				{
					try
					{
						this.DrawPresetTab();
						goto IL_018B;
					}
					finally
					{
					}
				}
				if (this._tabIndex == psdTabIndex)
				{
					try
					{
						this.DrawPsdTab();
						goto IL_018B;
					}
					finally
					{
					}
				}
				if (this._tabIndex == csbTabIndex)
				{
					try
					{
						this.DrawCsbTab();
						goto IL_018B;
					}
					finally
					{
					}
				}
				if (this._tabIndex == nbbTabIndex)
				{
					try
					{
						this.DrawNbbTab();
						goto IL_018B;
					}
					finally
					{
					}
				}
				if (this._tabIndex == num2)
				{
					try
					{
						this.DrawSettingsTab();
					}
					finally
					{
					}
				}
				IL_018B:
				if (Event.current != null && Event.current.type == EventType.Layout)
				{
					float yMax = GUILayoutUtility.GetLastRect().yMax;
					if (yMax > 1f && (this._measuredContentHeight <= 1f || Mathf.Abs(this._measuredContentHeight - yMax) > 1f))
					{
						this._measuredContentHeight = yMax;
					}
				}
				GUILayout.EndScrollView();
				flag = false;
				Rect rect2 = GUILayoutUtility.GetRect(10f, 14f, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
				rect = new Rect(rect2.xMax - 16f, rect2.y - 1f, 16f, 15f);
				if (Event.current != null && Event.current.type == EventType.Repaint)
				{
					this.DrawGripHandle(rect);
				}
				GUILayout.EndVertical();
			}
			catch (Exception ex)
			{
				if (flag)
				{
					GUILayout.EndScrollView();
				}
				GUILayout.EndVertical();
				ShapeEditorPlugin.Logger.LogWarning("GUI draw error: " + ex.Message);
			}
			finally
			{
				flatSkinScope.Dispose();
			}
			this.HandleResizeGrip(rect);
			IMGUIUtils.DrawTooltip(new Rect(0f, 0f, this._windowRect.width, this._windowRect.height), 400);

			if (!this._resizing)
			{
				GUI.DragWindow();
			}
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0001EF1C File Offset: 0x0001D11C
		private float ComputeViewportHeight()
		{
			float contentHeight = ((this._measuredContentHeight > 1f) ? this._measuredContentHeight : 400f);

			if (this._userHeightCap <= 0f)
			{
				return contentHeight + 2f;
			}

			return Mathf.Max(60f, this._userHeightCap - 70f);
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0001EF80 File Offset: 0x0001D180
		private void HandleResizeGrip(Rect gripRect)
		{
			Event current = Event.current;
			if (current == null)
			{
				return;
			}
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			if (this._resizing)
			{
				controlID = this._resizeControlId;
			}
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (current.button == 0 && gripRect.Contains(current.mousePosition))
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					if (realtimeSinceStartup - this._lastGripClickTime < 0.3f)
					{
						this._userHeightCap = 0f;
						ShapeEditorPlugin.WindowHeightCap.Value = 0f;
						this._resizing = false;
						GUIUtility.hotControl = 0;
						this._lastGripClickTime = 0f;
					}
					else
					{
						this._resizing = true;
						this._resizeControlId = controlID;
						GUIUtility.hotControl = controlID;
						this._resizeStartMouseScreen = GUIUtility.GUIToScreenPoint(current.mousePosition);
						this._resizeStartWidth = ((this._userWidth > 0f) ? this._userWidth : this._windowRect.width);
						this._resizeStartCap = ((this._userHeightCap > 0f) ? this._userHeightCap : this._windowRect.height);
						this._lastGripClickTime = realtimeSinceStartup;
					}
					current.Use();
					return;
				}
				break;
			case EventType.MouseUp:
				if (this._resizing && GUIUtility.hotControl == controlID)
				{
					this._resizing = false;
					this._resizeControlId = 0;
					GUIUtility.hotControl = 0;
					this._userWidth = Mathf.Max(350f, this._userWidth);
					this._userHeightCap = Mathf.Max(200f, this._userHeightCap);
					ShapeEditorPlugin.WindowWidth.Value = this._userWidth;
					ShapeEditorPlugin.WindowHeightCap.Value = this._userHeightCap;
					current.Use();
				}
				break;
			case EventType.MouseMove:
				break;
			case EventType.MouseDrag:
				if (this._resizing && GUIUtility.hotControl == controlID)
				{
					Vector2 currentMouseScreen = GUIUtility.GUIToScreenPoint(current.mousePosition);
					Vector2 mouseDelta = currentMouseScreen - this._resizeStartMouseScreen;
					this._userWidth = Mathf.Max(350f, this._resizeStartWidth + mouseDelta.x);
					this._userHeightCap = Mathf.Max(200f, this._resizeStartCap + mouseDelta.y);
					current.Use();
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0001F144 File Offset: 0x0001D344
		private void DrawGripHandle(Rect grip)
		{
			Color color = GUI.color;
			GUI.color = ShapeEditorWindow._gripColor;
			float num = grip.xMax - 2f - 1f;
			float num2 = grip.yMax - 2f - 1f;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3 - i; j++)
				{
					GUI.DrawTexture(new Rect(num - (float)i * 4f, num2 - (float)j * 4f, 2f, 2f), Texture2D.whiteTexture);
				}
			}
			GUI.color = color;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0001F1DC File Offset: 0x0001D3DC
		private void DrawSettingsTab()
		{
			UI.BeginPanel(L.SectionSettings);
			bool value = ShapeEditorPlugin.EnableCharacterSwitchCarryover.Value;
			bool flag = UI.Checkbox(value, L.CarryoverToggleLabel, L.CarryoverTooltipText);
			if (flag != value)
			{
				ShapeEditorPlugin.EnableCharacterSwitchCarryover.Value = flag;
			}
			GUILayout.Space(4f);
			GUILayout.Label(L.CarryoverSettingsHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			UI.EndPanel();
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0001F241 File Offset: 0x0001D441
		private void DrawPresetTab()
		{
			if (this.PresetState == null)
			{
				this.PresetState = new PresetTabState();
			}
			if (this.PresetState.Mode == PresetTabState.TabMode.Idle)
			{
				this.DrawPresetIdle();
				return;
			}
			this.DrawPresetImportPreview();
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0001F270 File Offset: 0x0001D470
		private void DrawPresetIdle()
		{
			UI.BeginPanel(L.SectionPreset);
			if (this.Renderers == null || this.Renderers.Count == 0)
			{
				GUILayout.Label(L.PresetEmptyOwnerHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
				UI.EndPanel();
				return;
			}
			this.PresetState.Scroll = GUILayout.BeginScrollView(this.PresetState.Scroll, new GUILayoutOption[] { GUILayout.Height(280f) });
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < this.Renderers.Count; i++)
			{
				Renderer renderer = this.Renderers[i];
				if (!(renderer == null))
				{
					int subdivisionLevel = MeshHelper.GetSubdivisionLevel(renderer);
					string text = ((this.RendererPaths != null && i < this.RendererPaths.Count) ? this.RendererPaths[i] : null);
					int num4;
					int num5;
					int num6;
					int num7;
					ShapeEditorWindow.GetRendererPresetCounts(renderer, text, out num4, out num5, out num6, out num7);
					if (subdivisionLevel == 0 && num4 == 0)
					{
						if (this.PresetState.RowChecked.ContainsKey(i))
						{
							this.PresetState.RowChecked.Remove(i);
						}
					}
					else
					{
						num2++;
						bool flag;
						this.PresetState.RowChecked.TryGetValue(i, out flag);
						string text2 = ShapeEditorWindow.ComputePresetShortName(text);
						string text3 = string.Format(L.PresetRowFormatFmt, new object[] { text2, subdivisionLevel, num4, num5, num6, num7 });
						string text4 = (string.IsNullOrEmpty(text) ? L.PresetRowRootName : text);
						UI.BeginRowBg(num3++);
						bool flag2 = UI.Checkbox(flag, text3, text4);
						UI.EndRowBg();
						if (flag2 != flag)
						{
							this.PresetState.RowChecked[i] = flag2;
						}
						if (flag2)
						{
							num++;
						}
					}
				}
			}
			if (num2 == 0)
			{
				GUILayout.Label(L.PresetNoExportableHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndScrollView();
			Rect[] array = UI.SplitRow(2, 22f, 2f);
			bool enabled = GUI.enabled;
			if (num == 0)
			{
				GUI.enabled = false;
			}
			if (GUI.Button(array[0], L.ExportPresetSelected))
			{
				this.DeferPresetExport = true;
			}
			GUI.enabled = enabled;
			if (GUI.Button(array[1], L.ImportPreset))
			{
				this.DeferPresetImport = true;
			}
			UI.EndPanel();
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0001F4D0 File Offset: 0x0001D6D0
		private void DrawPresetImportPreview()
		{
			PresetBundle loadedBundle = this.PresetState.LoadedBundle;
			if (loadedBundle == null || loadedBundle.Entries == null)
			{
				this.PresetState.ResetForOwnerSwitch();
				return;
			}
			UI.BeginPanel(L.SectionPreset);
			this.PresetState.Scroll = GUILayout.BeginScrollView(this.PresetState.Scroll, new GUILayoutOption[] { GUILayout.Height(280f) });
			int num = 0;
			for (int i = 0; i < loadedBundle.Entries.Count; i++)
			{
				PresetEntry presetEntry = loadedBundle.Entries[i];
				if (presetEntry != null)
				{
					bool flag = PresetImporter.HasSameAssetRenderer(this.Renderers, this.RendererPaths, presetEntry);
					bool flag2;
					this.PresetState.RowChecked.TryGetValue(i, out flag2);
					string text = ShapeEditorWindow.ComputePresetShortName(presetEntry.RendererPath);
					string text2 = string.Format(L.PresetRowFormatFmt, new object[]
					{
						text,
						presetEntry.SubdivLevel,
						(presetEntry.Layers != null) ? presetEntry.Layers.Count : 0,
						(presetEntry.PsdDrivers != null) ? presetEntry.PsdDrivers.Count : 0,
						(presetEntry.CsbDrivers != null) ? presetEntry.CsbDrivers.Count : 0,
						(presetEntry.NbbDrivers != null) ? presetEntry.NbbDrivers.Count : 0
					}) + "  [" + (flag ? L.PresetMatchOk : L.PresetMatchNoMatch) + "]";
					string text3 = presetEntry.RendererPath ?? "";
					UI.BeginRowBg(i);
					bool flag3 = UI.Checkbox(flag2, text2, text3);
					UI.EndRowBg();
					if (flag3 != flag2)
					{
						this.PresetState.RowChecked[i] = flag3;
					}
					if (flag3 && flag)
					{
						num++;
					}
				}
			}
			GUILayout.EndScrollView();
			bool flag4 = UI.Checkbox(this.PresetState.PreserveExistingLayers, L.PresetPreserveLayers, L.PresetPreserveLayersTooltip);
			if (flag4 != this.PresetState.PreserveExistingLayers)
			{
				this.PresetState.PreserveExistingLayers = flag4;
			}
			bool flag5 = UI.Checkbox(this.PresetState.PreserveCoordinateScope, L.PresetPreserveScope, L.PresetPreserveScopeTooltip);
			if (flag5 != this.PresetState.PreserveCoordinateScope)
			{
				this.PresetState.PreserveCoordinateScope = flag5;
			}
			Rect[] array = UI.SplitRow(2, 22f, 2f);
			bool enabled = GUI.enabled;
			if (num == 0)
			{
				GUI.enabled = false;
			}
			if (GUI.Button(array[0], L.ApplyImportPreset))
			{
				this.DeferPresetApplyImport = true;
			}
			GUI.enabled = enabled;
			if (GUI.Button(array[1], L.CancelImportPreset))
			{
				this.DeferPresetCancelImport = true;
			}
			UI.EndPanel();
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0001F78C File Offset: 0x0001D98C
		private static string ComputePresetShortName(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return L.PresetRowRootName;
			}
			int num = path.LastIndexOf('/');
			if (num < 0)
			{
				return path;
			}
			return path.Substring(num + 1);
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0001F7C0 File Offset: 0x0001D9C0
		private static void GetRendererPresetCounts(Renderer r, string path, out int layerCount, out int psdCount, out int csbCount, out int nbbCount)
		{
			layerCount = 0;
			psdCount = 0;
			csbCount = 0;
			nbbCount = 0;
			if (r == null)
			{
				return;
			}
			ControllerResolver.Owner owner = ControllerResolver.Resolve(r);
			if (!owner.IsOnCharacter)
			{
				DeformData deformData = owner.GetDeformData(r);
				if (deformData != null && deformData.Layers != null)
				{
					layerCount = deformData.Layers.Count;
				}
				return;
			}
			ShapeEditorController characterController = owner.CharacterController;
			Dictionary<string, DeformData> allDeformData = characterController.GetAllDeformData();
			string text = path ?? "";
			DeformData deformData2;
			allDeformData.TryGetValue(text, out deformData2);
			if (deformData2 == null || deformData2.Layers == null || deformData2.Layers.Count == 0)
			{
				return;
			}
			layerCount = deformData2.Layers.Count;
			List<PsdDriver> drivers = characterController.Drivers;
			if (drivers != null)
			{
				for (int i = 0; i < drivers.Count; i++)
				{
					PsdDriver psdDriver = drivers[i];
					if (psdDriver != null && !((psdDriver.TargetRendererPath ?? "") != text) && PsdEvaluator.FindLayerById(allDeformData, psdDriver.TargetRendererPath, psdDriver.TargetLayerId) != null)
					{
						psdCount++;
					}
				}
			}
			List<CsbDriver> csbDrivers = characterController.CsbDrivers;
			if (csbDrivers != null)
			{
				for (int j = 0; j < csbDrivers.Count; j++)
				{
					CsbDriver csbDriver = csbDrivers[j];
					if (csbDriver != null && !((csbDriver.TargetRendererPath ?? "") != text) && PsdEvaluator.FindLayerById(allDeformData, csbDriver.TargetRendererPath, csbDriver.TargetLayerId) != null)
					{
						csbCount++;
					}
				}
			}
			List<NbbDriver> nbbDrivers = characterController.NbbDrivers;
			if (nbbDrivers != null)
			{
				for (int k = 0; k < nbbDrivers.Count; k++)
				{
					NbbDriver nbbDriver = nbbDrivers[k];
					if (nbbDriver != null && !((nbbDriver.TargetRendererPath ?? "") != text) && PsdEvaluator.FindLayerById(allDeformData, nbbDriver.TargetRendererPath, nbbDriver.TargetLayerId) != null)
					{
						nbbCount++;
					}
				}
			}
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0001F99C File Offset: 0x0001DB9C
		private static bool IsRendererEdited(Renderer r)
		{
			if (r == null)
			{
				return false;
			}
			if (MeshHelper.GetSubdivisionLevel(r) > 0)
			{
				return true;
			}
			DeformData deformData = ControllerResolver.Resolve(r).GetDeformData(r);
			return deformData != null && ((deformData.Layers != null && deformData.Layers.Count > 0) || (deformData.DeletedFaces != null && deformData.DeletedFaces.Count > 0));
		}

		// Token: 0x06000394 RID: 916 RVA: 0x0001FA08 File Offset: 0x0001DC08
		private DeformData ResolveDataForView()
		{
			if (this.Renderers == null || this.Renderers.Count == 0)
			{
				return null;
			}
			int primaryRendererIndex = this.PrimaryRendererIndex;
			if (primaryRendererIndex < 0 || primaryRendererIndex >= this.Renderers.Count)
			{
				return null;
			}
			Renderer renderer = this.Renderers[primaryRendererIndex];
			if (renderer == null)
			{
				return null;
			}
			return ControllerResolver.Resolve(renderer).GetDeformData(renderer);
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0001FA70 File Offset: 0x0001DC70
		private void DrawHelpWindow(int id)
		{
			using (UI.FlatSkinScope.Begin())
			{
				UI.BeginPanel(L.SectionHelp);
				this._helpScroll = GUILayout.BeginScrollView(this._helpScroll, Array.Empty<GUILayoutOption>());
				string text2;
				if (this._tabIndex == 0)
				{
					string text = L.FormatUndoRedoHint(ShapeEditorPlugin.UndoKey.Value, ShapeEditorPlugin.RedoKey.Value);
					text2 = string.Format((this.OperationMode == ShapeEditorWindow.OpMode.Brush) ? L.HelpBrush : L.HelpGizmo, text);
				}
				else if (this._tabIndex == 1)
				{
					text2 = L.HelpSubdivide + "\n\n" + L.FaceMaskHelp;
				}
				else if (this._tabIndex == ShapeEditorWindow.PresetTabIndex)
				{
					text2 = L.HelpPreset;
				}
				else if (this._tabIndex == ShapeEditorWindow.PsdTabIndex)
				{
					text2 = L.PsdHelp;
				}
				else if (this._tabIndex == ShapeEditorWindow.CsbTabIndex)
				{
					text2 = L.CsbHelp;
				}
				else if (this._tabIndex == ShapeEditorWindow.NbbTabIndex)
				{
					text2 = L.NbbHelp;
				}
				else
				{
					text2 = L.HelpSubdivide;
				}
				GUILayout.Label(text2, Theme.HelpLabelStyle, Array.Empty<GUILayoutOption>());
				GUILayout.EndScrollView();
				UI.EndPanel();
			}
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0001FB90 File Offset: 0x0001DD90
		private void DrawShapeTab()
		{
			DeformData activeDeformData = this.ActiveDeformData;
			this.DrawRendererSelection();
			if (this.IsEditMode)
			{
				this.DrawDeformDataExportImport();
			}
			if (this.IsEditMode && this.IsActiveRendererCorrupted())
			{
				GUILayout.Space(3f);
				GUILayout.Label(L.CorruptedEditModeBanner, Theme.BannerStyle, Array.Empty<GUILayoutOption>());
			}
			GUILayout.Space(2f);
			if (!this.IsEditMode)
			{
				bool enabled = GUI.enabled;
				if (this.FaceSelect != null)
				{
					GUI.enabled = false;
				}
				if (GUILayout.Button(L.EnterEditMode, Array.Empty<GUILayoutOption>()))
				{
					this.DeferEnterEditMode = true;
				}
				GUI.enabled = enabled;
				GUILayout.Space(2f);
				this.DrawLayerPanelWeightOnly(this.ResolveDataForView());
				return;
			}
			if (GUILayout.Button(L.ExitEditMode, Array.Empty<GUILayoutOption>()))
			{
				this.DeferExitEditMode = true;
			}
			GUILayout.Space(2f);
			if (activeDeformData == null || !activeDeformData.HasLayers)
			{
				GUILayout.Label(L.NoLayerWarning, Array.Empty<GUILayoutOption>());
				this.DrawLayerPanel(activeDeformData);
				return;
			}
			UI.BeginPanel(L.SectionOperationMode);
			Rect[] array = UI.SplitRow(2, 22f, 2f);
			if (GUI.Toggle(array[0], this.OperationMode == ShapeEditorWindow.OpMode.Brush, L.BrushMode, "Button"))
			{
				this.OperationMode = ShapeEditorWindow.OpMode.Brush;
			}
			if (GUI.Toggle(array[1], this.OperationMode == ShapeEditorWindow.OpMode.Gizmo, L.GizmoMode, "Button"))
			{
				this.OperationMode = ShapeEditorWindow.OpMode.Gizmo;
			}
			UI.EndPanel();
			if (this.OperationMode == ShapeEditorWindow.OpMode.Brush)
			{
				this.DrawBrushControls();
			}
			else
			{
				this.DrawGizmoControls();
			}
			GUILayout.Space(4f);
			this.DrawLayerPanel(activeDeformData);
			if (this.IsOnCharacter)
			{
				this.DrawWeightRemapControls();
			}
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0001FD38 File Offset: 0x0001DF38
		private void DrawWeightRemapControls()
		{
			UI.BeginPanel(L.SectionWeightRemap);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool enabled = GUI.enabled;
			if (!this.BodyMeshReadable)
			{
				GUI.enabled = false;
				GUILayout.Button(L.RemapWeights, Array.Empty<GUILayoutOption>());
				GUI.enabled = enabled;
			}
			else if (GUILayout.Button(L.RemapWeights, Array.Empty<GUILayoutOption>()))
			{
				this.DeferRemapWeights = true;
			}
			if (GUILayout.Button(L.RestoreWeights, Array.Empty<GUILayoutOption>()))
			{
				this.DeferRestoreWeights = true;
			}
			GUILayout.EndHorizontal();
			if (!this.BodyMeshReadable)
			{
				GUILayout.Label(L.BodyMeshNotReadable, Array.Empty<GUILayoutOption>());
			}
			UI.EndPanel();
		}

		// Token: 0x06000398 RID: 920 RVA: 0x0001FDD8 File Offset: 0x0001DFD8
		private void DrawBrushControls()
		{
			UI.BeginPanel(L.SectionBrush);

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Toggle(this.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Move, L.MoveTool, "Button", Array.Empty<GUILayoutOption>()))
			{
				this.SelectedBrushTool = ShapeEditorWindow.BrushToolType.Move;
			}
			if (GUILayout.Toggle(this.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Smooth, L.SmoothTool, "Button", Array.Empty<GUILayoutOption>()))
			{
				this.SelectedBrushTool = ShapeEditorWindow.BrushToolType.Smooth;
			}
			if (GUILayout.Toggle(this.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Relax, L.RelaxTool, "Button", Array.Empty<GUILayoutOption>()))
			{
				this.SelectedBrushTool = ShapeEditorWindow.BrushToolType.Relax;
			}
			if (GUILayout.Toggle(this.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Inflate, L.InflateTool, "Button", Array.Empty<GUILayoutOption>()))
			{
				this.SelectedBrushTool = ShapeEditorWindow.BrushToolType.Inflate;
			}
			if (GUILayout.Toggle(this.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Pinch, L.PinchTool, "Button", Array.Empty<GUILayoutOption>()))
			{
				this.SelectedBrushTool = ShapeEditorWindow.BrushToolType.Pinch;
			}
			if (GUILayout.Toggle(this.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Crease, L.CreaseTool, "Button", Array.Empty<GUILayoutOption>()))
			{
				this.SelectedBrushTool = ShapeEditorWindow.BrushToolType.Crease;
			}
			if (GUILayout.Toggle(this.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Seamless, "Seamless", "Button", Array.Empty<GUILayoutOption>()))
			{
				this.SelectedBrushTool = ShapeEditorWindow.BrushToolType.Seamless;
			}
			GUILayout.EndHorizontal();

			this.BrushRadius = UI.Slider(L.BrushRadiusLabel, this.BrushRadius, 0.001f, ShapeEditorPlugin.MaxBrushRadius.Value, "F3", null);
			this.BrushStrength = UI.Slider(L.StrengthLabel, this.BrushStrength, 0.01f, 1f, "F2", null);

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool value = ShapeEditorPlugin.UsePressure.Value;
			bool flag = UI.Checkbox(value, L.PressureLabel, null);
			if (flag != value)
			{
				ShapeEditorPlugin.UsePressure.Value = flag;
			}
			if (flag && !TabletManager.IsAvailable)
			{
				GUILayout.Label(L.TabletNotDetected, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Toggle(this.BrushFalloff == FalloffMode.Linear, L.FalloffLinear, "Button", Array.Empty<GUILayoutOption>()))
			{
				this.BrushFalloff = FalloffMode.Linear;
			}
			if (GUILayout.Toggle(this.BrushFalloff == FalloffMode.Smooth, L.FalloffSmooth, "Button", Array.Empty<GUILayoutOption>()))
			{
				this.BrushFalloff = FalloffMode.Smooth;
			}
			if (GUILayout.Toggle(this.BrushFalloff == FalloffMode.Sharp, L.FalloffSharp, "Button", Array.Empty<GUILayoutOption>()))
			{
				this.BrushFalloff = FalloffMode.Sharp;
			}
			GUILayout.EndHorizontal();

			if (this.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Seamless)
			{
				GUILayout.Space(4f);

				GUILayout.Label(
					"Seamless settings",
					Theme.DimHintStyle,
					Array.Empty<GUILayoutOption>());

				GUILayout.Label(
					"Seamless target character",
					Theme.DimHintStyle,
					Array.Empty<GUILayoutOption>());

				if (this.SeamlessCharacters == null || this.SeamlessCharacters.Count == 0)
				{
					GUILayout.Label(
						"No characters available.",
						Theme.DimHintStyle,
						Array.Empty<GUILayoutOption>());
				}
				else
				{
					string[] characterNames = new string[this.SeamlessCharacters.Count];

					for (int i = 0; i < this.SeamlessCharacters.Count; i++)
					{
						SeamlessCharacterOption option = this.SeamlessCharacters[i];
						characterNames[i] = option != null && !string.IsNullOrEmpty(option.Name)
							? option.Name
							: "Character " + i.ToString();
					}

					int selected = Mathf.Clamp(this.SeamlessCharacterIndex, 0, characterNames.Length - 1);
					int newSelected = GUILayout.SelectionGrid(selected, characterNames, 1, "Button");

					if (newSelected != selected)
					{
						this.SeamlessCharacterIndex = newSelected;
					}
				}

				GUILayout.Space(3f);

				GUILayout.Label(
					"Target part",
					Theme.DimHintStyle,
					Array.Empty<GUILayoutOption>());

				this.SeamlessBodyEnabled = UI.Checkbox(
					this.SeamlessBodyEnabled,
					"Body",
					"Use the target character body.");

				this.SeamlessHeadEnabled = UI.Checkbox(
					this.SeamlessHeadEnabled,
					"Head",
					"Use the target character head.");

				this.SeamlessBlendDistance = UI.Slider(
					"Blend Distance",
					this.SeamlessBlendDistance,
					0.001f,
					0.25f,
					"F4",
					"Maximum distance from the target surface.");

				this.SeamlessBorderStrength = UI.Slider(
					"Border Strength",
					this.SeamlessBorderStrength,
					0f,
					1f,
					"F2",
					"How strongly the border is corrected.");

				this.SeamlessBorderOffset = UI.Slider(
					"Border Offset",
					this.SeamlessBorderOffset,
					-0.05f,
					0.05f,
					"F4",
					"Offset from the target surface.");

				this.SeamlessBlendStrength = UI.Slider(
					"Blend Strength",
					this.SeamlessBlendStrength,
					0f,
					1f,
					"F2",
					"Falloff strength around the seam.");

				GUILayout.Label(
					"Select a target character and Body or Head.",
					Theme.DimHintStyle,
					Array.Empty<GUILayoutOption>());
			}

			UI.EndPanel();

			this.DrawSymmetryControls();
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00020040 File Offset: 0x0001E240
		private void DrawGizmoControls()
		{
			UI.BeginPanel(L.SectionGizmo);
			Rect[] array = UI.SplitRow(3, 22f, 2f);
			if (GUI.Toggle(array[0], this.GizmoModeIndex == 0, L.Translate, "Button"))
			{
				this.GizmoModeIndex = 0;
			}
			if (GUI.Toggle(array[1], this.GizmoModeIndex == 1, L.Rotate, "Button"))
			{
				this.GizmoModeIndex = 1;
			}
			if (GUI.Toggle(array[2], this.GizmoModeIndex == 2, L.Scale, "Button"))
			{
				this.GizmoModeIndex = 2;
			}
			GUILayout.Space(2f);
			Rect[] array2 = UI.SplitRow(3, 22f, 2f);
			if (GUI.Toggle(array2[0], this.GizmoSpaceIndex == 0, L.WorldSpace, "Button"))
			{
				this.GizmoSpaceIndex = 0;
			}
			if (GUI.Toggle(array2[1], this.GizmoSpaceIndex == 1, L.ObjectSpace, "Button"))
			{
				this.GizmoSpaceIndex = 1;
			}
			if (GUI.Toggle(array2[2], this.GizmoSpaceIndex == 2, L.NormalSpace, "Button"))
			{
				this.GizmoSpaceIndex = 2;
			}
			GUILayout.Space(2f);
			Rect[] array3 = UI.SplitRow(2, 22f, 2f);
			if (GUI.Toggle(array3[0], this.GizmoSelection == ShapeEditorWindow.GizmoSelectMethod.Box, L.GizmoSelectBox, "Button"))
			{
				this.GizmoSelection = ShapeEditorWindow.GizmoSelectMethod.Box;
			}
			if (GUI.Toggle(array3[1], this.GizmoSelection == ShapeEditorWindow.GizmoSelectMethod.Brush, L.GizmoSelectBrush, "Button"))
			{
				this.GizmoSelection = ShapeEditorWindow.GizmoSelectMethod.Brush;
			}
			GUILayout.Space(2f);
			Rect[] array4 = UI.SplitRow(2, 22f, 2f);
			if (GUI.Toggle(array4[0], this.SelectionMode == ShapeEditorWindow.SelectMode.Vertex, L.GizmoGranularityVertex, "Button"))
			{
				this.SelectionMode = ShapeEditorWindow.SelectMode.Vertex;
			}
			if (GUI.Toggle(array4[1], this.SelectionMode == ShapeEditorWindow.SelectMode.Face, L.GizmoGranularityFace, "Button"))
			{
				this.SelectionMode = ShapeEditorWindow.SelectMode.Face;
			}
			if (GUILayout.Button(L.GizmoClearSelection, new GUILayoutOption[] { GUILayout.Height(22f) }))
			{
				this.DeferClearGizmoSelection = true;
			}
			bool value = ShapeEditorPlugin.IncludeBackFaceVertices.Value;
			bool flag = UI.Checkbox(value, L.IncludeBackFaceLabel, L.IncludeBackFaceTooltip);
			if (flag != value)
			{
				ShapeEditorPlugin.IncludeBackFaceVertices.Value = flag;
			}
			this.GizmoSoftSelection = UI.Checkbox(this.GizmoSoftSelection, L.SoftSelection, null);
			if (this.GizmoSoftSelection)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (GUILayout.Toggle(this.SoftSelectModeIndex == 0, L.SoftModeVolume, "Button", Array.Empty<GUILayoutOption>()))
				{
					this.SoftSelectModeIndex = 0;
				}
				if (GUILayout.Toggle(this.SoftSelectModeIndex == 1, L.SoftModeSurface, "Button", Array.Empty<GUILayoutOption>()))
				{
					this.SoftSelectModeIndex = 1;
				}
				GUILayout.EndHorizontal();
				this.GizmoSoftRadius = UI.Slider(L.SoftSelectionRadius, this.GizmoSoftRadius, 0.001f, ShapeEditorPlugin.MaxSoftSelectionRadius.Value, "F3", null);
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (GUILayout.Toggle(this.GizmoFalloff == FalloffMode.Linear, L.FalloffLinear, "Button", Array.Empty<GUILayoutOption>()))
				{
					this.GizmoFalloff = FalloffMode.Linear;
				}
				if (GUILayout.Toggle(this.GizmoFalloff == FalloffMode.Smooth, L.FalloffSmooth, "Button", Array.Empty<GUILayoutOption>()))
				{
					this.GizmoFalloff = FalloffMode.Smooth;
				}
				if (GUILayout.Toggle(this.GizmoFalloff == FalloffMode.Sharp, L.FalloffSharp, "Button", Array.Empty<GUILayoutOption>()))
				{
					this.GizmoFalloff = FalloffMode.Sharp;
				}
				GUILayout.EndHorizontal();
			}
			UI.EndPanel();
			this.DrawSymmetryControls();
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00020414 File Offset: 0x0001E614
		private void DrawSymmetryControls()
		{
			UI.BeginPanel(L.SectionSymmetry);
			this.SymmetryEnabled = UI.Checkbox(this.SymmetryEnabled, L.Symmetry, null);
			if (this.SymmetryEnabled)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Label(L.SymmetryAxis, new GUILayoutOption[] { GUILayout.Width(30f) });
				if (GUILayout.Toggle(this.SymmetryAxisIndex == 0, "X", "Button", Array.Empty<GUILayoutOption>()))
				{
					this.SymmetryAxisIndex = 0;
				}
				if (GUILayout.Toggle(this.SymmetryAxisIndex == 1, "Y", "Button", Array.Empty<GUILayoutOption>()))
				{
					this.SymmetryAxisIndex = 1;
				}
				if (GUILayout.Toggle(this.SymmetryAxisIndex == 2, "Z", "Button", Array.Empty<GUILayoutOption>()))
				{
					this.SymmetryAxisIndex = 2;
				}
				GUILayout.EndHorizontal();
				if (this.SymmetryCenterSet)
				{
					GUILayout.Label(string.Format(L.SymmetryCenterFmt, this.SymmetryCenter), Array.Empty<GUILayoutOption>());
				}
				Rect[] array = UI.SplitRow(2, 22f, 2f);
				if (GUI.Button(array[0], L.SetCenter))
				{
					this.DeferSetSymmetryCenter = true;
				}
				if (GUI.Button(array[1], L.ClearCenter))
				{
					this.DeferClearSymmetryCenter = true;
				}
			}
			UI.EndPanel();
		}

		// Token: 0x0600039B RID: 923 RVA: 0x0002056C File Offset: 0x0001E76C
		private void DrawLayerPanel(DeformData data)
		{
			UI.BeginPanel(L.SectionLayers);
			if (GUILayout.Button(L.AddLayer, new GUILayoutOption[] { GUILayout.Height(22f) }))
			{
				this.DeferLayerAdd = true;
			}
			if (data == null || data.Layers.Count == 0)
			{
				UI.EndPanel();
				return;
			}
			this._layerScroll = GUILayout.BeginScrollView(this._layerScroll, new GUILayoutOption[] { GUILayout.Height(140f) });
			for (int i = 0; i < data.Layers.Count; i++)
			{
				DeformLayer deformLayer = data.Layers[i];
				bool flag = data.ActiveLayerIndex == i;
				int num = i;
				UI.BeginRowBg(i);
				Rect rect = GUILayoutUtility.GetRect(20f, 22f, new GUILayoutOption[]
				{
					GUILayout.Width(20f),
					GUILayout.Height(22f)
				});
				Rect rect2;
				rect2 = new Rect(rect.x + 3f, rect.y + (rect.height - 14f) * 0.5f, 14f, 14f);
				if (Event.current.type == EventType.Repaint)
				{
					UI.DrawToggleBox(rect2, flag);
				}
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
				{
					if (!flag)
					{
						data.SetActiveLayer(num);
					}
					Event.current.Use();
				}
				if (this._renamingLayerIndex == num)
				{
					Rect rect3 = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
					Rect rect4;
					rect4 = new Rect(rect3.x, rect3.y, rect3.width - 30f - 2f, rect3.height);
					Rect rect5 = new Rect(rect3.xMax - 30f, rect3.y, 30f, rect3.height);
					this._renamingText = GUI.TextField(rect4, this._renamingText, Theme.RowTextFieldStyle);
					if (GUI.Button(rect5, "OK", Theme.RowButtonStyle))
					{
						if (!string.IsNullOrEmpty(this._renamingText))
						{
							this.DeferLayerRename = num;
							this.DeferLayerRenameNewName = this._renamingText;
						}
						this._renamingLayerIndex = -1;
					}
				}
				else
				{
					float weight = deformLayer.Weight;
					float num2 = UI.Slider(deformLayer.Name, weight, 0f, 1f, "F2", null);
					if (!Mathf.Approximately(num2, weight))
					{
						if (this._weightSliderLayer != num)
						{
							this._weightSliderLayer = num;
							this._weightSliderBefore = weight;
						}
						data.SetLayerWeight(num, num2);
					}
					else if (this._weightSliderLayer == num)
					{
						this.WeightUndoLayer = num;
						this.WeightUndoBefore = this._weightSliderBefore;
						this.WeightUndoAfter = deformLayer.Weight;
						this._weightSliderLayer = -1;
					}
				}
				UI.EndRowBg();
			}
			GUILayout.EndScrollView();
			int activeLayerIndex = data.ActiveLayerIndex;
			Rect[] array = UI.SplitRow(5, 22f, 2f);
			if (GUI.Button(array[0], L.RemoveLayer) && activeLayerIndex >= 0)
			{
				this.DeferLayerRemove = activeLayerIndex;
			}
			if (GUI.Button(array[1], L.RenameLayer) && activeLayerIndex >= 0)
			{
				this._renamingLayerIndex = activeLayerIndex;
				this._renamingText = data.Layers[activeLayerIndex].Name;
			}
			ShapeEditorWindow._scratchContent.text = L.MirrorLayer;
			ShapeEditorWindow._scratchContent.tooltip = L.MirrorLayerTooltip;
			if (GUI.Button(array[2], ShapeEditorWindow._scratchContent) && activeLayerIndex >= 0)
			{
				this.DeferLayerMirror = activeLayerIndex;
			}
			if (GUI.Button(array[3], L.MoveUp) && activeLayerIndex > 0)
			{
				this.DeferLayerMoveUp = activeLayerIndex;
			}
			if (GUI.Button(array[4], L.MoveDown) && activeLayerIndex < data.Layers.Count - 1)
			{
				this.DeferLayerMoveDown = activeLayerIndex;
			}
			UI.EndPanel();
		}

		// Token: 0x0600039C RID: 924 RVA: 0x00020940 File Offset: 0x0001EB40
		private void DrawLayerPanelWeightOnly(DeformData data)
		{
			if (data == null || data.Layers.Count == 0)
			{
				return;
			}
			UI.BeginPanel(L.SectionLayers);
			this._layerScroll = GUILayout.BeginScrollView(this._layerScroll, new GUILayoutOption[] { GUILayout.Height(140f) });
			for (int i = 0; i < data.Layers.Count; i++)
			{
				DeformLayer deformLayer = data.Layers[i];
				int num = i;
				UI.BeginRowBg(i);
				float num2 = UI.Slider(deformLayer.Name, deformLayer.Weight, 0f, 1f, "F2", null);
				if (!Mathf.Approximately(num2, deformLayer.Weight))
				{
					data.SetLayerWeight(num, num2);
				}
				UI.EndRowBg();
			}
			GUILayout.EndScrollView();
			UI.EndPanel();
		}

		// Token: 0x0600039D RID: 925 RVA: 0x00020A00 File Offset: 0x0001EC00
		private void DrawSubdivideTab()
		{
			this.DrawRendererSelection();
			if (this.SelectedRendererIndex < 0 || this.SelectedRendererIndex >= this.Renderers.Count)
			{
				return;
			}
			Renderer renderer = this.Renderers[this.SelectedRendererIndex];
			if (renderer == null)
			{
				return;
			}
			Mesh mesh = MeshHelper.GetMesh(renderer);
			if (mesh == null)
			{
				return;
			}
			UI.BeginPanel(L.SectionSubdivide);
			int totalFaceCount = MeshHelper.GetTotalFaceCount(mesh);
			GUILayout.Label(string.Format(L.VerticesFacesFmt, mesh.vertexCount, totalFaceCount), Array.Empty<GUILayoutOption>());
			GUILayout.Space(2f);
			bool flag = this.FaceSelect != null;
			bool enabled = GUI.enabled;
			if (this.IsEditMode)
			{
				GUI.enabled = false;
			}
			bool flag2 = UI.Checkbox(flag, L.SelectFaces, null);
			if (this.IsEditMode)
			{
				GUI.enabled = enabled;
			}
			if (flag2 != flag)
			{
				if (flag2)
				{
					this.FaceSelect = FaceSelectOverlay.Create(renderer);
				}
				else if (this.FaceSelect != null)
				{
					UnityEngine.Object.Destroy(this.FaceSelect.gameObject);
					this.FaceSelect = null;
				}
			}
			if (this.FaceSelect != null)
			{
				Rect[] array = UI.SplitRow(2, 22f, 2f);
				if (GUI.Toggle(array[0], !this.FaceSelect.BoxSelectMode, L.Brush, "Button"))
				{
					this.FaceSelect.BoxSelectMode = false;
				}
				if (GUI.Toggle(array[1], this.FaceSelect.BoxSelectMode, L.BoxSelect, "Button"))
				{
					this.FaceSelect.BoxSelectMode = true;
				}
				if (!this.FaceSelect.BoxSelectMode)
				{
					this.FaceSelect.BrushRadius = UI.Slider(L.BrushRadiusLabel, this.FaceSelect.BrushRadius, 0.001f, ShapeEditorPlugin.MaxBrushRadius.Value, "F3", null);
				}
				GUILayout.Label(string.Format(L.SelectedFacesFmt, this.FaceSelect.SelectedFaces.Count, this.FaceSelect.TotalFaces), Array.Empty<GUILayoutOption>());
				ShapeEditorPlugin.FaceWireframeOpacity.Value = UI.Slider(L.FaceWireframeOpacity, ShapeEditorPlugin.FaceWireframeOpacity.Value, 0f, 1f, "F2", L.FaceWireframeOpacityTooltip);
				ShapeEditorPlugin.FaceSelectIncludeBackFace.Value = UI.Checkbox(ShapeEditorPlugin.FaceSelectIncludeBackFace.Value, L.FaceSelectIncludeBackFace, L.FaceSelectIncludeBackFaceTooltip);
				Rect[] array2 = UI.SplitRow(3, 22f, 2f);
				if (GUI.Button(array2[0], L.AllButton))
				{
					this.DeferFaceSelectAll = true;
				}
				if (GUI.Button(array2[1], L.NoneButton))
				{
					this.DeferFaceSelectNone = true;
				}
				if (GUI.Button(array2[2], L.InvertButton))
				{
					this.DeferFaceSelectInvert = true;
				}
			}
			GUILayout.Space(3f);
			if (this.IsEditMode)
			{
				GUI.enabled = false;
			}
			this.SubdivideSmooth = UI.Checkbox(this.SubdivideSmooth, L.SubdivideSmooth, L.SubdivideSmoothTooltip);
			GUILayout.Space(2f);
			Rect rect = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(22f)
			});
			Rect rect2;
			rect2 = new Rect(rect.x, rect.y, 40f, rect.height);
			GUI.Label(rect2, L.LevelLabel, Theme.RowLabelStyle);
			float num = (rect.width - 40f - 6f) / 3f;
			Rect rect3;
			rect3 = new Rect(rect2.xMax + 2f, rect.y, num, rect.height);
			Rect rect4;
			rect4 = new Rect(rect3.xMax + 2f, rect.y, num, rect.height);
			Rect rect5 = new Rect(rect4.xMax + 2f, rect.y, num, rect.height);
			if (GUI.Toggle(rect3, this.SubdivideLevel == 1, "1", "Button"))
			{
				this.SubdivideLevel = 1;
			}
			if (GUI.Toggle(rect4, this.SubdivideLevel == 2, "2", "Button"))
			{
				this.SubdivideLevel = 2;
			}
			if (GUI.Toggle(rect5, this.SubdivideLevel == 3, "3", "Button"))
			{
				this.SubdivideLevel = 3;
			}
			GUILayout.Space(2f);
			bool flag3 = MeshHelper.HasOriginal(renderer);
			Rect rect6 = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(22f)
			});
			Rect rect7;
			if (flag3)
			{
				float num2 = (rect6.width - 2f) / 2f;
				rect7 = new Rect(rect6.x, rect6.y, num2, rect6.height);
				if (GUI.Button(new Rect(rect7.xMax + 2f, rect6.y, num2, rect6.height), L.Restore))
				{
					this.DeferRestore = true;
				}
			}
			else
			{
				rect7 = rect6;
			}
			bool enabled2 = GUI.enabled;
			if (this.FaceSelect != null && this.FaceSelect.SelectedFaces.Count == 0)
			{
				GUI.enabled = false;
			}
			if (GUI.Button(rect7, L.Subdivide))
			{
				if (this.ActiveDeformData != null && this.ActiveDeformData.HasLayers)
				{
					this.ShowSubdivideLayerWarning = true;
				}
				else
				{
					this.DeferSubdivide = true;
				}
			}
			GUI.enabled = enabled2;
			List<bool> subdivisionSmooth = MeshHelper.GetSubdivisionSmooth(renderer);
			bool flag4 = subdivisionSmooth != null && subdivisionSmooth.Contains(true);
			bool flag5 = MeshHelper.IsCharacterBody(renderer) && MeshHelper.GetSubdivisionLevel(renderer) > 0 && flag4;
			GUILayout.Space(2f);
			bool enabled3 = GUI.enabled;
			if (!flag5)
			{
				GUI.enabled = false;
			}
			ShapeEditorWindow._scratchContent.text = L.RebakeSubdivision;
			ShapeEditorWindow._scratchContent.tooltip = L.RebakeSubdivisionTooltip;
			if (GUI.Button(GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(22f)
			}), ShapeEditorWindow._scratchContent))
			{
				this.DeferRebakeBodySubdivision = true;
			}
			GUI.enabled = enabled3;
			if (this.IsEditMode)
			{
				GUI.enabled = enabled;
			}
			if (this.ShowSubdivideLayerWarning)
			{
				GUILayout.Space(2f);
				GUILayout.Label(L.SubdivideLayerWarning, Array.Empty<GUILayoutOption>());
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (GUILayout.Button(L.ApplyButton, new GUILayoutOption[] { GUILayout.Width(80f) }))
				{
					this.ShowSubdivideLayerWarning = false;
					this.DeferSubdivide = true;
				}
				if (GUILayout.Button(L.ClearButton, new GUILayoutOption[] { GUILayout.Width(80f) }))
				{
					this.ShowSubdivideLayerWarning = false;
				}
				GUILayout.EndHorizontal();
			}
			UI.EndPanel();
			if (this.FaceSelect != null)
			{
				UI.BeginPanel(L.SectionDeletedFaces);
				DeformData deformData = ControllerResolver.Resolve(renderer).GetDeformData(renderer);
				int num3 = ((deformData != null) ? deformData.DeletedFaces.Count : 0);
				GUILayout.Label(string.Format(L.DeletedFacesCountFmt, num3), Array.Empty<GUILayoutOption>());
				bool enabled4 = GUI.enabled;
				Rect[] array3 = UI.SplitRow(2, 22f, 2f);
				GUI.enabled = enabled4 && this.FaceSelect.SelectedFaces.Count > 0;
				if (GUI.Button(array3[0], L.DeleteSelectedFaces))
				{
					this.DeferFaceDelete = true;
				}
				GUI.enabled = enabled4;
				bool flag6 = false;
				if (num3 > 0 && this.FaceSelect.SelectedFaces.Count > 0)
				{
					foreach (int num4 in this.FaceSelect.SelectedFaces)
					{
						if (deformData.DeletedFaces.Contains(num4))
						{
							flag6 = true;
							break;
						}
					}
				}
				GUI.enabled = enabled4 && flag6;
				if (GUI.Button(array3[1], L.RestoreSelectedFaces))
				{
					this.DeferFaceRestore = true;
				}
				GUI.enabled = enabled4;
				GUILayout.Space(2f);
				Rect rect8 = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true),
					GUILayout.Height(22f)
				});
				GUI.enabled = enabled4 && num3 > 0;
				if (GUI.Button(rect8, L.RestoreAllDeletedFaces))
				{
					this.DeferFaceRestoreAll = true;
				}
				GUI.enabled = enabled4;
				UI.EndPanel();
			}
		}

		// Token: 0x0600039E RID: 926 RVA: 0x0002127C File Offset: 0x0001F47C
		private void DrawRendererSelection()
		{
			UI.BeginPanel(L.SectionRenderer);
			if (this.Renderers.Count == 0)
			{
				GUILayout.Label(L.SelectObject, Array.Empty<GUILayoutOption>());
				UI.EndPanel();
				return;
			}
			this.ShowMeshHighlight = UI.Checkbox(this.ShowMeshHighlight, L.ShowMeshHighlight, null);
			this.ShowMeshWireframe = UI.Checkbox(this.ShowMeshWireframe, L.ShowMeshWireframe, null);
			this._showEditedOnly = UI.Checkbox(this._showEditedOnly, L.ShowEditedOnly, null);
			GUILayout.Label(L.TargetMesh, Array.Empty<GUILayoutOption>());
			this._rendererFilter = GUILayout.TextField(this._rendererFilter, Array.Empty<GUILayoutOption>());
			bool flag = !string.IsNullOrEmpty(this._rendererFilter);
			bool enabled = GUI.enabled;
			if (this.IsEditMode)
			{
				GUI.enabled = false;
			}
			this._rendererScroll = GUILayout.BeginScrollView(this._rendererScroll, new GUILayoutOption[] { GUILayout.Height(120f) });
			Color backgroundColor = GUI.backgroundColor;
			Color contentColor = GUI.contentColor;
			int num = 0;
			for (int i = 0; i < this.Renderers.Count; i++)
			{
				if (!(this.Renderers[i] == null) && (!flag || this.Renderers[i].name.IndexOf(this._rendererFilter, StringComparison.OrdinalIgnoreCase) >= 0) && (!this._showEditedOnly || ShapeEditorWindow.IsRendererEdited(this.Renderers[i])))
				{
					int num2 = num++;
					int num3 = i;
					bool flag2 = num3 == this.PrimaryRendererIndex && this.SelectedRendererIndices.Contains(num3);
					bool flag3 = !flag2 && this.SelectedRendererIndices.Contains(num3);
					string text = ((this.RendererPaths != null && num3 < this.RendererPaths.Count) ? this.RendererPaths[num3] : null);
					object obj = text != null && this.CorruptionState != null && this.CorruptionState.ContainsKey(text);
					string text2 = L.CategoryLabel((this.RendererCategories != null && num3 < this.RendererCategories.Count) ? this.RendererCategories[num3] : null);
					string text3 = (string.IsNullOrEmpty(text2) ? "" : ("[" + text2 + "] "));
					string text4 = (flag2 ? "● " : (flag3 ? "○ " : "")) + text3 + this.Renderers[num3].name;
					object obj2 = obj;
					string text5 = ((obj2 != null) ? (L.CorruptedEntryPrefix + text4) : text4);
					if (obj2 != null)
					{
						UI.BeginRowBg(num2);
						if (flag2)
						{
							GUI.backgroundColor = ShapeEditorWindow._primaryTint;
						}
						else if (flag3)
						{
							GUI.backgroundColor = ShapeEditorWindow._secondaryTint;
						}
						GUI.contentColor = ShapeEditorWindow._corruptedText;
						ShapeEditorWindow._scratchContent.text = text5;
						ShapeEditorWindow._scratchContent.tooltip = L.CorruptedTooltipFormat;
						bool flag4 = this.SelectedRendererIndices.Contains(num3);
						bool flag5 = GUILayout.Toggle(flag4, ShapeEditorWindow._scratchContent, "Button", Array.Empty<GUILayoutOption>());
						GUI.contentColor = contentColor;
						GUI.backgroundColor = backgroundColor;
						if (flag5 != flag4)
						{
							bool flag6 = InputHelper.IsCtrlHeldDirect();
							this.HandleRendererClick(num3, flag6);
						}
						this.DrawVisibilityIndicator(this.Renderers[num3]);
						bool enabled2 = GUI.enabled;
						GUI.enabled = true;
						Color backgroundColor2 = GUI.backgroundColor;
						GUI.backgroundColor = ShapeEditorWindow._resetButtonBg;
						if (GUILayout.Button(L.ResetCorruptedButton, new GUILayoutOption[] { GUILayout.Width(60f) }))
						{
							this._pendingResetConfirmPath = text;
							this._showResetConfirm = true;
						}
						GUI.backgroundColor = backgroundColor2;
						GUI.enabled = enabled2;
						UI.EndRowBg();
					}
					else
					{
						UI.BeginRowBg(num2);
						if (flag2)
						{
							GUI.backgroundColor = ShapeEditorWindow._primaryTint;
						}
						else if (flag3)
						{
							GUI.backgroundColor = ShapeEditorWindow._secondaryTint;
						}
						bool flag7 = this.SelectedRendererIndices.Contains(num3);
						bool flag8 = GUILayout.Toggle(flag7, text5, "Button", Array.Empty<GUILayoutOption>());
						GUI.backgroundColor = backgroundColor;
						this.DrawVisibilityIndicator(this.Renderers[num3]);
						if (flag8 != flag7)
						{
							bool flag9 = InputHelper.IsCtrlHeldDirect();
							this.HandleRendererClick(num3, flag9);
						}
						UI.EndRowBg();
					}
				}
			}
			GUILayout.EndScrollView();
			if (this.IsEditMode)
			{
				GUI.enabled = enabled;
			}
			if (StudioAPI.InsideStudio)
			{
				bool flag10 = this.PrimaryRendererIndex >= 0 && this.PrimaryRendererIndex < this.Renderers.Count && this.Renderers[this.PrimaryRendererIndex] != null && this.Renderers[this.PrimaryRendererIndex].gameObject.activeInHierarchy && this.SelectedRendererIndices.Contains(this.PrimaryRendererIndex);
				bool enabled3 = GUI.enabled;
				if (!flag10)
				{
					GUI.enabled = false;
				}
				if (GUILayout.Button(L.FocusRenderer, Array.Empty<GUILayoutOption>()))
				{
					this.DeferFocusRenderer = true;
				}
				GUI.enabled = enabled3;
			}
			UI.EndPanel();
		}

		// Token: 0x0600039F RID: 927 RVA: 0x00021758 File Offset: 0x0001F958
		private void DrawVisibilityIndicator(Renderer r)
		{
			if (ShapeEditorWindow._visibilityLabelStyle == null)
			{
				ShapeEditorWindow._visibilityLabelStyle = new GUIStyle(GUI.skin.label)
				{
					alignment = (TextAnchor)5,
					margin = new RectOffset(0, 0, 0, 0)
				};
			}
			bool flag = r != null && r.gameObject.activeInHierarchy;
			Color contentColor = GUI.contentColor;
			if (!flag)
			{
				GUI.contentColor = ShapeEditorWindow._dimText;
			}
			ShapeEditorWindow._scratchContent.text = (flag ? L.VisibilityShow : L.VisibilityHide);
			ShapeEditorWindow._scratchContent.tooltip = null;
			GUILayout.Label(ShapeEditorWindow._scratchContent, ShapeEditorWindow._visibilityLabelStyle, ShapeEditorWindow._visibilityWidthOpt);
			GUI.contentColor = contentColor;
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x00021800 File Offset: 0x0001FA00
		private void DrawDeformDataExportImport()
		{
			UI.BeginPanel(L.SectionDeformDataIO);
			Rect[] array = UI.SplitRow(2, 22f, 2f);
			bool flag = this.ActiveDeformData != null && this.ActiveDeformData.Layers.Count > 0;
			bool enabled = GUI.enabled;
			if (!flag)
			{
				GUI.enabled = false;
			}
			if (GUI.Button(array[0], L.ExportDeform))
			{
				this.DeferExport = true;
			}
			GUI.enabled = enabled;
			bool flag2 = this.SelectedRendererIndex >= 0 && this.SelectedRendererIndex < this.Renderers.Count && this.Renderers[this.SelectedRendererIndex] != null;
			bool enabled2 = GUI.enabled;
			if (!flag2 || !this.IsEditMode)
			{
				GUI.enabled = false;
			}
			if (GUI.Button(array[1], L.ImportDeform))
			{
				this.DeferImport = true;
			}
			GUI.enabled = enabled2;
			UI.EndPanel();
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x000218E4 File Offset: 0x0001FAE4
		public void SanitizeSelectionAgainstRenderers()
		{
			if (this.Renderers == null || this.Renderers.Count == 0)
			{
				this.SelectedRendererIndices.Clear();
				this.PrimaryRendererIndex = 0;
				return;
			}
			this.SelectedRendererIndices.RemoveWhere((int i) => i < 0 || i >= this.Renderers.Count || this.Renderers[i] == null);
			if (this.PrimaryRendererIndex < 0 || this.PrimaryRendererIndex >= this.Renderers.Count || !(this.Renderers[this.PrimaryRendererIndex] != null) || !this.SelectedRendererIndices.Contains(this.PrimaryRendererIndex))
			{
				int num = -1;
				foreach (int num2 in this.SelectedRendererIndices)
				{
					if (num < 0 || num2 < num)
					{
						num = num2;
					}
				}
				if (num < 0)
				{
					num = 0;
				}
				this.SelectedRendererIndex = num;
			}
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x000219D4 File Offset: 0x0001FBD4
		public void HandleRendererClick(int clickedIndex, bool ctrlHeld)
		{
			if (clickedIndex < 0 || clickedIndex >= this.Renderers.Count)
			{
				return;
			}
			if (this.Renderers[clickedIndex] == null)
			{
				return;
			}
			if (!ctrlHeld)
			{
				this.PrimaryRendererIndex = clickedIndex;
				this.SelectedRendererIndices.Clear();
				this.SelectedRendererIndices.Add(clickedIndex);
				return;
			}
			if (this.SelectedRendererIndices.Contains(clickedIndex))
			{
				if (this.SelectedRendererIndices.Count <= 1)
				{
					return;
				}
				this.SelectedRendererIndices.Remove(clickedIndex);
				if (this.PrimaryRendererIndex == clickedIndex)
				{
					int num = -1;
					foreach (int num2 in this.SelectedRendererIndices)
					{
						if (num < 0 || num2 < num)
						{
							num = num2;
						}
					}
					if (num >= 0)
					{
						this.PrimaryRendererIndex = num;
						return;
					}
				}
			}
			else
			{
				this.SelectedRendererIndices.Add(clickedIndex);
			}
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00021AC4 File Offset: 0x0001FCC4
		private bool IsActiveRendererCorrupted()
		{
			if (this.CorruptionState == null || this.RendererPaths == null)
			{
				return false;
			}
			if (this.PrimaryRendererIndex < 0 || this.PrimaryRendererIndex >= this.RendererPaths.Count)
			{
				return false;
			}
			string text = this.RendererPaths[this.PrimaryRendererIndex];
			return text != null && this.CorruptionState.ContainsKey(text);
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00021B24 File Offset: 0x0001FD24
		private int GetLayerCountForPath(string path)
		{
			if (string.IsNullOrEmpty(path) || this.Renderers == null || this.RendererPaths == null)
			{
				return 0;
			}
			int num = this.RendererPaths.IndexOf(path);
			if (num < 0 || num >= this.Renderers.Count)
			{
				return 0;
			}
			Renderer renderer = this.Renderers[num];
			if (renderer == null)
			{
				return 0;
			}
			DeformData deformData = ControllerResolver.Resolve(renderer).GetDeformData(renderer);
			if (deformData == null)
			{
				return 0;
			}
			return deformData.Layers.Count;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00021BA4 File Offset: 0x0001FDA4
		private void DrawResetConfirmPopup(int id)
		{
			using (UI.FlatSkinScope.Begin())
			{
				UI.BeginPanel(L.SectionResetConfirm);
				GUILayout.Space(4f);
				GUILayout.Label(string.Format(L.ResetCorruptedConfirmBody, this._pendingResetConfirmPath ?? "", this.GetLayerCountForPath(this._pendingResetConfirmPath)), Array.Empty<GUILayoutOption>());
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				Color backgroundColor = GUI.backgroundColor;
				GUI.backgroundColor = ShapeEditorWindow._resetButtonBg;
				if (GUILayout.Button(L.ConfirmButton, new GUILayoutOption[] { GUILayout.Height(28f) }))
				{
					this.DeferResetCorruptedLayers = true;
					this.DeferResetPath = this._pendingResetConfirmPath;
					this._showResetConfirm = false;
					this._pendingResetConfirmPath = null;
				}
				GUI.backgroundColor = backgroundColor;
				if (GUILayout.Button(L.CancelButton, new GUILayoutOption[] { GUILayout.Height(28f) }))
				{
					this._showResetConfirm = false;
					this._pendingResetConfirmPath = null;
				}
				GUILayout.EndHorizontal();
				UI.EndPanel();
			}
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x00021CB8 File Offset: 0x0001FEB8
		public void Cleanup()
		{
			if (this.FaceSelect != null)
			{
				UnityEngine.Object.Destroy(this.FaceSelect.gameObject);
				this.FaceSelect = null;
			}
			if (ShapeEditorWindow._activeInstance == this)
			{
				ShapeEditorWindow._activeInstance = null;
			}
			Theme.Destroy();
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x00021CF4 File Offset: 0x0001FEF4
		private void DrawCsbTab()
		{
			this.DrawRendererSelection();
			GUILayout.Space(2f);
			ShapeEditorController shapeEditorController = this.ResolveDriverTabOwner();
			if (shapeEditorController == null)
			{
				UI.BeginPanel(L.CsbSectionTitle);
				GUILayout.Label(L.CsbCharacterOnlyHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
				UI.EndPanel();
				return;
			}
			this.DrawCsbDriverList(shapeEditorController);
			GUILayout.Space(2f);
			this.DrawCsbAddForm(shapeEditorController);
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00021D60 File Offset: 0x0001FF60
		private void DrawCsbDriverList(ShapeEditorController owner)
		{
			UI.BeginPanel(L.CsbSectionTitle);
			List<CsbDriver> csbDrivers = owner.CsbDrivers;
			Dictionary<string, DeformData> allDeformData = owner.GetAllDeformData();
			bool isAvailable = TimelineCompat.IsAvailable;
			int frameCount = Time.frameCount;
			this._csbDriverScroll = GUILayout.BeginScrollView(this._csbDriverScroll, new GUILayoutOption[] { GUILayout.Height(180f) });
			CsbDriver csbDriver = null;
			int num = 0;
			for (int i = 0; i < csbDrivers.Count; i++)
			{
				CsbDriver csbDriver2 = csbDrivers[i];
				if (csbDriver2 != null)
				{
					DeformLayer deformLayer = PsdEvaluator.FindLayerById(allDeformData, csbDriver2.TargetRendererPath, csbDriver2.TargetLayerId);
					if (deformLayer != null)
					{
						num++;
						bool flag = PsdEvaluator.IsTimelineOwning(deformLayer, frameCount, isAvailable);
						string text = L.CategoryLabel(RendererCategory.ClothesCategoryKey(csbDriver2.ClothingKind));
						string text2 = this.CsbTargetMeshLabel(csbDriver2.TargetRendererPath);
						string text3 = (string.IsNullOrEmpty(text2) ? deformLayer.Name : (text2 + ":" + deformLayer.Name));
						string text4 = string.Format(L.CsbRowHeaderFmt, text, text3);
						string text5 = string.Format(L.CsbRowDetailFmt, new object[]
						{
							ShapeEditorWindow.FmtWeight(csbDriver2.StateWeights, 0),
							ShapeEditorWindow.FmtWeight(csbDriver2.StateWeights, 1),
							ShapeEditorWindow.FmtWeight(csbDriver2.StateWeights, 2),
							ShapeEditorWindow.FmtWeight(csbDriver2.StateWeights, 3),
							csbDriver2.UnequippedWeight.ToString("0.##", CultureInfo.InvariantCulture)
						});
						if (csbDriver2.CoordinateScope >= 0)
						{
							text5 += string.Format(L.CsbScopeRowFmt, csbDriver2.CoordinateScope);
						}
						bool flag2;
						if (this.DrawDriverRow(i, text4, text5, flag, L.CsbTimelineOverriding, L.CsbDeleteDriver, csbDriver2.Enabled, out flag2))
						{
							csbDriver = csbDriver2;
						}
						csbDriver2.Enabled = flag2;
					}
				}
			}
			if (num == 0)
			{
				GUILayout.Label(L.CsbNoLayerHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndScrollView();
			if (csbDriver != null)
			{
				owner.RemoveCsbDriver(csbDriver);
				this._csbNotice = null;
			}
			UI.EndPanel();
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x00021F6C File Offset: 0x0002016C
		private void DrawCsbAddForm(ShapeEditorController owner)
		{
			UI.BeginPanel(L.CsbAddDriver);
			Renderer renderer = this.Renderers[this.PrimaryRendererIndex];
			string text = ((this.RendererPaths != null && this.PrimaryRendererIndex < this.RendererPaths.Count) ? this.RendererPaths[this.PrimaryRendererIndex] : null);
			DeformData deformData = owner.GetDeformData(renderer);
			if (deformData == null || deformData.Layers.Count == 0)
			{
				GUILayout.Label(L.CsbNoLayerHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
				UI.EndPanel();
				return;
			}
			this.EnsureCsbFormText();
			GUILayout.Label(L.CsbTargetLayerLabel, Array.Empty<GUILayoutOption>());
			if (this._csbSelectedLayerIndex >= deformData.Layers.Count)
			{
				this._csbSelectedLayerIndex = -1;
			}
			for (int i = 0; i < deformData.Layers.Count; i++)
			{
				bool flag = this._csbSelectedLayerIndex == i;
				if (GUILayout.Toggle(flag, deformData.Layers[i].Name, "Button", Array.Empty<GUILayoutOption>()) && !flag)
				{
					this._csbSelectedLayerIndex = i;
				}
			}
			GUILayout.Space(2f);
			GUILayout.Label(L.CsbClothingKindLabel, Array.Empty<GUILayoutOption>());
			int num = -1;
			string[] array = new string[ShapeEditorWindow._csbClothingKinds.Length];
			for (int j = 0; j < ShapeEditorWindow._csbClothingKinds.Length; j++)
			{
				array[j] = L.CategoryLabel(RendererCategory.ClothesCategoryKey(ShapeEditorWindow._csbClothingKinds[j]));
				if (ShapeEditorWindow._csbClothingKinds[j] == this._csbSelectedKind)
				{
					num = j;
				}
			}
			int num2 = GUILayout.SelectionGrid(num, array, 3, Array.Empty<GUILayoutOption>());
			if (num2 != num && num2 >= 0 && num2 < ShapeEditorWindow._csbClothingKinds.Length)
			{
				this._csbSelectedKind = ShapeEditorWindow._csbClothingKinds[num2];
			}
			GUILayout.Space(2f);
			this._csbScopeThisCoordOnly = UI.Checkbox(this._csbScopeThisCoordOnly, L.CsbScopeToggle, null);
			GUILayout.Label(L.CsbScopeHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(2f);
			GUIStyle label = GUI.skin.label;
			float num3 = 0f;
			for (int k = 0; k < 4; k++)
			{
				ShapeEditorWindow._scratchContent.text = ShapeEditorWindow.StateLabel(k);
				float x = label.CalcSize(ShapeEditorWindow._scratchContent).x;
				if (x > num3)
				{
					num3 = x;
				}
			}
			ShapeEditorWindow._scratchContent.text = L.CsbUnequippedLabel;
			float x2 = label.CalcSize(ShapeEditorWindow._scratchContent).x;
			if (x2 > num3)
			{
				num3 = x2;
			}
			num3 += 2f;
			for (int l = 0; l < 4; l++)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Label(ShapeEditorWindow.StateLabel(l), new GUILayoutOption[] { GUILayout.Width(num3) });
				this._csbStateWeightText[l] = GUILayout.TextField(this._csbStateWeightText[l] ?? "0", Array.Empty<GUILayoutOption>());
				GUILayout.EndHorizontal();
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(L.CsbUnequippedLabel, new GUILayoutOption[] { GUILayout.Width(num3) });
			this._csbUnequippedText = GUILayout.TextField(this._csbUnequippedText ?? "0", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.Space(2f);
			bool flag2 = this._csbSelectedKind >= 0 && this._csbSelectedLayerIndex >= 0 && this._csbSelectedLayerIndex < deformData.Layers.Count;
			bool enabled = GUI.enabled;
			GUI.enabled = enabled && flag2;
			if (GUILayout.Button(L.CsbCaptureFromState, Array.Empty<GUILayoutOption>()))
			{
				this.CaptureCsbFromState(owner, deformData);
			}
			GUI.enabled = enabled;
			GUILayout.Label(L.CsbCaptureHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(2f);
			if (GUILayout.Button(L.CsbAddDriver, Array.Empty<GUILayoutOption>()))
			{
				this.TryAddCsbDriver(owner, text, deformData);
			}
			if (!string.IsNullOrEmpty(this._csbNotice))
			{
				GUILayout.Label(this._csbNotice, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			}
			UI.EndPanel();
		}

		// Token: 0x060003AA RID: 938 RVA: 0x0002234C File Offset: 0x0002054C
		private void TryAddCsbDriver(ShapeEditorController owner, string primaryPath, DeformData primaryData)
		{
			this._csbNotice = null;
			if (this._csbSelectedLayerIndex < 0 || this._csbSelectedLayerIndex >= primaryData.Layers.Count)
			{
				this._csbNotice = L.CsbNoneSelected;
				return;
			}
			if (this._csbSelectedKind < 0)
			{
				this._csbNotice = L.CsbNoneSelected;
				return;
			}
			CsbDriver csbDriver = new CsbDriver
			{
				ClothingKind = this._csbSelectedKind,
				TargetRendererPath = (primaryPath ?? ""),
				TargetLayerId = primaryData.Layers[this._csbSelectedLayerIndex].Id,
				Enabled = true,
				CoordinateScope = this.ResolveCsbAddScope(owner)
			};
			float[] array = new float[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = ShapeEditorWindow.ParseWeight(this._csbStateWeightText[i], (i == 0) ? 1f : 0f);
			}
			csbDriver.StateWeights = array;
			csbDriver.UnequippedWeight = ShapeEditorWindow.ParseWeight(this._csbUnequippedText, 0f);
			csbDriver.NormalizeStateWeights();
			if (!owner.TryAddCsbDriver(csbDriver))
			{
				this._csbNotice = L.CsbDuplicateNotice;
			}
		}

		// Token: 0x060003AB RID: 939 RVA: 0x00022458 File Offset: 0x00020658
		private int ResolveCsbAddScope(ShapeEditorController owner)
		{
			if (!this._csbScopeThisCoordOnly)
			{
				return -1;
			}
			if (owner == null || owner.ChaControl == null || owner.ChaControl.fileStatus == null)
			{
				return -1;
			}
			return -1; 
		}

		// Token: 0x060003AC RID: 940 RVA: 0x000224A8 File Offset: 0x000206A8
		private void CaptureCsbFromState(ShapeEditorController owner, DeformData primaryData)
		{
			this._csbNotice = null;
			if (owner == null)
			{
				return;
			}
			if (this._csbSelectedKind < 0 || this._csbSelectedLayerIndex < 0 || this._csbSelectedLayerIndex >= primaryData.Layers.Count)
			{
				this._csbNotice = L.CsbNoneSelected;
				return;
			}
			ChaControl chaControl = owner.ChaControl;
			if (chaControl == null)
			{
				return;
			}
			string text = CsbDriver.SanitizeWeight(primaryData.Layers[this._csbSelectedLayerIndex].Weight).ToString("0.###", CultureInfo.InvariantCulture);
			ChaFileStatus fileStatus = chaControl.fileStatus;
			byte[] array = ((fileStatus != null) ? fileStatus.clothesState : null);
			int csbSelectedKind = this._csbSelectedKind;
			if (!CsbEvaluator.IsClothingEquipped(array, chaControl, csbSelectedKind))
			{
				this._csbUnequippedText = text;
				return;
			}
			int num = (int)array[csbSelectedKind];
			if (num >= 0 && num < 4)
			{
				this._csbStateWeightText[num] = text;
				return;
			}
			this._csbUnequippedText = text;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x0002258C File Offset: 0x0002078C
		private void EnsureCsbFormText()
		{
			if (this._csbStateWeightText != null && this._csbStateWeightText.Length == 4)
			{
				return;
			}
			this._csbStateWeightText = new string[4];
			for (int i = 0; i < 4; i++)
			{
				this._csbStateWeightText[i] = ((i == 0) ? "1" : "0");
			}
		}

		// Token: 0x060003AE RID: 942 RVA: 0x000225DC File Offset: 0x000207DC
		private string CsbTargetMeshLabel(string path)
		{
			string text = path ?? "";
			string text2 = null;
			string text3 = null;
			if (this.Renderers != null && this.RendererPaths != null)
			{
				int num = 0;
				while (num < this.RendererPaths.Count && num < this.Renderers.Count)
				{
					if (!((this.RendererPaths[num] ?? "") != text))
					{
						if (this.Renderers[num] != null)
						{
							text2 = this.Renderers[num].name;
						}
						text3 = ((this.RendererCategories != null && num < this.RendererCategories.Count) ? this.RendererCategories[num] : null);
						break;
					}
					num++;
				}
			}
			if (text2 == null)
			{
				text2 = ShapeEditorWindow.ComputePresetShortName(path);
			}
			string text4 = L.CategoryLabel(text3);
			if (!string.IsNullOrEmpty(text4))
			{
				return "[" + text4 + "] " + text2;
			}
			return text2;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x000226D8 File Offset: 0x000208D8
		private static string StateLabel(int state)
		{
			if (L.CsbStateLabels != null && state >= 0 && state < L.CsbStateLabels.Length)
			{
				return L.CsbStateLabels[state] + ":";
			}
			return "State " + state.ToString(CultureInfo.InvariantCulture) + ":";
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00022727 File Offset: 0x00020927
		private static string FmtWeight(float[] weights, int index)
		{
			if (weights == null || index < 0 || index >= weights.Length)
			{
				return "-";
			}
			return weights[index].ToString("0.##", CultureInfo.InvariantCulture);
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x00022754 File Offset: 0x00020954
		private static float ParseWeight(string text, float fallback)
		{
			float num;
			if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
			{
				return CsbDriver.SanitizeWeight(num);
			}
			return fallback;
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x00022780 File Offset: 0x00020980
		private ShapeEditorController ResolveDriverTabOwner()
		{
			if (this.Renderers == null || this.Renderers.Count == 0)
			{
				return null;
			}
			int primaryRendererIndex = this.PrimaryRendererIndex;
			if (primaryRendererIndex < 0 || primaryRendererIndex >= this.Renderers.Count)
			{
				return null;
			}
			Renderer renderer = this.Renderers[primaryRendererIndex];
			if (renderer == null)
			{
				return null;
			}
			return ControllerResolver.Resolve(renderer).CharacterController;
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x000227E4 File Offset: 0x000209E4
		private bool DrawDriverRow(int index, string header, string detail, bool overriding, string overrideLabel, string deleteLabel, bool enabled, out bool newEnabled)
		{
			newEnabled = enabled;
			bool flag = false;
			UI.BeginRowBg(index);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			Rect rect = GUILayoutUtility.GetRect(18f, 22f, ShapeEditorWindow._driverRowBoxCellOpt);
			Rect rect2;
			rect2 = new Rect(rect.x + 2f, rect.y + (rect.height - 14f) * 0.5f, 14f, 14f);
			Event current = Event.current;
			if (GUI.enabled && current.type == EventType.MouseDown && current.button == 0 && rect2.Contains(current.mousePosition))
			{
				newEnabled = !enabled;
				current.Use();
			}
			if (current.type == EventType.Repaint)
			{
				UI.DrawToggleBox(rect2, newEnabled);
			}
			GUILayout.Space(10f);
			if (ShapeEditorWindow._driverRowLabelStyle == null)
			{
				ShapeEditorWindow._driverRowLabelStyle = new GUIStyle(Theme.RowLabelStyle)
				{
					margin = new RectOffset(0, 0, 0, 0),
					wordWrap = false
				};
			}
			Color color = GUI.color;
			if (overriding)
			{
				GUI.color = ShapeEditorWindow._timelineOverrideTint;
			}
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUILayout.Label(header, ShapeEditorWindow._driverRowLabelStyle, ShapeEditorWindow._driverRowH20);
			GUILayout.Label(detail, ShapeEditorWindow._driverRowLabelStyle, overriding ? ShapeEditorWindow._driverRowH20 : ShapeEditorWindow._driverRowH22);
			if (overriding)
			{
				GUILayout.Label("[" + overrideLabel + "]", ShapeEditorWindow._driverRowLabelStyle, ShapeEditorWindow._driverRowH24);
			}
			GUILayout.EndVertical();
			GUI.color = color;
			GUILayout.FlexibleSpace();
			if (ShapeEditorWindow._driverDeleteBtnStyle == null)
			{
				ShapeEditorWindow._driverDeleteBtnStyle = new GUIStyle(GUI.skin.button)
				{
					alignment = (TextAnchor)4,
					padding = new RectOffset(0, 0, 0, 0)
				};
			}
			Rect rect3 = GUILayoutUtility.GetRect(22f, 22f, ShapeEditorWindow._driverRowXCellOpt);
			Color backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = ShapeEditorWindow._driverDeleteBtnBg;
			if (GUI.Button(rect3, deleteLabel, ShapeEditorWindow._driverDeleteBtnStyle))
			{
				flag = true;
			}
			GUI.backgroundColor = backgroundColor;
			GUILayout.EndHorizontal();
			UI.EndRowBg();
			return flag;
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x000229CC File Offset: 0x00020BCC
		private static void RebuildFilteredIndices(List<string> names, string filter, List<int> outIndices)
		{
			outIndices.Clear();
			if (names == null)
			{
				return;
			}
			for (int i = 0; i < names.Count; i++)
			{
				if (filter.Length <= 0 || names[i].IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					outIndices.Add(i);
				}
			}
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00022A18 File Offset: 0x00020C18
		private void DrawNbbTab()
		{
			this.DrawRendererSelection();
			GUILayout.Space(2f);
			ShapeEditorController shapeEditorController = this.ResolveDriverTabOwner();
			if (shapeEditorController == null)
			{
				UI.BeginPanel(L.NbbSectionTitle);
				GUILayout.Label(L.NbbCharacterOnlyHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
				UI.EndPanel();
				return;
			}
			this.DrawNbbDriverList(shapeEditorController);
			GUILayout.Space(2f);
			this.DrawNbbAddForm(shapeEditorController);
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x00022A84 File Offset: 0x00020C84
		private void DrawNbbDriverList(ShapeEditorController owner)
		{
			UI.BeginPanel(L.NbbSectionTitle);
			List<NbbDriver> nbbDrivers = owner.NbbDrivers;
			Dictionary<string, DeformData> allDeformData = owner.GetAllDeformData();
			bool isAvailable = TimelineCompat.IsAvailable;
			int frameCount = Time.frameCount;
			this._nbbDriverScroll = GUILayout.BeginScrollView(this._nbbDriverScroll, new GUILayoutOption[] { GUILayout.Height(180f) });
			NbbDriver nbbDriver = null;
			int num = 0;
			for (int i = 0; i < nbbDrivers.Count; i++)
			{
				NbbDriver nbbDriver2 = nbbDrivers[i];
				if (nbbDriver2 != null)
				{
					DeformLayer deformLayer = PsdEvaluator.FindLayerById(allDeformData, nbbDriver2.TargetRendererPath, nbbDriver2.TargetLayerId);
					if (deformLayer != null)
					{
						num++;
						bool flag = PsdEvaluator.IsTimelineOwning(deformLayer, frameCount, isAvailable);
						string text = this.CsbTargetMeshLabel(nbbDriver2.TargetRendererPath);
						string text2 = string.Format(L.NbbRowHeaderFmt, string.IsNullOrEmpty(text) ? ShapeEditorWindow.ComputePresetShortName(nbbDriver2.TargetRendererPath) : text, deformLayer.Name);
						string text3 = string.Format(L.NbbRowDetailFmt, new object[]
						{
							ShapeEditorWindow.ComputePresetShortName(nbbDriver2.SourceRendererPath),
							nbbDriver2.SourceShapeName ?? "",
							ShapeEditorWindow.FmtNbbEndpoint(nbbDriver2.InputMin),
							ShapeEditorWindow.FmtNbbEndpoint(nbbDriver2.InputMax)
						});
						bool flag2;
						if (this.DrawDriverRow(i, text2, text3, flag, L.NbbTimelineOverriding, L.NbbDeleteDriver, nbbDriver2.Enabled, out flag2))
						{
							nbbDriver = nbbDriver2;
						}
						nbbDriver2.Enabled = flag2;
					}
				}
			}
			if (num == 0)
			{
				GUILayout.Label(L.NbbNoLayerHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndScrollView();
			if (nbbDriver != null)
			{
				owner.RemoveNbbDriver(nbbDriver);
				this._nbbNotice = null;
			}
			UI.EndPanel();
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x00022C2C File Offset: 0x00020E2C
		private void DrawNbbAddForm(ShapeEditorController owner)
		{
			UI.BeginPanel(L.NbbAddDriver);
			string text = ((this.RendererPaths != null && this.PrimaryRendererIndex < this.RendererPaths.Count) ? this.RendererPaths[this.PrimaryRendererIndex] : null);
			DeformData deformData;
			if (text != null)
			{
				owner.GetAllDeformData().TryGetValue(text, out deformData);
			}
			else
			{
				deformData = owner.GetDeformData(this.Renderers[this.PrimaryRendererIndex]);
			}
			if (deformData == null || deformData.Layers.Count == 0)
			{
				GUILayout.Label(L.NbbNoLayerHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
				UI.EndPanel();
				return;
			}
			GUILayout.Label(L.NbbTargetLayerLabel, Array.Empty<GUILayoutOption>());
			if (this._nbbSelectedLayerIndex >= deformData.Layers.Count)
			{
				this._nbbSelectedLayerIndex = -1;
			}
			for (int i = 0; i < deformData.Layers.Count; i++)
			{
				bool flag = this._nbbSelectedLayerIndex == i;
				if (GUILayout.Toggle(flag, deformData.Layers[i].Name, "Button", Array.Empty<GUILayoutOption>()) && !flag)
				{
					this._nbbSelectedLayerIndex = i;
				}
			}
			GUILayout.Space(2f);
			this.DrawNbbSourcePicker(owner);
			GUILayout.Space(2f);
			GUILayout.Label(L.NbbInputRangeLabel, Array.Empty<GUILayoutOption>());
			Rect[] array = UI.SplitRow(2, 22f, 2f);
			this._nbbInputMinText = GUI.TextField(array[0], this._nbbInputMinText ?? "0");
			this._nbbInputMaxText = GUI.TextField(array[1], this._nbbInputMaxText ?? "100");
			float num;
			bool flag2 = this.TryReadNbbSourceWeight(owner, out num);
			bool enabled = GUI.enabled;
			GUI.enabled = enabled && flag2;
			Rect[] array2 = UI.SplitRow(2, 22f, 2f);
			if (GUI.Button(array2[0], L.NbbSetMinFromCurrent))
			{
				this.CaptureNbbRangeEndpoint(owner, true);
			}
			if (GUI.Button(array2[1], L.NbbSetMaxFromCurrent))
			{
				this.CaptureNbbRangeEndpoint(owner, false);
			}
			GUI.enabled = enabled;
			GUILayout.Label(string.Format(L.NbbCurrentValueFmt, flag2 ? ShapeEditorWindow.FmtNbbEndpoint(num) : L.NbbNoneSelected), Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Label(L.NbbCaptureHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(2f);
			if (GUILayout.Button(L.NbbAddDriver, Array.Empty<GUILayoutOption>()))
			{
				this.TryAddNbbDriver(owner, text, deformData);
			}
			if (!string.IsNullOrEmpty(this._nbbNotice))
			{
				GUILayout.Label(this._nbbNotice, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			}
			UI.EndPanel();
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x00022EB8 File Offset: 0x000210B8
		private void TryAddNbbDriver(ShapeEditorController owner, string primaryPath, DeformData primaryData)
		{
			this._nbbNotice = null;
			if (this._nbbSelectedLayerIndex < 0 || this._nbbSelectedLayerIndex >= primaryData.Layers.Count)
			{
				this._nbbNotice = L.NbbNoneSelected;
				return;
			}
			if (string.IsNullOrEmpty(this._nbbSelectedShapeName) || this._nbbSelectedSourcePath == null)
			{
				this._nbbNotice = L.NbbNoneSelected;
				return;
			}
			float num;
			float num2;
			if (!float.TryParse(this._nbbInputMinText, NumberStyles.Float, CultureInfo.InvariantCulture, out num) || !float.TryParse(this._nbbInputMaxText, NumberStyles.Float, CultureInfo.InvariantCulture, out num2) || Mathf.Approximately(num, num2))
			{
				this._nbbNotice = L.NbbInvalidRangeNotice;
				return;
			}
			NbbDriver nbbDriver = new NbbDriver
			{
				SourceRendererPath = this._nbbSelectedSourcePath,
				SourceShapeName = this._nbbSelectedShapeName,
				InputMin = num,
				InputMax = num2,
				TargetRendererPath = (primaryPath ?? ""),
				TargetLayerId = primaryData.Layers[this._nbbSelectedLayerIndex].Id,
				Enabled = true
			};
			if (!owner.TryAddNbbDriver(nbbDriver))
			{
				this._nbbNotice = L.NbbDuplicateNotice;
			}
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00022FD0 File Offset: 0x000211D0
		private void CaptureNbbRangeEndpoint(ShapeEditorController owner, bool isMin)
		{
			float num;
			if (!this.TryReadNbbSourceWeight(owner, out num))
			{
				this._nbbNotice = L.NbbNoneSelected;
				return;
			}
			string text = num.ToString("0.#####", CultureInfo.InvariantCulture);
			if (isMin)
			{
				this._nbbInputMinText = text;
			}
			else
			{
				this._nbbInputMaxText = text;
			}
			this._nbbNotice = null;
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00023020 File Offset: 0x00021220
		private bool TryReadNbbSourceWeight(ShapeEditorController owner, out float value)
		{
			value = 0f;
			if (owner == null || string.IsNullOrEmpty(this._nbbSelectedShapeName) || this._nbbSelectedSourcePath == null)
			{
				return false;
			}
			Transform rootTransform = owner.RootTransform;
			if (rootTransform == null)
			{
				return false;
			}
			if (this._nbbProbe == null)
			{
				this._nbbProbe = new NbbDriver();
			}
			this._nbbProbe.SourceRendererPath = this._nbbSelectedSourcePath;
			this._nbbProbe.SourceShapeName = this._nbbSelectedShapeName;
			return this._nbbProbe.TryReadSourceWeight(rootTransform, out value);
		}

		// Token: 0x060003BB RID: 955 RVA: 0x000230A8 File Offset: 0x000212A8
		private void DrawNbbSourcePicker(ShapeEditorController owner)
		{
			this.EnsureNbbSourceCache(owner);
			if (this._nbbSourcePaths.Count == 0)
			{
				GUILayout.Label(L.NbbNoSourceHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
				return;
			}
			GUILayout.Label(L.NbbSourceRendererLabel, Array.Empty<GUILayoutOption>());
			for (int i = 0; i < this._nbbSourcePaths.Count; i++)
			{
				bool flag = this._nbbSelectedSourcePath == this._nbbSourcePaths[i];
				if (GUILayout.Toggle(flag, this._nbbSourceNames[i], "Button", Array.Empty<GUILayoutOption>()) && !flag)
				{
					this._nbbSelectedSourcePath = this._nbbSourcePaths[i];
					this._nbbSelectedShapeName = null;
				}
			}
			GUILayout.Space(2f);
			this.EnsureNbbShapeCache(owner);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(L.NbbSourceShapeLabel, new GUILayoutOption[] { GUILayout.Width(120f) });
			GUILayout.Label(string.IsNullOrEmpty(this._nbbSelectedShapeName) ? L.NbbNoneSelected : this._nbbSelectedShapeName, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(L.NbbFilterLabel, new GUILayoutOption[] { GUILayout.Width(120f) });
			this._nbbShapeFilter = GUILayout.TextField(this._nbbShapeFilter ?? "", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			this.EnsureNbbFilteredShapeIndices();
			if (ShapeEditorWindow._nbbShapeRowStyle == null)
			{
				ShapeEditorWindow._nbbShapeRowStyle = new GUIStyle(Theme.FlatButtonStyle)
				{
					margin = new RectOffset(0, 0, 0, 0)
				};
			}
			int count = this._nbbFilteredShapeIndices.Count;
			UI.VirtualScrollRange virtualScrollRange = UI.BeginVirtualScroll(ref this._nbbShapeScroll, 140f, count, 22f);
			for (int j = virtualScrollRange.First; j < virtualScrollRange.Last; j++)
			{
				int num = this._nbbFilteredShapeIndices[j];
				string text = this._nbbShapeNames[num];
				bool flag2 = this._nbbSelectedShapeName == text;
				if (GUILayout.Toggle(flag2, text, ShapeEditorWindow._nbbShapeRowStyle, ShapeEditorWindow._nbbShapeRowHeightOpt) && !flag2)
				{
					this._nbbSelectedShapeName = text;
				}
			}
			UI.EndVirtualScroll(virtualScrollRange, count, 22f);
		}

		// Token: 0x060003BC RID: 956 RVA: 0x000232C8 File Offset: 0x000214C8
		private void EnsureNbbSourceCache(ShapeEditorController owner)
		{
			if (this._nbbSourceCacheOwner == owner && this._nbbSourceCacheList == this.Renderers)
			{
				return;
			}
			this._nbbSourceCacheOwner = owner;
			this._nbbSourceCacheList = this.Renderers;
			this._nbbSourcePaths.Clear();
			this._nbbSourceNames.Clear();
			int num = ((this.Renderers != null) ? this.Renderers.Count : 0);
			Renderer renderer = ((this.PrimaryRendererIndex >= 0 && this.PrimaryRendererIndex < num) ? this.Renderers[this.PrimaryRendererIndex] : null);
			string text = null;
			for (int i = 0; i < num; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = this.Renderers[i] as SkinnedMeshRenderer;
				if (!(skinnedMeshRenderer == null))
				{
					Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
					if (!(sharedMesh == null) && ShapeEditorWindow.HasNativeBlendShape(sharedMesh))
					{
						string text2 = ((this.RendererPaths != null && i < this.RendererPaths.Count) ? this.RendererPaths[i] : "");
						this._nbbSourcePaths.Add(text2);
						this._nbbSourceNames.Add(skinnedMeshRenderer.name);
						if (this.Renderers[i] == renderer)
						{
							text = text2;
						}
					}
				}
			}
			if (this._nbbSelectedSourcePath == null || !this._nbbSourcePaths.Contains(this._nbbSelectedSourcePath))
			{
				this._nbbSelectedSourcePath = text;
				this._nbbSelectedShapeName = null;
			}
		}

		// Token: 0x060003BD RID: 957 RVA: 0x00023430 File Offset: 0x00021630
		private static bool HasNativeBlendShape(Mesh mesh)
		{
			int blendShapeCount = mesh.blendShapeCount;
			for (int i = 0; i < blendShapeCount; i++)
			{
				string blendShapeName = mesh.GetBlendShapeName(i);
				if (blendShapeName != null && !blendShapeName.StartsWith("kkse_"))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0002346C File Offset: 0x0002166C
		private void EnsureNbbShapeCache(ShapeEditorController owner)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = this.ResolveNbbSourceRenderer(owner);
			Mesh mesh = ((skinnedMeshRenderer != null) ? skinnedMeshRenderer.sharedMesh : null);
			int num = ((mesh != null) ? mesh.GetInstanceID() : 0);
			if (this._nbbShapeCacheMeshId == num)
			{
				return;
			}
			this._nbbShapeCacheMeshId = num;
			this._nbbFilteredShapeCache = null;
			this._nbbShapeNames.Clear();
			if (mesh == null)
			{
				return;
			}
			int blendShapeCount = mesh.blendShapeCount;
			for (int i = 0; i < blendShapeCount; i++)
			{
				string blendShapeName = mesh.GetBlendShapeName(i);
				if (blendShapeName != null && !blendShapeName.StartsWith("kkse_"))
				{
					this._nbbShapeNames.Add(blendShapeName);
				}
			}
			if (this._nbbSelectedShapeName != null && !this._nbbShapeNames.Contains(this._nbbSelectedShapeName))
			{
				this._nbbSelectedShapeName = null;
			}
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00023538 File Offset: 0x00021738
		private void EnsureNbbFilteredShapeIndices()
		{
			string text = this._nbbShapeFilter ?? "";
			if (this._nbbFilteredShapeCache == text)
			{
				return;
			}
			this._nbbFilteredShapeCache = text;
			ShapeEditorWindow.RebuildFilteredIndices(this._nbbShapeNames, text, this._nbbFilteredShapeIndices);
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00023580 File Offset: 0x00021780
		private SkinnedMeshRenderer ResolveNbbSourceRenderer(ShapeEditorController owner)
		{
			if (owner == null || this._nbbSelectedSourcePath == null)
			{
				return null;
			}
			Transform rootTransform = owner.RootTransform;
			if (rootTransform == null)
			{
				return null;
			}
			return ShapeEditorController.FindRendererByPath(rootTransform, this._nbbSelectedSourcePath) as SkinnedMeshRenderer;
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x000235C3 File Offset: 0x000217C3
		private static string FmtNbbEndpoint(float value)
		{
			return value.ToString("0.##", CultureInfo.InvariantCulture);
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x000235D8 File Offset: 0x000217D8
		private void DrawPsdTab()
		{
			this.DrawRendererSelection();
			GUILayout.Space(2f);
			ShapeEditorController shapeEditorController = this.ResolveDriverTabOwner();
			if (shapeEditorController == null)
			{
				UI.BeginPanel(L.PsdSectionTitle);
				GUILayout.Label(L.PsdCharacterOnlyHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
				UI.EndPanel();
				return;
			}
			this.DrawPsdDriverList(shapeEditorController);
			GUILayout.Space(2f);
			this.DrawPsdAddForm(shapeEditorController);
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00023644 File Offset: 0x00021844
		private void DrawPsdDriverList(ShapeEditorController owner)
		{
			UI.BeginPanel(L.PsdSectionTitle);
			List<PsdDriver> drivers = owner.Drivers;
			Dictionary<string, DeformData> allDeformData = owner.GetAllDeformData();
			bool isAvailable = TimelineCompat.IsAvailable;
			int frameCount = Time.frameCount;
			this._psdDriverScroll = GUILayout.BeginScrollView(this._psdDriverScroll, new GUILayoutOption[] { GUILayout.Height(180f) });
			PsdDriver psdDriver = null;
			int num = 0;
			for (int i = 0; i < drivers.Count; i++)
			{
				PsdDriver psdDriver2 = drivers[i];
				if (psdDriver2 != null)
				{
					DeformLayer deformLayer = PsdEvaluator.FindLayerById(allDeformData, psdDriver2.TargetRendererPath, psdDriver2.TargetLayerId);
					if (deformLayer != null)
					{
						num++;
						bool flag = PsdEvaluator.IsTimelineOwning(deformLayer, frameCount, isAvailable);
						string text = this.ResolvePsdPartPrefix(psdDriver2.TargetRendererPath) + string.Format(L.PsdRowHeaderFmt, ShapeEditorWindow.ComputePresetShortName(psdDriver2.TargetRendererPath), deformLayer.Name);
						string text2 = string.Format(L.PsdRowDetailFmt, new object[]
						{
							ShapeEditorWindow.ComputePresetShortName(psdDriver2.SourceBonePath),
							ShapeEditorWindow.ChannelName(psdDriver2.Channel),
							psdDriver2.InputMin.ToString("0.##", CultureInfo.InvariantCulture),
							psdDriver2.InputMax.ToString("0.##", CultureInfo.InvariantCulture)
						});
						bool flag2;
						if (this.DrawDriverRow(i, text, text2, flag, L.PsdTimelineOverriding, L.PsdDeleteDriver, psdDriver2.Enabled, out flag2))
						{
							psdDriver = psdDriver2;
						}
						psdDriver2.Enabled = flag2;
					}
				}
			}
			if (num == 0)
			{
				GUILayout.Label(L.PsdNoLayerHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndScrollView();
			if (psdDriver != null)
			{
				owner.RemoveDriver(psdDriver);
				this._psdNotice = null;
			}
			UI.EndPanel();
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x000237FC File Offset: 0x000219FC
		private string ResolvePsdPartPrefix(string rendererPath)
		{
			if (this.RendererPaths == null || this.RendererCategories == null)
			{
				return "";
			}
			int num = 0;
			while (num < this.RendererPaths.Count && num < this.RendererCategories.Count)
			{
				if (!(this.RendererPaths[num] != rendererPath))
				{
					string text = L.CategoryLabel(this.RendererCategories[num]);
					if (!string.IsNullOrEmpty(text))
					{
						return "[" + text + "] ";
					}
					return "";
				}
				else
				{
					num++;
				}
			}
			return "";
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00023890 File Offset: 0x00021A90
		private void DrawPsdAddForm(ShapeEditorController owner)
		{
			UI.BeginPanel(L.PsdAddDriver);
			Renderer renderer = this.Renderers[this.PrimaryRendererIndex];
			string text = ((this.RendererPaths != null && this.PrimaryRendererIndex < this.RendererPaths.Count) ? this.RendererPaths[this.PrimaryRendererIndex] : null);
			DeformData deformData = owner.GetDeformData(renderer);
			if (deformData == null || deformData.Layers.Count == 0)
			{
				GUILayout.Label(L.PsdNoLayerHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
				UI.EndPanel();
				return;
			}
			GUILayout.Label(L.PsdTargetLayerLabel, Array.Empty<GUILayoutOption>());
			if (this._psdSelectedLayerIndex >= deformData.Layers.Count)
			{
				this._psdSelectedLayerIndex = -1;
			}
			for (int i = 0; i < deformData.Layers.Count; i++)
			{
				bool flag = this._psdSelectedLayerIndex == i;
				if (GUILayout.Toggle(flag, deformData.Layers[i].Name, "Button", Array.Empty<GUILayoutOption>()) && !flag)
				{
					this._psdSelectedLayerIndex = i;
				}
			}
			GUILayout.Space(2f);
			GUILayout.Label(L.PsdChannelLabel, Array.Empty<GUILayoutOption>());
			this._psdChannelIndex = GUILayout.Toolbar(this._psdChannelIndex, L.PsdChannelNames, Array.Empty<GUILayoutOption>());
			GUILayout.Space(2f);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(L.PsdSourceBoneLabel, new GUILayoutOption[] { GUILayout.Width(80f) });
			GUILayout.Label(string.IsNullOrEmpty(this._psdSelectedBonePath) ? L.PsdNoneSelected : ShapeEditorWindow.ComputePresetShortName(this._psdSelectedBonePath), Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			this.EnsureBoneCache(owner);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(L.PsdFilterLabel, new GUILayoutOption[] { GUILayout.Width(80f) });
			this._psdBoneFilter = GUILayout.TextField(this._psdBoneFilter ?? "", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			this.EnsureFilteredBoneIndices();
			if (ShapeEditorWindow._psdBoneRowStyle == null)
			{
				ShapeEditorWindow._psdBoneRowStyle = new GUIStyle(Theme.FlatButtonStyle)
				{
					margin = new RectOffset(0, 0, 0, 0)
				};
			}
			int count = this._psdFilteredBoneIndices.Count;
			UI.VirtualScrollRange virtualScrollRange = UI.BeginVirtualScroll(ref this._psdBoneScroll, 140f, count, 22f);
			for (int j = virtualScrollRange.First; j < virtualScrollRange.Last; j++)
			{
				int num = this._psdFilteredBoneIndices[j];
				bool flag2 = this._psdSelectedBonePath == this._psdBonePaths[num];
				if (GUILayout.Toggle(flag2, this._psdBoneShortNames[num], ShapeEditorWindow._psdBoneRowStyle, ShapeEditorWindow._psdBoneRowHeightOpt) && !flag2)
				{
					this._psdSelectedBonePath = this._psdBonePaths[num];
				}
			}
			UI.EndVirtualScroll(virtualScrollRange, count, 22f);
			GUILayout.Space(2f);
			GUILayout.Label(L.PsdInputRangeLabel, Array.Empty<GUILayoutOption>());
			Rect[] array = UI.SplitRow(2, 22f, 2f);
			Rect rect = array[0];
			Rect rect2 = array[1];
			this._psdInputMinText = GUI.TextField(rect, this._psdInputMinText ?? "0");
			this._psdInputMaxText = GUI.TextField(rect2, this._psdInputMaxText ?? "0");
			bool flag3 = !string.IsNullOrEmpty(this._psdSelectedBonePath);
			bool enabled = GUI.enabled;
			GUI.enabled = enabled && flag3;
			Rect[] array2 = UI.SplitRow(2, 22f, 2f);
			if (GUI.Button(array2[0], L.PsdSetMinFromPose))
			{
				this.CapturePsdRangeEndpoint(owner, true);
			}
			if (GUI.Button(array2[1], L.PsdSetMaxFromPose))
			{
				this.CapturePsdRangeEndpoint(owner, false);
			}
			GUI.enabled = enabled;
			GUILayout.Label(L.PsdCaptureHint, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			GUILayout.Space(2f);
			if (GUILayout.Button(L.PsdAddDriver, Array.Empty<GUILayoutOption>()))
			{
				this.TryAddPsdDriver(owner, text, deformData);
			}
			if (!string.IsNullOrEmpty(this._psdNotice))
			{
				GUILayout.Label(this._psdNotice, Theme.DimHintStyle, Array.Empty<GUILayoutOption>());
			}
			UI.EndPanel();
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x00023C98 File Offset: 0x00021E98
		private void TryAddPsdDriver(ShapeEditorController owner, string primaryPath, DeformData primaryData)
		{
			this._psdNotice = null;
			if (this._psdSelectedLayerIndex < 0 || this._psdSelectedLayerIndex >= primaryData.Layers.Count)
			{
				this._psdNotice = L.PsdNoneSelected;
				return;
			}
			if (string.IsNullOrEmpty(this._psdSelectedBonePath))
			{
				this._psdNotice = L.PsdNoneSelected;
				return;
			}
			float num;
			float num2;
			if (!float.TryParse(this._psdInputMinText, NumberStyles.Float, CultureInfo.InvariantCulture, out num) || !float.TryParse(this._psdInputMaxText, NumberStyles.Float, CultureInfo.InvariantCulture, out num2) || Mathf.Approximately(num, num2))
			{
				this._psdNotice = L.PsdInvalidRangeNotice;
				return;
			}
			PsdDriver psdDriver = new PsdDriver
			{
				SourceBonePath = this._psdSelectedBonePath,
				Channel = (PsdChannel)this._psdChannelIndex,
				InputMin = num,
				InputMax = num2,
				TargetRendererPath = (primaryPath ?? ""),
				TargetLayerId = primaryData.Layers[this._psdSelectedLayerIndex].Id,
				Enabled = true
			};
			if (!owner.TryAddDriver(psdDriver))
			{
				this._psdNotice = L.PsdDuplicateNotice;
			}
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x00023DA8 File Offset: 0x00021FA8
		private void CapturePsdRangeEndpoint(ShapeEditorController owner, bool isMin)
		{
			if (owner == null || string.IsNullOrEmpty(this._psdSelectedBonePath))
			{
				return;
			}
			Transform rootTransform = owner.RootTransform;
			if (rootTransform == null)
			{
				return;
			}
			Transform transform = PsdEvaluator.ResolveBone(rootTransform, this._psdSelectedBonePath);
			if (transform == null)
			{
				this._psdNotice = L.PsdNoneSelected;
				return;
			}
			string text = PsdEvaluator.ReadChannel(transform, (PsdChannel)this._psdChannelIndex).ToString("0.#####", CultureInfo.InvariantCulture);
			if (isMin)
			{
				this._psdInputMinText = text;
			}
			else
			{
				this._psdInputMaxText = text;
			}
			this._psdNotice = null;
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x00023E38 File Offset: 0x00022038
		private void EnsureBoneCache(ShapeEditorController owner)
		{
			if (this._psdBoneCacheOwner == owner && this._psdBonePaths != null)
			{
				return;
			}
			this._psdBoneCacheOwner = owner;
			this._psdBonePaths = new List<string>();
			this._psdBoneShortNames = new List<string>();
			this._psdFilteredBoneFilterCache = null;
			Transform rootTransform = owner.RootTransform;
			if (rootTransform == null)
			{
				return;
			}
			foreach (Transform transform in rootTransform.GetComponentsInChildren<Transform>(true))
			{
				if (!(transform == null) && !(transform == rootTransform))
				{
					this._psdBonePaths.Add(ShapeEditorController.GetRelativePath(rootTransform, transform));
					this._psdBoneShortNames.Add(transform.name);
				}
			}
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00023EE0 File Offset: 0x000220E0
		private void EnsureFilteredBoneIndices()
		{
			string text = this._psdBoneFilter ?? "";
			if (this._psdFilteredBoneFilterCache == text)
			{
				return;
			}
			this._psdFilteredBoneFilterCache = text;
			ShapeEditorWindow.RebuildFilteredIndices(this._psdBoneShortNames, text, this._psdFilteredBoneIndices);
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00023F28 File Offset: 0x00022128
		private static string ChannelName(PsdChannel channel)
		{
			int num = (int)channel;
			if (L.PsdChannelNames != null && num >= 0 && num < L.PsdChannelNames.Length)
			{
				return L.PsdChannelNames[num];
			}
			return channel.ToString();
		}

		// Token: 0x04000300 RID: 768
		private readonly int _windowId;

		// Token: 0x04000301 RID: 769
		private Rect _windowRect;

		// Token: 0x04000302 RID: 770
		private float _settledWindowHeight;

		// Token: 0x04000303 RID: 771
		private bool _visible;

		// Token: 0x04000304 RID: 772
		private bool _showHelp;

		// Token: 0x04000305 RID: 773
		private Rect _helpWindowRect;

		// Token: 0x04000306 RID: 774
		private Vector2 _helpScroll;

		// Token: 0x04000307 RID: 775
		private bool _showResetConfirm;

		// Token: 0x04000308 RID: 776
		private string _pendingResetConfirmPath;

		// Token: 0x04000309 RID: 777
		private Rect _resetConfirmRect = new Rect(0f, 0f, 360f, 240f);

		// Token: 0x0400030A RID: 778
		private const float MinWidth = 350f;

		// Token: 0x0400030B RID: 779
		private const float MinCap = 200f;

		// Token: 0x0400030C RID: 780
		private const float GripSize = 16f;

		// Token: 0x0400030D RID: 781
		private const float GripStripHeight = 14f;

		// Token: 0x0400030E RID: 782
		private const float DoubleClickSeconds = 0.3f;

		// Token: 0x0400030F RID: 783
		private const float DefaultChromeHeight = 70f;

		// Token: 0x04000310 RID: 784
		private float _userWidth;

		// Token: 0x04000311 RID: 785
		private float _userHeightCap;

		// Token: 0x04000312 RID: 786
		private Vector2 _windowScroll;

		// Token: 0x04000313 RID: 787
		private float _measuredContentHeight;

		// Token: 0x04000314 RID: 788
		private float _lastViewportHeight;

		// Token: 0x04000315 RID: 789
		private float _chromeHeight = 70f;

		// Token: 0x04000316 RID: 790
		private int _lastTabIndexForScroll = -1;

		// Token: 0x04000317 RID: 791
		private bool _resizing;

		// Token: 0x04000318 RID: 792
		private Vector2 _resizeStartMouseScreen;

		// Token: 0x04000319 RID: 793
		private float _resizeStartWidth;

		// Token: 0x0400031A RID: 794
		private float _resizeStartCap;

		private int _resizeControlId;

		// Token: 0x0400031B RID: 795
		private float _lastGripClickTime;

		// Token: 0x0400031C RID: 796
		private static readonly Color _gripColor = new Color(0.7f, 0.7f, 0.7f, 0.9f);

		// Token: 0x0400031D RID: 797
		private static readonly GUIContent _scratchContent = new GUIContent();

		// Token: 0x0400031E RID: 798
		private static readonly Color _primaryTint = new Color(0.4f, 0.8f, 1f);

		// Token: 0x0400031F RID: 799
		private static readonly Color _secondaryTint = new Color(0.55f, 0.85f, 0.7f);

		// Token: 0x04000320 RID: 800
		private static readonly Color _corruptedText = new Color(1f, 0.4f, 0.4f);

		// Token: 0x04000321 RID: 801
		private static readonly Color _resetButtonBg = new Color(1f, 0.5f, 0.5f);

		// Token: 0x04000322 RID: 802
		private static readonly Color _dimText = new Color(0.6f, 0.6f, 0.6f, 1f);

		// Token: 0x04000323 RID: 803
		private static readonly GUILayoutOption[] _visibilityWidthOpt = new GUILayoutOption[] { GUILayout.Width(46f) };

		// Token: 0x04000324 RID: 804
		private static GUIStyle _visibilityLabelStyle;

		// Token: 0x04000326 RID: 806
		private static ShapeEditorWindow _activeInstance;

		// Token: 0x04000327 RID: 807
		private int _tabIndex;

		// Token: 0x04000328 RID: 808
		private Vector2 _layerScroll;

		// Token: 0x04000329 RID: 809
		private Vector2 _rendererScroll;

		// Token: 0x0400032A RID: 810
		private string _rendererFilter = "";

		// Token: 0x0400032B RID: 811
		private bool _showEditedOnly;

		// Token: 0x04000344 RID: 836
		private int _renamingLayerIndex = -1;

		// Token: 0x04000345 RID: 837
		private string _renamingText = "";

		// Token: 0x04000346 RID: 838
		public bool DeferEnterEditMode;

		// Token: 0x04000347 RID: 839
		public bool DeferExitEditMode;

		// Token: 0x04000348 RID: 840
		public bool DeferSubdivide;

		public List<SeamlessCharacterOption> SeamlessCharacters =
			new List<SeamlessCharacterOption>();

		public int SeamlessCharacterIndex = -1;

		public bool SeamlessBodyEnabled { get; set; } = true;

		public bool SeamlessHeadEnabled { get; set; }

		public AIChara.ChaControl SeamlessTargetCharacter
		{
			get
			{
				if (this.SeamlessCharacterIndex < 0 ||
					this.SeamlessCharacterIndex >= this.SeamlessCharacters.Count)
				{
					return null;
				}

				SeamlessCharacterOption option =
					this.SeamlessCharacters[this.SeamlessCharacterIndex];

				return option != null ? option.Character : null;
			}
		}

		// Token: 0x04000349 RID: 841
		public bool DeferRestore;

		// Token: 0x0400034A RID: 842
		public bool DeferRebakeBodySubdivision;

		// Token: 0x0400034B RID: 843
		public bool DeferFaceSelectAll;

		// Token: 0x0400034C RID: 844
		public bool DeferFaceSelectNone;

		// Token: 0x0400034D RID: 845
		public bool DeferFaceSelectInvert;

		// Token: 0x0400034E RID: 846
		public bool DeferFaceDelete;

		// Token: 0x0400034F RID: 847
		public bool DeferFaceRestore;

		// Token: 0x04000350 RID: 848
		public bool DeferFaceRestoreAll;

		// Token: 0x04000351 RID: 849
		public bool DeferLayerAdd;

		// Token: 0x04000352 RID: 850
		public int DeferLayerRemove = -1;

		// Token: 0x04000353 RID: 851
		public bool DeferSubdivideLayerWarningConfirm;

		// Token: 0x04000354 RID: 852
		public int DeferLayerMoveUp = -1;

		// Token: 0x04000355 RID: 853
		public int DeferLayerMoveDown = -1;

		// Token: 0x04000356 RID: 854
		public int DeferLayerRename = -1;

		// Token: 0x04000357 RID: 855
		public string DeferLayerRenameNewName;

		// Token: 0x04000358 RID: 856
		public int DeferLayerMirror = -1;

		// Token: 0x04000359 RID: 857
		public bool DeferRemapWeights;

		// Token: 0x0400035A RID: 858
		public bool DeferRestoreWeights;

		// Token: 0x0400035B RID: 859
		public bool DeferExport;

		// Token: 0x0400035C RID: 860
		public bool DeferImport;

		// Token: 0x0400035D RID: 861
		public bool DeferFocusRenderer;

		// Token: 0x0400035E RID: 862
		public bool DeferClearGizmoSelection;

		// Token: 0x0400035F RID: 863
		public bool DeferPresetExport;

		// Token: 0x04000360 RID: 864
		public bool DeferPresetImport;

		// Token: 0x04000361 RID: 865
		public bool DeferPresetApplyImport;

		// Token: 0x04000362 RID: 866
		public bool DeferPresetCancelImport;

		// Token: 0x04000363 RID: 867
		public bool DeferResetCorruptedLayers;

		// Token: 0x04000364 RID: 868
		public string DeferResetPath;

		// Token: 0x04000365 RID: 869
		public PresetTabState PresetState = new PresetTabState();

		// Token: 0x04000366 RID: 870
		private int _weightSliderLayer = -1;

		// Token: 0x04000367 RID: 871
		private float _weightSliderBefore;

		// Token: 0x04000368 RID: 872
		public int WeightUndoLayer = -1;

		// Token: 0x04000369 RID: 873
		public float WeightUndoBefore;

		// Token: 0x0400036A RID: 874
		public float WeightUndoAfter;

		// Token: 0x0400036B RID: 875
		public List<Renderer> Renderers = new List<Renderer>();

		// Token: 0x0400036C RID: 876
		public List<string> RendererPaths = new List<string>();

		// Token: 0x0400036D RID: 877
		public List<string> RendererCategories = new List<string>();

		// Token: 0x0400036E RID: 878
		public Dictionary<string, CorruptionReason> CorruptionState;

		// Token: 0x0400036F RID: 879
		public int PrimaryRendererIndex;

		// Token: 0x04000370 RID: 880
		public HashSet<int> SelectedRendererIndices = new HashSet<int>();

		// Token: 0x04000371 RID: 881
		public DeformData ActiveDeformData;

		// Token: 0x04000372 RID: 882
		public FaceSelectOverlay FaceSelect;

		// Token: 0x04000373 RID: 883
		public int VertexCount;

		// Token: 0x04000374 RID: 884
		public bool ShowSubdivideLayerWarning;

		// Token: 0x04000375 RID: 885
		public bool BodyMeshReadable;

		// Token: 0x04000376 RID: 886
		public bool IsOnCharacter;

		// Token: 0x04000377 RID: 887
		private string[] _tabNamesMakerCache;

		// Token: 0x04000378 RID: 888
		private string[] _tabNamesMakerBaseRef;

		// Token: 0x04000379 RID: 889
		private string[] _tabNamesStudioCache;

		// Token: 0x0400037A RID: 890
		private string[] _tabNamesStudioBaseRef;

		// Token: 0x0400037B RID: 891
		private static readonly int[] _csbClothingKinds = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

		// Token: 0x0400037C RID: 892
		private Vector2 _csbDriverScroll;

		// Token: 0x0400037D RID: 893
		private int _csbSelectedLayerIndex = -1;

		// Token: 0x0400037E RID: 894
		private int _csbSelectedKind = -1;

		// Token: 0x0400037F RID: 895
		private string[] _csbStateWeightText;

		// Token: 0x04000380 RID: 896
		private string _csbUnequippedText = "0";

		// Token: 0x04000381 RID: 897
		private bool _csbScopeThisCoordOnly;

		// Token: 0x04000382 RID: 898
		private string _csbNotice;

		// Token: 0x04000383 RID: 899
		private static readonly Color _timelineOverrideTint = new Color(0.7f, 0.7f, 0.7f, 1f);

		// Token: 0x04000384 RID: 900
		private static readonly Color _driverDeleteBtnBg = new Color(1f, 0.28f, 0.28f, 1f);

		// Token: 0x04000385 RID: 901
		private static GUIStyle _driverDeleteBtnStyle;

		// Token: 0x04000386 RID: 902
		private static GUIStyle _driverRowLabelStyle;

		// Token: 0x04000387 RID: 903
		private static readonly GUILayoutOption[] _driverRowBoxCellOpt = new GUILayoutOption[] { GUILayout.Width(18f) };

		// Token: 0x04000388 RID: 904
		private static readonly GUILayoutOption[] _driverRowXCellOpt = new GUILayoutOption[]
		{
			GUILayout.Width(22f),
			GUILayout.ExpandHeight(false)
		};

		// Token: 0x04000389 RID: 905
		private static readonly GUILayoutOption[] _driverRowH20 = new GUILayoutOption[] { GUILayout.Height(20f) };

		// Token: 0x0400038A RID: 906
		private static readonly GUILayoutOption[] _driverRowH22 = new GUILayoutOption[] { GUILayout.Height(22f) };

		// Token: 0x0400038B RID: 907
		private static readonly GUILayoutOption[] _driverRowH24 = new GUILayoutOption[] { GUILayout.Height(24f) };

		// Token: 0x0400038C RID: 908
		private Vector2 _nbbDriverScroll;

		// Token: 0x0400038D RID: 909
		private string _nbbNotice;

		// Token: 0x0400038E RID: 910
		private int _nbbSelectedLayerIndex = -1;

		// Token: 0x0400038F RID: 911
		private string _nbbSelectedSourcePath;

		// Token: 0x04000390 RID: 912
		private string _nbbSelectedShapeName;

		// Token: 0x04000391 RID: 913
		private string _nbbInputMinText = "0";

		// Token: 0x04000392 RID: 914
		private string _nbbInputMaxText = "100";

		// Token: 0x04000393 RID: 915
		private NbbDriver _nbbProbe;

		// Token: 0x04000394 RID: 916
		private readonly List<string> _nbbSourcePaths = new List<string>();

		// Token: 0x04000395 RID: 917
		private readonly List<string> _nbbSourceNames = new List<string>();

		// Token: 0x04000396 RID: 918
		private ShapeEditorController _nbbSourceCacheOwner;

		// Token: 0x04000397 RID: 919
		private List<Renderer> _nbbSourceCacheList;

		// Token: 0x04000398 RID: 920
		private readonly List<string> _nbbShapeNames = new List<string>();

		// Token: 0x04000399 RID: 921
		private int _nbbShapeCacheMeshId;

		// Token: 0x0400039A RID: 922
		private readonly List<int> _nbbFilteredShapeIndices = new List<int>();

		// Token: 0x0400039B RID: 923
		private string _nbbFilteredShapeCache;

		// Token: 0x0400039C RID: 924
		private Vector2 _nbbShapeScroll;

		// Token: 0x0400039D RID: 925
		private string _nbbShapeFilter = "";

		// Token: 0x0400039E RID: 926
		private static GUIStyle _nbbShapeRowStyle;

		// Token: 0x0400039F RID: 927
		private const float NbbShapeRowHeight = 22f;

		// Token: 0x040003A0 RID: 928
		private static readonly GUILayoutOption[] _nbbShapeRowHeightOpt = new GUILayoutOption[] { GUILayout.Height(22f) };

		// Token: 0x040003A1 RID: 929
		private Vector2 _psdDriverScroll;

		// Token: 0x040003A2 RID: 930
		private Vector2 _psdBoneScroll;

		// Token: 0x040003A3 RID: 931
		private string _psdBoneFilter = "";

		// Token: 0x040003A4 RID: 932
		private string _psdSelectedBonePath;

		// Token: 0x040003A5 RID: 933
		private int _psdSelectedLayerIndex = -1;

		// Token: 0x040003A6 RID: 934
		private int _psdChannelIndex;

		// Token: 0x040003A7 RID: 935
		private string _psdInputMinText = "0";

		// Token: 0x040003A8 RID: 936
		private string _psdInputMaxText = "90";

		// Token: 0x040003A9 RID: 937
		private string _psdNotice;

		// Token: 0x040003AA RID: 938
		private ShapeEditorController _psdBoneCacheOwner;

		// Token: 0x040003AB RID: 939
		private List<string> _psdBonePaths;

		// Token: 0x040003AC RID: 940
		private List<string> _psdBoneShortNames;

		// Token: 0x040003AD RID: 941
		private readonly List<int> _psdFilteredBoneIndices = new List<int>();

		// Token: 0x040003AE RID: 942
		private string _psdFilteredBoneFilterCache;

		// Token: 0x040003AF RID: 943
		private static GUIStyle _psdBoneRowStyle;

		// Token: 0x040003B0 RID: 944
		private const float PsdBoneRowHeight = 22f;

		// Token: 0x040003B1 RID: 945
		private static readonly GUILayoutOption[] _psdBoneRowHeightOpt = new GUILayoutOption[] { GUILayout.Height(22f) };

		// Token: 0x02000075 RID: 117
		public enum OpMode
		{
			// Token: 0x040004F8 RID: 1272
			Brush,
			// Token: 0x040004F9 RID: 1273
			Gizmo
		}

		// Token: 0x02000076 RID: 118
		public enum BrushToolType
		{
			// Token: 0x040004FB RID: 1275
			Move,
			// Token: 0x040004FC RID: 1276
			Smooth,
			// Token: 0x040004FD RID: 1277
			Relax,
			// Token: 0x040004FE RID: 1278
			Inflate,
			// Token: 0x040004FF RID: 1279
			Pinch,
			// Token: 0x04000500 RID: 1280
			Crease,
			Seamless
		}

		// Token: 0x02000077 RID: 119
		public enum GizmoSelectMethod
		{
			// Token: 0x04000502 RID: 1282
			Box,
			// Token: 0x04000503 RID: 1283
			Brush
		}

		// Token: 0x02000078 RID: 120
		public enum SelectMode
		{
			// Token: 0x04000505 RID: 1285
			Vertex,
			// Token: 0x04000506 RID: 1286
			Face
		}
	}
}
