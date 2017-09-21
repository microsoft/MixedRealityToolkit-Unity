using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using HoloToolkit.Unity.SpatialMapping;

namespace HoloToolkit.Unity.SharingWithUNET
{
    public class UNetSharedHologram : NetworkBehaviour, IInputClickHandler
    {

        /// <summary>
        /// The position relative to the shared world anchor.
        /// </summary>
        [SyncVar(hook = "xformchange")]
        private Vector3 localPosition;

        void xformchange(Vector3 update)
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

        bool Moving = false;
        int layerMask;
        InputManager inputManager;
        public Vector3 movementOffset = Vector3.zero;
        bool isOpaque;

        // Use this for initialization
        void Start()
        {
            isOpaque = UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque;
            transform.SetParent(SharedCollection.Instance.transform, true);
            if (isServer)
            {
                localPosition = transform.localPosition;
                localRotation = transform.localRotation;
            }

            layerMask = HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance.LayerMask;
            inputManager = InputManager.Instance;

        }

        // Update is called once per frame
        void Update()
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

        Vector3 ProposeTransformPosition()
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
                        PlayerController.Instance.SendSharedTransform(this.gameObject, localPosition, localRotation);
                    }
                }

                eventData.Use();
            }
        }
    }
}
