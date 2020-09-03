// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Represents a position where a <see cref="Dockable"/> object can be docked.
    /// This component also adds a Collider and a Rigidbody, if they're not already present.
    /// </summary>
    /// <seealso cref="Dock"/>
    /// <seealso cref="Dockable"/>
    [AddComponentMenu("Scripts/MRTK/Experimental/Dock/DockPosition")]
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class DockPosition : MonoBehaviour
    {
        /// <summary>
        /// The object that is currently docked in this position (can be null).
        /// </summary>
        [Experimental]
        [SerializeField]
        [Tooltip("The object that is currently docked in this position (can be null).")]
        private Dockable dockedObject = null;

        /// <summary>
        /// The object that is currently docked in this position (can be null).
        /// </summary>
        public Dockable DockedObject
        {
            get => dockedObject;
            set => dockedObject = value;
        }

        /// <summary>
        /// True if this position is occupied, false otherwise.
        /// </summary>
        public bool IsOccupied => dockedObject != null;

        /// <summary>
        /// Ensure this object has a triggering collider, and ensure that
        /// this object doesn't block manipulations.
        /// </summary>
        public void Awake()
        {
            // Don't raycast this object to prevent blocking collisions
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            // Ensure there's a trigger collider for this position
            // The shape can be customized, but this adds a box as default.
            var collider = gameObject.GetComponent<Collider>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<BoxCollider>();
            }

            collider.isTrigger = true;

            // Ensure this collider can be used as a trigger by having
            // a RigidBody attached to it.
            var rigidBody = gameObject.EnsureComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }

        /// <summary>
        /// If an object was set to be docked to this at start up, ensure it's docked.
        /// </summary>
        public void Start()
        {
            if (dockedObject != null)
            {
                dockedObject.Dock(this);
            }
        }
    }
}
