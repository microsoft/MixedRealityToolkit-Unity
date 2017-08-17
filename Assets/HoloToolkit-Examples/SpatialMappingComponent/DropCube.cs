// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

#if UNITY_EDITOR || UNITY_WSA
using UnityEngine.VR.WSA.Input;
#endif

namespace HoloToolkit.Examples.SpatialMappingComponent
{
    /// <summary>
    /// Simple test script for dropping cubes with physics to observe interactions
    /// </summary>
    public class DropCube : MonoBehaviour
    {
#if UNITY_EDITOR || UNITY_WSA
        GestureRecognizer recognizer;

        private void Start()
        {
            recognizer = new GestureRecognizer();
            recognizer.SetRecognizableGestures(GestureSettings.Tap);
            recognizer.TappedEvent += Recognizer_TappedEvent;
            recognizer.StartCapturingGestures();
        }

        private void OnDestroy()
        {
            recognizer.TappedEvent -= Recognizer_TappedEvent;
        }

        private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
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