using System;
using System.Collections.Generic;
using KKAPI.Utilities;
using Studio;
using UnityEngine;
using AIChara;

namespace KKShapeEditor
{
	internal static class TimelineRegistry
	{
		public static void SyncController(ItemShapeController controller)
		{
			if (controller == null || !TimelineCompat.IsAvailable)
			{
				return;
			}
			ObjectCtrlInfo orResolveOci = TimelineRegistry.GetOrResolveOci<ItemShapeController>(controller, (c) => TimelineRegistry.ResolveItemOci(c));
			if (orResolveOci == null)
			{
				return;
			}
			TimelineRegistry.SyncImpl(controller, orResolveOci, controller.GetAllDeformData());
		}

		public static void SyncController(ShapeEditorController controller)
		{
			if (controller == null || !TimelineCompat.IsAvailable)
			{
				return;
			}
			ObjectCtrlInfo orResolveOci = TimelineRegistry.GetOrResolveOci<ShapeEditorController>(controller, (c) => TimelineRegistry.ResolveCharaOci(c));
			if (orResolveOci == null)
			{
				return;
			}
			TimelineRegistry.SyncImpl(controller, orResolveOci, controller.GetAllDeformData());
		}

		public static void UnregisterController(MonoBehaviour controller)
		{
			if (controller == null)
			{
				return;
			}
			bool flag = false;
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, List<TimelineRegistry.LayerOwner>> keyValuePair in TimelineRegistry._lookup)
			{
				List<TimelineRegistry.LayerOwner> value = keyValuePair.Value;
				for (int i = value.Count - 1; i >= 0; i--)
				{
					if (value[i].Controller == controller)
					{
						value.RemoveAt(i);
						flag = true;
					}
				}
				if (value.Count == 0)
				{
					list.Add(keyValuePair.Key);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				TimelineRegistry._lookup.Remove(list[j]);
			}
			TimelineRegistry._ociCache.Remove(controller);
			if (flag)
			{
				TimelineCompat.RefreshList();
			}
		}

		private static ObjectCtrlInfo GetOrResolveOci<T>(T controller, Func<T, ObjectCtrlInfo> resolver) where T : MonoBehaviour
		{
			ObjectCtrlInfo objectCtrlInfo;
			if (TimelineRegistry._ociCache.TryGetValue(controller, out objectCtrlInfo) && objectCtrlInfo != null)
			{
				return objectCtrlInfo;
			}
			ObjectCtrlInfo objectCtrlInfo2 = resolver(controller);
			if (objectCtrlInfo2 != null)
			{
				TimelineRegistry._ociCache[controller] = objectCtrlInfo2;
			}
			return objectCtrlInfo2;
		}

