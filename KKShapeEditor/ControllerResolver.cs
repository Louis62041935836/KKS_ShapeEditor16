using System;
using UnityEngine;

namespace KKShapeEditor
{
	// Token: 0x02000007 RID: 7
	public static class ControllerResolver
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00002DF0 File Offset: 0x00000FF0
		public static ControllerResolver.Owner Resolve(Renderer renderer)
		{
			if (renderer == null)
			{
				return default(ControllerResolver.Owner);
			}
			return ControllerResolver.Resolve(renderer.transform);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002E1C File Offset: 0x0000101C
		public static ControllerResolver.Owner Resolve(Transform start)
		{
			if (start == null)
			{
				return default(ControllerResolver.Owner);
			}
			Transform transform = start;
			while (transform != null)
			{
				ItemShapeController component = transform.GetComponent<ItemShapeController>();
				if (component != null)
				{
					return new ControllerResolver.Owner
					{
						ItemController = component,
						RootTransform = transform
					};
				}
				ShapeEditorController component2 = transform.GetComponent<ShapeEditorController>();
				if (component2 != null)
				{
					return new ControllerResolver.Owner
					{
						CharacterController = component2,
						RootTransform = transform
					};
				}
				transform = transform.parent;
			}
			return default(ControllerResolver.Owner);
		}

		// Token: 0x02000053 RID: 83
		public struct Owner
		{
			// Token: 0x170000D7 RID: 215
			// (get) Token: 0x06000463 RID: 1123 RVA: 0x0002D27F File Offset: 0x0002B47F
			public bool IsOnCharacter
			{
				get
				{
					return this.CharacterController != null;
				}
			}

			// Token: 0x170000D8 RID: 216
			// (get) Token: 0x06000464 RID: 1124 RVA: 0x0002D28D File Offset: 0x0002B48D
			public bool IsOnItem
			{
				get
				{
					return this.ItemController != null;
				}
			}

			// Token: 0x170000D9 RID: 217
			// (get) Token: 0x06000465 RID: 1125 RVA: 0x0002D29B File Offset: 0x0002B49B
			public bool HasOwner
			{
				get
				{
					return this.CharacterController != null || this.ItemController != null;
				}
			}

			// Token: 0x06000466 RID: 1126 RVA: 0x0002D2B9 File Offset: 0x0002B4B9
			public DeformData GetDeformData(Renderer renderer)
			{
				ShapeEditorController characterController = this.CharacterController;
				DeformData deformData;
				if ((deformData = ((characterController != null) ? characterController.GetDeformData(renderer) : null)) == null)
				{
					ItemShapeController itemController = this.ItemController;
					if (itemController == null)
					{
						return null;
					}
					deformData = itemController.GetDeformData(renderer);
				}
				return deformData;
			}

			// Token: 0x06000467 RID: 1127 RVA: 0x0002D2E4 File Offset: 0x0002B4E4
			public DeformData GetOrCreateDeformData(Renderer renderer)
			{
				ShapeEditorController characterController = this.CharacterController;
				DeformData deformData;
				if ((deformData = ((characterController != null) ? characterController.GetOrCreateDeformData(renderer) : null)) == null)
				{
					ItemShapeController itemController = this.ItemController;
					if (itemController == null)
					{
						return null;
					}
					deformData = itemController.GetOrCreateDeformData(renderer);
				}
				return deformData;
			}

			// Token: 0x04000463 RID: 1123
			public ShapeEditorController CharacterController;

			// Token: 0x04000464 RID: 1124
			public ItemShapeController ItemController;

			// Token: 0x04000465 RID: 1125
			public Transform RootTransform;
		}
	}
}
