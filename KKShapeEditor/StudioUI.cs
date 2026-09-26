using System;
using System.Collections.Generic;
using System.Linq;
using KKAPI.Studio;
using Studio;
using UnityEngine;
using AIChara;

namespace KKShapeEditor
{
	public static class StudioUI
	{
		public static void Init()
		{
			if (!StudioAPI.InsideStudio)
			{
				return;
			}
			StudioUI._window = new ShapeEditorWindow(1263730688, new Rect(20f, 20f, 380f, 600f));
			StudioUI._overlayGo = new GameObject("KKShapeEditor_StudioOverlay");
			UnityEngine.Object.DontDestroyOnLoad(StudioUI._overlayGo);
			StudioUI._overlay = StudioUI._overlayGo.AddComponent<ShapePaintOverlay>();
			StudioUI._overlay.Window = StudioUI._window;
			StudioUI._overlay.SelectionTool = new SelectionTool();
			StudioUI._overlay.Input = new InputHelper();
			StudioUI._overlay.Input.Init();
			StudioUI._overlay.OnRefreshRenderers = new Action(StudioUI.RefreshRenderers);
			StudioUI._overlay.GetCurrentSelection = new Func<object>(StudioUI.GetSelectedNode);
		}

		private static object GetSelectedNode()
		{
			global::Studio.Studio instance = Singleton<global::Studio.Studio>.Instance;
			if (instance == null || instance.treeNodeCtrl == null)
			{
				return null;
			}
			return instance.treeNodeCtrl.selectNode;
		}

		public static void RefreshRenderers()
		{
			if (StudioUI._window == null)
			{
				return;
			}
			IEnumerable<OCIChar> selectedCharacters = StudioAPI.GetSelectedCharacters();
			OCIChar[] array = ((selectedCharacters != null) ? selectedCharacters.ToArray<OCIChar>() : null);
			if (array != null && array.Length != 0)
			{
				ChaControl charInfo = array[0].charInfo;
				ShapeEditorController shapeEditorController = ((charInfo != null) ? charInfo.gameObject.GetComponent<ShapeEditorController>() : null);
				if (shapeEditorController != null)
				{
					shapeEditorController.ChaControl = charInfo;
					shapeEditorController.BindWindow(
						StudioUI._window,
						new Action(StudioUI.RefreshRenderers),
						ref StudioUI._subscribedCtrl);

					RefreshAllSeamlessCharacters();
					return;
				}
			}
			ShapeEditorController.UnbindCorruptionSubscription(new Action(StudioUI.RefreshRenderers), ref StudioUI._subscribedCtrl);
			StudioUI._window.Renderers.Clear();
			StudioUI._window.RendererPaths = new List<string>();
			StudioUI._window.RendererCategories = new List<string>();
			StudioUI._window.CorruptionState = null;
			IEnumerable<ObjectCtrlInfo> selectedObjects = StudioAPI.GetSelectedObjects();
			ObjectCtrlInfo[] array2 = ((selectedObjects != null) ? selectedObjects.ToArray<ObjectCtrlInfo>() : null);
			if (array2 != null && array2.Length != 0)
			{
				ObjectCtrlInfo[] array3 = array2;
				for (int i = 0; i < array3.Length; i++)
				{
					OCIItem ociitem = array3[i] as OCIItem;
					if (ociitem != null && ociitem.objectItem != null)
					{
						ItemShapeController itemShapeController = ociitem.objectItem.GetComponent<ItemShapeController>();
						if (itemShapeController == null)
						{
							itemShapeController = ociitem.objectItem.AddComponent<ItemShapeController>();
						}
						StudioUI._window.Renderers = itemShapeController.GetAllRenderers();
						Transform rootTransform = itemShapeController.RootTransform;
						List<string> list = new List<string>(StudioUI._window.Renderers.Count);
						List<string> list2 = new List<string>(StudioUI._window.Renderers.Count);
						for (int j = 0; j < StudioUI._window.Renderers.Count; j++)
						{
							Renderer renderer = StudioUI._window.Renderers[j];
							list.Add((renderer != null) ? ShapeEditorController.GetRelativePath(rootTransform, renderer.transform) : null);
							list2.Add("Item");
						}
						StudioUI._window.RendererPaths = list;
						StudioUI._window.RendererCategories = list2;
						break;
					}
				}
			}
			RefreshAllSeamlessCharacters();

			StudioUI._window.SanitizeSelectionAgainstRenderers();
			PresetTabState presetState = StudioUI._window.PresetState;
			if (presetState == null)
			{
				return;
			}
			presetState.ResetForOwnerSwitch();
		}

