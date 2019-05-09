// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine.XR.WSA;
using System.Runtime.InteropServices;
using System.Globalization;
using System;
using System.IO;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.Perception;
using Windows.Perception.Spatial;
using Calendar = Windows.Globalization.Calendar;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.HolographicCamera
{
    [RequireComponent(typeof(TCPConnectionManager))]
    public class CameraPoseProvider : MonoBehaviour
    {
        [SerializeField]
        private int port = 7502;

        [SerializeField]
        private Transform originTransform = null;

        private TCPConnectionManager tcpConnectionManager;
        private Stopwatch timestampStopwatch;

#if !UNITY_EDITOR && UNITY_WSA
        private Calendar timeConversionCalendar;
#endif

        private void Awake()
        {
            tcpConnectionManager = GetComponent<TCPConnectionManager>();
            tcpConnectionManager.OnConnected += TcpConnectionManager_OnConnected;
            tcpConnectionManager.StartListening(port);
        }

        private void Update()
        {
            if (!tcpConnectionManager.HasConnections)
            {
                return;
            }

            // Get an adjusted position and rotation based on the historical pose for the current time.
            // The Unity camera uses pose prediction and doesn't reflect the actual historical pose of
            // the device.
            Vector3 cameraPosition;
            Quaternion cameraRotation;
            bool hasNewPose = GetHistoricalPose(out cameraPosition, out cameraRotation);

            if (hasNewPose)
            {
                float timestamp = (float)timestampStopwatch.Elapsed.TotalSeconds;

                // Translate the camera pose into an anchor-relative pose.
                cameraPosition = originTransform.InverseTransformPoint(cameraPosition);
                cameraRotation = Quaternion.Inverse(originTransform.rotation) * cameraRotation;

                SendCameraPose(timestamp, cameraPosition, cameraRotation);
            }
        }

        private void TcpConnectionManager_OnConnected(SocketEndpoint endpoint)
        {
            timestampStopwatch = Stopwatch.StartNew();
        }

        private void SendCameraPose(float timestamp, Vector3 cameraPosition, Quaternion cameraRotation)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(stream))
            {
                message.Write("Camera");
                message.Write(timestamp);
                message.Write(cameraPosition);
                message.Write(cameraRotation);

                tcpConnectionManager.Broadcast(stream.ToArray());
            }
        }

        private bool GetHistoricalPose(out Vector3 cameraPosition, out Quaternion cameraRotation)
        {
#if !UNITY_EDITOR && UNITY_WSA
            SpatialCoordinateSystem unityCoordinateSystem = Marshal.GetObjectForIUnknown(WorldManager.GetNativeISpatialCoordinateSystemPtr()) as SpatialCoordinateSystem;
            if (unityCoordinateSystem == null)
            {
                Debug.LogError("Failed to get the native SpatialCoordinateSystem");
                cameraPosition = default(Vector3);
                cameraRotation = default(Quaternion);
                return false;
            }

            if (timeConversionCalendar == null)
            {
                timeConversionCalendar = new Calendar();
            }

            timeConversionCalendar.SetToNow();

            PerceptionTimestamp perceptionTimestamp = PerceptionTimestampHelper.FromHistoricalTargetTime(timeConversionCalendar.GetDateTime());

            if (perceptionTimestamp != null)
            {
                SpatialLocator locator = SpatialLocator.GetDefault();
                if (locator != null)
                {
                    SpatialLocation headPose = locator.TryLocateAtTimestamp(perceptionTimestamp, unityCoordinateSystem);
                    if (headPose != null)
                    {
                        var systemOrientation = headPose.Orientation;
                        var systemPostion = headPose.Position;

                        // Convert the orientation and position from Windows to Unity coordinate spaces
                        cameraRotation.x = -systemOrientation.X;
                        cameraRotation.y = -systemOrientation.Y;
                        cameraRotation.z = systemOrientation.Z;
                        cameraRotation.w = systemOrientation.W;

                        cameraPosition.x = systemPostion.X;
                        cameraPosition.y = systemPostion.Y;
                        cameraPosition.z = -systemPostion.Z;
                        return true;
                    }
                }
            }

            cameraPosition = default(Vector3);
            cameraRotation = default(Quaternion);
            return false;
#else
            cameraPosition = Camera.main.transform.position;
            cameraRotation = Camera.main.transform.rotation;
            return true;
#endif
        }
    }
}