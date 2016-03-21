// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Encapsulate a single Unity AudioClip with playback settings.
    /// </summary>
    [Serializable]
    public class UAudioClip
    {
        public UnityEngine.AudioClip sound = null;
        public bool looping = false;

        public float delayCenter = 0;
        public float delayRandomization = 0;
    }
}