// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A basic convenience registry allowing easy reference
    /// to controllers.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("MRTK/Core/Controller Lookup")]
    public class ControllerLookup : MonoBehaviour
    {
        // Gaze
        [SerializeField]
        [Tooltip("The camera rig's gaze controller.")]
        private XRBaseController gazeController = null;

        /// <summary>
        /// The camera rig's gaze controller.
        /// </summary>
        public XRBaseController GazeController
        {
            get => gazeController;
            set => gazeController = value;
        }

        // Left Hand
        [SerializeField]
        [Tooltip("The camera rig's left hand controller.")]
        private XRBaseController leftHandController = null;

        /// <summary>
        /// The camera rig's left hand controller.
        /// </summary>
        public XRBaseController LeftHandController
        {
            get => leftHandController;
            set => leftHandController = value;
        }

        // Right Hand
        [SerializeField]
        [Tooltip("The camera rig's right hand controller.")]
        private XRBaseController rightHandController = null;

        /// <summary>
        /// The camera rig's right hand controller.
        /// </summary>
        public XRBaseController RightHandController
        {
            get => rightHandController;
            set => rightHandController = value;
        }

        private void OnValidate()
        {
            if (FindObjectsByType<ControllerLookup>(FindObjectsSortMode.None).Length > 1)
            {
                Debug.LogWarning("Found more than one instance of the ControllerLookup class in the hierarchy. There should only be one");
            }
        }
    }
}
