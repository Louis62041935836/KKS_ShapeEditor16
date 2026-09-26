using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000040 RID: 64
	[DefaultExecutionOrder(32001)]
	public class FaceSelectOverlay : MonoBehaviour
	{
		// Token: 0x060002DE RID: 734
		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out FaceSelectOverlay.POINT lpPoint);

		// Token: 0x060002DF RID: 735
		[DllImport("user32.dll")]
		private static extern bool ScreenToClient(IntPtr hWnd, ref FaceSelectOverlay.POINT lpPoint);

		// Token: 0x060002E0 RID: 736
		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int vKey);

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x00018079 File Offset: 0x00016279
		public HashSet<int> SelectedFaces
		{
			get
			{
				return this._selectedFaces;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x00018081 File Offset: 0x00016281
		public int TotalFaces
		{
			get
			{
				return this._totalFaces;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x00018089 File Offset: 0x00016289
		// (set) Token: 0x060002E4 RID: 740 RVA: 0x00018091 File Offset: 0x00016291
		public bool BoxSelectMode
		{
			get
			{
				return this._boxSelectMode;
			}
			set
			{
				this._boxSelectMode = value;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x0001809A File Offset: 0x0001629A
		// (set) Token: 0x060002E6 RID: 742 RVA: 0x000180A1 File Offset: 0x000162A1
		public float BrushRadius
		{
			get
			{
				return FaceSelectOverlay._brushRadius;
			}
			set
			{
				FaceSelectOverlay._brushRadius = value;
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x000180A9 File Offset: 0x000162A9
		public static FaceSelectOverlay Create(Renderer target)
		{
			if (target == null)
			{
				return null;
			}
			FaceSelectOverlay faceSelectOverlay = new GameObject("KKShapeEditor_FaceSelectOverlay").AddComponent<FaceSelectOverlay>();
			faceSelectOverlay._targetRenderer = target;
			faceSelectOverlay.Setup();
			return faceSelectOverlay;
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x000180D4 File Offset: 0x000162D4
		private void Setup()
		{
			this._deformer = this._targetRenderer.GetComponent<ShapeDeformer>();
			SkinnedMeshRenderer skinnedMeshRenderer = this._targetRenderer as SkinnedMeshRenderer;
			if (this._deformer == null && skinnedMeshRenderer != null)
			{
				this._deformer = this._targetRenderer.gameObject.AddComponent<ShapeDeformer>();
				this._deformer.StudioMode = ControllerResolver.Resolve(this._targetRenderer).IsOnItem;
				if (!this._deformer.Init(skinnedMeshRenderer))
				{
					UnityEngine.Object.DestroyImmediate(this._deformer);
					this._deformer = null;
				}
			}
			MeshHelper.CloneMeshIfShared(this._targetRenderer);
			SkinnedMeshRenderer skinnedMeshRenderer2 = this._targetRenderer as SkinnedMeshRenderer;
			if (skinnedMeshRenderer2 != null)
			{
				this._sourceMesh = skinnedMeshRenderer2.sharedMesh;
			}
			else
			{
				MeshFilter component = this._targetRenderer.GetComponent<MeshFilter>();
				if (component != null)
				{
					this._sourceMesh = component.sharedMesh;
				}
			}
			if (this._sourceMesh == null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			List<int> list = new List<int>();
			int num = ((this._deformer != null) ? this._deformer.OriginalSubmeshCount : 0);
			bool flag = false;
			if (this._deformer != null && num > 0 && num == this._sourceMesh.subMeshCount)
			{
				flag = true;
				for (int i = 0; i < num; i++)
				{
					int[] originalTriangles = this._deformer.GetOriginalTriangles(i);
					if (originalTriangles == null)
					{
						flag = false;
						list.Clear();
						break;
					}
					list.AddRange(originalTriangles);
				}
			}
			if (!flag)
			{
				for (int j = 0; j < this._sourceMesh.subMeshCount; j++)
				{
					list.AddRange(this._sourceMesh.GetTriangles(j));
				}
			}
			this._allTris = list.ToArray();
			this._totalFaces = this._allTris.Length / 3;
			this.CreateCollider();
			this._everRefreshed = false;
			this._wireTris = this._allTris;
			this.FindCameraControls();
			this.SetCameraCollidersEnabled(false);
			this.SetCameraEnabled(false);
			if (FaceSelectOverlay._gameWindowHandle == IntPtr.Zero)
			{
				try
				{
					FaceSelectOverlay._gameWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
				}
				catch
				{
				}
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00018304 File Offset: 0x00016504
		private void Awake()
		{
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			if (shader != null)
			{
				this._cursorMaterial = new Material(shader);
				this._cursorMaterial.SetInt("_SrcBlend", 5);
				this._cursorMaterial.SetInt("_DstBlend", 10);
				this._cursorMaterial.SetInt("_Cull", 0);
				this._cursorMaterial.SetInt("_ZWrite", 0);
				this._cursorMaterial.SetInt("_ZTest", 0);
			}
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00018388 File Offset: 0x00016588
		private void CreateCollider()
		{
			Mesh mesh = new Mesh();
			SkinnedMeshRenderer skinnedMeshRenderer = this._targetRenderer as SkinnedMeshRenderer;
			if (skinnedMeshRenderer != null)
			{
				Mesh mesh2 = ((this._deformer != null) ? this._deformer.RequestPosedMesh() : null);
				if (mesh2 != null)
				{
					mesh.vertices = mesh2.vertices;
				}
				else
				{
					if (this._bakeMeshCache == null)
					{
						this._bakeMeshCache = new Mesh();
					}
					skinnedMeshRenderer.BakeMesh(this._bakeMeshCache);
					mesh.vertices = this._bakeMeshCache.vertices;
				}
			}
			else
			{
				mesh.vertices = this._sourceMesh.vertices;
			}
			int[] allTris = this._allTris;
			int[] array = new int[allTris.Length * 2];
			Array.Copy(allTris, array, allTris.Length);
			for (int i = 0; i < allTris.Length; i += 3)
			{
				int num = allTris.Length + i;
				array[num] = allTris[i];
				array[num + 1] = allTris[i + 2];
				array[num + 2] = allTris[i + 1];
			}
			mesh.subMeshCount = 1;
			mesh.triangles = array;
			mesh.RecalculateBounds();
			this._colliderGo = new GameObject("_kkse_facesel_collider");
			SelectionTool.EnsureLayerIsolated();
			this._colliderGo.layer = 29;
			this._colliderGo.transform.SetParent(this._targetRenderer.transform, false);
			this._colliderGo.transform.localPosition = Vector3.zero;
			this._colliderGo.transform.localRotation = Quaternion.identity;
			this._colliderGo.transform.localScale = Vector3.one;
			this._collider = this._colliderGo.AddComponent<MeshCollider>();
			this._collider.sharedMesh = mesh;
			this._cachedLocalVerts = mesh.vertices;
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00018540 File Offset: 0x00016740
		private void DrawDeletedFaces(Vector3[] verts)
		{
			if (this._allTris == null)
			{
				return;
			}
			DeformData deformData = ControllerResolver.Resolve(this._targetRenderer).GetDeformData(this._targetRenderer);
			if (deformData == null || deformData.DeletedFaces.Count == 0)
			{
				return;
			}
			GL.Begin(4);
			GL.Color(FaceSelectOverlay.DeletedFaceColor);
			foreach (int num in deformData.DeletedFaces)
			{
				int num2 = num * 3;
				if (num2 + 2 < this._allTris.Length)
				{
					int num3 = this._allTris[num2];
					int num4 = this._allTris[num2 + 1];
					int num5 = this._allTris[num2 + 2];
					if (num3 < verts.Length && num4 < verts.Length && num5 < verts.Length)
					{
						GL.Vertex(verts[num3]);
						GL.Vertex(verts[num4]);
						GL.Vertex(verts[num5]);
					}
				}
			}
			GL.End();
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00018644 File Offset: 0x00016844
		private void DrawSelectedFaces(Vector3[] verts)
		{
			if (this._selectedFaces.Count == 0 || this._allTris == null)
			{
				return;
			}
			GL.Begin(4);
			GL.Color(FaceSelectOverlay.SelectedColor);
			foreach (int num in this._selectedFaces)
			{
				int num2 = num * 3;
				if (num2 + 2 < this._allTris.Length)
				{
					int num3 = this._allTris[num2];
					int num4 = this._allTris[num2 + 1];
					int num5 = this._allTris[num2 + 2];
					if (num3 < verts.Length && num4 < verts.Length && num5 < verts.Length)
					{
						GL.Vertex(verts[num3]);
						GL.Vertex(verts[num4]);
						GL.Vertex(verts[num5]);
					}
				}
			}
			GL.End();
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00018724 File Offset: 0x00016924
		private void Update()
		{
			if (this._targetRenderer == null)
			{
				return;
			}
			this._cachedMouseButton = ((int)FaceSelectOverlay.GetAsyncKeyState(1) & 32768) != 0;
			this._cachedCtrlHeld = ((int)FaceSelectOverlay.GetAsyncKeyState(162) & 32768) != 0 || ((int)FaceSelectOverlay.GetAsyncKeyState(163) & 32768) != 0;
			this._cachedShiftHeld = ((int)FaceSelectOverlay.GetAsyncKeyState(160) & 32768) != 0 || ((int)FaceSelectOverlay.GetAsyncKeyState(161) & 32768) != 0;
			this._cachedAltHeld = ((int)FaceSelectOverlay.GetAsyncKeyState(164) & 32768) != 0 || ((int)FaceSelectOverlay.GetAsyncKeyState(165) & 32768) != 0;
			this._cachedBracketLeft = ((int)FaceSelectOverlay.GetAsyncKeyState(219) & 32768) != 0;
			this._cachedBracketRight = ((int)FaceSelectOverlay.GetAsyncKeyState(221) & 32768) != 0;
			if (this._cachedCtrlHeld != this._cameraEnabled)
			{
				this.SetCameraEnabled(this._cachedCtrlHeld);
			}
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0001882C File Offset: 0x00016A2C
		private void LateUpdate()
		{
			if (this._targetRenderer == null || this._collider == null)
			{
				return;
			}
			int num = this._bracketRepeater.Tick(this._cachedBracketLeft, this._cachedBracketRight, Time.unscaledTime);
			if (num != 0 && !this._cachedShiftHeld)
			{
				FaceSelectOverlay._brushRadius = Mathf.Clamp(FaceSelectOverlay._brushRadius + (float)num * 0.005f, 0.001f, ShapeEditorPlugin.MaxBrushRadius.Value);
			}
			this.RefreshCollider();
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			Vector3 realMousePosition = this.GetRealMousePosition();
			if (!this._cachedCtrlHeld && !ShapeEditorWindow.IsScreenPointOverUI(realMousePosition))
			{
				if (this._boxSelectMode)
				{
					this.HandleBoxSelect(realMousePosition, main);
				}
				else
				{
					this.HandleBrushSelect(realMousePosition, main);
				}
			}
			this._wasMouseDown = this._cachedMouseButton;
		}

		// Token: 0x060002EF RID: 751 RVA: 0x000188F8 File Offset: 0x00016AF8
		private void HandleBrushSelect(Vector3 mousePos, Camera cam)
		{
			try
			{
				Ray ray = cam.ScreenPointToRay(mousePos);
				RaycastHit raycastHit;
				this._hasHit = this._collider.Raycast(ray, out raycastHit, 1000f);
				if (this._hasHit)
				{
					this._lastHitPoint = raycastHit.point;
					this._lastHitNormal = raycastHit.normal;
					if (this._cachedMouseButton)
					{
						bool flag = !ShapeEditorPlugin.FaceSelectIncludeBackFace.Value;
						Vector3 position = cam.transform.position;
						Matrix4x4 localToWorldMatrix = this._collider.transform.localToWorldMatrix;
						int num = raycastHit.triangleIndex;
						if (num >= this._totalFaces)
						{
							num -= this._totalFaces;
						}
						if (num >= 0 && num < this._totalFaces && (!flag || !this.IsFaceBackFacing(num, this._cachedLocalVerts, localToWorldMatrix, position)))
						{
							if (this._cachedAltHeld)
							{
								this._selectedFaces.Remove(num);
							}
							else
							{
								this._selectedFaces.Add(num);
							}
						}
						if (this._cachedLocalVerts != null)
						{
							Vector3 vector = this._collider.transform.InverseTransformPoint(raycastHit.point);
							Vector3[] cachedLocalVerts = this._cachedLocalVerts;
							float num2 = FaceSelectOverlay._brushRadius * FaceSelectOverlay._brushRadius;
							for (int i = 0; i < this._totalFaces; i++)
							{
								int num3 = i * 3;
								if (((cachedLocalVerts[this._allTris[num3]] + cachedLocalVerts[this._allTris[num3 + 1]] + cachedLocalVerts[this._allTris[num3 + 2]]) / 3f - vector).sqrMagnitude <= num2 && (!flag || !this.IsFaceBackFacing(i, cachedLocalVerts, localToWorldMatrix, position)))
								{
									if (this._cachedAltHeld)
									{
										this._selectedFaces.Remove(i);
									}
									else
									{
										this._selectedFaces.Add(i);
									}
								}
							}
						}
					}
				}
				else
				{
					this._lastHitPoint = Vector3.zero;
					this._lastHitNormal = Vector3.up;
				}
			}
			finally
			{
			}
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00018B10 File Offset: 0x00016D10
		private void HandleBoxSelect(Vector3 mousePos, Camera cam)
		{
			if (this._cachedMouseButton && !this._wasMouseDown)
			{
				this._boxStart = new Vector2(mousePos.x, mousePos.y);
				this._boxEnd = this._boxStart;
				this._isBoxSelecting = true;
			}
			else if (this._cachedMouseButton && this._isBoxSelecting)
			{
				this._boxEnd = new Vector2(mousePos.x, mousePos.y);
			}
			else if (!this._cachedMouseButton && this._isBoxSelecting)
			{
				this._isBoxSelecting = false;
				float num = Mathf.Min(this._boxStart.x, this._boxEnd.x);
				float num2 = Mathf.Max(this._boxStart.x, this._boxEnd.x);
				float num3 = Mathf.Min(this._boxStart.y, this._boxEnd.y);
				float num4 = Mathf.Max(this._boxStart.y, this._boxEnd.y);
				if (num2 - num > 2f && num4 - num3 > 2f)
				{
					Rect rect;
					rect = new Rect(num, num3, num2 - num, num4 - num3);
					this.SelectFacesInRect(cam, rect);
				}
			}
			this._hasHit = false;
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00018C48 File Offset: 0x00016E48
		private void SelectFacesInRect(Camera cam, Rect rect)
		{
			Matrix4x4 localToWorldMatrix = this._collider.transform.localToWorldMatrix;
			Vector3[] cachedLocalVerts = this._cachedLocalVerts;
			if (cachedLocalVerts == null)
			{
				return;
			}
			bool cachedShiftHeld = this._cachedShiftHeld;
			bool cachedAltHeld = this._cachedAltHeld;
			bool flag = !ShapeEditorPlugin.FaceSelectIncludeBackFace.Value;
			Vector3 position = cam.transform.position;
			if (!cachedShiftHeld && !cachedAltHeld)
			{
				this._selectedFaces.Clear();
			}
			for (int i = 0; i < this._allTris.Length / 3; i++)
			{
				int num = i * 3;
				Vector3 vector = (cachedLocalVerts[this._allTris[num]] + cachedLocalVerts[this._allTris[num + 1]] + cachedLocalVerts[this._allTris[num + 2]]) / 3f;
				Vector3 vector2 = localToWorldMatrix.MultiplyPoint3x4(vector);
				Vector3 vector3 = cam.WorldToScreenPoint(vector2);
				if (vector3.z > 0f && (!flag || !this.IsFaceBackFacing(i, cachedLocalVerts, localToWorldMatrix, position)) && rect.Contains(new Vector2(vector3.x, vector3.y)))
				{
					if (cachedAltHeld)
					{
						this._selectedFaces.Remove(i);
					}
					else
					{
						this._selectedFaces.Add(i);
					}
				}
			}
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00018D88 File Offset: 0x00016F88
		private bool IsFaceBackFacing(int face, Vector3[] localVerts, Matrix4x4 localToWorld, Vector3 camPos)
		{
			int num = face * 3;
			if (localVerts == null || num + 2 >= this._allTris.Length)
			{
				return false;
			}
			int num2 = this._allTris[num];
			int num3 = this._allTris[num + 1];
			int num4 = this._allTris[num + 2];
			if (num2 >= localVerts.Length || num3 >= localVerts.Length || num4 >= localVerts.Length)
			{
				return false;
			}
			Vector3 vector = localToWorld.MultiplyPoint3x4(localVerts[num2]);
			Vector3 vector2 = localToWorld.MultiplyPoint3x4(localVerts[num3]);
			Vector3 vector3 = localToWorld.MultiplyPoint3x4(localVerts[num4]);
			return Vector3.Dot(Vector3.Cross(vector2 - vector, vector3 - vector), vector - camPos) > 0f;
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00018E38 File Offset: 0x00017038
		public void SelectAll()
		{
			this._selectedFaces.Clear();
			for (int i = 0; i < this._totalFaces; i++)
			{
				this._selectedFaces.Add(i);
			}
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00018E6E File Offset: 0x0001706E
		public void ClearSelection()
		{
			this._selectedFaces.Clear();
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00018E7C File Offset: 0x0001707C
		public void InvertSelection()
		{
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i < this._totalFaces; i++)
			{
				if (!this._selectedFaces.Contains(i))
				{
					hashSet.Add(i);
				}
			}
			this._selectedFaces = hashSet;
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00018EC0 File Offset: 0x000170C0
		private void OnRenderObject()
		{
			if (Camera.current != Camera.main)
			{
				return;
			}
			if (this._cursorMaterial == null || this._targetRenderer == null)
			{
				return;
			}
			Mesh mesh = ((this._deformer != null) ? this._deformer.RequestPosedMesh() : null);
			Mesh mesh2;
			Vector3[] array;
			Matrix4x4 matrix4x;
			if (mesh != null && this._deformer.DisplayTransform != null)
			{
				mesh2 = mesh;
				array = mesh.vertices;
				matrix4x = this._deformer.DisplayTransform.localToWorldMatrix;
			}
			else
			{
				SkinnedMeshRenderer skinnedMeshRenderer = this._targetRenderer as SkinnedMeshRenderer;
				if (skinnedMeshRenderer != null)
				{
					if (skinnedMeshRenderer.sharedMesh == null)
					{
						return;
					}
					if (this._bakeMeshCache == null)
					{
						this._bakeMeshCache = new Mesh();
					}
					skinnedMeshRenderer.BakeMesh(this._bakeMeshCache);
					mesh2 = this._bakeMeshCache;
					array = this._bakeMeshCache.vertices;
					Transform transform = this._targetRenderer.transform;
					matrix4x = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
				}
				else
				{
					MeshFilter component = this._targetRenderer.GetComponent<MeshFilter>();
					if (component == null || component.sharedMesh == null)
					{
						return;
					}
					mesh2 = component.sharedMesh;
					array = component.sharedMesh.vertices;
					matrix4x = this._targetRenderer.localToWorldMatrix;
				}
			}
			if (this._wireTris == null)
			{
				return;
			}
			if (ShapeEditorPlugin.FaceWireframeOpacity.Value > 0f)
			{
				this.RefreshWireBundle(mesh2, array, matrix4x);
				this.DrawWireframeBundle();
			}
			else
			{
				this._lastWireOpacity = 0f;
			}
			this._cursorMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(matrix4x);
			this.DrawDeletedFaces(array);
			this.DrawSelectedFaces(array);
			GL.PopMatrix();
			this._cursorMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(Matrix4x4.identity);
			if (!this._boxSelectMode && this._hasHit)
			{
				this.DrawBrushCircle(this._lastHitPoint, this._lastHitNormal, FaceSelectOverlay._brushRadius);
			}
			if (this._isBoxSelecting && this._boxSelectMode)
			{
				this.DrawBoxSelectRect();
			}
			GL.PopMatrix();
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x000190E8 File Offset: 0x000172E8
		private void RefreshWireBundle(Mesh sourceMesh, Vector3[] localVerts, Matrix4x4 matrix)
		{
			int num = localVerts.Length;
			if (this._wireBundle.Verts == null || this._wireBundle.Verts.Length != num)
			{
				this._wireBundle.Verts = new Vector3[num];
			}
			for (int i = 0; i < num; i++)
			{
				this._wireBundle.Verts[i] = matrix.MultiplyPoint3x4(localVerts[i]);
			}
			int instanceID = sourceMesh.GetInstanceID();
			if (this._wireBundle.Tris == null || this._wireBundle.MeshId != instanceID)
			{
				this._wireBundle.Tris = this._wireTris;
				this._wireBundle.MeshId = instanceID;
				ShapePaintOverlay.ExtractUniqueEdgesIntoBundle(this._wireBundle);
				if (this._wireBundle.LineMesh != null)
				{
					UnityEngine.Object.Destroy(this._wireBundle.LineMesh);
				}
				this._wireBundle.LineMesh = new Mesh();
				this._wireBundle.LineMesh.MarkDynamic();
				this._wireBundle.PrevVisibleCount = -1;
				this._wireBundle.ColorsDirty = true;
				this._wireBundle.EverDrawn = false;
				this._wireBundle.ColorsUploaded = false;
				this._wireBundle.TriFacing = null;
			}
			float value = ShapeEditorPlugin.FaceWireframeOpacity.Value;
			if (value != this._lastWireOpacity)
			{
				this._wireBundle.ColorsDirty = true;
				this._wireBundle.ColorsUploaded = false;
				this._wireBundle.EverDrawn = false;
				this._lastWireOpacity = value;
			}
			if (this._wireBundle.Colors == null || this._wireBundle.Colors.Length != num)
			{
				this._wireBundle.Colors = new Color32[num];
				this._wireBundle.ColorsDirty = true;
				this._wireBundle.ColorsUploaded = false;
			}
			if (this._wireBundle.ColorsDirty)
			{
				Color32 wireDefaultColor = ShapePaintOverlay.WireDefaultColor32;
				wireDefaultColor.a = (byte)Mathf.RoundToInt((float)wireDefaultColor.a * value);
				for (int j = 0; j < num; j++)
				{
					this._wireBundle.Colors[j] = wireDefaultColor;
				}
				this._wireBundle.ColorsDirty = false;
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00019300 File Offset: 0x00017500
		private void DrawWireframeBundle()
		{
			try
			{
				Camera main = Camera.main;
				if (!(main == null))
				{
					if (this._wireBundle.Edges != null && !(this._wireBundle.LineMesh == null) && this._wireBundle.Verts != null && this._wireBundle.Tris != null)
					{
						Transform transform = main.transform;
						Vector3 position = transform.position;
						int num = position.GetHashCode() ^ transform.forward.GetHashCode();
						int num2 = ((this._deformer != null) ? this._deformer.LastVertsHash : 0);
						int num3 = ((this._deformer != null && this._deformer.DisplayTransform != null) ? ShapeDeformer.HashMatrix4x4(this._deformer.DisplayTransform.localToWorldMatrix) : 0);
						int num4 = num2 ^ num3;
						if (this._wireBundle.EverDrawn && this._wireBundle.LastDrawVertsHash == num4 && this._wireBundle.LastDrawCamHash == num && this._wireBundle.Colors != null && this._wireBundle.Colors.Length == this._wireBundle.Verts.Length)
						{
							this._cursorMaterial.SetPass(0);
							Graphics.DrawMeshNow(this._wireBundle.LineMesh, Matrix4x4.identity);
						}
						else
						{
							int[] tris = this._wireBundle.Tris;
							Vector3[] verts = this._wireBundle.Verts;
							int num5 = tris.Length / 3;
							if (this._wireBundle.TriFacing == null || this._wireBundle.TriFacing.Length < num5)
							{
								this._wireBundle.TriFacing = new bool[num5];
							}
							bool[] triFacing = this._wireBundle.TriFacing;
							for (int i = 0; i < num5; i++)
							{
								int num6 = i * 3;
								Vector3 vector = verts[tris[num6]];
								Vector3 vector2 = verts[tris[num6 + 1]];
								Vector3 vector3 = verts[tris[num6 + 2]];
								Vector3 vector4 = Vector3.Cross(vector2 - vector, vector3 - vector);
								triFacing[i] = Vector3.Dot(vector4, vector - position) <= 0f;
							}
							int num7 = this._wireBundle.Edges.Length;
							int num8 = 0;
							if (this._wireBundle.LineIndexBuffer == null || this._wireBundle.LineIndexBuffer.Length < num7 * 2)
							{
								this._wireBundle.LineIndexBuffer = new int[num7 * 2];
							}
							for (int j = 0; j < num7; j++)
							{
								int tri = this._wireBundle.Edges[j].tri0;
								int tri2 = this._wireBundle.Edges[j].tri1;
								bool flag = triFacing[tri];
								bool flag2 = tri2 >= 0 && triFacing[tri2];
								if (flag || flag2)
								{
									this._wireBundle.LineIndexBuffer[num8++] = this._wireBundle.Edges[j].v0;
									this._wireBundle.LineIndexBuffer[num8++] = this._wireBundle.Edges[j].v1;
								}
							}
							this._wireBundle.LineMesh.vertices = this._wireBundle.Verts;
							if (!this._wireBundle.ColorsUploaded)
							{
								this._wireBundle.LineMesh.colors32 = this._wireBundle.Colors;
								this._wireBundle.ColorsUploaded = true;
							}
							if (num8 != this._wireBundle.PrevVisibleCount)
							{
								this._wireBundle.VisibleLineIndices = new int[num8];
								this._wireBundle.PrevVisibleCount = num8;
							}
							Array.Copy(this._wireBundle.LineIndexBuffer, this._wireBundle.VisibleLineIndices, num8);
							this._wireBundle.LineMesh.SetIndices(this._wireBundle.VisibleLineIndices, UnityEngine.MeshTopology.Lines, 0);
							this._cursorMaterial.SetPass(0);
							Graphics.DrawMeshNow(this._wireBundle.LineMesh, Matrix4x4.identity);
							this._wireBundle.LastDrawVertsHash = num4;
							this._wireBundle.LastDrawCamHash = num;
							this._wireBundle.EverDrawn = true;
						}
					}
				}
			}
			finally
			{
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00019764 File Offset: 0x00017964
		private void DrawBrushCircle(Vector3 center, Vector3 normal, float radius)
		{
			if (normal.sqrMagnitude < 0.001f)
			{
				normal = Vector3.up;
			}
			normal.Normalize();
			Vector3 vector = Vector3.Cross(normal, Vector3.up);
			if (vector.sqrMagnitude < 0.001f)
			{
				vector = Vector3.Cross(normal, Vector3.right);
			}
			vector.Normalize();
			Vector3 vector2 = Vector3.Cross(normal, vector);
			GL.Begin(1);
			GL.Color(new Color(1f, 0.8f, 0f, 0.9f));
			for (int i = 0; i < 32; i++)
			{
				float num = (float)i / 32f * 3.1415927f * 2f;
				float num2 = (float)(i + 1) / 32f * 3.1415927f * 2f;
				Vector3 vector3 = center + (vector * Mathf.Cos(num) + vector2 * Mathf.Sin(num)) * radius;
				Vector3 vector4 = center + (vector * Mathf.Cos(num2) + vector2 * Mathf.Sin(num2)) * radius;
				GL.Vertex(vector3);
				GL.Vertex(vector4);
			}
			GL.End();
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00019890 File Offset: 0x00017A90
		private void DrawBoxSelectRect()
		{
			GL.PushMatrix();
			GL.LoadPixelMatrix();
			float num = Mathf.Min(this._boxStart.x, this._boxEnd.x);
			float num2 = Mathf.Max(this._boxStart.x, this._boxEnd.x);
			float num3 = Mathf.Min(this._boxStart.y, this._boxEnd.y);
			float num4 = Mathf.Max(this._boxStart.y, this._boxEnd.y);
			GL.Begin(7);
			GL.Color(new Color(0.2f, 0.8f, 0.3f, 0.15f));
			GL.Vertex3(num, num3, 0f);
			GL.Vertex3(num2, num3, 0f);
			GL.Vertex3(num2, num4, 0f);
			GL.Vertex3(num, num4, 0f);
			GL.End();
			GL.Begin(1);
			GL.Color(new Color(0.2f, 0.8f, 0.3f, 0.8f));
			GL.Vertex3(num, num3, 0f);
			GL.Vertex3(num2, num3, 0f);
			GL.Vertex3(num2, num3, 0f);
			GL.Vertex3(num2, num4, 0f);
			GL.Vertex3(num2, num4, 0f);
			GL.Vertex3(num, num4, 0f);
			GL.Vertex3(num, num4, 0f);
			GL.Vertex3(num, num3, 0f);
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x060002FB RID: 763 RVA: 0x000199FC File Offset: 0x00017BFC
		private void OnGUI()
		{
			if (this._hudStyle == null)
			{
				this._hudStyle = new GUIStyle(GUI.skin.box);
				this._hudStyle.alignment = (TextAnchor)4;
				this._hudStyle.fontSize = 14;
				this._hudStyle.normal.textColor = Color.green;
			}
			string text = (this._cameraEnabled ? L.CameraMode : (this._boxSelectMode ? L.FaceSelectBox : L.FaceSelectBrush));
			GUI.Box(new Rect((float)(Screen.width / 2 - 100), 10f, 200f, 30f), text, this._hudStyle);
			string text2 = string.Format(L.SelectedFacesFmt, this._selectedFaces.Count, this._totalFaces);
			if (!this._boxSelectMode)
			{
				text2 += string.Format(L.RadiusSuffixFmt, FaceSelectOverlay._brushRadius.ToString("F3"));
			}
			GUI.Box(new Rect((float)(Screen.width / 2 - 150), 45f, 300f, 25f), text2, this._hudStyle);
			if (!this._cachedCtrlHeld)
			{
				Input.ResetInputAxes();
			}
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00019B2C File Offset: 0x00017D2C
		private void RefreshCollider()
		{
			try
			{
				Mesh mesh = ((this._collider != null) ? this._collider.sharedMesh : null);
				if (!(mesh == null))
				{
					Mesh mesh2 = ((this._deformer != null) ? this._deformer.RequestPosedMesh() : null);
					if (mesh2 != null)
					{
						this._refreshTimer = 0f;
						int lastVertsHash = this._deformer.LastVertsHash;
						int instanceID = mesh2.GetInstanceID();
						if (!this._everRefreshed || lastVertsHash != this._lastSkinningHash || instanceID != this._lastMeshId)
						{
							Vector3[] vertices = mesh2.vertices;
							if (vertices.Length == this._cachedLocalVerts.Length)
							{
								mesh.vertices = vertices;
								mesh.RecalculateBounds();
								this._collider.sharedMesh = null;
								this._collider.sharedMesh = mesh;
								this._cachedLocalVerts = vertices;
								this._lastSkinningHash = lastVertsHash;
								this._lastMeshId = instanceID;
								this._everRefreshed = true;
							}
						}
					}
					else
					{
						this._refreshTimer += Time.deltaTime;
						if (this._refreshTimer >= 0.5f)
						{
							this._refreshTimer = 0f;
							Vector3[] array = null;
							SkinnedMeshRenderer skinnedMeshRenderer = this._targetRenderer as SkinnedMeshRenderer;
							if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
							{
								if (this._bakeMeshCache == null)
								{
									this._bakeMeshCache = new Mesh();
								}
								skinnedMeshRenderer.BakeMesh(this._bakeMeshCache);
								array = this._bakeMeshCache.vertices;
							}
							if (array != null && array.Length == this._cachedLocalVerts.Length)
							{
								mesh.vertices = array;
								mesh.RecalculateBounds();
								this._collider.sharedMesh = null;
								this._collider.sharedMesh = mesh;
								this._cachedLocalVerts = array;
							}
							this._everRefreshed = false;
						}
					}
				}
			}
			finally
			{
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00019D24 File Offset: 0x00017F24
		private Vector3 GetRealMousePosition()
		{
			FaceSelectOverlay.POINT point;
			if (FaceSelectOverlay._gameWindowHandle != IntPtr.Zero && FaceSelectOverlay.GetCursorPos(out point))
			{
				FaceSelectOverlay.ScreenToClient(FaceSelectOverlay._gameWindowHandle, ref point);
				return new Vector3((float)point.X, (float)(Screen.height - point.Y), 0f);
			}
			return Input.mousePosition;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00019D7C File Offset: 0x00017F7C
		private void FindCameraControls()
		{
			CameraControlResolver.Resolve(this._cameraScripts, this._cameraColliders);
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00019D90 File Offset: 0x00017F90
		private void SetCameraEnabled(bool enabled)
		{
			this._cameraEnabled = enabled;
			foreach (MonoBehaviour monoBehaviour in this._cameraScripts)
			{
				if (monoBehaviour != null)
				{
					monoBehaviour.enabled = enabled;
				}
			}
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00019DF4 File Offset: 0x00017FF4
		private void SetCameraCollidersEnabled(bool enabled)
		{
			foreach (Collider collider in this._cameraColliders)
			{
				if (collider != null)
				{
					collider.enabled = enabled;
				}
			}
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00019E50 File Offset: 0x00018050
		private void OnDestroy()
		{
			this.SetCameraEnabled(true);
			this.SetCameraCollidersEnabled(true);
			if (this._colliderGo != null)
			{
				UnityEngine.Object.DestroyImmediate(this._colliderGo);
			}
			if (this._wireBundle != null && this._wireBundle.LineMesh != null)
			{
				UnityEngine.Object.Destroy(this._wireBundle.LineMesh);
			}
			if (this._bakeMeshCache != null)
			{
				UnityEngine.Object.Destroy(this._bakeMeshCache);
			}
			if (this._cursorMaterial != null)
			{
				UnityEngine.Object.Destroy(this._cursorMaterial);
			}
		}

		// Token: 0x0400018D RID: 397
		private Renderer _targetRenderer;

		// Token: 0x0400018E RID: 398
		private ShapeDeformer _deformer;

		// Token: 0x0400018F RID: 399
		private Mesh _sourceMesh;

		// Token: 0x04000190 RID: 400
		private MeshCollider _collider;

		// Token: 0x04000191 RID: 401
		private GameObject _colliderGo;

		// Token: 0x04000192 RID: 402
		private int[] _allTris;

		// Token: 0x04000193 RID: 403
		private HashSet<int> _selectedFaces = new HashSet<int>();

		// Token: 0x04000194 RID: 404
		private int _totalFaces;

		// Token: 0x04000195 RID: 405
		private bool _boxSelectMode;

		// Token: 0x04000196 RID: 406
		private static float _brushRadius = 0.1f;

		// Token: 0x04000197 RID: 407
		private bool _isBoxSelecting;

		// Token: 0x04000198 RID: 408
		private Vector2 _boxStart;

		// Token: 0x04000199 RID: 409
		private Vector2 _boxEnd;

		// Token: 0x0400019A RID: 410
		private bool _wasMouseDown;

		// Token: 0x0400019B RID: 411
		private Material _cursorMaterial;

		// Token: 0x0400019C RID: 412
		private Vector3 _lastHitPoint;

		// Token: 0x0400019D RID: 413
		private Vector3 _lastHitNormal;

		// Token: 0x0400019E RID: 414
		private bool _hasHit;

		// Token: 0x0400019F RID: 415
		private const int CursorSegments = 32;

		// Token: 0x040001A0 RID: 416
		private readonly List<MonoBehaviour> _cameraScripts = new List<MonoBehaviour>();

		// Token: 0x040001A1 RID: 417
		private readonly List<Collider> _cameraColliders = new List<Collider>();

		// Token: 0x040001A2 RID: 418
		private bool _cameraEnabled;

		// Token: 0x040001A3 RID: 419
		private const float ColliderRefreshInterval = 0.5f;

		// Token: 0x040001A4 RID: 420
		private float _refreshTimer;

		// Token: 0x040001A5 RID: 421
		private int _lastSkinningHash;

		// Token: 0x040001A6 RID: 422
		private int _lastMeshId;

		// Token: 0x040001A7 RID: 423
		private bool _everRefreshed;

		// Token: 0x040001A8 RID: 424
		private int[] _wireTris;

		// Token: 0x040001A9 RID: 425
		private Vector3[] _cachedLocalVerts;

		// Token: 0x040001AA RID: 426
		private Mesh _bakeMeshCache;

		// Token: 0x040001AB RID: 427
		private readonly WireBundle _wireBundle = new WireBundle();

		// Token: 0x040001AC RID: 428
		private float _lastWireOpacity = -1f;

		// Token: 0x040001AD RID: 429
		private bool _cachedMouseButton;

		// Token: 0x040001AE RID: 430
		private bool _cachedCtrlHeld;

		// Token: 0x040001AF RID: 431
		private bool _cachedShiftHeld;

		// Token: 0x040001B0 RID: 432
		private bool _cachedAltHeld;

		// Token: 0x040001B1 RID: 433
		private bool _cachedBracketLeft;

		// Token: 0x040001B2 RID: 434
		private bool _cachedBracketRight;

		// Token: 0x040001B3 RID: 435
		private BracketStepRepeater _bracketRepeater;

		// Token: 0x040001B4 RID: 436
		private const int VK_LBUTTON = 1;

		// Token: 0x040001B5 RID: 437
		private const int VK_LCONTROL = 162;

		// Token: 0x040001B6 RID: 438
		private const int VK_RCONTROL = 163;

		// Token: 0x040001B7 RID: 439
		private const int VK_LSHIFT = 160;

		// Token: 0x040001B8 RID: 440
		private const int VK_RSHIFT = 161;

		// Token: 0x040001B9 RID: 441
		private const int VK_LMENU = 164;

		// Token: 0x040001BA RID: 442
		private const int VK_RMENU = 165;

		// Token: 0x040001BB RID: 443
		private const int VK_OEM_4 = 219;

		// Token: 0x040001BC RID: 444
		private const int VK_OEM_6 = 221;

		// Token: 0x040001BD RID: 445
		private static IntPtr _gameWindowHandle;

		// Token: 0x040001BE RID: 446
		private GUIStyle _hudStyle;

		// Token: 0x040001BF RID: 447
		private static readonly Color SelectedColor = new Color(0.2f, 1f, 0.3f, 0.35f);

		// Token: 0x040001C0 RID: 448
		private static readonly Color DeletedFaceColor = new Color(1f, 0.2f, 0.2f, 0.45f);

		// Token: 0x040001C1 RID: 449
		public Func<bool> OnBeforeSubdivide;

		// Token: 0x0200006B RID: 107
		private struct POINT
		{
			// Token: 0x040004CB RID: 1227
			public int X;

			// Token: 0x040004CC RID: 1228
			public int Y;
		}
	}
}