		public static ShapeEditorWindow Window
		{
			get
			{
				return StudioUI._window;
			}
		}

		public static ShapePaintOverlay Overlay
		{
			get
			{
				return StudioUI._overlay;
			}
		}

		public static void Toggle()
		{
			ShapeEditorWindow window = StudioUI._window;
			if (window == null)
			{
				return;
			}
			window.Toggle();
		}

		/// <summary>
		/// Находит основное тело персонажа
		/// </summary>
		public static Renderer FindCharacterBody(
			AIChara.ChaControl character)
		{
			if (character == null ||
				character.objBody == null)
			{
				return null;
			}

			return character.objBody
				.GetComponentInChildren<SkinnedMeshRenderer>(true);
		}

		/// <summary>
		/// Находит голову персонажа
		/// </summary>
		public static Renderer FindCharacterHead(
			AIChara.ChaControl character)
		{
			if (character == null ||
				character.objHead == null)
			{
				return null;
			}

			return character.objHead
				.GetComponentInChildren<SkinnedMeshRenderer>(true);
		}

		/// <summary>
		/// Обновляет список доступных персонажей для Seamless Blend
		/// </summary>
		private static void RefreshAllSeamlessCharacters()
		{
			if (StudioUI._window == null)
			{
				return;
			}

			StudioUI._window.SeamlessCharacters.Clear();

			global::Studio.Studio studio =
				Singleton<global::Studio.Studio>.Instance;

			if (studio == null ||
				studio.dicObjectCtrl == null)
			{
				return;
			}

			foreach (KeyValuePair<int, ObjectCtrlInfo> pair
				in studio.dicObjectCtrl)
			{
				OCIChar ociChar = pair.Value as OCIChar;

				if (ociChar == null ||
					ociChar.charInfo == null)
				{
					continue;
				}

				ChaControl chaControl = ociChar.charInfo;

				StudioUI._window.SeamlessCharacters.Add(
					new ShapeEditorWindow.SeamlessCharacterOption
					{
						Name = chaControl.name,
						Character = chaControl
					});
			}

			if (StudioUI._window.SeamlessCharacters.Count > 0)
			{
				if (StudioUI._window.SeamlessCharacterIndex < 0 ||
					StudioUI._window.SeamlessCharacterIndex >=
					StudioUI._window.SeamlessCharacters.Count)
				{
					StudioUI._window.SeamlessCharacterIndex = 0;
				}
			}
			else
			{
				StudioUI._window.SeamlessCharacterIndex = -1;
			}
		}

		/// <summary>
		/// Отрисовывает Seamless Blend UI панель
		/// Используется в ShapeEditorWindow для отрисовки управления Seamless функционалом
		/// </summary>
		public static void DrawSeamlessBlendPanel(ShapeEditorWindow window)
		{
			if (window == null)
			{
				return;
			}

			GUILayout.Label("Seamless Blend Settings", 
				new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold }, 
				Array.Empty<GUILayoutOption>());
			
			GUILayout.Space(8f);

			// ============ Выбор целевого персонажа ============
			DrawSeamlessCharacterSelector(window);

			GUILayout.Space(6f);

