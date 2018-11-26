// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.Examples
{
    public class MicrophoneHelperSample : MonoBehaviour
    {
        [Tooltip("Assign key to quickly test the microphone helper code")]
        public KeyCode checkMicrophoneKey = KeyCode.M;
#if ENABLE_WINMD_SUPPORT
        private async void Update()
#else
        private void Update()
#endif
        {
            if(Input.GetKeyDown(checkMicrophoneKey))
            {
#if ENABLE_WINMD_SUPPORT
                var status = await MicrophoneHelper.GetMicrophoneStatus();
#else
                var status = MicrophoneHelper.GetMicrophoneStatus();
#endif
                Debug.LogFormat("Microphone status: {0}", status);
            }
        }
    }
}
