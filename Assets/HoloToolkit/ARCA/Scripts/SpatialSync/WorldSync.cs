// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace HoloToolkit.ARCapture
{
    public class WorldSync : NetworkBehaviour
    {
        [Tooltip("Transform of the content container")]
        public Transform WorldRoot;

        [Tooltip("Component for sending hololens webcam feed to the marker detection code")]
        public MarkerDetectionHololens HololensMarkerDetector;

        [Tooltip("Number of captures used to find find a average position/rotation")]
        public int NumCapturesRequired;

        [Tooltip("The maximum distance between a capture and the average of the number of captures required")]
        public float MarkerCaptureErrorDistance;

        [Tooltip("The offset from the marker position displayed on screen and the phones camera")]
        public Vector3 OffsetBetweenMarkerAndCamera;

        [Tooltip("Event fired after the marker position/rotation has been found")]
        public UnityEvent OnDetectedMobile;

        [Tooltip("")] public MarkerGeneration3D MarkerGeneration3D;

        public delegate void OnWorldSyncCompleteEvent();

        // Invoked once world adjustment has finished
        public OnWorldSyncCompleteEvent OnWorldSyncComplete;
        public OnWorldSyncCompleteEvent OnWorldSyncCompleteClient;

        // String used to sync transform information
        // stored in the format: xPos:yPos:zPos:yRot
        [SyncVar(hook = "AdjustOrientation")]
        private string syncedTransformString;

        // Position of the marker in World-Space
        private Vector3 orientationPosition;

        // Y axis rotation of the marker in World-Space
        private float orientationRotation;
        private List<Vector3> positions = new List<Vector3>();
        private List<Quaternion> rotations = new List<Quaternion>();

        void OnDestroy()
        {
            HololensMarkerDetector.OnMarkerDetected -= UpdatePositionAndRotation;
        }

        public void StartSyncing()
        {
            HololensMarkerDetector.OnMarkerDetected -= UpdatePositionAndRotation;
            HololensMarkerDetector.OnMarkerDetected += UpdatePositionAndRotation;
        }

        public void StopSyncing()
        {
            HololensMarkerDetector.OnMarkerDetected -= UpdatePositionAndRotation;
        }

        void UpdatePositionAndRotation( int markerId, Vector3 pos, Quaternion rot )
        {
            if (positions.Count < NumCapturesRequired)
            {
                positions.Add(pos);
                rotations.Add(rot);
            }
            else
            {
                // Find the average marker position
                Vector3 averagePosition = Vector3.zero;
                for (int i = 0; i < positions.Count; i++)
                {
                    averagePosition += positions[i];
                }

                averagePosition /= positions.Count;

                // Remove any positions that are far away from the average
                for (var i = 0; i < positions.Count; i++)
                {
                    if (Vector3.Distance(positions[i], averagePosition) > MarkerCaptureErrorDistance)
                    {
                        positions.Clear();
                        rotations.Clear();
                        //No point continuing with the execution. Return and let it all begin again. s
                        return;
                    }
                }

                // Find the average marker rotation
                Quaternion averageRotation = Quaternion.identity;
                averageRotation =
                    Quaternion.Lerp(rotations[2], Quaternion.Lerp(rotations[0], rotations[1], 0.5f), 0.5f);

                syncedTransformString = averagePosition.x + ":" + averagePosition.y + ":" + averagePosition.z + ":" +
                                        averageRotation.eulerAngles.y + ":" + markerId;

                if (OnWorldSyncComplete != null)
                {
                    OnWorldSyncComplete();
                }

                StopSyncing();

                positions.Clear();
                rotations.Clear();
            }
        }

        void AdjustOrientation( string str )
        {
            var isHost = FindObjectOfType<PlatformSwitcher>().TargetPlatform == PlatformSwitcher.Platform.Hololens;
            if (!isHost)
            {
                string[] strings = str.Split(new Char[] {':'});
                int markerId;
                if (strings.Length >= 4)
                {
                    orientationPosition.x = float.Parse(strings[0], CultureInfo.InvariantCulture.NumberFormat);
                    orientationPosition.y = float.Parse(strings[1], CultureInfo.InvariantCulture.NumberFormat);
                    orientationPosition.z = float.Parse(strings[2], CultureInfo.InvariantCulture.NumberFormat);
                    orientationRotation = float.Parse(strings[3], CultureInfo.InvariantCulture.NumberFormat);
                    markerId = int.Parse(strings[4], CultureInfo.InvariantCulture.NumberFormat);

                    if (markerId == MarkerGeneration3D.MarkerId)
                    {
                        AdjustWorld();
                        if (OnWorldSyncCompleteClient != null)
                        {
                            OnWorldSyncCompleteClient();
                        }
                    }
                }
            }

            syncedTransformString = str;
        }

        void AdjustWorld()
        {
            if (isServer)
            {
                return;
            }

            // put the container at phone position
            WorldRoot.transform.position = Camera.main.transform.position;

            // place container world looking in same direction as camera.
            WorldRoot.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);

            // rotate according to world
            WorldRoot.transform.eulerAngles -= new Vector3(0, orientationRotation - 180, 0);

            // adjust container to 0,0 of HL
            WorldRoot.transform.Translate(-orientationPosition + OffsetBetweenMarkerAndCamera, Space.Self);

            OnDetectedMobile.Invoke();
        }
    }
}
