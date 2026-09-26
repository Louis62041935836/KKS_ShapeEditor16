using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x0200003E RID: 62
	public class TransformGizmo
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600027A RID: 634 RVA: 0x00014D3B File Offset: 0x00012F3B
		// (set) Token: 0x0600027B RID: 635 RVA: 0x00014D43 File Offset: 0x00012F43
		public GizmoMode Mode { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600027C RID: 636 RVA: 0x00014D4C File Offset: 0x00012F4C
		// (set) Token: 0x0600027D RID: 637 RVA: 0x00014D54 File Offset: 0x00012F54
		public GizmoSpace Space { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600027E RID: 638 RVA: 0x00014D5D File Offset: 0x00012F5D
		// (set) Token: 0x0600027F RID: 639 RVA: 0x00014D65 File Offset: 0x00012F65
		public bool SoftSelectionEnabled { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000280 RID: 640 RVA: 0x00014D6E File Offset: 0x00012F6E
		// (set) Token: 0x06000281 RID: 641 RVA: 0x00014D76 File Offset: 0x00012F76
		public float SoftSelectionRadius { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000282 RID: 642 RVA: 0x00014D7F File Offset: 0x00012F7F
		// (set) Token: 0x06000283 RID: 643 RVA: 0x00014D87 File Offset: 0x00012F87
		public FalloffMode SoftFalloff { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000284 RID: 644 RVA: 0x00014D90 File Offset: 0x00012F90
		// (set) Token: 0x06000285 RID: 645 RVA: 0x00014D98 File Offset: 0x00012F98
		public float SharpExponent { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000286 RID: 646 RVA: 0x00014DA1 File Offset: 0x00012FA1
		// (set) Token: 0x06000287 RID: 647 RVA: 0x00014DA9 File Offset: 0x00012FA9
		public SoftSelectMode SoftMode { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000288 RID: 648 RVA: 0x00014DB2 File Offset: 0x00012FB2
		// (set) Token: 0x06000289 RID: 649 RVA: 0x00014DBA File Offset: 0x00012FBA
		public bool SymmetryEnabled { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600028A RID: 650 RVA: 0x00014DC3 File Offset: 0x00012FC3
		// (set) Token: 0x0600028B RID: 651 RVA: 0x00014DCB File Offset: 0x00012FCB
		public int SymmetryAxis { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600028C RID: 652 RVA: 0x00014DD4 File Offset: 0x00012FD4
		// (set) Token: 0x0600028D RID: 653 RVA: 0x00014DDC File Offset: 0x00012FDC
		public float SymmetryCenter { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600028E RID: 654 RVA: 0x00014DE5 File Offset: 0x00012FE5
		// (set) Token: 0x0600028F RID: 655 RVA: 0x00014DED File Offset: 0x00012FED
		public MultiDeltaUndoEntry PendingDragEntry { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000290 RID: 656 RVA: 0x00014DF6 File Offset: 0x00012FF6
		// (set) Token: 0x06000291 RID: 657 RVA: 0x00014DFE File Offset: 0x00012FFE
		public Func<int, DeformLayer> LazyEnsureLayerCallback { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000292 RID: 658 RVA: 0x00014E07 File Offset: 0x00013007
		public GizmoAxis HoveredAxis
		{
			get
			{
				return this._hoveredAxis;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000293 RID: 659 RVA: 0x00014E0F File Offset: 0x0001300F
		public bool IsDragging
		{
			get
			{
				return this._isDragging;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000294 RID: 660 RVA: 0x00014E18 File Offset: 0x00013018
		public bool HasTarget
		{
			get
			{
				for (int i = 0; i < this._targetIndices.Count; i++)
				{
					if (this._targetIndices[i] != null && this._targetIndices[i].Count > 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000295 RID: 661 RVA: 0x00014E60 File Offset: 0x00013060
		public bool HasMirrorTarget
		{
			get
			{
				for (int i = 0; i < this._mirrorIndices.Count; i++)
				{
					if (this._mirrorIndices[i] != null && this._mirrorIndices[i].Count > 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000296 RID: 662 RVA: 0x00014EA8 File Offset: 0x000130A8
		public Vector3 CentroidWorld
		{
			get
			{
				return this._centroidWorld;
			}
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00014EB0 File Offset: 0x000130B0
		public IDictionary<int, float> GetCombinedSoftWeights(int r)
		{
			if (r < 0 || r >= this._combinedSoftWeights.Count)
			{
				return null;
			}
			return this._combinedSoftWeights[r];
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00014ED2 File Offset: 0x000130D2
		public IDictionary<int, Vector3> GetDragStartDeltas(int r)
		{
			if (r < 0 || r >= this._dragStartDeltas.Count)
			{
				return null;
			}
			return this._dragStartDeltas[r];
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000299 RID: 665 RVA: 0x00014EF4 File Offset: 0x000130F4
		public int RendererCount
		{
			get
			{
				return this._targetIndices.Count;
			}
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00014F04 File Offset: 0x00013104
		public TransformGizmo()
		{
			this.Mode = GizmoMode.Translate;
			this.Space = GizmoSpace.World;
			this.SoftSelectionRadius = 0.1f;
			this.SoftFalloff = FalloffMode.Smooth;
			this.SharpExponent = 3f;
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00015005 File Offset: 0x00013205
		public void SetObjectRoot(Transform root)
		{
			this._objectRoot = root;
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600029C RID: 668 RVA: 0x0001500E File Offset: 0x0001320E
		private Transform PrimaryXform
		{
			get
			{
				if (this._primaryIdx < 0 || this._primaryIdx >= this._xforms.Count)
				{
					return null;
				}
				return this._xforms[this._primaryIdx];
			}
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00015040 File Offset: 0x00013240
		public void SetTarget(List<HashSet<int>> selectionsPerR, List<Vector3[]> verticesPerR, List<Vector3[]> normalsPerR, List<Transform> xformsPerR, List<ShapeDeformer> deformersPerR, List<DeformLayer> layersPerR, List<SpatialHashGrid> gridsPerR, List<List<int>[]> adjacenciesPerR, int primaryIdx)
		{
			this.ClearTarget();
			int num = ((selectionsPerR != null) ? selectionsPerR.Count : 0);
			for (int i = 0; i < num; i++)
			{
				this._targetIndices.Add(selectionsPerR[i] ?? new HashSet<int>());
				this._vertices.Add((verticesPerR != null && i < verticesPerR.Count) ? verticesPerR[i] : null);
				this._normals.Add((normalsPerR != null && i < normalsPerR.Count) ? normalsPerR[i] : null);
				this._xforms.Add((xformsPerR != null && i < xformsPerR.Count) ? xformsPerR[i] : null);
				this._deformers.Add((deformersPerR != null && i < deformersPerR.Count) ? deformersPerR[i] : null);
				this._layers.Add((layersPerR != null && i < layersPerR.Count) ? layersPerR[i] : null);
				this._grids.Add((gridsPerR != null && i < gridsPerR.Count) ? gridsPerR[i] : null);
				this._adjacencies.Add((adjacenciesPerR != null && i < adjacenciesPerR.Count) ? adjacenciesPerR[i] : null);
				Dictionary<int, float> dictionary = new Dictionary<int, float>();
				if (this._targetIndices[i] != null)
				{
					foreach (int num2 in this._targetIndices[i])
					{
						dictionary[num2] = 1f;
					}
				}
				this._softWeights.Add(dictionary);
				this._mirrorIndices.Add(new HashSet<int>());
				this._mirrorWeights.Add(new Dictionary<int, float>());
				this._combinedSoftWeights.Add(new Dictionary<int, float>(dictionary));
				this._dragStartDeltas.Add(new Dictionary<int, Vector3>());
				this._dragStartPositionsWorld.Add(new Dictionary<int, Vector3>());
			}
			this._primaryIdx = ((num > 0 && primaryIdx >= 0 && primaryIdx < num) ? primaryIdx : ((num > 0) ? 0 : (-1)));
			this.ComputeUnionCentroidWorld();
			this.ComputeLocalOrientationFromPrimary();
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00015274 File Offset: 0x00013474
		public void ClearTarget()
		{
			this._targetIndices.Clear();
			this._softWeights.Clear();
			this._mirrorIndices.Clear();
			this._mirrorWeights.Clear();
			this._combinedSoftWeights.Clear();
			this._vertices.Clear();
			this._normals.Clear();
			this._xforms.Clear();
			this._deformers.Clear();
			this._layers.Clear();
			this._grids.Clear();
			this._adjacencies.Clear();
			this._dragStartDeltas.Clear();
			this._dragStartPositionsWorld.Clear();
			this._primaryIdx = -1;
			this._hoveredAxis = GizmoAxis.None;
			this._isDragging = false;
			this._activeAxis = GizmoAxis.None;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00015338 File Offset: 0x00013538
		public void RefreshFrameVertices(List<Vector3[]> verticesPerR)
		{
			if (verticesPerR == null)
			{
				return;
			}
			int num = Mathf.Min(this._vertices.Count, verticesPerR.Count);
			for (int i = 0; i < num; i++)
			{
				this._vertices[i] = verticesPerR[i];
			}
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00015380 File Offset: 0x00013580
		public void SetMirrorTarget(List<HashSet<int>> mirrorIndicesPerR)
		{
			int count = this._mirrorIndices.Count;
			for (int i = 0; i < count; i++)
			{
				this._mirrorIndices[i] = ((mirrorIndicesPerR != null && i < mirrorIndicesPerR.Count && mirrorIndicesPerR[i] != null) ? mirrorIndicesPerR[i] : new HashSet<int>());
				this._mirrorWeights[i].Clear();
				foreach (int num in this._mirrorIndices[i])
				{
					this._mirrorWeights[i][num] = 1f;
				}
			}
			this.RebuildCombinedWeights();
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00015450 File Offset: 0x00013650
		public void ClearMirrorTarget()
		{
			for (int i = 0; i < this._mirrorIndices.Count; i++)
			{
				if (this._mirrorIndices[i] != null)
				{
					this._mirrorIndices[i].Clear();
				}
				this._mirrorWeights[i].Clear();
			}
			this.RebuildCombinedWeights();
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x000154A9 File Offset: 0x000136A9
		public void UpdateCentroid()
		{
			this.ComputeUnionCentroidWorld();
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x000154B4 File Offset: 0x000136B4
		private void ComputeUnionCentroidWorld()
		{
			Vector3 vector = Vector3.zero;
			int num = 0;
			for (int i = 0; i < this._targetIndices.Count; i++)
			{
				HashSet<int> hashSet = this._targetIndices[i];
				Vector3[] array = this._vertices[i];
				Transform transform = this._xforms[i];
				if (hashSet != null && array != null)
				{
					bool flag = transform != null;
					Matrix4x4 matrix4x = (flag ? transform.localToWorldMatrix : Matrix4x4.identity);
					foreach (int num2 in hashSet)
					{
						if (num2 >= 0 && num2 < array.Length)
						{
							vector += (flag ? matrix4x.MultiplyPoint3x4(array[num2]) : array[num2]);
							num++;
						}
					}
				}
			}
			this._centroidWorld = ((num > 0) ? (vector / (float)num) : Vector3.zero);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x000155C8 File Offset: 0x000137C8
		private void ComputeLocalOrientationFromPrimary()
		{
			if (this._primaryIdx < 0 || this._primaryIdx >= this._normals.Count)
			{
				this._localOrientation = Quaternion.identity;
				return;
			}
			HashSet<int> hashSet = this._targetIndices[this._primaryIdx];
			Vector3[] array = this._normals[this._primaryIdx];
			if (hashSet == null || array == null)
			{
				this._localOrientation = Quaternion.identity;
				return;
			}
			Vector3 vector = Vector3.zero;
			foreach (int num in hashSet)
			{
				if (num >= 0 && num < array.Length)
				{
					vector += array[num];
				}
			}
			if (vector.sqrMagnitude < 0.001f)
			{
				this._localOrientation = Quaternion.identity;
				return;
			}
			vector.Normalize();
			Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
			if (vector2.sqrMagnitude < 0.001f)
			{
				vector2 = Vector3.Cross(Vector3.right, vector);
			}
			vector2.Normalize();
			Vector3 normalized = Vector3.Cross(vector, vector2).normalized;
			this._localOrientation = Quaternion.LookRotation(normalized, vector);
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00015700 File Offset: 0x00013900
		private void RebuildCombinedWeights()
		{
			int count = this._softWeights.Count;
			for (int i = 0; i < count; i++)
			{
				Dictionary<int, float> dictionary = this._combinedSoftWeights[i];
				dictionary.Clear();
				foreach (KeyValuePair<int, float> keyValuePair in this._softWeights[i])
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
				foreach (KeyValuePair<int, float> keyValuePair2 in this._mirrorWeights[i])
				{
					float num;
					if (!dictionary.TryGetValue(keyValuePair2.Key, out num) || keyValuePair2.Value > num)
					{
						dictionary[keyValuePair2.Key] = keyValuePair2.Value;
					}
				}
			}
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0001580C File Offset: 0x00013A0C
		public void ComputeSoftWeights()
		{
			this.ComputeSoftWeightsForSet(this._targetIndices, this._softWeights);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00015820 File Offset: 0x00013A20
		public void ComputeMirrorSoftWeights()
		{
			this.ComputeSoftWeightsForSet(this._mirrorIndices, this._mirrorWeights);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00015834 File Offset: 0x00013A34
		private void ComputeSoftWeightsForSet(List<HashSet<int>> sourcesPerR, List<Dictionary<int, float>> outWeightsPerR)
		{
			try
			{
				int count = sourcesPerR.Count;
				for (int i = 0; i < count; i++)
				{
					Dictionary<int, float> dictionary = outWeightsPerR[i];
					dictionary.Clear();
					if (sourcesPerR[i] != null)
					{
						foreach (int num in sourcesPerR[i])
						{
							dictionary[num] = 1f;
						}
					}
				}
				if (!this.SoftSelectionEnabled || this.SoftSelectionRadius <= 0f)
				{
					this.RebuildCombinedWeights();
				}
				else
				{
					this._srcWorldBuf.Clear();
					for (int j = 0; j < sourcesPerR.Count; j++)
					{
						HashSet<int> hashSet = sourcesPerR[j];
						Vector3[] array = this._vertices[j];
						Transform transform = this._xforms[j];
						if (hashSet != null && array != null)
						{
							foreach (int num2 in hashSet)
							{
								if (num2 >= 0 && num2 < array.Length)
								{
									this._srcWorldBuf.Add((transform != null) ? transform.TransformPoint(array[num2]) : array[num2]);
								}
							}
						}
					}
					if (this._srcWorldBuf.Count == 0)
					{
						this.RebuildCombinedWeights();
					}
					else
					{
						for (int k = 0; k < count; k++)
						{
							Vector3[] array2 = this._vertices[k];
							if (array2 != null)
							{
								if (this.SoftMode == SoftSelectMode.Surface && this._adjacencies[k] != null)
								{
									this.ComputeFalloffSurface(k, array2, this._adjacencies[k], sourcesPerR[k], outWeightsPerR[k]);
								}
								else if (this._grids[k] != null)
								{
									this.ComputeFalloffVolume(k, array2, this._grids[k], sourcesPerR[k], outWeightsPerR[k], this._srcWorldBuf);
								}
							}
						}
						this.RebuildCombinedWeights();
					}
				}
			}
			finally
			{
			}
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00015A9C File Offset: 0x00013C9C
		private void ComputeFalloffVolume(int r, Vector3[] vertices, SpatialHashGrid grid, HashSet<int> sources, Dictionary<int, float> outWeights, List<Vector3> srcWorld)
		{
			float radius = this.SoftSelectionRadius;
			float radiusSq = radius * radius;
			Transform xform = this._xforms[r];
			Vector3 vector;
			vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2;
			vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			for (int i = 0; i < srcWorld.Count; i++)
			{
				Vector3 vector3 = ((xform != null) ? xform.InverseTransformPoint(srcWorld[i]) : srcWorld[i]);
				if (vector3.x < vector.x)
				{
					vector.x = vector3.x;
				}
				if (vector3.y < vector.y)
				{
					vector.y = vector3.y;
				}
				if (vector3.z < vector.z)
				{
					vector.z = vector3.z;
				}
				if (vector3.x > vector2.x)
				{
					vector2.x = vector3.x;
				}
				if (vector3.y > vector2.y)
				{
					vector2.y = vector3.y;
				}
				if (vector3.z > vector2.z)
				{
					vector2.z = vector3.z;
				}
			}
			Vector3 vector4 = ((xform != null) ? xform.lossyScale : Vector3.one);
			float num = (Mathf.Approximately(vector4.x, 0f) ? radius : (radius / Mathf.Abs(vector4.x)));
			float num2 = (Mathf.Approximately(vector4.y, 0f) ? radius : (radius / Mathf.Abs(vector4.y)));
			float num3 = (Mathf.Approximately(vector4.z, 0f) ? radius : (radius / Mathf.Abs(vector4.z)));
			vector.x -= num;
			vector.y -= num2;
			vector.z -= num3;
			vector2.x += num;
			vector2.y += num2;
			vector2.z += num3;
			grid.FindVerticesInBounds(vector, vector2, delegate(int idx)
			{
				if (sources != null && sources.Contains(idx))
				{
					return;
				}
				if (idx < 0 || idx >= vertices.Length)
				{
					return;
				}
				Vector3 vector5 = ((xform != null) ? xform.TransformPoint(vertices[idx]) : vertices[idx]);
				float num4 = float.MaxValue;
				for (int j = 0; j < srcWorld.Count; j++)
				{
					float sqrMagnitude = (vector5 - srcWorld[j]).sqrMagnitude;
					if (sqrMagnitude < num4)
					{
						num4 = sqrMagnitude;
					}
				}
				if (num4 > radiusSq)
				{
					return;
				}
				float num5 = this.CalculateFalloff(Mathf.Sqrt(num4) / radius);
				if (num5 <= 0.001f)
				{
					return;
				}
				float num6;
				if (!outWeights.TryGetValue(idx, out num6) || num5 > num6)
				{
					outWeights[idx] = num5;
				}
			});
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00015D44 File Offset: 0x00013F44
		private void ComputeFalloffSurface(int r, Vector3[] vertices, List<int>[] adjacency, HashSet<int> sources, Dictionary<int, float> outWeights)
		{
			float softSelectionRadius = this.SoftSelectionRadius;
			Transform transform = this._xforms[r];
			Vector3[] array = new Vector3[vertices.Length];
			if (transform != null)
			{
				Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
				for (int i = 0; i < vertices.Length; i++)
				{
					array[i] = localToWorldMatrix.MultiplyPoint3x4(vertices[i]);
				}
			}
			else
			{
				Array.Copy(vertices, array, vertices.Length);
			}
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			Queue<KeyValuePair<int, float>> queue = new Queue<KeyValuePair<int, float>>();
			if (sources == null)
			{
				goto IL_01DD;
			}
			using (HashSet<int>.Enumerator enumerator = sources.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int num = enumerator.Current;
					if (num >= 0 && num < vertices.Length)
					{
						dictionary[num] = 0f;
						queue.Enqueue(new KeyValuePair<int, float>(num, 0f));
					}
				}
				goto IL_01DD;
			}
			IL_00D3:
			KeyValuePair<int, float> keyValuePair = queue.Dequeue();
			int key = keyValuePair.Key;
			float value = keyValuePair.Value;
			if (key >= 0 && key < adjacency.Length && adjacency[key] != null)
			{
				List<int> list = adjacency[key];
				Vector3 vector = array[key];
				for (int j = 0; j < list.Count; j++)
				{
					int num2 = list[j];
					if (num2 >= 0 && num2 < vertices.Length)
					{
						float num3 = Vector3.Distance(vector, array[num2]);
						float num4 = value + num3;
						float num5;
						if (num4 <= softSelectionRadius && (!dictionary.TryGetValue(num2, out num5) || num5 > num4))
						{
							dictionary[num2] = num4;
							if (sources == null || !sources.Contains(num2))
							{
								float num6 = this.CalculateFalloff(num4 / softSelectionRadius);
								float num7;
								if (num6 > 0.001f && (!outWeights.TryGetValue(num2, out num7) || num6 > num7))
								{
									outWeights[num2] = num6;
								}
							}
							queue.Enqueue(new KeyValuePair<int, float>(num2, num4));
						}
					}
				}
			}
			IL_01DD:
			if (queue.Count <= 0)
			{
				return;
			}
			goto IL_00D3;
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00015F4C File Offset: 0x0001414C
		public float CalculateFalloff(float normalizedDist)
		{
			normalizedDist = Mathf.Clamp01(normalizedDist);
			switch (this.SoftFalloff)
			{
			case FalloffMode.Linear:
				return 1f - normalizedDist;
			case FalloffMode.Smooth:
			{
				float num = 1f - normalizedDist;
				return num * num * (3f - 2f * num);
			}
			case FalloffMode.Sharp:
				return Mathf.Pow(1f - normalizedDist, this.SharpExponent);
			default:
				return 1f - normalizedDist;
			}
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00015FB8 File Offset: 0x000141B8
		public void UpdateHover(Vector2 mouseScreen, Camera cam)
		{
			if (!this.HasTarget || cam == null)
			{
				this._hoveredAxis = GizmoAxis.None;
				return;
			}
			if (this._isDragging)
			{
				return;
			}
			Vector3 centroidWorld = this._centroidWorld;
			float num = TransformGizmo.GizmoScale(cam, centroidWorld);
			Transform primaryXform = this.PrimaryXform;
			if (this.Mode != GizmoMode.Rotate)
			{
				float num2 = 0.09f * num;
				Vector3 vector = centroidWorld + cam.transform.right * num2 + cam.transform.up * num2;
				Vector2 vector2 = TransformGizmo.WorldToScreen(cam, centroidWorld);
				float num3 = Vector2.Distance(vector2, TransformGizmo.WorldToScreen(cam, vector));
				if (Vector2.Distance(mouseScreen, vector2) <= num3)
				{
					this._hoveredAxis = GizmoAxis.Free;
					return;
				}
			}
			if (this.Mode == GizmoMode.Translate)
			{
				GizmoAxis gizmoAxis = this.TestPlaneHandleHit(mouseScreen, cam, centroidWorld, primaryXform, num);
				if (gizmoAxis != GizmoAxis.None)
				{
					this._hoveredAxis = gizmoAxis;
					return;
				}
			}
			float num4 = 14f;
			GizmoAxis gizmoAxis2 = GizmoAxis.None;
			if (this.Mode == GizmoMode.Rotate)
			{
				this.TestRingHit(mouseScreen, cam, centroidWorld, cam.transform.forward, num, ref num4, ref gizmoAxis2, GizmoAxis.ViewRotate, 1.1f);
				this.TestRingHit(mouseScreen, cam, centroidWorld, this.AxisW(GizmoAxis.X, primaryXform), num, ref num4, ref gizmoAxis2, GizmoAxis.X, 0.85f);
				this.TestRingHit(mouseScreen, cam, centroidWorld, this.AxisW(GizmoAxis.Y, primaryXform), num, ref num4, ref gizmoAxis2, GizmoAxis.Y, 0.85f);
				this.TestRingHit(mouseScreen, cam, centroidWorld, this.AxisW(GizmoAxis.Z, primaryXform), num, ref num4, ref gizmoAxis2, GizmoAxis.Z, 0.85f);
			}
			else
			{
				this.TestAxisHit(mouseScreen, cam, centroidWorld, this.AxisW(GizmoAxis.X, primaryXform), num, ref num4, ref gizmoAxis2, GizmoAxis.X);
				this.TestAxisHit(mouseScreen, cam, centroidWorld, this.AxisW(GizmoAxis.Y, primaryXform), num, ref num4, ref gizmoAxis2, GizmoAxis.Y);
				this.TestAxisHit(mouseScreen, cam, centroidWorld, this.AxisW(GizmoAxis.Z, primaryXform), num, ref num4, ref gizmoAxis2, GizmoAxis.Z);
			}
			this._hoveredAxis = gizmoAxis2;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00016164 File Offset: 0x00014364
		private GizmoAxis TestPlaneHandleHit(Vector2 mp, Camera cam, Vector3 wc, Transform xform, float s)
		{
			for (int i = 0; i < TransformGizmo.PLANE_AXES.Length; i++)
			{
				Vector3 vector;
				Vector3 vector2;
				this.GetPlaneAxes(TransformGizmo.PLANE_AXES[i], xform, out vector, out vector2);
				Vector3 vector3 = wc + vector * 0.25f * s + vector2 * 0.25f * s;
				Vector3 vector4 = wc + vector * 0.5f * s + vector2 * 0.25f * s;
				Vector3 vector5 = wc + vector * 0.5f * s + vector2 * 0.5f * s;
				Vector3 vector6 = wc + vector * 0.25f * s + vector2 * 0.5f * s;
				Vector2 vector7 = TransformGizmo.WorldToScreen(cam, vector3);
				Vector2 vector8 = TransformGizmo.WorldToScreen(cam, vector4);
				Vector2 vector9 = TransformGizmo.WorldToScreen(cam, vector5);
				Vector2 vector10 = TransformGizmo.WorldToScreen(cam, vector6);
				if (TransformGizmo.PointInQuad(mp, vector7, vector8, vector9, vector10))
				{
					return TransformGizmo.PLANE_AXES[i];
				}
			}
			return GizmoAxis.None;
		}

		// Token: 0x060002AE RID: 686 RVA: 0x000162A0 File Offset: 0x000144A0
		private void GetPlaneAxes(GizmoAxis plane, Transform xform, out Vector3 ax1, out Vector3 ax2)
		{
			if (plane == GizmoAxis.XY)
			{
				ax1 = this.AxisW(GizmoAxis.X, xform);
				ax2 = this.AxisW(GizmoAxis.Y, xform);
				return;
			}
			if (plane != GizmoAxis.XZ)
			{
				ax1 = this.AxisW(GizmoAxis.Y, xform);
				ax2 = this.AxisW(GizmoAxis.Z, xform);
				return;
			}
			ax1 = this.AxisW(GizmoAxis.X, xform);
			ax2 = this.AxisW(GizmoAxis.Z, xform);
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00016310 File Offset: 0x00014510
		private static bool PointInQuad(Vector2 p, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
		{
			return TransformGizmo.PointInTriangle(p, a, b, c) || TransformGizmo.PointInTriangle(p, a, c, d);
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0001632C File Offset: 0x0001452C
		private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
		{
			float num = TransformGizmo.Cross2D(p - a, b - a);
			float num2 = TransformGizmo.Cross2D(p - b, c - b);
			float num3 = TransformGizmo.Cross2D(p - c, a - c);
			bool flag = num < 0f || num2 < 0f || num3 < 0f;
			bool flag2 = num > 0f || num2 > 0f || num3 > 0f;
			return !flag || !flag2;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x000163B1 File Offset: 0x000145B1
		private static float Cross2D(Vector2 a, Vector2 b)
		{
			return a.x * b.y - a.y * b.x;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x000163D0 File Offset: 0x000145D0
		private void TestAxisHit(Vector2 mp, Camera cam, Vector3 c, Vector3 dir, float s, ref float best, ref GizmoAxis bestA, GizmoAxis axis)
		{
			Vector2 vector = TransformGizmo.WorldToScreen(cam, c);
			Vector2 vector2 = TransformGizmo.WorldToScreen(cam, c + dir * 1f * s);
			float num = TransformGizmo.PointToSegmentDist(mp, vector, vector2);
			if (num < best)
			{
				best = num;
				bestA = axis;
			}
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00016420 File Offset: 0x00014620
		private void TestRingHit(Vector2 mp, Camera cam, Vector3 c, Vector3 normal, float s, ref float best, ref GizmoAxis bestA, GizmoAxis axis, float ringRadius)
		{
			Vector3 vector;
			Vector3 vector2;
			TransformGizmo.Perpendiculars(normal, out vector, out vector2);
			float num = ringRadius * s;
			float num2 = float.MaxValue;
			for (int i = 0; i < 64; i++)
			{
				float num3 = (float)i / 64f * 3.1415927f * 2f;
				Vector3 vector3 = c + (vector * Mathf.Cos(num3) + vector2 * Mathf.Sin(num3)) * num;
				float num4 = Vector2.Distance(mp, TransformGizmo.WorldToScreen(cam, vector3));
				if (num4 < num2)
				{
					num2 = num4;
				}
			}
			if (num2 < best)
			{
				best = num2;
				bestA = axis;
			}
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x000164C0 File Offset: 0x000146C0
		public bool BeginDrag(Vector2 mouseScreen, Camera cam)
		{
			if (this._hoveredAxis == GizmoAxis.None || !this.HasTarget)
			{
				return false;
			}
			this._activeAxis = this._hoveredAxis;
			this._isDragging = true;
			this._dragStartScreen = mouseScreen;
			this._dragStartCentroidWorld = this._centroidWorld;
			for (int i = 0; i < this._targetIndices.Count; i++)
			{
				this._dragStartDeltas[i].Clear();
				this._dragStartPositionsWorld[i].Clear();
				Vector3[] array = this._vertices[i];
				Transform transform = this._xforms[i];
				DeformLayer deformLayer = this._layers[i];
				Dictionary<int, float> dictionary = this._combinedSoftWeights[i];
				if (array != null && dictionary != null)
				{
					Vector3[] array2 = ((deformLayer != null) ? deformLayer.Deltas : null);
					foreach (KeyValuePair<int, float> keyValuePair in dictionary)
					{
						int key = keyValuePair.Key;
						if (key >= 0 && key < array.Length)
						{
							Vector3 vector = ((array2 != null && key < array2.Length) ? array2[key] : Vector3.zero);
							this._dragStartDeltas[i][key] = vector;
							this._dragStartPositionsWorld[i][key] = ((transform != null) ? transform.TransformPoint(array[key]) : array[key]);
						}
					}
				}
			}
			if (this.HasMirrorTarget)
			{
				this._dragStartMirrorCentroidWorld = this.MirrorPointWorld(this._dragStartCentroidWorld);
			}
			return true;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00016670 File Offset: 0x00014870
		public void UpdateDrag(Vector2 mouseScreen, Camera cam)
		{
			if (!this._isDragging || cam == null)
			{
				return;
			}
			switch (this.Mode)
			{
			case GizmoMode.Translate:
				this.ApplyTranslate(mouseScreen, cam);
				break;
			case GizmoMode.Rotate:
				this.ApplyRotate(mouseScreen, cam);
				break;
			case GizmoMode.Scale:
				this.ApplyScale(mouseScreen, cam);
				break;
			}
			this.ComputeUnionCentroidWorld();
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x000166CC File Offset: 0x000148CC
		public void EndDrag()
		{
			this._isDragging = false;
			this._activeAxis = GizmoAxis.None;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x000166DC File Offset: 0x000148DC
		private void ApplyTranslate(Vector2 ms, Camera cam)
		{
			Transform primaryXform = this.PrimaryXform;
			Vector3 vector2;
			if (this._activeAxis == GizmoAxis.Free)
			{
				Vector3 vector = TransformGizmo.RayPlaneIntersect(cam, this._dragStartScreen, this._dragStartCentroidWorld, cam.transform.forward);
				vector2 = TransformGizmo.RayPlaneIntersect(cam, ms, this._dragStartCentroidWorld, cam.transform.forward) - vector;
			}
			else if (this._activeAxis == GizmoAxis.XY || this._activeAxis == GizmoAxis.XZ || this._activeAxis == GizmoAxis.YZ)
			{
				Vector3 planeNormal = this.GetPlaneNormal(this._activeAxis, primaryXform);
				Vector3 vector3 = TransformGizmo.RayPlaneIntersect(cam, this._dragStartScreen, this._dragStartCentroidWorld, planeNormal);
				vector2 = TransformGizmo.RayPlaneIntersect(cam, ms, this._dragStartCentroidWorld, planeNormal) - vector3;
			}
			else
			{
				Vector3 vector4 = this.AxisW(this._activeAxis, primaryXform);
				Vector3 vector5 = TransformGizmo.ClosestPointOnAxis(cam, this._dragStartScreen, this._dragStartCentroidWorld, vector4);
				vector2 = TransformGizmo.ClosestPointOnAxis(cam, ms, this._dragStartCentroidWorld, vector4) - vector5;
			}
			this.WriteTranslateDeltas(this._softWeights, vector2, false);
			if (this.HasMirrorTarget && this.SymmetryEnabled)
			{
				this.WriteTranslateDeltas(this._mirrorWeights, this.MirrorWorldVector(vector2), true);
			}
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00016800 File Offset: 0x00014A00
		private void WriteTranslateDeltas(List<Dictionary<int, float>> weightsPerR, Vector3 dispWorld, bool skipPrimary)
		{
			for (int i = 0; i < weightsPerR.Count; i++)
			{
				Dictionary<int, float> dictionary = weightsPerR[i];
				if (dictionary != null && dictionary.Count != 0)
				{
					Vector3[] array = this.EnsureDeltasForWrite(i);
					if (array != null)
					{
						Dictionary<int, float> dictionary2 = (skipPrimary ? this._softWeights[i] : null);
						foreach (KeyValuePair<int, float> keyValuePair in dictionary)
						{
							int key = keyValuePair.Key;
							Vector3 vector;
							if ((dictionary2 == null || !dictionary2.ContainsKey(key)) && this._dragStartDeltas[i].TryGetValue(key, out vector) && key >= 0 && key < array.Length)
							{
								array[key] = vector + this.WorldToBindDelta(i, key, dispWorld) * keyValuePair.Value;
							}
						}
						this._layers[i].Dirty = true;
					}
				}
			}
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0001690C File Offset: 0x00014B0C
		private void ApplyRotate(Vector2 ms, Camera cam)
		{
			if (this._activeAxis == GizmoAxis.Free || this._activeAxis == GizmoAxis.None)
			{
				return;
			}
			Vector2 vector = TransformGizmo.WorldToScreen(cam, this._dragStartCentroidWorld);
			Vector2 vector2 = this._dragStartScreen - vector;
			Vector2 vector3 = ms - vector;
			if (vector2.sqrMagnitude < 1f || vector3.sqrMagnitude < 1f)
			{
				return;
			}
			float num = Mathf.Atan2(vector3.y, vector3.x) - Mathf.Atan2(vector2.y, vector2.x);
			num *= -57.29578f;
			Vector3 vector4 = ((this._activeAxis == GizmoAxis.ViewRotate) ? cam.transform.forward : this.AxisW(this._activeAxis, this.PrimaryXform));
			if (Vector3.Dot(vector4, cam.transform.forward) > 0f)
			{
				num = -num;
			}
			Quaternion quaternion = Quaternion.AngleAxis(num, vector4);
			this.WriteRotateDeltas(this._softWeights, quaternion, this._dragStartCentroidWorld, false);
			if (this.HasMirrorTarget && this.SymmetryEnabled)
			{
				this.ApplyMirrorRotate(num, vector4);
			}
		}

		// Token: 0x060002BA RID: 698 RVA: 0x00016A14 File Offset: 0x00014C14
		private void ApplyMirrorRotate(float angle, Vector3 axW)
		{
			Vector3 vector = this.SymmetryAxisVectorRoot();
			Quaternion quaternion = Quaternion.AngleAxis((Mathf.Abs(Vector3.Dot((this._objectRoot != null) ? this._objectRoot.InverseTransformDirection(axW).normalized : axW.normalized, vector)) > 0.9f) ? angle : (-angle), axW);
			this.WriteRotateDeltas(this._mirrorWeights, quaternion, this._dragStartMirrorCentroidWorld, true);
		}

		// Token: 0x060002BB RID: 699 RVA: 0x00016A88 File Offset: 0x00014C88
		private void WriteRotateDeltas(List<Dictionary<int, float>> weightsPerR, Quaternion worldRot, Vector3 centroidWorld, bool skipPrimary)
		{
			for (int i = 0; i < weightsPerR.Count; i++)
			{
				Dictionary<int, float> dictionary = weightsPerR[i];
				if (dictionary != null && dictionary.Count != 0)
				{
					Vector3[] array = this.EnsureDeltasForWrite(i);
					if (array != null)
					{
						Dictionary<int, float> dictionary2 = (skipPrimary ? this._softWeights[i] : null);
						foreach (KeyValuePair<int, float> keyValuePair in dictionary)
						{
							int key = keyValuePair.Key;
							Vector3 vector;
							Vector3 vector2;
							if ((dictionary2 == null || !dictionary2.ContainsKey(key)) && this._dragStartDeltas[i].TryGetValue(key, out vector) && this._dragStartPositionsWorld[i].TryGetValue(key, out vector2) && key >= 0 && key < array.Length)
							{
								Vector3 vector3 = vector2 - centroidWorld;
								Vector3 vector4 = Quaternion.Slerp(Quaternion.identity, worldRot, keyValuePair.Value) * vector3 - vector3;
								array[key] = vector + this.WorldToBindDelta(i, key, vector4);
							}
						}
						this._layers[i].Dirty = true;
					}
				}
			}
		}

		// Token: 0x060002BC RID: 700 RVA: 0x00016BD4 File Offset: 0x00014DD4
		private void ApplyScale(Vector2 ms, Camera cam)
		{
			Transform primaryXform = this.PrimaryXform;
			float num2;
			bool flag;
			if (this._activeAxis == GizmoAxis.Free)
			{
				Vector2 vector = TransformGizmo.WorldToScreen(cam, this._dragStartCentroidWorld);
				float num = Mathf.Max(Vector2.Distance(this._dragStartScreen, vector), 1f);
				num2 = Vector2.Distance(ms, vector) / num;
				flag = true;
			}
			else
			{
				Vector3 vector2 = this.AxisW(this._activeAxis, primaryXform);
				Vector2 vector3 = TransformGizmo.WorldToScreen(cam, this._dragStartCentroidWorld);
				Vector2 vector4 = TransformGizmo.WorldToScreen(cam, this._dragStartCentroidWorld + vector2) - vector3;
				if (vector4.sqrMagnitude < 0.01f)
				{
					return;
				}
				vector4.Normalize();
				float num3 = Vector2.Dot(this._dragStartScreen - vector3, vector4);
				float num4 = Vector2.Dot(ms - vector3, vector4);
				if (Mathf.Abs(num3) < 1f)
				{
					return;
				}
				num2 = num4 / num3;
				flag = false;
			}
			Vector3 vector5 = (flag ? Vector3.zero : this.AxisW(this._activeAxis, primaryXform));
			this.WriteScaleDeltas(this._softWeights, num2, flag, vector5, this._dragStartCentroidWorld, false);
			if (this.HasMirrorTarget && this.SymmetryEnabled)
			{
				this.WriteScaleDeltas(this._mirrorWeights, num2, flag, vector5, this._dragStartMirrorCentroidWorld, true);
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x00016D0C File Offset: 0x00014F0C
		private void WriteScaleDeltas(List<Dictionary<int, float>> weightsPerR, float factor, bool uniform, Vector3 scaleAxisWorld, Vector3 centroidWorld, bool skipPrimary)
		{
			for (int i = 0; i < weightsPerR.Count; i++)
			{
				Dictionary<int, float> dictionary = weightsPerR[i];
				if (dictionary != null && dictionary.Count != 0)
				{
					Vector3[] array = this.EnsureDeltasForWrite(i);
					if (array != null)
					{
						Dictionary<int, float> dictionary2 = (skipPrimary ? this._softWeights[i] : null);
						foreach (KeyValuePair<int, float> keyValuePair in dictionary)
						{
							int key = keyValuePair.Key;
							Vector3 vector;
							Vector3 vector2;
							if ((dictionary2 == null || !dictionary2.ContainsKey(key)) && this._dragStartDeltas[i].TryGetValue(key, out vector) && this._dragStartPositionsWorld[i].TryGetValue(key, out vector2) && key >= 0 && key < array.Length)
							{
								Vector3 vector3 = vector2 - centroidWorld;
								float num = Mathf.Lerp(1f, factor, keyValuePair.Value);
								Vector3 vector4;
								if (uniform)
								{
									vector4 = vector3 * num;
								}
								else
								{
									float num2 = Vector3.Dot(vector3, scaleAxisWorld);
									vector4 = vector3 - scaleAxisWorld * num2 + scaleAxisWorld * (num2 * num);
								}
								Vector3 vector5 = vector4 - vector3;
								array[key] = vector + this.WorldToBindDelta(i, key, vector5);
							}
						}
						this._layers[i].Dirty = true;
					}
				}
			}
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00016EA0 File Offset: 0x000150A0
		private Vector3[] EnsureDeltasForWrite(int r)
		{
			DeformLayer deformLayer = this._layers[r];
			if (deformLayer == null && this.LazyEnsureLayerCallback != null)
			{
				deformLayer = this.LazyEnsureLayerCallback(r);
				this._layers[r] = deformLayer;
			}
			if (deformLayer == null)
			{
				return null;
			}
			return deformLayer.Deltas;
		}

		// Token: 0x060002BF RID: 703 RVA: 0x00016EEC File Offset: 0x000150EC
		private Vector3 WorldToBindDelta(int r, int vertexIdx, Vector3 worldDisp)
		{
			ShapeDeformer shapeDeformer = this._deformers[r];
			Transform transform = this._xforms[r];
			if (shapeDeformer != null)
			{
				Vector3 vector;
				shapeDeformer.WorldDeltaToBindDelta(vertexIdx, worldDisp, out vector);
				return vector;
			}
			if (!(transform != null))
			{
				return worldDisp;
			}
			return transform.InverseTransformVector(worldDisp);
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x00016F3C File Offset: 0x0001513C
		public Vector3 MirrorWorldVector(Vector3 worldVec)
		{
			Transform transform = ((this._objectRoot != null) ? this._objectRoot : this.PrimaryXform);
			if (transform == null)
			{
				return worldVec;
			}
			Vector3 vector = transform.InverseTransformVector(worldVec);
			int symmetryAxis = this.SymmetryAxis;
			if (symmetryAxis == 0)
			{
				vector.x = -vector.x;
			}
			else if (symmetryAxis == 1)
			{
				vector.y = -vector.y;
			}
			else
			{
				vector.z = -vector.z;
			}
			return transform.TransformVector(vector);
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x00016FBC File Offset: 0x000151BC
		public Vector3 MirrorPointWorld(Vector3 worldPoint)
		{
			Transform transform = ((this._objectRoot != null) ? this._objectRoot : this.PrimaryXform);
			if (transform == null)
			{
				return worldPoint;
			}
			Vector3 vector = transform.InverseTransformPoint(worldPoint);
			int symmetryAxis = this.SymmetryAxis;
			float symmetryCenter = this.SymmetryCenter;
			if (symmetryAxis == 0)
			{
				vector.x = symmetryCenter * 2f - vector.x;
			}
			else if (symmetryAxis == 1)
			{
				vector.y = symmetryCenter * 2f - vector.y;
			}
			else
			{
				vector.z = symmetryCenter * 2f - vector.z;
			}
			return transform.TransformPoint(vector);
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00017058 File Offset: 0x00015258
		private Vector3 SymmetryAxisVectorRoot()
		{
			int symmetryAxis = this.SymmetryAxis;
			if (symmetryAxis == 0)
			{
				return Vector3.right;
			}
			if (symmetryAxis == 1)
			{
				return Vector3.up;
			}
			return Vector3.forward;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00017084 File Offset: 0x00015284
		public void Render(Camera cam)
		{
			if (!this.HasTarget || cam == null)
			{
				return;
			}
			if (!this.EnsureMaterial())
			{
				return;
			}
			this._glMaterial.SetPass(0);
			Transform primaryXform = this.PrimaryXform;
			Vector3 centroidWorld = this._centroidWorld;
			float num = TransformGizmo.GizmoScale(cam, centroidWorld);
			Quaternion quaternion = this.GizmoRotation(primaryXform);
			switch (this.Mode)
			{
			case GizmoMode.Translate:
				this.DrawAxisLine(centroidWorld, this.AxisW(GizmoAxis.X, primaryXform), num, this.GetColor(GizmoAxis.X));
				this.DrawAxisLine(centroidWorld, this.AxisW(GizmoAxis.Y, primaryXform), num, this.GetColor(GizmoAxis.Y));
				this.DrawAxisLine(centroidWorld, this.AxisW(GizmoAxis.Z, primaryXform), num, this.GetColor(GizmoAxis.Z));
				this.DrawArrowHead(centroidWorld, this.AxisW(GizmoAxis.X, primaryXform), num, this.GetColor(GizmoAxis.X));
				this.DrawArrowHead(centroidWorld, this.AxisW(GizmoAxis.Y, primaryXform), num, this.GetColor(GizmoAxis.Y));
				this.DrawArrowHead(centroidWorld, this.AxisW(GizmoAxis.Z, primaryXform), num, this.GetColor(GizmoAxis.Z));
				this.DrawPlaneHandle(centroidWorld, primaryXform, num, GizmoAxis.XY);
				this.DrawPlaneHandle(centroidWorld, primaryXform, num, GizmoAxis.XZ);
				this.DrawPlaneHandle(centroidWorld, primaryXform, num, GizmoAxis.YZ);
				this.DrawCenterCube(centroidWorld, quaternion, num, this.GetColor(GizmoAxis.Free));
				return;
			case GizmoMode.Rotate:
				this.DrawRing(centroidWorld, this.AxisW(GizmoAxis.X, primaryXform), num, this.GetColor(GizmoAxis.X), 0.85f);
				this.DrawRing(centroidWorld, this.AxisW(GizmoAxis.Y, primaryXform), num, this.GetColor(GizmoAxis.Y), 0.85f);
				this.DrawRing(centroidWorld, this.AxisW(GizmoAxis.Z, primaryXform), num, this.GetColor(GizmoAxis.Z), 0.85f);
				this.DrawRing(centroidWorld, cam.transform.forward, num, this.GetColor(GizmoAxis.ViewRotate), 1.1f);
				return;
			case GizmoMode.Scale:
				this.DrawAxisLine(centroidWorld, this.AxisW(GizmoAxis.X, primaryXform), num, this.GetColor(GizmoAxis.X));
				this.DrawAxisLine(centroidWorld, this.AxisW(GizmoAxis.Y, primaryXform), num, this.GetColor(GizmoAxis.Y));
				this.DrawAxisLine(centroidWorld, this.AxisW(GizmoAxis.Z, primaryXform), num, this.GetColor(GizmoAxis.Z));
				this.DrawCubeEnd(centroidWorld, GizmoAxis.X, quaternion, num, this.GetColor(GizmoAxis.X));
				this.DrawCubeEnd(centroidWorld, GizmoAxis.Y, quaternion, num, this.GetColor(GizmoAxis.Y));
				this.DrawCubeEnd(centroidWorld, GizmoAxis.Z, quaternion, num, this.GetColor(GizmoAxis.Z));
				this.DrawCenterCube(centroidWorld, quaternion, num, this.GetColor(GizmoAxis.Free));
				return;
			default:
				return;
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x000172A8 File Offset: 0x000154A8
		public void RenderSoftRadius(Camera cam)
		{
			if (!this.HasTarget || !this.SoftSelectionEnabled || cam == null)
			{
				return;
			}
			if (this.SoftSelectionRadius <= 0f)
			{
				return;
			}
			if (!this.EnsureMaterial())
			{
				return;
			}
			this._glMaterial.SetPass(0);
			this.DrawWireSphere(this._centroidWorld, this.SoftSelectionRadius, TransformGizmo.COL_SOFT);
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0001730A File Offset: 0x0001550A
		private void DrawAxisLine(Vector3 center, Vector3 dir, float scale, Color col)
		{
			GL.Begin(1);
			GL.Color(col);
			GL.Vertex(center);
			GL.Vertex(center + dir * 1f * scale);
			GL.End();
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00017340 File Offset: 0x00015540
		private void DrawArrowHead(Vector3 center, Vector3 dir, float scale, Color col)
		{
			Vector3 vector = center + dir * 1f * scale;
			Vector3 vector2 = center + dir * 0.82f * scale;
			float num = 0.055f * scale;
			Vector3 vector3;
			Vector3 vector4;
			TransformGizmo.Perpendiculars(dir, out vector3, out vector4);
			GL.Begin(4);
			GL.Color(col);
			for (int i = 0; i < 8; i++)
			{
				float num2 = (float)i / 8f * 3.1415927f * 2f;
				float num3 = (float)(i + 1) / 8f * 3.1415927f * 2f;
				Vector3 vector5 = vector2 + (vector3 * Mathf.Cos(num2) + vector4 * Mathf.Sin(num2)) * num;
				Vector3 vector6 = vector2 + (vector3 * Mathf.Cos(num3) + vector4 * Mathf.Sin(num3)) * num;
				GL.Vertex(vector);
				GL.Vertex(vector5);
				GL.Vertex(vector6);
			}
			GL.End();
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00017454 File Offset: 0x00015654
		private void DrawRing(Vector3 center, Vector3 normal, float scale, Color col, float ringRadius)
		{
			Vector3 vector;
			Vector3 vector2;
			TransformGizmo.Perpendiculars(normal, out vector, out vector2);
			float num = ringRadius * scale;
			GL.Begin(1);
			GL.Color(col);
			for (int i = 0; i < 64; i++)
			{
				float num2 = (float)i / 64f * 3.1415927f * 2f;
				float num3 = (float)(i + 1) / 64f * 3.1415927f * 2f;
				GL.Vertex(center + (vector * Mathf.Cos(num2) + vector2 * Mathf.Sin(num2)) * num);
				GL.Vertex(center + (vector * Mathf.Cos(num3) + vector2 * Mathf.Sin(num3)) * num);
			}
			GL.End();
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x00017524 File Offset: 0x00015724
		private void DrawPlaneHandle(Vector3 wc, Transform xform, float scale, GizmoAxis plane)
		{
			Vector3 vector;
			Vector3 vector2;
			this.GetPlaneAxes(plane, xform, out vector, out vector2);
			Color color = this.GetColor(plane);
			color.a = 0.35f;
			Vector3 vector3 = wc + vector * 0.25f * scale + vector2 * 0.25f * scale;
			Vector3 vector4 = wc + vector * 0.5f * scale + vector2 * 0.25f * scale;
			Vector3 vector5 = wc + vector * 0.5f * scale + vector2 * 0.5f * scale;
			Vector3 vector6 = wc + vector * 0.25f * scale + vector2 * 0.5f * scale;
			GL.Begin(7);
			GL.Color(color);
			GL.Vertex(vector3);
			GL.Vertex(vector4);
			GL.Vertex(vector5);
			GL.Vertex(vector6);
			GL.End();
			Color color2 = color;
			color2.a = 0.8f;
			GL.Begin(1);
			GL.Color(color2);
			TransformGizmo.GLEdge(vector3, vector4);
			TransformGizmo.GLEdge(vector4, vector5);
			TransformGizmo.GLEdge(vector5, vector6);
			TransformGizmo.GLEdge(vector6, vector3);
			GL.End();
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0001767D File Offset: 0x0001587D
		private Vector3 GetPlaneNormal(GizmoAxis plane, Transform xform)
		{
			switch (plane)
			{
			case GizmoAxis.XY:
				return this.AxisW(GizmoAxis.Z, xform);
			case GizmoAxis.XZ:
				return this.AxisW(GizmoAxis.Y, xform);
			case GizmoAxis.YZ:
				return this.AxisW(GizmoAxis.X, xform);
			default:
				return Vector3.up;
			}
		}

		// Token: 0x060002CA RID: 714 RVA: 0x000176B8 File Offset: 0x000158B8
		private void DrawCubeEnd(Vector3 center, GizmoAxis axis, Quaternion gizmoRot, float scale, Color col)
		{
			Vector3 vector = gizmoRot * TransformGizmo.BaseDir(axis);
			Vector3 vector2 = center + vector * 1f * scale;
			float num = 0.055f * scale;
			Vector3 vector3 = gizmoRot * Vector3.right * num;
			Vector3 vector4 = gizmoRot * Vector3.up * num;
			Vector3 vector5 = gizmoRot * Vector3.forward * num;
			Vector3[] cubeCorners = this._cubeCorners;
			this.FillCubeCorners(vector2, vector3, vector4, vector5);
			GL.Begin(1);
			GL.Color(col);
			TransformGizmo.GLEdge(cubeCorners[0], cubeCorners[1]);
			TransformGizmo.GLEdge(cubeCorners[2], cubeCorners[3]);
			TransformGizmo.GLEdge(cubeCorners[4], cubeCorners[5]);
			TransformGizmo.GLEdge(cubeCorners[6], cubeCorners[7]);
			TransformGizmo.GLEdge(cubeCorners[0], cubeCorners[2]);
			TransformGizmo.GLEdge(cubeCorners[1], cubeCorners[3]);
			TransformGizmo.GLEdge(cubeCorners[4], cubeCorners[6]);
			TransformGizmo.GLEdge(cubeCorners[5], cubeCorners[7]);
			TransformGizmo.GLEdge(cubeCorners[0], cubeCorners[4]);
			TransformGizmo.GLEdge(cubeCorners[1], cubeCorners[5]);
			TransformGizmo.GLEdge(cubeCorners[2], cubeCorners[6]);
			TransformGizmo.GLEdge(cubeCorners[3], cubeCorners[7]);
			GL.End();
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00017850 File Offset: 0x00015A50
		private void FillCubeCorners(Vector3 center, Vector3 rx, Vector3 ry, Vector3 rz)
		{
			for (int i = 0; i < 8; i++)
			{
				float num = (((i & 1) == 0) ? (-1f) : 1f);
				float num2 = (((i & 2) == 0) ? (-1f) : 1f);
				float num3 = (((i & 4) == 0) ? (-1f) : 1f);
				this._cubeCorners[i] = center + rx * num + ry * num2 + rz * num3;
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x000178D4 File Offset: 0x00015AD4
		private void DrawCenterCube(Vector3 center, Quaternion gizmoRot, float scale, Color col)
		{
			float num = 0.09f * scale;
			Vector3 vector = gizmoRot * Vector3.right * num;
			Vector3 vector2 = gizmoRot * Vector3.up * num;
			Vector3 vector3 = gizmoRot * Vector3.forward * num;
			Vector3[] cubeCorners = this._cubeCorners;
			this.FillCubeCorners(center, vector, vector2, vector3);
			GL.Begin(1);
			GL.Color(col);
			TransformGizmo.GLEdge(cubeCorners[0], cubeCorners[1]);
			TransformGizmo.GLEdge(cubeCorners[1], cubeCorners[3]);
			TransformGizmo.GLEdge(cubeCorners[3], cubeCorners[2]);
			TransformGizmo.GLEdge(cubeCorners[2], cubeCorners[0]);
			TransformGizmo.GLEdge(cubeCorners[4], cubeCorners[5]);
			TransformGizmo.GLEdge(cubeCorners[5], cubeCorners[7]);
			TransformGizmo.GLEdge(cubeCorners[7], cubeCorners[6]);
			TransformGizmo.GLEdge(cubeCorners[6], cubeCorners[4]);
			TransformGizmo.GLEdge(cubeCorners[0], cubeCorners[4]);
			TransformGizmo.GLEdge(cubeCorners[1], cubeCorners[5]);
			TransformGizmo.GLEdge(cubeCorners[2], cubeCorners[6]);
			TransformGizmo.GLEdge(cubeCorners[3], cubeCorners[7]);
			GL.End();
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00017A40 File Offset: 0x00015C40
		private void DrawWireSphere(Vector3 center, float radius, Color col)
		{
			GL.Begin(1);
			GL.Color(col);
			this.DrawGLCircle(center, Vector3.right, Vector3.up, radius);
			this.DrawGLCircle(center, Vector3.up, Vector3.forward, radius);
			this.DrawGLCircle(center, Vector3.right, Vector3.forward, radius);
			GL.End();
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00017A94 File Offset: 0x00015C94
		private void DrawGLCircle(Vector3 center, Vector3 ax1, Vector3 ax2, float r)
		{
			for (int i = 0; i < 48; i++)
			{
				float num = (float)i / 48f * 3.1415927f * 2f;
				float num2 = (float)(i + 1) / 48f * 3.1415927f * 2f;
				GL.Vertex(center + (ax1 * Mathf.Cos(num) + ax2 * Mathf.Sin(num)) * r);
				GL.Vertex(center + (ax1 * Mathf.Cos(num2) + ax2 * Mathf.Sin(num2)) * r);
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00017B3E File Offset: 0x00015D3E
		private static void GLEdge(Vector3 a, Vector3 b)
		{
			GL.Vertex(a);
			GL.Vertex(b);
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00017B4C File Offset: 0x00015D4C
		private Quaternion GizmoRotation(Transform xform)
		{
			if (this.Space == GizmoSpace.Object && this._objectRoot != null)
			{
				return this._objectRoot.rotation;
			}
			if (this.Space == GizmoSpace.Normal)
			{
				return ((xform != null) ? xform.rotation : Quaternion.identity) * this._localOrientation;
			}
			return Quaternion.identity;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00017BAC File Offset: 0x00015DAC
		private Vector3 AxisW(GizmoAxis axis, Transform xform)
		{
			return this.GizmoRotation(xform) * TransformGizmo.BaseDir(axis);
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00017BC0 File Offset: 0x00015DC0
		private bool EnsureMaterial()
		{
			if (this._glMaterial != null)
			{
				return true;
			}
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			if (shader == null)
			{
				return false;
			}
			this._glMaterial = new Material(shader);
			this._glMaterial.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
			this._glMaterial.SetInt("_SrcBlend", 5);
			this._glMaterial.SetInt("_DstBlend", 10);
			this._glMaterial.SetInt("_Cull", 0);
			this._glMaterial.SetInt("_ZWrite", 0);
			this._glMaterial.SetInt("_ZTest", 8);
			return true;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00017C64 File Offset: 0x00015E64
		private Color GetColor(GizmoAxis axis)
		{
			if (axis == this._hoveredAxis || axis == this._activeAxis)
			{
				return TransformGizmo.COL_HOVER;
			}
			switch (axis)
			{
			case GizmoAxis.X:
				return TransformGizmo.COL_X;
			case GizmoAxis.Y:
				return TransformGizmo.COL_Y;
			case GizmoAxis.Z:
				return TransformGizmo.COL_Z;
			case GizmoAxis.XY:
				return TransformGizmo.COL_Z;
			case GizmoAxis.XZ:
				return TransformGizmo.COL_Y;
			case GizmoAxis.YZ:
				return TransformGizmo.COL_X;
			case GizmoAxis.ViewRotate:
				return TransformGizmo.COL_VIEW;
			}
			return TransformGizmo.COL_FREE;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00017CE2 File Offset: 0x00015EE2
		private static float GizmoScale(Camera cam, Vector3 worldPos)
		{
			return Vector3.Distance(cam.transform.position, worldPos) * 0.12f;
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00017CFB File Offset: 0x00015EFB
		private static Vector3 BaseDir(GizmoAxis axis)
		{
			switch (axis)
			{
			case GizmoAxis.X:
				return Vector3.right;
			case GizmoAxis.Y:
				return Vector3.up;
			case GizmoAxis.Z:
				return Vector3.forward;
			default:
				return Vector3.zero;
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00017D2C File Offset: 0x00015F2C
		private static Vector2 WorldToScreen(Camera cam, Vector3 worldPos)
		{
			Vector3 vector = cam.WorldToScreenPoint(worldPos);
			return new Vector2(vector.x, vector.y);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00017D54 File Offset: 0x00015F54
		private static Vector3 RayPlaneIntersect(Camera cam, Vector2 screenPos, Vector3 planePoint, Vector3 planeNormal)
		{
			Ray ray = cam.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0f));
			float num = Vector3.Dot(ray.direction, planeNormal);
			if (Mathf.Abs(num) < 0.0001f)
			{
				return planePoint;
			}
			float num2 = Vector3.Dot(planePoint - ray.origin, planeNormal) / num;
			return ray.origin + ray.direction * num2;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00017DCC File Offset: 0x00015FCC
		private static Vector3 ClosestPointOnAxis(Camera cam, Vector2 screenPos, Vector3 axisOrigin, Vector3 axisDir)
		{
			Ray ray = cam.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0f));
			Vector3 vector = axisOrigin - ray.origin;
			float num = Vector3.Dot(axisDir, axisDir);
			float num2 = Vector3.Dot(axisDir, ray.direction);
			float num3 = Vector3.Dot(ray.direction, ray.direction);
			float num4 = Vector3.Dot(axisDir, vector);
			float num5 = num * num3 - num2 * num2;
			if (Mathf.Abs(num5) < 0.0001f)
			{
				return axisOrigin;
			}
			float num6 = (num2 * Vector3.Dot(ray.direction, vector) - num3 * num4) / num5;
			return axisOrigin + axisDir * num6;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00017E78 File Offset: 0x00016078
		private static float PointToSegmentDist(Vector2 p, Vector2 a, Vector2 b)
		{
			Vector2 vector = b - a;
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude < 0.001f)
			{
				return Vector2.Distance(p, a);
			}
			float num = Mathf.Clamp01(Vector2.Dot(p - a, vector) / sqrMagnitude);
			return Vector2.Distance(p, a + vector * num);
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00017ED0 File Offset: 0x000160D0
		private static void Perpendiculars(Vector3 dir, out Vector3 tan, out Vector3 bin)
		{
			Vector3 vector = ((Mathf.Abs(Vector3.Dot(dir, Vector3.up)) < 0.99f) ? Vector3.up : Vector3.right);
			tan = Vector3.Cross(dir, vector).normalized;
			bin = Vector3.Cross(dir, tan).normalized;
		}

		// Token: 0x04000157 RID: 343
		private readonly List<HashSet<int>> _targetIndices = new List<HashSet<int>>();

		// Token: 0x04000158 RID: 344
		private readonly List<Dictionary<int, float>> _softWeights = new List<Dictionary<int, float>>();

		// Token: 0x04000159 RID: 345
		private readonly List<HashSet<int>> _mirrorIndices = new List<HashSet<int>>();

		// Token: 0x0400015A RID: 346
		private readonly List<Dictionary<int, float>> _mirrorWeights = new List<Dictionary<int, float>>();

		// Token: 0x0400015B RID: 347
		private readonly List<Dictionary<int, float>> _combinedSoftWeights = new List<Dictionary<int, float>>();

		// Token: 0x0400015C RID: 348
		private readonly List<Vector3[]> _vertices = new List<Vector3[]>();

		// Token: 0x0400015D RID: 349
		private readonly List<Vector3[]> _normals = new List<Vector3[]>();

		// Token: 0x0400015E RID: 350
		private readonly List<Transform> _xforms = new List<Transform>();

		// Token: 0x0400015F RID: 351
		private readonly List<ShapeDeformer> _deformers = new List<ShapeDeformer>();

		// Token: 0x04000160 RID: 352
		private readonly List<DeformLayer> _layers = new List<DeformLayer>();

		// Token: 0x04000161 RID: 353
		private readonly List<SpatialHashGrid> _grids = new List<SpatialHashGrid>();

		// Token: 0x04000162 RID: 354
		private readonly List<List<int>[]> _adjacencies = new List<List<int>[]>();

		// Token: 0x04000163 RID: 355
		private int _primaryIdx = -1;

		// Token: 0x04000164 RID: 356
		private readonly List<Dictionary<int, Vector3>> _dragStartDeltas = new List<Dictionary<int, Vector3>>();

		// Token: 0x04000165 RID: 357
		private readonly List<Dictionary<int, Vector3>> _dragStartPositionsWorld = new List<Dictionary<int, Vector3>>();

		// Token: 0x04000166 RID: 358
		private Vector3 _centroidWorld;

		// Token: 0x04000167 RID: 359
		private Quaternion _localOrientation = Quaternion.identity;

		// Token: 0x04000168 RID: 360
		private Transform _objectRoot;

		// Token: 0x04000169 RID: 361
		private GizmoAxis _hoveredAxis;

		// Token: 0x0400016A RID: 362
		private GizmoAxis _activeAxis;

		// Token: 0x0400016B RID: 363
		private bool _isDragging;

		// Token: 0x0400016C RID: 364
		private Vector2 _dragStartScreen;

		// Token: 0x0400016D RID: 365
		private Vector3 _dragStartCentroidWorld;

		// Token: 0x0400016E RID: 366
		private Vector3 _dragStartMirrorCentroidWorld;

		// Token: 0x0400016F RID: 367
		private Material _glMaterial;

		// Token: 0x04000170 RID: 368
		private const float AXIS_LEN = 1f;

		// Token: 0x04000171 RID: 369
		private const float ARROW_LEN = 0.18f;

		// Token: 0x04000172 RID: 370
		private const float ARROW_RAD = 0.055f;

		// Token: 0x04000173 RID: 371
		private const float CUBE_HALF = 0.055f;

		// Token: 0x04000174 RID: 372
		private const float CENTER_HALF = 0.09f;

		// Token: 0x04000175 RID: 373
		private const float RING_RAD = 0.85f;

		// Token: 0x04000176 RID: 374
		private const float HIT_PX = 14f;

		// Token: 0x04000177 RID: 375
		private const int RING_SEG = 64;

		// Token: 0x04000178 RID: 376
		private const int ARROW_SEG = 8;

		// Token: 0x04000179 RID: 377
		private const int SPHERE_SEG = 48;

		// Token: 0x0400017A RID: 378
		private const float PLANE_OFFSET = 0.25f;

		// Token: 0x0400017B RID: 379
		private const float PLANE_SIZE = 0.25f;

		// Token: 0x0400017C RID: 380
		private const float PLANE_ALPHA = 0.35f;

		// Token: 0x0400017D RID: 381
		private const float VIEW_RING_RAD = 1.1f;

		// Token: 0x0400017E RID: 382
		private const float SCREEN_SCALE = 0.12f;

		// Token: 0x0400017F RID: 383
		private static readonly Color COL_X = new Color(0.9f, 0.2f, 0.2f);

		// Token: 0x04000180 RID: 384
		private static readonly Color COL_Y = new Color(0.2f, 0.9f, 0.2f);

		// Token: 0x04000181 RID: 385
		private static readonly Color COL_Z = new Color(0.3f, 0.3f, 0.95f);

		// Token: 0x04000182 RID: 386
		private static readonly Color COL_HOVER = Color.yellow;

		// Token: 0x04000183 RID: 387
		private static readonly Color COL_FREE = new Color(0.85f, 0.85f, 0.85f);

		// Token: 0x04000184 RID: 388
		private static readonly Color COL_VIEW = new Color(0.8f, 0.8f, 0.8f);

		// Token: 0x04000185 RID: 389
		private static readonly Color COL_SOFT = new Color(0.4f, 0.7f, 1f, 0.3f);

		// Token: 0x04000186 RID: 390
		private static readonly GizmoAxis[] PLANE_AXES = new GizmoAxis[]
		{
			GizmoAxis.XY,
			GizmoAxis.XZ,
			GizmoAxis.YZ
		};

		// Token: 0x04000187 RID: 391
		private readonly Vector3[] _cubeCorners = new Vector3[8];

		// Token: 0x04000188 RID: 392
		private readonly List<Vector3> _srcWorldBuf = new List<Vector3>();
	}
}