		private static void SyncImpl(MonoBehaviour controller, ObjectCtrlInfo oci, Dictionary<string, DeformData> dataMap)
		{
			if (dataMap == null)
			{
				return;
			}
			Dictionary<string, TimelineRegistry.LayerOwner> dictionary = new Dictionary<string, TimelineRegistry.LayerOwner>();
			foreach (KeyValuePair<string, DeformData> keyValuePair in dataMap)
			{
				string key = keyValuePair.Key;
				DeformData value = keyValuePair.Value;
				if (value != null)
				{
					foreach (DeformLayer deformLayer in value.Layers)
					{
						if (deformLayer != null && !string.IsNullOrEmpty(deformLayer.Id))
						{
							string text = key + "/" + deformLayer.Id;
							dictionary[text] = new TimelineRegistry.LayerOwner
							{
								Controller = controller,
								RendererPath = key,
								Layer = deformLayer,
								Oci = oci
							};
						}
					}
				}
			}
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, List<TimelineRegistry.LayerOwner>> keyValuePair2 in TimelineRegistry._lookup)
			{
				if (!dictionary.ContainsKey(keyValuePair2.Key))
				{
					List<TimelineRegistry.LayerOwner> value2 = keyValuePair2.Value;
					for (int i = value2.Count - 1; i >= 0; i--)
					{
						if (value2[i].Controller == controller)
						{
							value2.RemoveAt(i);
						}
					}
					if (value2.Count == 0)
					{
						list.Add(keyValuePair2.Key);
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				TimelineRegistry._lookup.Remove(list[j]);
			}
			foreach (KeyValuePair<string, TimelineRegistry.LayerOwner> keyValuePair3 in dictionary)
			{
				List<TimelineRegistry.LayerOwner> list2;
				if (!TimelineRegistry._lookup.TryGetValue(keyValuePair3.Key, out list2))
				{
					list2 = new List<TimelineRegistry.LayerOwner>(1);
					TimelineRegistry._lookup[keyValuePair3.Key] = list2;
				}
				bool flag = false;
				for (int k = 0; k < list2.Count; k++)
				{
					if (list2[k].Controller == controller)
					{
						list2[k] = keyValuePair3.Value;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list2.Add(keyValuePair3.Value);
				}
			}
			foreach (string text2 in dictionary.Keys)
			{
				if (TimelineRegistry._registeredIds.Add(text2))
				{
					TimelineRegistry.RegisterModel(text2);
				}
			}
			TimelineCompat.RefreshList();
		}

		private static void RegisterModel(string id)
		{
			try
			{
				TimelineRegistry.RegisterModelImpl(id);
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning(string.Concat(new string[]
				{
					"Timeline parameter registration failed for id=",
					id,
					" ex=",
					ex.GetType().Name,
					": ",
					ex.Message
				}));
			}
		}

		private static void RegisterModelImpl(string id)
		{
			TimelineCompatibility.AddInterpolableModelDynamic<float, DeformLayer>("KKShapeEditor", id, TimelineRegistry.BuildInitialName(id), 
				(ObjectCtrlInfo oci, DeformLayer layer, float left, float right, float factor) =>
				{
					if (layer == null)
					{
						return;
					}
					float num = Mathf.Lerp(left, right, factor);
					layer.Weight = Mathf.Clamp01(num);
					layer.Dirty = true;
					layer.LastTimelineWriteFrame = Time.frameCount;
				}, 
				null, 
				(ObjectCtrlInfo oci) => TimelineRegistry.IsCompatible(id, oci), 
				(ObjectCtrlInfo oci, DeformLayer layer) =>
				{
					if (layer == null)
					{
						return 0f;
					}
					return layer.Weight;
				}, 
				null, 
				null, 
				(ObjectCtrlInfo oci) => TimelineRegistry.GetLayerForOci(id, oci), 
				null, 
				null, 
				(ObjectCtrlInfo oci, DeformLayer layer, float left, float right) => layer != null, 
				true, 
				(string defaultName, ObjectCtrlInfo oci, DeformLayer layer) => TimelineRegistry.GetDisplayName(id, layer, defaultName), 
				(ObjectCtrlInfo oci, DeformLayer layer) => layer != null);
		}

		private static string GetDisplayName(string id, DeformLayer layer, string defaultName)
		{
			if (layer == null)
			{
				return defaultName;
			}
			List<TimelineRegistry.LayerOwner> list;
			if (!TimelineRegistry._lookup.TryGetValue(id, out list) || list.Count == 0)
			{
				return layer.Name;
			}
			return TimelineRegistry.GetRendererShortName(list[0].RendererPath) + ":" + layer.Name;
		}

		private static string BuildInitialName(string id)
		{
			List<TimelineRegistry.LayerOwner> list;
			if (TimelineRegistry._lookup.TryGetValue(id, out list) && list.Count > 0 && list[0].Layer != null)
			{
				return TimelineRegistry.GetRendererShortName(list[0].RendererPath) + ":" + list[0].Layer.Name;
			}
			return id;
		}

		private static string GetRendererShortName(string rendererPath)
		{
			if (string.IsNullOrEmpty(rendererPath))
			{
				return "(root)";
			}
			int num = rendererPath.LastIndexOf('/');
			if (num < 0)
			{
				return rendererPath;
			}
			return rendererPath.Substring(num + 1);
		}

		private static bool IsCompatible(string id, ObjectCtrlInfo oci)
		{
			List<TimelineRegistry.LayerOwner> list;
			if (!TimelineRegistry._lookup.TryGetValue(id, out list))
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				TimelineRegistry.LayerOwner layerOwner = list[i];
				if (layerOwner.Oci == oci && layerOwner.Layer != null)
				{
					return true;
				}
			}
			return false;
		}

		private static DeformLayer GetLayerForOci(string id, ObjectCtrlInfo oci)
		{
			List<TimelineRegistry.LayerOwner> list;
			if (!TimelineRegistry._lookup.TryGetValue(id, out list))
			{
				return null;
			}
			for (int i = 0; i < list.Count; i++)
			{
				TimelineRegistry.LayerOwner layerOwner = list[i];
				if (layerOwner.Oci == oci)
				{
					return layerOwner.Layer;
				}
			}
			return null;
		}

		private static ObjectCtrlInfo ResolveItemOci(ItemShapeController controller)
		{
			global::Studio.Studio instance = Singleton<global::Studio.Studio>.Instance;
			if (instance == null || controller == null)
			{
				return null;
			}
			GameObject gameObject = controller.gameObject;
			foreach (KeyValuePair<int, ObjectCtrlInfo> keyValuePair in instance.dicObjectCtrl)
			{
				OCIItem ociitem = keyValuePair.Value as OCIItem;
				if (ociitem != null && ociitem.objectItem == gameObject)
				{
					return ociitem;
				}
			}
			return null;
		}

		private static ObjectCtrlInfo ResolveCharaOci(ShapeEditorController controller)
		{
			global::Studio.Studio instance = Singleton<global::Studio.Studio>.Instance;
			if (instance == null || controller == null || controller.ChaControl == null)
			{
				return null;
			}
			ChaControl chaControl = controller.ChaControl;
			foreach (KeyValuePair<int, ObjectCtrlInfo> keyValuePair in instance.dicObjectCtrl)
			{
				OCIChar ocichar = keyValuePair.Value as OCIChar;
				if (ocichar != null && ocichar.charInfo == chaControl)
				{
					return ocichar;
				}
			}
			return null;
		}

		private static readonly HashSet<string> _registeredIds = new HashSet<string>();
		private static readonly Dictionary<string, List<TimelineRegistry.LayerOwner>> _lookup = new Dictionary<string, List<TimelineRegistry.LayerOwner>>();
		private static readonly Dictionary<MonoBehaviour, ObjectCtrlInfo> _ociCache = new Dictionary<MonoBehaviour, ObjectCtrlInfo>();

		private class LayerOwner
		{
			public MonoBehaviour Controller;
			public string RendererPath;
			public DeformLayer Layer;
			public ObjectCtrlInfo Oci;
		}
	}
}