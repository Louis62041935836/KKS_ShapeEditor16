using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000021 RID: 33
	public class SpatialHashGrid
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x0001050C File Offset: 0x0000E70C
		public SpatialHashGrid(Vector3[] vertices, Bounds bounds)
		{
			this.Rebuild(vertices, bounds);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0001051C File Offset: 0x0000E71C
		public SpatialHashGrid(Vector3[] vertices)
		{
			this.Rebuild(vertices, SpatialHashGrid.ComputeBounds(vertices));
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00010534 File Offset: 0x0000E734
		private static Bounds ComputeBounds(Vector3[] vertices)
		{
			if (vertices == null || vertices.Length == 0)
			{
				return new Bounds(Vector3.zero, Vector3.zero);
			}
			Bounds bounds = new Bounds(vertices[0], Vector3.zero);
			for (int i = 1; i < vertices.Length; i++)
			{
				bounds.Encapsulate(vertices[i]);
			}
			return bounds;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00010588 File Offset: 0x0000E788
		public void Rebuild(Vector3[] vertices, Bounds bounds)
		{
			this._vertices = vertices;
			if (vertices == null || vertices.Length == 0)
			{
				this._grid = null;
				return;
			}
			int num = vertices.Length;
			float num2 = Mathf.Max(bounds.size.magnitude / Mathf.Sqrt((float)num), 0.0001f);
			this._invCell = 1f / num2;
			if (this._grid != null)
			{
				foreach (List<int> list in this._grid.Values)
				{
					list.Clear();
				}
				this._grid.Clear();
			}
			else
			{
				this._grid = new Dictionary<long, List<int>>();
			}
			for (int i = 0; i < num; i++)
			{
				long num3 = SpatialHashGrid.HashKey(vertices[i], this._invCell);
				List<int> list2;
				if (!this._grid.TryGetValue(num3, out list2))
				{
					list2 = new List<int>();
					this._grid[num3] = list2;
				}
				list2.Add(i);
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0001069C File Offset: 0x0000E89C
		public void FindVerticesInRadius(Vector3 queryPos, float radius, Action<int, float> callback)
		{
			if (this._grid == null || this._vertices == null)
			{
				return;
			}
			float num = radius * radius;
			int num2 = Mathf.CeilToInt(radius * this._invCell);
			long num3 = 2L * (long)num2 + 1L;
			num3 = num3 * num3 * num3;
			if (num3 > (long)this._vertices.Length * 4L)
			{
				int num4 = this._vertices.Length;
				for (int i = 0; i < num4; i++)
				{
					float sqrMagnitude = (this._vertices[i] - queryPos).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						callback(i, sqrMagnitude);
					}
				}
				return;
			}
			int num5 = Mathf.FloorToInt(queryPos.x * this._invCell);
			int num6 = Mathf.FloorToInt(queryPos.y * this._invCell);
			int num7 = Mathf.FloorToInt(queryPos.z * this._invCell);
			for (int j = -num2; j <= num2; j++)
			{
				for (int k = -num2; k <= num2; k++)
				{
					for (int l = -num2; l <= num2; l++)
					{
						long num8 = ((long)(num5 + j) * 73856093L) ^ ((long)(num6 + k) * 19349663L) ^ ((long)(num7 + l) * 83492791L);
						List<int> list;
						if (this._grid.TryGetValue(num8, out list))
						{
							for (int m = 0; m < list.Count; m++)
							{
								int num9 = list[m];
								float sqrMagnitude2 = (this._vertices[num9] - queryPos).sqrMagnitude;
								if (sqrMagnitude2 <= num)
								{
									callback(num9, sqrMagnitude2);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0001083C File Offset: 0x0000EA3C
		public int FindNearest(Vector3 queryPos)
		{
			if (this._grid == null || this._vertices == null)
			{
				return -1;
			}
			int num = -1;
			float num2 = float.MaxValue;
			int num3 = Mathf.FloorToInt(queryPos.x * this._invCell);
			int num4 = Mathf.FloorToInt(queryPos.y * this._invCell);
			int num5 = Mathf.FloorToInt(queryPos.z * this._invCell);
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					for (int k = -1; k <= 1; k++)
					{
						long num6 = ((long)(num3 + i) * 73856093L) ^ ((long)(num4 + j) * 19349663L) ^ ((long)(num5 + k) * 83492791L);
						List<int> list;
						if (this._grid.TryGetValue(num6, out list))
						{
							for (int l = 0; l < list.Count; l++)
							{
								int num7 = list[l];
								float sqrMagnitude = (this._vertices[num7] - queryPos).sqrMagnitude;
								if (sqrMagnitude < num2)
								{
									num2 = sqrMagnitude;
									num = num7;
								}
							}
						}
					}
				}
			}
			return num;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00010964 File Offset: 0x0000EB64
		public void FindVerticesInBounds(Vector3 min, Vector3 max, Action<int> callback)
		{
			if (this._grid == null || this._vertices == null)
			{
				return;
			}
			int num = Mathf.FloorToInt(min.x * this._invCell);
			int num2 = Mathf.FloorToInt(min.y * this._invCell);
			int num3 = Mathf.FloorToInt(min.z * this._invCell);
			int num4 = Mathf.FloorToInt(max.x * this._invCell);
			int num5 = Mathf.FloorToInt(max.y * this._invCell);
			int num6 = Mathf.FloorToInt(max.z * this._invCell);
			for (int i = num; i <= num4; i++)
			{
				for (int j = num2; j <= num5; j++)
				{
					for (int k = num3; k <= num6; k++)
					{
						long num7 = ((long)i * 73856093L) ^ ((long)j * 19349663L) ^ ((long)k * 83492791L);
						List<int> list;
						if (this._grid.TryGetValue(num7, out list))
						{
							for (int l = 0; l < list.Count; l++)
							{
								callback(list[l]);
							}
						}
					}
				}
			}
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00010A80 File Offset: 0x0000EC80
		private static long HashKey(Vector3 pos, float invCell)
		{
			long num = (long)Mathf.FloorToInt(pos.x * invCell);
			int num2 = Mathf.FloorToInt(pos.y * invCell);
			int num3 = Mathf.FloorToInt(pos.z * invCell);
			return (num * 73856093L) ^ ((long)num2 * 19349663L) ^ ((long)num3 * 83492791L);
		}

		// Token: 0x040000D7 RID: 215
		private Dictionary<long, List<int>> _grid;

		// Token: 0x040000D8 RID: 216
		private Vector3[] _vertices;

		// Token: 0x040000D9 RID: 217
		private float _invCell;
	}
}
