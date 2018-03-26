// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;

namespace ARCA
{
    public class ARMarkerController : MonoBehaviour
    {
        [Tooltip("Background plane")]
        public GameObject BackgroundPlane;

        [Tooltip("GameObject that will contain the code")]
        public GameObject CodeContainer;

        void OnEnable()
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

        void OnDisable()
        {
            OnDismiss();
        }

        public void OnCodeFound()
        {
            Handheld.Vibrate();
            TurnOffMarker();
        }

        void TurnOffMarker()
        {
            foreach(Transform tr in CodeContainer.transform)
            {
                Destroy(tr.gameObject);
            }
                
            CodeContainer.transform.localScale = new Vector3(1,1,1);
            BackgroundPlane.GetComponent<Renderer>().material.color = Color.white;
            gameObject.SetActive(false);
        }
    }
}
