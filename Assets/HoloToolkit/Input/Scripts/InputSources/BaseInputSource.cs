// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Base class for an input source.
    /// </summary>
    public abstract class BaseInputSource : MonoBehaviour, IInputSource
    {
        public abstract SupportedInputInfo GetSupportedInputInfo(uint sourceId);

        public bool SupportsInputInfo(uint sourceId, SupportedInputInfo inputInfo)
        {
            return ((GetSupportedInputInfo(sourceId) & inputInfo) == inputInfo);
        }

        public abstract bool TryGetSourceKind(uint sourceId, out InteractionSourceInfo sourceKind);

        public abstract bool TryGetPointerPosition(uint sourceId, out Vector3 position);

        public abstract bool TryGetPointerRotation(uint sourceId, out Quaternion rotation);

        public abstract bool TryGetPointingRay(uint sourceId, out Ray pointingRay);

        public abstract bool TryGetGripPosition(uint sourceId, out Vector3 position);

        public abstract bool TryGetGripRotation(uint sourceId, out Quaternion rotation);

        public abstract bool TryGetThumbstick(uint sourceId, out bool isPressed, out Vector2 position);

        public abstract bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out Vector2 position);

        public abstract bool TryGetSelect(uint sourceId, out bool isPressed, out double pressedValue);

        public abstract bool TryGetGrasp(uint sourceId, out bool isPressed);

        public abstract bool TryGetMenu(uint sourceId, out bool isPressed);
    }
}
