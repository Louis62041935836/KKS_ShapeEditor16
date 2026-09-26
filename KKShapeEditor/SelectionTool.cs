using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000033 RID: 51
	public class SelectionTool
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000222 RID: 546 RVA: 0x0001261D File Offset: 0x0001081D
		// (set) Token: 0x06000223 RID: 547 RVA: 0x00012625 File Offset: 0x00010825
		public float Radius { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0001262E File Offset: 0x0001082E
		// (set) Token: 0x06000225 RID: 549 RVA: 0x00012636 File Offset: 0x00010836
		public float Strength { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000226 RID: 550 RVA: 0x0001263F File Offset: 0x0001083F
		// (set) Token: 0x06000227 RID: 551 RVA: 0x00012647 File Offset: 0x00010847
		public FalloffMode Falloff { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000228 RID: 552 RVA: 0x00012650 File Offset: 0x00010850
		// (set) Token: 0x06000229 RID: 553 RVA: 0x00012658 File Offset: 0x00010858
		public float SharpExponent { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600022A RID: 554 RVA: 0x00012661 File Offset: 0x00010861
		public HashSet<int> SelectedVertices
		{
			get
			{
				return this._selectedVertices;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600022B RID: 555 RVA: 0x00012669 File Offset: 0x00010869
		public SpatialHashGrid Grid
		{
			get
			{
				return this._grid;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600022C RID: 556 RVA: 0x00012671 File Offset: 0x00010871
		public Transform ColliderTransform
		{
			get
			{
				if (!(this._tempCollider != null))
				{
					return null;
				}
				return this._tempCollider.transform;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600022D RID: 557 RVA: 0x0001268E File Offset: 0x0001088E
		public Mesh BakedMesh
		{
			get
			{
				return this._bakedMesh;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600022E RID: 558 RVA: 0x00012696 File Offset: 0x00010896
		public SkinnedMeshRenderer TargetRenderer
		{
			get
			{
				return this._targetRenderer;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x0600022F RID: 559 RVA: 0x0001269E File Offset: 0x0001089E
		public Vector3[] CachedVertices
		{
			get
			{
				return this._cachedVertices;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000230 RID: 560 RVA: 0x000126A6 File Offset: 0x000108A6
		public Vector3[] CachedNormals
		{
			get
			{
				return this._cachedNormals;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000231 RID: 561 RVA: 0x000126AE File Offset: 0x000108AE
		public Transform TargetTransform
		{
			get
			{
				if (this._targetRenderer != null)
				{
					return this._targetRenderer.transform;
				}
				if (this._targetMeshFilter != null)
				{
					return this._targetMeshFilter.transform;
				}
				return null;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000232 RID: 562 RVA: 0x000126E5 File Offset: 0x000108E5
		public bool HasTarget
		{
			get
			{
				return this._targetRenderer != null || this._targetMeshFilter != null;
			}
		}

		// Token: 0x06000233 RID: 563 RVA: 0x00012704 File Offset: 0x00010904
		public SelectionTool()
		{
			this.Radius = ((ShapeEditorPlugin.DefaultBrushRadius != null) ? ShapeEditorPlugin.DefaultBrushRadius.Value : 0.05f);
			this.Strength = ((ShapeEditorPlugin.DefaultBrushStrength != null) ? ShapeEditorPlugin.DefaultBrushStrength.Value : 0.5f);
			this.Falloff = FalloffMode.Smooth;
			this.SharpExponent = 3f;
			this._weldAddCallback = delegate(int j, float distSq)
			{
				this._selectedVertices.Add(j);
			};
			this._weldRemoveCallback = delegate(int j, float distSq)
			{
				this._selectedVertices.Remove(j);
			};
			this._keepAddCallback = delegate(int j, float distSq)
			{
				this._orphanKeepSet.Add(j);
			};
			this._ringAddCallback = delegate(int j, float distSq)
			{
				this._ringSet.Add(j);
			};
		}

		// Token: 0x06000234 RID: 564 RVA: 0x000127F0 File Offset: 0x000109F0
		public void SetTarget(SkinnedMeshRenderer renderer)
		{
			this.CleanupCollider();
			this._targetRenderer = renderer;
			this._targetMeshFilter = null;
			this._isStatic = false;
			if (renderer == null)
			{
				return;
			}
			this._bakedMesh = new Mesh();
			renderer.BakeMesh(this._bakedMesh);
			SelectionTool.UndoBakedScale(this._bakedMesh, renderer);
			this.SetupCollider(renderer.transform);
			this.RebuildGrid();
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00012858 File Offset: 0x00010A58
		public void SetTarget(MeshFilter meshFilter)
		{
			this.CleanupCollider();
			this._targetRenderer = null;
			this._targetMeshFilter = meshFilter;
			this._isStatic = true;
			if (meshFilter == null)
			{
				return;
			}
			Mesh sharedMesh = meshFilter.sharedMesh;
			this._bakedMesh = new Mesh();
			this._bakedMesh.vertices = sharedMesh.vertices;
			this._bakedMesh.normals = sharedMesh.normals;
			this._bakedMesh.uv = sharedMesh.uv;
			this._bakedMesh.triangles = sharedMesh.triangles;
			this._bakedMesh.RecalculateBounds();
			this.SetupCollider(meshFilter.transform);
			this.RebuildGrid();
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00012900 File Offset: 0x00010B00
		internal static void EnsureLayerIsolated()
		{
			if (SelectionTool._layerIsolated)
			{
				return;
			}
			for (int i = 0; i < 32; i++)
			{
				Physics.IgnoreLayerCollision(29, i, true);
			}
			SelectionTool._layerIsolated = true;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00012934 File Offset: 0x00010B34
		private void SetupCollider(Transform parent)
		{
			SelectionTool.EnsureLayerIsolated();
			GameObject gameObject = new GameObject("_kkse_selectionCollider_" + ((parent != null) ? parent.name : "?"));
			gameObject.layer = 29;
			gameObject.transform.SetParent(parent, false);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			this._tempCollider = gameObject.AddComponent<MeshCollider>();
			this._tempCollider.sharedMesh = this._bakedMesh;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x000129D0 File Offset: 0x00010BD0
		public void RefreshCollider()
		{
			if (this._tempCollider == null)
			{
				return;
			}
			if (this._isStatic)
			{
				return;
			}
			if (this._targetRenderer == null)
			{
				return;
			}
			bool flag = !this._targetRenderer.enabled;
			if (flag)
			{
				this._targetRenderer.enabled = true;
			}
			this._targetRenderer.BakeMesh(this._bakedMesh);
			if (flag)
			{
				this._targetRenderer.enabled = false;
			}
			SelectionTool.UndoBakedScale(this._bakedMesh, this._targetRenderer);
			this._tempCollider.sharedMesh = null;
			this._tempCollider.sharedMesh = this._bakedMesh;
			this.RebuildGrid();
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00012A74 File Offset: 0x00010C74
		public void RefreshCollider(Mesh deformedMesh)
		{
			if (this._tempCollider == null || deformedMesh == null)
			{
				return;
			}
			this._bakedMesh.vertices = deformedMesh.vertices;
			this._bakedMesh.RecalculateBounds();
			this._tempCollider.sharedMesh = null;
			this._tempCollider.sharedMesh = this._bakedMesh;
			this.RebuildGrid();
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00012AD8 File Offset: 0x00010CD8
		private void RebuildGrid()
		{
			if (this._bakedMesh == null)
			{
				this._cachedVertices = null;
				this._grid = null;
			}
			else
			{
				this._cachedVertices = this._bakedMesh.vertices;
				this._cachedNormals = this._bakedMesh.normals;
				if (this._cachedVertices.Length == 0)
				{
					this._grid = null;
				}
				else if (this._grid != null)
				{
					this._grid.Rebuild(this._cachedVertices, this._bakedMesh.bounds);
				}
				else
				{
					this._grid = new SpatialHashGrid(this._cachedVertices, this._bakedMesh.bounds);
				}
			}
			this.RebuildFaceCache();
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00012B80 File Offset: 0x00010D80
		private void RebuildFaceCache()
		{
			if (this._bakedMesh == null || this._cachedVertices == null || this._cachedVertices.Length == 0)
			{
				this._faceTriangles = null;
				this._faceCentroids = null;
				return;
			}
			int[] triangles = this._bakedMesh.triangles;
			int num = triangles.Length / 3;
			if (this._faceCentroids == null || this._faceCentroids.Length != num)
			{
				this._faceCentroids = new Vector3[num];
			}
			this._faceTriangles = triangles;
			Vector3[] cachedVertices = this._cachedVertices;
			int num2 = cachedVertices.Length;
			for (int i = 0; i < num; i++)
			{
				int num3 = triangles[i * 3];
				int num4 = triangles[i * 3 + 1];
				int num5 = triangles[i * 3 + 2];
				if (num3 >= num2 || num4 >= num2 || num5 >= num2)
				{
					this._faceCentroids[i] = cachedVertices[0];
				}
				else
				{
					this._faceCentroids[i] = (cachedVertices[num3] + cachedVertices[num4] + cachedVertices[num5]) * 0.33333334f;
				}
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00012C90 File Offset: 0x00010E90
		public bool Raycast(Ray ray, out Vector3 hitPoint, out Vector3 hitNormal)
		{
			hitPoint = Vector3.zero;
			hitNormal = Vector3.up;
			if (this._tempCollider == null)
			{
				return false;
			}
			RaycastHit raycastHit;
			if (this._tempCollider.Raycast(ray, out raycastHit, 1000f))
			{
				hitPoint = raycastHit.point;
				hitNormal = raycastHit.normal;
				return true;
			}
			return false;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00012CF8 File Offset: 0x00010EF8
		private float ToLocalRadius(float worldRadius)
		{
			Transform targetTransform = this.TargetTransform;
			if (targetTransform == null)
			{
				return worldRadius;
			}
			Vector3 lossyScale = targetTransform.lossyScale;
			float num = (Mathf.Abs(lossyScale.x) + Mathf.Abs(lossyScale.y) + Mathf.Abs(lossyScale.z)) / 3f;
			if (num < 1E-06f)
			{
				return worldRadius;
			}
			return worldRadius / num;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00012D58 File Offset: 0x00010F58
		public BrushResult BrushSelect(Ray mouseRay)
		{
			if (this._tempCollider == null)
			{
				return null;
			}
			if (this._targetRenderer == null && this._targetMeshFilter == null)
			{
				return null;
			}
			RaycastHit raycastHit;
			if (!this._tempCollider.Raycast(mouseRay, out raycastHit, 1000f))
			{
				return null;
			}
			Vector3 vector = this._tempCollider.transform.InverseTransformPoint(raycastHit.point);
			Dictionary<int, float> affected = this._brushResultCache.AffectedVertices;
			affected.Clear();
			this._brushResultCache.HitPoint = raycastHit.point;
			this._brushResultCache.HitNormal = raycastHit.normal;
			float radius = this.ToLocalRadius(this.Radius);
			float strength = this.Strength;
			bool[] liveMask = this.VertexLiveMask;
			int num = ((this._cachedVertices != null) ? this._cachedVertices.Length : 0);
			bool hasLiveMask = liveMask != null && liveMask.Length == num;
			if (this._grid != null && this._cachedVertices != null)
			{
				this._grid.FindVerticesInRadius(vector, radius, delegate(int i, float distSq)
				{
					if (hasLiveMask && !liveMask[i])
					{
						return;
					}
					float num5 = Mathf.Sqrt(distSq);
					float num6 = this.CalculateFalloff(num5 / radius);
					affected[i] = num6 * strength;
				});
			}
			else
			{
				Vector3[] array = this._cachedVertices ?? ((this._bakedMesh != null) ? this._bakedMesh.vertices : null);
				if (array == null)
				{
					return null;
				}
				bool flag = liveMask != null && liveMask.Length == array.Length;
				float num2 = radius * radius;
				for (int j = 0; j < array.Length; j++)
				{
					if (!flag || liveMask[j])
					{
						float sqrMagnitude = (array[j] - vector).sqrMagnitude;
						if (sqrMagnitude <= num2)
						{
							float num3 = Mathf.Sqrt(sqrMagnitude);
							float num4 = this.CalculateFalloff(num3 / radius);
							affected[j] = num4 * strength;
						}
					}
				}
			}
			return this._brushResultCache;
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00012F6C File Offset: 0x0001116C
		public BrushResult BrushSelectAtPoint(Vector3 localPoint, Vector3 worldPoint, Vector3 worldNormal)
		{
			if (this._grid == null || this._cachedVertices == null)
			{
				return null;
			}
			Dictionary<int, float> affected = new Dictionary<int, float>();
			float radius = this.ToLocalRadius(this.Radius);
			float strength = this.Strength;
			bool[] liveMask = this.VertexLiveMask;
			bool hasLiveMask = liveMask != null && liveMask.Length == this._cachedVertices.Length;
			this._grid.FindVerticesInRadius(localPoint, radius, delegate(int i, float distSq)
			{
				if (hasLiveMask && !liveMask[i])
				{
					return;
				}
				float num = Mathf.Sqrt(distSq);
				float num2 = this.CalculateFalloff(num / radius);
				affected[i] = num2 * strength;
			});
			if (affected.Count == 0)
			{
				return null;
			}
			return new BrushResult
			{
				HitPoint = worldPoint,
				HitNormal = worldNormal,
				AffectedVertices = affected
			};
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0001303C File Offset: 0x0001123C
		private static bool IsOccluded(Vector3 camPos, Vector3 worldPos)
		{
			Vector3 vector = worldPos - camPos;
			float magnitude = vector.magnitude;
			if (magnitude < 0.0001f)
			{
				return false;
			}
			vector /= magnitude;
			RaycastHit raycastHit;
			return Physics.Raycast(camPos, vector, out raycastHit, magnitude + 0.01f, SelectionTool._kkseLayerMask) && raycastHit.distance < magnitude - 0.0001f;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00013094 File Offset: 0x00011294
		private bool PassesVisibilityFilter(int i, Vector3 worldPos, Vector3 camPos, Matrix4x4 toWorld, Vector3[] worldNormalsOverride, bool hasOverride, Vector3[] localNormals)
		{
			Vector3 vector;
			if (hasOverride)
			{
				vector = worldNormalsOverride[i];
			}
			else
			{
				if (localNormals == null || i >= localNormals.Length)
				{
					return true;
				}
				vector = toWorld.MultiplyVector(localNormals[i]);
			}
			return Vector3.Dot(vector, camPos - worldPos) > 0f && !SelectionTool.IsOccluded(camPos, worldPos);
		}

		// Token: 0x06000242 RID: 578 RVA: 0x000130F4 File Offset: 0x000112F4
		public void SelectBox(Camera cam, Rect screenRect, bool additive, bool includeBackFace, Vector3[] worldNormalsOverride = null)
		{
			if (this._tempCollider == null)
			{
				return;
			}
			Vector3[] array = this._cachedVertices ?? ((this._bakedMesh != null) ? this._bakedMesh.vertices : null);
			if (array == null)
			{
				return;
			}
			if (!additive)
			{
				this._selectedVertices.Clear();
			}
			Matrix4x4 localToWorldMatrix = this._tempCollider.transform.localToWorldMatrix;
			Vector3 position = cam.transform.position;
			bool flag;
			Vector3[] array2;
			bool flag2;
			bool[] array3;
			bool flag3;
			this.ResolveSelectionFilter(array, includeBackFace, worldNormalsOverride, out flag, out array2, out flag2, out array3, out flag3);
			for (int i = 0; i < array.Length; i++)
			{
				if (!flag3 || array3[i])
				{
					Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(array[i]);
					Vector3 vector2 = cam.WorldToScreenPoint(vector);
					if (vector2.z > 0f && screenRect.Contains(new Vector2(vector2.x, vector2.y)) && (!flag || this.PassesVisibilityFilter(i, vector, position, localToWorldMatrix, worldNormalsOverride, flag2, array2)))
					{
						this._selectedVertices.Add(i);
					}
				}
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00013208 File Offset: 0x00011408
		public void DeselectBox(Camera cam, Rect screenRect, bool includeBackFace, Vector3[] worldNormalsOverride = null)
		{
			if (this._tempCollider == null)
			{
				return;
			}
			if (this._selectedVertices.Count == 0)
			{
				return;
			}
			Vector3[] array = this._cachedVertices ?? ((this._bakedMesh != null) ? this._bakedMesh.vertices : null);
			if (array == null)
			{
				return;
			}
			Matrix4x4 localToWorldMatrix = this._tempCollider.transform.localToWorldMatrix;
			Vector3 position = cam.transform.position;
			bool flag;
			Vector3[] array2;
			bool flag2;
			bool[] array3;
			bool flag3;
			this.ResolveSelectionFilter(array, includeBackFace, worldNormalsOverride, out flag, out array2, out flag2, out array3, out flag3);
			for (int i = 0; i < array.Length; i++)
			{
				if (this._selectedVertices.Contains(i) && (!flag3 || array3[i]))
				{
					Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(array[i]);
					Vector3 vector2 = cam.WorldToScreenPoint(vector);
					if (vector2.z > 0f && screenRect.Contains(new Vector2(vector2.x, vector2.y)) && (!flag || this.PassesVisibilityFilter(i, vector, position, localToWorldMatrix, worldNormalsOverride, flag2, array2)))
					{
						this._selectedVertices.Remove(i);
					}
				}
			}
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0001332C File Offset: 0x0001152C
		public bool BrushAccumulate(Camera cam, Vector3 worldCenter, float worldRadius, bool erase, bool includeBackFace, Vector3[] worldNormalsOverride = null)
		{
			if (this._tempCollider == null || cam == null)
			{
				return false;
			}
			Vector3[] vertices = this._cachedVertices ?? ((this._bakedMesh != null) ? this._bakedMesh.vertices : null);
			if (vertices == null)
			{
				return false;
			}
			if (erase && this._selectedVertices.Count == 0)
			{
				return false;
			}
			Transform transform = this._tempCollider.transform;
			Matrix4x4 toWorld = transform.localToWorldMatrix;
			Vector3 vector = transform.InverseTransformPoint(worldCenter);
			float num = this.ToLocalRadius(worldRadius);
			Vector3 camPos = cam.transform.position;
			bool hasLiveMask;
			bool[] liveMask;
			bool useFilter;
			bool hasOverride;
			Vector3[] localNormals;
			this.ResolveSelectionFilter(vertices, includeBackFace, worldNormalsOverride, out useFilter, out localNormals, out hasOverride, out liveMask, out hasLiveMask);
			bool changed = false;
			if (this._grid != null)
			{
				this._grid.FindVerticesInRadius(vector, num, delegate(int i, float distSq)
				{
					if (hasLiveMask && !liveMask[i])
					{
						return;
					}
					if (erase && !this._selectedVertices.Contains(i))
					{
						return;
					}
					Vector3 vector3 = toWorld.MultiplyPoint3x4(vertices[i]);
					if (useFilter && !this.PassesVisibilityFilter(i, vector3, camPos, toWorld, worldNormalsOverride, hasOverride, localNormals))
					{
						return;
					}
					if (erase)
					{
						if (this._selectedVertices.Remove(i))
						{
							changed = true;
							return;
						}
					}
					else if (this._selectedVertices.Add(i))
					{
						changed = true;
					}
				});
			}
			else
			{
				float num2 = num * num;
				for (int j = 0; j < vertices.Length; j++)
				{
					if ((!hasLiveMask || liveMask[j]) && (!erase || this._selectedVertices.Contains(j)) && (vertices[j] - vector).sqrMagnitude <= num2)
					{
						Vector3 vector2 = toWorld.MultiplyPoint3x4(vertices[j]);
						if (!useFilter || this.PassesVisibilityFilter(j, vector2, camPos, toWorld, worldNormalsOverride, hasOverride, localNormals))
						{
							if (erase)
							{
								if (this._selectedVertices.Remove(j))
								{
									changed = true;
								}
							}
							else if (this._selectedVertices.Add(j))
							{
								changed = true;
							}
						}
					}
				}
			}
			return changed;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00013551 File Offset: 0x00011751
		public void ClearSelection()
		{
			this._selectedVertices.Clear();
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00013560 File Offset: 0x00011760
		private void ResolveSelectionFilter(Vector3[] vertices, bool includeBackFace, Vector3[] worldNormalsOverride, out bool useFilter, out Vector3[] localNormals, out bool hasOverride, out bool[] liveMask, out bool hasLiveMask)
		{
			useFilter = !includeBackFace;
			localNormals = ((useFilter && (worldNormalsOverride == null || worldNormalsOverride.Length != vertices.Length)) ? this._cachedNormals : null);
			hasOverride = useFilter && worldNormalsOverride != null && worldNormalsOverride.Length == vertices.Length;
			liveMask = this.VertexLiveMask;
			hasLiveMask = liveMask != null && liveMask.Length == vertices.Length;
		}

		// Token: 0x06000247 RID: 583 RVA: 0x000135C4 File Offset: 0x000117C4
		public void SelectFacesBox(Camera cam, Rect screenRect, bool additive, bool includeBackFace, Vector3[] worldNormalsOverride = null)
		{
			if (this._tempCollider == null || cam == null)
			{
				return;
			}
			Vector3[] array = this._cachedVertices ?? ((this._bakedMesh != null) ? this._bakedMesh.vertices : null);
			if (array == null || this._faceTriangles == null || this._faceCentroids == null)
			{
				return;
			}
			if (!additive)
			{
				this._selectedVertices.Clear();
			}
			Matrix4x4 localToWorldMatrix = this._tempCollider.transform.localToWorldMatrix;
			Vector3 position = cam.transform.position;
			bool flag;
			Vector3[] array2;
			bool flag2;
			bool[] array3;
			bool flag3;
			this.ResolveSelectionFilter(array, includeBackFace, worldNormalsOverride, out flag, out array2, out flag2, out array3, out flag3);
			int[] faceTriangles = this._faceTriangles;
			Vector3[] faceCentroids = this._faceCentroids;
			int num = array.Length;
			for (int i = 0; i < faceCentroids.Length; i++)
			{
				int num2 = faceTriangles[i * 3];
				int num3 = faceTriangles[i * 3 + 1];
				int num4 = faceTriangles[i * 3 + 2];
				if (num2 < num && num3 < num && num4 < num && (!flag3 || array3[num2] || array3[num3] || array3[num4]))
				{
					Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(faceCentroids[i]);
					Vector3 vector2 = cam.WorldToScreenPoint(vector);
					if (vector2.z > 0f && screenRect.Contains(new Vector2(vector2.x, vector2.y)) && (!flag || this.FacePassesVisibility(num2, num3, num4, vector, position, localToWorldMatrix, worldNormalsOverride, flag2, array2)))
					{
						this.AddFaceVertexWelded(num2);
						this.AddFaceVertexWelded(num3);
						this.AddFaceVertexWelded(num4);
					}
				}
			}
		}

		// Token: 0x06000248 RID: 584 RVA: 0x00013760 File Offset: 0x00011960
		public void DeselectFacesBox(Camera cam, Rect screenRect, bool includeBackFace, Vector3[] worldNormalsOverride = null)
		{
			if (this._tempCollider == null || cam == null)
			{
				return;
			}
			if (this._selectedVertices.Count == 0)
			{
				return;
			}
			Vector3[] array = this._cachedVertices ?? ((this._bakedMesh != null) ? this._bakedMesh.vertices : null);
			if (array == null || this._faceTriangles == null || this._faceCentroids == null)
			{
				return;
			}
			Matrix4x4 localToWorldMatrix = this._tempCollider.transform.localToWorldMatrix;
			Vector3 position = cam.transform.position;
			bool flag;
			Vector3[] array2;
			bool flag2;
			bool[] array3;
			bool flag3;
			this.ResolveSelectionFilter(array, includeBackFace, worldNormalsOverride, out flag, out array2, out flag2, out array3, out flag3);
			int[] faceTriangles = this._faceTriangles;
			Vector3[] faceCentroids = this._faceCentroids;
			int num = array.Length;
			int count = this._selectedVertices.Count;
			for (int i = 0; i < faceCentroids.Length; i++)
			{
				int num2 = faceTriangles[i * 3];
				int num3 = faceTriangles[i * 3 + 1];
				int num4 = faceTriangles[i * 3 + 2];
				if (num2 < num && num3 < num && num4 < num && (!flag3 || array3[num2] || array3[num3] || array3[num4]))
				{
					Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(faceCentroids[i]);
					Vector3 vector2 = cam.WorldToScreenPoint(vector);
					if (vector2.z > 0f && screenRect.Contains(new Vector2(vector2.x, vector2.y)) && (!flag || this.FacePassesVisibility(num2, num3, num4, vector, position, localToWorldMatrix, worldNormalsOverride, flag2, array2)))
					{
						this.RemoveFaceVertexWelded(num2);
						this.RemoveFaceVertexWelded(num3);
						this.RemoveFaceVertexWelded(num4);
					}
				}
			}
			if (this._selectedVertices.Count != count)
			{
				this.PruneOrphanFaceVertices();
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0001391C File Offset: 0x00011B1C
		public bool BrushAccumulateFaces(Camera cam, Vector3 worldCenter, float worldRadius, bool erase, bool includeBackFace, Vector3[] worldNormalsOverride = null)
		{
			if (this._tempCollider == null || cam == null)
			{
				return false;
			}
			Vector3[] array = this._cachedVertices ?? ((this._bakedMesh != null) ? this._bakedMesh.vertices : null);
			if (array == null || this._faceTriangles == null || this._faceCentroids == null)
			{
				return false;
			}
			if (erase && this._selectedVertices.Count == 0)
			{
				return false;
			}
			Transform transform = this._tempCollider.transform;
			Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
			Vector3 vector = transform.InverseTransformPoint(worldCenter);
			float num = this.ToLocalRadius(worldRadius);
			float num2 = num * num;
			Vector3 position = cam.transform.position;
			bool flag;
			Vector3[] array2;
			bool flag2;
			bool[] array3;
			bool flag3;
			this.ResolveSelectionFilter(array, includeBackFace, worldNormalsOverride, out flag, out array2, out flag2, out array3, out flag3);
			int[] faceTriangles = this._faceTriangles;
			Vector3[] faceCentroids = this._faceCentroids;
			int num3 = array.Length;
			int count = this._selectedVertices.Count;
			for (int i = 0; i < faceCentroids.Length; i++)
			{
				if ((faceCentroids[i] - vector).sqrMagnitude <= num2)
				{
					int num4 = faceTriangles[i * 3];
					int num5 = faceTriangles[i * 3 + 1];
					int num6 = faceTriangles[i * 3 + 2];
					if (num4 < num3 && num5 < num3 && num6 < num3 && (!flag3 || array3[num4] || array3[num5] || array3[num6]))
					{
						if (flag)
						{
							Vector3 vector2 = localToWorldMatrix.MultiplyPoint3x4(faceCentroids[i]);
							if (!this.FacePassesVisibility(num4, num5, num6, vector2, position, localToWorldMatrix, worldNormalsOverride, flag2, array2))
							{
								goto IL_01AA;
							}
						}
						if (erase)
						{
							this.RemoveFaceVertexWelded(num4);
							this.RemoveFaceVertexWelded(num5);
							this.RemoveFaceVertexWelded(num6);
						}
						else
						{
							this.AddFaceVertexWelded(num4);
							this.AddFaceVertexWelded(num5);
							this.AddFaceVertexWelded(num6);
						}
					}
				}
				IL_01AA:;
			}
			if (erase && this._selectedVertices.Count != count)
			{
				this.PruneOrphanFaceVertices();
			}
			return this._selectedVertices.Count != count;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x00013B10 File Offset: 0x00011D10
		private bool FacePassesVisibility(int a, int b, int c, Vector3 centroidWorld, Vector3 camPos, Matrix4x4 toWorld, Vector3[] worldNormalsOverride, bool hasOverride, Vector3[] localNormals)
		{
			Vector3 vector;
			if (hasOverride)
			{
				vector = worldNormalsOverride[a] + worldNormalsOverride[b] + worldNormalsOverride[c];
			}
			else
			{
				if (localNormals == null || a >= localNormals.Length || b >= localNormals.Length || c >= localNormals.Length)
				{
					return true;
				}
				vector = toWorld.MultiplyVector(localNormals[a] + localNormals[b] + localNormals[c]);
			}
			return Vector3.Dot(vector, camPos - centroidWorld) > 0f && !SelectionTool.IsOccluded(camPos, centroidWorld);
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00013BB4 File Offset: 0x00011DB4
		private void AddFaceVertexWelded(int idx)
		{
			if (this._grid != null && this._cachedVertices != null && idx < this._cachedVertices.Length)
			{
				this._grid.FindVerticesInRadius(this._cachedVertices[idx], 0.0002f, this._weldAddCallback);
				return;
			}
			this._selectedVertices.Add(idx);
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00013C0C File Offset: 0x00011E0C
		private void RemoveFaceVertexWelded(int idx)
		{
			if (this._grid != null && this._cachedVertices != null && idx < this._cachedVertices.Length)
			{
				this._grid.FindVerticesInRadius(this._cachedVertices[idx], 0.0002f, this._weldRemoveCallback);
				return;
			}
			this._selectedVertices.Remove(idx);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00013C64 File Offset: 0x00011E64
		private void PruneOrphanFaceVertices()
		{
			HashSet<int> selectedVertices = this._selectedVertices;
			if (selectedVertices.Count == 0)
			{
				return;
			}
			int[] faceTriangles = this._faceTriangles;
			if (faceTriangles == null)
			{
				return;
			}
			HashSet<int> orphanKeepSet = this._orphanKeepSet;
			orphanKeepSet.Clear();
			int num = faceTriangles.Length / 3;
			for (int i = 0; i < num; i++)
			{
				int num2 = faceTriangles[i * 3];
				int num3 = faceTriangles[i * 3 + 1];
				int num4 = faceTriangles[i * 3 + 2];
				if (selectedVertices.Contains(num2) && selectedVertices.Contains(num3) && selectedVertices.Contains(num4))
				{
					orphanKeepSet.Add(num2);
					orphanKeepSet.Add(num3);
					orphanKeepSet.Add(num4);
				}
			}
			this.WeldColocatedTwins(orphanKeepSet, this._keepAddCallback);
			if (orphanKeepSet.Count != selectedVertices.Count)
			{
				selectedVertices.Clear();
				foreach (int num5 in orphanKeepSet)
				{
					selectedVertices.Add(num5);
				}
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00013D6C File Offset: 0x00011F6C
		public void GrowSelection(List<int>[] adjacency)
		{
			if (adjacency == null)
			{
				return;
			}
			HashSet<int> selectedVertices = this._selectedVertices;
			if (selectedVertices.Count == 0)
			{
				return;
			}
			bool[] vertexLiveMask = this.VertexLiveMask;
			bool flag = vertexLiveMask != null && this._cachedVertices != null && vertexLiveMask.Length == this._cachedVertices.Length;
			HashSet<int> ringSet = this._ringSet;
			ringSet.Clear();
			foreach (int num in selectedVertices)
			{
				if (num < adjacency.Length)
				{
					List<int> list = adjacency[num];
					if (list != null)
					{
						for (int i = 0; i < list.Count; i++)
						{
							int num2 = list[i];
							if (num2 < adjacency.Length && (!flag || vertexLiveMask[num2]) && !selectedVertices.Contains(num2))
							{
								ringSet.Add(num2);
							}
						}
					}
				}
			}
			this.WeldColocatedTwins(ringSet, this._ringAddCallback);
			selectedVertices.UnionWith(ringSet);
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00013E64 File Offset: 0x00012064
		private void WeldColocatedTwins(HashSet<int> set, Action<int, float> addCallback)
		{
			if (set.Count == 0 || this._grid == null || this._cachedVertices == null)
			{
				return;
			}
			List<int> weldSnapshot = this._weldSnapshot;
			weldSnapshot.Clear();
			foreach (int num in set)
			{
				weldSnapshot.Add(num);
			}
			int num2 = this._cachedVertices.Length;
			for (int i = 0; i < weldSnapshot.Count; i++)
			{
				int num3 = weldSnapshot[i];
				if (num3 < num2)
				{
					this._grid.FindVerticesInRadius(this._cachedVertices[num3], 0.0002f, addCallback);
				}
			}
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00013F24 File Offset: 0x00012124
		public void ShrinkSelection(List<int>[] adjacency, bool faceMode)
		{
			if (adjacency == null)
			{
				return;
			}
			HashSet<int> selectedVertices = this._selectedVertices;
			if (selectedVertices.Count == 0)
			{
				return;
			}
			HashSet<int> ringSet = this._ringSet;
			ringSet.Clear();
			foreach (int num in selectedVertices)
			{
				if (num < adjacency.Length)
				{
					List<int> list = adjacency[num];
					if (list != null)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (!selectedVertices.Contains(list[i]))
							{
								ringSet.Add(num);
								break;
							}
						}
					}
				}
			}
			if (ringSet.Count == 0)
			{
				return;
			}
			this.WeldColocatedTwins(ringSet, this._ringAddCallback);
			selectedVertices.ExceptWith(ringSet);
			if (faceMode)
			{
				this.PruneOrphanFaceVertices();
			}
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00013FF4 File Offset: 0x000121F4
		public float CalculateFalloff(float normalizedDist)
		{
			normalizedDist = Mathf.Clamp01(normalizedDist);
			switch (this.Falloff)
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

		// Token: 0x06000252 RID: 594 RVA: 0x00014060 File Offset: 0x00012260
		public void CleanupCollider()
		{
			if (this._tempCollider != null)
			{
				UnityEngine.Object.Destroy(this._tempCollider.gameObject);
				this._tempCollider = null;
			}
			if (this._bakedMesh != null)
			{
				UnityEngine.Object.Destroy(this._bakedMesh);
				this._bakedMesh = null;
			}
			this._targetRenderer = null;
			this._targetMeshFilter = null;
			this._isStatic = false;
			this._cachedVertices = null;
			this._cachedNormals = null;
			this._grid = null;
			this._faceTriangles = null;
			this._faceCentroids = null;
			this._selectedVertices.Clear();
		}

		// Token: 0x06000253 RID: 595 RVA: 0x000140F8 File Offset: 0x000122F8
		private static void UndoBakedScale(Mesh mesh, SkinnedMeshRenderer smr)
		{
			Vector3 lossyScale = smr.transform.lossyScale;
			if (Mathf.Approximately(lossyScale.x, 1f) && Mathf.Approximately(lossyScale.y, 1f) && Mathf.Approximately(lossyScale.z, 1f))
			{
				return;
			}
			Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3[] array = vertices;
				int num = i;
				array[num].x = array[num].x / lossyScale.x;
				Vector3[] array2 = vertices;
				int num2 = i;
				array2[num2].y = array2[num2].y / lossyScale.y;
				Vector3[] array3 = vertices;
				int num3 = i;
				array3[num3].z = array3[num3].z / lossyScale.z;
			}
			mesh.vertices = vertices;
		}

		// Token: 0x0400010D RID: 269
		private HashSet<int> _selectedVertices = new HashSet<int>();

		// Token: 0x0400010E RID: 270
		private MeshCollider _tempCollider;

		// Token: 0x0400010F RID: 271
		private SkinnedMeshRenderer _targetRenderer;

		// Token: 0x04000110 RID: 272
		private MeshFilter _targetMeshFilter;

		// Token: 0x04000111 RID: 273
		private bool _isStatic;

		// Token: 0x04000112 RID: 274
		private Mesh _bakedMesh;

		// Token: 0x04000113 RID: 275
		private SpatialHashGrid _grid;

		// Token: 0x04000114 RID: 276
		private Vector3[] _cachedVertices;

		// Token: 0x04000115 RID: 277
		private Vector3[] _cachedNormals;

		// Token: 0x04000116 RID: 278
		private int[] _faceTriangles;

		// Token: 0x04000117 RID: 279
		private Vector3[] _faceCentroids;

		// Token: 0x04000118 RID: 280
		private const float ColocatedWeldTolerance = 0.0002f;

		// Token: 0x04000119 RID: 281
		private readonly Action<int, float> _weldAddCallback;

		// Token: 0x0400011A RID: 282
		private readonly Action<int, float> _weldRemoveCallback;

		// Token: 0x0400011B RID: 283
		private readonly HashSet<int> _orphanKeepSet = new HashSet<int>();

		// Token: 0x0400011C RID: 284
		private readonly Action<int, float> _keepAddCallback;

		// Token: 0x0400011D RID: 285
		private readonly HashSet<int> _ringSet = new HashSet<int>();

		// Token: 0x0400011E RID: 286
		private readonly List<int> _weldSnapshot = new List<int>();

		// Token: 0x0400011F RID: 287
		private readonly Action<int, float> _ringAddCallback;

		// Token: 0x04000120 RID: 288
		private readonly BrushResult _brushResultCache = new BrushResult
		{
			AffectedVertices = new Dictionary<int, float>()
		};

		// Token: 0x04000121 RID: 289
		public bool[] VertexLiveMask;

		// Token: 0x04000122 RID: 290
		internal const int KkseColliderLayer = 29;

		// Token: 0x04000123 RID: 291
		private static bool _layerIsolated;

		// Token: 0x04000124 RID: 292
		private static readonly int _kkseLayerMask = 536870912;
	}
}
