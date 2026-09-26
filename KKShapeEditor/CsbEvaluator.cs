using System;
using System.Collections.Generic;
using UnityEngine;
using AIChara;

namespace KKShapeEditor
{
	// Token: 0x0200000A RID: 10
	public static class CsbEvaluator
	{
		// Token: 0x0600002D RID: 45 RVA: 0x00003084 File Offset: 0x00001284
		public static void Evaluate(ShapeEditorController controller)
		{
			if (controller == null)
			{
				return;
			}
			List<CsbDriver> csbDrivers = controller.CsbDrivers;
			if (csbDrivers == null || csbDrivers.Count == 0)
			{
				return;
			}
			ChaControl chaControl = controller.ChaControl;
			if (chaControl == null)
			{
				return;
			}
			ChaFileStatus fileStatus = chaControl.fileStatus;
			byte[] array = ((fileStatus != null) ? fileStatus.clothesState : null);
			int num = -1;
			Dictionary<string, DeformData> allDeformData = controller.GetAllDeformData();
			if (allDeformData == null)
			{
				return;
			}
			bool isAvailable = TimelineCompat.IsAvailable;
			int frameCount = Time.frameCount;
			CsbEvaluator._seenTargets.Clear();
			for (int i = 0; i < csbDrivers.Count; i++)
			{
				CsbDriver csbDriver = csbDrivers[i];
				if (csbDriver != null && csbDriver.Enabled && CsbEvaluator._seenTargets.Add(csbDriver.TargetLayerId ?? ""))
				{
					DeformLayer deformLayer = PsdEvaluator.FindLayerById(allDeformData, csbDriver.TargetRendererPath, csbDriver.TargetLayerId);
					if (deformLayer != null && !PsdEvaluator.IsTimelineOwning(deformLayer, frameCount, isAvailable))
					{
						int clothingKind = csbDriver.ClothingKind;
						float num3;
						if ((csbDriver.CoordinateScope < 0 || csbDriver.CoordinateScope == num) && CsbEvaluator.IsClothingEquipped(array, chaControl, clothingKind))
						{
							int num2 = (int)array[clothingKind];
							float[] stateWeights = csbDriver.StateWeights;
							if (stateWeights == null || stateWeights.Length == 0)
							{
								num3 = csbDriver.UnequippedWeight;
							}
							else
							{
								num3 = ((num2 < stateWeights.Length) ? stateWeights[num2] : stateWeights[stateWeights.Length - 1]);
							}
						}
						else
						{
							num3 = csbDriver.UnequippedWeight;
						}
						num3 = CsbDriver.SanitizeWeight(num3);
						deformLayer.LastCsbWriteFrame = frameCount;
						if (!Mathf.Approximately(deformLayer.Weight, num3))
						{
							deformLayer.Weight = num3;
							deformLayer.Dirty = true;
						}
					}
				}
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003237 File Offset: 0x00001437
		internal static bool IsCsbOwning(DeformLayer layer, int frame)
		{
			return layer != null && frame - layer.LastCsbWriteFrame <= 1;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000324C File Offset: 0x0000144C
		internal static bool IsClothingEquipped(byte[] clothesState, ChaControl chaCtrl, int kind)
		{
			return kind >= 0 && clothesState != null && kind < clothesState.Length && chaCtrl != null && chaCtrl.IsClothes(kind);
		}

		// Token: 0x04000019 RID: 25
		private static readonly HashSet<string> _seenTargets = new HashSet<string>();
	}
}
