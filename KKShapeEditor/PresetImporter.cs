using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000013 RID: 19
	public static class PresetImporter
	{
		// Token: 0x060000C3 RID: 195 RVA: 0x00006F0C File Offset: 0x0000510C
		public static List<int> DefaultCheckedIndices(PresetBundle bundle, Func<PresetEntry, List<Renderer>> resolveTargets)
		{
			List<int> list = new List<int>();
			if (bundle == null || bundle.Entries == null || resolveTargets == null)
			{
				return list;
			}
			for (int i = 0; i < bundle.Entries.Count; i++)
			{
				PresetEntry presetEntry = bundle.Entries[i];
				if (presetEntry != null)
				{
					List<Renderer> list2 = resolveTargets(presetEntry);
					if (list2 != null && list2.Count > 0)
					{
						list.Add(i);
					}
				}
			}
			return list;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00006F74 File Offset: 0x00005174
		public static Renderer ResolveByPath(IList<Renderer> renderers, IList<string> paths, string presetPath)
		{
			if (renderers == null || paths == null)
			{
				return null;
			}
			string text = presetPath ?? "";
			int num = 0;
			while (num < paths.Count && num < renderers.Count)
			{
				if ((paths[num] ?? "") == text)
				{
					return renderers[num];
				}
				num++;
			}
			return null;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006FD0 File Offset: 0x000051D0
		public static List<Renderer> FindSameAssetRenderers(IList<Renderer> renderers, IList<string> paths, PresetEntry entry)
		{
			List<Renderer> list = new List<Renderer>();
			if (renderers == null || paths == null || entry == null)
			{
				return list;
			}
			string text = entry.RendererPath ?? "";
			int num = 0;
			while (num < paths.Count && num < renderers.Count)
			{
				Renderer renderer = renderers[num];
				if (renderer != null && PresetImporter.LeafEquals(paths[num], text))
				{
					list.Add(renderer);
				}
				num++;
			}
			return list;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00007040 File Offset: 0x00005240
		public static bool HasSameAssetRenderer(IList<Renderer> renderers, IList<string> paths, PresetEntry entry)
		{
			if (renderers == null || paths == null || entry == null)
			{
				return false;
			}
			string text = entry.RendererPath ?? "";
			int num = 0;
			while (num < paths.Count && num < renderers.Count)
			{
				if (renderers[num] != null && PresetImporter.LeafEquals(paths[num], text))
				{
					return true;
				}
				num++;
			}
			return false;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000070A4 File Offset: 0x000052A4
		private static bool LeafEquals(string a, string b)
		{
			if (a == null)
			{
				a = "";
			}
			if (b == null)
			{
				b = "";
			}
			int num = a.LastIndexOf('/') + 1;
			int num2 = b.LastIndexOf('/') + 1;
			int num3 = a.Length - num;
			return num3 == b.Length - num2 && string.CompareOrdinal(a, num, b, num2, num3) == 0;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000070FE File Offset: 0x000052FE
		public static IEnumerator ApplyImport(PresetBundle bundle, IList<int> checkedIndices, bool preserveExisting, Func<PresetEntry, List<Renderer>> resolveTargets, bool preserveCoordinateScope = true)
		{
			if (bundle == null || bundle.Entries == null || checkedIndices == null || resolveTargets == null)
			{
				yield break;
			}

			PresetImporter.BatchClaims claims = PresetImporter.BuildExactPathClaims(bundle.Entries, resolveTargets);
			HashSet<ShapeEditorController> charaControllers = new HashSet<ShapeEditorController>();
			HashSet<ItemShapeController> itemControllers = new HashSet<ItemShapeController>();
			List<string> skipped = new List<string>();

			for (int i = 0; i < checkedIndices.Count; i++)
			{
				int index = checkedIndices[i];
				if (index >= 0 && index < bundle.Entries.Count)
				{
					PresetEntry entry = bundle.Entries[index];
					if (entry != null)
					{
						List<Renderer> targets = PresetImporter.ResolveEffectiveTargets(entry, resolveTargets, claims);
						if (targets != null)
						{
							for (int j = 0; j < targets.Count; j++)
							{
								PresetImporter.ApplyEntryToTarget(entry, targets[j], preserveExisting, preserveCoordinateScope, skipped, charaControllers, itemControllers);
								yield return null;
							}
						}
					}
				}
			}

			foreach (ShapeEditorController chara in charaControllers)
			{
				if (chara != null)
				{
					chara.NotifyDataChanged();
				}
			}

			foreach (ItemShapeController item in itemControllers)
			{
				if (item != null)
				{
					item.NotifyDataChanged();
				}
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000712C File Offset: 0x0000532C
		private static PresetImporter.BatchClaims BuildExactPathClaims(List<PresetEntry> entries, Func<PresetEntry, List<Renderer>> resolveTargets)
		{
			PresetImporter.BatchClaims batchClaims = new PresetImporter.BatchClaims();
			for (int i = 0; i < entries.Count; i++)
			{
				PresetEntry presetEntry = entries[i];
				if (!batchClaims.ByEntry.ContainsKey(presetEntry))
				{
					string text = presetEntry.RendererPath ?? "";
					List<Renderer> list = resolveTargets(presetEntry);
					if (list != null)
					{
						for (int j = 0; j < list.Count; j++)
						{
							Renderer renderer = list[j];
							if (!(renderer == null) && !batchClaims.Renderers.Contains(renderer))
							{
								string ownerRelativePath = PresetImporter.GetOwnerRelativePath(renderer);
								if (ownerRelativePath != null && !(ownerRelativePath != text))
								{
									batchClaims.Add(presetEntry, renderer, ownerRelativePath);
									break;
								}
							}
						}
					}
				}
			}
			return batchClaims;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000071E8 File Offset: 0x000053E8
		private static List<Renderer> ResolveEffectiveTargets(PresetEntry entry, Func<PresetEntry, List<Renderer>> resolveTargets, PresetImporter.BatchClaims claims)
		{
			return PresetImporter.FilterClaimedTargets(entry, resolveTargets(entry), claims);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000071F8 File Offset: 0x000053F8
		private static List<Renderer> FilterClaimedTargets(PresetEntry entry, List<Renderer> targets, PresetImporter.BatchClaims claims)
		{
			if (targets == null || targets.Count == 0 || claims.Renderers.Count == 0)
			{
				return targets;
			}
			Renderer renderer;
			claims.ByEntry.TryGetValue(entry, out renderer);
			string text = entry.RendererPath ?? "";
			List<Renderer> list = new List<Renderer>(targets.Count);
			for (int i = 0; i < targets.Count; i++)
			{
				Renderer renderer2 = targets[i];
				if (!(renderer2 == null))
				{
					if (renderer2 == renderer)
					{
						list.Insert(0, renderer2);
					}
					else if (!claims.Renderers.Contains(renderer2))
					{
						string ownerRelativePath = PresetImporter.GetOwnerRelativePath(renderer2);
						if (ownerRelativePath == null || !claims.Paths.Contains(ownerRelativePath) || (renderer != null && ownerRelativePath == text))
						{
							list.Add(renderer2);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000072CC File Offset: 0x000054CC
		private static string GetOwnerRelativePath(Renderer renderer)
		{
			if (renderer == null)
			{
				return null;
			}
			ControllerResolver.Owner owner = ControllerResolver.Resolve(renderer);
			if (!owner.HasOwner)
			{
				return null;
			}
			return ShapeEditorController.GetRelativePath(owner.RootTransform, renderer.transform);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00007308 File Offset: 0x00005508
		private static bool ApplyEntryToTarget(PresetEntry entry, Renderer target, bool preserveExisting, bool preserveCoordinateScope, List<string> skipped, HashSet<ShapeEditorController> charaControllers, HashSet<ItemShapeController> itemControllers)
		{
			if (target == null)
			{
				return false;
			}
			ControllerResolver.Owner owner = ControllerResolver.Resolve(target);
			if (!owner.HasOwner)
			{
				skipped.Add(entry.RendererPath ?? "");
				ShapeEditorPlugin.Logger.LogInfo("[Preset] skip (no owner controller): " + entry.RendererPath);
				return false;
			}
			string relativePath = ShapeEditorController.GetRelativePath(owner.RootTransform, target.transform);
			Mesh mesh = MeshHelper.GetMesh(target);
			if (mesh == null)
			{
				skipped.Add(relativePath);
				ShapeEditorPlugin.Logger.LogInfo("[Preset] skip (mesh null post-subdivide): " + relativePath);
				return false;
			}
			if (mesh.vertexCount != entry.VertexCount)
			{
				skipped.Add(relativePath);
				ShapeEditorPlugin.Logger.LogInfo(string.Concat(new string[]
				{
					"[Preset] skip (vertex count mismatch: live=",
					mesh.vertexCount.ToString(),
					", preset=",
					entry.VertexCount.ToString(),
					", subdivLevel=",
					entry.SubdivLevel.ToString(),
					", current=",
					MeshHelper.GetSubdivisionLevel(target).ToString(),
					"): ",
					relativePath
				}));
				return false;
			}
			DeformData orCreateDeformData = owner.GetOrCreateDeformData(target);
			if (orCreateDeformData == null)
			{
				skipped.Add(relativePath);
				return false;
			}
			PresetSerializer.ApplyEntryToRenderer(entry, orCreateDeformData, preserveExisting);
			if (!preserveExisting && owner.IsOnCharacter && ((entry.PsdDrivers != null && entry.PsdDrivers.Count > 0) || (entry.CsbDrivers != null && entry.CsbDrivers.Count > 0) || (entry.NbbDrivers != null && entry.NbbDrivers.Count > 0)))
			{
				owner.CharacterController.InstallImportedDrivers(relativePath, entry.CollectLayerIds(), entry.PsdDrivers, entry.CsbDrivers, entry.NbbDrivers, preserveCoordinateScope);
			}
			if (owner.IsOnCharacter && owner.CharacterController.CorruptionState != null)
			{
				owner.CharacterController.CorruptionState.Remove(relativePath);
			}
			if (owner.IsOnItem)
			{
				owner.ItemController.ReattachAll();
			}
			else if (owner.IsOnCharacter)
			{
				owner.CharacterController.ReinitDeformerForPath(relativePath);
			}
			ShapeDeformer component = target.GetComponent<ShapeDeformer>();
			if (component != null)
			{
				component.InvalidateDeltaCache();
			}
			if (owner.IsOnCharacter)
			{
				charaControllers.Add(owner.CharacterController);
			}
			else
			{
				itemControllers.Add(owner.ItemController);
			}
			return true;
		}

		// Token: 0x02000059 RID: 89
		private sealed class BatchClaims
		{
			// Token: 0x06000475 RID: 1141 RVA: 0x0002D443 File Offset: 0x0002B643
			public void Add(PresetEntry entry, Renderer renderer, string path)
			{
				this.ByEntry[entry] = renderer;
				this.Renderers.Add(renderer);
				this.Paths.Add(path);
			}

			// Token: 0x04000475 RID: 1141
			public readonly Dictionary<PresetEntry, Renderer> ByEntry = new Dictionary<PresetEntry, Renderer>();

			// Token: 0x04000476 RID: 1142
			public readonly HashSet<Renderer> Renderers = new HashSet<Renderer>();

			// Token: 0x04000477 RID: 1143
			public readonly HashSet<string> Paths = new HashSet<string>();
		}
	}
}
