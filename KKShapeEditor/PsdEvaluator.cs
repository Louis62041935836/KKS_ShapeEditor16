using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000019 RID: 25
	public static class PsdEvaluator
	{
		// Token: 0x060000F0 RID: 240 RVA: 0x000088E4 File Offset: 0x00006AE4
		public static void Evaluate(ShapeEditorController controller)
		{
			if (controller == null)
			{
				return;
			}
			List<PsdDriver> drivers = controller.Drivers;
			if (drivers == null || drivers.Count == 0)
			{
				return;
			}
			Transform rootTransform = controller.RootTransform;
			if (rootTransform == null)
			{
				return;
			}
			Dictionary<string, DeformData> allDeformData = controller.GetAllDeformData();
			if (allDeformData == null)
			{
				return;
			}
			bool isAvailable = TimelineCompat.IsAvailable;
			int frameCount = Time.frameCount;
			PsdEvaluator._seenTargets.Clear();
			for (int i = 0; i < drivers.Count; i++)
			{
				PsdDriver psdDriver = drivers[i];
				if (psdDriver != null && psdDriver.Enabled && !Mathf.Approximately(psdDriver.InputMin, psdDriver.InputMax) && PsdEvaluator._seenTargets.Add(psdDriver.TargetLayerId ?? ""))
				{
					DeformLayer deformLayer = PsdEvaluator.FindLayerById(allDeformData, psdDriver.TargetRendererPath, psdDriver.TargetLayerId);
					if (deformLayer != null && !PsdEvaluator.IsTimelineOwning(deformLayer, frameCount, isAvailable) && !CsbEvaluator.IsCsbOwning(deformLayer, frameCount) && !NbbEvaluator.IsNbbOwning(deformLayer, frameCount))
					{
						Transform transform = PsdEvaluator.ResolveBone(rootTransform, psdDriver.SourceBonePath);
						if (!(transform == null))
						{
							float num = PsdEvaluator.ReadChannel(transform, psdDriver.Channel);
							float num2 = Mathf.InverseLerp(psdDriver.InputMin, psdDriver.InputMax, num);
							if (!Mathf.Approximately(deformLayer.Weight, num2))
							{
								deformLayer.Weight = num2;
								deformLayer.Dirty = true;
							}
						}
					}
				}
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00008A50 File Offset: 0x00006C50
		internal static DeformLayer FindLayerById(Dictionary<string, DeformData> dataMap, string rendererPath, string layerId)
		{
			if (dataMap == null || string.IsNullOrEmpty(layerId))
			{
				return null;
			}
			DeformData deformData;
			if (!dataMap.TryGetValue(rendererPath ?? "", out deformData) || deformData == null)
			{
				return null;
			}
			List<DeformLayer> layers = deformData.Layers;
			for (int i = 0; i < layers.Count; i++)
			{
				if (layers[i] != null && layers[i].Id == layerId)
				{
					return layers[i];
				}
			}
			return null;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00008AC1 File Offset: 0x00006CC1
		internal static bool IsTimelineOwning(DeformLayer layer, int frame, bool timelineActive)
		{
			return layer != null && timelineActive && frame - layer.LastTimelineWriteFrame <= 1;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00008ADB File Offset: 0x00006CDB
		internal static Transform ResolveBone(Transform root, string bonePath)
		{
			if (string.IsNullOrEmpty(bonePath))
			{
				return root;
			}
			return root.Find(bonePath);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00008AF0 File Offset: 0x00006CF0
		internal static float ReadChannel(Transform bone, PsdChannel channel)
		{
			switch (channel)
			{
			case PsdChannel.RotationX:
				return Mathf.DeltaAngle(0f, bone.localRotation.eulerAngles.x);
			case PsdChannel.RotationY:
				return Mathf.DeltaAngle(0f, bone.localRotation.eulerAngles.y);
			case PsdChannel.RotationZ:
				return Mathf.DeltaAngle(0f, bone.localRotation.eulerAngles.z);
			case PsdChannel.PositionX:
				return bone.localPosition.x;
			case PsdChannel.PositionY:
				return bone.localPosition.y;
			case PsdChannel.PositionZ:
				return bone.localPosition.z;
			default:
				return 0f;
			}
		}

		// Token: 0x04000060 RID: 96
		private static readonly HashSet<string> _seenTargets = new HashSet<string>();
	}
}