			// ============ Выбор целевых частей ============
			DrawSeamlessTargetParts(window);

			GUILayout.Space(6f);

			// ============ Режимы смешивания ============
			DrawSeamlessBlendModes(window);

			GUILayout.Space(6f);

			// ============ Параметры силы смешивания ============
			DrawSeamlessBlendParameters(window);

			GUILayout.Space(6f);

			// ============ Кнопка применения ============
			DrawSeamlessApplyButton(window);

			GUILayout.Space(4f);

			// ============ Информация ============
			DrawSeamlessInfo(window);
		}

		/// <summary>
		/// Отрисовывает селектор целевого персонажа
		/// </summary>
		private static void DrawSeamlessCharacterSelector(ShapeEditorWindow window)
		{
			GUILayout.Label("Target Character", Array.Empty<GUILayoutOption>());

			if (window.SeamlessCharacters == null || window.SeamlessCharacters.Count == 0)
			{
				GUILayout.Label("No other characters in scene", 
					new GUIStyle(GUI.skin.label) { normal = { textColor = Color.gray } },
					Array.Empty<GUILayoutOption>());
				return;
			}

			string[] characterNames = new string[window.SeamlessCharacters.Count];
			for (int i = 0; i < window.SeamlessCharacters.Count; i++)
			{
				ShapeEditorWindow.SeamlessCharacterOption option = window.SeamlessCharacters[i];
				characterNames[i] = (option != null && !string.IsNullOrEmpty(option.Name))
					? option.Name
					: "Character " + i.ToString();
			}

			int selected = Mathf.Clamp(window.SeamlessCharacterIndex, 0, characterNames.Length - 1);
			int newSelected = GUILayout.SelectionGrid(selected, characterNames, 1, "Button");

			if (newSelected != selected)
			{
				window.SeamlessCharacterIndex = newSelected;
			}
		}

		/// <summary>
		/// Отрисовывает выбор целевых частей тела (Body/Head)
		/// </summary>
		private static void DrawSeamlessTargetParts(ShapeEditorWindow window)
		{
			GUILayout.Label("Target Parts", Array.Empty<GUILayoutOption>());

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			
			window.SeamlessBodyEnabled = GUILayout.Toggle(
				window.SeamlessBodyEnabled,
				"Body",
				"Button",
				Array.Empty<GUILayoutOption>());

			window.SeamlessHeadEnabled = GUILayout.Toggle(
				window.SeamlessHeadEnabled,
				"Head",
				"Button",
				Array.Empty<GUILayoutOption>());

			GUILayout.EndHorizontal();

			if (!window.SeamlessBodyEnabled && !window.SeamlessHeadEnabled)
			{
				GUILayout.Label("At least one part must be selected",
					new GUIStyle(GUI.skin.label) { normal = { textColor = Color.yellow } },
					Array.Empty<GUILayoutOption>());
			}
		}

		/// <summary>
		/// Отрисовывает выбор режимов смешивания
		/// </summary>
		private static void DrawSeamlessBlendModes(ShapeEditorWindow window)
		{
			GUILayout.Label("Blend Mode", Array.Empty<GUILayoutOption>());

			string[] modes = new string[]
			{
				"Normal Blend",
				"Surface Snap",
				"Smooth Gradient",
				"Texture Aware",
				"Color Matching"
			};

			int modeIndex = (int)window.SeamlessBlendMode;
			int newModeIndex = GUILayout.SelectionGrid(
				modeIndex,
				modes,
				1,
				"Button",
				Array.Empty<GUILayoutOption>());

			if (newModeIndex != modeIndex)
			{
				window.SeamlessBlendMode = (SeamlessBlendMode)newModeIndex;
			}

			// Описание текущего режима
			string modeDescription = GetSeamlessModeDescription(window.SeamlessBlendMode);
			GUILayout.Label(modeDescription,
				new GUIStyle(GUI.skin.label) { wordWrap = true, fontSize = 10 },
				Array.Empty<GUILayoutOption>());
		}

