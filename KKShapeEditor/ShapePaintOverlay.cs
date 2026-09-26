using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using KKAPI;
using KKAPI.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace KKShapeEditor
{
	// Token: 0x02000047 RID: 71
	[DefaultExecutionOrder(32001)]
	public class ShapePaintOverlay : MonoBehaviour
	{
		// Token: 0x060003CD RID: 973 RVA: 0x00024160 File Offset: 0x00022360
		private bool MultiRaycast(Ray ray, out Vector3 hitPoint, out Vector3 hitNormal)
		{
			hitPoint = Vector3.zero;
			hitNormal = Vector3.up;
			float num = float.MaxValue;
			bool flag = false;
			for (int i = 0; i < this._activeSelTools.Count; i++)
			{
				SelectionTool selectionTool = this._activeSelTools[i];
				Vector3 vector;
				Vector3 vector2;
				if (selectionTool != null && selectionTool.Raycast(ray, out vector, out vector2))
				{
					float sqrMagnitude = (vector - ray.origin).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						hitPoint = vector;
						hitNormal = vector2;
						flag = true;
					}
				}
			}
			return flag;
		}

		// Token: 0x060003CE RID: 974 RVA: 0x000241F4 File Offset: 0x000223F4
		private void ProcessBrushInteraction(Camera cam, Ray ray, Vector3 mousePos, DeformLayer primaryActiveLayer)
		{
			if (primaryActiveLayer == null || !this.Input.MouseButton || !this._hasHit || this.Input.CtrlHeld)
			{
				return;
			}
			if (this._activeRenderers.Count == 0 || this._primaryListIdx < 0)
			{
				return;
			}
			if (!this._isBrushing)
			{
				this._isBrushing = true;
				this._strokeStates = new List<ShapePaintOverlay.StrokeRendererState>(this._activeRenderers.Count);
				this._currentBrushEntry = new MultiDeltaUndoEntry();
				this._strokeStartHitPoint = this._lastHitPoint;
				this._strokeStartHitNormal = this._lastHitNormal;
				this._lastValidHitPoint = this._lastHitPoint;
				this._lastValidHitNormal = this._lastHitNormal;
				string name = primaryActiveLayer.Name;
				for (int i = 0; i < this._activeRenderers.Count; i++)
				{
					ShapePaintOverlay.StrokeRendererState strokeRendererState = new ShapePaintOverlay.StrokeRendererState
					{
						Renderer = this._activeRenderers[i],
						Deformer = this._activeDeformers[i],
						SelTool = this._activeSelTools[i],
						SmoothTool = ((i < this._activeSmoothTools.Count) ? this._activeSmoothTools[i] : null)
					};
					strokeRendererState.Data = ShapePaintOverlay.GetExistingDeformData(strokeRendererState.Renderer);
					if (strokeRendererState.Data != null)
					{
						strokeRendererState.Layer = ShapePaintOverlay.FindLayerByName(strokeRendererState.Data, name);
						if (strokeRendererState.Layer == null && i != this._primaryListIdx)
						{
							Mesh mesh = MeshHelper.GetMesh(strokeRendererState.Renderer);
							if (mesh == null)
							{
								goto IL_0215;
							}
							DeformLayer deformLayer = new DeformLayer(name, mesh.vertexCount);
							strokeRendererState.Data.Layers.Add(deformLayer);
							strokeRendererState.LazyCreated = true;
							strokeRendererState.LazyCreatedIdx = strokeRendererState.Data.Layers.Count - 1;
							strokeRendererState.Layer = deformLayer;
							this._currentBrushEntry.Add(new LayerAddUndoEntry(strokeRendererState.Data, deformLayer, strokeRendererState.LazyCreatedIdx));
						}
						if (strokeRendererState.Layer != null)
						{
							this._currentBrushEntry.TrackDeformer(strokeRendererState.Deformer);
							this._strokeStates.Add(strokeRendererState);
						}
					}
					IL_0215:;
				}
			}
			if (this._strokeStates == null || this._strokeStates.Count == 0)
			{
				return;
			}
			if (this.Window.SelectedBrushTool ==
				ShapeEditorWindow.BrushToolType.Seamless)
			{
				this.ProcessSeamlessInteraction(
					cam,
					ray,
					mousePos,
					primaryActiveLayer);

				return;
			}
			IDeformTool activeBrushTool = this.GetActiveBrushTool();
			if (activeBrushTool == null)
			{
				return;
			}
			float num = mousePos.x - this._prevMousePos.x;
			float num2 = mousePos.y - this._prevMousePos.y;
			MoveTool moveTool = activeBrushTool as MoveTool;
			if (moveTool != null)
			{
				Vector3 vector = cam.WorldToScreenPoint(this._lastHitPoint);
				Vector3 vector2 = cam.ScreenToWorldPoint(vector);
				Vector3 vector3 = cam.ScreenToWorldPoint(new Vector3(vector.x + 1f, vector.y, vector.z));
				float num3 = Vector3.Distance(vector2, vector3);
				moveTool.UseViewPlane = !this.Input.ShiftHeld;
				moveTool.MirrorRoot = this._objectRoot;
				if (moveTool.UseViewPlane)
				{
					moveTool.MouseDelta = new Vector2(num * num3, num2 * num3);
				}
				else
				{
					moveTool.DragDelta = num2 * num3;
				}
			}
			else
			{
				InflateTool inflateTool = activeBrushTool as InflateTool;
				if (inflateTool != null)
				{
					inflateTool.Amount = (this.Input.AltHeld ? (-0.005f) : 0.005f);
				}
				else
				{
					PinchTool pinchTool = activeBrushTool as PinchTool;
					if (pinchTool != null)
					{
						pinchTool.Amount = (this.Input.AltHeld ? (-0.005f) : 0.005f);
					}
				}
			}
			for (int j = 0; j < this._strokeStates.Count; j++)
			{
				ShapePaintOverlay.StrokeRendererState strokeRendererState2 = this._strokeStates[j];
				if (strokeRendererState2.SelTool != null && strokeRendererState2.Layer != null)
				{
					this.ApplyBrushToRenderer(strokeRendererState2, activeBrushTool, this._lastHitPoint, this._lastHitNormal, cam, false);
					if (this._symmetryEnabled)
					{
						this.ApplyBrushToRenderer(strokeRendererState2, activeBrushTool, this._lastHitPoint, this._lastHitNormal, cam, true);
					}
				}
			}
		}

		// Token: 0x060003CF RID: 975 RVA: 0x000245E8 File Offset: 0x000227E8
		private void ApplyBrushToRenderer(ShapePaintOverlay.StrokeRendererState st, IDeformTool tool, Vector3 worldHitPoint, Vector3 worldHitNormal, Camera cam, bool mirror)
		{
			SelectionTool selTool = st.SelTool;
			Vector3[] cachedVertices = selTool.CachedVertices;
			Vector3[] cachedNormals = selTool.CachedNormals;
			if (cachedVertices == null)
			{
				return;
			}
			Vector3 vector = worldHitPoint;
			Vector3 vector2 = worldHitNormal;
			if (mirror)
			{
				if (this._objectRoot == null)
				{
					return;
				}
				Vector3 vector3 = this._objectRoot.InverseTransformPoint(worldHitPoint);
				float num = (this._symmetryCenterSet ? this._symmetryCenter : 0f);
				int symmetryAxis = this._symmetryAxis;
				if (symmetryAxis == 0)
				{
					vector3.x = num * 2f - vector3.x;
				}
				else if (symmetryAxis == 1)
				{
					vector3.y = num * 2f - vector3.y;
				}
				else
				{
					vector3.z = num * 2f - vector3.z;
				}
				vector = this._objectRoot.TransformPoint(vector3);
				Vector3 vector4 = this._objectRoot.InverseTransformDirection(worldHitNormal);
				if (symmetryAxis == 0)
				{
					vector4.x = -vector4.x;
				}
				else if (symmetryAxis == 1)
				{
					vector4.y = -vector4.y;
				}
				else
				{
					vector4.z = -vector4.z;
				}
				vector2 = this._objectRoot.TransformDirection(vector4);
			}
			Transform colliderTransform = selTool.ColliderTransform;
			BrushResult brushResult;
			if (tool is MoveTool)
			{
				if (mirror && st.MoveGrabMirrorResult != null)
				{
					brushResult = st.MoveGrabMirrorResult;
				}
				else if (!mirror && st.MoveGrabResult != null)
				{
					brushResult = st.MoveGrabResult;
				}
				else
				{
					Vector3 vector5 = ((colliderTransform != null) ? colliderTransform.InverseTransformPoint(vector) : vector);
					brushResult = selTool.BrushSelectAtPoint(vector5, vector, vector2);
					if (brushResult == null || brushResult.AffectedVertices.Count == 0)
					{
						return;
					}
					BrushResult brushResult2 = new BrushResult
					{
						HitPoint = brushResult.HitPoint,
						HitNormal = brushResult.HitNormal,
						AffectedVertices = new Dictionary<int, float>(brushResult.AffectedVertices)
					};
					if (mirror)
					{
						st.MoveGrabMirrorVertices = brushResult2.AffectedVertices;
						st.MoveGrabMirrorResult = brushResult2;
					}
					else
					{
						st.MoveGrabVertices = brushResult2.AffectedVertices;
						st.MoveGrabResult = brushResult2;
					}
					brushResult = brushResult2;
				}
			}
			else
			{
				Vector3 vector6 = ((colliderTransform != null) ? colliderTransform.InverseTransformPoint(vector) : vector);
				brushResult = selTool.BrushSelectAtPoint(vector6, vector, vector2);
				if (brushResult == null || brushResult.AffectedVertices.Count == 0)
				{
					return;
				}
			}
			Vector3[] deltas = st.Layer.Deltas;
			foreach (KeyValuePair<int, float> keyValuePair in brushResult.AffectedVertices)
			{
				if (keyValuePair.Key >= 0 && keyValuePair.Key < deltas.Length && !st.BeforeSnapshot.ContainsKey(keyValuePair.Key))
				{
					st.BeforeSnapshot[keyValuePair.Key] = deltas[keyValuePair.Key];
				}
			}
			MoveTool moveTool = tool as MoveTool;
			if (moveTool != null)
			{
				moveTool.RendererTransform = ((st.Renderer != null) ? st.Renderer.transform : null);
				moveTool.MirrorAxis = (mirror ? this._symmetryAxis : (-1));
			}
			IDeformTool deformTool;
			if (!(tool is SmoothTool) || st.SmoothTool == null)
			{
				deformTool = tool;
			}
			else
			{
				IDeformTool smoothTool = st.SmoothTool;
				deformTool = smoothTool;
			}
			IDeformTool deformTool2 = deformTool;
			try
			{
				deformTool2.Apply(st.Deformer, st.Layer, brushResult, cachedVertices, cachedNormals, cam);
			}
			finally
			{
				if (moveTool != null)
				{
					moveTool.MirrorAxis = -1;
				}
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00024960 File Offset: 0x00022B60
		private void CommitBrushUndoEntry(DeformLayer primaryActiveLayer)
		{
			if (this._undoStack == null || this._strokeStates == null || this._currentBrushEntry == null)
			{
				this._strokeStates = null;
				this._currentBrushEntry = null;
				return;
			}
			bool flag = false;
			for (int i = 0; i < this._strokeStates.Count; i++)
			{
				ShapePaintOverlay.StrokeRendererState strokeRendererState = this._strokeStates[i];
				if (strokeRendererState.LazyCreated)
				{
					flag = true;
				}
				if (strokeRendererState.Layer != null && strokeRendererState.BeforeSnapshot.Count != 0)
				{
					int count = strokeRendererState.BeforeSnapshot.Count;
					int[] array = new int[count];
					Vector3[] array2 = new Vector3[count];
					Vector3[] array3 = new Vector3[count];
					int num = 0;
					Vector3[] deltas = strokeRendererState.Layer.Deltas;
					foreach (KeyValuePair<int, Vector3> keyValuePair in strokeRendererState.BeforeSnapshot)
					{
						array[num] = keyValuePair.Key;
						array2[num] = keyValuePair.Value;
						array3[num] = deltas[keyValuePair.Key];
						num++;
					}
					this._currentBrushEntry.Add(new DeltaUndoEntry(strokeRendererState.Layer, array, array2, array3, strokeRendererState.Deformer));
				}
			}
			if (this._currentBrushEntry.ChildCount > 0)
			{
				this._undoStack.Push(this._currentBrushEntry);
			}
			this._strokeStates = null;
			this._currentBrushEntry = null;
			this._strokeStartHitPoint = Vector3.zero;
			this._strokeStartHitNormal = Vector3.zero;
			this._lastValidHitPoint = Vector3.zero;
			this._lastValidHitNormal = Vector3.zero;
			if (flag)
			{
				this.NotifyLayerStructureChanged();
			}
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00024B14 File Offset: 0x00022D14
		private void ProcessBrushAdjust()
		{
			if (this.Window == null || this.Input == null)
			{
				return;
			}
			if (!this.Window.IsEditMode || (this.Window.OperationMode != ShapeEditorWindow.OpMode.Brush && this.Window.OperationMode != ShapeEditorWindow.OpMode.Gizmo))
			{
				this._bracketRepeater.Tick(false, false, 0f);
				return;
			}
			int num = this._bracketRepeater.Tick(this.Input.BracketLeftHeld, this.Input.BracketRightHeld, Time.unscaledTime);
			if (num == 0)
			{
				return;
			}
			if (this.Window.OperationMode == ShapeEditorWindow.OpMode.Brush)
			{
				this.ApplyBrushStep(num);
				return;
			}
			if (this.Input.ShiftHeld)
			{
				return;
			}
			if (this.Window.GizmoSelection == ShapeEditorWindow.GizmoSelectMethod.Brush)
			{
				this.StepBrushRadius(num);
				return;
			}
			float num2 = this.Window.GizmoSoftRadius + (float)num * 0.005f;
			this.Window.GizmoSoftRadius = Mathf.Clamp(num2, 0.001f, ShapeEditorPlugin.MaxSoftSelectionRadius.Value);
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00024C08 File Offset: 0x00022E08
		private void ApplyBrushStep(int dir)
		{
			if (this.Input.ShiftHeld)
			{
				float num = this.Window.BrushStrength + (float)dir * 0.05f;
				this.Window.BrushStrength = Mathf.Clamp(num, 0.01f, 1f);
				return;
			}
			this.StepBrushRadius(dir);
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00024C5C File Offset: 0x00022E5C
		private void StepBrushRadius(int dir)
		{
			float num = this.Window.BrushRadius + (float)dir * 0.005f;
			this.Window.BrushRadius = Mathf.Clamp(num, 0.001f, ShapeEditorPlugin.MaxBrushRadius.Value);
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00024CA0 File Offset: 0x00022EA0
		static ShapePaintOverlay()
		{
			ShapePaintOverlay.WeightGradient.SetKeys(new GradientColorKey[]
			{
				new GradientColorKey(new Color(0f, 0f, 1f), 0f),
				new GradientColorKey(new Color(0f, 1f, 0f), 0.33f),
				new GradientColorKey(new Color(1f, 1f, 0f), 0.66f),
				new GradientColorKey(new Color(1f, 0f, 0f), 1f)
			}, new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			});
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00024E44 File Offset: 0x00023044
		public void SetTarget(Renderer renderer)
		{
			this._targetRenderer = renderer;
			this._deformer = ((renderer != null) ? renderer.GetComponent<ShapeDeformer>() : null);
			if (this._moveTool != null)
			{
				this._moveTool.Deformer = this._deformer;
			}
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x00024E80 File Offset: 0x00023080
		private void Awake()
		{
			this._packetHandler = new TabletEvent(this.OnTabletPacket);
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			if (shader != null)
			{
				this._cursorMaterial = ShapePaintOverlay.CreateBlendMaterial(shader, 5, 10, 0, 0);
				this._highlightMaterial = ShapePaintOverlay.CreateBlendMaterial(shader, 5, 1, 0, 8);
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00024ED4 File Offset: 0x000230D4
		private static Material CreateBlendMaterial(Shader shader, int srcBlend, int dstBlend, int zWrite, int zTest)
		{
			Material material = new Material(shader);
			material.SetInt("_SrcBlend", srcBlend);
			material.SetInt("_DstBlend", dstBlend);
			material.SetInt("_Cull", 0);
			material.SetInt("_ZWrite", zWrite);
			material.SetInt("_ZTest", zTest);
			return material;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00024F24 File Offset: 0x00023124
		private void Update()
		{
			if (ShapeEditorPlugin.StudioToggleKey.Value.IsDown())
			{
				ShapeEditorWindow window = this.Window;
				if (window != null)
				{
					window.Toggle();
				}
				if (this.Window != null && this.Window.Visible && this.OnRefreshRenderers != null)
				{
					this.OnRefreshRenderers();
				}
			}
			if (this.Window == null)
			{
				return;
			}
			if (this.Window.Visible && !this.Window.IsEditMode && this.GetCurrentSelection != null)
			{
				object obj = this.GetCurrentSelection();
				if (obj != this._lastSelection)
				{
					this._lastSelection = obj;
					if (this.OnRefreshRenderers != null)
					{
						this.OnRefreshRenderers();
					}
				}
			}
			if (this.Window.Visible)
			{
				this.ProcessDeferredActions();
			}
			if (this.Window.IsEditMode)
			{
				InputHelper input = this.Input;
				if (input != null)
				{
					input.PollInput();
				}
				InputHelper input2 = this.Input;
				if (input2 == null)
				{
					return;
				}
				input2.UpdateCameraIsolation(true);
			}
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0002501C File Offset: 0x0002321C
		private void ProcessDeferredActions()
		{
			if (this.Window == null)
			{
				return;
			}
			if (this.Window.DeferEnterEditMode)
			{
				this.Window.DeferEnterEditMode = false;
				if (this.OnRefreshRenderers != null)
				{
					this.OnRefreshRenderers();
				}
				this.DoEnterEditMode();
			}
			if (this.Window.DeferExitEditMode)
			{
				this.Window.DeferExitEditMode = false;
				this.DoExitEditMode();
			}
			if (this.Window.DeferLayerAdd)
			{
				this.Window.DeferLayerAdd = false;
				this.DoLayerAdd();
			}
			if (this.Window.DeferLayerRemove >= 0)
			{
				int deferLayerRemove = this.Window.DeferLayerRemove;
				this.Window.DeferLayerRemove = -1;
				this.DoLayerRemove(deferLayerRemove);
			}
			if (this.Window.DeferLayerRename >= 0)
			{
				int deferLayerRename = this.Window.DeferLayerRename;
				string deferLayerRenameNewName = this.Window.DeferLayerRenameNewName;
				this.Window.DeferLayerRename = -1;
				this.Window.DeferLayerRenameNewName = null;
				this.DoLayerRename(deferLayerRename, deferLayerRenameNewName);
			}
			if (this.Window.DeferLayerMirror >= 0)
			{
				int deferLayerMirror = this.Window.DeferLayerMirror;
				this.Window.DeferLayerMirror = -1;
				this.DoLayerMirror(deferLayerMirror);
			}
			if (this.Window.DeferSubdivide)
			{
				this.Window.DeferSubdivide = false;
				this.DoSubdivide();
				if (this._undoStack != null)
				{
					this._undoStack.Clear();
				}
			}
			if (this.Window.DeferRestore)
			{
				this.Window.DeferRestore = false;
				this.DoRestore();
			}
			if (this.Window.DeferRebakeBodySubdivision)
			{
				this.Window.DeferRebakeBodySubdivision = false;
				this.DoRebakeBodySubdivision();
			}
			if (this.Window.DeferLayerMoveUp >= 0)
			{
				int deferLayerMoveUp = this.Window.DeferLayerMoveUp;
				this.Window.DeferLayerMoveUp = -1;
				this.DoLayerMove(deferLayerMoveUp, true);
			}
			if (this.Window.DeferLayerMoveDown >= 0)
			{
				int deferLayerMoveDown = this.Window.DeferLayerMoveDown;
				this.Window.DeferLayerMoveDown = -1;
				this.DoLayerMove(deferLayerMoveDown, false);
			}
			if (this.Window.WeightUndoLayer >= 0)
			{
				int weightUndoLayer = this.Window.WeightUndoLayer;
				this.Window.WeightUndoLayer = -1;
				this.DoLayerWeightCommit(weightUndoLayer, this.Window.WeightUndoBefore, this.Window.WeightUndoAfter);
			}
			if (this.Window.DeferFaceSelectAll)
			{
				this.Window.DeferFaceSelectAll = false;
				if (this.Window.FaceSelect != null)
				{
					this.Window.FaceSelect.SelectAll();
				}
			}
			if (this.Window.DeferFaceSelectNone)
			{
				this.Window.DeferFaceSelectNone = false;
				if (this.Window.FaceSelect != null)
				{
					this.Window.FaceSelect.ClearSelection();
				}
			}
			if (this.Window.DeferFaceSelectInvert)
			{
				this.Window.DeferFaceSelectInvert = false;
				if (this.Window.FaceSelect != null)
				{
					this.Window.FaceSelect.InvertSelection();
				}
			}
			if (this.Window.DeferFaceDelete)
			{
				this.Window.DeferFaceDelete = false;
				this.DoFaceDelete();
			}
			if (this.Window.DeferFaceRestore)
			{
				this.Window.DeferFaceRestore = false;
				this.DoFaceRestoreSelected();
			}
			if (this.Window.DeferFaceRestoreAll)
			{
				this.Window.DeferFaceRestoreAll = false;
				this.DoFaceRestoreAll();
			}
			if (this.Window.DeferSetSymmetryCenter)
			{
				this.Window.DeferSetSymmetryCenter = false;
				if (this._gizmo != null && this._gizmo.HasTarget && this._primaryListIdx >= 0)
				{
					Transform transform = this._activeRenderers[this._primaryListIdx].transform;
					Vector3 centroidWorld = this._gizmo.CentroidWorld;
					Vector3 vector = ((this._objectRoot != null) ? this._objectRoot : transform).InverseTransformPoint(centroidWorld);
					int symmetryAxisIndex = this.Window.SymmetryAxisIndex;
					float num = ((symmetryAxisIndex == 0) ? vector.x : ((symmetryAxisIndex == 1) ? vector.y : vector.z));
					this.Window.SymmetryCenter = num;
					this.Window.SymmetryCenterSet = true;
					this._symmetryCenter = num;
					this._symmetryCenterSet = true;
				}
			}
			if (this.Window.DeferClearSymmetryCenter)
			{
				this.Window.DeferClearSymmetryCenter = false;
				this.Window.SymmetryCenter = 0f;
				this.Window.SymmetryCenterSet = false;
				this._symmetryCenter = 0f;
				this._symmetryCenterSet = false;
			}
			if (this.Window.DeferRemapWeights)
			{
				this.Window.DeferRemapWeights = false;
				this.DoRemapWeights();
			}
			if (this.Window.DeferRestoreWeights)
			{
				this.Window.DeferRestoreWeights = false;
				this.DoRestoreWeights();
			}
			if (this.Window.DeferExport)
			{
				this.Window.DeferExport = false;
				this.DoExportDeform();
			}
			if (this.Window.DeferImport)
			{
				this.Window.DeferImport = false;
				this.DoImportDeform();
			}
			if (this.Window.DeferFocusRenderer)
			{
				this.Window.DeferFocusRenderer = false;
				int primaryRendererIndex = this.Window.PrimaryRendererIndex;
				if (primaryRendererIndex >= 0 && primaryRendererIndex < this.Window.Renderers.Count && this.Window.SelectedRendererIndices.Contains(primaryRendererIndex))
				{
					Renderer renderer = this.Window.Renderers[primaryRendererIndex];
					if (renderer != null && renderer.gameObject.activeInHierarchy)
					{
						CameraControlResolver.FocusOn(renderer.bounds.center, this);
					}
				}
			}
			if (this.Window.DeferClearGizmoSelection)
			{
				this.Window.DeferClearGizmoSelection = false;
				for (int i = 0; i < this._activeSelTools.Count; i++)
				{
					if (this._activeSelTools[i] != null)
					{
						this._activeSelTools[i].ClearSelection();
					}
				}
				this.UpdateGizmoTarget();
			}
			if (this.Window.DeferPresetExport)
			{
				this.Window.DeferPresetExport = false;
				this.DoApplyPresetExport();
			}
			if (this.Window.DeferPresetImport)
			{
				this.Window.DeferPresetImport = false;
				this.DoLoadPresetForPreview();
			}
			if (this.Window.DeferPresetApplyImport)
			{
				this.Window.DeferPresetApplyImport = false;
				this.DoApplyPresetImport();
			}
			if (this.Window.DeferPresetCancelImport)
			{
				this.Window.DeferPresetCancelImport = false;
				this.DoCancelPresetImport();
			}
			if (this.Window.DeferResetCorruptedLayers)
			{
				string deferResetPath = this.Window.DeferResetPath;
				this.Window.DeferResetCorruptedLayers = false;
				this.Window.DeferResetPath = null;
				this.DoResetCorruptedLayers(deferResetPath);
			}
		}

		// Token: 0x060003DA RID: 986 RVA: 0x000256B8 File Offset: 0x000238B8
		private Renderer GetCurrentRenderer()
		{
			if (this.Window == null || this.Window.Renderers.Count == 0)
			{
				return null;
			}
			int num = Mathf.Clamp(this.Window.SelectedRendererIndex, 0, this.Window.Renderers.Count - 1);
			return this.Window.Renderers[num];
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00025718 File Offset: 0x00023918
		private static DeformData GetExistingDeformData(Renderer renderer)
		{
			return ControllerResolver.Resolve(renderer).GetDeformData(renderer);
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00025734 File Offset: 0x00023934
		private static DeformData GetDeformDataForRenderer(Renderer renderer, out bool studioMode)
		{
			ControllerResolver.Owner owner = ControllerResolver.Resolve(renderer);
			studioMode = owner.IsOnItem;
			return owner.GetOrCreateDeformData(renderer);
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0002575C File Offset: 0x0002395C
		private void OnDestroy()
		{
			this.EnsurePressureUnsubscribed();
			this.DeactivateHighlight();
			if (this.SelectionTool != null)
			{
				this.SelectionTool.CleanupCollider();
			}
			InputHelper input = this.Input;
			if (input != null)
			{
				input.Cleanup();
			}
			if (this._cursorMaterial != null)
			{
				UnityEngine.Object.Destroy(this._cursorMaterial);
			}
			if (this._highlightMaterial != null)
			{
				UnityEngine.Object.Destroy(this._highlightMaterial);
			}
			foreach (KeyValuePair<int, Mesh> keyValuePair in this._bakeMeshCacheByRenderer)
			{
				if (keyValuePair.Value != null)
				{
					UnityEngine.Object.Destroy(keyValuePair.Value);
				}
			}
			this._bakeMeshCacheByRenderer.Clear();
			this._highlightTrisByMesh.Clear();
			this.DestroyAllWireBundles();
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00025844 File Offset: 0x00023A44
		private void DestroyAllWireBundles()
		{
			for (int i = 0; i < this._wireBundles.Count; i++)
			{
				if (this._wireBundles[i] != null && this._wireBundles[i].LineMesh != null)
				{
					UnityEngine.Object.Destroy(this._wireBundles[i].LineMesh);
				}
			}
			this._wireBundles.Clear();
		}

		// Token: 0x060003DF RID: 991 RVA: 0x000258B0 File Offset: 0x00023AB0
		private void InvalidateAllWireColors()
		{
			for (int i = 0; i < this._wireBundles.Count; i++)
			{
				if (this._wireBundles[i] != null)
				{
					this._wireBundles[i].ColorsDirty = true;
				}
			}
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x000258F4 File Offset: 0x00023AF4
		private void NotifyLayerStructureChanged()
		{
			this._notifyOwnerBuf.Clear();
			if (this._activeRenderers.Count > 0)
			{
				for (int i = 0; i < this._activeRenderers.Count; i++)
				{
					this.AddOwnerToNotifyBuf(this._activeRenderers[i]);
				}
			}
			else if (this.Window != null && this.Window.Renderers.Count > 0)
			{
				int num = Mathf.Clamp(this.Window.PrimaryRendererIndex, 0, this.Window.Renderers.Count - 1);
				this.AddOwnerToNotifyBuf(this.Window.Renderers[num]);
			}
			foreach (MonoBehaviour monoBehaviour in this._notifyOwnerBuf)
			{
				ShapeEditorController shapeEditorController = monoBehaviour as ShapeEditorController;
				if (shapeEditorController != null)
				{
					shapeEditorController.NotifyDataChanged();
				}
				else
				{
					ItemShapeController itemShapeController = monoBehaviour as ItemShapeController;
					if (itemShapeController != null)
					{
						itemShapeController.NotifyDataChanged();
					}
				}
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00025A10 File Offset: 0x00023C10
		private void AddOwnerToNotifyBuf(Renderer renderer)
		{
			if (renderer == null)
			{
				return;
			}
			ControllerResolver.Owner owner = ControllerResolver.Resolve(renderer);
			if (owner.IsOnCharacter)
			{
				this._notifyOwnerBuf.Add(owner.CharacterController);
				return;
			}
			if (owner.IsOnItem)
			{
				this._notifyOwnerBuf.Add(owner.ItemController);
			}
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x00025A68 File Offset: 0x00023C68
		private static FieldInfo ResolvePressureField()
		{
			if (ShapePaintOverlay._pressureFieldResolved)
			{
				return ShapePaintOverlay._pressureField;
			}
			Type typeFromHandle = typeof(Packet);
			ShapePaintOverlay._pressureField = typeFromHandle.GetField("NormalPressure") ?? typeFromHandle.GetField("pkNormalPressure");
			ShapePaintOverlay._pressureFieldResolved = true;
			return ShapePaintOverlay._pressureField;
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00025AB8 File Offset: 0x00023CB8
		private void OnTabletPacket(Packet[] packets)
		{
			if (packets == null || packets.Length == 0)
			{
				return;
			}
			uint maxPressure = TabletManager.MaxPressure;
			if (maxPressure == 0U)
			{
				return;
			}
			FieldInfo fieldInfo = ShapePaintOverlay.ResolvePressureField();
			if (fieldInfo == null)
			{
				return;
			}
			uint num = (uint)fieldInfo.GetValue(packets[packets.Length - 1]);
			this._lastPressure01 = Mathf.Clamp01(num / maxPressure);
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x00025B15 File Offset: 0x00023D15
		private void EnsurePressureSubscription()
		{
			if (this._pressureSubscribed)
			{
				return;
			}
			TabletManager.Subscribe(this._packetHandler);
			this._pressureSubscribed = true;
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x00025B32 File Offset: 0x00023D32
		private void EnsurePressureUnsubscribed()
		{
			if (!this._pressureSubscribed)
			{
				return;
			}
			TabletManager.Unsubscribe(this._packetHandler);
			this._pressureSubscribed = false;
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00025B50 File Offset: 0x00023D50
		private void DoResetCorruptedLayers(string path)
		{
			if (string.IsNullOrEmpty(path) || this.Window == null)
			{
				return;
			}
			ShapeEditorController shapeEditorController = null;
			if (this.Window.RendererPaths != null && this.Window.Renderers != null)
			{
				int num = this.Window.RendererPaths.IndexOf(path);
				if (num >= 0 && num < this.Window.Renderers.Count && this.Window.Renderers[num] != null)
				{
					shapeEditorController = this.Window.Renderers[num].GetComponentInParent<ShapeEditorController>();
				}
			}
			if (shapeEditorController == null && this.Window.Renderers != null)
			{
				foreach (Renderer renderer in this.Window.Renderers)
				{
					if (!(renderer == null))
					{
						shapeEditorController = renderer.GetComponentInParent<ShapeEditorController>();
						if (shapeEditorController != null)
						{
							break;
						}
					}
				}
			}
			if (shapeEditorController == null)
			{
				ShapeEditorPlugin.Logger.LogWarning("Reset corrupted layers: no controller found for path " + path);
				return;
			}
			shapeEditorController.ResetLayersForPath(path);
			if (this._undoStack != null)
			{
				this._undoStack.Clear();
			}
			if (this.OnRefreshRenderers != null)
			{
				this.OnRefreshRenderers();
			}
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00025CA4 File Offset: 0x00023EA4
		private void DoEnterEditMode()
		{
			if (this.Window.Renderers.Count == 0)
			{
				return;
			}
			int num = Mathf.Clamp(this.Window.PrimaryRendererIndex, 0, this.Window.Renderers.Count - 1);
			Renderer renderer = this.Window.Renderers[num];
			if (renderer == null)
			{
				return;
			}
			List<int> list = new List<int> { num };
			if (this.Window.SelectedRendererIndices != null)
			{
				List<int> list2 = new List<int>(this.Window.SelectedRendererIndices);
				list2.Sort();
				foreach (int num2 in list2)
				{
					if (num2 != num && num2 >= 0 && num2 < this.Window.Renderers.Count && !(this.Window.Renderers[num2] == null))
					{
						list.Add(num2);
					}
				}
			}
			this._activeRenderers.Clear();
			this._activeDeformers.Clear();
			this._activeSelTools.Clear();
			this._activeSmoothTools.Clear();
			this._primaryListIdx = -1;
			if (this._moveTool == null)
			{
				this._moveTool = new MoveTool();
			}
			if (this._smoothTool == null)
			{
				this._smoothTool = new SmoothTool();
			}
			if (this._inflateTool == null)
			{
				this._inflateTool = new InflateTool();
			}
			if (this._pinchTool == null)
			{
				this._pinchTool = new PinchTool();
			}
			if (this._gizmo == null)
			{
				this._gizmo = new TransformGizmo();
			}
			if (this._seamlessTool == null)
			{
				this._seamlessTool = new SeamlessTool();
			}
			DeformData deformData = null;
			for (int i = 0; i < list.Count; i++)
			{
				int num3 = list[i];
				Renderer renderer2 = this.Window.Renderers[num3];
				bool flag;
				DeformData deformDataForRenderer = ShapePaintOverlay.GetDeformDataForRenderer(renderer2, out flag);
				ShapeDeformer shapeDeformer = renderer2.GetComponent<ShapeDeformer>();
				if (shapeDeformer == null)
				{
					shapeDeformer = renderer2.gameObject.AddComponent<ShapeDeformer>();
				}
				if (deformDataForRenderer != null)
				{
					shapeDeformer.StudioMode = flag;
					shapeDeformer.DeformData = deformDataForRenderer;
				}
				SkinnedMeshRenderer skinnedMeshRenderer = renderer2 as SkinnedMeshRenderer;
				if (skinnedMeshRenderer != null)
				{
					shapeDeformer.Init(skinnedMeshRenderer);
				}
				else
				{
					MeshFilter component = renderer2.GetComponent<MeshFilter>();
					MeshRenderer meshRenderer = renderer2 as MeshRenderer;
					if (component != null && meshRenderer != null)
					{
						shapeDeformer.Init(component, meshRenderer);
					}
				}
				SelectionTool selectionTool = new SelectionTool();
				if (skinnedMeshRenderer != null)
				{
					selectionTool.SetTarget(skinnedMeshRenderer);
				}
				else
				{
					MeshFilter component2 = renderer2.GetComponent<MeshFilter>();
					if (component2 != null)
					{
						selectionTool.SetTarget(component2);
					}
				}
				SmoothTool smoothTool = new SmoothTool();
				Mesh mesh = MeshHelper.GetMesh(renderer2);
				if (mesh != null)
				{
					smoothTool.BuildAdjacency(mesh.triangles, mesh.vertexCount, mesh.vertices);
				}
				this._activeRenderers.Add(renderer2);
				this._activeDeformers.Add(shapeDeformer);
				this._activeSelTools.Add(selectionTool);
				this._activeSmoothTools.Add(smoothTool);
				if (num3 == num)
				{
					this._primaryListIdx = this._activeRenderers.Count - 1;
					deformData = deformDataForRenderer;
				}
			}
			if (this._primaryListIdx < 0)
			{
				this.CleanupActiveLists();
				return;
			}
			this.SetTarget(this._activeRenderers[this._primaryListIdx]);
			this.SelectionTool = this._activeSelTools[this._primaryListIdx];
			this._smoothTool = this._activeSmoothTools[this._primaryListIdx];
			this.Window.ActiveDeformData = deformData;
			this.Window.SetEditMode(true);
			this._undoStack = new UndoStack(ShapeEditorPlugin.UndoMaxSteps.Value);
			this._moveTool.Deformer = this._deformer;
			Transform rootTransform = ControllerResolver.Resolve(renderer).RootTransform;
			this._objectRoot = ((rootTransform != null) ? rootTransform : renderer.transform);
			this._gizmo.SetObjectRoot(rootTransform);
			Mesh mesh2 = MeshHelper.GetMesh(renderer);
			if (mesh2 != null)
			{
				this.Window.VertexCount = mesh2.vertexCount;
				this.ActivateHighlight(mesh2.vertexCount);
			}
			for (int j = 0; j < this._activeDeformers.Count; j++)
			{
				if (j != this._primaryListIdx && this._activeDeformers[j] != null)
				{
					this._activeDeformers[j].EnterEditMode(null);
				}
			}
			if (this.Window.OperationMode == ShapeEditorWindow.OpMode.Brush && ShapeEditorPlugin.UsePressure.Value)
			{
				this.EnsurePressureSubscription();
			}
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x00026134 File Offset: 0x00024334
		private void DoExitEditMode()
		{
			if (this._gizmo != null && this._gizmo.IsDragging)
			{
				this._gizmo.EndDrag();
			}
			if (this._gizmo != null)
			{
				this._gizmo.SetObjectRoot(null);
				this._gizmo.ClearTarget();
				this._gizmo.PendingDragEntry = null;
				this._gizmo.LazyEnsureLayerCallback = null;
			}
			for (int i = 0; i < this._activeDeformers.Count; i++)
			{
				if (i != this._primaryListIdx && this._activeDeformers[i] != null)
				{
					this._activeDeformers[i].ExitEditMode();
				}
			}
			this.DeactivateHighlight();
			this.CleanupActiveLists();
			this.SetTarget(null);
			this.SelectionTool = new SelectionTool();
			this._objectRoot = null;
			this.Window.ActiveDeformData = null;
			this.Window.SetEditMode(false);
			this.DestroyAllWireBundles();
			this._undoStack = null;
			this._isBrushing = false;
			this.EnsurePressureUnsubscribed();
			this._lastPressure01 = 1f;
			InputHelper input = this.Input;
			if (input == null)
			{
				return;
			}
			input.UpdateCameraIsolation(false);
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00026250 File Offset: 0x00024450
		private void CleanupActiveLists()
		{
			for (int i = 0; i < this._activeSelTools.Count; i++)
			{
				if (this._activeSelTools[i] != null)
				{
					this._activeSelTools[i].CleanupCollider();
				}
			}
			this._activeRenderers.Clear();
			this._activeDeformers.Clear();
			this._activeSelTools.Clear();
			this._activeSmoothTools.Clear();
			this._primaryListIdx = -1;
			this._colliderCache.Clear();
			List<Vector3[]> boxSelectWorldNormals = this._boxSelectWorldNormals;
			if (boxSelectWorldNormals == null)
			{
				return;
			}
			boxSelectWorldNormals.Clear();
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x000262E0 File Offset: 0x000244E0
		private void DoRemapWeights()
		{
			if (this._deformer == null || this._targetRenderer == null)
			{
				return;
			}
			if (this.Window.ActiveDeformData == null)
			{
				return;
			}
			SkinnedMeshRenderer skinnedMeshRenderer = this._targetRenderer as SkinnedMeshRenderer;
			if (skinnedMeshRenderer == null)
			{
				return;
			}
			ControllerResolver.Owner owner = ControllerResolver.Resolve(this._targetRenderer);
			if (!owner.IsOnCharacter)
			{
				return;
			}
			SkinnedMeshRenderer bodySmr = owner.CharacterController.GetBodySmr();
			if (bodySmr == null)
			{
				return;
			}
			Mesh sharedMesh = bodySmr.sharedMesh;
			Mesh sharedMesh2 = skinnedMeshRenderer.sharedMesh;
			if (sharedMesh2 == null)
			{
				return;
			}
			Vector3[] array = this._deformer.BindVertices ?? sharedMesh2.vertices;
			Vector3[] array2 = this.Window.ActiveDeformData.ComputeFinalDelta();
			if (array2 == null || array2.Length != array.Length)
			{
				return;
			}
			BoneWeight[] array3 = WeightRemapper.ComputeRemappedWeights(array, array2, skinnedMeshRenderer.bones, sharedMesh.vertices, sharedMesh.boneWeights, this._deformer.OriginalBoneWeights, bodySmr.bones, sharedMesh.triangles);
			if (array3 != null)
			{
				this._deformer.RemappedBoneWeights = array3;
				this.Window.ActiveDeformData.WeightRemapped = true;
			}
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00026403 File Offset: 0x00024603
		private void DoRestoreWeights()
		{
			if (this._deformer == null)
			{
				return;
			}
			this._deformer.ClearRemappedWeights();
			if (this.Window.ActiveDeformData != null)
			{
				this.Window.ActiveDeformData.WeightRemapped = false;
			}
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00026440 File Offset: 0x00024640
		private static bool TryShowSaveDialog(string title, string defaultName, string filter, out string path)
		{
			if (!ShapePaintOverlay._systemDialogUnavailable)
			{
				try
				{
					return SystemFileDialog.ShowDialog(title, defaultName, out path, (SystemFileDialog.FOS)10, filter);
				}
				catch (DllNotFoundException)
				{
					ShapePaintOverlay.MarkSystemDialogUnavailable();
				}
			}
			string text = Path.GetExtension(defaultName);
			if (!string.IsNullOrEmpty(text) && text[0] == '.')
			{
				text = text.Substring(1);
			}
			string text2 = FileDialogHelper.ShowSaveDialog(title, defaultName, filter, text);
			path = text2 ?? string.Empty;
			return !string.IsNullOrEmpty(text2);
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x000264C0 File Offset: 0x000246C0
		private static bool TryShowOpenDialog(string title, string filter, out string path)
		{
			if (!ShapePaintOverlay._systemDialogUnavailable)
			{
				try
				{
					return SystemFileDialog.ShowDialog(title, "", out path,  (SystemFileDialog.FOS)4104, filter);
				}
				catch (DllNotFoundException)
				{
					ShapePaintOverlay.MarkSystemDialogUnavailable();
				}
			}
			string text = FileDialogHelper.ShowOpenDialog(title, filter);
			path = text ?? string.Empty;
			return !string.IsNullOrEmpty(text);
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00026520 File Offset: 0x00024720
		private static void MarkSystemDialogUnavailable()
		{
			ShapePaintOverlay._systemDialogUnavailable = true;
			ShapeEditorPlugin.Logger.LogWarning("KKAPI.NativeHelper.dll not found — falling back to legacy comdlg32 file dialog. For the modern dialog, reinstall KKAPI completely (1.45+) so KKAPI.NativeHelper.dll sits next to KKAPI.dll.");
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x00026538 File Offset: 0x00024738
		private void DoExportDeform()
		{
			if (this.Window.Renderers.Count == 0)
			{
				return;
			}
			int num = Mathf.Clamp(this.Window.SelectedRendererIndex, 0, this.Window.Renderers.Count - 1);
			Renderer renderer = this.Window.Renderers[num];
			if (renderer == null)
			{
				return;
			}
			DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(renderer);
			if (existingDeformData == null || existingDeformData.Layers.Count == 0)
			{
				return;
			}
			string text;
			if (!ShapePaintOverlay.TryShowSaveDialog(L.ExportDeform, "deform.kksd", L.DeformFileFilter, out text) || string.IsNullOrEmpty(text))
			{
				return;
			}
			byte[] array = ShapeSerializer.SerializeSingleRenderer(existingDeformData);
			if (array == null)
			{
				return;
			}
			try
			{
				File.WriteAllBytes(text, array);
				ShapeEditorPlugin.Logger.LogInfo(L.ExportSuccess);
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("Export failed: " + ex.Message);
			}
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00026628 File Offset: 0x00024828
		private void DoImportDeform()
		{
			if (this.Window.Renderers.Count == 0)
			{
				return;
			}
			int num = Mathf.Clamp(this.Window.SelectedRendererIndex, 0, this.Window.Renderers.Count - 1);
			Renderer renderer = this.Window.Renderers[num];
			if (renderer == null)
			{
				return;
			}
			string text;
			if (!ShapePaintOverlay.TryShowOpenDialog(L.ImportDeform, L.DeformFileFilter, out text) || string.IsNullOrEmpty(text))
			{
				return;
			}
			byte[] array;
			try
			{
				array = File.ReadAllBytes(text);
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("Import failed: " + ex.Message);
				return;
			}
			int num2;
			List<DeformLayer> list = ShapeSerializer.DeserializeSingleRenderer(array, out num2);
			if (list == null)
			{
				ShapeEditorPlugin.Logger.LogWarning(L.ImportInvalidFile);
				return;
			}
			Mesh mesh = MeshHelper.GetMesh(renderer);
			if (mesh == null)
			{
				return;
			}
			if (num2 != mesh.vertexCount)
			{
				ShapeEditorPlugin.Logger.LogWarning(string.Format(L.ImportVertexMismatchFmt, num2, mesh.vertexCount));
				return;
			}
			bool flag;
			DeformData deformDataForRenderer = ShapePaintOverlay.GetDeformDataForRenderer(renderer, out flag);
			if (deformDataForRenderer == null)
			{
				return;
			}
			this.Window.ActiveDeformData = deformDataForRenderer;
			HashSet<string> hashSet = new HashSet<string>();
			foreach (DeformLayer deformLayer in deformDataForRenderer.Layers)
			{
				if (!string.IsNullOrEmpty(deformLayer.Id))
				{
					hashSet.Add(deformLayer.Id);
				}
			}
			foreach (DeformLayer deformLayer2 in list)
			{
				if (string.IsNullOrEmpty(deformLayer2.Id) || hashSet.Contains(deformLayer2.Id))
				{
					deformLayer2.Id = Guid.NewGuid().ToString("N");
				}
				hashSet.Add(deformLayer2.Id);
				deformLayer2.Dirty = true;
				deformDataForRenderer.Layers.Add(deformLayer2);
			}
			deformDataForRenderer.ActiveLayerIndex = deformDataForRenderer.Layers.Count - 1;
			if (this._deformer != null)
			{
				this._deformer.InvalidateDeltaCache();
			}
			ShapeEditorPlugin.Logger.LogInfo(L.ImportSuccess);
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00026898 File Offset: 0x00024A98
		private void DoFaceDelete()
		{
			ShapeEditorWindow window = this.Window;
			if (((window != null) ? window.FaceSelect : null) == null || this.Window.FaceSelect.SelectedFaces.Count == 0)
			{
				return;
			}
			Renderer currentRenderer = this.GetCurrentRenderer();
			if (currentRenderer == null)
			{
				return;
			}
			bool flag;
			DeformData deformDataForRenderer = ShapePaintOverlay.GetDeformDataForRenderer(currentRenderer, out flag);
			if (deformDataForRenderer == null)
			{
				return;
			}
			HashSet<int> selectedFaces = this.Window.FaceSelect.SelectedFaces;
			List<int> list = new List<int>();
			foreach (int num in selectedFaces)
			{
				if (!deformDataForRenderer.DeletedFaces.Contains(num))
				{
					list.Add(num);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			deformDataForRenderer.UnionDeletedFaces(list);
			UndoStack undoStack = this._undoStack;
			if (undoStack != null)
			{
				undoStack.Push(new FaceDeleteUndoEntry(deformDataForRenderer, list.ToArray()));
			}
			ShapePaintOverlay.EnsureDeformerAttachedForRenderer(currentRenderer);
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00026990 File Offset: 0x00024B90
		private static void EnsureDeformerAttachedForRenderer(Renderer renderer)
		{
			if (renderer == null)
			{
				return;
			}
			ControllerResolver.Owner owner = ControllerResolver.Resolve(renderer);
			if (owner.CharacterController != null)
			{
				ShapeEditorController characterController = owner.CharacterController;
				string relativePath = ShapeEditorController.GetRelativePath(characterController.RootTransform, renderer.transform);
				characterController.ReinitDeformerForPath(relativePath);
				return;
			}
			if (owner.ItemController != null)
			{
				owner.ItemController.ReattachAll();
			}
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x000269F4 File Offset: 0x00024BF4
		private void DoFaceRestoreSelected()
		{
			ShapeEditorWindow window = this.Window;
			if (((window != null) ? window.FaceSelect : null) == null || this.Window.FaceSelect.SelectedFaces.Count == 0)
			{
				return;
			}
			Renderer currentRenderer = this.GetCurrentRenderer();
			if (currentRenderer == null)
			{
				return;
			}
			DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(currentRenderer);
			if (existingDeformData == null || existingDeformData.DeletedFaces.Count == 0)
			{
				return;
			}
			HashSet<int> selectedFaces = this.Window.FaceSelect.SelectedFaces;
			List<int> list = new List<int>();
			foreach (int num in selectedFaces)
			{
				if (existingDeformData.DeletedFaces.Contains(num))
				{
					list.Add(num);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			existingDeformData.ExceptDeletedFaces(list);
			UndoStack undoStack = this._undoStack;
			if (undoStack == null)
			{
				return;
			}
			undoStack.Push(new FaceRestoreUndoEntry(existingDeformData, list.ToArray()));
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00026AF0 File Offset: 0x00024CF0
		private void DoFaceRestoreAll()
		{
			Renderer currentRenderer = this.GetCurrentRenderer();
			if (currentRenderer == null)
			{
				return;
			}
			DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(currentRenderer);
			if (existingDeformData == null || existingDeformData.DeletedFaces.Count == 0)
			{
				return;
			}
			int[] array = new int[existingDeformData.DeletedFaces.Count];
			existingDeformData.DeletedFaces.CopyTo(array);
			existingDeformData.ClearDeletedFaces();
			UndoStack undoStack = this._undoStack;
			if (undoStack == null)
			{
				return;
			}
			undoStack.Push(new FaceRestoreUndoEntry(existingDeformData, array));
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x00026B60 File Offset: 0x00024D60
		private void DoSubdivide()
		{
			Renderer currentRenderer = this.GetCurrentRenderer();
			if (currentRenderer == null)
			{
				return;
			}
			MeshHelper.CloneMeshIfShared(currentRenderer);
			Mesh mesh = MeshHelper.GetMesh(currentRenderer);
			if (mesh == null)
			{
				return;
			}
			HashSet<int> hashSet = null;
			int[] array = null;
			if (this.Window.FaceSelect != null && this.Window.FaceSelect.SelectedFaces.Count > 0)
			{
				hashSet = this.Window.FaceSelect.SelectedFaces;
				array = new int[hashSet.Count];
				hashSet.CopyTo(array);
			}
			ShapeDeformer component = currentRenderer.GetComponent<ShapeDeformer>();
			if (component != null)
			{
				component.RestoreFullTriangles();
			}
			int subdivideLevel = this.Window.SubdivideLevel;
			bool subdivideSmooth = this.Window.SubdivideSmooth;
			Matrix4x4[] array2 = (subdivideSmooth ? MeshHelper.BuildBodySkinMatrices(currentRenderer, mesh) : null);
			List<List<int[]>> list;
			MeshHelper.Subdivide(mesh, subdivideLevel, hashSet, subdivideSmooth, out list, array2);
			MeshHelper.AppendSubdivisionFaces(mesh, array, subdivideLevel);
			MeshHelper.AppendSubdivisionSmooth(mesh, subdivideSmooth, subdivideLevel);
			ControllerResolver.Owner owner = ControllerResolver.Resolve(currentRenderer);
			DeformData deformData = owner.GetDeformData(currentRenderer);
			if (deformData != null)
			{
				MeshHelper.ResetLayersForNewVertexCount(deformData, mesh.vertexCount);
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						MeshHelper.TransformFaceMaskThroughSubdivision(deformData.DeletedFaces, list[i]);
					}
					if (deformData.DeletedFaces.Count > 0)
					{
						deformData.DeletedFacesDirty = true;
					}
				}
			}
			if (owner.IsOnCharacter)
			{
				string relativePath = ShapeEditorController.GetRelativePath(owner.RootTransform, currentRenderer.transform);
				int subdivisionLevel = MeshHelper.GetSubdivisionLevel(currentRenderer);
				if (subdivisionLevel > 0)
				{
					owner.CharacterController.SetPendingSubLevel(relativePath, subdivisionLevel);
					owner.CharacterController.SetPendingSubFaces(relativePath, MeshHelper.GetSubdivisionFaces(currentRenderer));
					List<bool> subdivisionSmooth = MeshHelper.GetSubdivisionSmooth(currentRenderer);
					owner.CharacterController.SetPendingSubSmooth(relativePath, (subdivisionSmooth != null) ? subdivisionSmooth.ToArray() : null);
					ShapeEditorPlugin.Logger.LogInfo(string.Concat(new string[]
					{
						"Subdivide: synced pending intent for '",
						relativePath,
						"' level=",
						subdivisionLevel.ToString(),
						" (survives coordinate switch)"
					}));
				}
			}
			this.RefreshFaceSelectOverlay(currentRenderer);
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00026D74 File Offset: 0x00024F74
		private void DoRestore()
		{
			Renderer currentRenderer = this.GetCurrentRenderer();
			if (currentRenderer == null)
			{
				return;
			}
			MeshHelper.RestoreOriginal(currentRenderer);
			ControllerResolver.Owner owner = ControllerResolver.Resolve(currentRenderer);
			if (owner.IsOnCharacter)
			{
				string relativePath = ShapeEditorController.GetRelativePath(owner.RootTransform, currentRenderer.transform);
				owner.CharacterController.RestoreSingleRendererAndQueueSubdivide(relativePath, 0, null, null);
			}
			DeformData deformData = owner.GetDeformData(currentRenderer);
			Mesh mesh = MeshHelper.GetMesh(currentRenderer);
			if (mesh != null && deformData != null)
			{
				MeshHelper.ResetLayersForNewVertexCount(deformData, mesh.vertexCount);
			}
			if (deformData != null && (deformData.HasLayers || (deformData.DeletedFaces != null && deformData.DeletedFaces.Count > 0)))
			{
				ShapePaintOverlay.EnsureDeformerAttachedForRenderer(currentRenderer);
			}
			else
			{
				ShapeDeformer.DestroyAttached(currentRenderer);
			}
			this.RefreshFaceSelectOverlay(currentRenderer);
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00026E38 File Offset: 0x00025038
		private void DoRebakeBodySubdivision()
		{
			Renderer currentRenderer = this.GetCurrentRenderer();
			if (currentRenderer == null)
			{
				return;
			}
			List<int[]> subdivisionFaces = MeshHelper.GetSubdivisionFaces(currentRenderer);
			if (subdivisionFaces == null || subdivisionFaces.Count == 0)
			{
				return;
			}
			List<bool> subdivisionSmooth = MeshHelper.GetSubdivisionSmooth(currentRenderer);
			DeformData deformData = ControllerResolver.Resolve(currentRenderer).GetDeformData(currentRenderer);
			MeshHelper.RestoreOriginal(currentRenderer);
			MeshHelper.CloneMeshIfShared(currentRenderer);
			Mesh mesh = MeshHelper.GetMesh(currentRenderer);
			if (mesh == null)
			{
				return;
			}
			Matrix4x4[] array = MeshHelper.BuildBodySkinMatrices(currentRenderer, mesh);
			List<List<int[]>> list;
			MeshHelper.SubdivideReplay(mesh, subdivisionFaces, subdivisionSmooth, out list, array);
			if (deformData != null && (deformData.HasLayers || (deformData.DeletedFaces != null && deformData.DeletedFaces.Count > 0)))
			{
				ShapePaintOverlay.EnsureDeformerAttachedForRenderer(currentRenderer);
			}
			else
			{
				ShapeDeformer.DestroyAttached(currentRenderer);
			}
			if (deformData != null && deformData.DeletedFaces.Count > 0)
			{
				deformData.DeletedFacesDirty = true;
			}
			ShapeEditorPlugin.Logger.LogInfo("Rebake: re-baked body smooth subdivision at current pose for '" + currentRenderer.name + "' (sculpt deltas preserved)");
			this.RefreshFaceSelectOverlay(currentRenderer);
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x00026F38 File Offset: 0x00025138
		private void RefreshFaceSelectOverlay(Renderer renderer)
		{
			if (this.Window.FaceSelect == null)
			{
				return;
			}
			UnityEngine.Object.Destroy(this.Window.FaceSelect.gameObject);
			this.Window.FaceSelect = FaceSelectOverlay.Create(renderer);
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00026F74 File Offset: 0x00025174
		private void PostUndoRedoCleanup()
		{
			this._selectedFacesDirty = true;
			DeformData activeDeformData = this.Window.ActiveDeformData;
			if (activeDeformData != null && activeDeformData.ActiveLayer == null)
			{
				if (this._gizmo != null)
				{
					this._gizmo.ClearTarget();
				}
				if (this.SelectionTool != null)
				{
					this.SelectionTool.ClearSelection();
				}
			}
			else if (this._gizmo != null && this._gizmo.HasTarget)
			{
				this._deferGizmoCentroidRefresh = true;
			}
			this.NotifyLayerStructureChanged();
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00026FEC File Offset: 0x000251EC
		private void CommitGizmoUndoEntry()
		{
			MultiDeltaUndoEntry multiDeltaUndoEntry = ((this._gizmo != null) ? this._gizmo.PendingDragEntry : null);
			if (this._undoStack == null || multiDeltaUndoEntry == null)
			{
				return;
			}
			bool gizmoLazyCreatedThisDrag = this._gizmoLazyCreatedThisDrag;
			this._gizmoLazyCreatedThisDrag = false;
			int rendererCount = this._gizmo.RendererCount;
			bool flag = false;
			int num = 0;
			while (num < rendererCount && num < this._activeRenderers.Count)
			{
				IDictionary<int, Vector3> dragStartDeltas = this._gizmo.GetDragStartDeltas(num);
				if (dragStartDeltas != null && dragStartDeltas.Count != 0)
				{
					DeformData activeDeformData = this.Window.ActiveDeformData;
					if (activeDeformData != null && activeDeformData.ActiveLayer != null)
					{
						DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(this._activeRenderers[num]);
						if (existingDeformData != null)
						{
							DeformLayer deformLayer = ShapePaintOverlay.FindLayerByName(existingDeformData, activeDeformData.ActiveLayer.Name);
							if (deformLayer != null)
							{
								Vector3[] deltas = deformLayer.Deltas;
								int count = dragStartDeltas.Count;
								int[] array = new int[count];
								Vector3[] array2 = new Vector3[count];
								Vector3[] array3 = new Vector3[count];
								int num2 = 0;
								bool flag2 = false;
								foreach (KeyValuePair<int, Vector3> keyValuePair in dragStartDeltas)
								{
									if (keyValuePair.Key >= 0 && keyValuePair.Key < deltas.Length)
									{
										array[num2] = keyValuePair.Key;
										array2[num2] = keyValuePair.Value;
										array3[num2] = deltas[keyValuePair.Key];
										if ((array3[num2] - array2[num2]).sqrMagnitude > 0f)
										{
											flag2 = true;
										}
										num2++;
									}
								}
								if (num2 < count)
								{
									Array.Resize<int>(ref array, num2);
									Array.Resize<Vector3>(ref array2, num2);
									Array.Resize<Vector3>(ref array3, num2);
								}
								if (num2 > 0 && flag2)
								{
									ShapeDeformer shapeDeformer = ((num < this._activeDeformers.Count) ? this._activeDeformers[num] : null);
									multiDeltaUndoEntry.Add(new DeltaUndoEntry(deformLayer, array, array2, array3, shapeDeformer));
									if (shapeDeformer != null)
									{
										multiDeltaUndoEntry.TrackDeformer(shapeDeformer);
									}
									flag = true;
								}
							}
						}
					}
				}
				num++;
			}
			if (flag)
			{
				this._undoStack.Push(multiDeltaUndoEntry);
			}
			else
			{
				multiDeltaUndoEntry.Undo(new UndoContext
				{
					Window = this.Window
				});
			}
			if (gizmoLazyCreatedThisDrag && flag)
			{
				this.NotifyLayerStructureChanged();
			}
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00027274 File Offset: 0x00025474
		private Vector3[] BuildWorldNormalsForFacingTest(int rendererSlot)
		{
			if (rendererSlot < 0 || rendererSlot >= this._activeDeformers.Count)
			{
				return null;
			}
			ShapeDeformer shapeDeformer = this._activeDeformers[rendererSlot];
			if (shapeDeformer == null || shapeDeformer.DisplayTransform == null)
			{
				return null;
			}
			Mesh mesh = shapeDeformer.RequestPosedMesh();
			if (mesh == null)
			{
				return null;
			}
			Vector3[] normals = mesh.normals;
			if (normals == null || normals.Length == 0)
			{
				return null;
			}
			if (this._boxSelectWorldNormals == null)
			{
				this._boxSelectWorldNormals = new List<Vector3[]>();
			}
			while (this._boxSelectWorldNormals.Count <= rendererSlot)
			{
				this._boxSelectWorldNormals.Add(null);
			}
			Vector3[] array = this._boxSelectWorldNormals[rendererSlot];
			if (array == null || array.Length != normals.Length)
			{
				array = new Vector3[normals.Length];
				this._boxSelectWorldNormals[rendererSlot] = array;
			}
			Matrix4x4 localToWorldMatrix = shapeDeformer.DisplayTransform.localToWorldMatrix;
			for (int i = 0; i < normals.Length; i++)
			{
				array[i] = localToWorldMatrix.MultiplyVector(normals[i]);
			}
			return array;
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00027374 File Offset: 0x00025574
		private void ProcessGizmoInteraction(Camera cam, Vector2 mousePos, DeformLayer activeLayer)
		{
			if (this._gizmo == null || activeLayer == null)
			{
				return;
			}
			this._gizmo.UpdateHover(mousePos, cam);
			if (this.Input.MouseButtonDown && !this.Input.CtrlHeld && !this._isBoxSelecting && !this._isBrushSelecting && !this._gizmo.IsDragging && !this.Window.ContainsScreenPoint(global::UnityEngine.Input.mousePosition))
			{
				if (this._gizmo.HoveredAxis != GizmoAxis.None)
				{
					this._gizmo.PendingDragEntry = new MultiDeltaUndoEntry();
					this._gizmo.LazyEnsureLayerCallback = new Func<int, DeformLayer>(this.LazyEnsureLayerForGizmo);
					this._gizmoLazyCreatedThisDrag = false;
					if (!this._gizmo.BeginDrag(mousePos, cam))
					{
						this._gizmo.PendingDragEntry = null;
						this._gizmo.LazyEnsureLayerCallback = null;
					}
				}
				else if (this.Window.GizmoSelection == ShapeEditorWindow.GizmoSelectMethod.Brush)
				{
					this._isBrushSelecting = true;
				}
				else
				{
					this._isBoxSelecting = true;
					this._boxStart = mousePos;
					this._boxEnd = mousePos;
				}
			}
			if (!this._isBoxSelecting)
			{
				if (this._isBrushSelecting)
				{
					if (!this.Input.MouseButton)
					{
						this._isBrushSelecting = false;
						return;
					}
					if (this._hasHit && !this.Input.CtrlHeld)
					{
						bool value = ShapeEditorPlugin.IncludeBackFaceVertices.Value;
						bool altHeld = this.Input.AltHeld;
						float brushRadius = this.Window.BrushRadius;
						bool flag = this.Window.SelectionMode == ShapeEditorWindow.SelectMode.Face;
						bool flag2 = false;
						for (int i = 0; i < this._activeSelTools.Count; i++)
						{
							SelectionTool selectionTool = this._activeSelTools[i];
							if (selectionTool != null)
							{
								Vector3[] array = (value ? null : this.BuildWorldNormalsForFacingTest(i));
								if (flag ? selectionTool.BrushAccumulateFaces(cam, this._lastHitPoint, brushRadius, altHeld, value, array) : selectionTool.BrushAccumulate(cam, this._lastHitPoint, brushRadius, altHeld, value, array))
								{
									flag2 = true;
								}
							}
						}
						if (flag2)
						{
							this.UpdateGizmoTarget();
							return;
						}
					}
				}
				else if (this._gizmo.IsDragging)
				{
					if (this.Input.MouseButton)
					{
						this._gizmo.UpdateDrag(mousePos, cam);
						return;
					}
					this._gizmo.EndDrag();
					this.CommitGizmoUndoEntry();
					this._gizmo.PendingDragEntry = null;
					this._gizmo.LazyEnsureLayerCallback = null;
				}
				return;
			}
			if (this.Input.MouseButton)
			{
				this._boxEnd = mousePos;
				return;
			}
			this._isBoxSelecting = false;
			float num = Mathf.Min(this._boxStart.x, this._boxEnd.x);
			float num2 = Mathf.Max(this._boxStart.x, this._boxEnd.x);
			float num3 = Mathf.Min(this._boxStart.y, this._boxEnd.y);
			float num4 = Mathf.Max(this._boxStart.y, this._boxEnd.y);
			Rect rect;
			rect = new Rect(num, num3, num2 - num, num4 - num3);
			bool flag3 = this.Window.SelectionMode == ShapeEditorWindow.SelectMode.Face;
			for (int j = 0; j < this._activeSelTools.Count; j++)
			{
				SelectionTool selectionTool2 = this._activeSelTools[j];
				if (selectionTool2 != null)
				{
					bool value2 = ShapeEditorPlugin.IncludeBackFaceVertices.Value;
					Vector3[] array2 = this.BuildWorldNormalsForFacingTest(j);
					if (this.Input.AltHeld)
					{
						if (flag3)
						{
							selectionTool2.DeselectFacesBox(cam, rect, value2, array2);
						}
						else
						{
							selectionTool2.DeselectBox(cam, rect, value2, array2);
						}
					}
					else if (flag3)
					{
						selectionTool2.SelectFacesBox(cam, rect, this.Input.ShiftHeld, value2, array2);
					}
					else
					{
						selectionTool2.SelectBox(cam, rect, this.Input.ShiftHeld, value2, array2);
					}
				}
			}
			this.UpdateGizmoTarget();
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x0002773C File Offset: 0x0002593C
		private void UpdateGizmoTarget()
		{
			if (this._gizmo == null || this._activeRenderers.Count == 0)
			{
				return;
			}
			this._selectedFacesDirty = true;
			int count = this._activeRenderers.Count;
			List<HashSet<int>> list = new List<HashSet<int>>(count);
			List<Vector3[]> list2 = new List<Vector3[]>(count);
			List<Vector3[]> list3 = new List<Vector3[]>(count);
			List<Transform> list4 = new List<Transform>(count);
			List<ShapeDeformer> list5 = new List<ShapeDeformer>(count);
			List<DeformLayer> list6 = new List<DeformLayer>(count);
			List<SpatialHashGrid> list7 = new List<SpatialHashGrid>(count);
			List<List<int>[]> list8 = new List<List<int>[]>(count);
			DeformData activeDeformData = this.Window.ActiveDeformData;
			string text = ((activeDeformData != null && activeDeformData.ActiveLayer != null) ? activeDeformData.ActiveLayer.Name : null);
			for (int i = 0; i < count; i++)
			{
				SelectionTool selectionTool = this._activeSelTools[i];
				list.Add((selectionTool != null) ? new HashSet<int>(selectionTool.SelectedVertices) : new HashSet<int>());
				list2.Add((selectionTool != null) ? selectionTool.CachedVertices : null);
				list3.Add((selectionTool != null) ? selectionTool.CachedNormals : null);
				list4.Add((this._activeRenderers[i] != null) ? this._activeRenderers[i].transform : null);
				list5.Add((i < this._activeDeformers.Count) ? this._activeDeformers[i] : null);
				list7.Add((selectionTool != null) ? selectionTool.Grid : null);
				list8.Add((this._activeSmoothTools[i] != null) ? this._activeSmoothTools[i].Adjacency : null);
				DeformLayer deformLayer = null;
				if (text != null)
				{
					DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(this._activeRenderers[i]);
					if (existingDeformData != null)
					{
						deformLayer = ShapePaintOverlay.FindLayerByName(existingDeformData, text);
					}
				}
				list6.Add(deformLayer);
			}
			this._gizmo.SetTarget(list, list2, list3, list4, list5, list6, list7, list8, this._primaryListIdx);
			if (this._symmetryEnabled && this._gizmo.HasTarget)
			{
				List<HashSet<int>> list9 = new List<HashSet<int>>(count);
				for (int j = 0; j < count; j++)
				{
					HashSet<int> setI = new HashSet<int>();
					SelectionTool selectionTool2 = this._activeSelTools[j];
					SpatialHashGrid spatialHashGrid = ((selectionTool2 != null) ? selectionTool2.Grid : null);
					Transform transform = ((this._activeRenderers[j] != null) ? this._activeRenderers[j].transform : null);
					Vector3[] array = ((selectionTool2 != null) ? selectionTool2.CachedVertices : null);
					if (spatialHashGrid == null || transform == null || array == null)
					{
						list9.Add(setI);
					}
					else
					{
						HashSet<int> selected = list[j];
						foreach (int num in selected)
						{
							if (num >= 0 && num < array.Length)
							{
								Vector3 vector = transform.TransformPoint(array[num]);
								Vector3 vector2 = this._gizmo.MirrorPointWorld(vector);
								Vector3 vector3 = transform.InverseTransformPoint(vector2);
								SpatialHashGrid spatialHashGrid2 = spatialHashGrid;
								Vector3 vector4 = vector3;
								float num2 = 0.0002f;
								spatialHashGrid2.FindVerticesInRadius(vector4, num2, (int found, float distSq) =>
								{
									if (!selected.Contains(found))
									{
										setI.Add(found);
									}
								});
							}
						}
						list9.Add(setI);
					}
				}
				this._gizmo.SetMirrorTarget(list9);
			}
			if (this._gizmo.SoftSelectionEnabled && this._gizmo.HasTarget)
			{
				this._gizmo.ComputeSoftWeights();
			}
			if (this._symmetryEnabled && this._gizmo.HasMirrorTarget && this._gizmo.SoftSelectionEnabled)
			{
				this._gizmo.ComputeMirrorSoftWeights();
			}
			this.InvalidateAllWireColors();
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x00027B28 File Offset: 0x00025D28
		private DeformLayer LazyEnsureLayerForGizmo(int rendererIdx)
		{
			if (rendererIdx < 0 || rendererIdx >= this._activeRenderers.Count)
			{
				return null;
			}
			DeformData activeDeformData = this.Window.ActiveDeformData;
			if (activeDeformData == null || activeDeformData.ActiveLayer == null)
			{
				return null;
			}
			string name = activeDeformData.ActiveLayer.Name;
			Renderer renderer = this._activeRenderers[rendererIdx];
			DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(renderer);
			if (existingDeformData == null)
			{
				return null;
			}
			DeformLayer deformLayer = ShapePaintOverlay.FindLayerByName(existingDeformData, name);
			if (deformLayer != null)
			{
				return deformLayer;
			}
			Mesh mesh = MeshHelper.GetMesh(renderer);
			if (mesh == null)
			{
				return null;
			}
			DeformLayer deformLayer2 = new DeformLayer(name, mesh.vertexCount);
			existingDeformData.Layers.Add(deformLayer2);
			int num = existingDeformData.Layers.Count - 1;
			MultiDeltaUndoEntry multiDeltaUndoEntry = ((this._gizmo != null) ? this._gizmo.PendingDragEntry : null);
			if (multiDeltaUndoEntry != null)
			{
				multiDeltaUndoEntry.Add(new LayerAddUndoEntry(existingDeformData, deformLayer2, num));
				if (rendererIdx < this._activeDeformers.Count)
				{
					multiDeltaUndoEntry.TrackDeformer(this._activeDeformers[rendererIdx]);
				}
			}
			if (rendererIdx < this._activeDeformers.Count && this._activeDeformers[rendererIdx] != null)
			{
				this._activeDeformers[rendererIdx].InvalidateDeltaCache();
			}
			this._gizmoLazyCreatedThisDrag = true;
			return deformLayer2;
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x00027C60 File Offset: 0x00025E60
		private IDeformTool GetActiveBrushTool()
		{
			switch (this.Window.SelectedBrushTool)
			{
			case ShapeEditorWindow.BrushToolType.Move:
				return this._moveTool;
			case ShapeEditorWindow.BrushToolType.Smooth:
			case ShapeEditorWindow.BrushToolType.Relax:
				return this._smoothTool;
			case ShapeEditorWindow.BrushToolType.Inflate:
				return this._inflateTool;
			case ShapeEditorWindow.BrushToolType.Pinch:
			case ShapeEditorWindow.BrushToolType.Crease:
				return this._pinchTool;
			default:
				return this._moveTool;
			}
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x00027CBC File Offset: 0x00025EBC
		private void LateUpdate()
		{
			if (this._targetRenderer == null || this.SelectionTool == null)
			{
				return;
			}
			if (this.Window == null || !this.Window.IsEditMode)
			{
				return;
			}
			bool flag = false;
			this._refreshTimer += Time.deltaTime;
			if (this._refreshTimer >= 0.5f)
			{
				this._refreshTimer = 0f;
				flag = true;
			}
			int count = this._activeSelTools.Count;
			while (this._colliderCache.Count < count)
			{
				this._colliderCache.Add(default(ShapePaintOverlay.ColliderCacheSlot));
			}
			while (this._colliderCache.Count > count)
			{
				this._colliderCache.RemoveAt(this._colliderCache.Count - 1);
			}
			try
			{
				for (int i = 0; i < count; i++)
				{
					SelectionTool selectionTool = this._activeSelTools[i];
					ShapeDeformer shapeDeformer = ((i < this._activeDeformers.Count) ? this._activeDeformers[i] : null);
					if (selectionTool != null)
					{
						Mesh mesh = ((shapeDeformer != null) ? shapeDeformer.RequestPosedMesh() : null);
						if (mesh != null)
						{
							int instanceID = mesh.GetInstanceID();
							int lastVertsHash = shapeDeformer.LastVertsHash;
							ShapePaintOverlay.ColliderCacheSlot colliderCacheSlot = this._colliderCache[i];
							if (!colliderCacheSlot.EverRefreshed || colliderCacheSlot.MeshId != instanceID || colliderCacheSlot.Hash != lastVertsHash)
							{
								selectionTool.RefreshCollider(mesh);
								this._colliderCache[i] = new ShapePaintOverlay.ColliderCacheSlot
								{
									Hash = lastVertsHash,
									MeshId = instanceID,
									EverRefreshed = true
								};
							}
						}
						else if (flag)
						{
							selectionTool.RefreshCollider();
							ShapePaintOverlay.ColliderCacheSlot colliderCacheSlot2 = this._colliderCache[i];
							colliderCacheSlot2.EverRefreshed = false;
							this._colliderCache[i] = colliderCacheSlot2;
						}
					}
				}
			}
			finally
			{
			}
			if ((this._deformer != null && this._deformer.DisplayMesh != null) || flag)
			{
				this.RefreshWireframe();
			}
			if (this._gizmo != null && this._activeSelTools.Count > 0)
			{
				if (this._gizmoFrameVertsBuf == null || this._gizmoFrameVertsBuf.Capacity < this._activeSelTools.Count)
				{
					this._gizmoFrameVertsBuf = new List<Vector3[]>(this._activeSelTools.Count);
				}
				this._gizmoFrameVertsBuf.Clear();
				for (int j = 0; j < this._activeSelTools.Count; j++)
				{
					this._gizmoFrameVertsBuf.Add((this._activeSelTools[j] != null) ? this._activeSelTools[j].CachedVertices : null);
				}
				this._gizmo.RefreshFrameVertices(this._gizmoFrameVertsBuf);
			}
			if (this._deferGizmoCentroidRefresh)
			{
				this._deferGizmoCentroidRefresh = false;
				if (this._gizmo != null)
				{
					this._gizmo.UpdateCentroid();
				}
			}
			Camera main = Camera.main;
			if (main == null || this.Input == null)
			{
				return;
			}
			if (this._targetRenderer != null)
			{
				ControllerResolver.Owner owner = ControllerResolver.Resolve(this._targetRenderer);
				this.Window.IsOnCharacter = owner.IsOnCharacter;
				this.Window.BodyMeshReadable = owner.IsOnCharacter && owner.CharacterController.GetBodySmr() != null;
			}
			else
			{
				this.Window.IsOnCharacter = false;
				this.Window.BodyMeshReadable = false;
			}
			bool value = ShapeEditorPlugin.UsePressure.Value;
			bool flag2 = this.Window.OperationMode == ShapeEditorWindow.OpMode.Brush && value;
			if (flag2 && !this._pressureSubscribed)
			{
				this.EnsurePressureSubscription();
			}
			else if (!flag2 && this._pressureSubscribed)
			{
				this.EnsurePressureUnsubscribed();
				this._lastPressure01 = 1f;
			}
			this.ProcessBrushAdjust();
			float num = this.Window.BrushStrength * (value ? this._lastPressure01 : 1f);
			for (int k = 0; k < this._activeSelTools.Count; k++)
			{
				SelectionTool selectionTool2 = this._activeSelTools[k];
				if (selectionTool2 != null)
				{
					selectionTool2.Radius = this.Window.BrushRadius;
					selectionTool2.Strength = num;
					selectionTool2.Falloff = this.Window.BrushFalloff;
					ShapeDeformer shapeDeformer2 = ((k < this._activeDeformers.Count) ? this._activeDeformers[k] : null);
					selectionTool2.VertexLiveMask = ((shapeDeformer2 != null) ? shapeDeformer2.LiveVertexMask : null);
					SmoothTool smoothTool = ((k < this._activeSmoothTools.Count) ? this._activeSmoothTools[k] : null);
					if (smoothTool != null)
					{
						smoothTool.Mode = ((this.Window.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Relax) ? SmoothTool.SmoothMode.Relax : SmoothTool.SmoothMode.Smooth);
					}
				}
			}
			if (this._pinchTool != null)
			{
				this._pinchTool.Mode = ((this.Window.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Crease) ? PinchTool.PinchMode.Crease : PinchTool.PinchMode.Pinch);
			}
			if (this._gizmo != null)
			{
				bool softSelectionEnabled = this._gizmo.SoftSelectionEnabled;
				float softSelectionRadius = this._gizmo.SoftSelectionRadius;
				SoftSelectMode softMode = this._gizmo.SoftMode;
				this._gizmo.Mode = (GizmoMode)this.Window.GizmoModeIndex;
				this._gizmo.Space = (GizmoSpace)this.Window.GizmoSpaceIndex;
				this._gizmo.SoftSelectionEnabled = this.Window.GizmoSoftSelection;
				this._gizmo.SoftSelectionRadius = this.Window.GizmoSoftRadius;
				this._gizmo.SoftFalloff = this.Window.GizmoFalloff;
				this._gizmo.SoftMode = (SoftSelectMode)this.Window.SoftSelectModeIndex;
				if (this._gizmo.HasTarget && (softSelectionEnabled != this._gizmo.SoftSelectionEnabled || !Mathf.Approximately(softSelectionRadius, this._gizmo.SoftSelectionRadius) || softMode != this._gizmo.SoftMode))
				{
					this._softWeightsDirtyTime = Time.unscaledTime;
					this._softWeightsDirty = true;
				}
				if (this._softWeightsDirty && Time.unscaledTime - this._softWeightsDirtyTime >= 0.15f)
				{
					this._softWeightsDirty = false;
					this._gizmo.ComputeSoftWeights();
					if (this._symmetryEnabled && this._gizmo.HasMirrorTarget)
					{
						this._gizmo.ComputeMirrorSoftWeights();
					}
					this.InvalidateAllWireColors();
				}
			}
			bool symmetryEnabled = this._symmetryEnabled;
			int symmetryAxis = this._symmetryAxis;
			float symmetryCenter = this._symmetryCenter;
			bool symmetryCenterSet = this._symmetryCenterSet;
			this._symmetryEnabled = this.Window.SymmetryEnabled;
			this._symmetryAxis = this.Window.SymmetryAxisIndex;
			this._symmetryCenter = this.Window.SymmetryCenter;
			this._symmetryCenterSet = this.Window.SymmetryCenterSet;
			if (this._gizmo != null)
			{
				this._gizmo.SymmetryEnabled = this._symmetryEnabled;
				this._gizmo.SymmetryAxis = this._symmetryAxis;
				this._gizmo.SymmetryCenter = (this._symmetryCenterSet ? this._symmetryCenter : 0f);
				if (!this._symmetryEnabled && symmetryEnabled && this._gizmo.HasMirrorTarget)
				{
					this._gizmo.ClearMirrorTarget();
				}
				if ((this._symmetryEnabled != symmetryEnabled || this._symmetryAxis != symmetryAxis || this._symmetryCenterSet != symmetryCenterSet || !Mathf.Approximately(this._symmetryCenter, symmetryCenter)) && this._symmetryEnabled && this._gizmo.HasTarget)
				{
					this.UpdateGizmoTarget();
				}
			}
			if (this._undoStack != null && this.Window.IsEditMode)
			{
				if (this.Input.UndoPressed && this._undoStack.CanUndo)
				{
					UndoContext undoContext = new UndoContext
					{
						Deformer = this._deformer,
						Data = this.Window.ActiveDeformData,
						Window = this.Window
					};
					this._undoStack.Undo(undoContext);
					this.PostUndoRedoCleanup();
				}
				else if (this.Input.RedoPressed && this._undoStack.CanRedo)
				{
					UndoContext undoContext2 = new UndoContext
					{
						Deformer = this._deformer,
						Data = this.Window.ActiveDeformData,
						Window = this.Window
					};
					this._undoStack.Redo(undoContext2);
					this.PostUndoRedoCleanup();
				}
			}
			if (this.Window.OperationMode == ShapeEditorWindow.OpMode.Gizmo && (this.Input.GrowPressed || this.Input.ShrinkPressed))
			{
				bool growPressed = this.Input.GrowPressed;
				bool flag3 = this.Window.SelectionMode == ShapeEditorWindow.SelectMode.Face;
				for (int l = 0; l < this._activeSelTools.Count; l++)
				{
					SelectionTool selectionTool3 = this._activeSelTools[l];
					if (selectionTool3 != null)
					{
						List<int>[] array = ((l < this._activeSmoothTools.Count && this._activeSmoothTools[l] != null) ? this._activeSmoothTools[l].Adjacency : null);
						if (growPressed)
						{
							selectionTool3.GrowSelection(array);
						}
						else
						{
							selectionTool3.ShrinkSelection(array, flag3);
						}
					}
				}
				this.UpdateGizmoTarget();
			}
			if (ShapeEditorWindow.IsMouseOverUI)
			{
				this._hasHit = false;
				return;
			}
			Vector3 mousePosition = this.Input.MousePosition;
			Ray ray = main.ScreenPointToRay(mousePosition);
			bool flag4 = false;
			if (this.Window.OperationMode == ShapeEditorWindow.OpMode.Brush)
			{
				Vector3 vector;
				Vector3 vector2;
				if (this.MultiRaycast(ray, out vector, out vector2))
				{
					this._lastHitPoint = vector;
					this._lastHitNormal = vector2;
					this._lastValidHitPoint = vector;
					this._lastValidHitNormal = vector2;
					this._hasHit = true;
				}
				else if (!this._isBrushing)
				{
					this._hasHit = false;
				}
				else if (this.Window.SelectedBrushTool == ShapeEditorWindow.BrushToolType.Move)
				{
					Vector3 forward = main.transform.forward;
					Plane plane;
					plane = new Plane(forward, this._strokeStartHitPoint);
					float num2;
					if (plane.Raycast(ray, out num2))
					{
						this._lastHitPoint = ray.GetPoint(num2);
						this._lastHitNormal = this._strokeStartHitNormal;
						this._hasHit = true;
					}
					else
					{
						this._lastHitPoint = this._lastValidHitPoint;
						this._lastHitNormal = this._lastValidHitNormal;
						this._hasHit = true;
					}
				}
				else
				{
					this._lastHitPoint = this._lastValidHitPoint;
					this._lastHitNormal = this._lastValidHitNormal;
					this._hasHit = true;
					flag4 = true;
				}
			}
			else if (this.Window.GizmoSelection == ShapeEditorWindow.GizmoSelectMethod.Brush)
			{
				Vector3 vector3;
				Vector3 vector4;
				if (this.MultiRaycast(ray, out vector3, out vector4))
				{
					this._lastHitPoint = vector3;
					this._lastHitNormal = vector4;
					this._hasHit = true;
				}
				else
				{
					this._hasHit = false;
				}
			}
			else
			{
				this._hasHit = false;
			}
			DeformData activeDeformData = this.Window.ActiveDeformData;
			if (activeDeformData == null)
			{
				return;
			}
			DeformLayer activeLayer = activeDeformData.ActiveLayer;
			if (this.Window.OperationMode == ShapeEditorWindow.OpMode.Brush)
			{
				try
				{
					if (!flag4)
					{
						this.ProcessBrushInteraction(main, ray, mousePosition, activeLayer);
					}
					if (this._isBrushing && !this.Input.MouseButton)
					{
						this._isBrushing = false;
						this.CommitBrushUndoEntry(activeLayer);
					}
					goto IL_0AFA;
				}
				finally
				{
				}
			}
			if (this._isBrushing)
			{
				this._isBrushing = false;
				this.CommitBrushUndoEntry(activeLayer);
			}
			this.ProcessGizmoInteraction(main, mousePosition, activeLayer);
			IL_0AFA:
			this._prevMousePos = new Vector2(mousePosition.x, mousePosition.y);
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00028810 File Offset: 0x00026A10
		private void DoLayerAdd()
		{
			if (!this.Window.IsEditMode)
			{
				if (this.Window.ActiveDeformData == null)
				{
					if (this.Window.Renderers.Count == 0)
					{
						return;
					}
					int num = Mathf.Clamp(this.Window.PrimaryRendererIndex, 0, this.Window.Renderers.Count - 1);
					Renderer renderer = this.Window.Renderers[num];
					if (renderer == null)
					{
						return;
					}
					bool flag;
					DeformData deformDataForRenderer = ShapePaintOverlay.GetDeformDataForRenderer(renderer, out flag);
					if (deformDataForRenderer == null)
					{
						return;
					}
					this.Window.ActiveDeformData = deformDataForRenderer;
				}
				Mesh mesh = MeshHelper.GetMesh(this.GetCurrentRenderer());
				if (mesh == null)
				{
					return;
				}
				DeformLayer deformLayer = this.Window.ActiveDeformData.AddLayer(mesh.vertexCount);
				if (this._undoStack != null)
				{
					this._undoStack.Push(new LayerAddUndoEntry(this.Window.ActiveDeformData, deformLayer, this.Window.ActiveDeformData.Layers.Count - 1));
				}
				this.NotifyLayerStructureChanged();
				return;
			}
			else
			{
				if (this._activeRenderers.Count == 0 || this._primaryListIdx < 0)
				{
					return;
				}
				DeformData activeDeformData = this.Window.ActiveDeformData;
				if (activeDeformData == null)
				{
					return;
				}
				string text = activeDeformData.GenerateUniqueLayerName();
				MultiDeltaUndoEntry multiDeltaUndoEntry = new MultiDeltaUndoEntry();
				DeformLayer deformLayer2 = null;
				for (int i = 0; i < this._activeRenderers.Count; i++)
				{
					Renderer renderer2 = this._activeRenderers[i];
					DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(renderer2);
					if (existingDeformData != null && ShapePaintOverlay.FindLayerByName(existingDeformData, text) == null)
					{
						Mesh mesh2 = MeshHelper.GetMesh(renderer2);
						if (!(mesh2 == null))
						{
							DeformLayer deformLayer3 = new DeformLayer(text, mesh2.vertexCount);
							existingDeformData.Layers.Add(deformLayer3);
							int num2 = existingDeformData.Layers.Count - 1;
							if (i == this._primaryListIdx)
							{
								existingDeformData.ActiveLayerIndex = num2;
								deformLayer2 = deformLayer3;
							}
							multiDeltaUndoEntry.Add(new LayerAddUndoEntry(existingDeformData, deformLayer3, num2));
							multiDeltaUndoEntry.TrackDeformer(this._activeDeformers[i]);
						}
					}
				}
				if (this._undoStack != null && multiDeltaUndoEntry.ChildCount > 0)
				{
					this._undoStack.Push(multiDeltaUndoEntry);
				}
				this.NotifyLayerStructureChanged();
				if (deformLayer2 == null && activeDeformData.Layers.Count > 0)
				{
					activeDeformData.ActiveLayerIndex = activeDeformData.Layers.Count - 1;
				}
				return;
			}
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x00028A64 File Offset: 0x00026C64
		private void DoLayerRemove(int layerIndex)
		{
			if (!this.Window.IsEditMode)
			{
				if (this.Window.ActiveDeformData == null)
				{
					return;
				}
				DeformData activeDeformData = this.Window.ActiveDeformData;
				DeformLayer deformLayer = null;
				int activeLayerIndex = activeDeformData.ActiveLayerIndex;
				if (layerIndex >= 0 && layerIndex < activeDeformData.Layers.Count)
				{
					deformLayer = activeDeformData.Layers[layerIndex];
				}
				activeDeformData.RemoveLayer(layerIndex);
				if (this._undoStack != null && deformLayer != null)
				{
					this._undoStack.Push(new LayerRemoveUndoEntry(activeDeformData, deformLayer, layerIndex, activeLayerIndex));
				}
				if (this._deformer != null)
				{
					this._deformer.InvalidateDeltaCache();
				}
				this.NotifyLayerStructureChanged();
				return;
			}
			else
			{
				if (this._activeRenderers.Count == 0 || this._primaryListIdx < 0)
				{
					return;
				}
				DeformData activeDeformData2 = this.Window.ActiveDeformData;
				if (activeDeformData2 == null)
				{
					return;
				}
				if (layerIndex < 0 || layerIndex >= activeDeformData2.Layers.Count)
				{
					return;
				}
				string name = activeDeformData2.Layers[layerIndex].Name;
				MultiDeltaUndoEntry multiDeltaUndoEntry = new MultiDeltaUndoEntry();
				for (int i = 0; i < this._activeRenderers.Count; i++)
				{
					DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(this._activeRenderers[i]);
					if (existingDeformData != null)
					{
						int num = -1;
						for (int j = 0; j < existingDeformData.Layers.Count; j++)
						{
							if (existingDeformData.Layers[j].Name == name)
							{
								num = j;
								break;
							}
						}
						if (num >= 0)
						{
							DeformLayer deformLayer2 = existingDeformData.Layers[num];
							int activeLayerIndex2 = existingDeformData.ActiveLayerIndex;
							existingDeformData.RemoveLayer(num);
							multiDeltaUndoEntry.Add(new LayerRemoveUndoEntry(existingDeformData, deformLayer2, num, activeLayerIndex2));
							multiDeltaUndoEntry.TrackDeformer(this._activeDeformers[i]);
							if (this._activeDeformers[i] != null)
							{
								this._activeDeformers[i].InvalidateDeltaCache();
							}
						}
					}
				}
				if (this._undoStack != null && multiDeltaUndoEntry.ChildCount > 0)
				{
					this._undoStack.Push(multiDeltaUndoEntry);
				}
				if (this.Window.ActiveDeformData != null && this.Window.ActiveDeformData.ActiveLayer == null)
				{
					if (this._gizmo != null)
					{
						this._gizmo.ClearTarget();
					}
					if (this.SelectionTool != null)
					{
						this.SelectionTool.ClearSelection();
					}
				}
				else if (this._gizmo != null && this._gizmo.HasTarget)
				{
					this._deferGizmoCentroidRefresh = true;
				}
				this.NotifyLayerStructureChanged();
				return;
			}
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00028CD8 File Offset: 0x00026ED8
		private void DoLayerRename(int layerIndex, string newName)
		{
			if (string.IsNullOrEmpty(newName))
			{
				return;
			}
			if (!this.Window.IsEditMode)
			{
				if (this.Window.ActiveDeformData == null)
				{
					return;
				}
				DeformData activeDeformData = this.Window.ActiveDeformData;
				if (layerIndex < 0 || layerIndex >= activeDeformData.Layers.Count)
				{
					return;
				}
				if (activeDeformData.Layers[layerIndex].Name == newName)
				{
					return;
				}
				DeformLayer deformLayer = activeDeformData.Layers[layerIndex];
				string name = deformLayer.Name;
				activeDeformData.RenameLayer(layerIndex, newName);
				if (this._undoStack != null)
				{
					this._undoStack.Push(new LayerRenameUndoEntry(deformLayer, name, newName));
				}
				this.NotifyLayerStructureChanged();
				return;
			}
			else
			{
				if (this._activeRenderers.Count == 0 || this._primaryListIdx < 0)
				{
					return;
				}
				DeformData activeDeformData2 = this.Window.ActiveDeformData;
				if (activeDeformData2 == null)
				{
					return;
				}
				if (layerIndex < 0 || layerIndex >= activeDeformData2.Layers.Count)
				{
					return;
				}
				string name2 = activeDeformData2.Layers[layerIndex].Name;
				if (name2 == newName)
				{
					return;
				}
				for (int i = 0; i < this._activeRenderers.Count; i++)
				{
					DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(this._activeRenderers[i]);
					if (existingDeformData != null)
					{
						for (int j = 0; j < existingDeformData.Layers.Count; j++)
						{
							if (existingDeformData.Layers[j].Name == newName && existingDeformData.Layers[j].Name != name2)
							{
								ShapeEditorPlugin.Logger.LogWarning(string.Concat(new string[]
								{
									"Rename rejected: layer name '",
									newName,
									"' already exists on renderer '",
									this._activeRenderers[i].name,
									"'"
								}));
								return;
							}
						}
					}
				}
				MultiDeltaUndoEntry multiDeltaUndoEntry = new MultiDeltaUndoEntry();
				for (int k = 0; k < this._activeRenderers.Count; k++)
				{
					DeformData existingDeformData2 = ShapePaintOverlay.GetExistingDeformData(this._activeRenderers[k]);
					if (existingDeformData2 != null)
					{
						for (int l = 0; l < existingDeformData2.Layers.Count; l++)
						{
							if (existingDeformData2.Layers[l].Name == name2)
							{
								DeformLayer deformLayer2 = existingDeformData2.Layers[l];
								existingDeformData2.RenameLayer(l, newName);
								multiDeltaUndoEntry.Add(new LayerRenameUndoEntry(deformLayer2, name2, newName));
								multiDeltaUndoEntry.TrackDeformer(this._activeDeformers[k]);
								break;
							}
						}
					}
				}
				if (this._undoStack != null && multiDeltaUndoEntry.ChildCount > 0)
				{
					this._undoStack.Push(multiDeltaUndoEntry);
				}
				this.NotifyLayerStructureChanged();
				return;
			}
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00028F8C File Offset: 0x0002718C
		private void DoLayerMove(int layerIndex, bool up)
		{
			if (!this.Window.IsEditMode)
			{
				if (this.Window.ActiveDeformData == null)
				{
					return;
				}
				if (up)
				{
					this.Window.ActiveDeformData.MoveLayerUp(layerIndex);
				}
				else
				{
					this.Window.ActiveDeformData.MoveLayerDown(layerIndex);
				}
				if (this._undoStack != null)
				{
					this._undoStack.Push(new LayerReorderUndoEntry(this.Window.ActiveDeformData, up));
				}
				if (this._deformer != null)
				{
					this._deformer.InvalidateDeltaCache();
				}
				return;
			}
			else
			{
				if (this._activeRenderers.Count == 0 || this._primaryListIdx < 0)
				{
					return;
				}
				DeformData activeDeformData = this.Window.ActiveDeformData;
				if (activeDeformData == null)
				{
					return;
				}
				DeformLayer[] array = activeDeformData.Layers.ToArray();
				int activeLayerIndex = activeDeformData.ActiveLayerIndex;
				if (up)
				{
					activeDeformData.MoveLayerUp(layerIndex);
				}
				else
				{
					activeDeformData.MoveLayerDown(layerIndex);
				}
				DeformLayer[] array2 = activeDeformData.Layers.ToArray();
				int activeLayerIndex2 = activeDeformData.ActiveLayerIndex;
				MultiDeltaUndoEntry multiDeltaUndoEntry = new MultiDeltaUndoEntry();
				multiDeltaUndoEntry.Add(new LayerOrderUndoEntry(activeDeformData, array, array2, activeLayerIndex, activeLayerIndex2));
				multiDeltaUndoEntry.TrackDeformer(this._activeDeformers[this._primaryListIdx]);
				for (int i = 0; i < this._activeRenderers.Count; i++)
				{
					if (i != this._primaryListIdx)
					{
						DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(this._activeRenderers[i]);
						if (existingDeformData != null && existingDeformData.Layers.Count != 0)
						{
							DeformLayer[] array3 = existingDeformData.Layers.ToArray();
							int activeLayerIndex3 = existingDeformData.ActiveLayerIndex;
							ShapePaintOverlay.RealignLayerOrderToReference(existingDeformData, activeDeformData);
							DeformLayer[] array4 = existingDeformData.Layers.ToArray();
							int activeLayerIndex4 = existingDeformData.ActiveLayerIndex;
							if (!ShapePaintOverlay.OrderEquals(array3, array4))
							{
								multiDeltaUndoEntry.Add(new LayerOrderUndoEntry(existingDeformData, array3, array4, activeLayerIndex3, activeLayerIndex4));
								multiDeltaUndoEntry.TrackDeformer(this._activeDeformers[i]);
							}
						}
					}
				}
				if (this._undoStack != null && multiDeltaUndoEntry.ChildCount > 0)
				{
					this._undoStack.Push(multiDeltaUndoEntry);
				}
				return;
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00029188 File Offset: 0x00027388
		private static void RealignLayerOrderToReference(DeformData target, DeformData reference)
		{
			Dictionary<string, int> refOrder = new Dictionary<string, int>();
			for (int i = 0; i < reference.Layers.Count; i++)
			{
				string name = reference.Layers[i].Name;
				if (!refOrder.ContainsKey(name))
				{
					refOrder[name] = i;
				}
			}
			DeformLayer deformLayer = ((target.ActiveLayerIndex >= 0 && target.ActiveLayerIndex < target.Layers.Count) ? target.Layers[target.ActiveLayerIndex] : null);
			List<DeformLayer> list = new List<DeformLayer>();
			List<DeformLayer> list2 = new List<DeformLayer>();
			for (int j = 0; j < target.Layers.Count; j++)
			{
				DeformLayer deformLayer2 = target.Layers[j];
				if (refOrder.ContainsKey(deformLayer2.Name))
				{
					list.Add(deformLayer2);
				}
				else
				{
					list2.Add(deformLayer2);
				}
			}
			list.Sort((DeformLayer a, DeformLayer b) => refOrder[a.Name].CompareTo(refOrder[b.Name]));
			target.Layers.Clear();
			for (int k = 0; k < list.Count; k++)
			{
				target.Layers.Add(list[k]);
			}
			for (int l = 0; l < list2.Count; l++)
			{
				target.Layers.Add(list2[l]);
			}
			if (deformLayer != null)
			{
				int num = target.Layers.IndexOf(deformLayer);
				if (num < 0)
				{
					ShapeEditorPlugin.Logger.LogWarning("RealignLayerOrderToReference: active layer reference vanished after reorder; resetting to first layer.");
					target.ActiveLayerIndex = ((target.Layers.Count > 0) ? 0 : (-1));
					return;
				}
				target.ActiveLayerIndex = num;
			}
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00029334 File Offset: 0x00027534
		private static bool OrderEquals(DeformLayer[] a, DeformLayer[] b)
		{
			if (a == null || b == null || a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x0002936C File Offset: 0x0002756C
		private void DoLayerWeightCommit(int primaryLayerIndex, float before, float after)
		{
			if (this.Window.ActiveDeformData == null)
			{
				return;
			}
			if (primaryLayerIndex < 0 || primaryLayerIndex >= this.Window.ActiveDeformData.Layers.Count)
			{
				return;
			}
			string name = this.Window.ActiveDeformData.Layers[primaryLayerIndex].Name;
			if (!this.Window.IsEditMode || this._activeRenderers.Count == 0)
			{
				if (this._undoStack != null)
				{
					this._undoStack.Push(new LayerWeightUndoEntry(this.Window.ActiveDeformData.Layers[primaryLayerIndex], before, after));
				}
				return;
			}
			MultiDeltaUndoEntry multiDeltaUndoEntry = new MultiDeltaUndoEntry();
			for (int i = 0; i < this._activeRenderers.Count; i++)
			{
				DeformData existingDeformData = ShapePaintOverlay.GetExistingDeformData(this._activeRenderers[i]);
				if (existingDeformData != null)
				{
					for (int j = 0; j < existingDeformData.Layers.Count; j++)
					{
						if (existingDeformData.Layers[j].Name == name)
						{
							if (i != this._primaryListIdx)
							{
								existingDeformData.SetLayerWeight(j, after);
							}
							multiDeltaUndoEntry.Add(new LayerWeightUndoEntry(existingDeformData.Layers[j], before, after));
							multiDeltaUndoEntry.TrackDeformer(this._activeDeformers[i]);
							break;
						}
					}
				}
			}
			if (this._undoStack != null && multiDeltaUndoEntry.ChildCount > 0)
			{
				this._undoStack.Push(multiDeltaUndoEntry);
			}
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x000294D4 File Offset: 0x000276D4
		private void DoLayerMirror(int layerIndex)
		{
			if (this.Window == null || !this.Window.IsEditMode)
			{
				return;
			}
			if (this._activeRenderers.Count == 0 || this._primaryListIdx < 0)
			{
				return;
			}
			DeformData activeDeformData = this.Window.ActiveDeformData;
			if (activeDeformData == null)
			{
				return;
			}
			if (layerIndex < 0 || layerIndex >= activeDeformData.Layers.Count)
			{
				return;
			}
			DeformLayer deformLayer = activeDeformData.Layers[layerIndex];
			Renderer renderer = this._activeRenderers[this._primaryListIdx];
			ShapeDeformer shapeDeformer = this._activeDeformers[this._primaryListIdx];
			if (renderer == null || shapeDeformer == null)
			{
				return;
			}
			Transform transform = renderer.transform;
			Transform transform2 = ((this._objectRoot != null) ? this._objectRoot : transform);
			DeformLayer deformLayer2 = LayerMirror.MirrorLayerIntoNew(shapeDeformer, transform, transform2, activeDeformData, deformLayer);
			if (deformLayer2 == null)
			{
				return;
			}
			if (this._undoStack != null)
			{
				this._undoStack.Push(new LayerAddUndoEntry(activeDeformData, deformLayer2, activeDeformData.Layers.Count - 1));
			}
			this.NotifyLayerStructureChanged();
			shapeDeformer.InvalidateDeltaCache();
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x000295E0 File Offset: 0x000277E0
		private static DeformLayer FindLayerByName(DeformData data, string name)
		{
			if (data == null || name == null)
			{
				return null;
			}
			for (int i = 0; i < data.Layers.Count; i++)
			{
				if (data.Layers[i].Name == name)
				{
					return data.Layers[i];
				}
			}
			return null;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00029634 File Offset: 0x00027834
		private void DoApplyPresetExport()
		{
			if (this.Window == null || this.Window.Renderers == null || this.Window.Renderers.Count == 0)
			{
				return;
			}
			PresetTabState presetState = this.Window.PresetState;
			if (presetState == null)
			{
				return;
			}
			List<PresetEntry> list = new List<PresetEntry>();
			for (int i = 0; i < this.Window.Renderers.Count; i++)
			{
				bool flag;
				if (presetState.RowChecked.TryGetValue(i, out flag) && flag)
				{
					Renderer renderer = this.Window.Renderers[i];
					if (!(renderer == null))
					{
						string text = ((this.Window.RendererPaths != null && i < this.Window.RendererPaths.Count) ? (this.Window.RendererPaths[i] ?? "") : "");
						ControllerResolver.Owner owner = ControllerResolver.Resolve(renderer);
						DeformData deformData = owner.GetDeformData(renderer);
						int subdivisionLevel = MeshHelper.GetSubdivisionLevel(renderer);
						PresetEntry presetEntry = PresetSerializer.BuildEntryFromRenderer(renderer, deformData, subdivisionLevel, text);
						if (presetEntry != null)
						{
							if (owner.IsOnCharacter)
							{
								ShapePaintOverlay.CollectDriversForEntry(owner.CharacterController, presetEntry);
							}
							list.Add(presetEntry);
						}
					}
				}
			}
			if (list.Count == 0)
			{
				ShapeEditorPlugin.Logger.LogInfo("[Preset] export skipped: no rows checked / no exportable renderers");
				return;
			}
			string text2;
			if (!ShapePaintOverlay.TryShowSaveDialog(L.ExportPreset, "preset.kksp", L.PresetFileFilter, out text2) || string.IsNullOrEmpty(text2))
			{
				return;
			}
			try
			{
				byte[] array = PresetSerializer.SerializeBundle(list);
				File.WriteAllBytes(text2, array);
				ShapeEditorPlugin.Logger.LogInfo(string.Format(L.PresetExportSuccessFmt, list.Count));
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("Preset export failed: " + ex.Message);
			}
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00029808 File Offset: 0x00027A08
		private static void CollectDriversForEntry(ShapeEditorController controller, PresetEntry entry)
		{
			if (controller == null || entry == null)
			{
				return;
			}
			string text = entry.RendererPath ?? "";
			HashSet<string> hashSet = entry.CollectLayerIds();
			if (hashSet.Count == 0)
			{
				return;
			}
			List<PsdDriver> drivers = controller.Drivers;
			if (drivers != null)
			{
				for (int i = 0; i < drivers.Count; i++)
				{
					PsdDriver psdDriver = drivers[i];
					if (psdDriver != null && !((psdDriver.TargetRendererPath ?? "") != text) && hashSet.Contains(psdDriver.TargetLayerId ?? ""))
					{
						entry.PsdDrivers.Add(psdDriver.Clone());
					}
				}
			}
			List<CsbDriver> csbDrivers = controller.CsbDrivers;
			if (csbDrivers != null)
			{
				for (int j = 0; j < csbDrivers.Count; j++)
				{
					CsbDriver csbDriver = csbDrivers[j];
					if (csbDriver != null && !((csbDriver.TargetRendererPath ?? "") != text) && hashSet.Contains(csbDriver.TargetLayerId ?? ""))
					{
						entry.CsbDrivers.Add(csbDriver.Clone());
					}
				}
			}
			List<NbbDriver> nbbDrivers = controller.NbbDrivers;
			if (nbbDrivers != null)
			{
				for (int k = 0; k < nbbDrivers.Count; k++)
				{
					NbbDriver nbbDriver = nbbDrivers[k];
					if (nbbDriver != null && !((nbbDriver.TargetRendererPath ?? "") != text) && hashSet.Contains(nbbDriver.TargetLayerId ?? ""))
					{
						entry.NbbDrivers.Add(nbbDriver.Clone());
					}
				}
			}
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00029994 File Offset: 0x00027B94
		private void DoLoadPresetForPreview()
		{
			if (this.Window == null)
			{
				return;
			}
			PresetTabState presetState = this.Window.PresetState;
			if (presetState == null)
			{
				return;
			}
			string text;
			if (!ShapePaintOverlay.TryShowOpenDialog(L.ImportPreset, L.PresetFileFilter, out text) || string.IsNullOrEmpty(text))
			{
				return;
			}
			byte[] array;
			try
			{
				array = File.ReadAllBytes(text);
			}
			catch (Exception ex)
			{
				ShapeEditorPlugin.Logger.LogWarning("Preset import read failed: " + ex.Message);
				return;
			}
			PresetBundle presetBundle = PresetSerializer.DeserializeBundle(array);
			if (presetBundle == null)
			{
				return;
			}
			presetState.LoadedBundle = presetBundle;
			presetState.RowChecked.Clear();
			foreach (int num in PresetImporter.DefaultCheckedIndices(presetBundle, new Func<PresetEntry, List<Renderer>>(this.ResolvePresetTargets)))
			{
				presetState.RowChecked[num] = true;
			}
			presetState.Mode = PresetTabState.TabMode.ImportPreview;
			presetState.Scroll = Vector2.zero;
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00029A98 File Offset: 0x00027C98
		private void DoCancelPresetImport()
		{
			if (this.Window == null || this.Window.PresetState == null)
			{
				return;
			}
			this.Window.PresetState.ResetForOwnerSwitch();
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00029AC0 File Offset: 0x00027CC0
		private void DoApplyPresetImport()
		{
			if (this.Window == null || this.Window.PresetState == null)
			{
				return;
			}
			PresetBundle loadedBundle = this.Window.PresetState.LoadedBundle;
			if (loadedBundle == null || loadedBundle.Entries == null)
			{
				return;
			}
			List<int> list = new List<int>();
			for (int i = 0; i < loadedBundle.Entries.Count; i++)
			{
				bool flag;
				if (this.Window.PresetState.RowChecked.TryGetValue(i, out flag) && flag)
				{
					list.Add(i);
				}
			}
			bool preserveExistingLayers = this.Window.PresetState.PreserveExistingLayers;
			bool preserveCoordinateScope = this.Window.PresetState.PreserveCoordinateScope;
			ShapeEditorPlugin.Logger.LogInfo(string.Concat(new string[]
			{
				"[Preset] Apply Import: ",
				list.Count.ToString(),
				" entries checked, preserveExisting=",
				preserveExistingLayers.ToString(),
				", preserveCoordinateScope=",
				preserveCoordinateScope.ToString(),
				", Window.IsEditMode=",
				this.Window.IsEditMode.ToString()
			}));
			ShapeEditorPlugin.Instance.StartCoroutine(PresetImporter.ApplyImport(loadedBundle, list, preserveExistingLayers, new Func<PresetEntry, List<Renderer>>(this.ResolvePresetTargets), preserveCoordinateScope));
			this.Window.PresetState.ResetForOwnerSwitch();
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x00029C0A File Offset: 0x00027E0A
		private List<Renderer> ResolvePresetTargets(PresetEntry entry)
		{
			if (this.Window == null)
			{
				return null;
			}
			return PresetImporter.FindSameAssetRenderers(this.Window.Renderers, this.Window.RendererPaths, entry);
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x00029C34 File Offset: 0x00027E34
		private void OnRenderObject()
		{
			if (Camera.current != Camera.main)
			{
				return;
			}
			if (this.Window == null)
			{
				return;
			}
			if (this.Window.Visible && !this.Window.IsEditMode && this.Window.ShowMeshHighlight && this._highlightMaterial != null)
			{
				this.DrawMeshHighlight();
			}
			if (this._cursorMaterial == null)
			{
				return;
			}
			if (!this.Window.IsEditMode)
			{
				return;
			}
			bool flag = this._wireBundles.Count > 0;
			if (!flag && !this._hasHit)
			{
				return;
			}
			if (flag && this.Window.ShowMeshWireframe)
			{
				this.DrawWireframeAllBundles();
			}
			this._cursorMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(Matrix4x4.identity);
			if (this.Window.OperationMode == ShapeEditorWindow.OpMode.Brush)
			{
				if (this._hasHit)
				{
					this.DrawBrushCircle(this._lastHitPoint, this._lastHitNormal, this.Window.BrushRadius);
				}
			}
			else
			{
				if (flag)
				{
					if (this.Window.SelectionMode == ShapeEditorWindow.SelectMode.Face)
					{
						this.DrawSelectedFacesAllBundles();
					}
					else
					{
						this.DrawSelectedVerticesAllBundles();
					}
				}
				if (this.Window.GizmoSelection == ShapeEditorWindow.GizmoSelectMethod.Brush && this._hasHit)
				{
					this.DrawBrushCircle(this._lastHitPoint, this._lastHitNormal, this.Window.BrushRadius);
				}
			}
			if (this._isBoxSelecting)
			{
				this.DrawBoxSelectRect();
			}
			GL.PopMatrix();
			if (this.Window.OperationMode == ShapeEditorWindow.OpMode.Gizmo && this._gizmo != null && this._gizmo.HasTarget)
			{
				this._gizmo.Render(Camera.main);
				this._gizmo.RenderSoftRadius(Camera.main);
			}
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00029DE0 File Offset: 0x00027FE0
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
			GL.Color(new Color(1f, 1f, 0f, 0.9f));
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
			float num3 = radius * 0.05f;
			if (num3 < 0.001f)
			{
				num3 = 0.001f;
			}
			GL.Begin(1);
			GL.Color(new Color(1f, 1f, 0f, 0.9f));
			GL.Vertex(center - vector * num3);
			GL.Vertex(center + vector * num3);
			GL.Vertex(center - vector2 * num3);
			GL.Vertex(center + vector2 * num3);
			GL.End();
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00029F94 File Offset: 0x00028194
		private void DrawSelectedVerticesAllBundles()
		{
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			Vector3 position = main.transform.position;
			Vector3 right = main.transform.right;
			Vector3 up = main.transform.up;
			float value = ShapeEditorPlugin.SelectedVertexDotSize.Value;
			for (int i = 0; i < 12; i++)
			{
				float num = (float)i / 12f * 3.1415927f * 2f;
				ShapePaintOverlay._vertexDotBasis[i] = right * Mathf.Cos(num) + up * Mathf.Sin(num);
			}
			GL.Begin(4);
			int num2 = Mathf.Min(this._wireBundles.Count, this._activeSelTools.Count);
			for (int j = 0; j < num2; j++)
			{
				WireBundle wireBundle = this._wireBundles[j];
				SelectionTool selectionTool = this._activeSelTools[j];
				if (wireBundle != null && wireBundle.Verts != null && selectionTool != null)
				{
					foreach (int num3 in selectionTool.SelectedVertices)
					{
						if (num3 >= 0 && num3 < wireBundle.Verts.Length)
						{
							Vector3 vector = wireBundle.Verts[num3];
							float num4 = Vector3.Distance(position, vector) * value;
							float num5 = num4 * 0.7f;
							GL.Color(ShapePaintOverlay.VertexDotOutlineColor);
							for (int k = 0; k < 12; k++)
							{
								int num6 = (k + 1) % 12;
								GL.Vertex(vector);
								GL.Vertex(vector + ShapePaintOverlay._vertexDotBasis[k] * num4);
								GL.Vertex(vector + ShapePaintOverlay._vertexDotBasis[num6] * num4);
							}
							GL.Color(ShapePaintOverlay.VertexDotFillColor);
							for (int l = 0; l < 12; l++)
							{
								int num7 = (l + 1) % 12;
								GL.Vertex(vector);
								GL.Vertex(vector + ShapePaintOverlay._vertexDotBasis[l] * num5);
								GL.Vertex(vector + ShapePaintOverlay._vertexDotBasis[num7] * num5);
							}
						}
					}
				}
			}
			GL.End();
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x0002A210 File Offset: 0x00028410
		private void DrawSelectedFacesAllBundles()
		{
			bool selectedFacesDirty = this._selectedFacesDirty;
			this._selectedFacesDirty = false;
			GL.Begin(4);
			GL.Color(ShapePaintOverlay.SelectedFaceFillColor);
			int num = Mathf.Min(this._wireBundles.Count, this._activeSelTools.Count);
			for (int i = 0; i < num; i++)
			{
				WireBundle wireBundle = this._wireBundles[i];
				SelectionTool selectionTool = this._activeSelTools[i];
				if (wireBundle != null && wireBundle.Verts != null && selectionTool != null)
				{
					if (selectedFacesDirty || wireBundle.SelectedFaceVerts == null)
					{
						ShapePaintOverlay.RebuildSelectedFaceVerts(wireBundle, selectionTool.SelectedVertices);
					}
					List<int> selectedFaceVerts = wireBundle.SelectedFaceVerts;
					Vector3[] verts = wireBundle.Verts;
					int num2 = verts.Length;
					int num3 = 0;
					while (num3 + 2 < selectedFaceVerts.Count)
					{
						int num4 = selectedFaceVerts[num3];
						int num5 = selectedFaceVerts[num3 + 1];
						int num6 = selectedFaceVerts[num3 + 2];
						if (num4 < num2 && num5 < num2 && num6 < num2)
						{
							GL.Vertex(verts[num4]);
							GL.Vertex(verts[num5]);
							GL.Vertex(verts[num6]);
						}
						num3 += 3;
					}
				}
			}
			GL.End();
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0002A348 File Offset: 0x00028548
		private static void RebuildSelectedFaceVerts(WireBundle bundle, HashSet<int> selected)
		{
			if (bundle.SelectedFaceVerts == null)
			{
				bundle.SelectedFaceVerts = new List<int>();
			}
			List<int> selectedFaceVerts = bundle.SelectedFaceVerts;
			selectedFaceVerts.Clear();
			int[] tris = bundle.Tris;
			if (tris == null || selected.Count == 0)
			{
				return;
			}
			int num = 0;
			while (num + 2 < tris.Length)
			{
				int num2 = tris[num];
				int num3 = tris[num + 1];
				int num4 = tris[num + 2];
				if (selected.Contains(num2) && selected.Contains(num3) && selected.Contains(num4))
				{
					selectedFaceVerts.Add(num2);
					selectedFaceVerts.Add(num3);
					selectedFaceVerts.Add(num4);
				}
				num += 3;
			}
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x0002A3E0 File Offset: 0x000285E0
		private void DrawWireframeAllBundles()
		{
			try
			{
				Camera main = Camera.main;
				if (!(main == null))
				{
					Transform transform = main.transform;
					Vector3 position = transform.position;
					int num = position.GetHashCode() ^ transform.forward.GetHashCode();
					bool flag = this._gizmo != null && this._gizmo.SoftSelectionEnabled && this._gizmo.HasTarget && this.Window.OperationMode == ShapeEditorWindow.OpMode.Gizmo;
					bool flag2 = flag != this._prevUseSoftColors;
					this._prevUseSoftColors = flag;
					for (int i = 0; i < this._wireBundles.Count; i++)
					{
						WireBundle wireBundle = this._wireBundles[i];
						if (wireBundle != null && wireBundle.Edges != null && !(wireBundle.LineMesh == null) && wireBundle.Verts != null && wireBundle.Tris != null)
						{
							ShapeDeformer shapeDeformer = ((i < this._activeDeformers.Count) ? this._activeDeformers[i] : null);
							int num2 = ((shapeDeformer != null) ? shapeDeformer.LastVertsHash : 0);
							int num3 = ((shapeDeformer != null && shapeDeformer.DisplayTransform != null) ? ShapeDeformer.HashMatrix4x4(shapeDeformer.DisplayTransform.localToWorldMatrix) : 0);
							int num4 = num2 ^ num3;
							if (wireBundle.EverDrawn && !wireBundle.ColorsDirty && !flag2 && wireBundle.LastDrawVertsHash == num4 && wireBundle.LastDrawCamHash == num && wireBundle.Colors != null && wireBundle.Colors.Length == wireBundle.Verts.Length)
							{
								this._cursorMaterial.SetPass(0);
								Graphics.DrawMeshNow(wireBundle.LineMesh, Matrix4x4.identity);
							}
							else
							{
								if (wireBundle.ColorsDirty || flag2 || wireBundle.Colors == null || wireBundle.Colors.Length != wireBundle.Verts.Length)
								{
									this.RebuildWireColorsForBundle(wireBundle, i, flag);
									wireBundle.ColorsDirty = false;
									wireBundle.ColorsUploaded = false;
								}
								int[] tris = wireBundle.Tris;
								Vector3[] verts = wireBundle.Verts;
								int num5 = tris.Length / 3;
								if (wireBundle.TriFacing == null || wireBundle.TriFacing.Length < num5)
								{
									wireBundle.TriFacing = new bool[num5];
								}
								bool[] triFacing = wireBundle.TriFacing;
								for (int j = 0; j < num5; j++)
								{
									int num6 = j * 3;
									Vector3 vector = verts[tris[num6]];
									Vector3 vector2 = verts[tris[num6 + 1]];
									Vector3 vector3 = verts[tris[num6 + 2]];
									Vector3 vector4 = Vector3.Cross(vector2 - vector, vector3 - vector);
									triFacing[j] = Vector3.Dot(vector4, vector - position) <= 0f;
								}
								int num7 = wireBundle.Edges.Length;
								int num8 = 0;
								if (wireBundle.LineIndexBuffer == null || wireBundle.LineIndexBuffer.Length < num7 * 2)
								{
									wireBundle.LineIndexBuffer = new int[num7 * 2];
								}
								for (int k = 0; k < num7; k++)
								{
									int tri = wireBundle.Edges[k].tri0;
									int tri2 = wireBundle.Edges[k].tri1;
									bool flag3 = wireBundle.TriFacing[tri];
									bool flag4 = tri2 >= 0 && wireBundle.TriFacing[tri2];
									if (flag3 || flag4)
									{
										wireBundle.LineIndexBuffer[num8++] = wireBundle.Edges[k].v0;
										wireBundle.LineIndexBuffer[num8++] = wireBundle.Edges[k].v1;
									}
								}
								wireBundle.LineMesh.vertices = wireBundle.Verts;
								if (!wireBundle.ColorsUploaded)
								{
									wireBundle.LineMesh.colors32 = wireBundle.Colors;
									wireBundle.ColorsUploaded = true;
								}
								if (num8 != wireBundle.PrevVisibleCount)
								{
									wireBundle.VisibleLineIndices = new int[num8];
									wireBundle.PrevVisibleCount = num8;
								}
								Array.Copy(wireBundle.LineIndexBuffer, wireBundle.VisibleLineIndices, num8);
								wireBundle.LineMesh.SetIndices(wireBundle.VisibleLineIndices, UnityEngine.MeshTopology.Lines, 0);
								this._cursorMaterial.SetPass(0);
								Graphics.DrawMeshNow(wireBundle.LineMesh, Matrix4x4.identity);
								wireBundle.LastDrawVertsHash = num4;
								wireBundle.LastDrawCamHash = num;
								wireBundle.EverDrawn = true;
							}
						}
					}
				}
			}
			finally
			{
			}
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x0002A884 File Offset: 0x00028A84
		private void RebuildWireColorsForBundle(WireBundle bundle, int rendererSlot, bool useSoftColors)
		{
			int num = bundle.Verts.Length;
			if (bundle.Colors == null || bundle.Colors.Length != num)
			{
				bundle.Colors = new Color32[num];
			}
			IDictionary<int, float> dictionary = (useSoftColors ? this._gizmo.GetCombinedSoftWeights(rendererSlot) : null);
			if (dictionary != null)
			{
				for (int i = 0; i < num; i++)
				{
					float num2;
					if (dictionary.TryGetValue(i, out num2))
					{
						if (num2 <= 0.5f)
						{
							byte b = (byte)(num2 * 2f * 255f);
							bundle.Colors[i] = new Color32(b, 0, 0, byte.MaxValue);
						}
						else
						{
							byte b2 = (byte)((num2 - 0.5f) * 2f * 255f);
							bundle.Colors[i] = new Color32(byte.MaxValue, b2, 0, byte.MaxValue);
						}
					}
					else
					{
						bundle.Colors[i] = ShapePaintOverlay.WireDefaultColor32;
					}
				}
				return;
			}
			for (int j = 0; j < num; j++)
			{
				bundle.Colors[j] = ShapePaintOverlay.WireDefaultColor32;
			}
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x0002A990 File Offset: 0x00028B90
		private void DrawBoxSelectRect()
		{
			GL.PushMatrix();
			GL.LoadPixelMatrix();
			float num = Mathf.Min(this._boxStart.x, this._boxEnd.x);
			float num2 = Mathf.Max(this._boxStart.x, this._boxEnd.x);
			float num3 = Mathf.Min(this._boxStart.y, this._boxEnd.y);
			float num4 = Mathf.Max(this._boxStart.y, this._boxEnd.y);
			GL.Begin(7);
			GL.Color(new Color(0.2f, 0.6f, 1f, 0.15f));
			GL.Vertex3(num, num3, 0f);
			GL.Vertex3(num2, num3, 0f);
			GL.Vertex3(num2, num4, 0f);
			GL.Vertex3(num, num4, 0f);
			GL.End();
			GL.Begin(1);
			GL.Color(new Color(0.2f, 0.6f, 1f, 0.8f));
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

		// Token: 0x06000418 RID: 1048 RVA: 0x0002AAFC File Offset: 0x00028CFC
		private void DrawMeshHighlight()
		{
			try
			{
				HashSet<int> selectedRendererIndices = this.Window.SelectedRendererIndices;
				if (selectedRendererIndices == null || selectedRendererIndices.Count == 0)
				{
					if (this.Window.PrimaryRendererIndex >= 0 && this.Window.PrimaryRendererIndex < this.Window.Renderers.Count)
					{
						this.DrawMeshHighlightFor(this.Window.Renderers[this.Window.PrimaryRendererIndex]);
					}
				}
				else
				{
					foreach (int num in selectedRendererIndices)
					{
						if (num >= 0 && num < this.Window.Renderers.Count)
						{
							Renderer renderer = this.Window.Renderers[num];
							if (!(renderer == null))
							{
								this.DrawMeshHighlightFor(renderer);
							}
						}
					}
				}
			}
			finally
			{
			}
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x0002ABF4 File Offset: 0x00028DF4
		private void DrawMeshHighlightFor(Renderer r)
		{
			if (r == null)
			{
				return;
			}
			ShapeDeformer component = r.GetComponent<ShapeDeformer>();
			if (component != null)
			{
				Mesh mesh = component.RequestPosedMesh();
				if (mesh != null && component.DisplayTransform != null)
				{
					this.DrawMeshHighlightFromDeformer(component, mesh);
					return;
				}
			}
			Mesh mesh2 = null;
			int instanceID = r.GetInstanceID();
			SkinnedMeshRenderer skinnedMeshRenderer = r as SkinnedMeshRenderer;
			Mesh mesh4;
			Matrix4x4 matrix4x;
			if (skinnedMeshRenderer != null)
			{
				if (skinnedMeshRenderer.sharedMesh == null)
				{
					return;
				}
				Mesh mesh3;
				if (!this._bakeMeshCacheByRenderer.TryGetValue(instanceID, out mesh3) || mesh3 == null)
				{
					mesh3 = new Mesh();
					this._bakeMeshCacheByRenderer[instanceID] = mesh3;
				}
				skinnedMeshRenderer.BakeMesh(mesh3);
				mesh2 = mesh3;
				mesh4 = skinnedMeshRenderer.sharedMesh;
				Transform transform = r.transform;
				matrix4x = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			}
			else
			{
				matrix4x = r.localToWorldMatrix;
				MeshFilter component2 = r.GetComponent<MeshFilter>();
				if (component2 != null)
				{
					mesh2 = component2.sharedMesh;
				}
				mesh4 = mesh2;
			}
			if (mesh2 == null || !mesh2.isReadable)
			{
				return;
			}
			int instanceID2 = mesh4.GetInstanceID();
			int[] triangles;
			if (!this._highlightTrisByMesh.TryGetValue(instanceID2, out triangles))
			{
				triangles = mesh2.triangles;
				this._highlightTrisByMesh[instanceID2] = triangles;
			}
			Vector3[] vertices = mesh2.vertices;
			this._highlightMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(matrix4x);
			GL.Begin(4);
			GL.Color(ShapePaintOverlay.HighlightColor);
			for (int i = 0; i < triangles.Length; i++)
			{
				GL.Vertex(vertices[triangles[i]]);
			}
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x0002ADA0 File Offset: 0x00028FA0
		private void DrawMeshHighlightFromDeformer(ShapeDeformer deformer, Mesh mesh)
		{
			if (mesh == null || !mesh.isReadable)
			{
				return;
			}
			int instanceID = mesh.GetInstanceID();
			int[] triangles;
			if (!this._highlightTrisByMesh.TryGetValue(instanceID, out triangles))
			{
				triangles = mesh.triangles;
				this._highlightTrisByMesh[instanceID] = triangles;
			}
			Vector3[] vertices = mesh.vertices;
			Matrix4x4 localToWorldMatrix = deformer.DisplayTransform.localToWorldMatrix;
			this._highlightMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(localToWorldMatrix);
			GL.Begin(4);
			GL.Color(ShapePaintOverlay.HighlightColor);
			for (int i = 0; i < triangles.Length; i++)
			{
				GL.Vertex(vertices[triangles[i]]);
			}
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x0002AE4C File Offset: 0x0002904C
		public void ActivateHighlight(int vertexCount)
		{
			if (this._deformer == null || vertexCount <= 0)
			{
				return;
			}
			this._highlightVertexCount = vertexCount;
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			if (shader == null)
			{
				shader = Shader.Find("Sprites/Default");
			}
			this._weightMaterial = ShapePaintOverlay.CreateBlendMaterial(shader, 1, 0, 1, 4);
			this._deformer.EnterEditMode(this._weightMaterial);
			this._highlightActive = true;
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x0002AEBC File Offset: 0x000290BC
		public void DeactivateHighlight()
		{
			if (this._deformer != null)
			{
				this._deformer.ExitEditMode();
			}
			if (this._weightMaterial != null)
			{
				UnityEngine.Object.Destroy(this._weightMaterial);
				this._weightMaterial = null;
			}
			this._highlightColors = null;
			this._highlightActive = false;
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x0002AF10 File Offset: 0x00029110
		public void UpdateHighlightColors(float[] weights)
		{
			if (!this._highlightActive || this._deformer == null || weights == null)
			{
				return;
			}
			this._highlightColors = new Color[this._highlightVertexCount];
			int num = Mathf.Min(weights.Length, this._highlightColors.Length);
			for (int i = 0; i < num; i++)
			{
				this._highlightColors[i] = ShapePaintOverlay.WeightGradient.Evaluate(weights[i]);
			}
			this._deformer.SetEditColors(this._highlightColors);
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x0002AF90 File Offset: 0x00029190
		public void UpdateHighlightColors(Dictionary<int, float> affectedVertices)
		{
			if (!this._highlightActive || this._deformer == null || this._highlightColors == null)
			{
				return;
			}
			foreach (KeyValuePair<int, float> keyValuePair in affectedVertices)
			{
				if (keyValuePair.Key >= 0 && keyValuePair.Key < this._highlightColors.Length)
				{
					this._highlightColors[keyValuePair.Key] = ShapePaintOverlay.WeightGradient.Evaluate(keyValuePair.Value);
				}
			}
			this._deformer.SetEditColors(this._highlightColors);
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x0002B048 File Offset: 0x00029248
		private void RefreshWireframe()
		{
			try
			{
				int count = this._activeRenderers.Count;
				while (this._wireBundles.Count < count)
				{
					this._wireBundles.Add(new WireBundle());
				}
				while (this._wireBundles.Count > count)
				{
					WireBundle wireBundle = this._wireBundles[this._wireBundles.Count - 1];
					if (wireBundle != null && wireBundle.LineMesh != null)
					{
						UnityEngine.Object.Destroy(wireBundle.LineMesh);
					}
					this._wireBundles.RemoveAt(this._wireBundles.Count - 1);
				}
				for (int i = 0; i < count; i++)
				{
					WireBundle wireBundle2 = this._wireBundles[i];
					ShapeDeformer shapeDeformer = ((i < this._activeDeformers.Count) ? this._activeDeformers[i] : null);
					Mesh mesh = null;
					Matrix4x4 matrix4x = Matrix4x4.identity;
					if (shapeDeformer != null)
					{
						mesh = shapeDeformer.RequestPosedMesh();
						if (mesh != null && shapeDeformer.DisplayTransform != null)
						{
							matrix4x = shapeDeformer.DisplayTransform.localToWorldMatrix;
						}
						else
						{
							mesh = null;
						}
					}
					if (mesh == null)
					{
						wireBundle2.Verts = null;
						wireBundle2.Tris = null;
						wireBundle2.Edges = null;
						wireBundle2.SelectedFaceVerts = null;
						wireBundle2.RefreshValid = false;
					}
					else
					{
						int instanceID = mesh.GetInstanceID();
						bool flag = wireBundle2.Tris != null && wireBundle2.MeshId == instanceID;
						int lastRebakeFrame = shapeDeformer.LastRebakeFrame;
						int num = ShapeDeformer.HashMatrix4x4(matrix4x);
						int num2 = shapeDeformer.LastVertsHash ^ num;
						if (!flag || !wireBundle2.RefreshValid || wireBundle2.Verts == null || wireBundle2.LastRefreshRebakeFrame != lastRebakeFrame || wireBundle2.LastRefreshSig != num2)
						{
							mesh.GetVertices(this._posedVertsBuffer);
							int count2 = this._posedVertsBuffer.Count;
							if (wireBundle2.Verts == null || wireBundle2.Verts.Length != count2)
							{
								wireBundle2.Verts = new Vector3[count2];
							}
							for (int j = 0; j < count2; j++)
							{
								wireBundle2.Verts[j] = matrix4x.MultiplyPoint3x4(this._posedVertsBuffer[j]);
							}
							wireBundle2.LastRefreshRebakeFrame = lastRebakeFrame;
							wireBundle2.LastRefreshSig = num2;
							wireBundle2.RefreshValid = true;
						}
						if (!flag)
						{
							wireBundle2.Tris = mesh.triangles;
							wireBundle2.MeshId = instanceID;
							wireBundle2.SelectedFaceVerts = null;
							ShapePaintOverlay.ExtractUniqueEdgesIntoBundle(wireBundle2);
							if (wireBundle2.LineMesh != null)
							{
								UnityEngine.Object.Destroy(wireBundle2.LineMesh);
							}
							wireBundle2.LineMesh = new Mesh();
							wireBundle2.LineMesh.MarkDynamic();
							wireBundle2.PrevVisibleCount = -1;
							wireBundle2.ColorsDirty = true;
							wireBundle2.EverDrawn = false;
							wireBundle2.ColorsUploaded = false;
							wireBundle2.TriFacing = null;
						}
					}
				}
			}
			finally
			{
			}
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x0002B320 File Offset: 0x00029520
		private void OnGUI()
		{
			ShapeEditorWindow window = this.Window;
			if (window != null)
			{
				window.DrawGUI();
			}
			if (this.Window != null && this.Window.IsEditMode)
			{
				this.DrawEditModeHud();
				InputHelper input = this.Input;
				if (input == null)
				{
					return;
				}
				input.ResetGameInput();
			}
		}

		private void ProcessSeamlessInteraction(
			Camera cam,
			Ray ray,
			Vector3 mousePos,
			DeformLayer primaryActiveLayer)
		{
			if (primaryActiveLayer == null)
			{
				return;
			}

			if (!this.Input.MouseButton ||
				!this._hasHit ||
				this.Input.CtrlHeld)
			{
				return;
			}

			if (this.Window.SeamlessTargetCharacter == null)
			{
				ShapeEditorPlugin.Logger.LogWarning(
					"Seamless target character is not selected.");
				return;
			}

			if (!this.Window.SeamlessBodyEnabled &&
				!this.Window.SeamlessHeadEnabled)
			{
				ShapeEditorPlugin.Logger.LogWarning(
					"Seamless requires Body or Head target.");
				return;
			}

		}

		// Token: 0x06000421 RID: 1057 RVA: 0x0002B360 File Offset: 0x00029560
		private void DrawEditModeHud()
		{
			if (this._hudStyle == null)
			{
				this._hudStyle = new GUIStyle(GUI.skin.box);
				this._hudStyle.alignment = 0;
				this._hudStyle.fontSize = 13;
				this._hudStyle.normal.textColor = Color.green;
				this._hudStyle.padding = new RectOffset(8, 8, 6, 6);
			}
			string text2;
			if (this.Window.OperationMode == ShapeEditorWindow.OpMode.Brush)
			{
				string text;
				switch (this.Window.SelectedBrushTool)
				{
				case ShapeEditorWindow.BrushToolType.Move:
					text = L.MoveTool;
					break;
				case ShapeEditorWindow.BrushToolType.Smooth:
					text = L.SmoothTool;
					break;
				case ShapeEditorWindow.BrushToolType.Relax:
					text = L.RelaxTool;
					break;
				case ShapeEditorWindow.BrushToolType.Inflate:
					text = L.InflateTool;
					break;
				case ShapeEditorWindow.BrushToolType.Pinch:
					text = L.PinchTool;
					break;
				case ShapeEditorWindow.BrushToolType.Crease:
					text = L.CreaseTool;
					break;
				case ShapeEditorWindow.BrushToolType.Seamless:
					text = "Seamless";
					break;
				default:
					text = "?";
					break;
				}
				text2 = L.BrushMode + ": " + text;
			}
			else
			{
				text2 = L.GizmoMode;
			}
			DeformData activeDeformData = this.Window.ActiveDeformData;
			DeformLayer deformLayer = ((activeDeformData != null) ? activeDeformData.ActiveLayer : null);
			string text3 = ((deformLayer != null) ? string.Format(L.HudLayerFmt, deformLayer.Name) : L.HudNoLayer);
			string text4 = null;
			int count = this._activeRenderers.Count;
			if (count > 1 && this._primaryListIdx >= 0)
			{
				string text5 = ((this._activeRenderers[this._primaryListIdx] != null) ? this._activeRenderers[this._primaryListIdx].name : "?");
				text4 = string.Format(L.HudRenderersFmt, count, text5);
			}
			string text6 = string.Format("R:{0}  S:{1}  F:{2}", this.Window.BrushRadius.ToString("F2"), this.Window.BrushStrength.ToString("F2"), this.Window.BrushFalloff);
			string text7 = text2 + "\n" + text3 + "\n";
			if (text4 != null)
			{
				text7 = text7 + text4 + "\n";
			}
			text7 = text7 + text6 + "\n" + L.HudShortcuts;
			if (this.Window.OperationMode == ShapeEditorWindow.OpMode.Gizmo)
			{
				text7 = text7 + "\n" + L.HudGrowShrink;
			}
			text7 = text7 + "\n" + L.FormatUndoRedoHint(ShapeEditorPlugin.UndoKey.Value, ShapeEditorPlugin.RedoKey.Value);
			float num = 250f;
			float num2 = (float)Screen.width - num - 360f;
			this._hudContent.text = text7;
			float num3 = this._hudStyle.CalcHeight(this._hudContent, num);
			GUI.Box(new Rect(num2, 10f, num, num3), text7, this._hudStyle);
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x0002B624 File Offset: 0x00029824
		internal static void ExtractUniqueEdgesIntoBundle(WireBundle bundle)
		{
			int[] tris = bundle.Tris;
			Dictionary<long, int> dictionary = new Dictionary<long, int>();
			List<WireEdge> list = new List<WireEdge>();
			int num = tris.Length / 3;
			for (int i = 0; i < num; i++)
			{
				int num2 = i * 3;
				int num3 = tris[num2];
				int num4 = tris[num2 + 1];
				int num5 = tris[num2 + 2];
				ShapePaintOverlay.AddEdge(dictionary, list, num3, num4, i);
				ShapePaintOverlay.AddEdge(dictionary, list, num4, num5, i);
				ShapePaintOverlay.AddEdge(dictionary, list, num5, num3, i);
			}
			bundle.Edges = list.ToArray();
			bundle.LineIndexBuffer = new int[bundle.Edges.Length * 2];
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x0002B6C0 File Offset: 0x000298C0
		internal static void AddEdge(Dictionary<long, int> edgeMap, List<WireEdge> edgeList, int v0, int v1, int triIdx)
		{
			int num = ((v0 < v1) ? v0 : v1);
			int num2 = ((v0 < v1) ? v1 : v0);
			long num3 = ((long)(uint)num << 32) | (uint)num2;
			int num4;
			if (edgeMap.TryGetValue(num3, out num4))
			{
				WireEdge wireEdge = edgeList[num4];
				wireEdge.tri1 = triIdx;
				edgeList[num4] = wireEdge;
				return;
			}
			edgeMap[num3] = edgeList.Count;
			edgeList.Add(new WireEdge
			{
				v0 = num,
				v1 = num2,
				tri0 = triIdx,
				tri1 = -1
			});
		}

		// Token: 0x040003B2 RID: 946
		private const float StrengthStep = 0.05f;

		// Token: 0x040003B3 RID: 947
		private const float RadiusMin = 0.001f;

		// Token: 0x040003B4 RID: 948
		private const float StrengthMin = 0.01f;

		// Token: 0x040003B5 RID: 949
		private const float StrengthMax = 1f;

		// Token: 0x040003B6 RID: 950
		private BracketStepRepeater _bracketRepeater;

		// Token: 0x040003B7 RID: 951
		public ShapeEditorWindow Window;

		// Token: 0x040003B8 RID: 952
		public SelectionTool SelectionTool;

		// Token: 0x040003B9 RID: 953
		public InputHelper Input;

		// Token: 0x040003BA RID: 954
		public Action OnRefreshRenderers;

		// Token: 0x040003BB RID: 955
		public Func<object> GetCurrentSelection;

		// Token: 0x040003BC RID: 956
		private Renderer _targetRenderer;

		// Token: 0x040003BD RID: 957
		private ShapeDeformer _deformer;

		// Token: 0x040003BE RID: 958
		private readonly List<Renderer> _activeRenderers = new List<Renderer>();

		// Token: 0x040003BF RID: 959
		private readonly List<ShapeDeformer> _activeDeformers = new List<ShapeDeformer>();

		// Token: 0x040003C0 RID: 960
		private readonly List<SelectionTool> _activeSelTools = new List<SelectionTool>();

		// Token: 0x040003C1 RID: 961
		private readonly List<SmoothTool> _activeSmoothTools = new List<SmoothTool>();

		// Token: 0x040003C2 RID: 962
		private int _primaryListIdx = -1;

		// Token: 0x040003C3 RID: 963
		private MoveTool _moveTool;

		// Token: 0x040003C4 RID: 964
		private SmoothTool _smoothTool;

		// Token: 0x040003C5 RID: 965
		private InflateTool _inflateTool;

		// Token: 0x040003C6 RID: 966
		private PinchTool _pinchTool;

		// Seamless seam correction tool
		private SeamlessTool _seamlessTool;

		// Token: 0x040003C7 RID: 967
		private TransformGizmo _gizmo;

		// Token: 0x040003C8 RID: 968
		private Transform _objectRoot;

		// Token: 0x040003C9 RID: 969
		private bool _deferGizmoCentroidRefresh;

		// Token: 0x040003CA RID: 970
		private Vector2 _prevMousePos;

		// Token: 0x040003CB RID: 971
		private Material _cursorMaterial;

		// Token: 0x040003CC RID: 972
		private Vector3 _lastHitPoint;

		// Token: 0x040003CD RID: 973
		private Vector3 _lastHitNormal;

		// Token: 0x040003CE RID: 974
		private bool _hasHit;

		// Token: 0x040003CF RID: 975
		private const int CursorSegments = 32;

		// Token: 0x040003D0 RID: 976
		private static bool _systemDialogUnavailable = false;

		// Token: 0x040003D1 RID: 977
		private Vector3 _strokeStartHitPoint;

		// Token: 0x040003D2 RID: 978
		private Vector3 _strokeStartHitNormal;

		// Token: 0x040003D3 RID: 979
		private Vector3 _lastValidHitPoint;

		// Token: 0x040003D4 RID: 980
		private Vector3 _lastValidHitNormal;

		// Token: 0x040003D5 RID: 981
		private Material _highlightMaterial;

		// Token: 0x040003D6 RID: 982
		private readonly Dictionary<int, int[]> _highlightTrisByMesh = new Dictionary<int, int[]>();

		// Token: 0x040003D7 RID: 983
		private readonly Dictionary<int, Mesh> _bakeMeshCacheByRenderer = new Dictionary<int, Mesh>();

		// Token: 0x040003D8 RID: 984
		private static readonly Color HighlightColor = new Color(1f, 0.6f, 0f, 0.35f);

		// Token: 0x040003D9 RID: 985
		private object _lastSelection;

		// Token: 0x040003DA RID: 986
		private bool _isBoxSelecting;

		// Token: 0x040003DB RID: 987
		private Vector2 _boxStart;

		// Token: 0x040003DC RID: 988
		private Vector2 _boxEnd;

		// Token: 0x040003DD RID: 989
		private bool _isBrushSelecting;

		// Token: 0x040003DE RID: 990
		private UndoStack _undoStack;

		// Token: 0x040003DF RID: 991
		private bool _isBrushing;

		// Token: 0x040003E0 RID: 992
		private List<ShapePaintOverlay.StrokeRendererState> _strokeStates;

		// Token: 0x040003E1 RID: 993
		private MultiDeltaUndoEntry _currentBrushEntry;

		// Token: 0x040003E2 RID: 994
		private bool _gizmoLazyCreatedThisDrag;

		// Token: 0x040003E3 RID: 995
		private float _refreshTimer;

		// Token: 0x040003E4 RID: 996
		private const float ColliderRefreshInterval = 0.5f;

		// Token: 0x040003E5 RID: 997
		private readonly List<ShapePaintOverlay.ColliderCacheSlot> _colliderCache = new List<ShapePaintOverlay.ColliderCacheSlot>();

		// Token: 0x040003E6 RID: 998
		private readonly List<WireBundle> _wireBundles = new List<WireBundle>();

		// Token: 0x040003E7 RID: 999
		private bool _prevUseSoftColors;

		// Token: 0x040003E8 RID: 1000
		private bool _softWeightsDirty;

		// Token: 0x040003E9 RID: 1001
		private float _softWeightsDirtyTime;

		// Token: 0x040003EA RID: 1002
		private const float SoftWeightsThrottle = 0.15f;

		// Token: 0x040003EB RID: 1003
		private float _lastPressure01 = 1f;

		// Token: 0x040003EC RID: 1004
		private TabletEvent _packetHandler;

		// Token: 0x040003ED RID: 1005
		private bool _pressureSubscribed;

		// Token: 0x040003EE RID: 1006
		private bool _symmetryEnabled;

		// Token: 0x040003EF RID: 1007
		private int _symmetryAxis;

		// Token: 0x040003F0 RID: 1008
		private float _symmetryCenter;

		// Token: 0x040003F1 RID: 1009
		private bool _symmetryCenterSet;

		// Token: 0x040003F2 RID: 1010
		private Material _weightMaterial;

		// Token: 0x040003F3 RID: 1011
		private Color[] _highlightColors;

		// Token: 0x040003F4 RID: 1012
		private int _highlightVertexCount;

		// Token: 0x040003F5 RID: 1013
		private bool _highlightActive;

		// Token: 0x040003F6 RID: 1014
		private static readonly Gradient WeightGradient = new Gradient();

		// Token: 0x040003F7 RID: 1015
		private readonly HashSet<MonoBehaviour> _notifyOwnerBuf = new HashSet<MonoBehaviour>();

		// Token: 0x040003F8 RID: 1016
		private static FieldInfo _pressureField;

		// Token: 0x040003F9 RID: 1017
		private static bool _pressureFieldResolved;

		// Token: 0x040003FA RID: 1018
		private List<Vector3[]> _gizmoFrameVertsBuf;

		// Token: 0x040003FB RID: 1019
		private List<Vector3[]> _boxSelectWorldNormals;

		// Token: 0x040003FC RID: 1020
		private static readonly Color WireDefaultColor = new Color(0f, 0f, 0f, 0.3f);

		// Token: 0x040003FD RID: 1021
		internal static readonly Color32 WireDefaultColor32 = new Color32(0, 0, 0, 77);

		// Token: 0x040003FE RID: 1022
		private readonly List<Vector3> _posedVertsBuffer = new List<Vector3>();

		// Token: 0x040003FF RID: 1023
		private static readonly Color VertexDotOutlineColor = new Color(0f, 0f, 0f, 0.9f);

		// Token: 0x04000400 RID: 1024
		private static readonly Color VertexDotFillColor = new Color(1f, 1f, 1f, 0.95f);

		// Token: 0x04000401 RID: 1025
		private const int VertexDotSegments = 12;

		// Token: 0x04000402 RID: 1026
		private const float VertexDotInnerRatio = 0.7f;

		// Token: 0x04000403 RID: 1027
		private static readonly Vector3[] _vertexDotBasis = new Vector3[12];

		// Token: 0x04000404 RID: 1028
		private static readonly Color SelectedFaceFillColor = new Color(0.25f, 0.7f, 1f, 0.35f);

		// Token: 0x04000405 RID: 1029
		private bool _selectedFacesDirty = true;

		// Token: 0x04000406 RID: 1030
		private GUIStyle _hudStyle;

		private Renderer _seamlessTargetBody;
		private Renderer _seamlessTargetHead;
		private ShapeDeformer _seamlessTargetBodyDeformer;
		private ShapeDeformer _seamlessTargetHeadDeformer;

		// Token: 0x04000407 RID: 1031
		private readonly GUIContent _hudContent = new GUIContent();

		// Token: 0x02000079 RID: 121
		private class StrokeRendererState
		{
			// Token: 0x04000507 RID: 1287
			public ShapeDeformer Deformer;

			// Token: 0x04000508 RID: 1288
			public SelectionTool SelTool;

			// Token: 0x04000509 RID: 1289
			public SmoothTool SmoothTool;

			// Token: 0x0400050A RID: 1290
			public Renderer Renderer;

			// Token: 0x0400050B RID: 1291
			public DeformData Data;

			// Token: 0x0400050C RID: 1292
			public DeformLayer Layer;

			// Token: 0x0400050D RID: 1293
			public bool LazyCreated;

			// Token: 0x0400050E RID: 1294
			public int LazyCreatedIdx;

			// Token: 0x0400050F RID: 1295
			public Dictionary<int, Vector3> BeforeSnapshot = new Dictionary<int, Vector3>();

			// Token: 0x04000510 RID: 1296
			public Dictionary<int, float> MoveGrabVertices;

			// Token: 0x04000511 RID: 1297
			public BrushResult MoveGrabResult;

			// Token: 0x04000512 RID: 1298
			public Dictionary<int, float> MoveGrabMirrorVertices;

			// Token: 0x04000513 RID: 1299
			public BrushResult MoveGrabMirrorResult;
		}

		// Token: 0x0200007A RID: 122
		private struct ColliderCacheSlot
		{
			// Token: 0x04000514 RID: 1300
			public int Hash;

			// Token: 0x04000515 RID: 1301
			public int MeshId;

			// Token: 0x04000516 RID: 1302
			public bool EverRefreshed;
		}
	}
}
