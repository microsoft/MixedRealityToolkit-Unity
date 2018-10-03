// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if WINDOWS_UWP
using Windows.Perception.Spatial;
#endif
namespace HoloToolkit.Unity
{
    public class SpatialGraphCoordinateSystem : MonoBehaviour
    {
#if WINDOWS_UWP
        private SpatialCoordinateSystem CoordinateSystem = null;
#endif
        private System.Guid id;
        private UnityEngine.XR.WSA.PositionalLocatorState CurrentState { get; set; }
        public System.Guid Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
#if WINDOWS_UWP
                CoordinateSystem = Windows.Perception.Spatial.Preview.SpatialGraphInteropPreview.CreateCoordinateSystemForNode(id);
#endif
            }
        }

        private void Awake()
        {
            CurrentState = UnityEngine.XR.WSA.PositionalLocatorState.Unavailable;
        }

        private void Start()
        {
            UnityEngine.XR.WSA.WorldManager.OnPositionalLocatorStateChanged += WorldManager_OnPositionalLocatorStateChanged;
            CurrentState = UnityEngine.XR.WSA.WorldManager.state;
#if WINDOWS_UWP
            if (CoordinateSystem == null)
            {
                CoordinateSystem = Windows.Perception.Spatial.Preview.SpatialGraphInteropPreview.CreateCoordinateSystemForNode(id);
            }
#endif
        }

        private void WorldManager_OnPositionalLocatorStateChanged(UnityEngine.XR.WSA.PositionalLocatorState oldState, UnityEngine.XR.WSA.PositionalLocatorState newState)
        {
            CurrentState = newState;
            if (newState == UnityEngine.XR.WSA.PositionalLocatorState.Active)
            {
                // This simply activates/deactivates this object and all children when tracking changes
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void UpdateLocation()
        {
            if (CurrentState == UnityEngine.XR.WSA.PositionalLocatorState.Active)
            {
#if WINDOWS_UWP
                if (CoordinateSystem == null)
                {
                    CoordinateSystem = Windows.Perception.Spatial.Preview.SpatialGraphInteropPreview.CreateCoordinateSystemForNode(id);
                }

                if (CoordinateSystem != null)
                {
                    Quaternion rotation = Quaternion.identity;
                    Vector3 translation = new Vector3(0.0f, 0.0f, 0.0f);

                    SpatialCoordinateSystem rootSpatialCoordinateSystem = (SpatialCoordinateSystem)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(UnityEngine.XR.WSA.WorldManager.GetNativeISpatialCoordinateSystemPtr());

                    // Get the relative transform from the unity origin
                    System.Numerics.Matrix4x4? relativePose = CoordinateSystem.TryGetTransformTo(rootSpatialCoordinateSystem);

                    if (relativePose != null)
                    {
                        System.Numerics.Vector3 scale;
                        System.Numerics.Quaternion rotation1;
                        System.Numerics.Vector3 translation1;
       
                        System.Numerics.Matrix4x4 newMatrix = relativePose.Value;

                        // Platform coordinates are all right handed and unity uses left handed matrices. so we convert the matrix
                        // from rhs-rhs to lhs-lhs 
                        // Convert from right to left coordinate system
                        newMatrix.M13 = -newMatrix.M13;
                        newMatrix.M23 = -newMatrix.M23;
                        newMatrix.M43 = -newMatrix.M43;

                        newMatrix.M31 = -newMatrix.M31;
                        newMatrix.M32 = -newMatrix.M32;
                        newMatrix.M34 = -newMatrix.M34;

                        System.Numerics.Matrix4x4.Decompose(newMatrix, out scale, out rotation1, out translation1);
                        translation = new Vector3(translation1.X, translation1.Y, translation1.Z);
                        rotation = new Quaternion(rotation1.X, rotation1.Y, rotation1.Z, rotation1.W);
                        Pose pose = new Pose(translation, rotation);

                        // If there is a parent to the camera that means we are using teleport and we should not apply the teleport
                        // to these objects so apply the inverse
                        if (CameraCache.Main.transform.parent != null)
                        {
                            pose = pose.GetTransformedBy(CameraCache.Main.transform.parent);
                        }

                        gameObject.transform.SetPositionAndRotation(pose.position, pose.rotation);
                        //Debug.Log("Id= " + id + " QRPose = " +  pose.position.ToString("F7") + " QRRot = "  +  pose.rotation.ToString("F7"));
                    }
                }
                else
                {
                   gameObject.SetActive(false);
                }
#endif
            }
        }

        private void Update()
        {
            UpdateLocation();
        }
    }
}