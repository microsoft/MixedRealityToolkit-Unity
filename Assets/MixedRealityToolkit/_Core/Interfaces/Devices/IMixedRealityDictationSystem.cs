// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices
{
    /// <summary>
    /// Mixed Reality Toolkit controller definition, used to manage a specific controller type
    /// </summary>
    public interface IMixedRealityDictationSystem : IMixedRealityManager
    {
        void StartRecording(GameObject listener, float initialSilenceTimeout, float autoSilenceTimeout, int recordingTime);

        void StopRecording();
    }
}
