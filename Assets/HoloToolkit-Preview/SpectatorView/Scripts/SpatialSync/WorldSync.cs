// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Manages the sync of the world between the HoloLens and the mobile
    /// </summary>
    public class WorldSync : NetworkBehaviour
    {
        /// <summary>
        /// Transform of the content container
        /// </summary>
        [Tooltip("Transform of the content container")]
        [SerializeField]
        private Transform worldRoot;

        /// <summary>
        /// Component for sending hololens webcam feed to the marker detection code
        /// </summary>
        [Tooltip("Component for sending hololens webcam feed to the marker detection code")]
        [SerializeField]
        private MarkerDetectionHololens hololensMarkerDetector;

        /// <summary>
        /// Number of captures used to find find a average position/rotation
        /// </summary>
        [Tooltip("Number of captures used to find find a average position/rotation")]
        [SerializeField]
        private int numCapturesRequired;

        /// <summary>
        /// The maximum distance between a capture and the average of the number of captures required
        /// </summary>
        [Tooltip("The maximum distance between a capture and the average of the number of captures required")]
        [SerializeField]
        private float markerCaptureErrorDistance;

        /// <summary>
        /// The offset from the marker position displayed on screen and the phones camera
        /// </summary>
        [Tooltip("The offset from the marker position displayed on screen and the phones camera")]
        [SerializeField]
        private Vector3 offsetBetweenMarkerAndCamera = Vector3.zero;

        /// <summary>
        /// Event fired after the marker position/rotation has been found
        /// </summary>
        [Tooltip("Event fired after the marker position/rotation has been found")]
        [SerializeField]
        private UnityEvent onDetectedMobile;

        /// <summary>
        /// Marker generation component
        /// </summary>
        [Tooltip("Marker generation component")]
        [SerializeField]
        private MarkerGeneration3D markerGeneration3D;

        /// <summary>
        /// Event for when the world sync
        /// </summary>
        public delegate void OnWorldSyncCompleteEvent();

        /// <summary>
        /// Invoked once world adjustment has finished
        /// </summary>
        public OnWorldSyncCompleteEvent OnWorldSyncComplete;

        /// <summary>
        /// Invoked on the client once world adjustment has finished
        /// </summary>
        public OnWorldSyncCompleteEvent OnWorldSyncCompleteClient;

#pragma warning disable 0414
        /// <summary>
        /// String used to sync transform information
        /// stored in the format: xPos:yPos:zPos:yRot
        /// </summary>
        [SyncVar(hook = "AdjustOrientation")]
        private string syncedTransformString;
#pragma warning restore 0414

        /// <summary>
        /// Position of the marker in World-Space
        /// </summary>
        private Vector3 orientationPosition;

        /// <summary>
        /// Y axis rotation of the marker in World-Space
        /// </summary>
        private float orientationRotation;

        /// <summary>
        /// List of positions where the marker was found. It'll be used to create the average position
        /// </summary>
        private List<Vector3> positions = new List<Vector3>();

        /// <summary>
        /// List of rotations where the marker was found. It'll be used to create the average rotation
        /// </summary>
        private List<Quaternion> rotations = new List<Quaternion>();

        /// <summary>
        /// Transform of the content container
        /// </summary>
        public Transform WorldRoot
        {
            get { return worldRoot; }
            set { worldRoot = value; }
        }

        /// <summary>
        /// Component for sending hololens webcam feed to the marker detection code
        /// </summary>
        public MarkerDetectionHololens HololensMarkerDetector
        {
            get { return hololensMarkerDetector; }
            set { hololensMarkerDetector = value; }
        }

        /// <summary>
        /// Number of captures used to find find a average position/rotation
        /// </summary>
        public int NumCapturesRequired
        {
            get { return numCapturesRequired; }
            set { numCapturesRequired = value; }
        }

        /// <summary>
        /// The maximum distance between a capture and the average of the number of captures required
        /// </summary>
        public float MarkerCaptureErrorDistance
        {
            get { return markerCaptureErrorDistance; }
            set { markerCaptureErrorDistance = value; }
        }

        /// <summary>
        /// Event fired after the marker position/rotation has been found
        /// </summary>
        public UnityEvent OnDetectedMobile
        {
            get { return onDetectedMobile; }
            set { onDetectedMobile = value; }
        }

        /// <summary>
        /// Marker generation component
        /// </summary>
        public MarkerGeneration3D Generation3D
        {
            get { return markerGeneration3D; }
            set { markerGeneration3D = value; }
        }

        void OnDestroy()
        {
            HololensMarkerDetector.OnMarkerDetected -= UpdatePositionAndRotation;
        }

        /// <summary>
        /// Starts the sync process
        /// </summary>
        public void StartSyncing()
        {
            HololensMarkerDetector.OnMarkerDetected -= UpdatePositionAndRotation;
            HololensMarkerDetector.OnMarkerDetected += UpdatePositionAndRotation;
        }

        /// <summary>
        /// Stops the sync process
        /// </summary>
        public void StopSyncing()
        {
            HololensMarkerDetector.OnMarkerDetected -= UpdatePositionAndRotation;
        }

        /// <summary>
        /// Takes various photos and logs the position and rotation on each iteration
        /// Once that finishes it finds the average position and rotation for the entire process
        /// </summary>
        /// <param name="markerId">Id of the marker</param>
        /// <param name="pos">Position where the marker was found</param>
        /// <param name="rot">Rotation of the marker</param>
        private void UpdatePositionAndRotation(int markerId, Vector3 pos, Quaternion rot)
        {
            if (positions.Count < NumCapturesRequired)
            {
                positions.Add(pos);
                rotations.Add(rot);
            }
            else
            {
                // Find the average marker position
                var averagePosition = Vector3.zero;
                for (var i = 0; i < positions.Count; i++)
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
                        // No point continuing with the execution. Return and let it all begin again.
                        return;
                    }
                }

                // Find the average marker rotation
                var averageRotation = Quaternion.Lerp(rotations[2], Quaternion.Lerp(rotations[0], rotations[1], 0.5f), 0.5f);

                syncedTransformString = string.Format("{0}:{1}:{2}:{3}:{4}",
                                                       averagePosition.x, averagePosition.y, averagePosition.z,
                                                       averageRotation.eulerAngles.y,
                                                       markerId);

                if (OnWorldSyncComplete != null)
                {
                    OnWorldSyncComplete();
                }

                StopSyncing();

                positions.Clear();
                rotations.Clear();
            }
        }

        /// <summary>
        /// Adjust the orientation on the client to match the HoloLens's
        /// </summary>
        /// <param name="str"></param>
        private void AdjustOrientation(string str)
        {
            var isHost = FindObjectOfType<PlatformSwitcher>().TargetPlatform == PlatformSwitcher.Platform.Hololens;
            if (!isHost)
            {
                string[] strings = str.Split(new Char[] { ':' });
                int markerId;
                if (strings.Length >= 4)
                {
                    orientationPosition.x = float.Parse(strings[0], CultureInfo.InvariantCulture.NumberFormat);
                    orientationPosition.y = float.Parse(strings[1], CultureInfo.InvariantCulture.NumberFormat);
                    orientationPosition.z = float.Parse(strings[2], CultureInfo.InvariantCulture.NumberFormat);
                    orientationRotation = float.Parse(strings[3], CultureInfo.InvariantCulture.NumberFormat);
                    markerId = int.Parse(strings[4], CultureInfo.InvariantCulture.NumberFormat);

                    if (markerId == Generation3D.MarkerId)
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

        /// <summary>
        /// Adjusts the world in the client to match the HoloLens's world
        /// </summary>
        private void AdjustWorld()
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
            WorldRoot.transform.Translate(-orientationPosition + offsetBetweenMarkerAndCamera, Space.Self);

            OnDetectedMobile.Invoke();
        }
    }
}
