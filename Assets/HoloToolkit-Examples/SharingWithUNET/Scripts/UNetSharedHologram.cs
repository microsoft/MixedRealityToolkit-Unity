// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Unity.SharingWithUNET
{
    public class UNetSharedHologram : NetworkBehaviour, IInputClickHandler
    {
        /// <summary>
        /// The position relative to the shared world anchor.
        /// </summary>
        [SyncVar(hook = "xformchange")]
        private Vector3 localPosition;

        private void xformchange(Vector3 update)
        {
            Debug.Log(localPosition + " xform change " + update);
            localPosition = update;
        }

        // <summary>
        /// The rotation relative to the shared world anchor.
        /// </summary>
        [SyncVar]
        private Quaternion localRotation;

        /// <summary>
        /// Sets the localPosition and localRotation on clients.
        /// </summary>
        /// <param name="postion">the localPosition to set</param>
        /// <param name="rotation">the localRotation to set</param>
        [Command]
        public void CmdTransform(Vector3 postion, Quaternion rotation)
        {
            if (!isLocalPlayer)
            {
                localPosition = postion;
                localRotation = rotation;
            }
        }

        private bool Moving;
        private int layerMask;
        private InputManager inputManager;
        public Vector3 movementOffset = Vector3.zero;
        private bool isOpaque = false;

        // Use this for initialization
        private void Start()
        {
#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
            isOpaque = UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque;
#else
            isOpaque = !UnityEngine.VR.VRDevice.isPresent;
#endif
#endif
            transform.SetParent(SharedCollection.Instance.transform, true);
            if (isServer)
            {
                localPosition = transform.localPosition;
                localRotation = transform.localRotation;
            }

            layerMask = SpatialMappingManager.Instance.LayerMask;
            inputManager = InputManager.Instance;

        }

        // Update is called once per frame
        private void Update()
        {

            if (Moving)
            {
                transform.position = Vector3.Lerp(transform.position, ProposeTransformPosition(), 0.2f);
            }
            else
            {

                transform.localPosition = localPosition;
                transform.localRotation = localRotation;
            }
        }

        private Vector3 ProposeTransformPosition()
        {
            // Put the model 3m in front of the user.
            Vector3 retval = Camera.main.transform.position + Camera.main.transform.forward * 3 + movementOffset;
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 5.0f, layerMask))
            {
                retval = hitInfo.point + movementOffset;
            }
            return retval;
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (isOpaque == false)
            {
                Moving = !Moving;
                if (Moving)
                {
                    inputManager.AddGlobalListener(gameObject);
                    if (SpatialMappingManager.Instance != null)
                    {
                        SpatialMappingManager.Instance.DrawVisualMeshes = true;
                    }
                }
                else
                {
                    inputManager.RemoveGlobalListener(gameObject);
                    if (SpatialMappingManager.Instance != null)
                    {
                        SpatialMappingManager.Instance.DrawVisualMeshes = false;
                    }
                    // Depending on if you are host or client, either setting the SyncVar (host) 
                    // or calling the Cmd (client) will update the other users in the session.
                    // So we have to do both.
                    localPosition = transform.localPosition;
                    localRotation = transform.localRotation;
                    if (PlayerController.Instance != null)
                    {
                        PlayerController.Instance.SendSharedTransform(gameObject, localPosition, localRotation);
                    }
                }

                eventData.Use();
            }
        }
    }
}
