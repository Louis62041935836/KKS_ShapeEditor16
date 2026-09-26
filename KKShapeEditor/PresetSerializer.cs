using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000016 RID: 22
	public static class PresetSerializer
	{
		// Token: 0x060000D1 RID: 209 RVA: 0x0000763C File Offset: 0x0000583C
		public static byte[] SerializeBundle(IList<PresetEntry> entries)
		{
			if (entries == null)
			{
				throw new ArgumentNullException("entries");
			}
			if (entries.Count == 0)
			{
				throw new ArgumentException("entries must contain at least one entry", "entries");
			}
			byte[] array2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write((byte)Magic0);
					binaryWriter.Write((byte)Magic1);
					binaryWriter.Write(FormatVersion);
					binaryWriter.Write(entries.Count);
					for (int i = 0; i < entries.Count; i++)
					{
						PresetEntry presetEntry = entries[i];
						if (presetEntry == null)
						{
							throw new ArgumentException("entries[" + i.ToString() + "] is null", "entries");
						}
						binaryWriter.Write(presetEntry.RendererPath ?? "");
						binaryWriter.Write(presetEntry.SubdivLevel);
						binaryWriter.Write(presetEntry.VertexCount);
						PresetSerializer.WriteFacesPerLevel(binaryWriter, presetEntry.FacesPerLevel, presetEntry.SubdivLevel);
						PresetSerializer.WriteSmoothPerLevel(binaryWriter, presetEntry.SmoothPerLevel, presetEntry.SubdivLevel);
						List<DeformLayer> list = presetEntry.Layers ?? new List<DeformLayer>();
						binaryWriter.Write(list.Count);
						binaryWriter.Write(0);
						for (int j = 0; j < list.Count; j++)
						{
							DeformLayer deformLayer = list[j];
							binaryWriter.Write(deformLayer.Name ?? "");
							binaryWriter.Write(deformLayer.Weight);
							binaryWriter.Write((deformLayer.Deltas != null) ? deformLayer.Deltas.Length : 0);
							if (deformLayer.Deltas != null)
							{
								for (int k = 0; k < deformLayer.Deltas.Length; k++)
								{
									binaryWriter.Write(deformLayer.Deltas[k].x);
									binaryWriter.Write(deformLayer.Deltas[k].y);
									binaryWriter.Write(deformLayer.Deltas[k].z);
								}
							}
							binaryWriter.Write(deformLayer.Id ?? "");
						}
						if (presetEntry.DeletedFaces != null && presetEntry.DeletedFaces.Length != 0)
						{
							int[] array = (int[])presetEntry.DeletedFaces.Clone();
							Array.Sort<int>(array);
							binaryWriter.Write(array.Length);
							for (int l = 0; l < array.Length; l++)
							{
								binaryWriter.Write(array[l]);
							}
						}
						else
						{
							binaryWriter.Write(0);
						}
						PresetSerializer.WritePsdDrivers(binaryWriter, presetEntry.PsdDrivers);
						PresetSerializer.WriteCsbDrivers(binaryWriter, presetEntry.CsbDrivers);
						PresetSerializer.WriteNbbDrivers(binaryWriter, presetEntry.NbbDrivers);
					}
					array2 = memoryStream.ToArray();
				}
			}
			return array2;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00007914 File Offset: 0x00005B14
		private static void WriteFacesPerLevel(BinaryWriter w, List<int[]> facesPerLevel, int expectedCount)
		{
			for (int i = 0; i < expectedCount; i++)
			{
				int[] array = ((facesPerLevel != null && i < facesPerLevel.Count) ? facesPerLevel[i] : null);
				if (array == null)
				{
					w.Write(-1);
				}
				else
				{
					w.Write(array.Length);
					for (int j = 0; j < array.Length; j++)
					{
						w.Write(array[j]);
					}
				}
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00007970 File Offset: 0x00005B70
		private static List<int[]> ReadFacesPerLevel(BinaryReader r, int count)
		{
			List<int[]> list = new List<int[]>(count);
			for (int i = 0; i < count; i++)
			{
				int num = r.ReadInt32();
				if (num < 0)
				{
					list.Add(null);
				}
				else
				{
					int[] array = new int[num];
					for (int j = 0; j < num; j++)
					{
						array[j] = r.ReadInt32();
					}
					list.Add(array);
				}
			}
			return list;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000079D0 File Offset: 0x00005BD0
		private static void WriteSmoothPerLevel(BinaryWriter w, bool[] smoothPerLevel, int expectedCount)
		{
			for (int i = 0; i < expectedCount; i++)
			{
				w.Write(MeshHelper.SmoothAt(smoothPerLevel, i));
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000079F8 File Offset: 0x00005BF8
		private static bool[] ReadSmoothPerLevel(BinaryReader r, int count)
		{
			bool[] array = new bool[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = r.ReadBoolean();
			}
			return array;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00007A24 File Offset: 0x00005C24
		private static void WritePsdDrivers(BinaryWriter w, List<PsdDriver> drivers)
		{
			int num = ((drivers != null) ? drivers.Count : 0);
			w.Write(num);
			for (int i = 0; i < num; i++)
			{
				PsdDriver psdDriver = drivers[i];
				w.Write(psdDriver.SourceBonePath ?? "");
				w.Write((int)psdDriver.Channel);
				w.Write(psdDriver.InputMin);
				w.Write(psdDriver.InputMax);
				w.Write(psdDriver.TargetRendererPath ?? "");
				w.Write(psdDriver.TargetLayerId ?? "");
				w.Write(psdDriver.Enabled);
			}
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00007AC8 File Offset: 0x00005CC8
		private static void WriteCsbDrivers(BinaryWriter w, List<CsbDriver> drivers)
		{
			int num = ((drivers != null) ? drivers.Count : 0);
			w.Write(num);
			for (int i = 0; i < num; i++)
			{
				CsbDriver csbDriver = drivers[i];
				w.Write(csbDriver.ClothingKind);
				float[] array = csbDriver.StateWeights ?? new float[0];
				for (int j = 0; j < 4; j++)
				{
					w.Write((j < array.Length) ? array[j] : 0f);
				}
				w.Write(csbDriver.UnequippedWeight);
				w.Write(csbDriver.TargetRendererPath ?? "");
				w.Write(csbDriver.TargetLayerId ?? "");
				w.Write(csbDriver.Enabled);
				w.Write(csbDriver.CoordinateScope);
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00007B98 File Offset: 0x00005D98
		private static void WriteNbbDrivers(BinaryWriter w, List<NbbDriver> drivers)
		{
			int num = ((drivers != null) ? drivers.Count : 0);
			w.Write(num);
			for (int i = 0; i < num; i++)
			{
				NbbDriver nbbDriver = drivers[i];
				w.Write(nbbDriver.SourceRendererPath ?? "");
				w.Write(nbbDriver.SourceShapeName ?? "");
				w.Write(nbbDriver.InputMin);
				w.Write(nbbDriver.InputMax);
				w.Write(nbbDriver.TargetRendererPath ?? "");
				w.Write(nbbDriver.TargetLayerId ?? "");
				w.Write(nbbDriver.Enabled);
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00007C4C File Offset: 0x00005E4C
		private static List<NbbDriver> ReadNbbDrivers(BinaryReader r)
		{
			int num = r.ReadInt32();
			if (num < 0 || num > 100000)
			{
				return null;
			}
			List<NbbDriver> list = new List<NbbDriver>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(new NbbDriver
				{
					SourceRendererPath = r.ReadString(),
					SourceShapeName = r.ReadString(),
					InputMin = r.ReadSingle(),
					InputMax = r.ReadSingle(),
					TargetRendererPath = r.ReadString(),
					TargetLayerId = r.ReadString(),
					Enabled = r.ReadBoolean()
				});
			}
			return list;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00007CE4 File Offset: 0x00005EE4
		private static List<PsdDriver> ReadPsdDrivers(BinaryReader r)
		{
			int num = r.ReadInt32();
			if (num < 0 || num > 100000)
			{
				return null;
			}
			List<PsdDriver> list = new List<PsdDriver>(num);
			for (int i = 0; i < num; i++)
			{
				PsdDriver psdDriver = new PsdDriver();
				psdDriver.SourceBonePath = r.ReadString();
				int num2 = r.ReadInt32();
				psdDriver.InputMin = r.ReadSingle();
				psdDriver.InputMax = r.ReadSingle();
				psdDriver.TargetRendererPath = r.ReadString();
				psdDriver.TargetLayerId = r.ReadString();
				psdDriver.Enabled = r.ReadBoolean();
				if (num2 >= 0 && num2 <= 5)
				{
					psdDriver.Channel = (PsdChannel)num2;
					list.Add(psdDriver);
				}
			}
			return list;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00007D8C File Offset: 0x00005F8C
		private static List<CsbDriver> ReadCsbDrivers(BinaryReader r)
		{
			int num = r.ReadInt32();
			if (num < 0 || num > 100000)
			{
				return null;
			}
			List<CsbDriver> list = new List<CsbDriver>(num);
			for (int i = 0; i < num; i++)
			{
				CsbDriver csbDriver = new CsbDriver();
				csbDriver.ClothingKind = r.ReadInt32();
				float[] array = new float[4];
				for (int j = 0; j < 4; j++)
				{
					array[j] = r.ReadSingle();
				}
				csbDriver.StateWeights = array;
				csbDriver.UnequippedWeight = r.ReadSingle();
				csbDriver.TargetRendererPath = r.ReadString();
				csbDriver.TargetLayerId = r.ReadString();
				csbDriver.Enabled = r.ReadBoolean();
				csbDriver.CoordinateScope = r.ReadInt32();
				csbDriver.NormalizeStateWeights();
				list.Add(csbDriver);
			}
			return list;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00007E50 File Offset: 0x00006050
		public static PresetBundle DeserializeBundle(byte[] bytes)
		{
			if (bytes == null || bytes.Length < 4)
			{
				ShapeEditorPlugin.Logger.LogWarning(L.PresetInvalidFile);
				return null;
			}
			if (bytes[0] != 75 || bytes[1] != 80)
			{
				ShapeEditorPlugin.Logger.LogWarning(L.PresetInvalidFile);
				return null;
			}
			PresetBundle presetBundle;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(bytes))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.ReadByte();
						binaryReader.ReadByte();
						int num = binaryReader.ReadInt32();
						if (num < 1 || num > 6)
						{
							ShapeEditorPlugin.Logger.LogWarning(string.Format("PresetSerializer: unsupported format version {0}", num));
							presetBundle = null;
						}
						else
						{
							int num2 = binaryReader.ReadInt32();
							if (num2 < 0 || num2 > 4096)
							{
								ShapeEditorPlugin.Logger.LogWarning(L.PresetInvalidFile);
								presetBundle = null;
							}
							else
							{
								PresetBundle presetBundle2 = new PresetBundle();
								presetBundle2.FormatVersion = num;
								for (int i = 0; i < num2; i++)
								{
									PresetEntry presetEntry = new PresetEntry();
									presetEntry.RendererPath = binaryReader.ReadString();
									presetEntry.SubdivLevel = binaryReader.ReadInt32();
									presetEntry.VertexCount = binaryReader.ReadInt32();
									if (presetEntry.SubdivLevel < 0 || presetEntry.SubdivLevel > 16 || presetEntry.VertexCount < 0 || presetEntry.VertexCount > 10000000)
									{
										ShapeEditorPlugin.Logger.LogWarning(L.PresetInvalidFile);
										return null;
									}
									if (num >= 2)
									{
										presetEntry.FacesPerLevel = PresetSerializer.ReadFacesPerLevel(binaryReader, presetEntry.SubdivLevel);
									}
									else
									{
										presetEntry.FacesPerLevel = new List<int[]>(presetEntry.SubdivLevel);
										for (int j = 0; j < presetEntry.SubdivLevel; j++)
										{
											presetEntry.FacesPerLevel.Add(null);
										}
									}
									presetEntry.SmoothPerLevel = ((num >= 5) ? PresetSerializer.ReadSmoothPerLevel(binaryReader, presetEntry.SubdivLevel) : new bool[presetEntry.SubdivLevel]);
									int num3 = binaryReader.ReadInt32();
									if (num3 < 0 || num3 > 256)
									{
										ShapeEditorPlugin.Logger.LogWarning(L.PresetInvalidFile);
										return null;
									}
									binaryReader.ReadInt32();
									for (int k = 0; k < num3; k++)
									{
										string text = binaryReader.ReadString();
										float num4 = binaryReader.ReadSingle();
										int num5 = binaryReader.ReadInt32();
										if (num5 != presetEntry.VertexCount)
										{
											ShapeEditorPlugin.Logger.LogWarning(L.PresetInvalidFile);
											return null;
										}
										DeformLayer deformLayer = new DeformLayer(text, num5);
										deformLayer.Weight = num4;
										for (int l = 0; l < num5; l++)
										{
											deformLayer.Deltas[l].x = binaryReader.ReadSingle();
											deformLayer.Deltas[l].y = binaryReader.ReadSingle();
											deformLayer.Deltas[l].z = binaryReader.ReadSingle();
										}
										string text2 = binaryReader.ReadString();
										if (!string.IsNullOrEmpty(text2))
										{
											deformLayer.Id = text2;
										}
										presetEntry.Layers.Add(deformLayer);
									}
									if (num >= 3)
									{
										int num6 = binaryReader.ReadInt32();
										if (num6 < 0 || num6 > 10000000)
										{
											ShapeEditorPlugin.Logger.LogWarning(L.PresetInvalidFile);
											return null;
										}
										if (num6 > 0)
										{
											presetEntry.DeletedFaces = new int[num6];
											for (int m = 0; m < num6; m++)
											{
												presetEntry.DeletedFaces[m] = binaryReader.ReadInt32();
											}
										}
									}
									if (num >= 4)
									{
										List<PsdDriver> list = PresetSerializer.ReadPsdDrivers(binaryReader);
										List<CsbDriver> list2 = ((list != null) ? PresetSerializer.ReadCsbDrivers(binaryReader) : null);
										if (list == null || list2 == null)
										{
											ShapeEditorPlugin.Logger.LogWarning(L.PresetInvalidFile);
											return null;
										}
										presetEntry.PsdDrivers = list;
										presetEntry.CsbDrivers = list2;
									}
									if (num >= 6)
									{
										List<NbbDriver> list3 = PresetSerializer.ReadNbbDrivers(binaryReader);
										if (list3 == null)
										{
											ShapeEditorPlugin.Logger.LogWarning(L.PresetInvalidFile);
											return null;
										}
										presetEntry.NbbDrivers = list3;
									}
									presetBundle2.Entries.Add(presetEntry);
								}
								presetBundle = presetBundle2;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("PresetSerializer.DeserializeBundle failed: " + ex.Message);
				presetBundle = null;
			}
			return presetBundle;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000082B0 File Offset: 0x000064B0
		public static PresetEntry BuildEntryFromRenderer(Renderer renderer, DeformData data, int subdivLevel, string rendererPath)
		{
			if (renderer == null)
			{
				return null;
			}
			Mesh mesh = MeshHelper.GetMesh(renderer);
			if (mesh == null)
			{
				return null;
			}
			PresetEntry presetEntry = new PresetEntry();
			presetEntry.RendererPath = rendererPath ?? "";
			presetEntry.SubdivLevel = subdivLevel;
			presetEntry.VertexCount = mesh.vertexCount;
			presetEntry.FacesPerLevel = new List<int[]>(subdivLevel);
			List<int[]> subdivisionFaces = MeshHelper.GetSubdivisionFaces(renderer);
			for (int i = 0; i < subdivLevel; i++)
			{
				if (subdivisionFaces != null && i < subdivisionFaces.Count && subdivisionFaces[i] != null)
				{
					int[] array = subdivisionFaces[i];
					int[] array2 = new int[array.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array2[j] = array[j];
					}
					presetEntry.FacesPerLevel.Add(array2);
				}
				else
				{
					presetEntry.FacesPerLevel.Add(null);
				}
			}
			presetEntry.SmoothPerLevel = new bool[subdivLevel];
			List<bool> subdivisionSmooth = MeshHelper.GetSubdivisionSmooth(renderer);
			for (int k = 0; k < subdivLevel; k++)
			{
				presetEntry.SmoothPerLevel[k] = MeshHelper.SmoothAt(subdivisionSmooth, k);
			}
			if (data != null && data.Layers != null)
			{
				for (int l = 0; l < data.Layers.Count; l++)
				{
					DeformLayer deformLayer = data.Layers[l];
					DeformLayer deformLayer2 = new DeformLayer(deformLayer.Name, (deformLayer.Deltas != null) ? deformLayer.Deltas.Length : 0);
					deformLayer2.Weight = deformLayer.Weight;
					if (deformLayer.Id != null)
					{
						deformLayer2.Id = deformLayer.Id;
					}
					if (deformLayer.Deltas != null)
					{
						for (int m = 0; m < deformLayer.Deltas.Length; m++)
						{
							deformLayer2.Deltas[m] = deformLayer.Deltas[m];
						}
					}
					presetEntry.Layers.Add(deformLayer2);
				}
			}
			if (data != null && data.DeletedFaces.Count > 0)
			{
				presetEntry.DeletedFaces = new int[data.DeletedFaces.Count];
				data.DeletedFaces.CopyTo(presetEntry.DeletedFaces);
				Array.Sort<int>(presetEntry.DeletedFaces);
			}
			return presetEntry;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000084D4 File Offset: 0x000066D4
		public static void ApplyEntryToRenderer(PresetEntry entry, DeformData targetData, bool preserveExisting)
		{
			if (entry == null || targetData == null)
			{
				return;
			}
			if (preserveExisting)
			{
				PresetSerializer.AppendEntryLayers(entry, targetData);
				return;
			}
			targetData.Layers.Clear();
			if (entry.Layers != null)
			{
				for (int i = 0; i < entry.Layers.Count; i++)
				{
					DeformLayer deformLayer = entry.Layers[i];
					if (deformLayer != null)
					{
						DeformLayer deformLayer2 = new DeformLayer(deformLayer.Name, (deformLayer.Deltas != null) ? deformLayer.Deltas.Length : 0);
						deformLayer2.Weight = deformLayer.Weight;
						if (!string.IsNullOrEmpty(deformLayer.Id))
						{
							deformLayer2.Id = deformLayer.Id;
						}
						if (deformLayer.Deltas != null)
						{
							for (int j = 0; j < deformLayer.Deltas.Length; j++)
							{
								deformLayer2.Deltas[j] = deformLayer.Deltas[j];
							}
						}
						deformLayer2.Dirty = true;
						targetData.Layers.Add(deformLayer2);
					}
				}
			}
			targetData.ActiveLayerIndex = ((targetData.Layers.Count > 0) ? (targetData.Layers.Count - 1) : (-1));
			targetData.DeletedFaces.Clear();
			if (entry.DeletedFaces != null && entry.DeletedFaces.Length != 0)
			{
				for (int k = 0; k < entry.DeletedFaces.Length; k++)
				{
					targetData.DeletedFaces.Add(entry.DeletedFaces[k]);
				}
			}
			targetData.DeletedFacesDirty = true;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00008638 File Offset: 0x00006838
		private static void AppendEntryLayers(PresetEntry entry, DeformData targetData)
		{
			if (entry.Layers == null)
			{
				return;
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (DeformLayer deformLayer in targetData.Layers)
			{
				if (!string.IsNullOrEmpty(deformLayer.Id))
				{
					hashSet.Add(deformLayer.Id);
				}
			}
			int count = targetData.Layers.Count;
			for (int i = 0; i < entry.Layers.Count; i++)
			{
				DeformLayer deformLayer2 = entry.Layers[i];
				if (deformLayer2 != null)
				{
					DeformLayer deformLayer3 = new DeformLayer(deformLayer2.Name, (deformLayer2.Deltas != null) ? deformLayer2.Deltas.Length : 0);
					deformLayer3.Weight = deformLayer2.Weight;
					if (!string.IsNullOrEmpty(deformLayer2.Id) && !hashSet.Contains(deformLayer2.Id))
					{
						deformLayer3.Id = deformLayer2.Id;
					}
					hashSet.Add(deformLayer3.Id);
					if (deformLayer2.Deltas != null)
					{
						for (int j = 0; j < deformLayer2.Deltas.Length; j++)
						{
							deformLayer3.Deltas[j] = deformLayer2.Deltas[j];
						}
					}
					deformLayer3.Dirty = true;
					targetData.Layers.Add(deformLayer3);
				}
			}
			if (targetData.Layers.Count > count)
			{
				targetData.ActiveLayerIndex = targetData.Layers.Count - 1;
			}
		}

		// Token: 0x0400004E RID: 78
		private const byte Magic0 = 75;

		// Token: 0x0400004F RID: 79
		private const byte Magic1 = 80;

		// Token: 0x04000050 RID: 80
		public const int FormatVersion = 6;

		// Token: 0x04000051 RID: 81
		private const int MaxDriverCount = 100000;
	}
}
