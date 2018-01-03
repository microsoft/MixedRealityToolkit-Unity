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
            return (GetSupportedInputInfo(sourceId) & inputInfo) == inputInfo;
        }

        public abstract bool TryGetPointerPosition(uint sourceId, out Vector3 position);

        public abstract bool TryGetPointingRay(uint sourceId, out Ray pointingRay);
    }
}
