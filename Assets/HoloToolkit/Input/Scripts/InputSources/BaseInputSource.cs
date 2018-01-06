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
        public uint SourceId { get; protected set; }

        public abstract SupportedInputInfo GetSupportedInputInfo();

        public bool SupportsInputInfo(SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo() & inputInfo) == inputInfo;
        }
    }
}
