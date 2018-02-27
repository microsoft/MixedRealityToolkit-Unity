// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

namespace MixedRealityToolkit.Examples.UX
{
    public class DialogExamples : MonoBehaviour
    {
        [Header("Test buttons size optimization")]
        [SerializeField]
        private Vector3 iHMDScalar = new Vector3(1.3f, 1.3f, 1);

        [SerializeField]
        private GameObject buttonCollection = null;


        public GameObject ButtonCollection
        {
            get
            {
                return buttonCollection;
            }

            set
            {
                buttonCollection = value;
            }
        }

        public Vector3 IHMDScalar
        {
            get
            {
                return iHMDScalar;
            }

            set
            {
                iHMDScalar = value;
            }
        }

        private void Start()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Optimize the content for immersive headset
            if (HolographicSettings.IsDisplayOpaque)
            {
               buttonCollection.transform.localScale = IHMDScalar;
            }
#endif
        }
    }
}
