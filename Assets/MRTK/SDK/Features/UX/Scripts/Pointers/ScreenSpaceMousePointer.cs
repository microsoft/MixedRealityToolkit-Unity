// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Uses the desktop mouse cursor instead of any mouse representation within the scene.
    /// Its movement is bound to screenspace.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/ScreenSpaceMousePointer")]
    public class ScreenSpaceMousePointer : BaseMousePointer
    {
        private Vector2 lastMousePosition;

        /// <inheritdoc />
        protected override string ControllerName => "ScreenSpace Mouse Pointer";

        private static readonly ProfilerMarker OnPreSceneQueryPerfMarker = new ProfilerMarker("[MRTK] ScreenSpaceMousePointer.OnPreSceneQuery");

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            using (OnPreSceneQueryPerfMarker.Auto())
            {
                if (UInput.mousePosition.x < 0 ||
                    UInput.mousePosition.y < 0 ||
                    UInput.mousePosition.x > Screen.width ||
                    UInput.mousePosition.y > Screen.height)
                {
                    return;
                }

                Vector3 currentMousePosition = UInput.mousePosition;

                if ((lastMousePosition - (Vector2)currentMousePosition).magnitude >= MovementThresholdToUnHide)
                {
                    SetVisibility(true);
                }

                lastMousePosition = currentMousePosition;

                Camera mainCamera = CameraCache.Main;
                Ray ray = mainCamera.ScreenPointToRay(currentMousePosition);
                Rays[0].CopyRay(ray, float.MaxValue);

                transform.position = mainCamera.transform.position;
                transform.rotation = Quaternion.LookRotation(ray.direction);
            }
        }

        /// <inheritdoc />
        protected override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);
            Cursor.visible = visible;
        }
    }
}