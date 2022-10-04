// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Basic implementation of a <see cref="IInteractionModeDetector"/>,
    /// which reports the specified hover and select modes whenever the associated
    /// interactor has a valid hover or select target.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Interaction Detector")]
    public class InteractionDetector : MonoBehaviour, IInteractionModeDetector
    {
        [SerializeField]
        [Tooltip("The interactor to listen to.")]
        private XRBaseInteractor interactor;

        /// <summary>
        /// The interactor to listen to.
        /// </summary>
        public XRBaseInteractor Interactor
        {
            get => interactor;
            set => interactor = value;
        }

        [SerializeField]
        [Tooltip("Should this detector set a mode when the specified interactor has a hover target?")]
        private bool detectHover;

        /// <summary>
        /// Should this detector set a mode when the specified interactor has a hover target?
        /// </summary>
        public bool DetectHover
        {
            get => detectHover;
            set => detectHover = value;
        }

        [SerializeField]
        [FormerlySerializedAs("farHoverMode")]
        [Tooltip("The interaction mode to be set when the interactor is hovering over an interactable.")]
        private InteractionMode modeOnHover;

        /// <summary>
        /// The interaction mode to be set when the interactor is hovering over an interactable.
        /// </summary>
        public InteractionMode ModeOnHover
        {
            get => modeOnHover;
            set => modeOnHover = value;
        }

        [SerializeField]
        [Tooltip("Should this detector set a mode when the specified interactor has a selection?")]
        private bool detectSelect;

        /// <summary>
        /// Should this detector set a mode when the specified interactor has a selection?
        /// </summary>
        public bool DetectSelect
        {
            get => detectSelect;
            set => detectSelect = value;
        }

        [SerializeField]
        [FormerlySerializedAs("farSelectMode")]
        [Tooltip("The interaction mode to be set when the interactor is selecting an interactable.")]
        private InteractionMode modeOnSelect;

        /// <summary>
        /// The interaction mode to be set when the interactor is selecting an interactable.
        /// </summary>
        public InteractionMode ModeOnSelect
        {
            get => modeOnSelect;
            set => modeOnSelect = value;
        }

        /// <inheritdoc />
        public InteractionMode ModeOnDetection => GetDetectedMode();

        /// <summary>
        /// Determines which mode should be set.
        /// </summary>
        /// <returns>The detected mode.</returns>
        private InteractionMode GetDetectedMode()
        {
            if (interactor.hasSelection)
            {
                return modeOnSelect;
            }
            else
            {
                return modeOnHover;
            }

        }

        [SerializeField]
        [FormerlySerializedAs("Controllers")]
        [Tooltip("List of GameObjects which represent the 'controllers' that this interaction mode detector has jurisdiction over. Interaction modes will be set on all specified controllers.")]
        private List<GameObject> controllers;

        /// <inheritdoc />
        public List<GameObject> GetControllers() => controllers;

        /// <inheritdoc />
        public bool IsModeDetected()
        {
            bool isDetected = (interactor.hasHover && detectHover) || (interactor.hasSelection && detectSelect);

            // Remove if/when XRI sets hasHover/Selection when their ray interactor is hovering/selecting legacy UI.
            if (interactor is MRTKRayInteractor rayInteractor)
            {
                isDetected |= (rayInteractor.HasUIHover && detectHover) || (rayInteractor.HasUISelection && detectSelect);
            }

            return isDetected;
        }
    }
}
