// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Controls displaying of the AR marker on the mobile device
    /// </summary>
    public class ARMarkerController : MonoBehaviour
    {
        /// <summary>
        /// Background plane
        /// </summary>
        [Tooltip("Background plane")]
        [SerializeField]
        private GameObject backgroundPlane;
        /// <summary>
        /// Background plane
        /// </summary>
        public GameObject BackgroundPlane
        {
            get
            {
                return backgroundPlane;
            }

            set
            {
                backgroundPlane = value;
            }
        }

        /// <summary>
        /// GameObject that will contain the code
        /// </summary>
        [Tooltip("GameObject that will contain the code")]
        [SerializeField]
        private GameObject codeContainer;
        /// <summary>
        /// GameObject that will contain the code
        /// </summary>
        public GameObject CodeContainer
        {
            get
            {
                return codeContainer;
            }

            set
            {
                codeContainer = value;
            }
        }

        private void OnEnable()
        {
            // Setting screen rotation to portrait when dispalying AR code
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
        }

        private void OnDismiss()
        {
            // Setting screen rotation to autorotation when AR code is dismissed
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
            }
        }

        private void OnDestroy()
        {
            OnDismiss();
        }

        private void OnDisable()
        {
            OnDismiss();
        }

        /// <summary>
        /// Called on mobile when the HoloLens finds the marker
        /// </summary>
        public void OnCodeFound()
        {
            Handheld.Vibrate();
            TurnOffMarker();
        }

        /// <summary>
        /// Turns off the markers visuals, executed on the mobile
        /// </summary>
        private void TurnOffMarker()
        {
            foreach(Transform tr in CodeContainer.transform)
            {
                Destroy(tr.gameObject);
            }

            CodeContainer.transform.localScale = Vector3.one;
            BackgroundPlane.GetComponent<Renderer>().sharedMaterial.color = Color.white;
            gameObject.SetActive(false);
        }
    }
}
