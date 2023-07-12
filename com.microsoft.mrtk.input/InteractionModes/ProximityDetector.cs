// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An <see cref="IInteractionModeDetector"/> that detects nearby interactables.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Proximity Detector")]
    public class ProximityDetector : MonoBehaviour, IInteractionModeDetector
    {
        [SerializeField]
        [Tooltip("The interaction manager used to check the presence of interactables on colliders.")]
        private XRInteractionManager interactionManager;

        [SerializeField]
        [Tooltip("The interaction mode to be toggled if when the detector determines it to be active")]
        private InteractionMode modeOnDetection;

        /// <inheritdoc />
        public InteractionMode ModeOnDetection => modeOnDetection;

        /// <inheritdoc />
        /// <remarks>
        /// This is safe to read at any time, even during FixedUpdate. However, as a result of
        /// its internal buffering, it will return true for one extra frame after all objects
        /// have left the detection zone.
        /// </remarks>
        public virtual bool IsModeDetected() => detectedAnythingLastFrame || DetectedAnythingSoFar;

        [SerializeField]
        [FormerlySerializedAs("associatedControllers")]
        [Tooltip("List of GameObjects which represent the 'controllers' that this interaction mode detector has jurisdiction over. Interaction modes will be set on all specified controllers.")]
        private List<GameObject> controllers;

        /// <inheritdoc />
        public List<GameObject> GetControllers() => controllers;

        // Visualizing the proximity zone
        private SphereCollider detectionZone;

        // Was an interactable detected within the proximity zone last physics-tick?
        private bool detectedAnythingLastFrame = false;

        // During this current FixedUpdate cycle, have we yet detected an interactable?
        private bool DetectedAnythingSoFar => colliders.Count > 0;

        // Hashset of colliders with IXRInteractables associated with them.
        private HashSet<Collider> colliders = new HashSet<Collider>();

        /// <summary>
        /// A collection of colliders currently detected by this proximity detector.
        /// Only colliders associated with IXRInteractables are included.
        /// </summary>
        public HashSet<Collider> DetectedColliders => colliders;

        private void Awake()
        {
            if (interactionManager == null)
            {
                interactionManager = ComponentCache<XRInteractionManager>.FindFirstActiveInstance();
            }

            if (interactionManager == null)
            {
                Debug.LogWarning("No interaction manager found in scene. Please add an interaction manager to the scene.");
            }

            detectionZone = GetComponentInChildren<SphereCollider>();
        }

        private void OnTriggerStay(Collider other)
        {
            // Does this collider have an interactable associated with it?
            // We only detect actual interactables, not just all colliders.
            if (interactionManager != null && interactionManager.TryGetInteractableForCollider(other, out _))
            {
                colliders.Add(other);
            }
        }

        private void FixedUpdate()
        {
            detectedAnythingLastFrame = colliders.Count > 0;

            // Wipe the collection so that nothing persists from frame-to-frame.
            colliders.Clear();
        }

        private void OnDrawGizmos()
        {
            if (detectionZone != null)
            {
                Gizmos.color = Color.green - Color.black * 0.8f;
                // Gizmos.DrawSphere(attachTransform.position, detectionZone.radius);
            }
        }
    }
}
