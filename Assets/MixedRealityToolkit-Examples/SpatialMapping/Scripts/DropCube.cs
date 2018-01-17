// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using UnityEngine;

#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif
#endif

namespace MixedRealityToolkit.Examples.SpatialMapping
{
    /// <summary>
    /// Simple test script for dropping cubes with physics to observe interactions
    /// </summary>
    public class DropCube : MonoBehaviour
    {
#if UNITY_WSA
        private GestureRecognizer recognizer;

        private void Start()
        {
            recognizer = new GestureRecognizer();
            recognizer.SetRecognizableGestures(GestureSettings.Tap);
#if UNITY_2017_2_OR_NEWER
            recognizer.Tapped += Recognizer_Tapped;
#else
            recognizer.TappedEvent += Recognizer_Tapped;
#endif
            recognizer.StartCapturingGestures();
        }

        private void OnDestroy()
        {
#if UNITY_2017_2_OR_NEWER
            recognizer.Tapped -= Recognizer_Tapped;
#else
            recognizer.TappedEvent -= Recognizer_Tapped;
#endif
        }

        private void Recognizer_Tapped(
#if UNITY_2017_2_OR_NEWER
            TappedEventArgs obj
#else
            InteractionSourceKind source, int tapCount, Ray headRay
#endif
            )
        {
            // Create a cube
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // Make the cube smaller
            cube.transform.localScale = Vector3.one * 0.3f;
            // Start to drop it in front of the camera
            cube.transform.position = CameraCache.Main.transform.position + CameraCache.Main.transform.forward;
            // Apply physics
            cube.AddComponent<Rigidbody>();
        }
#endif
    }
}