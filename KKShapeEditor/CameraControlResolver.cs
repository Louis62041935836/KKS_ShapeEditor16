using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using KKAPI.Maker;
using KKAPI.Studio;
using Studio;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000005 RID: 5
	internal static class CameraControlResolver
	{
		// Token: 0x0600000C RID: 12 RVA: 0x000023C8 File Offset: 0x000005C8
		public static void Resolve(List<MonoBehaviour> scripts, List<Collider> triggerColliders)
		{
			if (scripts == null)
			{
				return;
			}
			scripts.Clear();
			if (triggerColliders != null)
			{
				triggerColliders.Clear();
			}
			if (StudioAPI.InsideStudio)
			{
				global::Studio.Studio instance = Singleton<global::Studio.Studio>.Instance;
				if (instance != null)
				{
					CameraControlResolver.Append(instance.cameraCtrl, scripts, triggerColliders);
				}
			}
			if (MakerAPI.InsideMaker)
			{
				CameraControlResolver.Append(UnityEngine.Object.FindObjectOfType<CameraControl_Ver2>(), scripts, triggerColliders);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002420 File Offset: 0x00000620
		private static void Append(MonoBehaviour script, List<MonoBehaviour> scripts, List<Collider> triggerColliders)
		{
			if (script == null)
			{
				return;
			}
			scripts.Add(script);
			if (triggerColliders == null)
			{
				return;
			}
			foreach (Collider collider in script.GetComponents<Collider>())
			{
				if (collider != null && collider.isTrigger)
				{
					triggerColliders.Add(collider);
				}
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002473 File Offset: 0x00000673
		public static bool IsFocusInProgress
		{
			get
			{
				return Time.frameCount <= CameraControlResolver._focusActiveUntilFrame;
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002484 File Offset: 0x00000684
		public static void FocusOn(Vector3 worldCenter, MonoBehaviour coroutineHost)
		{
			if (coroutineHost == null)
			{
				return;
			}
			CameraControlResolver._focusScratch.Clear();
			CameraControlResolver.Resolve(CameraControlResolver._focusScratch, null);
			MonoBehaviour monoBehaviour = ((CameraControlResolver._focusScratch.Count > 0) ? CameraControlResolver._focusScratch[0] : null);
			CameraControlResolver._focusScratch.Clear();
			if (monoBehaviour == null)
			{
				return;
			}
			PropertyInfo propertyInfo = CameraControlResolver.ResolveTargetProperty(monoBehaviour.GetType());
			if (propertyInfo == null)
			{
				CameraControlResolver.WarnNoTargetOnce();
				return;
			}
			CameraControlResolver._focusActiveUntilFrame = Time.frameCount + 12 + 1;
			coroutineHost.StartCoroutine(CameraControlResolver.LerpTargetCoroutine(monoBehaviour, propertyInfo, worldCenter));
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000251C File Offset: 0x0000071C
		private static PropertyInfo ResolveTargetProperty(Type ctrlType)
		{
			if (ctrlType == null)
			{
				return null;
			}
			if (CameraControlResolver._targetPropType == ctrlType)
			{
				return CameraControlResolver._targetProp;
			}
			CameraControlResolver._targetPropType = ctrlType;
			CameraControlResolver._targetProp = null;
			try
			{
				PropertyInfo propertyInfo = ctrlType.GetProperty("TargetPos", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? ctrlType.GetProperty("targetPos", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (propertyInfo != null && propertyInfo.PropertyType == typeof(Vector3) && propertyInfo.CanRead && propertyInfo.CanWrite)
				{
					CameraControlResolver._targetProp = propertyInfo;
				}
			}
			catch
			{
				CameraControlResolver._targetProp = null;
			}
			return CameraControlResolver._targetProp;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000025C8 File Offset: 0x000007C8
		private static IEnumerator LerpTargetCoroutine(MonoBehaviour ctrl, PropertyInfo prop, Vector3 worldCenter)
		{
			Vector3 startPos = (Vector3)prop.GetValue(ctrl);
			int frameCount = 0;
			int totalFrames = 12; // FocusLerpFrames constant

			while (frameCount < totalFrames)
			{
				frameCount++;
				float t = (float)frameCount / totalFrames;
				Vector3 newPos = Vector3.Lerp(startPos, worldCenter, t);
				prop.SetValue(ctrl, newPos);
				yield return null;
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000025E5 File Offset: 0x000007E5
		private static void WarnNoTargetOnce()
		{
			if (CameraControlResolver._warnedNoTarget)
			{
				return;
			}
			CameraControlResolver._warnedNoTarget = true;
			ManualLogSource logger = ShapeEditorPlugin.Logger;
			if (logger == null)
			{
				return;
			}
			logger.LogWarning("[CameraFocus] Could not access camera target property (TargetPos/targetPos); focus skipped.");
		}

		// Token: 0x04000009 RID: 9
		private const int FocusLerpFrames = 12;

		// Token: 0x0400000A RID: 10
		private static readonly List<MonoBehaviour> _focusScratch = new List<MonoBehaviour>();

		// Token: 0x0400000B RID: 11
		private static Type _targetPropType;

		// Token: 0x0400000C RID: 12
		private static PropertyInfo _targetProp;

		// Token: 0x0400000D RID: 13
		private static bool _warnedNoTarget;

		// Token: 0x0400000E RID: 14
		private static int _focusActiveUntilFrame = -1;
	}
}
