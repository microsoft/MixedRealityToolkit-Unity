// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using UInput = UnityEngine.Input;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Screen Space Mouse Pointer Implementation.
    /// </summary>
    public class ScreenSpaceMousePointer : BaseMousePointer
    {
        private Vector2 mousePosition;

        protected override string ControllerName => "ScreenSpace Mouse Pointer";

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            if (UInput.mousePosition.x < 0 ||
                UInput.mousePosition.y < 0 ||
                UInput.mousePosition.x > Screen.width ||
                UInput.mousePosition.y > Screen.height)
            {
                return;
            }

            if ((mousePosition - (Vector2) UInput.mousePosition).magnitude >= MovementThresholdToUnHide)
            {
                SetVisibility(true);
            }
            mousePosition = UInput.mousePosition;

            Camera mainCamera = CameraCache.Main;
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            Rays[0].CopyRay(ray, float.MaxValue);

            transform.position = mainCamera.transform.position;
            transform.rotation = Quaternion.LookRotation(ray.direction);
        }

        protected override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);
            Cursor.visible = visible;
        }
    }
}