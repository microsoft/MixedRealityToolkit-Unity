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
        public UnityEngine.AudioClip Sound = null;
        public bool Looping = false;

        public float DelayCenter = 0;
        public float DelayRandomization = 0;
    }
}