		/// <summary>
		/// Отрисовывает параметры смешивания (слайдеры)
		/// </summary>
		private static void DrawSeamlessBlendParameters(ShapeEditorWindow window)
		{
			GUILayout.Label("Blend Parameters", Array.Empty<GUILayoutOption>());

			// Blend Distance
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Blend Distance", new GUILayoutOption[] { GUILayout.Width(120f) });
			window.SeamlessBlendDistance = GUILayout.HorizontalSlider(
				window.SeamlessBlendDistance,
				0.001f,
				0.1f,
				Array.Empty<GUILayoutOption>());
			GUILayout.Label(window.SeamlessBlendDistance.ToString("F4"), new GUILayoutOption[] { GUILayout.Width(60f) });
			GUILayout.EndHorizontal();

			// Border Strength
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Border Strength", new GUILayoutOption[] { GUILayout.Width(120f) });
			window.SeamlessBorderStrength = GUILayout.HorizontalSlider(
				window.SeamlessBorderStrength,
				0f,
				1f,
				Array.Empty<GUILayoutOption>());
			GUILayout.Label(window.SeamlessBorderStrength.ToString("F2"), new GUILayoutOption[] { GUILayout.Width(60f) });
			GUILayout.EndHorizontal();

			// Border Offset
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Border Offset", new GUILayoutOption[] { GUILayout.Width(120f) });
			window.SeamlessBorderOffset = GUILayout.HorizontalSlider(
				window.SeamlessBorderOffset,
				-0.05f,
				0.05f,
				Array.Empty<GUILayoutOption>());
			GUILayout.Label(window.SeamlessBorderOffset.ToString("F4"), new GUILayoutOption[] { GUILayout.Width(60f) });
			GUILayout.EndHorizontal();

			// Blend Strength
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Blend Strength", new GUILayoutOption[] { GUILayout.Width(120f) });
			window.SeamlessBlendStrength = GUILayout.HorizontalSlider(
				window.SeamlessBlendStrength,
				0f,
				1f,
				Array.Empty<GUILayoutOption>());
			GUILayout.Label(window.SeamlessBlendStrength.ToString("F2"), new GUILayoutOption[] { GUILayout.Width(60f) });
			GUILayout.EndHorizontal();
		}

		/// <summary>
		/// Отрисовывает кнопку применения Seamless Blend
		/// </summary>
		private static void DrawSeamlessApplyButton(ShapeEditorWindow window)
		{
			bool canApply = window.SeamlessCharacterIndex >= 0 &&
				window.SeamlessCharacterIndex < window.SeamlessCharacters.Count &&
				(window.SeamlessBodyEnabled || window.SeamlessHeadEnabled);

			bool previousEnabled = GUI.enabled;
			if (!canApply)
			{
				GUI.enabled = false;
			}

			if (GUILayout.Button("Apply Seamless Blend", new GUILayoutOption[] { GUILayout.Height(35f) }))
			{
				ApplySeamlessBlend(window);
			}

			GUI.enabled = previousEnabled;

			if (!canApply)
			{
				GUILayout.Label("Select a character and at least one part",
					new GUIStyle(GUI.skin.label) { normal = { textColor = Color.yellow }, wordWrap = true },
					Array.Empty<GUILayoutOption>());
			}
		}

		/// <summary>
		/// Отрисовывает информационную панель
		/// </summary>
		private static void DrawSeamlessInfo(ShapeEditorWindow window)
		{
			GUILayout.Label("Info", 
				new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold }, 
				Array.Empty<GUILayoutOption>());

			string infoText = "Seamless Blend: Smoothly blends seams between character meshes. " +
				"Select target character, body/head parts, and adjust blend parameters. " +
				"Click 'Apply Seamless Blend' to process.";

