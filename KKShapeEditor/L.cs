using System;
using BepInEx.Configuration;

namespace KKShapeEditor
{
	// Token: 0x02000043 RID: 67
	public static class L
	{
		// Token: 0x06000336 RID: 822 RVA: 0x0001A868 File Offset: 0x00018A68
		public static string CategoryLabel(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return "";
			}
			if (key.StartsWith("AccSlot", StringComparison.Ordinal))
			{
				return L.CatAccSlot + key.Substring("AccSlot".Length);
			}
			if (key != null)
			{
				switch (key.Length)
				{
				case 3:
				{
					char c = key[0];
					if (c != 'B')
					{
						if (c == 'T')
						{
							if (key == "Top")
							{
								return L.CatTop;
							}
						}
					}
					else if (key == "Bra")
					{
						return L.CatBra;
					}
					break;
				}
				case 4:
				{
					char c = key[1];
					if (c <= 'e')
					{
						if (c != 'a')
						{
							if (c == 'e')
							{
								if (key == "Head")
								{
									return L.CatHead;
								}
							}
						}
						else if (key == "Hair")
						{
							return L.CatHair;
						}
					}
					else if (c != 'o')
					{
						if (c == 't')
						{
							if (key == "Item")
							{
								return L.CatItem;
							}
						}
					}
					else if (key == "Body")
					{
						return L.CatBody;
					}
					break;
				}
				case 6:
				{
					char c = key[0];
					if (c != 'B')
					{
						if (c == 'G')
						{
							if (key == "Gloves")
							{
								return L.CatGloves;
							}
						}
					}
					else if (key == "Bottom")
					{
						return L.CatBottom;
					}
					break;
				}
				case 7:
				{
					char c = key[0];
					if (c != 'C')
					{
						if (c == 'L')
						{
							if (key == "Legwear")
							{
								return L.CatLegwear;
							}
						}
					}
					else if (key == "Clothes")
					{
						return L.CatClothes;
					}
					break;
				}
				case 8:
				{
					char c = key[4];
					if (c != 'B')
					{
						if (c == 'S')
						{
							if (key == "HairSide")
							{
								return L.CatHairSide;
							}
						}
					}
					else if (key == "HairBack")
					{
						return L.CatHairBack;
					}
					break;
				}
				case 9:
				{
					char c = key[0];
					if (c <= 'H')
					{
						if (c != 'A')
						{
							if (c == 'H')
							{
								if (key == "HairFront")
								{
									return L.CatHairFront;
								}
							}
						}
						else if (key == "Accessory")
						{
							return L.CatAccessory;
						}
					}
					else if (c != 'P')
					{
						if (c == 'U')
						{
							if (key == "Underwear")
							{
								return L.CatUnderwear;
							}
						}
					}
					else if (key == "Pantyhose")
					{
						return L.CatPantyhose;
					}
					break;
				}
				case 10:
				{
					char c = key[5];
					if (c != 'I')
					{
						if (c != 'O')
						{
							if (c == 'p')
							{
								if (key == "HairOption")
								{
									return L.CatHairOption;
								}
							}
						}
						else if (key == "ShoesOuter")
						{
							return L.CatShoesOuter;
						}
					}
					else if (key == "ShoesInner")
					{
						return L.CatShoesInner;
					}
					break;
				}
				}
			}
			return key;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0001ABE4 File Offset: 0x00018DE4
		public static string FormatUndoRedoHint(KeyboardShortcut undo, KeyboardShortcut redo)
		{
			string[] array = new string[7];
			int num = 0;
			KeyboardShortcut keyboardShortcut = undo;
			array[num] = keyboardShortcut.ToString();
			array[1] = ":";
			array[2] = L.UndoLabel;
			array[3] = "  ";
			int num2 = 4;
			keyboardShortcut = redo;
			array[num2] = keyboardShortcut.ToString();
			array[5] = ":";
			array[6] = L.RedoLabel;
			return string.Concat(array);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0001AC48 File Offset: 0x00018E48
		static L()
		{
			L.Reload();
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0001AC5F File Offset: 0x00018E5F
		public static void SetLanguage(L.Language lang)
		{
			L._current = lang;
			L.Reload();
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0001AC6C File Offset: 0x00018E6C
		private static void Reload()
		{
			switch (L._current)
			{
			case L.Language.Japanese:
				L.LoadJapanese();
				return;
			case L.Language.Korean:
				L.LoadKorean();
				return;
			case L.Language.TraditionalChinese:
				L.LoadTraditionalChinese();
				return;
			case L.Language.SimplifiedChinese:
				L.LoadSimplifiedChinese();
				return;
			default:
				L.LoadEnglish();
				return;
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0001ACB8 File Offset: 0x00018EB8
		private static void LoadEnglish()
		{
			L.SelectObject = "Please select an object";
			L.FilterLabel = "Filter:";
			L.TabNames = new string[] { "Shape", "Subdivide" };
			L.BrushMode = "Brush";
			L.GizmoMode = "Gizmo";
			L.MoveTool = "Move";
			L.SmoothTool = "Smooth";
			L.RelaxTool = "Relax";
			L.InflateTool = "Inflate";
			L.PinchTool = "Pinch";
			L.CreaseTool = "Crease";
			L.Translate = "Translate";
			L.Rotate = "Rotate";
			L.Scale = "Scale";
			L.WorldSpace = "World";
			L.ObjectSpace = "Object";
			L.NormalSpace = "Normal";
			L.SoftSelection = "Soft Selection";
			L.SoftModeVolume = "Volume";
			L.SoftModeSurface = "Surface";
			L.SoftSelectionRadius = "Soft Selection Radius";
			L.IncludeBackFaceLabel = "Include Back-Facing Vertices";
			L.IncludeBackFaceTooltip = "When enabled (default), Gizmo vertex selection (box and brush) includes vertices on the back side of the mesh whose normals point away from the camera. When disabled, only front-facing vertices are selected. The sculpt Brush mode is unaffected either way.";
			L.GizmoSelectBox = "Box Select";
			L.GizmoSelectBrush = "Brush Select";
			L.GizmoGranularityVertex = "Vertex";
			L.GizmoGranularityFace = "Face";
			L.GizmoClearSelection = "Clear Selection";
			L.Symmetry = "Symmetry";
			L.SymmetryAxis = "Axis:";
			L.SetCenter = "Set Center";
			L.ClearCenter = "Clear";
			L.SymmetryCenterFmt = "Center: {0:F3}";
			L.Layers = "Layers";
			L.LayerWeight = "Weight";
			L.AddLayer = "Add Layer";
			L.RemoveLayer = "Remove";
			L.RenameLayer = "Rename";
			L.MoveUp = "Up";
			L.MoveDown = "Down";
			L.MirrorLayer = "Mirror";
			L.MirrorLayerTooltip = "Mirror this layer across the body's left-right center into a new layer (X axis).";
			L.NoLayerWarning = "Create a layer to start sculpting";
			L.LayerDefaultNameFmt = "Layer {0}";
			L.TargetMesh = "Target Mesh:";
			L.VerticesFacesFmt = "Vertices: {0} | Faces: {1}";
			L.Brush = "Brush";
			L.BoxSelect = "Box Select";
			L.BrushRadiusFmt = "Brush Radius: {0}";
			L.SelectedVerticesFmt = "Selected: {0} vertices";
			L.FalloffLinear = "Linear";
			L.FalloffSmooth = "Smooth";
			L.FalloffSharp = "Sharp";
			L.SelectFaces = "Select Faces";
			L.SelectedFacesFmt = "Selected: {0} / {1} faces";
			L.AllButton = "All";
			L.NoneButton = "None";
			L.InvertButton = "Invert";
			L.LevelLabel = "Level:";
			L.Subdivide = "Subdivide";
			L.Restore = "Restore";
			L.SubdivideSmooth = "Smooth surface";
			L.SubdivideSmoothTooltip = "When enabled, subdivision pushes new edge midpoints toward the surface curvature (interpolating), rounding the shape without moving original vertices. In face-select mode, only the selected faces are smoothed.";
			L.RebakeSubdivision = "Rebake";
			L.RebakeSubdivisionTooltip = "Re-bakes the body's smooth-subdivision midpoints at the current pose to fix wireframe bumps that reappear after moving to a different pose. Preserves your sculpting. Only available for the character body (o_body*) with a smooth subdivision. On save/reload the bake is redone at the load pose.";
			L.FaceWireframeOpacity = "Wireframe Opacity";
			L.FaceWireframeOpacityTooltip = "Fades the Face Select wireframe. 0 = fully hidden (face selection still works), 1 = default density. Lower it to inspect the subdivided surface without the dense mesh lines.";
			L.FaceSelectIncludeBackFace = "Include Back Faces";
			L.FaceSelectIncludeBackFaceTooltip = "When checked (default), brush and box selection can pick faces on the back of the mesh whose normals point away from the camera. Uncheck to select only front-facing faces, handy for precisely picking the front shell before deleting faces. Occlusion is not tested, and it cannot make the brush reach the far side of a thick mesh.";
			L.SubdivideLayerWarning = "Subdividing will reset all layer data. Continue?";
			L.StrengthFmt = "Strength: {0}";
			L.ApplyButton = "Apply";
			L.ClearButton = "Clear";
			L.ConfirmButton = "Confirm";
			L.CancelButton = "Cancel";
			L.EnterEditMode = "Enter Edit Mode";
			L.ExitEditMode = "Exit Edit Mode";
			L.CameraMode = "Camera Mode (Ctrl)";
			L.BoxSelectModeName = "Box Select Mode";
			L.EditModeActive = "Edit Mode Active";
			L.BoxSelectInfoFmt = "Strength: {0} | {1} | Selected: {2} vertices";
			L.BrushInfoFmt = "Radius: {0} | Strength: {1} | {2} | {3}";
			L.ShowMeshHighlight = "Show Mesh Highlight";
			L.ShowMeshWireframe = "Show Mesh Wireframe";
			L.ShowEditedOnly = "Edited Only";
			L.FocusRenderer = "Focus Camera";
			L.CatTop = "Top";
			L.CatBottom = "Bottom";
			L.CatBra = "Bra";
			L.CatUnderwear = "Underwear";
			L.CatGloves = "Gloves";
			L.CatPantyhose = "Pantyhose";
			L.CatLegwear = "Legwear";
			L.CatShoesInner = "ShoesInner";
			L.CatShoesOuter = "ShoesOuter";
			L.CatHairBack = "HairBack";
			L.CatHairFront = "HairFront";
			L.CatHairSide = "HairSide";
			L.CatHairOption = "HairOption";
			L.CatBody = "Body";
			L.CatHead = "Head";
			L.CatClothes = "Clothes";
			L.CatAccessory = "Accessory";
			L.CatHair = "Hair";
			L.CatItem = "Item";
			L.CatAccSlot = "AccSlot";
			L.VisibilityShow = "Show";
			L.VisibilityHide = "Hide";
			L.PressureLabel = "Pressure";
			L.TabletNotDetected = "Tablet not detected";
			L.HudLayerFmt = "Layer: {0}";
			L.HudNoLayer = "(No Layer)";
			L.HudShortcuts = "Ctrl+LMB:Rotate  Shift:Normal\nCtrl+RMB:Zoom  Alt:Deflate\nLMB:Brush  RMB:Select\n[ ]:Radius  Shift+[ ]:Strength";
			L.HudGrowShrink = "=:Grow  -:Shrink";
			L.UndoLabel = "Undo";
			L.RedoLabel = "Redo";
			L.HudRenderersFmt = "Renderers: {0} (primary: {1})";
			L.FaceSelectBrush = "Face Select: Brush";
			L.FaceSelectBox = "Face Select: Box";
			L.RadiusSuffixFmt = " | Radius: {0}";
			L.RemapWeights = "Remap Weights";
			L.RestoreWeights = "Restore Weights";
			L.BodyMeshNotReadable = "Body mesh is not readable";
			L.ExportDeform = "Export";
			L.ImportDeform = "Import";
			L.DeformFileFilter = "Shape Deform (*.kksd)|*.kksd";
			L.ExportSuccess = "Export successful";
			L.ImportSuccess = "Import successful";
			L.ImportVertexMismatchFmt = "Vertex count mismatch: file has {0}, mesh has {1}";
			L.ImportInvalidFile = "Invalid file format";
			L.PresetTabLabel = "Preset";
			L.ExportPreset = "Export...";
			L.ExportPresetSelected = "Export Selected";
			L.ImportPreset = "Import...";
			L.ApplyImportPreset = "Apply Import";
			L.CancelImportPreset = "Cancel";
			L.PresetFileFilter = "Shape Preset (*.kksp)|*.kksp";
			L.PresetExportSuccessFmt = "Preset exported: {0} entries";
			L.PresetImportSummaryFmt = "[Preset] imported {0} entries; skipped paths: {1}";
			L.PresetInvalidFile = "Invalid preset file (.kksp magic / version mismatch or corrupted entries)";
			L.PresetMatchOk = "OK";
			L.PresetMatchNoMatch = "no match";
			L.PresetRowFormatFmt = "{0}  |  sub:{1}  |  layers:{2}  |  PSD:{3}  |  CSB:{4}  |  NBB:{5}";
			L.PresetRowRootName = "(root)";
			L.PresetEmptyOwnerHint = "Select a character or Studio item to populate the preset list.";
			L.PresetNoExportableHint = "No exportable data: no renderer on the current owner has subdivision or layers.";
			L.LiteShapeNoLayersHint = "No editable layers on the current character or item.";
			L.PresetPreserveLayers = "Preserve existing layers";
			L.PresetPreserveLayersTooltip = "Append the preset's layers after the existing layers instead of replacing them. Only applies when the vertex count matches; subdivision level and face mask are left unchanged.";
			L.PresetPreserveScope = "Preserve outfit scope";
			L.PresetPreserveScopeTooltip = "For imported CSB drivers (replace mode): keep global drivers global and rebind outfit-specific drivers to the current outfit. Turn off to make every imported CSB driver global.";
			L.HelpTitle = "Help";
			L.HelpBrush = "Brush Mode — sculpt by painting on the mesh.\n\nSetup\n- Pick a renderer (Click), then click Enter Edit Mode.\n- Multi-renderer (split clothing seams): Ctrl+Click extra renderers to add or remove them from the active set; the click target becomes primary.\n- Brush strokes paint across all active renderers using a shared world hit point — seams between renderers stay aligned.\n- Add a Layer before sculpting (required); layer ops (add/remove/rename/weight/reorder) broadcast to all active renderers.\n\nTools\n- Move: drags vertices along screen direction. Hold Shift to push/pull along the normal.\n- Smooth: averages vertex positions within the brush (flattens surface detail, even on un-sculpted surfaces).\n- Relax: averages sculpt deltas toward neighbors; leaves un-sculpted areas unchanged.\n- Inflate: pushes along the normal. Hold Alt to deflate.\n- Pinch: pulls vertices toward the brush center along the surface, tightening detail. Hold Alt to push outward.\n- Crease: pinch plus an inward push along the normal, carving a sharp groove. Hold Alt for a sharp ridge.\n\nParameters\n- Radius: brush size.\n- Strength: per-frame influence.\n- Falloff: Linear / Smooth / Sharp.\n- [ / ]: step Radius; Shift+[ / Shift+]: step Strength.\n\nSymmetry\n- Enable, pick an axis (X/Y/Z).\n- Set Center on a reference vertex, or leave at 0.\n\nLayers\n- Stack multiple deformations; each has its own Weight.\n- Reorder with Up / Down; final position = sum(layer delta * weight).\n\nRemap Weights (clothing)\n- After large deformation, click Remap Weights so skinning follows the nearest body bones.\n- Restore Weights reverts to the original bone weights.\n\nExport / Import (.kksd)\n- Export the current renderer's layers any time.\n- Import merges layers into the current renderer (Edit Mode only).\n\nShortcuts\n- {0}\n- Hold Ctrl: pass input to camera control.";
			L.HelpGizmo = "Gizmo Mode — transform selected vertices with handles.\n\nSelection Method (Box / Brush)\n- Box: left-click drag on empty space draws a screen rectangle to select.\n- Brush: left-click drag paints selection along the surface (a brush circle shows at the cursor).\n- Clear Selection: empties the current selection (brush is additive by default, so this is the reset).\n\nMouse (left button)\n- Box method: drag on empty space to box-select; Shift adds, Alt removes; the selection applies when you release.\n- Brush method: drag on the surface to add vertices every frame; Alt + drag erases. Additive by default (strokes stack).\n- Left-click drag on a handle (axis / plane / center / ViewRotate ring): translate / rotate / scale.\n- Once a selection gesture or handle drag starts, it stays locked until you release the button.\n- Hold Ctrl: gives input to the camera; no new selection or handle drag is started.\n- Right mouse button is unused in Gizmo mode.\n- With multiple active renderers (Click + Ctrl+Click in the list), selection spans every renderer; the gizmo appears at the union centroid and writes to each renderer's same-name layer (lazily creating it on first edit).\n\nHandles\n- Translate: three axes, XY/XZ/YZ planes, center cube (Free).\n- Rotate: three axis rings + outer white ring (ViewRotate, faces camera).\n- Scale: three axes + center cube (uniform).\n\nCoordinate Space\n- World: fixed XYZ.\n- Object: aligned to the character/item root rotation.\n- Normal: aligned to the selected vertices' average normal.\n\nSoft Selection\n- Extends influence beyond hard-selected vertices.\n- Radius: influence distance.\n- [ / ]: Box method steps this Soft Selection Radius (works whether or not Soft Selection is enabled); Brush method steps the Brush Select radius instead.\n- Volume: straight-line distance through space.\n- Surface: BFS along mesh edges (does not pierce thin meshes).\n- Radius/mode changes are throttled 150 ms before recomputing.\n\nSymmetry\n- Enable + pick an axis; moves/rotations/scales mirror automatically.\n\nGrow / Shrink Selection\n- =: grow the selection outward by one topological ring (Vertex and Face mode).\n- -: shrink the selection inward by one ring; one ring per press (no auto-repeat).\n- Works even while the cursor is over the plugin window.\n\nShortcuts\n- {0}\n- Hold Ctrl: pass input to camera control.";
			L.HelpSubdivide = "Subdivide — increase mesh density for finer sculpting.\n\nFace Selection\n- Use the face-select overlay to pick triangles to subdivide.\n- All / None / Invert helpers are provided.\n\nControls\n- Level: subdivision depth (1 = 4x, 2 = 16x triangles).\n- Subdivide: applies to the selected faces.\n- Restore: reverts the mesh to its original topology.\n\nWarnings\n- Subdividing resets all Layer delta data (vertex count changes).\n- Undo history is cleared after subdivision.\n\nPlatform Limits\n- Koikatsu (net35): mesh cannot exceed 65535 vertices — Subdivide aborts above this.\n- Koikatsu Sunshine: supports >65535 vertices via UInt32 index format.";
			L.HelpPreset = "Preset — save and reuse deformation across characters via .kksp bundles.\n\nExport\n- Check the renderers you want, then Export to a .kksp file.\n- Only renderers with subdivision or layers are listed.\n\nImport\n- Import a .kksp to enter preview, then check the entries to apply and press Apply.\n- Same asset in two slots (e.g. shoes) is applied to both automatically.\n\nReplace vs Preserve\n- Replace (default): rebuilds subdivision, replaces layers and face mask.\n- Preserve: appends layers only, topology must match, subdivision and face mask untouched.\n\nDrivers (PSD / CSB)\n- PSD and CSB drivers travel with the preset and are restored in Replace mode only.\n- Preserve mode ignores the bundled drivers (layer GUIDs are reassigned on append).\n- Preserve outfit scope: keep outfit-specific CSB drivers bound to the current outfit (off = make them global).";
			L.CorruptedEntryPrefix = "[!] ";
			L.CorruptedTooltipFormat = "Saved layer deltas don't match this mesh, and the subdivision data needed to rebuild the topology is missing.";
			L.ResetCorruptedButton = "Reset";
			L.ResetCorruptedConfirmTitle = "Reset Corrupted Layers";
			L.ResetCorruptedConfirmBody = "Path: {0}\nLayers to discard: {1}\n\nThis cannot be undone.";
			L.CorruptedEditModeBanner = "This target is corrupted. Brush and gizmo edits may produce incorrect results.";
			L.SettingsTabLabel = "Settings";
			L.CarryoverToggleLabel = "Carry over body/head deformation on character switch";
			L.CarryoverTooltipText = "When enabled in Studio, replacing a character preserves the previous character's body and head deform layers, but only for paths whose topology (vertex count + subdivision level + triangle hash) matches the new character. Mismatched paths fall back to the new card's data.";
			L.CarryoverSkippedPathsFormat = "Character-switch carry-over: skipped paths due to topology mismatch: {0}";
			L.CarryoverSettingsHint = "More user preferences will be added here in future updates.";
			L.PsdTabLabel = "PSD";
			L.PsdSectionTitle = "Pose Space Driver";
			L.PsdCharacterOnlyHint = "PSD drivers are available for characters only.";
			L.PsdNoLayerHint = "Select a renderer that has at least one layer to add a driver.";
			L.PsdAddDriver = "Add Driver";
			L.PsdDeleteDriver = "X";
			L.PsdTimelineOverriding = "Timeline overriding";
			L.PsdTargetLayerLabel = "Target Layer:";
			L.PsdSourceBoneLabel = "Source Bone:";
			L.PsdChannelLabel = "Channel:";
			L.PsdInputRangeLabel = "Input Range (Layer Value 0 / Layer Value 1):";
			L.PsdSetMinFromPose = "Set Min from Pose";
			L.PsdSetMaxFromPose = "Set Max from Pose";
			L.PsdCaptureHint = "Pose the bone, then capture each endpoint from the current pose.";
			L.PsdFilterLabel = "Filter:";
			L.PsdNoneSelected = "(none)";
			L.PsdDuplicateNotice = "A driver already targets this layer.";
			L.PsdInvalidRangeNotice = "Input min and max must differ.";
			L.PsdRowHeaderFmt = "{0} : {1}";
			L.PsdRowDetailFmt = "{0}  |  {1}  |  [{2} ~ {3}]";
			L.PsdHelp = "Pose Space Driven (PSD): drive a layer's weight from a bone's pose.\n\nPick a target layer, a source bone, a channel (rotation or position axis) and an input range. As the bone value moves from min to max, the layer weight blends 0 to 1.\n\nIf the same layer has a Timeline keyframe, PSD yields to Timeline (the row shows 'Timeline overriding'); remove the keyframe and PSD resumes.\n\nNote: single-bone driver, not true RBF pose-space deformation.";
			L.PsdChannelNames = new string[] { "Rot X", "Rot Y", "Rot Z", "Pos X", "Pos Y", "Pos Z" };
			L.CsbTabLabel = "CSB";
			L.CsbSectionTitle = "Clothing Status Binding";
			L.CsbCharacterOnlyHint = "CSB drivers are available for characters only.";
			L.CsbNoLayerHint = "Select a renderer that has at least one layer to add a driver.";
			L.CsbAddDriver = "Add Driver";
			L.CsbDeleteDriver = "X";
			L.CsbTimelineOverriding = "Timeline overriding";
			L.CsbTargetLayerLabel = "Target Layer:";
			L.CsbClothingKindLabel = "Clothing:";
			L.CsbUnequippedLabel = "Unequipped:";
			L.CsbCaptureFromState = "Capture from Current State";
			L.CsbCaptureHint = "Sets the current clothing state's weight from the target layer's current weight.";
			L.CsbDuplicateNotice = "A driver already targets this layer.";
			L.CsbNoneSelected = "(none)";
			L.CsbRowHeaderFmt = "{0} → {1}";
			L.CsbRowDetailFmt = "[{0} / {1} / {2} / {3}]  off: {4}";
			L.CsbScopeToggle = "This outfit only";
			L.CsbScopeHint = "On = this driver only applies while wearing the current outfit; off (default) = every outfit that equips this clothing.";
			L.CsbScopeRowFmt = "  · outfit #{0} only";
			L.CsbHelp = "Clothing Status Binding (CSB): drive a layer's weight from a clothing category's wear state.\n\nPick a target layer and a clothing category, then set a weight for each state (On / Shift / Hang / Off) and an unequipped weight. As the clothing state changes the layer weight jumps discretely to the matching entry; taking the clothing off applies the unequipped weight.\n\nPriority is Timeline > CSB > PSD: a Timeline keyframe on the same layer overrides CSB (the row shows 'Timeline overriding'), and CSB overrides PSD.";
			L.CsbStateLabels = new string[] { "On", "Shift", "Hang", "Off" };
			L.NbbTabLabel = "NBB";
			L.NbbSectionTitle = "Native BlendShape Binding";
			L.NbbCharacterOnlyHint = "NBB drivers are available for characters only.";
			L.NbbNoLayerHint = "Select a renderer that has at least one layer to add a driver.";
			L.NbbNoSourceHint = "No renderer on this character exposes a native blend shape.";
			L.NbbAddDriver = "Add Driver";
			L.NbbDeleteDriver = "X";
			L.NbbTimelineOverriding = "Timeline overriding";
			L.NbbTargetLayerLabel = "Target Layer:";
			L.NbbSourceRendererLabel = "Source Mesh:";
			L.NbbSourceShapeLabel = "Source Blend Shape:";
			L.NbbInputRangeLabel = "Input Range 0-100 (Layer Value 0 / Layer Value 1):";
			L.NbbSetMinFromCurrent = "Set Min from Current";
			L.NbbSetMaxFromCurrent = "Set Max from Current";
			L.NbbCaptureHint = "Set the expression, then capture each endpoint from the current value.";
			L.NbbCurrentValueFmt = "Current value: {0}";
			L.NbbFilterLabel = "Filter:";
			L.NbbNoneSelected = "(none)";
			L.NbbDuplicateNotice = "A driver already targets this layer.";
			L.NbbInvalidRangeNotice = "Input min and max must differ.";
			L.NbbRowHeaderFmt = "{0} : {1}";
			L.NbbRowDetailFmt = "{0}  |  {1}  |  [{2} ~ {3}]";
			L.NbbHelp = "Native BlendShape Binding (NBB): drive a layer's weight from one of the character's own blend shapes.\n\nPick a target layer, then a source mesh and one of its blend shapes, and an input range on Unity's native 0-100 scale. As the blend shape weight moves from min to max, the layer weight blends 0 to 1. Set min above max to invert the mapping.\n\nUse the capture buttons: pose the expression you want, then capture the endpoint from the current value instead of guessing numbers.\n\nPriority is Timeline > CSB > NBB > PSD: a Timeline keyframe or a CSB driver on the same layer overrides NBB, and NBB overrides PSD.\n\nThe source mesh list only shows meshes that carry a native blend shape; the plugin's own layer frames are never selectable. If the source mesh or shape is missing (a coordinate without that garment, a swapped head), the driver is skipped and the layer keeps the last weight it was given.";
			L.DeleteSelectedFaces = "Delete Selected Faces";
			L.RestoreSelectedFaces = "Restore Selected Faces";
			L.RestoreAllDeletedFaces = "Restore All Deleted Faces";
			L.DeletedFacesCountFmt = "Deleted: {0} faces";
			L.FaceMaskHelp = "Select faces with the brush or box, then click Delete Selected Faces to hide them from rendering. Deleted faces stay visible in Face Select mode as a red overlay and can still be hit-tested. Click Restore Selected Faces (after selecting the red ones) or Restore All Deleted Faces to bring them back. Press [ / ] to step the Face Select brush radius.";
			L.SectionRenderer = "Renderer";
			L.SectionBrush = "Brush";
			L.SectionGizmo = "Gizmo";
			L.SectionSymmetry = "Symmetry";
			L.SectionLayers = "Layers";
			L.SectionSubdivide = "Subdivide";
			L.SectionPreset = "Preset";
			L.SectionSettings = "Settings";
			L.SectionWeightRemap = "Weight Remap";
			L.SectionHelp = "Help";
			L.SectionResetConfirm = "Confirm Reset";
			L.SectionDeformDataIO = "Deform Data I/O";
			L.SectionDeletedFaces = "Deleted Faces";
			L.SectionOperationMode = "Operation Mode";
			L.BrushRadiusLabel = "Brush Radius";
			L.StrengthLabel = "Strength";
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0001B714 File Offset: 0x00019914
		private static void LoadJapanese()
		{
			L.SelectObject = "オブジェクトを選択してください";
			L.FilterLabel = "フィルター:";
			L.TabNames = new string[] { "シェイプ", "サブディバイド" };
			L.BrushMode = "ブラシ";
			L.GizmoMode = "ギズモ";
			L.MoveTool = "移動";
			L.SmoothTool = "スムーズ";
			L.RelaxTool = "リラックス";
			L.InflateTool = "膨張";
			L.PinchTool = "ピンチ";
			L.CreaseTool = "クリース";
			L.Translate = "移動";
			L.Rotate = "回転";
			L.Scale = "スケール";
			L.WorldSpace = "ワールド";
			L.ObjectSpace = "オブジェクト";
			L.NormalSpace = "法線";
			L.SoftSelection = "ソフト選択";
			L.SoftModeVolume = "ボリューム";
			L.SoftModeSurface = "サーフェス";
			L.SoftSelectionRadius = "ソフト選択半径";
			L.IncludeBackFaceLabel = "裏面の頂点も選択";
			L.IncludeBackFaceTooltip = "有効時（既定）、ギズモの頂点選択（ボックス／ブラシ）はメッシュ裏側（法線がカメラから離れる頂点）も含めます。無効時は表面側の頂点のみ選択。スカルプトのブラシモードには影響しません。";
			L.GizmoSelectBox = "ボックス選択";
			L.GizmoSelectBrush = "ブラシ選択";
			L.GizmoGranularityVertex = "頂点";
			L.GizmoGranularityFace = "面";
			L.GizmoClearSelection = "選択をクリア";
			L.Symmetry = "シンメトリー";
			L.SymmetryAxis = "軸:";
			L.SetCenter = "中心設定";
			L.ClearCenter = "クリア";
			L.SymmetryCenterFmt = "中心: {0:F3}";
			L.Layers = "レイヤー";
			L.LayerWeight = "ウェイト";
			L.AddLayer = "レイヤー追加";
			L.RemoveLayer = "削除";
			L.RenameLayer = "名前変更";
			L.MoveUp = "上へ";
			L.MoveDown = "下へ";
			L.MirrorLayer = "ミラー";
			L.MirrorLayerTooltip = "このレイヤーを左右中心で反転し、新しいレイヤーを作成します（X軸）。";
			L.NoLayerWarning = "レイヤーを作成してください";
			L.LayerDefaultNameFmt = "レイヤー {0}";
			L.TargetMesh = "ターゲットメッシュ:";
			L.VerticesFacesFmt = "頂点: {0} | 面: {1}";
			L.Brush = "ブラシ";
			L.BoxSelect = "ボックス選択";
			L.BrushRadiusFmt = "ブラシ半径: {0}";
			L.SelectedVerticesFmt = "選択: {0} 頂点";
			L.FalloffLinear = "リニア";
			L.FalloffSmooth = "スムーズ";
			L.FalloffSharp = "シャープ";
			L.SelectFaces = "面を選択";
			L.SelectedFacesFmt = "選択: {0} / {1} 面";
			L.AllButton = "全選択";
			L.NoneButton = "選択解除";
			L.InvertButton = "反転";
			L.LevelLabel = "レベル:";
			L.Subdivide = "サブディバイド";
			L.Restore = "復元";
			L.SubdivideSmooth = "表面をスムーズ化";
			L.SubdivideSmoothTooltip = "有効にすると、サブディバイド時に新しい辺の中点を表面の曲率方向へ押し出し（補間方式）、元の頂点を動かさずに形状を滑らかにします。面選択モードでは選択した面のみがスムーズ化されます。";
			L.RebakeSubdivision = "再ベイク";
			L.RebakeSubdivisionTooltip = "別のポーズに切り替えた際に再発するワイヤーフレームの凹凸を修正するため、現在のポーズでボディのスムーズサブディバイド中点を再ベイクします。スカルプトは保持されます。キャラクターのボディ（o_body*）でスムーズサブディバイドがある場合のみ使用可能。保存／再読み込み時は読み込みポーズで再ベイクされます。";
			L.FaceWireframeOpacity = "ワイヤーフレーム不透明度";
			L.FaceWireframeOpacityTooltip = "面選択のワイヤーフレームを薄くします。0 = 完全に非表示（面選択は引き続き可能）、1 = 既定の濃さ。密なメッシュ線なしでサブディバイド後の表面を確認したいときに下げてください。";
			L.FaceSelectIncludeBackFace = "背面も選択";
			L.FaceSelectIncludeBackFaceTooltip = "チェック時（既定）、ブラシ・ボックス選択でカメラと反対を向く裏面も選択できます。オフにすると正面の面だけを選択します。面を削除する前に前面のシェルだけを正確に選びたいときに便利です。オクルージョンは判定されず、厚いメッシュの反対側までブラシが届くわけではありません。";
			L.SubdivideLayerWarning = "サブディバイドするとレイヤーデータがリセットされます。続行しますか？";
			L.StrengthFmt = "強度: {0}";
			L.ApplyButton = "適用";
			L.ClearButton = "クリア";
			L.ConfirmButton = "確認";
			L.CancelButton = "キャンセル";
			L.EnterEditMode = "編集モード開始";
			L.ExitEditMode = "編集モード終了";
			L.CameraMode = "カメラモード (Ctrl)";
			L.BoxSelectModeName = "ボックス選択モード";
			L.EditModeActive = "編集モード";
			L.BoxSelectInfoFmt = "強度: {0} | {1} | 選択: {2} 頂点";
			L.BrushInfoFmt = "半径: {0} | 強度: {1} | {2} | {3}";
			L.ShowMeshHighlight = "メッシュハイライト表示";
			L.ShowMeshWireframe = "メッシュワイヤーフレーム表示";
			L.ShowEditedOnly = "編集済みのみ";
			L.FocusRenderer = "カメラを合わせる";
			L.CatTop = "トップス";
			L.CatBottom = "ボトムス";
			L.CatBra = "ブラ";
			L.CatUnderwear = "ショーツ";
			L.CatGloves = "手袋";
			L.CatPantyhose = "パンスト";
			L.CatLegwear = "靴下";
			L.CatShoesInner = "室内履き";
			L.CatShoesOuter = "外履き";
			L.CatHairBack = "髪(後)";
			L.CatHairFront = "髪(前)";
			L.CatHairSide = "髪(横)";
			L.CatHairOption = "髪(その他)";
			L.CatBody = "ボディ";
			L.CatHead = "ヘッド";
			L.CatClothes = "衣装";
			L.CatAccessory = "アクセ";
			L.CatHair = "髪";
			L.CatItem = "アイテム";
			L.CatAccSlot = "アクセ枠";
			L.VisibilityShow = "表示";
			L.VisibilityHide = "非表示";
			L.PressureLabel = "筆圧";
			L.TabletNotDetected = "タブレット未検出";
			L.HudLayerFmt = "レイヤー: {0}";
			L.HudNoLayer = "(レイヤーなし)";
			L.HudShortcuts = "Ctrl+LMB:回転  Shift:法線\nCtrl+RMB:ズーム  Alt:収縮\nLMB:ブラシ  RMB:選択\n[ ]:半径  Shift+[ ]:強度";
			L.HudGrowShrink = "=:拡大  -:縮小";
			L.UndoLabel = "元に戻す";
			L.RedoLabel = "やり直し";
			L.FaceSelectBrush = "面選択: ブラシ";
			L.FaceSelectBox = "面選択: ボックス";
			L.RadiusSuffixFmt = " | 半径: {0}";
			L.RemapWeights = "ウェイト再割り当て";
			L.RestoreWeights = "ウェイト復元";
			L.BodyMeshNotReadable = "ボディメッシュが読み取れません";
			L.ExportDeform = "エクスポート";
			L.ImportDeform = "インポート";
			L.DeformFileFilter = "形状変形 (*.kksd)|*.kksd";
			L.ExportSuccess = "エクスポート成功";
			L.ImportSuccess = "インポート成功";
			L.ImportVertexMismatchFmt = "頂点数不一致: ファイル {0}, メッシュ {1}";
			L.ImportInvalidFile = "無効なファイル形式";
			L.PresetTabLabel = "プリセット";
			L.ExportPreset = "エクスポート…";
			L.ExportPresetSelected = "選択をエクスポート";
			L.ImportPreset = "インポート…";
			L.ApplyImportPreset = "インポート適用";
			L.CancelImportPreset = "キャンセル";
			L.PresetFileFilter = "シェイププリセット (*.kksp)|*.kksp";
			L.PresetExportSuccessFmt = "プリセットを書き出しました: {0} 件";
			L.PresetImportSummaryFmt = "[Preset] インポート {0} 件、スキップ: {1}";
			L.PresetInvalidFile = "プリセットファイルが無効です（.kksp マジック／バージョン不一致または破損）";
			L.PresetMatchOk = "OK";
			L.PresetMatchNoMatch = "未対応";
			L.PresetRowFormatFmt = "{0}  |  細分:{1}  |  レイヤー:{2}  |  PSD:{3}  |  CSB:{4}  |  NBB:{5}";
			L.PresetRowRootName = "(ルート)";
			L.PresetEmptyOwnerHint = "キャラクターまたは Studio アイテムを選択してください。";
			L.PresetNoExportableHint = "エクスポート可能なデータがありません: 現在の対象にはサブディビジョンもレイヤーもありません。";
			L.LiteShapeNoLayersHint = "現在のキャラクター／アイテムに編集可能なレイヤーがありません。";
			L.PresetPreserveLayers = "既存レイヤーを保持";
			L.PresetPreserveLayersTooltip = "プリセットのレイヤーを置き換えず、既存のレイヤーの後ろに追加します。頂点数が一致する場合のみ適用され、サブディビジョンとフェイスマスクは変更されません。";
			L.PresetPreserveScope = "衣装スコープを保持";
			L.PresetPreserveScopeTooltip = "インポートする CSB ドライバー（置換モード）向け：グローバルはグローバルのまま、衣装限定のドライバーは現在の衣装に再バインドします。オフにするとインポートした CSB ドライバーをすべてグローバルにします。";
			L.HelpTitle = "ヘルプ";
			L.HelpBrush = "ブラシモード — メッシュ上をペイントして造形します。\n\n準備\n- レンダラーを選択し、編集モード開始を押します。\n- 造形前にレイヤーを追加してください(必須)。\n\nツール\n- 移動: 画面方向に頂点をドラッグ。Shift で法線方向の押し引き。\n- スムーズ: ブラシ内の頂点位置を平均化（未編集の面でも凹凸を平らに）。\n- リラックス: 変形デルタを近傍へ平均化（未編集の面には無効）。\n- 膨張: 法線方向に押し出し。Alt で収縮。\n- ピンチ: 頂点を表面に沿ってブラシ中心へ引き寄せ、ディテールを締める。Alt で外側へ。\n- クリース: ピンチに法線方向の押し込みを加え、鋭い溝を彫る。Alt で鋭い尾根に。\n\nパラメータ\n- 半径: ブラシのサイズ。\n- 強度: フレーム毎の影響度。\n- フォールオフ: リニア / スムーズ / シャープ。\n- [ / ]: 半径を増減; Shift+[ / Shift+]: 強度を増減。\n\nシンメトリー\n- 有効化して軸(X/Y/Z)を選択。\n- 参照頂点で中心を設定、または 0 のままにします。\n\nレイヤー\n- 複数の変形を重ね、各レイヤーにウェイトを設定。\n- 上/下で順序変更。最終位置 = Σ(レイヤーのデルタ × ウェイト)。\n\nウェイト再割り当て(衣装向け)\n- 大変形後、ウェイト再割り当てを押すとスキニングが最寄りのボディボーンに追従します。\n- ウェイト復元で元のボーンウェイトに戻します。\n\nエクスポート / インポート (.kksd)\n- 現在のレンダラーのレイヤーをいつでもエクスポート可能。\n- インポートは現在のレンダラーにレイヤーを統合します(編集モード時のみ)。\n\nショートカット\n- {0}\n- Ctrl を押しっぱなし: 入力をカメラ操作に渡します。";
			L.HelpGizmo = "ギズモモード — 選択頂点をハンドルで変換します。\n\n選択方式(ボックス / ブラシ)\n- ボックス: 何もない場所で左クリックドラッグし、画面上の矩形で選択します。\n- ブラシ: 左クリックドラッグで表面に沿って選択を塗り重ねます(カーソルにブラシ円が表示)。\n- 選択をクリア: 現在の選択を空にします(ブラシは既定で加算のみなのでリセット用)。\n\nマウス操作(左ボタン)\n- ボックス方式: 何もない場所でドラッグしてボックス選択。Shift で追加、Alt で削除。離した時点で選択が確定。\n- ブラシ方式: 表面をドラッグして毎フレーム頂点を追加。Alt + ドラッグで消去。既定で加算(ストロークが重なる)。\n- ハンドル(軸 / 平面 / 中心立方体 / ViewRotate リング)で左クリックドラッグ: 移動 / 回転 / スケール。\n- 選択操作またはハンドル操作は、開始したらボタンを離すまでロックされます。\n- Ctrl 押し: 入力をカメラ操作に渡します。新規の選択 / ハンドル操作は開始されません。\n- 右マウスボタンはギズモモードでは未使用です。\n\nハンドル\n- 移動: 3 軸 + XY/XZ/YZ 平面 + 中心立方体 (Free)。\n- 回転: 3 軸リング + 外側の白いリング (ViewRotate、カメラに正対)。\n- スケール: 3 軸 + 中心立方体 (均等)。\n\n座標空間\n- ワールド: 固定 XYZ。\n- オブジェクト: キャラ/アイテムのルート回転に沿う。\n- 法線: 選択頂点の平均法線に沿う。\n\nソフト選択\n- 確定選択の外側まで影響を広げます。\n- 半径: 影響距離。\n- [ / ]: ボックス方式ではこのソフト選択半径をステップ調整(ソフト選択の有効/無効を問わず動作)。ブラシ方式ではブラシ選択の半径を調整。\n- ボリューム: 空間を貫通する直線距離。\n- サーフェス: メッシュ辺に沿った BFS(薄いメッシュを貫通しません)。\n- 半径/モード変更は 150 ms のスロットリング後に再計算されます。\n\nシンメトリー\n- 有効化して軸を選択。移動/回転/スケールが自動的にミラーリングされます。\n\n選択の拡大/縮小\n- =: 選択をトポロジー的に 1 リング外側へ拡大(頂点モード・面モード共通)。\n- -: 選択を 1 リング内側へ縮小。1 回押すごとに 1 リング(オートリピートなし)。\n- カーソルがプラグインウィンドウ上にあっても動作します。\n\nショートカット\n- {0}\n- Ctrl を押しっぱなし: 入力をカメラ操作に渡します。";
			L.HelpSubdivide = "サブディバイド — 造形精度を高めるためメッシュ密度を増やします。\n\n面の選択\n- 面選択オーバーレイで分割対象の三角形を選びます。\n- 全選択 / 選択解除 / 反転の補助機能があります。\n\n操作\n- レベル: 分割段階(1 = 4倍、2 = 16倍の三角形)。\n- サブディバイド: 選択された面に適用。\n- 復元: メッシュを元のトポロジーに戻します。\n\n注意\n- サブディバイドすると全レイヤーのデルタデータがリセットされます(頂点数が変化するため)。\n- サブディバイド後は Undo 履歴がクリアされます。\n\nプラットフォーム制限\n- Koikatsu (net35): メッシュは 65535 頂点を超えられません — 超過時はサブディバイドが中止されます。\n- Koikatsu Sunshine: UInt32 インデックス形式により 65535 超の頂点をサポート。";
			L.HelpPreset = "プリセット — .kksp ファイルで変形をキャラクター間で保存・再利用します。\n\nエクスポート\n- 対象のレンダラーにチェックを入れ、.kksp ファイルへエクスポートします。\n- サブディバイドまたはレイヤーを持つレンダラーのみ表示されます。\n\nインポート\n- .kksp をインポートするとプレビューに入り、適用する項目にチェックして Apply を押します。\n- 同じアセットが2つのスロット(靴など)にある場合は自動的に両方へ適用されます。\n\n置換とプリザーブ\n- 置換(既定): サブディバイドを再構築し、レイヤーとフェイスマスクを置き換えます。\n- プリザーブ: レイヤーを追加するのみ。トポロジーが一致する必要があり、サブディバイドとフェイスマスクは変更しません。\n\nドライバー (PSD / CSB)\n- PSD / CSB ドライバーはプリセットに含まれ、置換モードでのみ復元されます。\n- プリザーブモードでは同梱ドライバーは無視されます(追加時にレイヤー GUID が再割り当てされるため)。\n- 衣装スコープを保持: 衣装限定の CSB ドライバーを現在の衣装にバインドしたまま保持します(オフでグローバル化)。";
			L.CorruptedEntryPrefix = "[!] ";
			L.CorruptedTooltipFormat = "保存されたレイヤーのデルタがこのメッシュと一致せず、トポロジーを再構築するためのサブディバイド情報が欠落しています。";
			L.ResetCorruptedButton = "リセット";
			L.ResetCorruptedConfirmTitle = "破損レイヤーをリセット";
			L.ResetCorruptedConfirmBody = "パス: {0}\n破棄するレイヤー数: {1}\n\nこの操作は取り消せません。";
			L.CorruptedEditModeBanner = "対象が破損しています。ブラシ／ギズモ編集は正しく動作しない可能性があります。";
			L.SettingsTabLabel = "設定";
			L.CarryoverToggleLabel = "キャラクター切替時にボディ／頭部の変形を引き継ぐ";
			L.CarryoverTooltipText = "Studio で有効にすると、キャラクターを切り替える際、トポロジー(頂点数 + サブディバイドレベル + 三角形ハッシュ)が新キャラクターと一致するパスのみ前キャラクターのボディ／頭部のレイヤーを引き継ぎます。一致しないパスは新カードのデータが使われます。";
			L.CarryoverSkippedPathsFormat = "キャラクター切替の引き継ぎ: トポロジー不一致でスキップしたパス: {0}";
			L.CarryoverSettingsHint = "今後のアップデートでさらに設定項目が追加される予定です。";
			L.PsdTabLabel = "PSD";
			L.PsdSectionTitle = "ポーズ駆動 (PSD)";
			L.PsdCharacterOnlyHint = "PSD ドライバーはキャラクターのみ利用できます。";
			L.PsdNoLayerHint = "ドライバーを追加するにはレイヤーが1つ以上あるレンダラーを選択してください。";
			L.PsdAddDriver = "ドライバー追加";
			L.PsdDeleteDriver = "X";
			L.PsdTimelineOverriding = "Timeline 制御中";
			L.PsdTargetLayerLabel = "対象レイヤー:";
			L.PsdSourceBoneLabel = "参照ボーン:";
			L.PsdChannelLabel = "チャンネル:";
			L.PsdInputRangeLabel = "入力範囲 (レイヤー値 0 / レイヤー値 1):";
			L.PsdSetMinFromPose = "最小=現在の姿勢";
			L.PsdSetMaxFromPose = "最大=現在の姿勢";
			L.PsdCaptureHint = "ボーンをポーズさせてから、現在の姿勢で各端点を取得します。";
			L.PsdFilterLabel = "フィルター:";
			L.PsdNoneSelected = "(なし)";
			L.PsdDuplicateNotice = "このレイヤーには既にドライバーが設定されています。";
			L.PsdInvalidRangeNotice = "入力の最小値と最大値は異なる必要があります。";
			L.PsdRowHeaderFmt = "{0} : {1}";
			L.PsdRowDetailFmt = "{0}  |  {1}  |  [{2} ~ {3}]";
			L.PsdHelp = "ポーズ駆動 (PSD): ボーンのポーズからレイヤーのウェイトを駆動します。\n\n対象レイヤー、参照ボーン、チャンネル（回転または位置軸）、入力範囲を選びます。ボーン値が最小から最大に動くと、ウェイトが 0 から 1 に補間されます。\n\n同じレイヤーに Timeline のキーフレームがある場合、PSD は Timeline に譲ります（行に「Timeline 制御中」と表示）。キーフレームを削除すると PSD が再開します。\n\n注: 単一ボーンのドライバーであり、真の RBF ポーズ空間変形ではありません。";
			L.PsdChannelNames = new string[] { "Rot X", "Rot Y", "Rot Z", "Pos X", "Pos Y", "Pos Z" };
			L.CsbTabLabel = "CSB";
			L.CsbSectionTitle = "衣装状態バインド (CSB)";
			L.CsbCharacterOnlyHint = "CSB ドライバーはキャラクターのみ利用できます。";
			L.CsbNoLayerHint = "ドライバーを追加するにはレイヤーが1つ以上あるレンダラーを選択してください。";
			L.CsbAddDriver = "ドライバーを追加";
			L.CsbDeleteDriver = "X";
			L.CsbTimelineOverriding = "Timeline 制御中";
			L.CsbTargetLayerLabel = "対象レイヤー:";
			L.CsbClothingKindLabel = "衣装:";
			L.CsbUnequippedLabel = "未装着:";
			L.CsbCaptureFromState = "現在の状態から取得";
			L.CsbCaptureHint = "現在の衣装状態の重みを、対象レイヤーの現在の重みから設定します。";
			L.CsbDuplicateNotice = "このレイヤーには既にドライバーがあります。";
			L.CsbNoneSelected = "(なし)";
			L.CsbRowHeaderFmt = "{0} → {1}";
			L.CsbRowDetailFmt = "[{0} / {1} / {2} / {3}]  未装着: {4}";
			L.CsbScopeToggle = "この衣装のみ";
			L.CsbScopeHint = "オン = 現在の衣装を着ている間のみ適用。オフ（既定）= この衣装を着けている全コーディネートで適用。";
			L.CsbScopeRowFmt = "  · 衣装 #{0} のみ";
			L.CsbHelp = "衣装状態バインド (CSB): 衣装カテゴリの着用状態からレイヤーの重みを駆動します。\n\n対象レイヤーと衣装カテゴリを選び、各状態 (On / Shift / Hang / Off) の重みと未装着時の重みを設定します。衣装状態が変わると、レイヤーの重みは対応する値へ離散的に切り替わります。衣装を脱ぐと未装着の重みが適用されます。\n\n優先順位は Timeline > CSB > PSD です。同じレイヤーに Timeline キーフレームがあると CSB より優先され (行に「Timeline 制御中」と表示)、CSB は PSD より優先されます。";
			L.CsbStateLabels = new string[] { "On", "Shift", "Hang", "Off" };
			L.NbbTabLabel = "NBB";
			L.NbbSectionTitle = "ネイティブブレンドシェイプ連動 (NBB)";
			L.NbbCharacterOnlyHint = "NBB ドライバーはキャラクターのみ利用できます。";
			L.NbbNoLayerHint = "ドライバーを追加するにはレイヤーが1つ以上あるレンダラーを選択してください。";
			L.NbbNoSourceHint = "このキャラクターにはブレンドシェイプを持つレンダラーがありません。";
			L.NbbAddDriver = "ドライバーを追加";
			L.NbbDeleteDriver = "X";
			L.NbbTimelineOverriding = "Timeline 制御中";
			L.NbbTargetLayerLabel = "対象レイヤー:";
			L.NbbSourceRendererLabel = "ソースメッシュ:";
			L.NbbSourceShapeLabel = "ソースブレンドシェイプ:";
			L.NbbInputRangeLabel = "入力範囲 0-100 (レイヤー値 0 / レイヤー値 1):";
			L.NbbSetMinFromCurrent = "現在値から最小を取得";
			L.NbbSetMaxFromCurrent = "現在値から最大を取得";
			L.NbbCaptureHint = "表情を作ってから、現在の値で各端点を取得します。";
			L.NbbCurrentValueFmt = "現在値: {0}";
			L.NbbFilterLabel = "フィルター:";
			L.NbbNoneSelected = "(なし)";
			L.NbbDuplicateNotice = "このレイヤーには既にドライバーがあります。";
			L.NbbInvalidRangeNotice = "入力の最小値と最大値は異なる必要があります。";
			L.NbbRowHeaderFmt = "{0} : {1}";
			L.NbbRowDetailFmt = "{0}  |  {1}  |  [{2} ~ {3}]";
			L.NbbHelp = "ネイティブブレンドシェイプ連動 (NBB): キャラクター本来のブレンドシェイプでレイヤーの重みを駆動します。\n\n対象レイヤーを選び、ソースメッシュとそのブレンドシェイプ、そして Unity 標準の 0-100 スケールでの入力範囲を指定します。ブレンドシェイプの値が最小から最大へ動くと、レイヤーの重みが 0 から 1 へ変化します。最小値を最大値より大きくすると反転します。\n\n取得ボタンの活用: 目的の表情にしてから現在値で端点を取得すれば、数値を推測する必要はありません。\n\n優先順位は Timeline > CSB > NBB > PSD です。同じレイヤーに Timeline キーフレームまたは CSB ドライバーがあると NBB は譲り、NBB は PSD より優先されます。\n\nソースメッシュ一覧にはブレンドシェイプを持つメッシュのみ表示され、プラグイン自身のレイヤーフレームは選択できません。ソースのメッシュやシェイプが見つからない場合（その衣装がないコーディネート、頭部の差し替えなど）、ドライバーはスキップされ、レイヤーは最後に設定された重みを保ちます。";
			L.DeleteSelectedFaces = "選択フェースを削除";
			L.RestoreSelectedFaces = "選択フェースを復元";
			L.RestoreAllDeletedFaces = "すべての削除フェースを復元";
			L.DeletedFacesCountFmt = "削除済み: {0} フェース";
			L.FaceMaskHelp = "ブラシやボックスでフェースを選択し、「選択フェースを削除」をクリックすると描画から除外されます。削除されたフェースはフェース選択モードで赤い半透明として表示され、引き続き選択可能です。赤いフェースを選んでから「選択フェースを復元」、または「すべての削除フェースを復元」で元に戻せます。ブラシ半径は [ / ] でステップ調整できます。";
			L.SectionRenderer = "レンダラー";
			L.SectionBrush = "ブラシ";
			L.SectionGizmo = "ギズモ";
			L.SectionSymmetry = "シンメトリー";
			L.SectionLayers = "レイヤー";
			L.SectionSubdivide = "細分化";
			L.SectionPreset = "プリセット";
			L.SectionSettings = "設定";
			L.SectionWeightRemap = "ウェイト再マッピング";
			L.SectionHelp = "ヘルプ";
			L.SectionResetConfirm = "リセット確認";
			L.SectionDeformDataIO = "変形データ入出力";
			L.SectionDeletedFaces = "削除された面";
			L.SectionOperationMode = "操作モード";
			L.BrushRadiusLabel = "ブラシ半径";
			L.StrengthLabel = "強度";
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0001C168 File Offset: 0x0001A368
		private static void LoadTraditionalChinese()
		{
			L.SelectObject = "請選擇物件";
			L.FilterLabel = "篩選:";
			L.TabNames = new string[] { "形狀", "細分" };
			L.BrushMode = "筆刷";
			L.GizmoMode = "Gizmo";
			L.MoveTool = "推拉";
			L.SmoothTool = "平滑";
			L.RelaxTool = "鬆弛";
			L.InflateTool = "膨脹";
			L.PinchTool = "夾捏";
			L.CreaseTool = "皺摺";
			L.Translate = "位移";
			L.Rotate = "旋轉";
			L.Scale = "縮放";
			L.WorldSpace = "世界";
			L.ObjectSpace = "物件";
			L.NormalSpace = "法線";
			L.SoftSelection = "軟選取";
			L.SoftModeVolume = "體積";
			L.SoftModeSurface = "表面";
			L.SoftSelectionRadius = "軟選取半徑";
			L.IncludeBackFaceLabel = "選取背面頂點";
			L.IncludeBackFaceTooltip = "啟用（預設）時，Gizmo 頂點選取（框選與筆刷）會選到 mesh 背面（法線朝後）的頂點。關閉時只選表面（朝向相機）的頂點。雕刻的 Brush 模式不受此設定影響。";
			L.GizmoSelectBox = "框選";
			L.GizmoSelectBrush = "筆刷選取";
			L.GizmoGranularityVertex = "頂點";
			L.GizmoGranularityFace = "面";
			L.GizmoClearSelection = "清除選取";
			L.Symmetry = "對稱";
			L.SymmetryAxis = "軸:";
			L.SetCenter = "設定中心";
			L.ClearCenter = "清除";
			L.SymmetryCenterFmt = "中心: {0:F3}";
			L.Layers = "圖層";
			L.LayerWeight = "權重";
			L.AddLayer = "新增圖層";
			L.RemoveLayer = "刪除";
			L.RenameLayer = "重新命名";
			L.MoveUp = "上移";
			L.MoveDown = "下移";
			L.MirrorLayer = "鏡像";
			L.MirrorLayerTooltip = "將此圖層沿身體左右中線鏡射成一個新圖層（X 軸）。";
			L.NoLayerWarning = "請先建立圖層才能開始雕刻";
			L.LayerDefaultNameFmt = "圖層 {0}";
			L.TargetMesh = "目標網格:";
			L.VerticesFacesFmt = "頂點: {0} | 面: {1}";
			L.Brush = "筆刷";
			L.BoxSelect = "框選";
			L.BrushRadiusFmt = "筆刷半徑: {0}";
			L.SelectedVerticesFmt = "已選: {0} 頂點";
			L.FalloffLinear = "線性";
			L.FalloffSmooth = "平滑";
			L.FalloffSharp = "銳利";
			L.SelectFaces = "選取面";
			L.SelectedFacesFmt = "已選: {0} / {1} 面";
			L.AllButton = "全選";
			L.NoneButton = "取消";
			L.InvertButton = "反轉";
			L.LevelLabel = "層級:";
			L.Subdivide = "細分";
			L.Restore = "還原";
			L.SubdivideSmooth = "平滑表面";
			L.SubdivideSmoothTooltip = "啟用後，細分時會把新的邊中點朝表面曲率方向外推（插值方式），在不移動原始頂點的前提下讓造型更圓滑。面選取模式下只平滑被選取的面。";
			L.RebakeSubdivision = "重新烤定";
			L.RebakeSubdivisionTooltip = "以當前姿勢重新烤定身體平滑細分的中點，修正切換到其他姿勢後重新出現的線框凹凸。會保留你的雕刻。僅在選取角色身體（o_body*）且已套用平滑細分時可用。存檔／重載時會以載入姿勢重新烤定。";
			L.FaceWireframeOpacity = "線框不透明度";
			L.FaceWireframeOpacityTooltip = "淡化面選取的線框。0 = 完全隱藏（仍可選面）、1 = 預設濃度。想檢視細分後的表面而不受密集網格線干擾時可調低。";
			L.FaceSelectIncludeBackFace = "納入背面";
			L.FaceSelectIncludeBackFaceTooltip = "勾選時（預設），筆刷與框選可選到法線背向相機的背面面片。取消勾選只選正面面片，想在刪面前精準選取正面殼時很方便。不做遮擋判定，也無法讓筆刷伸到厚網格的遠側。";
			L.SubdivideLayerWarning = "細分將會重置所有圖層資料，是否繼續？";
			L.StrengthFmt = "強度: {0}";
			L.ApplyButton = "套用";
			L.ClearButton = "清除";
			L.ConfirmButton = "確認";
			L.CancelButton = "取消";
			L.EnterEditMode = "進入編輯模式";
			L.ExitEditMode = "退出編輯模式";
			L.CameraMode = "相機模式 (Ctrl)";
			L.BoxSelectModeName = "框選模式";
			L.EditModeActive = "編輯模式";
			L.BoxSelectInfoFmt = "強度: {0} | {1} | 已選: {2} 頂點";
			L.BrushInfoFmt = "半徑: {0} | 強度: {1} | {2} | {3}";
			L.ShowMeshHighlight = "顯示網格高亮";
			L.ShowMeshWireframe = "顯示網格線框";
			L.ShowEditedOnly = "只顯示已編輯";
			L.FocusRenderer = "聚焦相機";
			L.CatTop = "上衣";
			L.CatBottom = "下著";
			L.CatBra = "胸罩";
			L.CatUnderwear = "內褲";
			L.CatGloves = "手套";
			L.CatPantyhose = "褲襪";
			L.CatLegwear = "襪子";
			L.CatShoesInner = "室內鞋";
			L.CatShoesOuter = "室外鞋";
			L.CatHairBack = "後髮";
			L.CatHairFront = "前髮";
			L.CatHairSide = "側髮";
			L.CatHairOption = "選用髮";
			L.CatBody = "身體";
			L.CatHead = "頭部";
			L.CatClothes = "服裝";
			L.CatAccessory = "飾品";
			L.CatHair = "頭髮";
			L.CatItem = "物件";
			L.CatAccSlot = "飾品槽";
			L.VisibilityShow = "顯示";
			L.VisibilityHide = "隱藏";
			L.PressureLabel = "筆壓";
			L.TabletNotDetected = "未偵測到繪圖板";
			L.HudLayerFmt = "圖層: {0}";
			L.HudNoLayer = "(無圖層)";
			L.HudShortcuts = "Ctrl+LMB:旋轉  Shift:法線\nCtrl+RMB:縮放  Alt:收縮\nLMB:筆刷  RMB:選取\n[ ]:半徑  Shift+[ ]:力道";
			L.HudGrowShrink = "=:擴張  -:收縮";
			L.UndoLabel = "復原";
			L.RedoLabel = "重做";
			L.FaceSelectBrush = "面選取: 筆刷";
			L.FaceSelectBox = "面選取: 框選";
			L.RadiusSuffixFmt = " | 半徑: {0}";
			L.RemapWeights = "重新分配權重";
			L.RestoreWeights = "還原權重";
			L.BodyMeshNotReadable = "身體網格無法讀取";
			L.ExportDeform = "匯出";
			L.ImportDeform = "匯入";
			L.DeformFileFilter = "形狀變形 (*.kksd)|*.kksd";
			L.ExportSuccess = "匯出成功";
			L.ImportSuccess = "匯入成功";
			L.ImportVertexMismatchFmt = "頂點數不一致: 檔案 {0}, 網格 {1}";
			L.ImportInvalidFile = "無效的檔案格式";
			L.PresetTabLabel = "預設集";
			L.ExportPreset = "匯出…";
			L.ExportPresetSelected = "匯出已勾選";
			L.ImportPreset = "匯入…";
			L.ApplyImportPreset = "套用匯入";
			L.CancelImportPreset = "取消";
			L.PresetFileFilter = "形狀預設集 (*.kksp)|*.kksp";
			L.PresetExportSuccessFmt = "已匯出預設集：{0} 筆";
			L.PresetImportSummaryFmt = "[Preset] 已匯入 {0} 筆，跳過：{1}";
			L.PresetInvalidFile = "預設集檔案無效（.kksp magic／版本不符或內容損毀）";
			L.PresetMatchOk = "OK";
			L.PresetMatchNoMatch = "找不到對應";
			L.PresetRowFormatFmt = "{0}  |  細分:{1}  |  圖層:{2}  |  PSD:{3}  |  CSB:{4}  |  NBB:{5}";
			L.PresetRowRootName = "(根節點)";
			L.PresetEmptyOwnerHint = "請先選擇角色或 Studio 物件以填入清單。";
			L.PresetNoExportableHint = "目前角色 / 物件沒有可匯出的細分或變形。";
			L.LiteShapeNoLayersHint = "目前角色／物件沒有可編輯的圖層。";
			L.PresetPreserveLayers = "保留現有 Layer";
			L.PresetPreserveLayersTooltip = "將 preset 的 Layer 疊加到現有 Layer 之後，而非取代。僅在頂點數一致時生效；不更動細分等級與 face mask。";
			L.PresetPreserveScope = "保留服裝範圍";
			L.PresetPreserveScopeTooltip = "針對匯入的 CSB driver（Replace 模式）：全域維持全域，僅限某套裝的 driver 改綁到目前套裝。關閉則把匯入的 CSB driver 全部設為全域。";
			L.HelpTitle = "說明";
			L.HelpBrush = "筆刷模式 — 在網格上繪製來雕刻形狀。\n\n準備\n- 選擇目標 renderer，按下「進入編輯模式」。\n- 雕刻前必須先新增圖層。\n\n工具\n- 推拉：預設沿畫面方向拖動頂點。按住 Shift 改為沿法線推拉。\n- 平滑：將筆刷內的頂點位置平均化（即使未雕區也削平表面起伏）。\n- 鬆弛：將雕刻位移（delta）向鄰域平均化（未雕區不受影響）。\n- 膨脹：沿法線推出。按住 Alt 反向收縮。\n- 夾捏：沿表面把頂點拉向筆刷中心，收緊細節。按住 Alt 往外推開。\n- 皺摺：夾捏再沿法線內推，刻出尖銳溝槽。按住 Alt 形成尖銳凸脊。\n\n參數\n- 半徑：筆刷大小。\n- 強度：每幀影響強度。\n- 衰減：線性 / 平滑 / 銳利。\n- [ / ]：調整半徑; Shift+[ / Shift+]：調整力道。\n\n對稱\n- 啟用後選擇軸向 (X/Y/Z)。\n- 於參考頂點設定中心，或保持為 0。\n\n圖層\n- 多段變形可堆疊，各自有獨立權重。\n- 用「上移 / 下移」調整順序；最終位移 = Σ(圖層 delta × 權重)。\n\n重新分配權重（衣服專用）\n- 大幅變形後按「重新分配權重」，可讓 skinning 跟隨最近的身體骨骼。\n- 「還原權重」可回復原始骨骼權重。\n\n匯出 / 匯入 (.kksd)\n- 任何時候都可匯出當前 renderer 的圖層。\n- 匯入會合併圖層到當前 renderer（僅編輯模式可用）。\n\n快捷鍵\n- {0}\n- 按住 Ctrl：輸入交給相機控制。";
			L.HelpGizmo = "Gizmo 模式 — 以手柄變換已選取的頂點。\n\n選取方式（框選 / 筆刷）\n- 框選：在空白處左鍵拖曳出螢幕矩形來選取。\n- 筆刷：左鍵拖曳沿表面塗抹累積選取（游標顯示筆刷圈）。\n- 清除選取：清空目前選取（筆刷預設只加選，此為歸零路徑）。\n\n滑鼠操作（左鍵）\n- 框選方式：在空白處拖曳框選；Shift 加選、Alt 減選；放開左鍵時套用選取。\n- 筆刷方式：沿表面拖曳逐幀加選；Alt + 拖曳擦除。預設加選（多筆堆疊）。\n- 左鍵拖曳手柄（軸 / 平面 / 中心方塊 / ViewRotate 環）：位移 / 旋轉 / 縮放。\n- 選取手勢或手柄拖曳一旦啟動，會鎖到放開左鍵為止。\n- 按住 Ctrl：輸入交給相機控制；不啟動新的選取或手柄拖曳。\n- 右鍵在 Gizmo 模式下未使用。\n\n手柄\n- 位移：3 軸 + XY/XZ/YZ 平面 + 中心方塊 (Free)。\n- 旋轉：3 軸環 + 外圍白環 (ViewRotate，面向相機)。\n- 縮放：3 軸 + 中心方塊（均勻縮放）。\n\n座標空間\n- World：固定 XYZ。\n- Object：對齊角色 / 物件根節點旋轉。\n- Normal：對齊選取頂點的平均法線。\n\n軟選取\n- 將影響範圍擴展到硬選取頂點之外。\n- 半徑：影響距離。\n- [ / ]：框選方式步進此軟選取半徑（軟選取啟用與否皆可用）；筆刷方式改步進筆刷選取半徑。\n- 體積：空間中的直線距離。\n- 表面：沿 mesh 邊做 BFS（不穿透薄 mesh）。\n- 半徑 / 模式變更節流 150 ms 後才重算。\n\n對稱\n- 啟用並選擇軸向，移動 / 旋轉 / 縮放自動鏡射。\n\n擴張 / 收縮選取\n- =：沿 mesh 拓樸把選取往外擴張一環（點模式與面模式共用）。\n- -：把選取往內收縮一環；每按一次一環（無長按自動重複）。\n- 游標懸在插件視窗上時仍可使用。\n\n快捷鍵\n- {0}\n- 按住 Ctrl：輸入交給相機控制。";
			L.HelpSubdivide = "細分 — 提高 mesh 密度以進行更精細的雕刻。\n\n面選取\n- 使用面選取 overlay 選出要細分的三角形。\n- 提供「全選 / 取消 / 反轉」輔助按鈕。\n\n控制\n- 層級：細分深度（1 = 4 倍、2 = 16 倍三角形）。\n- 細分：對已選取的面套用細分。\n- 還原：回復到原始 mesh 拓撲。\n\n注意事項\n- 細分會重置所有圖層 delta（頂點數會改變）。\n- 細分後 Undo 歷史會被清空。\n\n平台限制\n- Koikatsu (net35)：mesh 不可超過 65535 頂點，超過時細分會中止。\n- Koikatsu Sunshine：透過 UInt32 索引格式支援超過 65535 頂點。";
			L.HelpPreset = "Preset — 透過 .kksp 檔在角色之間儲存與重用變形。\n\n匯出\n- 勾選要匯出的 renderer，再匯出成 .kksp 檔。\n- 只列出有細分或有圖層的 renderer。\n\n匯入\n- 匯入 .kksp 進入預覽，勾選要套用的項目後按 Apply。\n- 同一素材裝在兩個槽位（例如鞋子）會自動套到兩者。\n\nReplace 與 Preserve\n- Replace（預設）：重建細分、取代圖層與 face mask。\n- Preserve：只疊加圖層，拓樸須一致，不更動細分與 face mask。\n\nDriver（PSD / CSB）\n- PSD / CSB driver 會隨 preset 一起匯出，且僅在 Replace 模式還原。\n- Preserve 模式會忽略隨附的 driver（疊加時圖層 GUID 會被重新指派）。\n- 保留服裝範圍：讓僅限某套裝的 CSB driver 綁定目前套裝（關閉則全部改為全域）。";
			L.CorruptedEntryPrefix = "[!] ";
			L.CorruptedTooltipFormat = "已儲存的圖層 delta 與此網格不符，且重建拓樸所需的細分資料缺失。";
			L.ResetCorruptedButton = "重置";
			L.ResetCorruptedConfirmTitle = "重置損壞圖層";
			L.ResetCorruptedConfirmBody = "路徑: {0}\n將丟棄的圖層數: {1}\n\n此操作無法復原。";
			L.CorruptedEditModeBanner = "此目標已損壞。筆刷與 Gizmo 編輯可能產生錯誤結果。";
			L.SettingsTabLabel = "設定";
			L.CarryoverToggleLabel = "切換角色時延續身體 / 臉部變形";
			L.CarryoverTooltipText = "在 Studio 中啟用此選項後，切換角色時會保留前角色的身體 / 臉部變形圖層；僅當該 path 的拓樸（頂點數 + 細分層級 + 三角形 hash）與新角色完全一致才延續，否則使用新卡片資料。";
			L.CarryoverSkippedPathsFormat = "切換角色變形延續：以下 path 因拓樸不一致已略過：{0}";
			L.CarryoverSettingsHint = "未來會在此頁面新增更多偏好設定。";
			L.PsdTabLabel = "PSD";
			L.PsdSectionTitle = "姿勢驅動 (PSD)";
			L.PsdCharacterOnlyHint = "PSD 驅動僅適用於角色。";
			L.PsdNoLayerHint = "請選擇至少有一個 Layer 的 renderer 才能新增驅動。";
			L.PsdAddDriver = "新增驅動";
			L.PsdDeleteDriver = "X";
			L.PsdTimelineOverriding = "Timeline 接管中";
			L.PsdTargetLayerLabel = "目標 Layer：";
			L.PsdSourceBoneLabel = "來源骨：";
			L.PsdChannelLabel = "通道：";
			L.PsdInputRangeLabel = "輸入範圍（Layer 值 0 / Layer 值 1）：";
			L.PsdSetMinFromPose = "最小＝目前姿勢";
			L.PsdSetMaxFromPose = "最大＝目前姿勢";
			L.PsdCaptureHint = "先擺好骨頭姿勢，再從目前姿勢擷取各端點。";
			L.PsdFilterLabel = "篩選：";
			L.PsdNoneSelected = "（無）";
			L.PsdDuplicateNotice = "此 Layer 已有驅動。";
			L.PsdInvalidRangeNotice = "輸入最小值與最大值必須不同。";
			L.PsdRowHeaderFmt = "{0} : {1}";
			L.PsdRowDetailFmt = "{0}  |  {1}  |  [{2} ~ {3}]";
			L.PsdHelp = "姿勢驅動 (PSD)：由骨頭姿勢驅動某個 Layer 的 weight。\n\n選擇目標 Layer、來源骨、通道（旋轉或位置軸）與輸入範圍。當骨值從最小變到最大時，weight 在 0 到 1 之間線性混合。\n\n若同一 Layer 已在 Timeline 下 keyframe，PSD 會讓位給 Timeline（該列顯示「Timeline 接管中」）；刪除 keyframe 後 PSD 自動恢復。\n\n註：此為單骨 driver，非真正的 RBF pose-space 變形。";
			L.PsdChannelNames = new string[] { "Rot X", "Rot Y", "Rot Z", "Pos X", "Pos Y", "Pos Z" };
			L.CsbTabLabel = "CSB";
			L.CsbSectionTitle = "衣裝狀態綁定 (CSB)";
			L.CsbCharacterOnlyHint = "CSB driver 僅適用於角色。";
			L.CsbNoLayerHint = "請選擇至少有一個 Layer 的 renderer 才能新增 driver。";
			L.CsbAddDriver = "新增 Driver";
			L.CsbDeleteDriver = "X";
			L.CsbTimelineOverriding = "Timeline 接管中";
			L.CsbTargetLayerLabel = "目標 Layer：";
			L.CsbClothingKindLabel = "衣裝：";
			L.CsbUnequippedLabel = "未裝備：";
			L.CsbCaptureFromState = "從當前狀態擷取";
			L.CsbCaptureHint = "將當前衣裝狀態的 weight 設為目標 Layer 目前的 weight。";
			L.CsbDuplicateNotice = "此 Layer 已有一個 driver。";
			L.CsbNoneSelected = "（無）";
			L.CsbRowHeaderFmt = "{0} → {1}";
			L.CsbRowDetailFmt = "[{0} / {1} / {2} / {3}]  未裝備：{4}";
			L.CsbScopeToggle = "僅限目前服裝";
			L.CsbScopeHint = "開啟＝只在穿著目前服裝時生效；關閉（預設）＝所有裝備此衣裝的服裝都生效。";
			L.CsbScopeRowFmt = "  · 僅服裝 #{0}";
			L.CsbHelp = "衣裝狀態綁定 (CSB)：用衣裝類別的穿著狀態驅動 Layer 的 weight。\n\n選擇目標 Layer 與衣裝類別，再為各狀態（On / Shift / Hang / Off）與未裝備設定 weight。衣裝狀態改變時，Layer weight 會離散跳到對應數值；脫掉衣裝則套用未裝備 weight。\n\n優先序為 Timeline > CSB > PSD：同一 Layer 上的 Timeline keyframe 會蓋過 CSB（該列顯示「Timeline 接管中」），CSB 則蓋過 PSD。";
			L.CsbStateLabels = new string[] { "On", "Shift", "Hang", "Off" };
			L.NbbTabLabel = "NBB";
			L.NbbSectionTitle = "原生 BlendShape 綁定 (NBB)";
			L.NbbCharacterOnlyHint = "NBB 驅動器僅適用於角色。";
			L.NbbNoLayerHint = "請選擇至少有一個圖層的 Renderer 才能新增驅動器。";
			L.NbbNoSourceHint = "此角色沒有任何帶原生 BlendShape 的 Renderer。";
			L.NbbAddDriver = "新增驅動器";
			L.NbbDeleteDriver = "X";
			L.NbbTimelineOverriding = "Timeline 接管中";
			L.NbbTargetLayerLabel = "目標圖層:";
			L.NbbSourceRendererLabel = "來源網格:";
			L.NbbSourceShapeLabel = "來源 BlendShape:";
			L.NbbInputRangeLabel = "輸入區間 0-100 (圖層值 0 / 圖層值 1):";
			L.NbbSetMinFromCurrent = "以目前值設為最小";
			L.NbbSetMaxFromCurrent = "以目前值設為最大";
			L.NbbCaptureHint = "先擺好表情，再以目前值擷取各端點。";
			L.NbbCurrentValueFmt = "目前值: {0}";
			L.NbbFilterLabel = "篩選:";
			L.NbbNoneSelected = "(無)";
			L.NbbDuplicateNotice = "此圖層已有驅動器。";
			L.NbbInvalidRangeNotice = "輸入區間的最小值與最大值必須不同。";
			L.NbbRowHeaderFmt = "{0} : {1}";
			L.NbbRowDetailFmt = "{0}  |  {1}  |  [{2} ~ {3}]";
			L.NbbHelp = "原生 BlendShape 綁定 (NBB): 用角色本身的 BlendShape 驅動圖層權重。\n\n選擇目標圖層，接著選來源網格與其中一個 BlendShape，並以 Unity 原生的 0-100 刻度指定輸入區間。BlendShape 數值由最小移動到最大時，圖層權重從 0 過渡到 1；把最小值設得比最大值大即為反向映射。\n\n善用擷取按鈕: 先擺出想要的表情，再以目前值擷取端點，不必自行猜數字。\n\n優先序為 Timeline > CSB > NBB > PSD: 同一圖層上有 Timeline 關鍵影格或 CSB 驅動器時 NBB 讓位，而 NBB 優先於 PSD。\n\n來源網格清單只列出帶原生 BlendShape 的網格，插件自己的圖層 frame 不會出現。當來源網格或 BlendShape 不存在時（切到沒有該件衣服的服裝、換頭等），該驅動器會被跳過，圖層維持最後一次寫入的權重。";
			L.DeleteSelectedFaces = "刪除選取面";
			L.RestoreSelectedFaces = "還原選取面";
			L.RestoreAllDeletedFaces = "還原所有已刪除面";
			L.DeletedFacesCountFmt = "已刪除：{0} 個面";
			L.FaceMaskHelp = "用筆刷或框選選取面後，按「刪除選取面」可將其從渲染中隱藏。已刪除的面在面選取模式下以紅色半透明顯示且仍可選取——選回紅色面後按「還原選取面」，或按「還原所有已刪除面」一次清除整 mask。筆刷半徑可用 [ / ] 步進調整。";
			L.SectionRenderer = "渲染器";
			L.SectionBrush = "筆刷";
			L.SectionGizmo = "Gizmo";
			L.SectionSymmetry = "對稱";
			L.SectionLayers = "圖層";
			L.SectionSubdivide = "細分";
			L.SectionPreset = "預設";
			L.SectionSettings = "設定";
			L.SectionWeightRemap = "權重重映射";
			L.SectionHelp = "說明";
			L.SectionResetConfirm = "確認重置";
			L.SectionDeformDataIO = "變形資料存取";
			L.SectionDeletedFaces = "已刪除面";
			L.SectionOperationMode = "操作模式";
			L.BrushRadiusLabel = "筆刷半徑";
			L.StrengthLabel = "強度";
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0001CBBC File Offset: 0x0001ADBC
		private static void LoadSimplifiedChinese()
		{
			L.SelectObject = "请选择对象";
			L.FilterLabel = "筛选:";
			L.TabNames = new string[] { "形状", "细分" };
			L.BrushMode = "画笔";
			L.GizmoMode = "Gizmo";
			L.MoveTool = "推拉";
			L.SmoothTool = "平滑";
			L.RelaxTool = "松弛";
			L.InflateTool = "膨胀";
			L.PinchTool = "夹捏";
			L.CreaseTool = "褶皱";
			L.Translate = "位移";
			L.Rotate = "旋转";
			L.Scale = "缩放";
			L.WorldSpace = "世界";
			L.ObjectSpace = "对象";
			L.NormalSpace = "法线";
			L.SoftSelection = "软选取";
			L.SoftModeVolume = "体积";
			L.SoftModeSurface = "表面";
			L.SoftSelectionRadius = "软选取半径";
			L.IncludeBackFaceLabel = "选取背面顶点";
			L.IncludeBackFaceTooltip = "启用（默认）时，Gizmo 顶点选取（框选与笔刷）会选到 mesh 背面（法线朝后）的顶点。关闭时只选表面（朝向相机）的顶点。雕刻的 Brush 模式不受此设定影响。";
			L.GizmoSelectBox = "框选";
			L.GizmoSelectBrush = "笔刷选取";
			L.GizmoGranularityVertex = "顶点";
			L.GizmoGranularityFace = "面";
			L.GizmoClearSelection = "清除选取";
			L.Symmetry = "对称";
			L.SymmetryAxis = "轴:";
			L.SetCenter = "设定中心";
			L.ClearCenter = "清除";
			L.SymmetryCenterFmt = "中心: {0:F3}";
			L.Layers = "图层";
			L.LayerWeight = "权重";
			L.AddLayer = "添加图层";
			L.RemoveLayer = "删除";
			L.RenameLayer = "重命名";
			L.MoveUp = "上移";
			L.MoveDown = "下移";
			L.MirrorLayer = "镜像";
			L.MirrorLayerTooltip = "将此图层沿身体左右中线镜像为一个新图层（X 轴）。";
			L.NoLayerWarning = "请先创建图层才能开始雕刻";
			L.LayerDefaultNameFmt = "图层 {0}";
			L.TargetMesh = "目标网格:";
			L.VerticesFacesFmt = "顶点: {0} | 面: {1}";
			L.Brush = "画笔";
			L.BoxSelect = "框选";
			L.BrushRadiusFmt = "画笔半径: {0}";
			L.SelectedVerticesFmt = "已选: {0} 顶点";
			L.FalloffLinear = "线性";
			L.FalloffSmooth = "平滑";
			L.FalloffSharp = "锐利";
			L.SelectFaces = "选择面";
			L.SelectedFacesFmt = "已选: {0} / {1} 面";
			L.AllButton = "全选";
			L.NoneButton = "取消";
			L.InvertButton = "反转";
			L.LevelLabel = "级别:";
			L.Subdivide = "细分";
			L.Restore = "还原";
			L.SubdivideSmooth = "平滑表面";
			L.SubdivideSmoothTooltip = "启用后，细分时会把新的边中点朝表面曲率方向外推（插值方式），在不移动原始顶点的前提下让造型更圆滑。面选择模式下只平滑被选择的面。";
			L.RebakeSubdivision = "重新烘焙";
			L.RebakeSubdivisionTooltip = "以当前姿势重新烘焙身体平滑细分的中点，修正切换到其他姿势后重新出现的线框凹凸。会保留你的雕刻。仅在选取角色身体（o_body*）且已应用平滑细分时可用。存档／重载时会以载入姿势重新烘焙。";
			L.FaceWireframeOpacity = "线框不透明度";
			L.FaceWireframeOpacityTooltip = "淡化面选择的线框。0 = 完全隐藏（仍可选择面）、1 = 默认浓度。想查看细分后的表面而不受密集网格线干扰时可调低。";
			L.FaceSelectIncludeBackFace = "纳入背面";
			L.FaceSelectIncludeBackFaceTooltip = "勾选时（默认），画笔与框选可选到法线背向相机的背面面片。取消勾选只选正面面片，想在删面前精准选取正面壳时很方便。不做遮挡判定，也无法让画笔伸到厚网格的远侧。";
			L.SubdivideLayerWarning = "细分将会重置所有图层数据，是否继续？";
			L.StrengthFmt = "强度: {0}";
			L.ApplyButton = "应用";
			L.ClearButton = "清除";
			L.ConfirmButton = "确认";
			L.CancelButton = "取消";
			L.EnterEditMode = "进入编辑模式";
			L.ExitEditMode = "退出编辑模式";
			L.CameraMode = "相机模式 (Ctrl)";
			L.BoxSelectModeName = "框选模式";
			L.EditModeActive = "编辑模式";
			L.BoxSelectInfoFmt = "强度: {0} | {1} | 已选: {2} 顶点";
			L.BrushInfoFmt = "半径: {0} | 强度: {1} | {2} | {3}";
			L.ShowMeshHighlight = "显示网格高亮";
			L.ShowMeshWireframe = "显示网格线框";
			L.ShowEditedOnly = "只显示已编辑";
			L.FocusRenderer = "聚焦相机";
			L.CatTop = "上衣";
			L.CatBottom = "下装";
			L.CatBra = "胸罩";
			L.CatUnderwear = "内裤";
			L.CatGloves = "手套";
			L.CatPantyhose = "裤袜";
			L.CatLegwear = "袜子";
			L.CatShoesInner = "室内鞋";
			L.CatShoesOuter = "室外鞋";
			L.CatHairBack = "后发";
			L.CatHairFront = "前发";
			L.CatHairSide = "侧发";
			L.CatHairOption = "选用发";
			L.CatBody = "身体";
			L.CatHead = "头部";
			L.CatClothes = "服装";
			L.CatAccessory = "饰品";
			L.CatHair = "头发";
			L.CatItem = "物件";
			L.CatAccSlot = "饰品槽";
			L.VisibilityShow = "显示";
			L.VisibilityHide = "隐藏";
			L.PressureLabel = "笔压";
			L.TabletNotDetected = "未检测到绘图板";
			L.HudLayerFmt = "图层: {0}";
			L.HudNoLayer = "(无图层)";
			L.HudShortcuts = "Ctrl+LMB:旋转  Shift:法线\nCtrl+RMB:缩放  Alt:收缩\nLMB:画笔  RMB:选取\n[ ]:半径  Shift+[ ]:力度";
			L.HudGrowShrink = "=:扩张  -:收缩";
			L.UndoLabel = "撤销";
			L.RedoLabel = "重做";
			L.FaceSelectBrush = "面选择: 画笔";
			L.FaceSelectBox = "面选择: 框选";
			L.RadiusSuffixFmt = " | 半径: {0}";
			L.RemapWeights = "重新分配权重";
			L.RestoreWeights = "还原权重";
			L.BodyMeshNotReadable = "身体网格无法读取";
			L.ExportDeform = "导出";
			L.ImportDeform = "导入";
			L.DeformFileFilter = "形状变形 (*.kksd)|*.kksd";
			L.ExportSuccess = "导出成功";
			L.ImportSuccess = "导入成功";
			L.ImportVertexMismatchFmt = "顶点数不一致: 文件 {0}, 网格 {1}";
			L.ImportInvalidFile = "无效的文件格式";
			L.PresetTabLabel = "预设";
			L.ExportPreset = "导出…";
			L.ExportPresetSelected = "导出已勾选";
			L.ImportPreset = "导入…";
			L.ApplyImportPreset = "应用导入";
			L.CancelImportPreset = "取消";
			L.PresetFileFilter = "形状预设 (*.kksp)|*.kksp";
			L.PresetExportSuccessFmt = "已导出预设：{0} 条";
			L.PresetImportSummaryFmt = "[Preset] 已导入 {0} 条，跳过：{1}";
			L.PresetInvalidFile = "预设文件无效（.kksp magic／版本不符或内容损坏）";
			L.PresetMatchOk = "OK";
			L.PresetMatchNoMatch = "无对应";
			L.PresetRowFormatFmt = "{0}  |  细分:{1}  |  图层:{2}  |  PSD:{3}  |  CSB:{4}  |  NBB:{5}";
			L.PresetRowRootName = "(根节点)";
			L.PresetEmptyOwnerHint = "请先选择角色或 Studio 物件以填入列表。";
			L.PresetNoExportableHint = "当前角色 / 物件没有可导出的细分或变形。";
			L.LiteShapeNoLayersHint = "当前角色／物件没有可编辑的图层。";
			L.PresetPreserveLayers = "保留现有 Layer";
			L.PresetPreserveLayersTooltip = "将 preset 的 Layer 追加到现有 Layer 之后，而非替换。仅在顶点数一致时生效；不改动细分等级与 face mask。";
			L.PresetPreserveScope = "保留服装范围";
			L.PresetPreserveScopeTooltip = "针对导入的 CSB driver（Replace 模式）：全局维持全局，仅限某套装的 driver 改绑到当前套装。关闭则把导入的 CSB driver 全部设为全局。";
			L.HelpTitle = "帮助";
			L.HelpBrush = "画笔模式 — 在网格上绘制来雕刻形状。\n\n准备\n- 选择目标 renderer，按下「进入编辑模式」。\n- 雕刻前必须先新增图层。\n\n工具\n- 推拉：默认沿画面方向拖动顶点。按住 Shift 改为沿法线推拉。\n- 平滑：将画笔内的顶点位置平均化（即使未雕区也削平表面起伏）。\n- 松弛：将雕刻位移（delta）向邻域平均化（未雕区不受影响）。\n- 膨胀：沿法线推出。按住 Alt 反向收缩。\n- 夹捏：沿表面把顶点拉向画笔中心，收紧细节。按住 Alt 往外推开。\n- 褶皱：夹捏再沿法线内推，刻出尖锐沟槽。按住 Alt 形成尖锐凸脊。\n\n参数\n- 半径：画笔大小。\n- 强度：每帧影响强度。\n- 衰减：线性 / 平滑 / 锐利。\n- [ / ]：调整半径; Shift+[ / Shift+]：调整力度。\n\n对称\n- 启用后选择轴向 (X/Y/Z)。\n- 于参考顶点设定中心，或保持为 0。\n\n图层\n- 多段变形可堆叠，各自有独立权重。\n- 用「上移 / 下移」调整顺序；最终位移 = Σ(图层 delta × 权重)。\n\n重新分配权重（衣服专用）\n- 大幅变形后按「重新分配权重」，可让 skinning 跟随最近的身体骨骼。\n- 「还原权重」可恢复原始骨骼权重。\n\n导出 / 导入 (.kksd)\n- 任何时候都可导出当前 renderer 的图层。\n- 导入会合并图层到当前 renderer（仅编辑模式可用）。\n\n快捷键\n- {0}\n- 按住 Ctrl：输入交给相机控制。";
			L.HelpGizmo = "Gizmo 模式 — 以手柄变换已选取的顶点。\n\n选取方式（框选 / 笔刷）\n- 框选：在空白处左键拖曳出屏幕矩形来选取。\n- 笔刷：左键拖曳沿表面涂抹累积选取（光标显示笔刷圈）。\n- 清除选取：清空当前选取（笔刷默认只加选，此为归零路径）。\n\n鼠标操作（左键）\n- 框选方式：在空白处拖曳框选；Shift 加选、Alt 减选；放开左键时套用选取。\n- 笔刷方式：沿表面拖曳逐帧加选；Alt + 拖曳擦除。默认加选（多笔堆叠）。\n- 左键拖曳手柄（轴 / 平面 / 中心方块 / ViewRotate 环）：位移 / 旋转 / 缩放。\n- 选取手势或手柄拖曳一旦启动，会锁到放开左键为止。\n- 按住 Ctrl：输入交给相机控制；不启动新的选取或手柄拖曳。\n- 右键在 Gizmo 模式下未使用。\n\n手柄\n- 位移：3 轴 + XY/XZ/YZ 平面 + 中心方块 (Free)。\n- 旋转：3 轴环 + 外围白环 (ViewRotate，面向相机)。\n- 缩放：3 轴 + 中心方块（均匀缩放）。\n\n坐标空间\n- World：固定 XYZ。\n- Object：对齐角色 / 物件根节点旋转。\n- Normal：对齐选取顶点的平均法线。\n\n软选取\n- 将影响范围扩展到硬选取顶点之外。\n- 半径：影响距离。\n- [ / ]：框选方式步进此软选取半径（软选取启用与否皆可用）；笔刷方式改步进笔刷选取半径。\n- 体积：空间中的直线距离。\n- 表面：沿 mesh 边做 BFS（不穿透薄 mesh）。\n- 半径 / 模式变更节流 150 ms 后才重算。\n\n对称\n- 启用并选择轴向，移动 / 旋转 / 缩放自动镜射。\n\n扩张 / 收缩选取\n- =：沿 mesh 拓扑把选取往外扩张一环（点模式与面模式共用）。\n- -：把选取往内收缩一环；每按一次一环（无长按自动重复）。\n- 光标悬在插件窗口上时仍可使用。\n\n快捷键\n- {0}\n- 按住 Ctrl：输入交给相机控制。";
			L.HelpSubdivide = "细分 — 提高 mesh 密度以进行更精细的雕刻。\n\n面选取\n- 使用面选取 overlay 选出要细分的三角形。\n- 提供「全选 / 取消 / 反转」辅助按钮。\n\n控制\n- 级别：细分深度（1 = 4 倍、2 = 16 倍三角形）。\n- 细分：对已选取的面应用细分。\n- 还原：恢复到原始 mesh 拓扑。\n\n注意事项\n- 细分会重置所有图层 delta（顶点数会改变）。\n- 细分后 Undo 历史会被清空。\n\n平台限制\n- Koikatsu (net35)：mesh 不可超过 65535 顶点，超过时细分会中止。\n- Koikatsu Sunshine：透过 UInt32 索引格式支持超过 65535 顶点。";
			L.HelpPreset = "Preset — 透过 .kksp 档在角色之间保存与重用变形。\n\n导出\n- 勾选要导出的 renderer，再导出成 .kksp 档。\n- 只列出有细分或有图层的 renderer。\n\n导入\n- 导入 .kksp 进入预览，勾选要应用的项目后按 Apply。\n- 同一素材装在两个槽位（例如鞋子）会自动套到两者。\n\nReplace 与 Preserve\n- Replace（默认）：重建细分、替换图层与 face mask。\n- Preserve：只叠加图层，拓扑须一致，不改动细分与 face mask。\n\nDriver（PSD / CSB）\n- PSD / CSB driver 会随 preset 一起导出，且仅在 Replace 模式还原。\n- Preserve 模式会忽略随附的 driver（叠加时图层 GUID 会被重新指派）。\n- 保留服装范围：让仅限某套装的 CSB driver 绑定当前套装（关闭则全部改为全局）。";
			L.CorruptedEntryPrefix = "[!] ";
			L.CorruptedTooltipFormat = "已保存的图层 delta 与此网格不匹配，且重建拓扑所需的细分数据缺失。";
			L.ResetCorruptedButton = "重置";
			L.ResetCorruptedConfirmTitle = "重置损坏图层";
			L.ResetCorruptedConfirmBody = "路径: {0}\n将丢弃的图层数: {1}\n\n此操作无法撤销。";
			L.CorruptedEditModeBanner = "此目标已损坏。画笔与 Gizmo 编辑可能产生错误结果。";
			L.SettingsTabLabel = "设置";
			L.CarryoverToggleLabel = "切换角色时延续身体 / 脸部变形";
			L.CarryoverTooltipText = "在 Studio 中启用此选项后，切换角色时会保留前角色的身体 / 脸部变形图层；仅当该 path 的拓扑（顶点数 + 细分层级 + 三角形 hash）与新角色完全一致才延续，否则使用新卡片数据。";
			L.CarryoverSkippedPathsFormat = "切换角色变形延续：以下 path 因拓扑不一致已跳过：{0}";
			L.CarryoverSettingsHint = "未来会在此页面新增更多偏好设置。";
			L.PsdTabLabel = "PSD";
			L.PsdSectionTitle = "姿势驱动 (PSD)";
			L.PsdCharacterOnlyHint = "PSD 驱动仅适用于角色。";
			L.PsdNoLayerHint = "请选择至少有一个 Layer 的 renderer 才能新增驱动。";
			L.PsdAddDriver = "新增驱动";
			L.PsdDeleteDriver = "X";
			L.PsdTimelineOverriding = "Timeline 接管中";
			L.PsdTargetLayerLabel = "目标 Layer：";
			L.PsdSourceBoneLabel = "来源骨：";
			L.PsdChannelLabel = "通道：";
			L.PsdInputRangeLabel = "输入范围（Layer 值 0 / Layer 值 1）：";
			L.PsdSetMinFromPose = "最小＝当前姿势";
			L.PsdSetMaxFromPose = "最大＝当前姿势";
			L.PsdCaptureHint = "先摆好骨头姿势，再从当前姿势捕获各端点。";
			L.PsdFilterLabel = "筛选：";
			L.PsdNoneSelected = "（无）";
			L.PsdDuplicateNotice = "此 Layer 已有驱动。";
			L.PsdInvalidRangeNotice = "输入最小值与最大值必须不同。";
			L.PsdRowHeaderFmt = "{0} : {1}";
			L.PsdRowDetailFmt = "{0}  |  {1}  |  [{2} ~ {3}]";
			L.PsdHelp = "姿势驱动 (PSD)：由骨头姿势驱动某个 Layer 的 weight。\n\n选择目标 Layer、来源骨、通道（旋转或位置轴）与输入范围。当骨值从最小变到最大时，weight 在 0 到 1 之间线性混合。\n\n若同一 Layer 已在 Timeline 下 keyframe，PSD 会让位给 Timeline（该行显示「Timeline 接管中」）；删除 keyframe 后 PSD 自动恢复。\n\n注：此为单骨 driver，非真正的 RBF pose-space 变形。";
			L.PsdChannelNames = new string[] { "Rot X", "Rot Y", "Rot Z", "Pos X", "Pos Y", "Pos Z" };
			L.CsbTabLabel = "CSB";
			L.CsbSectionTitle = "服装状态绑定 (CSB)";
			L.CsbCharacterOnlyHint = "CSB driver 仅适用于角色。";
			L.CsbNoLayerHint = "请选择至少有一个 Layer 的 renderer 才能新增 driver。";
			L.CsbAddDriver = "新增 Driver";
			L.CsbDeleteDriver = "X";
			L.CsbTimelineOverriding = "Timeline 接管中";
			L.CsbTargetLayerLabel = "目标 Layer：";
			L.CsbClothingKindLabel = "服装：";
			L.CsbUnequippedLabel = "未装备：";
			L.CsbCaptureFromState = "从当前状态获取";
			L.CsbCaptureHint = "将当前服装状态的 weight 设为目标 Layer 当前的 weight。";
			L.CsbDuplicateNotice = "此 Layer 已有一个 driver。";
			L.CsbNoneSelected = "（无）";
			L.CsbRowHeaderFmt = "{0} → {1}";
			L.CsbRowDetailFmt = "[{0} / {1} / {2} / {3}]  未装备：{4}";
			L.CsbScopeToggle = "仅限当前服装";
			L.CsbScopeHint = "开启＝只在穿着当前服装时生效；关闭（默认）＝所有装备此服装的服装都生效。";
			L.CsbScopeRowFmt = "  · 仅服装 #{0}";
			L.CsbHelp = "服装状态绑定 (CSB)：用服装类别的穿着状态驱动 Layer 的 weight。\n\n选择目标 Layer 与服装类别，再为各状态（On / Shift / Hang / Off）与未装备设定 weight。服装状态改变时，Layer weight 会离散跳到对应数值；脱掉服装则套用未装备 weight。\n\n优先级为 Timeline > CSB > PSD：同一 Layer 上的 Timeline keyframe 会盖过 CSB（该行显示“Timeline 接管中”），CSB 则盖过 PSD。";
			L.CsbStateLabels = new string[] { "On", "Shift", "Hang", "Off" };
			L.NbbTabLabel = "NBB";
			L.NbbSectionTitle = "原生 BlendShape 绑定 (NBB)";
			L.NbbCharacterOnlyHint = "NBB 驱动器仅适用于角色。";
			L.NbbNoLayerHint = "请选择至少有一个图层的 Renderer 才能新增驱动器。";
			L.NbbNoSourceHint = "此角色没有任何带原生 BlendShape 的 Renderer。";
			L.NbbAddDriver = "新增驱动器";
			L.NbbDeleteDriver = "X";
			L.NbbTimelineOverriding = "Timeline 接管中";
			L.NbbTargetLayerLabel = "目标图层:";
			L.NbbSourceRendererLabel = "来源网格:";
			L.NbbSourceShapeLabel = "来源 BlendShape:";
			L.NbbInputRangeLabel = "输入区间 0-100 (图层值 0 / 图层值 1):";
			L.NbbSetMinFromCurrent = "以当前值设为最小";
			L.NbbSetMaxFromCurrent = "以当前值设为最大";
			L.NbbCaptureHint = "先摆好表情，再以当前值撷取各端点。";
			L.NbbCurrentValueFmt = "当前值: {0}";
			L.NbbFilterLabel = "筛选:";
			L.NbbNoneSelected = "(无)";
			L.NbbDuplicateNotice = "此图层已有驱动器。";
			L.NbbInvalidRangeNotice = "输入区间的最小值与最大值必须不同。";
			L.NbbRowHeaderFmt = "{0} : {1}";
			L.NbbRowDetailFmt = "{0}  |  {1}  |  [{2} ~ {3}]";
			L.NbbHelp = "原生 BlendShape 绑定 (NBB): 用角色本身的 BlendShape 驱动图层权重。\n\n选择目标图层，接着选来源网格与其中一个 BlendShape，并以 Unity 原生的 0-100 刻度指定输入区间。BlendShape 数值由最小移动到最大时，图层权重从 0 过渡到 1；把最小值设得比最大值大即为反向映射。\n\n善用撷取按钮: 先摆出想要的表情，再以当前值撷取端点，不必自行猜数字。\n\n优先级为 Timeline > CSB > NBB > PSD: 同一图层上有 Timeline 关键帧或 CSB 驱动器时 NBB 让位，而 NBB 优先于 PSD。\n\n来源网格列表只列出带原生 BlendShape 的网格，插件自己的图层 frame 不会出现。当来源网格或 BlendShape 不存在时（切到没有该件衣服的服装、换头等），该驱动器会被跳过，图层维持最后一次写入的权重。";
			L.DeleteSelectedFaces = "删除选中面";
			L.RestoreSelectedFaces = "还原选中面";
			L.RestoreAllDeletedFaces = "还原所有已删除面";
			L.DeletedFacesCountFmt = "已删除：{0} 个面";
			L.FaceMaskHelp = "用笔刷或框选选中面后，按「删除选中面」可将其从渲染中隐藏。已删除的面在面选取模式下以红色半透明显示且仍可选中——选回红色面后按「还原选中面」，或按「还原所有已删除面」一次清空整个 mask。画笔半径可用 [ / ] 步进调整。";
			L.SectionRenderer = "渲染器";
			L.SectionBrush = "笔刷";
			L.SectionGizmo = "Gizmo";
			L.SectionSymmetry = "对称";
			L.SectionLayers = "图层";
			L.SectionSubdivide = "细分";
			L.SectionPreset = "预设";
			L.SectionSettings = "设置";
			L.SectionWeightRemap = "权重重映射";
			L.SectionHelp = "说明";
			L.SectionResetConfirm = "确认重置";
			L.SectionDeformDataIO = "变形数据存取";
			L.SectionDeletedFaces = "已删除面";
			L.SectionOperationMode = "操作模式";
			L.BrushRadiusLabel = "笔刷半径";
			L.StrengthLabel = "强度";
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0001D610 File Offset: 0x0001B810
		private static void LoadKorean()
		{
			L.SelectObject = "오브젝트를 선택하세요";
			L.FilterLabel = "필터:";
			L.TabNames = new string[] { "셰이프", "서브디바이드" };
			L.BrushMode = "브러시";
			L.GizmoMode = "기즈모";
			L.MoveTool = "이동";
			L.SmoothTool = "스무스";
			L.RelaxTool = "릴랙스";
			L.InflateTool = "팔창";
			L.PinchTool = "핀치";
			L.CreaseTool = "크리스";
			L.Translate = "이동";
			L.Rotate = "회전";
			L.Scale = "스케일";
			L.WorldSpace = "월드";
			L.ObjectSpace = "오브젝트";
			L.NormalSpace = "법선";
			L.SoftSelection = "소프트 선택";
			L.SoftModeVolume = "볼륨";
			L.SoftModeSurface = "서피스";
			L.SoftSelectionRadius = "소프트 선택 반경";
			L.IncludeBackFaceLabel = "뒷면 버텍스도 선택";
			L.IncludeBackFaceTooltip = "활성화(기본값) 시 기즈모 버텍스 선택(박스/브러시)은 메시 뒷면(법선이 카메라 반대 방향) 버텍스도 포함합니다. 비활성화 시에는 앞면(카메라 방향) 버텍스만 선택됩니다. 스컬프트 브러시 모드는 이 설정의 영향을 받지 않습니다.";
			L.GizmoSelectBox = "박스 선택";
			L.GizmoSelectBrush = "브러시 선택";
			L.GizmoGranularityVertex = "정점";
			L.GizmoGranularityFace = "면";
			L.GizmoClearSelection = "선택 해제";
			L.Symmetry = "대칭";
			L.SymmetryAxis = "축:";
			L.SetCenter = "중심 설정";
			L.ClearCenter = "초기화";
			L.SymmetryCenterFmt = "중심: {0:F3}";
			L.Layers = "레이어";
			L.LayerWeight = "웨이트";
			L.AddLayer = "레이어 추가";
			L.RemoveLayer = "삭제";
			L.RenameLayer = "이름 변경";
			L.MoveUp = "위로";
			L.MoveDown = "아래로";
			L.MirrorLayer = "미러";
			L.MirrorLayerTooltip = "이 레이어를 좌우 중심으로 반전하여 새 레이어로 만듭니다 (X축).";
			L.NoLayerWarning = "레이어를 생성해야 조각을 시작할 수 있습니다";
			L.LayerDefaultNameFmt = "레이어 {0}";
			L.TargetMesh = "대상 메시:";
			L.VerticesFacesFmt = "버텍스: {0} | 면: {1}";
			L.Brush = "브러시";
			L.BoxSelect = "박스 선택";
			L.BrushRadiusFmt = "브러시 반경: {0}";
			L.SelectedVerticesFmt = "선택: {0} 버텍스";
			L.FalloffLinear = "리니어";
			L.FalloffSmooth = "스무스";
			L.FalloffSharp = "샤프";
			L.SelectFaces = "면 선택";
			L.SelectedFacesFmt = "선택: {0} / {1} 면";
			L.AllButton = "전체";
			L.NoneButton = "해제";
			L.InvertButton = "반전";
			L.LevelLabel = "레벨:";
			L.Subdivide = "서브디바이드";
			L.Restore = "복원";
			L.SubdivideSmooth = "표면 스무딩";
			L.SubdivideSmoothTooltip = "활성화하면 서브디바이드 시 새 모서리 중점을 표면 곡률 방향으로 밀어(보간 방식) 원래 정점을 움직이지 않고 형태를 부드럽게 만듭니다. 면 선택 모드에서는 선택한 면만 스무딩됩니다.";
			L.RebakeSubdivision = "리베이크";
			L.RebakeSubdivisionTooltip = "다른 포즈로 전환한 뒤 다시 나타나는 와이어프레임 요철을 수정하기 위해 현재 포즈에서 바디의 스무스 서브디바이드 중점을 다시 베이크합니다. 스컬핑은 유지됩니다. 캐릭터 바디(o_body*)이고 스무스 서브디바이드가 있을 때만 사용할 수 있습니다. 저장/재로드 시에는 로드 포즈에서 다시 베이크됩니다.";
			L.FaceWireframeOpacity = "와이어프레임 불투명도";
			L.FaceWireframeOpacityTooltip = "면 선택 와이어프레임을 흐리게 합니다. 0 = 완전히 숨김(면 선택은 계속 가능), 1 = 기본 농도. 조밀한 메시 선 없이 서브디바이드된 표면을 확인하려면 낮추세요.";
			L.FaceSelectIncludeBackFace = "뒷면 포함";
			L.FaceSelectIncludeBackFaceTooltip = "체크 시(기본값) 브러시와 박스 선택으로 법선이 카메라 반대쪽을 향하는 뒷면도 선택할 수 있습니다. 체크 해제하면 정면 면만 선택합니다. 면을 삭제하기 전에 앞쪽 셸만 정확히 선택할 때 유용합니다. 오클루전은 검사하지 않으며, 두꺼운 메시의 반대편까지 브러시가 닿지는 않습니다.";
			L.SubdivideLayerWarning = "서브디바이드하면 모든 레이어 데이터가 초기화됩니다. 계속하시겠습니까?";
			L.StrengthFmt = "강도: {0}";
			L.ApplyButton = "적용";
			L.ClearButton = "초기화";
			L.ConfirmButton = "확인";
			L.CancelButton = "취소";
			L.EnterEditMode = "편집 모드 시작";
			L.ExitEditMode = "편집 모드 종료";
			L.CameraMode = "카메라 모드 (Ctrl)";
			L.BoxSelectModeName = "박스 선택 모드";
			L.EditModeActive = "편집 모드";
			L.BoxSelectInfoFmt = "강도: {0} | {1} | 선택: {2} 버텍스";
			L.BrushInfoFmt = "반경: {0} | 강도: {1} | {2} | {3}";
			L.ShowMeshHighlight = "메시 하이라이트 표시";
			L.ShowMeshWireframe = "메시 와이어프레임 표시";
			L.ShowEditedOnly = "편집된 항목만";
			L.FocusRenderer = "카메라 포커스";
			L.CatTop = "상의";
			L.CatBottom = "하의";
			L.CatBra = "브라";
			L.CatUnderwear = "속옷";
			L.CatGloves = "장갑";
			L.CatPantyhose = "팬티스타킹";
			L.CatLegwear = "양말";
			L.CatShoesInner = "실내화";
			L.CatShoesOuter = "신발";
			L.CatHairBack = "뒷머리";
			L.CatHairFront = "앞머리";
			L.CatHairSide = "옆머리";
			L.CatHairOption = "기타머리";
			L.CatBody = "바디";
			L.CatHead = "헤드";
			L.CatClothes = "의상";
			L.CatAccessory = "액세서리";
			L.CatHair = "머리";
			L.CatItem = "아이템";
			L.CatAccSlot = "액세 슬롯";
			L.VisibilityShow = "표시";
			L.VisibilityHide = "숨김";
			L.PressureLabel = "필압";
			L.TabletNotDetected = "태블릿이 감지되지 않음";
			L.HudLayerFmt = "레이어: {0}";
			L.HudNoLayer = "(레이어 없음)";
			L.HudShortcuts = "Ctrl+LMB:회전  Shift:법선\nCtrl+RMB:줌  Alt:수축\nLMB:브러시  RMB:선택\n[ ]:반지름  Shift+[ ]:강도";
			L.HudGrowShrink = "=:확장  -:축소";
			L.UndoLabel = "실행 취소";
			L.RedoLabel = "다시 실행";
			L.FaceSelectBrush = "면 선택: 브러시";
			L.FaceSelectBox = "면 선택: 박스";
			L.RadiusSuffixFmt = " | 반경: {0}";
			L.RemapWeights = "가중치 재할당";
			L.RestoreWeights = "가중치 복원";
			L.BodyMeshNotReadable = "바디 메시를 읽을 수 없습니다";
			L.ExportDeform = "내보내기";
			L.ImportDeform = "가져오기";
			L.DeformFileFilter = "셰이프 변형 (*.kksd)|*.kksd";
			L.ExportSuccess = "내보내기 성공";
			L.ImportSuccess = "가져오기 성공";
			L.ImportVertexMismatchFmt = "버텍스 수 불일치: 파일 {0}, 메시 {1}";
			L.ImportInvalidFile = "잘못된 파일 형식";
			L.PresetTabLabel = "프리셋";
			L.ExportPreset = "내보내기…";
			L.ExportPresetSelected = "선택 내보내기";
			L.ImportPreset = "가져오기…";
			L.ApplyImportPreset = "가져오기 적용";
			L.CancelImportPreset = "취소";
			L.PresetFileFilter = "셰이프 프리셋 (*.kksp)|*.kksp";
			L.PresetExportSuccessFmt = "프리셋을 내보냈습니다: {0} 항목";
			L.PresetImportSummaryFmt = "[Preset] 가져오기 {0} 항목, 건너뛴 경로: {1}";
			L.PresetInvalidFile = "프리셋 파일이 잘못되었습니다 (.kksp 매직 / 버전 불일치 또는 손상)";
			L.PresetMatchOk = "OK";
			L.PresetMatchNoMatch = "대응 없음";
			L.PresetRowFormatFmt = "{0}  |  세분:{1}  |  레이어:{2}  |  PSD:{3}  |  CSB:{4}  |  NBB:{5}";
			L.PresetRowRootName = "(루트)";
			L.PresetEmptyOwnerHint = "캐릭터 또는 Studio 아이템을 선택해 목록을 채우세요.";
			L.PresetNoExportableHint = "내보낼 수 있는 데이터가 없습니다: 현재 대상에 서브디비전이나 레이어가 없습니다.";
			L.LiteShapeNoLayersHint = "현재 캐릭터/아이템에 편집 가능한 레이어가 없습니다.";
			L.PresetPreserveLayers = "기존 레이어 유지";
			L.PresetPreserveLayersTooltip = "프리셋의 레이어를 교체하지 않고 기존 레이어 뒤에 추가합니다. 정점 수가 일치할 때만 적용되며 서브디비전 레벨과 페이스 마스크는 변경되지 않습니다.";
			L.PresetPreserveScope = "의상 범위 유지";
			L.PresetPreserveScopeTooltip = "가져온 CSB 드라이버(교체 모드)용: 전역은 전역으로 유지하고 의상 전용 드라이버는 현재 의상에 다시 바인딩합니다. 끄면 가져온 CSB 드라이버를 모두 전역으로 만듭니다.";
			L.HelpTitle = "도움말";
			L.HelpBrush = "브러시 모드 — 메시 위에 페인팅하여 조각합니다.\n\n준비\n- 렌더러를 선택하고 「편집 모드 시작」을 누르세요.\n- 조각 전에 레이어를 먼저 추가해야 합니다.\n\n도구\n- 이동: 기본은 화면 방향으로 버텍스 드래그. Shift로 법선 방향으로 변경.\n- 스무스: 브러시 내 버텍스 위치를 평균화(편집되지 않은 면도 표면 요철을 평탄화).\n- 릴랙스: 변형 델타를 이웃으로 평균화(편집되지 않은 면은 영향 없음).\n- 팽창: 법선 방향으로 밀어냄. Alt로 수축.\n- 핀치: 버텍스를 표면을 따라 브러시 중심으로 끌어당겨 디테일을 조임. Alt로 바깥쪽으로.\n- 크리스: 핀치에 법선 방향 안쪽 밀기를 더해 날카로운 홈을 새김. Alt로 날카로운 능선.\n\n파라미터\n- 반경: 브러시 크기.\n- 강도: 프레임당 영향도.\n- 감쇠: 리니어 / 스무스 / 샤프.\n- [ / ]: 반지름 조절; Shift+[ / Shift+]: 강도 조절.\n\n대칭\n- 활성화 후 축(X/Y/Z)을 선택.\n- 기준 버텍스에서 중심을 설정하거나 0으로 유지.\n\n레이어\n- 여러 변형을 쌓고 각자 웨이트를 가집니다.\n- 위/아래로 순서 변경. 최종 위치 = Σ(레이어 델타 × 웨이트).\n\n가중치 재할당(의상 전용)\n- 큰 변형 후 가중치 재할당을 누르면 스키닝이 가장 가까운 바디 본을 따라갑니다.\n- 가중치 복원으로 원래 본 가중치로 되돌립니다.\n\n내보내기 / 가져오기 (.kksd)\n- 언제든 현재 렌더러의 레이어를 내보낼 수 있습니다.\n- 가져오기는 현재 렌더러에 레이어를 병합합니다(편집 모드에서만).\n\n단축키\n- {0}\n- Ctrl을 누르고 있으면 입력이 카메라 제어로 전달됩니다.";
			L.HelpGizmo = "기즈모 모드 — 선택된 버텍스를 핸들로 변환합니다.\n\n선택 방식(박스 / 브러시)\n- 박스: 빈 공간을 좌클릭 드래그하여 화면 사각형으로 선택합니다.\n- 브러시: 좌클릭 드래그로 표면을 따라 선택을 덧칠합니다(커서에 브러시 원 표시).\n- 선택 해제: 현재 선택을 비웁니다(브러시는 기본적으로 추가만 하므로 초기화용).\n\n마우스 조작(왼쪽 버튼)\n- 박스 방식: 빈 공간을 드래그하여 박스 선택. Shift 추가, Alt 제거. 버튼을 놓을 때 선택이 적용됩니다.\n- 브러시 방식: 표면을 드래그하여 매 프레임 버텍스 추가. Alt + 드래그로 지우기. 기본 추가(스트로크가 누적).\n- 핸들(축 / 평면 / 중심 큐브 / ViewRotate 링) 좌클릭 드래그: 이동 / 회전 / 크기 조정.\n- 선택 제스처 또는 핸들 드래그는 시작되면 마우스 버튼을 놓을 때까지 잠깁니다.\n- Ctrl 누름: 입력이 카메라 제어로 전달됩니다. 새 선택 / 핸들 드래그는 시작되지 않습니다.\n- 오른쪽 마우스 버튼은 기즈모 모드에서 사용되지 않습니다.\n\n핸들\n- 이동: 3축 + XY/XZ/YZ 평면 + 중앙 큐브 (Free).\n- 회전: 3축 링 + 외곽 흰색 링 (ViewRotate, 카메라를 향함).\n- 스케일: 3축 + 중앙 큐브(균일 스케일).\n\n좌표 공간\n- World: 고정 XYZ.\n- Object: 캐릭터/아이템 루트 회전에 정렬.\n- Normal: 선택 버텍스의 평균 법선에 정렬.\n\n소프트 선택\n- 하드 선택된 버텍스 바깥까지 영향 범위를 확장합니다.\n- 반경: 영향 거리.\n- [ / ]: 박스 방식은 이 소프트 선택 반경을 단계 조정(소프트 선택 활성화 여부와 무관); 브러시 방식은 브러시 선택 반경을 조정합니다.\n- 볼륨: 공간을 관통하는 직선 거리.\n- 서피스: 메시 에지를 따라 BFS(얇은 메시를 관통하지 않음).\n- 반경/모드 변경은 150 ms 스로틀링 후 재계산됩니다.\n\n대칭\n- 활성화하고 축을 선택. 이동/회전/스케일이 자동으로 미러링됩니다.\n\n선택 확장/축소\n- =: 선택을 토폴로지상 한 링 바깥으로 확장(버텍스/페이스 모드 공통).\n- -: 선택을 한 링 안쪽으로 축소; 한 번 누를 때마다 한 링(자동 반복 없음).\n- 커서가 플러그인 창 위에 있어도 작동합니다.\n\n단축키\n- {0}\n- Ctrl을 누르고 있으면 입력이 카메라 제어로 전달됩니다.";
			L.HelpSubdivide = "서브디바이드 — 메시 밀도를 높여 더 정밀한 조각을 가능하게 합니다.\n\n면 선택\n- 면 선택 오버레이로 서브디바이드할 삼각형을 고릅니다.\n- 전체 / 해제 / 반전 보조 기능이 제공됩니다.\n\n제어\n- 레벨: 서브디비전 깊이(1 = 4배, 2 = 16배 삼각형).\n- 서브디바이드: 선택된 면에 적용.\n- 복원: 메시를 원래 토폴로지로 되돌립니다.\n\n주의\n- 서브디바이드는 모든 레이어 델타 데이터를 재설정합니다(버텍스 수가 변경되므로).\n- 서브디바이드 후 실행 취소 이력은 지워집니다.\n\n플랫폼 제한\n- Koikatsu (net35): 메시는 65535 버텍스를 초과할 수 없으며 — 초과 시 서브디바이드가 중단됩니다.\n- Koikatsu Sunshine: UInt32 인덱스 형식으로 65535 초과 버텍스를 지원합니다.";
			L.HelpPreset = "프리셋 — .kksp 파일로 변형을 캐릭터 간에 저장하고 재사용합니다.\n\n내보내기\n- 원하는 렌더러를 체크한 뒤 .kksp 파일로 내보냅니다.\n- 서브디비전이나 레이어가 있는 렌더러만 표시됩니다.\n\n가져오기\n- .kksp를 가져오면 미리보기로 들어가며, 적용할 항목을 체크하고 Apply를 누릅니다.\n- 같은 에셋이 두 슬롯(예: 신발)에 있으면 자동으로 둘 다에 적용됩니다.\n\n교체 vs 유지\n- 교체(기본): 서브디비전을 재구성하고 레이어와 페이스 마스크를 교체합니다.\n- 유지: 레이어만 추가하며, 토폴로지가 일치해야 하고 서브디비전과 페이스 마스크는 변경하지 않습니다.\n\n드라이버 (PSD / CSB)\n- PSD / CSB 드라이버는 프리셋과 함께 이동하며 교체 모드에서만 복원됩니다.\n- 유지 모드는 포함된 드라이버를 무시합니다(추가 시 레이어 GUID가 재할당됨).\n- 의상 범위 유지: 의상 전용 CSB 드라이버를 현재 의상에 바인딩된 상태로 유지합니다(끄면 전역으로 전환).";
			L.CorruptedEntryPrefix = "[!] ";
			L.CorruptedTooltipFormat = "저장된 레이어 델타가 이 메시와 일치하지 않으며, 토폴로지를 재구성하는 데 필요한 서브디바이드 데이터가 없습니다.";
			L.ResetCorruptedButton = "재설정";
			L.ResetCorruptedConfirmTitle = "손상된 레이어 재설정";
			L.ResetCorruptedConfirmBody = "경로: {0}\n폐기할 레이어 수: {1}\n\n이 작업은 되돌릴 수 없습니다.";
			L.CorruptedEditModeBanner = "대상이 손상되었습니다. 브러시/기즈모 편집이 잘못된 결과를 낼 수 있습니다.";
			L.SettingsTabLabel = "설정";
			L.CarryoverToggleLabel = "캐릭터 교체 시 바디/머리 변형 유지";
			L.CarryoverTooltipText = "Studio에서 이 옵션을 활성화하면 캐릭터 교체 시 토폴로지(버텍스 수 + 서브디비전 레벨 + 삼각형 해시)가 새 캐릭터와 정확히 일치하는 경로의 이전 캐릭터 바디/머리 변형 레이어가 유지됩니다. 일치하지 않는 경로는 새 카드의 데이터가 사용됩니다.";
			L.CarryoverSkippedPathsFormat = "캐릭터 교체 변형 유지: 토폴로지 불일치로 건너뛴 경로: {0}";
			L.CarryoverSettingsHint = "향후 업데이트에서 더 많은 설정 항목이 추가될 예정입니다.";
			L.PsdTabLabel = "PSD";
			L.PsdSectionTitle = "포즈 구동 (PSD)";
			L.PsdCharacterOnlyHint = "PSD 드라이버는 캐릭터에서만 사용할 수 있습니다.";
			L.PsdNoLayerHint = "드라이버를 추가하려면 레이어가 하나 이상 있는 렌더러를 선택하세요.";
			L.PsdAddDriver = "드라이버 추가";
			L.PsdDeleteDriver = "X";
			L.PsdTimelineOverriding = "Timeline 제어 중";
			L.PsdTargetLayerLabel = "대상 레이어:";
			L.PsdSourceBoneLabel = "소스 본:";
			L.PsdChannelLabel = "채널:";
			L.PsdInputRangeLabel = "입력 범위 (Layer 값 0 / Layer 값 1):";
			L.PsdSetMinFromPose = "최소=현재 포즈";
			L.PsdSetMaxFromPose = "최대=현재 포즈";
			L.PsdCaptureHint = "본을 포즈한 뒤 현재 포즈에서 각 끝점을 캡처합니다.";
			L.PsdFilterLabel = "필터:";
			L.PsdNoneSelected = "(없음)";
			L.PsdDuplicateNotice = "이 레이어에는 이미 드라이버가 있습니다.";
			L.PsdInvalidRangeNotice = "입력 최소값과 최대값은 달라야 합니다.";
			L.PsdRowHeaderFmt = "{0} : {1}";
			L.PsdRowDetailFmt = "{0}  |  {1}  |  [{2} ~ {3}]";
			L.PsdHelp = "포즈 구동 (PSD): 본의 포즈로 레이어 가중치를 구동합니다.\n\n대상 레이어, 소스 본, 채널(회전 또는 위치 축), 입력 범위를 선택하세요. 본 값이 최소에서 최대로 이동하면 가중치가 0에서 1로 보간됩니다.\n\n같은 레이어에 Timeline 키프레임이 있으면 PSD는 Timeline에 양보합니다('Timeline 제어 중' 표시). 키프레임을 제거하면 PSD가 재개됩니다.\n\n참고: 단일 본 드라이버이며 진정한 RBF 포즈 공간 변형이 아닙니다.";
			L.PsdChannelNames = new string[] { "Rot X", "Rot Y", "Rot Z", "Pos X", "Pos Y", "Pos Z" };
			L.CsbTabLabel = "CSB";
			L.CsbSectionTitle = "의상 상태 바인딩 (CSB)";
			L.CsbCharacterOnlyHint = "CSB 드라이버는 캐릭터에만 사용할 수 있습니다.";
			L.CsbNoLayerHint = "드라이버를 추가하려면 레이어가 하나 이상 있는 렌더러를 선택하세요.";
			L.CsbAddDriver = "드라이버 추가";
			L.CsbDeleteDriver = "X";
			L.CsbTimelineOverriding = "Timeline 제어 중";
			L.CsbTargetLayerLabel = "대상 레이어:";
			L.CsbClothingKindLabel = "의상:";
			L.CsbUnequippedLabel = "미착용:";
			L.CsbCaptureFromState = "현재 상태에서 가져오기";
			L.CsbCaptureHint = "현재 의상 상태의 가중치를 대상 레이어의 현재 가중치로 설정합니다.";
			L.CsbDuplicateNotice = "이 레이어에는 이미 드라이버가 있습니다.";
			L.CsbNoneSelected = "(없음)";
			L.CsbRowHeaderFmt = "{0} → {1}";
			L.CsbRowDetailFmt = "[{0} / {1} / {2} / {3}]  미착용: {4}";
			L.CsbScopeToggle = "이 의상만";
			L.CsbScopeHint = "켜짐 = 현재 의상을 착용 중일 때만 적용; 꺼짐(기본) = 이 의류를 착용한 모든 코디에 적용.";
			L.CsbScopeRowFmt = "  · 의상 #{0} 만";
			L.CsbHelp = "의상 상태 바인딩 (CSB): 의상 카테고리의 착용 상태로 레이어 가중치를 구동합니다.\n\n대상 레이어와 의상 카테고리를 선택한 뒤 각 상태(On / Shift / Hang / Off)와 미착용 가중치를 설정합니다. 의상 상태가 바뀌면 레이어 가중치가 해당 값으로 불연속적으로 전환되고, 의상을 벗으면 미착용 가중치가 적용됩니다.\n\n우선순위는 Timeline > CSB > PSD입니다. 같은 레이어에 Timeline 키프레임이 있으면 CSB보다 우선하며(행에 'Timeline 제어 중' 표시), CSB는 PSD보다 우선합니다.";
			L.CsbStateLabels = new string[] { "On", "Shift", "Hang", "Off" };
			L.NbbTabLabel = "NBB";
			L.NbbSectionTitle = "네이티브 블렌드셰이프 바인딩 (NBB)";
			L.NbbCharacterOnlyHint = "NBB 드라이버는 캐릭터에서만 사용할 수 있습니다.";
			L.NbbNoLayerHint = "드라이버를 추가하려면 레이어가 하나 이상 있는 렌더러를 선택하세요.";
			L.NbbNoSourceHint = "이 캐릭터에는 네이티브 블렌드셰이프를 가진 렌더러가 없습니다.";
			L.NbbAddDriver = "드라이버 추가";
			L.NbbDeleteDriver = "X";
			L.NbbTimelineOverriding = "Timeline 제어 중";
			L.NbbTargetLayerLabel = "대상 레이어:";
			L.NbbSourceRendererLabel = "소스 메시:";
			L.NbbSourceShapeLabel = "소스 블렌드셰이프:";
			L.NbbInputRangeLabel = "입력 범위 0-100 (레이어 값 0 / 레이어 값 1):";
			L.NbbSetMinFromCurrent = "현재 값으로 최소 설정";
			L.NbbSetMaxFromCurrent = "현재 값으로 최대 설정";
			L.NbbCaptureHint = "원하는 표정을 만든 뒤 현재 값으로 각 끝점을 가져옵니다.";
			L.NbbCurrentValueFmt = "현재 값: {0}";
			L.NbbFilterLabel = "필터:";
			L.NbbNoneSelected = "(없음)";
			L.NbbDuplicateNotice = "이 레이어에는 이미 드라이버가 있습니다.";
			L.NbbInvalidRangeNotice = "입력 최솟값과 최댓값은 달라야 합니다.";
			L.NbbRowHeaderFmt = "{0} : {1}";
			L.NbbRowDetailFmt = "{0}  |  {1}  |  [{2} ~ {3}]";
			L.NbbHelp = "네이티브 블렌드셰이프 바인딩 (NBB): 캐릭터 고유의 블렌드셰이프로 레이어 가중치를 구동합니다.\n\n대상 레이어를 고른 다음 소스 메시와 그 블렌드셰이프, 그리고 Unity 기본 0-100 스케일의 입력 범위를 지정합니다. 블렌드셰이프 값이 최소에서 최대로 움직이면 레이어 가중치가 0에서 1로 변합니다. 최솟값을 최댓값보다 크게 하면 반전됩니다.\n\n캡처 버튼 활용: 원하는 표정을 만든 뒤 현재 값으로 끝점을 가져오면 숫자를 짐작할 필요가 없습니다.\n\n우선순위는 Timeline > CSB > NBB > PSD 입니다. 같은 레이어에 Timeline 키프레임이나 CSB 드라이버가 있으면 NBB가 양보하고, NBB는 PSD보다 우선합니다.\n\n소스 메시 목록에는 네이티브 블렌드셰이프를 가진 메시만 표시되며 플러그인 자체 레이어 프레임은 선택할 수 없습니다. 소스 메시나 셰이프를 찾을 수 없으면(해당 의상이 없는 코디, 머리 교체 등) 드라이버는 건너뛰고 레이어는 마지막으로 기록된 가중치를 유지합니다.";
			L.DeleteSelectedFaces = "선택 페이스 삭제";
			L.RestoreSelectedFaces = "선택 페이스 복원";
			L.RestoreAllDeletedFaces = "모든 삭제 페이스 복원";
			L.DeletedFacesCountFmt = "삭제됨: {0} 페이스";
			L.FaceMaskHelp = "브러시 또는 박스 선택으로 페이스를 고른 뒤 「선택 페이스 삭제」를 누르면 렌더링에서 숨겨집니다. 삭제된 페이스는 페이스 선택 모드에서 반투명 빨간색으로 표시되며 여전히 선택 가능합니다. 빨간 페이스를 선택한 뒤 「선택 페이스 복원」 또는 「모든 삭제 페이스 복원」으로 되돌릴 수 있습니다. 브러시 반경은 [ / ]로 단계 조정할 수 있습니다.";
			L.SectionRenderer = "렌더러";
			L.SectionBrush = "브러시";
			L.SectionGizmo = "기즈모";
			L.SectionSymmetry = "대칭";
			L.SectionLayers = "레이어";
			L.SectionSubdivide = "세분화";
			L.SectionPreset = "프리셋";
			L.SectionSettings = "설정";
			L.SectionWeightRemap = "가중치 재매핑";
			L.SectionHelp = "도움말";
			L.SectionResetConfirm = "초기화 확인";
			L.SectionDeformDataIO = "변형 데이터 입출력";
			L.SectionDeletedFaces = "삭제된 면";
			L.SectionOperationMode = "조작 모드";
			L.BrushRadiusLabel = "브러시 반경";
			L.StrengthLabel = "강도";
		}

		// Token: 0x040001F7 RID: 503
		private static L.Language _current = L.Language.English;

		// Token: 0x040001F8 RID: 504
		public static string SelectObject;

		// Token: 0x040001F9 RID: 505
		public static string FilterLabel;

		// Token: 0x040001FA RID: 506
		public static string[] TabNames;

		// Token: 0x040001FB RID: 507
		public static string BrushMode;

		// Token: 0x040001FC RID: 508
		public static string GizmoMode;

		// Token: 0x040001FD RID: 509
		public static string MoveTool;

		// Token: 0x040001FE RID: 510
		public static string SmoothTool;

		// Token: 0x040001FF RID: 511
		public static string RelaxTool;

		// Token: 0x04000200 RID: 512
		public static string InflateTool;

		// Token: 0x04000201 RID: 513
		public static string PinchTool;

		// Token: 0x04000202 RID: 514
		public static string CreaseTool;

		// Token: 0x04000203 RID: 515
		public static string Translate;

		// Token: 0x04000204 RID: 516
		public static string Rotate;

		// Token: 0x04000205 RID: 517
		public static string Scale;

		// Token: 0x04000206 RID: 518
		public static string WorldSpace;

		// Token: 0x04000207 RID: 519
		public static string ObjectSpace;

		// Token: 0x04000208 RID: 520
		public static string NormalSpace;

		// Token: 0x04000209 RID: 521
		public static string SoftSelection;

		// Token: 0x0400020A RID: 522
		public static string SoftModeVolume;

		// Token: 0x0400020B RID: 523
		public static string SoftModeSurface;

		// Token: 0x0400020C RID: 524
		public static string SoftSelectionRadius;

		// Token: 0x0400020D RID: 525
		public static string IncludeBackFaceLabel;

		// Token: 0x0400020E RID: 526
		public static string IncludeBackFaceTooltip;

		// Token: 0x0400020F RID: 527
		public static string GizmoSelectBox;

		// Token: 0x04000210 RID: 528
		public static string GizmoSelectBrush;

		// Token: 0x04000211 RID: 529
		public static string GizmoGranularityVertex;

		// Token: 0x04000212 RID: 530
		public static string GizmoGranularityFace;

		// Token: 0x04000213 RID: 531
		public static string GizmoClearSelection;

		// Token: 0x04000214 RID: 532
		public static string Symmetry;

		// Token: 0x04000215 RID: 533
		public static string SymmetryAxis;

		// Token: 0x04000216 RID: 534
		public static string SetCenter;

		// Token: 0x04000217 RID: 535
		public static string ClearCenter;

		// Token: 0x04000218 RID: 536
		public static string SymmetryCenterFmt;

		// Token: 0x04000219 RID: 537
		public static string Layers;

		// Token: 0x0400021A RID: 538
		public static string LayerWeight;

		// Token: 0x0400021B RID: 539
		public static string AddLayer;

		// Token: 0x0400021C RID: 540
		public static string RemoveLayer;

		// Token: 0x0400021D RID: 541
		public static string RenameLayer;

		// Token: 0x0400021E RID: 542
		public static string MoveUp;

		// Token: 0x0400021F RID: 543
		public static string MoveDown;

		// Token: 0x04000220 RID: 544
		public static string MirrorLayer;

		// Token: 0x04000221 RID: 545
		public static string MirrorLayerTooltip;

		// Token: 0x04000222 RID: 546
		public static string NoLayerWarning;

		// Token: 0x04000223 RID: 547
		public static string LayerDefaultNameFmt;

		// Token: 0x04000224 RID: 548
		public static string TargetMesh;

		// Token: 0x04000225 RID: 549
		public static string VerticesFacesFmt;

		// Token: 0x04000226 RID: 550
		public static string Brush;

		// Token: 0x04000227 RID: 551
		public static string BoxSelect;

		// Token: 0x04000228 RID: 552
		public static string BrushRadiusFmt;

		// Token: 0x04000229 RID: 553
		public static string SelectedVerticesFmt;

		// Token: 0x0400022A RID: 554
		public static string FalloffLinear;

		// Token: 0x0400022B RID: 555
		public static string FalloffSmooth;

		// Token: 0x0400022C RID: 556
		public static string FalloffSharp;

		// Token: 0x0400022D RID: 557
		public static string SelectFaces;

		// Token: 0x0400022E RID: 558
		public static string SelectedFacesFmt;

		// Token: 0x0400022F RID: 559
		public static string AllButton;

		// Token: 0x04000230 RID: 560
		public static string NoneButton;

		// Token: 0x04000231 RID: 561
		public static string InvertButton;

		// Token: 0x04000232 RID: 562
		public static string LevelLabel;

		// Token: 0x04000233 RID: 563
		public static string Subdivide;

		// Token: 0x04000234 RID: 564
		public static string Restore;

		// Token: 0x04000235 RID: 565
		public static string SubdivideSmooth;

		// Token: 0x04000236 RID: 566
		public static string SubdivideSmoothTooltip;

		// Token: 0x04000237 RID: 567
		public static string RebakeSubdivision;

		// Token: 0x04000238 RID: 568
		public static string RebakeSubdivisionTooltip;

		// Token: 0x04000239 RID: 569
		public static string FaceWireframeOpacity;

		// Token: 0x0400023A RID: 570
		public static string FaceWireframeOpacityTooltip;

		// Token: 0x0400023B RID: 571
		public static string FaceSelectIncludeBackFace;

		// Token: 0x0400023C RID: 572
		public static string FaceSelectIncludeBackFaceTooltip;

		// Token: 0x0400023D RID: 573
		public static string SubdivideLayerWarning;

		// Token: 0x0400023E RID: 574
		public static string StrengthFmt;

		// Token: 0x0400023F RID: 575
		public static string ApplyButton;

		// Token: 0x04000240 RID: 576
		public static string ClearButton;

		// Token: 0x04000241 RID: 577
		public static string ConfirmButton;

		// Token: 0x04000242 RID: 578
		public static string CancelButton;

		// Token: 0x04000243 RID: 579
		public static string EnterEditMode;

		// Token: 0x04000244 RID: 580
		public static string ExitEditMode;

		// Token: 0x04000245 RID: 581
		public static string CameraMode;

		// Token: 0x04000246 RID: 582
		public static string BoxSelectModeName;

		// Token: 0x04000247 RID: 583
		public static string EditModeActive;

		// Token: 0x04000248 RID: 584
		public static string BoxSelectInfoFmt;

		// Token: 0x04000249 RID: 585
		public static string BrushInfoFmt;

		// Token: 0x0400024A RID: 586
		public static string ShowMeshHighlight;

		// Token: 0x0400024B RID: 587
		public static string ShowMeshWireframe;

		// Token: 0x0400024C RID: 588
		public static string ShowEditedOnly;

		// Token: 0x0400024D RID: 589
		public static string FocusRenderer;

		// Token: 0x0400024E RID: 590
		public static string CatTop;

		// Token: 0x0400024F RID: 591
		public static string CatBottom;

		// Token: 0x04000250 RID: 592
		public static string CatBra;

		// Token: 0x04000251 RID: 593
		public static string CatUnderwear;

		// Token: 0x04000252 RID: 594
		public static string CatGloves;

		// Token: 0x04000253 RID: 595
		public static string CatPantyhose;

		// Token: 0x04000254 RID: 596
		public static string CatLegwear;

		// Token: 0x04000255 RID: 597
		public static string CatShoesInner;

		// Token: 0x04000256 RID: 598
		public static string CatShoesOuter;

		// Token: 0x04000257 RID: 599
		public static string CatHairBack;

		// Token: 0x04000258 RID: 600
		public static string CatHairFront;

		// Token: 0x04000259 RID: 601
		public static string CatHairSide;

		// Token: 0x0400025A RID: 602
		public static string CatHairOption;

		// Token: 0x0400025B RID: 603
		public static string CatBody;

		// Token: 0x0400025C RID: 604
		public static string CatHead;

		// Token: 0x0400025D RID: 605
		public static string CatClothes;

		// Token: 0x0400025E RID: 606
		public static string CatAccessory;

		// Token: 0x0400025F RID: 607
		public static string CatHair;

		// Token: 0x04000260 RID: 608
		public static string CatItem;

		// Token: 0x04000261 RID: 609
		public static string CatAccSlot;

		// Token: 0x04000262 RID: 610
		public static string VisibilityShow;

		// Token: 0x04000263 RID: 611
		public static string VisibilityHide;

		// Token: 0x04000264 RID: 612
		public static string PressureLabel;

		// Token: 0x04000265 RID: 613
		public static string TabletNotDetected;

		// Token: 0x04000266 RID: 614
		public static string HudLayerFmt;

		// Token: 0x04000267 RID: 615
		public static string HudNoLayer;

		// Token: 0x04000268 RID: 616
		public static string HudShortcuts;

		// Token: 0x04000269 RID: 617
		public static string HudGrowShrink;

		// Token: 0x0400026A RID: 618
		public static string UndoLabel;

		// Token: 0x0400026B RID: 619
		public static string RedoLabel;

		// Token: 0x0400026C RID: 620
		public static string HudRenderersFmt = "Renderers: {0} (primary: {1})";

		// Token: 0x0400026D RID: 621
		public static string FaceSelectBrush;

		// Token: 0x0400026E RID: 622
		public static string FaceSelectBox;

		// Token: 0x0400026F RID: 623
		public static string RadiusSuffixFmt;

		// Token: 0x04000270 RID: 624
		public static string RemapWeights;

		// Token: 0x04000271 RID: 625
		public static string RestoreWeights;

		// Token: 0x04000272 RID: 626
		public static string BodyMeshNotReadable;

		// Token: 0x04000273 RID: 627
		public static string ExportDeform;

		// Token: 0x04000274 RID: 628
		public static string ImportDeform;

		// Token: 0x04000275 RID: 629
		public static string DeformFileFilter;

		// Token: 0x04000276 RID: 630
		public static string ExportSuccess;

		// Token: 0x04000277 RID: 631
		public static string ImportSuccess;

		// Token: 0x04000278 RID: 632
		public static string ImportVertexMismatchFmt;

		// Token: 0x04000279 RID: 633
		public static string ImportInvalidFile;

		// Token: 0x0400027A RID: 634
		public static string PresetTabLabel;

		// Token: 0x0400027B RID: 635
		public static string ExportPreset;

		// Token: 0x0400027C RID: 636
		public static string ExportPresetSelected;

		// Token: 0x0400027D RID: 637
		public static string ImportPreset;

		// Token: 0x0400027E RID: 638
		public static string ApplyImportPreset;

		// Token: 0x0400027F RID: 639
		public static string CancelImportPreset;

		// Token: 0x04000280 RID: 640
		public static string PresetFileFilter;

		// Token: 0x04000281 RID: 641
		public static string PresetExportSuccessFmt;

		// Token: 0x04000282 RID: 642
		public static string PresetImportSummaryFmt;

		// Token: 0x04000283 RID: 643
		public static string PresetInvalidFile;

		// Token: 0x04000284 RID: 644
		public static string PresetMatchOk;

		// Token: 0x04000285 RID: 645
		public static string PresetMatchNoMatch;

		// Token: 0x04000286 RID: 646
		public static string PresetRowFormatFmt;

		// Token: 0x04000287 RID: 647
		public static string PresetRowRootName;

		// Token: 0x04000288 RID: 648
		public static string PresetEmptyOwnerHint;

		// Token: 0x04000289 RID: 649
		public static string PresetNoExportableHint;

		// Token: 0x0400028A RID: 650
		public static string LiteShapeNoLayersHint;

		// Token: 0x0400028B RID: 651
		public static string PresetPreserveLayers;

		// Token: 0x0400028C RID: 652
		public static string PresetPreserveLayersTooltip;

		// Token: 0x0400028D RID: 653
		public static string PresetPreserveScope;

		// Token: 0x0400028E RID: 654
		public static string PresetPreserveScopeTooltip;

		// Token: 0x0400028F RID: 655
		public static string HelpTitle;

		// Token: 0x04000290 RID: 656
		public static string HelpBrush;

		// Token: 0x04000291 RID: 657
		public static string HelpGizmo;

		// Token: 0x04000292 RID: 658
		public static string HelpSubdivide;

		// Token: 0x04000293 RID: 659
		public static string HelpPreset;

		// Token: 0x04000294 RID: 660
		public static string CorruptedEntryPrefix;

		// Token: 0x04000295 RID: 661
		public static string CorruptedTooltipFormat;

		// Token: 0x04000296 RID: 662
		public static string ResetCorruptedButton;

		// Token: 0x04000297 RID: 663
		public static string ResetCorruptedConfirmTitle;

		// Token: 0x04000298 RID: 664
		public static string ResetCorruptedConfirmBody;

		// Token: 0x04000299 RID: 665
		public static string CorruptedEditModeBanner;

		// Token: 0x0400029A RID: 666
		public static string DeleteSelectedFaces;

		// Token: 0x0400029B RID: 667
		public static string RestoreSelectedFaces;

		// Token: 0x0400029C RID: 668
		public static string RestoreAllDeletedFaces;

		// Token: 0x0400029D RID: 669
		public static string DeletedFacesCountFmt;

		// Token: 0x0400029E RID: 670
		public static string FaceMaskHelp;

		// Token: 0x0400029F RID: 671
		public static string SettingsTabLabel;

		// Token: 0x040002A0 RID: 672
		public static string CarryoverToggleLabel;

		// Token: 0x040002A1 RID: 673
		public static string CarryoverTooltipText;

		// Token: 0x040002A2 RID: 674
		public static string CarryoverSkippedPathsFormat;

		// Token: 0x040002A3 RID: 675
		public static string CarryoverSettingsHint;

		// Token: 0x040002A4 RID: 676
		public static string PsdTabLabel;

		// Token: 0x040002A5 RID: 677
		public static string PsdSectionTitle;

		// Token: 0x040002A6 RID: 678
		public static string PsdCharacterOnlyHint;

		// Token: 0x040002A7 RID: 679
		public static string PsdNoLayerHint;

		// Token: 0x040002A8 RID: 680
		public static string PsdAddDriver;

		// Token: 0x040002A9 RID: 681
		public static string PsdDeleteDriver;

		// Token: 0x040002AA RID: 682
		public static string PsdTimelineOverriding;

		// Token: 0x040002AB RID: 683
		public static string PsdTargetLayerLabel;

		// Token: 0x040002AC RID: 684
		public static string PsdSourceBoneLabel;

		// Token: 0x040002AD RID: 685
		public static string PsdChannelLabel;

		// Token: 0x040002AE RID: 686
		public static string PsdInputRangeLabel;

		// Token: 0x040002AF RID: 687
		public static string PsdSetMinFromPose;

		// Token: 0x040002B0 RID: 688
		public static string PsdSetMaxFromPose;

		// Token: 0x040002B1 RID: 689
		public static string PsdCaptureHint;

		// Token: 0x040002B2 RID: 690
		public static string PsdFilterLabel;

		// Token: 0x040002B3 RID: 691
		public static string PsdNoneSelected;

		// Token: 0x040002B4 RID: 692
		public static string PsdDuplicateNotice;

		// Token: 0x040002B5 RID: 693
		public static string PsdInvalidRangeNotice;

		// Token: 0x040002B6 RID: 694
		public static string PsdRowHeaderFmt;

		// Token: 0x040002B7 RID: 695
		public static string PsdRowDetailFmt;

		// Token: 0x040002B8 RID: 696
		public static string PsdHelp;

		// Token: 0x040002B9 RID: 697
		public static string[] PsdChannelNames;

		// Token: 0x040002BA RID: 698
		public static string CsbTabLabel;

		// Token: 0x040002BB RID: 699
		public static string CsbSectionTitle;

		// Token: 0x040002BC RID: 700
		public static string CsbCharacterOnlyHint;

		// Token: 0x040002BD RID: 701
		public static string CsbNoLayerHint;

		// Token: 0x040002BE RID: 702
		public static string CsbAddDriver;

		// Token: 0x040002BF RID: 703
		public static string CsbDeleteDriver;

		// Token: 0x040002C0 RID: 704
		public static string CsbTimelineOverriding;

		// Token: 0x040002C1 RID: 705
		public static string CsbTargetLayerLabel;

		// Token: 0x040002C2 RID: 706
		public static string CsbClothingKindLabel;

		// Token: 0x040002C3 RID: 707
		public static string CsbUnequippedLabel;

		// Token: 0x040002C4 RID: 708
		public static string CsbCaptureFromState;

		// Token: 0x040002C5 RID: 709
		public static string CsbCaptureHint;

		// Token: 0x040002C6 RID: 710
		public static string CsbDuplicateNotice;

		// Token: 0x040002C7 RID: 711
		public static string CsbNoneSelected;

		// Token: 0x040002C8 RID: 712
		public static string CsbRowHeaderFmt;

		// Token: 0x040002C9 RID: 713
		public static string CsbRowDetailFmt;

		// Token: 0x040002CA RID: 714
		public static string CsbScopeToggle;

		// Token: 0x040002CB RID: 715
		public static string CsbScopeHint;

		// Token: 0x040002CC RID: 716
		public static string CsbScopeRowFmt;

		// Token: 0x040002CD RID: 717
		public static string CsbHelp;

		// Token: 0x040002CE RID: 718
		public static string[] CsbStateLabels;

		// Token: 0x040002CF RID: 719
		public static string NbbTabLabel;

		// Token: 0x040002D0 RID: 720
		public static string NbbSectionTitle;

		// Token: 0x040002D1 RID: 721
		public static string NbbCharacterOnlyHint;

		// Token: 0x040002D2 RID: 722
		public static string NbbNoLayerHint;

		// Token: 0x040002D3 RID: 723
		public static string NbbNoSourceHint;

		// Token: 0x040002D4 RID: 724
		public static string NbbAddDriver;

		// Token: 0x040002D5 RID: 725
		public static string NbbDeleteDriver;

		// Token: 0x040002D6 RID: 726
		public static string NbbTimelineOverriding;

		// Token: 0x040002D7 RID: 727
		public static string NbbTargetLayerLabel;

		// Token: 0x040002D8 RID: 728
		public static string NbbSourceRendererLabel;

		// Token: 0x040002D9 RID: 729
		public static string NbbSourceShapeLabel;

		// Token: 0x040002DA RID: 730
		public static string NbbInputRangeLabel;

		// Token: 0x040002DB RID: 731
		public static string NbbSetMinFromCurrent;

		// Token: 0x040002DC RID: 732
		public static string NbbSetMaxFromCurrent;

		// Token: 0x040002DD RID: 733
		public static string NbbCaptureHint;

		// Token: 0x040002DE RID: 734
		public static string NbbCurrentValueFmt;

		// Token: 0x040002DF RID: 735
		public static string NbbFilterLabel;

		// Token: 0x040002E0 RID: 736
		public static string NbbNoneSelected;

		// Token: 0x040002E1 RID: 737
		public static string NbbDuplicateNotice;

		// Token: 0x040002E2 RID: 738
		public static string NbbInvalidRangeNotice;

		// Token: 0x040002E3 RID: 739
		public static string NbbRowHeaderFmt;

		// Token: 0x040002E4 RID: 740
		public static string NbbRowDetailFmt;

		// Token: 0x040002E5 RID: 741
		public static string NbbHelp;

		// Token: 0x040002E6 RID: 742
		public static string SectionRenderer;

		// Token: 0x040002E7 RID: 743
		public static string SectionBrush;

		// Token: 0x040002E8 RID: 744
		public static string SectionGizmo;

		// Token: 0x040002E9 RID: 745
		public static string SectionSymmetry;

		// Token: 0x040002EA RID: 746
		public static string SectionLayers;

		// Token: 0x040002EB RID: 747
		public static string SectionSubdivide;

		// Token: 0x040002EC RID: 748
		public static string SectionPreset;

		// Token: 0x040002ED RID: 749
		public static string SectionSettings;

		// Token: 0x040002EE RID: 750
		public static string SectionWeightRemap;

		// Token: 0x040002EF RID: 751
		public static string SectionHelp;

		// Token: 0x040002F0 RID: 752
		public static string SectionResetConfirm;

		// Token: 0x040002F1 RID: 753
		public static string SectionDeformDataIO;

		// Token: 0x040002F2 RID: 754
		public static string SectionDeletedFaces;

		// Token: 0x040002F3 RID: 755
		public static string SectionOperationMode;

		// Token: 0x040002F4 RID: 756
		public static string BrushRadiusLabel;

		// Token: 0x040002F5 RID: 757
		public static string StrengthLabel;

		// Token: 0x02000071 RID: 113
		public enum Language
		{
			// Token: 0x040004EB RID: 1259
			English,
			// Token: 0x040004EC RID: 1260
			Japanese,
			// Token: 0x040004ED RID: 1261
			Korean,
			// Token: 0x040004EE RID: 1262
			TraditionalChinese,
			// Token: 0x040004EF RID: 1263
			SimplifiedChinese
		}
	}
}
