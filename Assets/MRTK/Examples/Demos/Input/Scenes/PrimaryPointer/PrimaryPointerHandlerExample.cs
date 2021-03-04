// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    // Simple example script that subscribes to primary pointer changes and applies a cursor highlight to the current one.
    [AddComponentMenu("Scripts/MRTK/Examples/PrimaryPointerHandlerExample")]
    public class PrimaryPointerHandlerExample : MonoBehaviour
    {
        public GameObject CursorHighlight;

        private void OnEnable()
        {
            CoreServices.InputSystem?.FocusProvider?.SubscribeToPrimaryPointerChanged(OnPrimaryPointerChanged, true);
        }

        private void OnPrimaryPointerChanged(IMixedRealityPointer oldPointer, IMixedRealityPointer newPointer)
        {
            if (CursorHighlight != null)
            {
                if (newPointer != null)
                {
                    Transform parentTransform = null;

                    // For this example scene, use special logic if a GGVPointer becomes the primary pointer. 
                    // In particular, the GGV pointer defers its cursor management to the GazeProvider, which has its own internal pointer definition as well
                    // In the future, the pointer/cursor relationship will be reworked and standardized to remove this awkward set of co-dependencies
                    if (newPointer is GGVPointer)
                    {
                        parentTransform = CoreServices.InputSystem.GazeProvider.GazePointer.BaseCursor.TryGetMonoBehaviour(out MonoBehaviour baseCursor) ? baseCursor.transform : null;
                    }
                    else
                    {
                        parentTransform = newPointer.BaseCursor.TryGetMonoBehaviour(out MonoBehaviour baseCursor) ? baseCursor.transform : null;
                    }

                    // If there's no cursor try using the controller pointer transform instead
                    if (parentTransform == null)
                    {
                        if (newPointer.TryGetMonoBehaviour(out MonoBehaviour controllerPointer))
                        {
                            parentTransform = controllerPointer.transform;
                        }
                    }

                    if (parentTransform != null)
                    {
                        CursorHighlight.transform.SetParent(parentTransform, false);
                        CursorHighlight.SetActive(true);
                        return;
                    }
                }

                CursorHighlight.SetActive(false);
                CursorHighlight.transform.SetParent(null, false);
            }
        }

        private void OnDisable()
        {
            CoreServices.InputSystem?.FocusProvider?.UnsubscribeFromPrimaryPointerChanged(OnPrimaryPointerChanged);
            OnPrimaryPointerChanged(null, null);
        }
    }
}
