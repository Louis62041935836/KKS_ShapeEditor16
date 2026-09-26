using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000039 RID: 57
	public class SmoothTool : IDeformTool
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000273 RID: 627 RVA: 0x000147E4 File Offset: 0x000129E4
		public List<int>[] Adjacency
		{
			get
			{
				return this._adjacency;
			}
		}

		// Token: 0x06000274 RID: 628 RVA: 0x000147EC File Offset: 0x000129EC
		public void BuildAdjacency(int[] triangles, int vertexCount, Vector3[] vertices = null)
		{
			this._adjacency = new List<int>[vertexCount];
			this._colocatedGroups = null;
			for (int i = 0; i < vertexCount; i++)
			{
				this._adjacency[i] = new List<int>(6);
			}
			for (int j = 0; j < triangles.Length; j += 3)
			{
				int num = triangles[j];
				int num2 = triangles[j + 1];
				int num3 = triangles[j + 2];
				this.AddEdge(num, num2);
				this.AddEdge(num2, num3);
				this.AddEdge(num3, num);
			}
			if (vertices != null && vertices.Length == vertexCount)
			{
				this.LinkColocatedVertices(vertices);
			}
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00014870 File Offset: 0x00012A70
		private void LinkColocatedVertices(Vector3[] vertices)
		{
			Dictionary<long, List<int>> dictionary = new Dictionary<long, List<int>>();
			for (int i = 0; i < vertices.Length; i++)
			{
				long num = SmoothTool.QuantizePosition(vertices[i]);
				List<int> list;
				if (!dictionary.TryGetValue(num, out list))
				{
					list = new List<int>(2);
					dictionary[num] = list;
				}
				list.Add(i);
			}
			foreach (KeyValuePair<long, List<int>> keyValuePair in dictionary)
			{
				List<int> value = keyValuePair.Value;
				if (value.Count >= 2)
				{
					for (int j = 0; j < value.Count; j++)
					{
						for (int k = j + 1; k < value.Count; k++)
						{
							this.AddEdge(value[j], value[k]);
						}
					}
					if (this._colocatedGroups == null)
					{
						this._colocatedGroups = new List<int[]>();
					}
					this._colocatedGroups.Add(value.ToArray());
				}
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00014980 File Offset: 0x00012B80
		private static long QuantizePosition(Vector3 v)
		{
			int num = Mathf.RoundToInt(v.x * 10000f);
			int num2 = Mathf.RoundToInt(v.y * 10000f);
			int num3 = Mathf.RoundToInt(v.z * 10000f);
			return ((long)(num & 2097151) << 42) | ((long)(num2 & 2097151) << 21) | (long)(num3 & 2097151);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x000149E1 File Offset: 0x00012BE1
		private void AddEdge(int a, int b)
		{
			if (!this._adjacency[a].Contains(b))
			{
				this._adjacency[a].Add(b);
			}
			if (!this._adjacency[b].Contains(a))
			{
				this._adjacency[b].Add(a);
			}
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00014A20 File Offset: 0x00012C20
		public void Apply(ShapeDeformer deformer, DeformLayer layer, BrushResult brushResult, Vector3[] vertices, Vector3[] normals, Camera camera)
		{
			if (layer == null || brushResult == null || this._adjacency == null)
			{
				return;
			}
			Vector3[] deltas = layer.Deltas;
			Transform transform = ((this.Mode == SmoothTool.SmoothMode.Smooth && deformer != null) ? deformer.DisplayTransform : null);
			bool flag = transform != null && vertices != null && vertices.Length == deltas.Length;
			foreach (KeyValuePair<int, float> keyValuePair in brushResult.AffectedVertices)
			{
				int key = keyValuePair.Key;
				float value = keyValuePair.Value;
				if (key >= 0 && key < deltas.Length)
				{
					List<int> list = this._adjacency[key];
					if (list.Count != 0)
					{
						if (flag)
						{
							Vector3 vector = Vector3.zero;
							int num = 0;
							for (int i = 0; i < list.Count; i++)
							{
								int num2 = list[i];
								if (num2 >= 0 && num2 < deltas.Length)
								{
									vector += transform.TransformPoint(vertices[num2]);
									num++;
								}
							}
							if (num != 0)
							{
								vector /= (float)num;
								Vector3 vector2 = (vector - transform.TransformPoint(vertices[key])) * value;
								Vector3 vector3;
								deformer.WorldDeltaToBindDelta(key, vector2, out vector3);
								deltas[key] += vector3;
							}
						}
						else
						{
							Vector3 vector4 = Vector3.zero;
							for (int j = 0; j < list.Count; j++)
							{
								int num3 = list[j];
								if (num3 >= 0 && num3 < deltas.Length)
								{
									vector4 += deltas[num3];
								}
							}
							vector4 /= (float)list.Count;
							deltas[key] = Vector3.Lerp(deltas[key], vector4, value);
						}
					}
				}
			}
			if (this._colocatedGroups != null)
			{
				Dictionary<int, float> affectedVertices = brushResult.AffectedVertices;
				for (int k = 0; k < this._colocatedGroups.Count; k++)
				{
					int[] array = this._colocatedGroups[k];
					bool flag2 = false;
					for (int l = 0; l < array.Length; l++)
					{
						if (affectedVertices.ContainsKey(array[l]))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						Vector3 vector5 = Vector3.zero;
						int num4 = 0;
						foreach (int num5 in array)
						{
							if (num5 >= 0 && num5 < deltas.Length)
							{
								vector5 += deltas[num5];
								num4++;
							}
						}
						if (num4 != 0)
						{
							Vector3 vector6 = vector5 / (float)num4;
							foreach (int num6 in array)
							{
								if (num6 >= 0 && num6 < deltas.Length)
								{
									deltas[num6] = vector6;
								}
							}
						}
					}
				}
			}
			layer.Dirty = true;
		}

		// Token: 0x04000133 RID: 307
		public SmoothTool.SmoothMode Mode = SmoothTool.SmoothMode.Relax;

		// Token: 0x04000134 RID: 308
		private List<int>[] _adjacency;

		// Token: 0x04000135 RID: 309
		private List<int[]> _colocatedGroups;

		// Token: 0x02000069 RID: 105
		public enum SmoothMode
		{
			// Token: 0x040004C1 RID: 1217
			Smooth,
			// Token: 0x040004C2 RID: 1218
			Relax
		}
	}
}
