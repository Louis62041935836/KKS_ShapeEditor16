using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000012 RID: 18
	public static class NbbEvaluator
	{
		// Token: 0x060000C0 RID: 192 RVA: 0x00006DA4 File Offset: 0x00004FA4
		public static void Evaluate(ShapeEditorController controller)
		{
			if (controller == null)
			{
				return;
			}
			List<NbbDriver> nbbDrivers = controller.NbbDrivers;
			if (nbbDrivers == null || nbbDrivers.Count == 0)
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
			NbbEvaluator._seenTargets.Clear();
			for (int i = 0; i < nbbDrivers.Count; i++)
			{
				NbbDriver nbbDriver = nbbDrivers[i];
				if (nbbDriver != null && nbbDriver.Enabled && !Mathf.Approximately(nbbDriver.InputMin, nbbDriver.InputMax) && NbbEvaluator._seenTargets.Add(nbbDriver.TargetLayerId ?? ""))
				{
					DeformLayer deformLayer = PsdEvaluator.FindLayerById(allDeformData, nbbDriver.TargetRendererPath, nbbDriver.TargetLayerId);
					float num;
					if (deformLayer != null && !PsdEvaluator.IsTimelineOwning(deformLayer, frameCount, isAvailable) && !CsbEvaluator.IsCsbOwning(deformLayer, frameCount) && nbbDriver.TryReadSourceWeight(rootTransform, out num))
					{
						float num2 = Mathf.InverseLerp(nbbDriver.InputMin, nbbDriver.InputMax, num);
						deformLayer.LastNbbWriteFrame = frameCount;
						if (!Mathf.Approximately(deformLayer.Weight, num2))
						{
							deformLayer.Weight = num2;
							deformLayer.Dirty = true;
						}
					}
				}
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006EEB File Offset: 0x000050EB
		internal static bool IsNbbOwning(DeformLayer layer, int frame)
		{
			return layer != null && frame - layer.LastNbbWriteFrame <= 1;
		}

		// Token: 0x04000041 RID: 65
		private static readonly HashSet<string> _seenTargets = new HashSet<string>();
	}
}
