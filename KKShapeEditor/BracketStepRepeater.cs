using System;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200003F RID: 63
	public struct BracketStepRepeater
	{
		// Token: 0x060002DC RID: 732 RVA: 0x00017FF8 File Offset: 0x000161F8
		private static float RepeatInterval()
		{
			return 1f / Mathf.Max(0.01f, ShapeEditorPlugin.BrushAdjustRepeatRate.Value);
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00018014 File Offset: 0x00016214
		public int Tick(bool leftHeld, bool rightHeld, float now)
		{
			int num = 0;
			if (leftHeld && !rightHeld)
			{
				num = -1;
			}
			else if (rightHeld && !leftHeld)
			{
				num = 1;
			}
			if (num == 0)
			{
				this._activeDir = 0;
				return 0;
			}
			if (num != this._activeDir)
			{
				this._activeDir = num;
				this._nextRepeatTime = now + 0.4f;
				return num;
			}
			if (now >= this._nextRepeatTime)
			{
				this._nextRepeatTime = now + BracketStepRepeater.RepeatInterval();
				return num;
			}
			return 0;
		}

		// Token: 0x04000189 RID: 393
		public const float RadiusStep = 0.005f;

		// Token: 0x0400018A RID: 394
		private const float RepeatDelay = 0.4f;

		// Token: 0x0400018B RID: 395
		private int _activeDir;

		// Token: 0x0400018C RID: 396
		private float _nextRepeatTime;
	}
}
