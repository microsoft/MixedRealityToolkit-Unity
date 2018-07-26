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
        //Update is called one per frame
        private void Update()
#endif
        {
            //If the button assigned to checkMicrophoneKey is pressed
            if(Input.GetKeyDown(checkMicrophoneKey))
            {
#if ENABLE_WINMD_SUPPORT
                var status = await MicrophoneHelper.GetMicrophoneStatus();
#else
                //Call to GetMicrophoneStatus() method
                var status = MicrophoneHelper.GetMicrophoneStatus();
#endif
                Debug.LogFormat("Microphone status: {0}", status);
            }
        }
    }
}
