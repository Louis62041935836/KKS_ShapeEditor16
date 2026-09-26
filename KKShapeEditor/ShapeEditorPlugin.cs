using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using KKAPI.Chara;
using KKAPI.Studio;
using KKAPI.Studio.SaveLoad;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200001D RID: 29
	[BepInProcess("HoneySelect2")]
	[BepInProcess("StudioNEOV2")]
	[BepInDependency("marco.kkapi", "1.45.1")]
	[BepInDependency("com.bepis.bepinex.extendedsave")]
	[BepInDependency("com.joan6694.illusionplugins.timeline")]
	[BepInPlugin("com.ghostendsky.kkshapeeditor", "KKShapeEditor", "2.0.10")]
	[HelpURL("https://kkevo.booth.pm/")]
	public class ShapeEditorPlugin : BaseUnityPlugin
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600017F RID: 383 RVA: 0x0000DC4F File Offset: 0x0000BE4F
		// (set) Token: 0x06000180 RID: 384 RVA: 0x0000DC56 File Offset: 0x0000BE56
		internal static bool IsStudio { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000181 RID: 385 RVA: 0x0000DC5E File Offset: 0x0000BE5E
		// (set) Token: 0x06000182 RID: 386 RVA: 0x0000DC65 File Offset: 0x0000BE65
		public static ConfigEntry<KeyboardShortcut> StudioToggleKey { get; private set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000183 RID: 387 RVA: 0x0000DC6D File Offset: 0x0000BE6D
		// (set) Token: 0x06000184 RID: 388 RVA: 0x0000DC74 File Offset: 0x0000BE74
		public static ConfigEntry<float> DefaultBrushRadius { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000185 RID: 389 RVA: 0x0000DC7C File Offset: 0x0000BE7C
		// (set) Token: 0x06000186 RID: 390 RVA: 0x0000DC83 File Offset: 0x0000BE83
		public static ConfigEntry<float> DefaultBrushStrength { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000187 RID: 391 RVA: 0x0000DC8B File Offset: 0x0000BE8B
		// (set) Token: 0x06000188 RID: 392 RVA: 0x0000DC92 File Offset: 0x0000BE92
		public static ConfigEntry<float> MaxBrushRadius { get; private set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000189 RID: 393 RVA: 0x0000DC9A File Offset: 0x0000BE9A
		// (set) Token: 0x0600018A RID: 394 RVA: 0x0000DCA1 File Offset: 0x0000BEA1
		public static ConfigEntry<float> MaxSoftSelectionRadius { get; private set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600018B RID: 395 RVA: 0x0000DCA9 File Offset: 0x0000BEA9
		// (set) Token: 0x0600018C RID: 396 RVA: 0x0000DCB0 File Offset: 0x0000BEB0
		public static ConfigEntry<L.Language> UILanguage { get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600018D RID: 397 RVA: 0x0000DCB8 File Offset: 0x0000BEB8
		// (set) Token: 0x0600018E RID: 398 RVA: 0x0000DCBF File Offset: 0x0000BEBF
		public static ConfigEntry<int> UndoMaxSteps { get; private set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000DCC7 File Offset: 0x0000BEC7
		// (set) Token: 0x06000190 RID: 400 RVA: 0x0000DCCE File Offset: 0x0000BECE
		public static ConfigEntry<bool> UsePressure { get; private set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000191 RID: 401 RVA: 0x0000DCD6 File Offset: 0x0000BED6
		// (set) Token: 0x06000192 RID: 402 RVA: 0x0000DCDD File Offset: 0x0000BEDD
		public static ConfigEntry<bool> IncludeBackFaceVertices { get; private set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000193 RID: 403 RVA: 0x0000DCE5 File Offset: 0x0000BEE5
		// (set) Token: 0x06000194 RID: 404 RVA: 0x0000DCEC File Offset: 0x0000BEEC
		public static ConfigEntry<bool> EnableCharacterSwitchCarryover { get; private set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000195 RID: 405 RVA: 0x0000DCF4 File Offset: 0x0000BEF4
		// (set) Token: 0x06000196 RID: 406 RVA: 0x0000DCFB File Offset: 0x0000BEFB
		public static ConfigEntry<KeyboardShortcut> UndoKey { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000197 RID: 407 RVA: 0x0000DD03 File Offset: 0x0000BF03
		// (set) Token: 0x06000198 RID: 408 RVA: 0x0000DD0A File Offset: 0x0000BF0A
		public static ConfigEntry<KeyboardShortcut> RedoKey { get; private set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000199 RID: 409 RVA: 0x0000DD12 File Offset: 0x0000BF12
		// (set) Token: 0x0600019A RID: 410 RVA: 0x0000DD19 File Offset: 0x0000BF19
		public static ConfigEntry<float> BrushAdjustRepeatRate { get; private set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000DD21 File Offset: 0x0000BF21
		// (set) Token: 0x0600019C RID: 412 RVA: 0x0000DD28 File Offset: 0x0000BF28
		public static ConfigEntry<float> WindowWidth { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600019D RID: 413 RVA: 0x0000DD30 File Offset: 0x0000BF30
		// (set) Token: 0x0600019E RID: 414 RVA: 0x0000DD37 File Offset: 0x0000BF37
		public static ConfigEntry<float> WindowHeightCap { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600019F RID: 415 RVA: 0x0000DD3F File Offset: 0x0000BF3F
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x0000DD46 File Offset: 0x0000BF46
		public static ConfigEntry<float> SelectedVertexDotSize { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000DD4E File Offset: 0x0000BF4E
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x0000DD55 File Offset: 0x0000BF55
		public static ConfigEntry<float> FaceWireframeOpacity { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000DD5D File Offset: 0x0000BF5D
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x0000DD64 File Offset: 0x0000BF64
		public static ConfigEntry<bool> FaceSelectIncludeBackFace { get; private set; }

		// Token: 0x060001A5 RID: 421 RVA: 0x0000DD6C File Offset: 0x0000BF6C
		private void Awake()
		{
			ShapeEditorPlugin.Instance = this;
			ShapeEditorPlugin.Logger = base.Logger;
			ShapeEditorPlugin.IsStudio = StudioAPI.InsideStudio;
			ShapeEditorPlugin.StudioToggleKey = base.Config.Bind<KeyboardShortcut>("General", "Studio Panel Toggle", new KeyboardShortcut(KeyCode.S, new KeyCode[] { KeyCode.LeftShift }), "Toggle panel with hotkey (Shift+S)");
			ShapeEditorPlugin.DefaultBrushRadius = base.Config.Bind<float>("Shape Editor", "Default Brush Radius", 0.05f, new ConfigDescription("Default brush radius", new AcceptableValueRange<float>(0.001f, 1f), Array.Empty<object>()));
			ShapeEditorPlugin.DefaultBrushStrength = base.Config.Bind<float>("Shape Editor", "Default Brush Strength", 0.5f, new ConfigDescription("Default brush strength", new AcceptableValueRange<float>(0.01f, 1f), Array.Empty<object>()));
			ShapeEditorPlugin.MaxBrushRadius = base.Config.Bind<float>("Shape Editor", "Max Brush Radius", 0.5f, new ConfigDescription("Upper bound of the brush radius slider (and the Face Select brush radius slider), in world units. Raise this to edit large map objects or oversized accessories whose features exceed the default 0.5 reach.", new AcceptableValueRange<float>(0.1f, 5f), Array.Empty<object>()));
			ShapeEditorPlugin.MaxSoftSelectionRadius = base.Config.Bind<float>("Shape Editor", "Max Soft Selection Radius", 0.5f, new ConfigDescription("Upper bound of the Gizmo soft-selection influence radius slider, in world units. Raise this to spread soft-selection falloff over larger regions.", new AcceptableValueRange<float>(0.1f, 5f), Array.Empty<object>()));
			ShapeEditorPlugin.UILanguage = base.Config.Bind<L.Language>("General", "UI Language", L.Language.English, "UI display language");
			ShapeEditorPlugin.UndoMaxSteps = base.Config.Bind<int>("Shape Editor", "Undo Max Steps", 50, new ConfigDescription("Maximum number of undo steps", new AcceptableValueRange<int>(1, 200), Array.Empty<object>()));
			ShapeEditorPlugin.UsePressure = base.Config.Bind<bool>("Shape Editor", "Use Tablet Pressure", true, new ConfigDescription("Modulate brush strength by stylus pressure when a Wintab-compatible tablet is detected", null, Array.Empty<object>()));
			ShapeEditorPlugin.IncludeBackFaceVertices = base.Config.Bind<bool>("Shape Editor", "Include Back-Facing Vertices In Box Select", true, new ConfigDescription("When enabled (default), Gizmo box selection includes vertices on the back side of the mesh whose normals point away from the camera. When disabled, only front-facing vertices (per-vertex normal dot test) are selected. Brush selection is unaffected either way.", null, Array.Empty<object>()));
			ShapeEditorPlugin.EnableCharacterSwitchCarryover = base.Config.Bind<bool>("Studio", "Carry Over Body/Head Deformation On Character Switch", false, new ConfigDescription("When enabled in Studio, replacing a character preserves the previous character's body and head deform layers if topology (vertex count + subdivision level + triangle hash) matches the new character. Mismatched paths silently fall back to the new card's data.", null, Array.Empty<object>()));
			ShapeEditorPlugin.UndoKey = base.Config.Bind<KeyboardShortcut>("Shape Editor", "Undo Shortcut", new KeyboardShortcut(KeyCode.Z, Array.Empty<KeyCode>()), "Edit Mode undo shortcut. Default Z (no modifiers) avoids collision with Studio's Ctrl+Z. WARNING: Adding Ctrl modifier will trigger Studio's own Ctrl+Z / Ctrl+Y simultaneously alongside the plugin undo.");
			ShapeEditorPlugin.RedoKey = base.Config.Bind<KeyboardShortcut>("Shape Editor", "Redo Shortcut", new KeyboardShortcut(KeyCode.Y, Array.Empty<KeyCode>()), "Edit Mode redo shortcut. Default Y (no modifiers) avoids collision with Studio's Ctrl+Y. WARNING: Adding Ctrl modifier will trigger Studio's own Ctrl+Z / Ctrl+Y simultaneously alongside the plugin redo.");
			ShapeEditorPlugin.BrushAdjustRepeatRate = base.Config.Bind<float>("Shape Editor", "Brush Adjust Repeat Rate", 15f, new ConfigDescription("Auto-repeat speed (steps per second) when holding the bracket keys ([ / ]) to adjust brush radius or strength. Higher is faster. The initial press-and-hold delay before auto-repeat starts is fixed.", new AcceptableValueRange<float>(2f, 60f), Array.Empty<object>()));
			ShapeEditorPlugin.WindowWidth = base.Config.Bind<float>("Shape Editor", "Window Width", 0f, new ConfigDescription("Main tool panel width in pixels, set by dragging the bottom-right resize grip. 0 means automatic width (the built-in 350px minimum); values between 1 and 350 are treated as 350. Shared by Maker and Studio.", new AcceptableValueRange<float>(0f, 5000f), Array.Empty<object>()));
			ShapeEditorPlugin.WindowHeightCap = base.Config.Bind<float>("Shape Editor", "Window Height Cap", 0f, new ConfigDescription("Maximum main tool panel height in pixels, set by dragging the bottom-right resize grip. 0 means unlimited (the window tracks its content height as before); when content exceeds the cap the window stops growing and an outer scrollbar appears. Double-click the grip to reset to 0. Shared by Maker and Studio.", new AcceptableValueRange<float>(0f, 5000f), Array.Empty<object>()));
			ShapeEditorPlugin.SelectedVertexDotSize = base.Config.Bind<float>("Shape Editor", "Selected Vertex Dot Size", 0.003f, new ConfigDescription("Screen-space size factor of the selected-vertex disk markers shown in Gizmo edit mode. Lower this if the selected points look too large; raise it to make them more visible. Default 0.003 matches the previous hardcoded size.", new AcceptableValueRange<float>(0.0005f, 0.01f), Array.Empty<object>()));
			ShapeEditorPlugin.FaceWireframeOpacity = base.Config.Bind<float>("Shape Editor", "Face Select Wireframe Opacity", 1f, new ConfigDescription("Relative opacity of the Face Select mode wireframe. 1.0 keeps the original appearance; lower values fade the mesh wireframe so you can inspect the subdivided surface; 0 hides it entirely (face selection still works). Only affects the Face Select overlay, not the Edit Mode wireframe.", new AcceptableValueRange<float>(0f, 1f), Array.Empty<object>()));
			ShapeEditorPlugin.FaceSelectIncludeBackFace = base.Config.Bind<bool>("Shape Editor", "Include Back-Facing Faces In Face Select", true, new ConfigDescription("When enabled (default), Face Select brush and box selection can select faces on the back side of the mesh whose normals point away from the camera, matching the pre-change behavior. When disabled, only front-facing faces (per-face normal dot test) are selectable, which helps precisely select the front shell before deleting faces. Independent of the Gizmo box-select back-face option.", null, Array.Empty<object>()));
			L.SetLanguage(ShapeEditorPlugin.UILanguage.Value);
			ShapeEditorPlugin.UILanguage.SettingChanged += delegate(object s, EventArgs e)
			{
				L.SetLanguage(ShapeEditorPlugin.UILanguage.Value);
			};
			InputHelper.ApplyUndoRedoHotkeys(ShapeEditorPlugin.UndoKey.Value, ShapeEditorPlugin.RedoKey.Value);
			ShapeEditorPlugin.UndoKey.SettingChanged += delegate(object s, EventArgs e)
			{
				InputHelper.ApplyUndoRedoHotkeys(ShapeEditorPlugin.UndoKey.Value, ShapeEditorPlugin.RedoKey.Value);
			};
			ShapeEditorPlugin.RedoKey.SettingChanged += delegate(object s, EventArgs e)
			{
				InputHelper.ApplyUndoRedoHotkeys(ShapeEditorPlugin.UndoKey.Value, ShapeEditorPlugin.RedoKey.Value);
			};
			CharacterApi.RegisterExtraBehaviour<ShapeEditorController>("com.ghostendsky.kkshapeeditor");
			StudioSaveLoadApi.RegisterExtraBehaviour<ShapeEditorSceneController>("com.ghostendsky.kkshapeeditor");
			MakerUI.Init();
			StudioUI.Init();
			ShapeEditorPlugin.Logger.LogInfo("KKShapeEditor v2.0.10 loaded");
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000E218 File Offset: 0x0000C418
		private void Start()
		{
			TimelineCompat.Init();
			if (!TimelineCompat.IsAvailable)
			{
				return;
			}
			ItemShapeController[] array = UnityEngine.Object.FindObjectsOfType<ItemShapeController>();
			for (int i = 0; i < array.Length; i++)
			{
				TimelineRegistry.SyncController(array[i]);
			}
			ShapeEditorController[] array2 = UnityEngine.Object.FindObjectsOfType<ShapeEditorController>();
			for (int i = 0; i < array2.Length; i++)
			{
				TimelineRegistry.SyncController(array2[i]);
			}
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000E26C File Offset: 0x0000C46C
		internal static int KeyCodeToVk(KeyCode kc)
		{
			int num = (int)kc;
			if (num >= 97 && num <= 122)
			{
				return num - 32;
			}
			if (num >= 48 && num <= 57)
			{
				return num;
			}
			if (kc == KeyCode.None)
			{
				return 0;
			}
			switch (num)
			{
			case 282:
				return 112;
			case 283:
				return 113;
			case 284:
				return 114;
			case 285:
				return 115;
			case 286:
				return 116;
			case 287:
				return 117;
			case 288:
				return 118;
			case 289:
				return 119;
			case 290:
				return 120;
			case 291:
				return 121;
			case 292:
				return 122;
			case 293:
				return 123;
			default:
				ShapeEditorPlugin.Logger.LogWarning("Hotkey '" + kc.ToString() + "' is not supported; shortcut disabled. Use letters A-Z, digits 0-9, or F1-F12.");
				return 0;
			}
		}

		// Token: 0x040000B3 RID: 179
		public const string GUID = "com.ghostendsky.kkshapeeditor";

		// Token: 0x040000B4 RID: 180
		public const string PluginName = "KKShapeEditor";

		// Token: 0x040000B5 RID: 181
		public const string Version = "2.0.10";

		// Token: 0x040000B6 RID: 182
		public const string ExtendedDataId = "com.ghostendsky.kkshapeeditor";

		// Token: 0x040000B7 RID: 183
		internal const int StudioWindowId = 1263730688;

		// Token: 0x040000B8 RID: 184
		internal const int MakerWindowId = 1263734784;

		// Token: 0x040000B9 RID: 185
		internal static new ManualLogSource Logger;

		// Token: 0x040000BA RID: 186
		internal static ShapeEditorPlugin Instance;
	}
}
