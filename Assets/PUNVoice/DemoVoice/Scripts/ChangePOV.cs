// ----------------------------------------------------------------------------
// <copyright file="ChangePOV.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// "Camera manager" class that handles the switch between the three different cameras.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace ExitGames.Demos.DemoPunVoice {

    using UnityEngine;
    using UnityEngine.UI;

    public class ChangePOV : MonoBehaviour {
        private FirstPersonController firstPersonController;
        private ThirdPersonController thirdPersonController;
        private OrthographicController orthographicController;

        private Vector3 initialCameraPosition;
        private Quaternion initialCameraRotation;
        private Camera defaultCamera;

        [SerializeField]
        private GameObject ButtonsHolder;

        [SerializeField]
        private Button FirstPersonCamActivator;

        [SerializeField]
        private Button ThirdPersonCamActivator;

        [SerializeField]
        private Button OrthographicCamActivator;

        public delegate void OnCameraChanged(Camera newCamera);

        public static event OnCameraChanged CameraChanged;

        private void OnEnable() {
            CharacterInstantiation.CharacterInstantiated += OnCharacterInstantiated;
        }

        private void OnDisable() {
            CharacterInstantiation.CharacterInstantiated -= OnCharacterInstantiated;
        }


        private void Start() {
            defaultCamera = Camera.main;
            initialCameraPosition = new Vector3(defaultCamera.transform.position.x,
                defaultCamera.transform.position.y, defaultCamera.transform.position.z);
            initialCameraRotation = new Quaternion(defaultCamera.transform.rotation.x,
                defaultCamera.transform.rotation.y, defaultCamera.transform.rotation.z,
                defaultCamera.transform.rotation.w);
            //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
            FirstPersonCamActivator.onClick.AddListener(FirstPersonMode);
#else
            FirstPersonCamActivator.gameObject.SetActive(false);
#endif
            ThirdPersonCamActivator.onClick.AddListener(ThirdPersonMode);
            OrthographicCamActivator.onClick.AddListener(OrthographicMode);
        }

        private void OnCharacterInstantiated(GameObject character) {
            firstPersonController = character.GetComponent<FirstPersonController>();
            firstPersonController.enabled = false;
            thirdPersonController = character.GetComponent<ThirdPersonController>();
            thirdPersonController.enabled = false;
            orthographicController = character.GetComponent<OrthographicController>();
            ButtonsHolder.SetActive(true);
        }

        private void OnLeftRoom() {
            if (defaultCamera == null)
            {
                defaultCamera = Camera.main;
            }
            defaultCamera.gameObject.SetActive(true);
            FirstPersonCamActivator.interactable = true;
            ThirdPersonCamActivator.interactable = true;
            OrthographicCamActivator.interactable = false;
            defaultCamera.transform.position = initialCameraPosition;
            defaultCamera.transform.rotation = initialCameraRotation;
            ButtonsHolder.SetActive(false);
        }

        private void FirstPersonMode() {
            ToggleMode(firstPersonController);
        }

        private void ThirdPersonMode() {
            ToggleMode(thirdPersonController);
        }

        private void OrthographicMode() {
            ToggleMode(orthographicController);
        }

        private void ToggleMode(BaseController controller) {
            if (controller == null) { return; } // this should not happen, throw error
            if (controller.ControllerCamera == null) { return; } // probably game is closing 
            controller.ControllerCamera.gameObject.SetActive(true);
            controller.enabled = true;
            FirstPersonCamActivator.interactable = !(controller == firstPersonController);
            ThirdPersonCamActivator.interactable = !(controller == thirdPersonController);
            OrthographicCamActivator.interactable = !(controller == orthographicController);
            BroadcastChange(controller.ControllerCamera); // BroadcastChange(Camera.main);
        }

        private void BroadcastChange(Camera camera) {
            if (camera == null) { return; } // should not happen, throw error
            if (CameraChanged != null) {
                CameraChanged(camera);
            }
        }
    }
}