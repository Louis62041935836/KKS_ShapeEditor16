using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BepInEx.Configuration;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000042 RID: 66
	public class InputHelper
	{
		// Token: 0x0600030A RID: 778
		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out InputHelper.POINT lpPoint);

		// Token: 0x0600030B RID: 779
		[DllImport("user32.dll")]
		private static extern bool ScreenToClient(IntPtr hWnd, ref InputHelper.POINT lpPoint);

		// Token: 0x0600030C RID: 780
		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int vKey);

		// Token: 0x0600030D RID: 781 RVA: 0x0001A200 File Offset: 0x00018400
		public static void ApplyUndoRedoHotkeys(KeyboardShortcut undo, KeyboardShortcut redo)
		{
			InputHelper.ParseShortcut(undo, out InputHelper._undoVk, out InputHelper._undoReqCtrl, out InputHelper._undoReqShift, out InputHelper._undoReqAlt);
			InputHelper.ParseShortcut(redo, out InputHelper._redoVk, out InputHelper._redoReqCtrl, out InputHelper._redoReqShift, out InputHelper._redoReqAlt);
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0001A238 File Offset: 0x00018438
		private static void ParseShortcut(KeyboardShortcut sc, out int vk, out bool reqCtrl, out bool reqShift, out bool reqAlt)
		{
			vk = ShapeEditorPlugin.KeyCodeToVk(sc.MainKey);
			reqCtrl = false;
			reqShift = false;
			reqAlt = false;
			foreach (KeyCode keyCode in sc.Modifiers)
			{
				if (keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl)
				{
					reqCtrl = true;
				}
				else if (keyCode == KeyCode.LeftShift || keyCode == KeyCode.RightShift)
				{
					reqShift = true;
				}
				else if (keyCode == KeyCode.LeftAlt || keyCode == KeyCode.RightAlt)
				{
					reqAlt = true;
				}
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0001A2D8 File Offset: 0x000184D8
		// (set) Token: 0x06000310 RID: 784 RVA: 0x0001A2E0 File Offset: 0x000184E0
		public bool MouseButton { get; private set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000311 RID: 785 RVA: 0x0001A2E9 File Offset: 0x000184E9
		// (set) Token: 0x06000312 RID: 786 RVA: 0x0001A2F1 File Offset: 0x000184F1
		public bool MouseButtonR { get; private set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000313 RID: 787 RVA: 0x0001A2FA File Offset: 0x000184FA
		// (set) Token: 0x06000314 RID: 788 RVA: 0x0001A302 File Offset: 0x00018502
		public bool CtrlHeld { get; private set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000315 RID: 789 RVA: 0x0001A30B File Offset: 0x0001850B
		// (set) Token: 0x06000316 RID: 790 RVA: 0x0001A313 File Offset: 0x00018513
		public bool ShiftHeld { get; private set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000317 RID: 791 RVA: 0x0001A31C File Offset: 0x0001851C
		// (set) Token: 0x06000318 RID: 792 RVA: 0x0001A324 File Offset: 0x00018524
		public bool AltHeld { get; private set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000319 RID: 793 RVA: 0x0001A32D File Offset: 0x0001852D
		// (set) Token: 0x0600031A RID: 794 RVA: 0x0001A335 File Offset: 0x00018535
		public bool BracketLeftHeld { get; private set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x0600031B RID: 795 RVA: 0x0001A33E File Offset: 0x0001853E
		// (set) Token: 0x0600031C RID: 796 RVA: 0x0001A346 File Offset: 0x00018546
		public bool BracketRightHeld { get; private set; }

		// Token: 0x0600031D RID: 797 RVA: 0x0001A34F File Offset: 0x0001854F
		public static bool IsCtrlHeldDirect()
		{
			return ((int)InputHelper.GetAsyncKeyState(162) & 32768) != 0 || ((int)InputHelper.GetAsyncKeyState(163) & 32768) != 0;
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600031E RID: 798 RVA: 0x0001A378 File Offset: 0x00018578
		// (set) Token: 0x0600031F RID: 799 RVA: 0x0001A380 File Offset: 0x00018580
		public bool MouseButtonDown { get; private set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000320 RID: 800 RVA: 0x0001A389 File Offset: 0x00018589
		// (set) Token: 0x06000321 RID: 801 RVA: 0x0001A391 File Offset: 0x00018591
		public bool MouseButtonUp { get; private set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000322 RID: 802 RVA: 0x0001A39A File Offset: 0x0001859A
		// (set) Token: 0x06000323 RID: 803 RVA: 0x0001A3A2 File Offset: 0x000185A2
		public Vector3 MousePosition { get; private set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000324 RID: 804 RVA: 0x0001A3AB File Offset: 0x000185AB
		// (set) Token: 0x06000325 RID: 805 RVA: 0x0001A3B3 File Offset: 0x000185B3
		public bool UndoPressed { get; private set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000326 RID: 806 RVA: 0x0001A3BC File Offset: 0x000185BC
		// (set) Token: 0x06000327 RID: 807 RVA: 0x0001A3C4 File Offset: 0x000185C4
		public bool RedoPressed { get; private set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000328 RID: 808 RVA: 0x0001A3CD File Offset: 0x000185CD
		// (set) Token: 0x06000329 RID: 809 RVA: 0x0001A3D5 File Offset: 0x000185D5
		public bool GrowPressed { get; private set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600032A RID: 810 RVA: 0x0001A3DE File Offset: 0x000185DE
		// (set) Token: 0x0600032B RID: 811 RVA: 0x0001A3E6 File Offset: 0x000185E6
		public bool ShrinkPressed { get; private set; }

		// Token: 0x0600032C RID: 812 RVA: 0x0001A3F0 File Offset: 0x000185F0
		public void Init()
		{
			if (InputHelper._gameWindowHandle == IntPtr.Zero)
			{
				try
				{
					InputHelper._gameWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
				}
				catch
				{
				}
			}
			this.FindCameraControls();
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0001A438 File Offset: 0x00018638
		public void PollInput()
		{
			this._prevMouseButton = this.MouseButton;
			this.MouseButton = ((int)InputHelper.GetAsyncKeyState(1) & 32768) != 0;
			this.MouseButtonR = ((int)InputHelper.GetAsyncKeyState(2) & 32768) != 0;
			this.CtrlHeld = ((int)InputHelper.GetAsyncKeyState(162) & 32768) != 0 || ((int)InputHelper.GetAsyncKeyState(163) & 32768) != 0;
			this.ShiftHeld = ((int)InputHelper.GetAsyncKeyState(160) & 32768) != 0 || ((int)InputHelper.GetAsyncKeyState(161) & 32768) != 0;
			this.AltHeld = ((int)InputHelper.GetAsyncKeyState(164) & 32768) != 0 || ((int)InputHelper.GetAsyncKeyState(165) & 32768) != 0;
			this.BracketLeftHeld = ((int)InputHelper.GetAsyncKeyState(219) & 32768) != 0;
			this.BracketRightHeld = ((int)InputHelper.GetAsyncKeyState(221) & 32768) != 0;
			this.MouseButtonDown = this.MouseButton && !this._prevMouseButton;
			this.MouseButtonUp = !this.MouseButton && this._prevMouseButton;
			bool flag = ((int)InputHelper.GetAsyncKeyState(InputHelper._undoVk) & 32768) != 0;
			bool flag2 = ((int)InputHelper.GetAsyncKeyState(InputHelper._redoVk) & 32768) != 0;
			float unscaledTime = Time.unscaledTime;
			this.UndoPressed = flag && !this._prevUndoKey && this.MatchModifiers(InputHelper._undoReqCtrl, InputHelper._undoReqShift, InputHelper._undoReqAlt) && unscaledTime - this._undoDebounceTime > 0.15f;
			this.RedoPressed = flag2 && !this._prevRedoKey && this.MatchModifiers(InputHelper._redoReqCtrl, InputHelper._redoReqShift, InputHelper._redoReqAlt) && unscaledTime - this._redoDebounceTime > 0.15f;
			if (this.UndoPressed)
			{
				this._undoDebounceTime = unscaledTime;
			}
			if (this.RedoPressed)
			{
				this._redoDebounceTime = unscaledTime;
			}
			this._prevUndoKey = flag;
			this._prevRedoKey = flag2;
			bool flag3 = ((int)InputHelper.GetAsyncKeyState(187) & 32768) != 0;
			bool flag4 = ((int)InputHelper.GetAsyncKeyState(189) & 32768) != 0;
			bool flag5 = !this.CtrlHeld && !this.AltHeld;
			this.GrowPressed = flag3 && !this._prevGrowKey && flag5 && unscaledTime - this._growDebounceTime > 0.15f;
			this.ShrinkPressed = flag4 && !this._prevShrinkKey && flag5 && unscaledTime - this._shrinkDebounceTime > 0.15f;
			if (this.GrowPressed)
			{
				this._growDebounceTime = unscaledTime;
			}
			if (this.ShrinkPressed)
			{
				this._shrinkDebounceTime = unscaledTime;
			}
			this._prevGrowKey = flag3;
			this._prevShrinkKey = flag4;
			this.MousePosition = this.GetRealMousePosition();
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0001A708 File Offset: 0x00018908
		public void UpdateCameraIsolation(bool editorActive)
		{
			if (!editorActive)
			{
				if (!this._cameraEnabled)
				{
					this.SetCameraEnabled(true);
				}
				return;
			}
			bool flag = this.CtrlHeld || CameraControlResolver.IsFocusInProgress;
			if (flag != this._cameraEnabled)
			{
				this.SetCameraEnabled(flag);
			}
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0001A749 File Offset: 0x00018949
		public void ResetGameInput()
		{
			if (!this.CtrlHeld)
			{
				Input.ResetInputAxes();
			}
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0001A758 File Offset: 0x00018958
		private bool MatchModifiers(bool reqCtrl, bool reqShift, bool reqAlt)
		{
			return reqCtrl == this.CtrlHeld && reqShift == this.ShiftHeld && reqAlt == this.AltHeld;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0001A778 File Offset: 0x00018978
		private Vector3 GetRealMousePosition()
		{
			InputHelper.POINT point;
			if (InputHelper._gameWindowHandle != IntPtr.Zero && InputHelper.GetCursorPos(out point))
			{
				InputHelper.ScreenToClient(InputHelper._gameWindowHandle, ref point);
				return new Vector3((float)point.X, (float)(Screen.height - point.Y), 0f);
			}
			return Input.mousePosition;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0001A7D0 File Offset: 0x000189D0
		private void FindCameraControls()
		{
			CameraControlResolver.Resolve(this._cameraScripts, null);
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0001A7E0 File Offset: 0x000189E0
		private void SetCameraEnabled(bool enabled)
		{
			this._cameraEnabled = enabled;
			foreach (MonoBehaviour monoBehaviour in this._cameraScripts)
			{
				if (monoBehaviour != null)
				{
					monoBehaviour.enabled = enabled;
				}
			}
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0001A844 File Offset: 0x00018A44
		public void Cleanup()
		{
			this.SetCameraEnabled(true);
		}

		// Token: 0x040001C8 RID: 456
		private const int VK_LBUTTON = 1;

		// Token: 0x040001C9 RID: 457
		private const int VK_RBUTTON = 2;

		// Token: 0x040001CA RID: 458
		private const int VK_LCONTROL = 162;

		// Token: 0x040001CB RID: 459
		private const int VK_RCONTROL = 163;

		// Token: 0x040001CC RID: 460
		private const int VK_LSHIFT = 160;

		// Token: 0x040001CD RID: 461
		private const int VK_RSHIFT = 161;

		// Token: 0x040001CE RID: 462
		private const int VK_LMENU = 164;

		// Token: 0x040001CF RID: 463
		private const int VK_RMENU = 165;

		// Token: 0x040001D0 RID: 464
		private const int VK_OEM_4 = 219;

		// Token: 0x040001D1 RID: 465
		private const int VK_OEM_6 = 221;

		// Token: 0x040001D2 RID: 466
		private const int VK_OEM_PLUS = 187;

		// Token: 0x040001D3 RID: 467
		private const int VK_OEM_MINUS = 189;

		// Token: 0x040001D4 RID: 468
		private static int _undoVk;

		// Token: 0x040001D5 RID: 469
		private static bool _undoReqCtrl;

		// Token: 0x040001D6 RID: 470
		private static bool _undoReqShift;

		// Token: 0x040001D7 RID: 471
		private static bool _undoReqAlt;

		// Token: 0x040001D8 RID: 472
		private static int _redoVk;

		// Token: 0x040001D9 RID: 473
		private static bool _redoReqCtrl;

		// Token: 0x040001DA RID: 474
		private static bool _redoReqShift;

		// Token: 0x040001DB RID: 475
		private static bool _redoReqAlt;

		// Token: 0x040001EA RID: 490
		private bool _prevMouseButton;

		// Token: 0x040001EB RID: 491
		private bool _prevUndoKey;

		// Token: 0x040001EC RID: 492
		private bool _prevRedoKey;

		// Token: 0x040001ED RID: 493
		private bool _prevGrowKey;

		// Token: 0x040001EE RID: 494
		private bool _prevShrinkKey;

		// Token: 0x040001EF RID: 495
		private float _undoDebounceTime;

		// Token: 0x040001F0 RID: 496
		private float _redoDebounceTime;

		// Token: 0x040001F1 RID: 497
		private float _growDebounceTime;

		// Token: 0x040001F2 RID: 498
		private float _shrinkDebounceTime;

		// Token: 0x040001F3 RID: 499
		private const float DebounceInterval = 0.15f;

		// Token: 0x040001F4 RID: 500
		private readonly List<MonoBehaviour> _cameraScripts = new List<MonoBehaviour>();

		// Token: 0x040001F5 RID: 501
		private bool _cameraEnabled = true;

		// Token: 0x040001F6 RID: 502
		private static IntPtr _gameWindowHandle;

		// Token: 0x02000070 RID: 112
		private struct POINT
		{
			// Token: 0x040004E8 RID: 1256
			public int X;

			// Token: 0x040004E9 RID: 1257
			public int Y;
		}
	}
}
