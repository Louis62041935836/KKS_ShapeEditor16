using System;
using KKAPI.Maker;
using UnityEngine;
using AIChara;

namespace KKShapeEditor
{
	public static class MakerUI
	{
		public static void Init()
		{
			MakerAPI.MakerBaseLoaded += OnMakerBaseLoaded;
			MakerAPI.MakerExiting += OnMakerExiting;
		}

		private static void OnMakerBaseLoaded(object s, RegisterCustomControlsEvent e)
		{
			OnMakerLoaded();
		}

		private static void OnMakerExiting(object s, EventArgs e)
		{
			OnMakerExit();
		}

		private static void OnMakerLoaded()
		{
			_window = new ShapeEditorWindow(1263734784, new Rect(400f, 20f, 380f, 600f));
			_overlayGo = new GameObject("KKShapeEditor_MakerOverlay");
			_overlay = _overlayGo.AddComponent<ShapePaintOverlay>();
			_overlay.Window = _window;
			_overlay.SelectionTool = new SelectionTool();
			_overlay.Input = new InputHelper();
			_overlay.Input.Init();
			_overlay.OnRefreshRenderers = RefreshRenderers;
			RefreshRenderers();
		}

		private static void OnMakerExit()
		{
			ShapeEditorController.UnbindCorruptionSubscription(RefreshRenderers, ref _subscribedCtrl);
			if (_window != null)
			{
				_window.Cleanup();
				_window = null;
			}
			if (_overlayGo != null)
			{
				UnityEngine.Object.Destroy(_overlayGo);
				_overlayGo = null;
				_overlay = null;
			}
		}

		public static void RefreshRenderers()
		{
			if (_window == null)
			{
				return;
			}
			ChaControl characterControl = MakerAPI.GetCharacterControl();
			if (characterControl == null)
			{
				return;
			}
			ShapeEditorController component = characterControl.gameObject.GetComponent<ShapeEditorController>();
			if (component == null)
			{
				return;
			}
			component.ChaControl = characterControl;
			component.BindWindow(_window, RefreshRenderers, ref _subscribedCtrl);
		}

		public static ShapeEditorWindow Window
		{
			get { return _window; }
		}

		public static ShapePaintOverlay Overlay
		{
			get { return _overlay; }
		}

		private static ShapeEditorWindow _window;
		private static GameObject _overlayGo;
		private static ShapePaintOverlay _overlay;
		private static ShapeEditorController _subscribedCtrl;
	}
}