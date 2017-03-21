// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Base class for an input source.
    /// </summary>
    public abstract class BaseInputSource : StartAwareBehaviour, IInputSource
    {
        public abstract SupportedInputInfo GetSupportedInputInfo(uint sourceId);

        public bool SupportsInputInfo(uint sourceId, SupportedInputInfo inputInfo)
        {
            return ((GetSupportedInputInfo(sourceId) & inputInfo) == inputInfo);
        }

        public abstract bool TryGetSourceKind(uint sourceId, out InteractionSourceKind sourceKind);

        public abstract bool TryGetPosition(uint sourceId, out Vector3 position);

        public abstract bool TryGetOrientation(uint sourceId, out Quaternion orientation);

        public abstract bool TryGetPointingRay(uint sourceId, out Ray pointingRay);

        public abstract bool TryGetThumbstick(uint sourceId, out bool isPressed, out double x, out double y);
        public abstract bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out double x, out double y);
        public abstract bool TryGetTrigger(uint sourceId, out bool isPressed, out double pressedValue);
        public abstract bool TryGetGrasp(uint sourceId, out bool isPressed);
        public abstract bool TryGetMenu(uint sourceId, out bool isPressed);
    }
}
