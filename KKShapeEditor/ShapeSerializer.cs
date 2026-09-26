using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace KKShapeEditor
{
	// Token: 0x02000020 RID: 32
	public static class ShapeSerializer
	{
		// Token: 0x060001B0 RID: 432 RVA: 0x0000E99C File Offset: 0x0000CB9C
		public static byte[] SerializeAllLayers(Dictionary<string, DeformData> dataMap)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ShapeSerializer.WriteHeader(binaryWriter);
					binaryWriter.Write(dataMap.Count);
					foreach (KeyValuePair<string, DeformData> keyValuePair in dataMap)
					{
						binaryWriter.Write(keyValuePair.Key ?? "");
						ShapeSerializer.WriteDeformData(binaryWriter, keyValuePair.Value);
					}
					array = memoryStream.ToArray();
				}
			}
			return array;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000EA60 File Offset: 0x0000CC60
		public static Dictionary<string, DeformData> DeserializeAllLayers(byte[] data)
		{
			byte b;
			return ShapeSerializer.DeserializeAllLayers(data, out b);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000EA78 File Offset: 0x0000CC78
		public static Dictionary<string, DeformData> DeserializeAllLayers(byte[] data, out byte version)
		{
			version = 0;
			if (data == null || data.Length < 3)
			{
				return null;
			}
			if (!ShapeSerializer.HasMagicHeader(data))
			{
				return null;
			}
			Dictionary<string, DeformData> dictionary2;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.ReadByte();
						binaryReader.ReadByte();
						version = binaryReader.ReadByte();
						int num = binaryReader.ReadInt32();
						Dictionary<string, DeformData> dictionary = new Dictionary<string, DeformData>(num);
						ShapeSerializer.MigrationStats migrationStats = new ShapeSerializer.MigrationStats();
						for (int i = 0; i < num; i++)
						{
							string text = binaryReader.ReadString();
							DeformData deformData = ShapeSerializer.ReadDeformData(binaryReader, version, text, migrationStats);
							if (deformData != null)
							{
								dictionary[text] = deformData;
							}
						}
						ShapeSerializer.EmitMigrationLog(migrationStats, version, "DeserializeAllLayers");
						dictionary2 = dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("ShapeSerializer.DeserializeAllLayers failed: " + ex.Message);
				dictionary2 = null;
			}
			return dictionary2;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000EB80 File Offset: 0x0000CD80
		private static void WriteDeformData(BinaryWriter w, DeformData data)
		{
			w.Write(data.Layers.Count);
			w.Write(data.ActiveLayerIndex);
			foreach (DeformLayer deformLayer in data.Layers)
			{
				w.Write(deformLayer.Name ?? "");
				w.Write(deformLayer.Weight);
				w.Write(deformLayer.Deltas.Length);
				for (int i = 0; i < deformLayer.Deltas.Length; i++)
				{
					w.Write(deformLayer.Deltas[i].x);
					w.Write(deformLayer.Deltas[i].y);
					w.Write(deformLayer.Deltas[i].z);
				}
				w.Write(deformLayer.Id ?? "");
			}
			w.Write(data.WeightRemapped);
			ShapeSerializer.WriteFaceMask(w, data.DeletedFaces);
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000ECA4 File Offset: 0x0000CEA4
		private static void WriteFaceMask(BinaryWriter w, HashSet<int> deleted)
		{
			if (deleted.Count == 0)
			{
				w.Write(0);
				return;
			}
			int[] array = new int[deleted.Count];
			deleted.CopyTo(array);
			Array.Sort<int>(array);
			w.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				w.Write(array[i]);
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000ECFC File Offset: 0x0000CEFC
		private static DeformData ReadDeformData(BinaryReader r, byte version, string path, ShapeSerializer.MigrationStats stats)
		{
			int num = r.ReadInt32();
			int num2 = r.ReadInt32();
			DeformData deformData = new DeformData(path);
			for (int i = 0; i < num; i++)
			{
				string text = r.ReadString();
				float num3 = r.ReadSingle();
				int num4 = r.ReadInt32();
				DeformLayer deformLayer = new DeformLayer(text, num4);
				deformLayer.Weight = num3;
				for (int j = 0; j < num4; j++)
				{
					deformLayer.Deltas[j].x = r.ReadSingle();
					deformLayer.Deltas[j].y = r.ReadSingle();
					deformLayer.Deltas[j].z = r.ReadSingle();
				}
				if (version >= 3)
				{
					string text2 = r.ReadString();
					if (!string.IsNullOrEmpty(text2))
					{
						deformLayer.Id = text2;
					}
				}
				else if (stats != null)
				{
					stats.MigratedLayerCount++;
				}
				deformData.Layers.Add(deformLayer);
			}
			deformData.ActiveLayerIndex = ((num2 < deformData.Layers.Count) ? num2 : (-1));
			if (version >= 2)
			{
				deformData.WeightRemapped = r.ReadBoolean();
			}
			if (version >= 4)
			{
				int num5 = r.ReadInt32();
				if (num5 > 0 && num5 <= 10000000)
				{
					for (int k = 0; k < num5; k++)
					{
						deformData.DeletedFaces.Add(r.ReadInt32());
					}
					deformData.DeletedFacesDirty = true;
				}
				else if (num5 > 0)
				{
					return null;
				}
			}
			return deformData;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000EE68 File Offset: 0x0000D068
		private static void EmitMigrationLog(ShapeSerializer.MigrationStats stats, byte version, string source)
		{
			if (stats.MigratedLayerCount == 0 || version >= 3)
			{
				return;
			}
			ShapeEditorPlugin.Logger.LogInfo(string.Concat(new string[]
			{
				"ShapeSerializer.",
				source,
				": migrated v",
				version.ToString(),
				" card to v3 — assigned new GUIDs to ",
				stats.MigratedLayerCount.ToString(),
				" layer(s)"
			}));
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000EED4 File Offset: 0x0000D0D4
		public static byte[] SerializeSingleRenderer(DeformData data)
		{
			if (data == null || data.Layers.Count == 0)
			{
				return null;
			}
			int num = data.Layers[0].Deltas.Length;
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ShapeSerializer.WriteHeader(binaryWriter);
					binaryWriter.Write(num);
					binaryWriter.Write(data.Layers.Count);
					binaryWriter.Write(data.ActiveLayerIndex);
					foreach (DeformLayer deformLayer in data.Layers)
					{
						binaryWriter.Write(deformLayer.Name ?? "");
						binaryWriter.Write(deformLayer.Weight);
						binaryWriter.Write(deformLayer.Deltas.Length);
						for (int i = 0; i < deformLayer.Deltas.Length; i++)
						{
							binaryWriter.Write(deformLayer.Deltas[i].x);
							binaryWriter.Write(deformLayer.Deltas[i].y);
							binaryWriter.Write(deformLayer.Deltas[i].z);
						}
						binaryWriter.Write(deformLayer.Id ?? "");
					}
					ShapeSerializer.WriteFaceMask(binaryWriter, data.DeletedFaces);
					array = memoryStream.ToArray();
				}
			}
			return array;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000F098 File Offset: 0x0000D298
		public static List<DeformLayer> DeserializeSingleRenderer(byte[] data, out int fileVertexCount)
		{
			int[] array;
			return ShapeSerializer.DeserializeSingleRenderer(data, out fileVertexCount, out array);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000F0B0 File Offset: 0x0000D2B0
		public static List<DeformLayer> DeserializeSingleRenderer(byte[] data, out int fileVertexCount, out int[] deletedFaces)
		{
			fileVertexCount = 0;
			deletedFaces = null;
			if (data == null || data.Length < 3)
			{
				return null;
			}
			if (!ShapeSerializer.HasMagicHeader(data))
			{
				return null;
			}
			List<DeformLayer> list;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.ReadByte();
						binaryReader.ReadByte();
						byte b = binaryReader.ReadByte();
						if (b > 10)
						{
							list = null;
						}
						else
						{
							fileVertexCount = binaryReader.ReadInt32();
							if (fileVertexCount < 0 || fileVertexCount > 10000000)
							{
								list = null;
							}
							else
							{
								int num = binaryReader.ReadInt32();
								if (num < 0 || num > 256)
								{
									list = null;
								}
								else
								{
									binaryReader.ReadInt32();
									List<DeformLayer> list2 = new List<DeformLayer>(num);
									for (int i = 0; i < num; i++)
									{
										string text = binaryReader.ReadString();
										float num2 = binaryReader.ReadSingle();
										int num3 = binaryReader.ReadInt32();
										if (num3 != fileVertexCount)
										{
											return null;
										}
										DeformLayer deformLayer = new DeformLayer(text, num3);
										deformLayer.Weight = num2;
										for (int j = 0; j < num3; j++)
										{
											deformLayer.Deltas[j].x = binaryReader.ReadSingle();
											deformLayer.Deltas[j].y = binaryReader.ReadSingle();
											deformLayer.Deltas[j].z = binaryReader.ReadSingle();
										}
										if (b >= 3)
										{
											string text2 = binaryReader.ReadString();
											if (!string.IsNullOrEmpty(text2))
											{
												deformLayer.Id = text2;
											}
										}
										list2.Add(deformLayer);
									}
									if (b >= 4)
									{
										int num4 = binaryReader.ReadInt32();
										if (num4 > 0 && num4 <= 10000000)
										{
											deletedFaces = new int[num4];
											for (int k = 0; k < num4; k++)
											{
												deletedFaces[k] = binaryReader.ReadInt32();
											}
										}
									}
									list = list2;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("ShapeSerializer.DeserializeSingleRenderer failed: " + ex.Message);
				list = null;
			}
			return list;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000F2F0 File Offset: 0x0000D4F0
		public static byte[] SerializeSubdivisionInfo(Dictionary<string, int> levels, Dictionary<string, List<int[]>> faces, Dictionary<string, bool[]> smooth)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ShapeSerializer.WriteHeader(binaryWriter);
					ShapeSerializer.WriteSubdivisionLevels(binaryWriter, levels);
					ShapeSerializer.WriteSubdivisionFaces(binaryWriter, faces);
					ShapeSerializer.WriteSubdivisionSmooth(binaryWriter, smooth);
					array = memoryStream.ToArray();
				}
			}
			return array;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000F360 File Offset: 0x0000D560
		public static void DeserializeSubdivisionInfo(byte[] data, out Dictionary<string, int> levels, out Dictionary<string, List<int[]>> faces, out Dictionary<string, bool[]> smooth)
		{
			levels = null;
			faces = null;
			smooth = null;
			if (data == null || data.Length < 3)
			{
				return;
			}
			if (!ShapeSerializer.HasMagicHeader(data))
			{
				return;
			}
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.ReadByte();
						binaryReader.ReadByte();
						int num = (int)binaryReader.ReadByte();
						levels = ShapeSerializer.ReadSubdivisionLevels(binaryReader);
						faces = ShapeSerializer.ReadSubdivisionFaces(binaryReader);
						if (num >= 9)
						{
							smooth = ShapeSerializer.ReadSubdivisionSmooth(binaryReader);
						}
					}
				}
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("ShapeSerializer.DeserializeSubdivisionInfo failed: " + ex.Message);
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000F428 File Offset: 0x0000D628
		private static void WriteSubdivisionLevels(BinaryWriter w, Dictionary<string, int> levels)
		{
			if (levels == null)
			{
				w.Write(0);
				return;
			}
			w.Write(levels.Count);
			foreach (KeyValuePair<string, int> keyValuePair in levels)
			{
				w.Write(keyValuePair.Key ?? "");
				w.Write(keyValuePair.Value);
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000F4AC File Offset: 0x0000D6AC
		private static Dictionary<string, int> ReadSubdivisionLevels(BinaryReader r)
		{
			int num = r.ReadInt32();
			Dictionary<string, int> dictionary = new Dictionary<string, int>(num);
			for (int i = 0; i < num; i++)
			{
				string text = r.ReadString();
				int num2 = r.ReadInt32();
				dictionary[text] = num2;
			}
			return dictionary;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000F4EC File Offset: 0x0000D6EC
		private static void WriteSubdivisionFaces(BinaryWriter w, Dictionary<string, List<int[]>> faces)
		{
			if (faces == null)
			{
				w.Write(0);
				return;
			}
			w.Write(faces.Count);
			foreach (KeyValuePair<string, List<int[]>> keyValuePair in faces)
			{
				w.Write(keyValuePair.Key ?? "");
				List<int[]> value = keyValuePair.Value;
				w.Write(value.Count);
				foreach (int[] array in value)
				{
					if (array == null)
					{
						w.Write(-1);
					}
					else
					{
						w.Write(array.Length);
						for (int i = 0; i < array.Length; i++)
						{
							w.Write(array[i]);
						}
					}
				}
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000F5E8 File Offset: 0x0000D7E8
		private static Dictionary<string, List<int[]>> ReadSubdivisionFaces(BinaryReader r)
		{
			int num = r.ReadInt32();
			Dictionary<string, List<int[]>> dictionary = new Dictionary<string, List<int[]>>(num);
			for (int i = 0; i < num; i++)
			{
				string text = r.ReadString();
				int num2 = r.ReadInt32();
				List<int[]> list = new List<int[]>(num2);
				for (int j = 0; j < num2; j++)
				{
					int num3 = r.ReadInt32();
					if (num3 < 0)
					{
						list.Add(null);
					}
					else
					{
						int[] array = new int[num3];
						for (int k = 0; k < num3; k++)
						{
							array[k] = r.ReadInt32();
						}
						list.Add(array);
					}
				}
				dictionary[text] = list;
			}
			return dictionary;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000F688 File Offset: 0x0000D888
		private static void WriteSubdivisionSmooth(BinaryWriter w, Dictionary<string, bool[]> smooth)
		{
			if (smooth == null)
			{
				w.Write(0);
				return;
			}
			w.Write(smooth.Count);
			foreach (KeyValuePair<string, bool[]> keyValuePair in smooth)
			{
				w.Write(keyValuePair.Key ?? "");
				bool[] array = keyValuePair.Value ?? new bool[0];
				w.Write(array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					w.Write(array[i]);
				}
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000F730 File Offset: 0x0000D930
		private static Dictionary<string, bool[]> ReadSubdivisionSmooth(BinaryReader r)
		{
			int num = r.ReadInt32();
			Dictionary<string, bool[]> dictionary = new Dictionary<string, bool[]>(num);
			for (int i = 0; i < num; i++)
			{
				string text = r.ReadString();
				int num2 = r.ReadInt32();
				bool[] array = new bool[num2];
				for (int j = 0; j < num2; j++)
				{
					array[j] = r.ReadBoolean();
				}
				dictionary[text] = array;
			}
			return dictionary;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000F798 File Offset: 0x0000D998
		public static byte[] SerializeItemDict(Dictionary<int, ItemSaveData> dict)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ShapeSerializer.WriteHeader(binaryWriter);
					binaryWriter.Write(dict.Count);
					foreach (KeyValuePair<int, ItemSaveData> keyValuePair in dict)
					{
						binaryWriter.Write(keyValuePair.Key);
						ShapeSerializer.WriteItemSaveData(binaryWriter, keyValuePair.Value);
					}
					array = memoryStream.ToArray();
				}
			}
			return array;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000F850 File Offset: 0x0000DA50
		public static Dictionary<int, ItemSaveData> DeserializeItemDict(byte[] data)
		{
			if (data == null || data.Length < 3)
			{
				return null;
			}
			if (!ShapeSerializer.HasMagicHeader(data))
			{
				return null;
			}
			Dictionary<int, ItemSaveData> dictionary2;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.ReadByte();
						binaryReader.ReadByte();
						byte b = binaryReader.ReadByte();
						int num = binaryReader.ReadInt32();
						Dictionary<int, ItemSaveData> dictionary = new Dictionary<int, ItemSaveData>(num);
						ShapeSerializer.MigrationStats migrationStats = new ShapeSerializer.MigrationStats();
						for (int i = 0; i < num; i++)
						{
							int num2 = binaryReader.ReadInt32();
							dictionary[num2] = ShapeSerializer.ReadItemSaveData(binaryReader, b, migrationStats);
						}
						ShapeSerializer.EmitMigrationLog(migrationStats, b, "DeserializeItemDict");
						dictionary2 = dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("ShapeSerializer.DeserializeItemDict failed: " + ex.Message);
				dictionary2 = null;
			}
			return dictionary2;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000F94C File Offset: 0x0000DB4C
		public static byte[] SerializeMapItemDict(Dictionary<string, ItemSaveData> dict)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ShapeSerializer.WriteHeader(binaryWriter);
					binaryWriter.Write(dict.Count);
					foreach (KeyValuePair<string, ItemSaveData> keyValuePair in dict)
					{
						binaryWriter.Write(keyValuePair.Key ?? "");
						ShapeSerializer.WriteItemSaveData(binaryWriter, keyValuePair.Value);
					}
					array = memoryStream.ToArray();
				}
			}
			return array;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000FA10 File Offset: 0x0000DC10
		public static Dictionary<string, ItemSaveData> DeserializeMapItemDict(byte[] data)
		{
			if (data == null || data.Length < 3)
			{
				return null;
			}
			if (!ShapeSerializer.HasMagicHeader(data))
			{
				return null;
			}
			Dictionary<string, ItemSaveData> dictionary2;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.ReadByte();
						binaryReader.ReadByte();
						byte b = binaryReader.ReadByte();
						int num = binaryReader.ReadInt32();
						Dictionary<string, ItemSaveData> dictionary = new Dictionary<string, ItemSaveData>(num);
						ShapeSerializer.MigrationStats migrationStats = new ShapeSerializer.MigrationStats();
						for (int i = 0; i < num; i++)
						{
							string text = binaryReader.ReadString();
							dictionary[text] = ShapeSerializer.ReadItemSaveData(binaryReader, b, migrationStats);
						}
						ShapeSerializer.EmitMigrationLog(migrationStats, b, "DeserializeMapItemDict");
						dictionary2 = dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("ShapeSerializer.DeserializeMapItemDict failed: " + ex.Message);
				dictionary2 = null;
			}
			return dictionary2;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000FB0C File Offset: 0x0000DD0C
		private static void WriteItemSaveData(BinaryWriter w, ItemSaveData saveData)
		{
			bool flag = saveData.DeformDataMap != null && saveData.DeformDataMap.Count > 0;
			w.Write(flag);
			if (flag)
			{
				w.Write(saveData.DeformDataMap.Count);
				foreach (KeyValuePair<string, DeformData> keyValuePair in saveData.DeformDataMap)
				{
					w.Write(keyValuePair.Key ?? "");
					ShapeSerializer.WriteDeformData(w, keyValuePair.Value);
				}
			}
			ShapeSerializer.WriteSubdivisionLevels(w, saveData.SubdividedMeshes);
			ShapeSerializer.WriteSubdivisionFaces(w, saveData.SubdividedFaces);
			ShapeSerializer.WriteSubdivisionSmooth(w, saveData.SubdividedSmooth);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000FBD4 File Offset: 0x0000DDD4
		private static ItemSaveData ReadItemSaveData(BinaryReader r, byte version, ShapeSerializer.MigrationStats stats)
		{
			ItemSaveData itemSaveData = new ItemSaveData();
			if (r.ReadBoolean())
			{
				int num = r.ReadInt32();
				itemSaveData.DeformDataMap = new Dictionary<string, DeformData>(num);
				for (int i = 0; i < num; i++)
				{
					string text = r.ReadString();
					DeformData deformData = ShapeSerializer.ReadDeformData(r, version, text, stats);
					if (deformData != null)
					{
						itemSaveData.DeformDataMap[text] = deformData;
					}
				}
			}
			itemSaveData.SubdividedMeshes = ShapeSerializer.ReadSubdivisionLevels(r);
			itemSaveData.SubdividedFaces = ShapeSerializer.ReadSubdivisionFaces(r);
			if (version >= 9)
			{
				itemSaveData.SubdividedSmooth = ShapeSerializer.ReadSubdivisionSmooth(r);
			}
			return itemSaveData;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000FC5C File Offset: 0x0000DE5C
		public static byte[] SerializeDrivers(List<PsdDriver> drivers)
		{
			if (drivers == null || drivers.Count == 0)
			{
				return null;
			}
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ShapeSerializer.WriteHeader(binaryWriter);
					binaryWriter.Write(drivers.Count);
					foreach (PsdDriver psdDriver in drivers)
					{
						binaryWriter.Write(psdDriver.SourceBonePath ?? "");
						binaryWriter.Write((int)psdDriver.Channel);
						binaryWriter.Write(psdDriver.InputMin);
						binaryWriter.Write(psdDriver.InputMax);
						binaryWriter.Write(psdDriver.TargetRendererPath ?? "");
						binaryWriter.Write(psdDriver.TargetLayerId ?? "");
						binaryWriter.Write(psdDriver.Enabled);
					}
					array = memoryStream.ToArray();
				}
			}
			return array;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000FD78 File Offset: 0x0000DF78
		public static List<PsdDriver> DeserializeDrivers(byte[] data)
		{
			List<PsdDriver> list = new List<PsdDriver>();
			if (data == null || data.Length < 3)
			{
				return list;
			}
			if (!ShapeSerializer.HasMagicHeader(data))
			{
				return list;
			}
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.ReadByte();
						binaryReader.ReadByte();
						if (binaryReader.ReadByte() > 10)
						{
							return list;
						}
						int num = binaryReader.ReadInt32();
						if (num < 0 || num > 100000)
						{
							return list;
						}
						for (int i = 0; i < num; i++)
						{
							PsdDriver psdDriver = new PsdDriver();
							psdDriver.SourceBonePath = binaryReader.ReadString();
							int num2 = binaryReader.ReadInt32();
							psdDriver.InputMin = binaryReader.ReadSingle();
							psdDriver.InputMax = binaryReader.ReadSingle();
							psdDriver.TargetRendererPath = binaryReader.ReadString();
							psdDriver.TargetLayerId = binaryReader.ReadString();
							psdDriver.Enabled = binaryReader.ReadBoolean();
							if (num2 >= 0 && num2 <= 5)
							{
								psdDriver.Channel = (PsdChannel)num2;
								list.Add(psdDriver);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("ShapeSerializer.DeserializeDrivers failed: " + ex.Message);
			}
			return list;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000FEDC File Offset: 0x0000E0DC
		public static byte[] SerializeCsb(List<CsbDriver> drivers)
		{
			if (drivers == null || drivers.Count == 0)
			{
				return null;
			}
			byte[] array2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ShapeSerializer.WriteHeader(binaryWriter);
					binaryWriter.Write(drivers.Count);
					foreach (CsbDriver csbDriver in drivers)
					{
						binaryWriter.Write(csbDriver.ClothingKind);
						float[] array = csbDriver.StateWeights ?? new float[0];
						binaryWriter.Write(array.Length);
						for (int i = 0; i < array.Length; i++)
						{
							binaryWriter.Write(array[i]);
						}
						binaryWriter.Write(csbDriver.UnequippedWeight);
						binaryWriter.Write(csbDriver.TargetRendererPath ?? "");
						binaryWriter.Write(csbDriver.TargetLayerId ?? "");
						binaryWriter.Write(csbDriver.Enabled);
						binaryWriter.Write(csbDriver.CoordinateScope);
					}
					array2 = memoryStream.ToArray();
				}
			}
			return array2;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00010024 File Offset: 0x0000E224
		public static List<CsbDriver> DeserializeCsb(byte[] data)
		{
			List<CsbDriver> list = new List<CsbDriver>();
			if (data == null || data.Length < 3)
			{
				return list;
			}
			if (!ShapeSerializer.HasMagicHeader(data))
			{
				return list;
			}
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.ReadByte();
						binaryReader.ReadByte();
						byte b = binaryReader.ReadByte();
						if (b > 10)
						{
							return list;
						}
						int num = binaryReader.ReadInt32();
						if (num < 0 || num > 100000)
						{
							return list;
						}
						for (int i = 0; i < num; i++)
						{
							CsbDriver csbDriver = new CsbDriver();
							csbDriver.ClothingKind = binaryReader.ReadInt32();
							int num2 = binaryReader.ReadInt32();
							if (num2 < 0 || num2 > 64)
							{
								break;
							}
							float[] array = new float[num2];
							for (int j = 0; j < num2; j++)
							{
								array[j] = binaryReader.ReadSingle();
							}
							csbDriver.StateWeights = array;
							csbDriver.UnequippedWeight = binaryReader.ReadSingle();
							csbDriver.TargetRendererPath = binaryReader.ReadString();
							csbDriver.TargetLayerId = binaryReader.ReadString();
							csbDriver.Enabled = binaryReader.ReadBoolean();
							if (b >= 8)
							{
								csbDriver.CoordinateScope = binaryReader.ReadInt32();
							}
							csbDriver.NormalizeStateWeights();
							list.Add(csbDriver);
						}
					}
				}
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("ShapeSerializer.DeserializeCsb failed: " + ex.Message);
			}
			return list;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x000101EC File Offset: 0x0000E3EC
		public static byte[] SerializeNbb(List<NbbDriver> drivers)
		{
			if (drivers == null || drivers.Count == 0)
			{
				return null;
			}
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					ShapeSerializer.WriteHeader(binaryWriter);
					binaryWriter.Write(drivers.Count);
					foreach (NbbDriver nbbDriver in drivers)
					{
						binaryWriter.Write(nbbDriver.SourceRendererPath ?? "");
						binaryWriter.Write(nbbDriver.SourceShapeName ?? "");
						binaryWriter.Write(nbbDriver.InputMin);
						binaryWriter.Write(nbbDriver.InputMax);
						binaryWriter.Write(nbbDriver.TargetRendererPath ?? "");
						binaryWriter.Write(nbbDriver.TargetLayerId ?? "");
						binaryWriter.Write(nbbDriver.Enabled);
					}
					array = memoryStream.ToArray();
				}
			}
			return array;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00010318 File Offset: 0x0000E518
		public static List<NbbDriver> DeserializeNbb(byte[] data)
		{
			List<NbbDriver> list = new List<NbbDriver>();
			if (data == null || data.Length < 3)
			{
				return list;
			}
			if (!ShapeSerializer.HasMagicHeader(data))
			{
				return list;
			}
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.ReadByte();
						binaryReader.ReadByte();
						if (binaryReader.ReadByte() > 10)
						{
							return list;
						}
						int num = binaryReader.ReadInt32();
						if (num < 0 || num > 100000)
						{
							return list;
						}
						for (int i = 0; i < num; i++)
						{
							list.Add(new NbbDriver
							{
								SourceRendererPath = binaryReader.ReadString(),
								SourceShapeName = binaryReader.ReadString(),
								InputMin = binaryReader.ReadSingle(),
								InputMax = binaryReader.ReadSingle(),
								TargetRendererPath = binaryReader.ReadString(),
								TargetLayerId = binaryReader.ReadString(),
								Enabled = binaryReader.ReadBoolean()
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("ShapeSerializer.DeserializeNbb failed: " + ex.Message);
			}
			return list;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0001046C File Offset: 0x0000E66C
		private static void WriteHeader(BinaryWriter w)
		{
			w.Write((byte)Magic0);
			w.Write((byte)Magic1);
			w.Write((byte)FormatVersion);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00010486 File Offset: 0x0000E686
		private static bool HasMagicHeader(byte[] data)
		{
			return data.Length >= 2 && data[0] == 75 && data[1] == 83;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0001049E File Offset: 0x0000E69E
		public static string MakeClothingKey(int coordType, string relativePath)
		{
			return coordType.ToString(CultureInfo.InvariantCulture) + "/" + relativePath;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x000104B8 File Offset: 0x0000E6B8
		public static bool TryParseClothingKey(string key, out int coordType, out string relativePath)
		{
			coordType = -1;
			relativePath = key;
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}
			int num = key.IndexOf('/');
			if (num <= 0)
			{
				return false;
			}
			int num2;
			if (!int.TryParse(key.Substring(0, num), NumberStyles.None, CultureInfo.InvariantCulture, out num2))
			{
				return false;
			}
			coordType = num2;
			relativePath = key.Substring(num + 1);
			return true;
		}

		// Token: 0x040000D2 RID: 210
		private const byte Magic0 = 75;

		// Token: 0x040000D3 RID: 211
		private const byte Magic1 = 83;

		// Token: 0x040000D4 RID: 212
		private const byte FormatVersion = 10;

		// Token: 0x040000D5 RID: 213
		private const int MaxDriverCount = 100000;

		// Token: 0x040000D6 RID: 214
		private const int MaxStateWeightsLen = 64;

		// Token: 0x0200005F RID: 95
		private class MigrationStats
		{
			// Token: 0x04000495 RID: 1173
			public int MigratedLayerCount;
		}
	}
}