			GUILayout.Label(infoText,
				new GUIStyle(GUI.skin.label) { wordWrap = true, fontSize = 10 },
				Array.Empty<GUILayoutOption>());
		}

		/// <summary>
		/// Применяет Seamless Blend к текущему выделению
		/// </summary>
		private static void ApplySeamlessBlend(ShapeEditorWindow window)
		{
			if (window == null || window.SeamlessCharacterIndex < 0)
			{
				return;
			}

			if (window.SeamlessCharacterIndex >= window.SeamlessCharacters.Count)
			{
				ShapeEditorPlugin.Logger.LogError("Invalid seamless character index");
				return;
			}

			ShapeEditorWindow.SeamlessCharacterOption targetOption =
				window.SeamlessCharacters[window.SeamlessCharacterIndex];

			if (targetOption == null || targetOption.Character == null)
			{
				ShapeEditorPlugin.Logger.LogError("Target character is null");
				return;
			}

			// Получаем целевые рендереры
			List<Renderer> targetRenderers = new List<Renderer>();

			if (window.SeamlessBodyEnabled)
			{
				Renderer bodyRenderer = FindCharacterBody(targetOption.Character);
				if (bodyRenderer != null)
				{
					targetRenderers.Add(bodyRenderer);
				}
			}

			if (window.SeamlessHeadEnabled)
			{
				Renderer headRenderer = FindCharacterHead(targetOption.Character);
				if (headRenderer != null)
				{
					targetRenderers.Add(headRenderer);
				}
			}

			if (targetRenderers.Count == 0)
			{
				ShapeEditorPlugin.Logger.LogWarning("No valid target renderers found");
				return;
			}

			// Логируем применение
			ShapeEditorPlugin.Logger.LogInfo($"Applying Seamless Blend to {targetOption.Name}");
			ShapeEditorPlugin.Logger.LogInfo($"Mode: {window.SeamlessBlendMode}");
			ShapeEditorPlugin.Logger.LogInfo($"Body: {window.SeamlessBodyEnabled}, Head: {window.SeamlessHeadEnabled}");
			ShapeEditorPlugin.Logger.LogInfo($"Blend Distance: {window.SeamlessBlendDistance:F4}");
			ShapeEditorPlugin.Logger.LogInfo($"Border Strength: {window.SeamlessBorderStrength:F2}");
			ShapeEditorPlugin.Logger.LogInfo($"Blend Strength: {window.SeamlessBlendStrength:F2}");

			// TODO: Здесь будет вызов实際 применения seamless blend функции
			// ApplySeamlessBlendToRenderers(window.Renderers, targetRenderers, window);
		}

		/// <summary>
		/// Возвращает описание текущего режима смешивания
		/// </summary>
		private static string GetSeamlessModeDescription(SeamlessBlendMode mode)
		{
			switch (mode)
			{
				case SeamlessBlendMode.Normal:
					return "Basic blend: Smoothly interpolates vertices at seams.";
				case SeamlessBlendMode.SurfaceSnap:
					return "Surface Snap: Snaps vertices to target surface with offset correction.";
				case SeamlessBlendMode.SmoothGradient:
					return "Gradient: Creates smooth falloff gradient around seams.";
				case SeamlessBlendMode.TextureAware:
					return "Texture Aware: Considers texture boundaries for better blending.";
				case SeamlessBlendMode.ColorMatching:
					return "Color Matching: Blends vertex colors to match target mesh.";
				default:
					return "Unknown mode";
			}
		}

		private static ShapeEditorWindow _window;
		private static GameObject _overlayGo;
		private static ShapePaintOverlay _overlay;
		private static ShapeEditorController _subscribedCtrl;
	}

	/// <summary>
	/// Перечисление режимов смешивания для Seamless Blend
	/// </summary>
	public enum SeamlessBlendMode
	{
		Normal = 0,
		SurfaceSnap = 1,
		SmoothGradient = 2,
		TextureAware = 3,
		ColorMatching = 4
	}
}
