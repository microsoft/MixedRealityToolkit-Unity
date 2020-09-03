// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Windows.Input
{
#if UNITY_WSA
    public static class GestureRecognizerExtensions
    {
        public static void UpdateAndResetGestures(this UnityEngine.XR.WSA.Input.GestureRecognizer recognizer, UnityEngine.XR.WSA.Input.GestureSettings gestureSettings)
        {
            bool reset = recognizer.IsCapturingGestures();

            if (reset)
            {
                recognizer.CancelGestures();
            }

            recognizer.SetRecognizableGestures(gestureSettings);

            if (reset)
            {
                recognizer.StartCapturingGestures();
            }
        }
    }
#endif
}