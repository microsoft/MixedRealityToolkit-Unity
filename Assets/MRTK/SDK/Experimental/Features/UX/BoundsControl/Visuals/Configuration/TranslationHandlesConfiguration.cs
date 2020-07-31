// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    /// <summary>
    /// Configuration for <see cref="TranslationHandles"/> used in <see cref="BoundsControl"/>
    /// This class provides all data members needed to create translation handles for <see cref="BoundsControl"/>
    /// </summary>
    [CreateAssetMenu(fileName = "TranslationHandlesConfiguration", menuName = "Mixed Reality Toolkit/Experimental/Bounds Control/Translation Handles Configuration")]
    public class TranslationHandlesConfiguration : HandlesBaseConfiguration
    {

        [SerializeField]
        [Tooltip("Determines the type of collider that will surround the translation handle prefab.")]
        private HandlePrefabCollider translationHandlePrefabColliderType = HandlePrefabCollider.Box;

        /// <summary>
        /// Determines the type of collider that will surround the translation handle prefab.
        /// </summary>
        public HandlePrefabCollider TranslationHandlePrefabColliderType
        {
            get
            {
                return translationHandlePrefabColliderType;
            }
            set
            {
                if (translationHandlePrefabColliderType != value)
                {
                    translationHandlePrefabColliderType = value;
                    colliderTypeChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show translation handles for the X axis")]
        private bool showTranslationHandleForX = true;

        /// <summary>
        /// Check to show translation handles for the X axis
        /// </summary>
        public bool ShowTranslationHandleForX
        {
            get
            {
                return showTranslationHandleForX;
            }
            set
            {
                if (showTranslationHandleForX != value)
                {
                    showTranslationHandleForX = value;
                    handlesChanged.Invoke(HandlesChangedEventType.Visibility);
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show translation handles for the Y axis")]
        private bool showTranslationHandleForY = true;

        /// <summary>
        /// Check to show translation handles for the Y axis
        /// </summary>
        public bool ShowTranslationHandleForY
        {
            get
            {
                return showTranslationHandleForY;
            }
            set
            {
                if (showTranslationHandleForY != value)
                {
                    showTranslationHandleForY = value;
                    handlesChanged.Invoke(HandlesChangedEventType.Visibility);
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show translation handles for the Z axis")]
        private bool showTranslationHandleForZ = true;

        /// <summary>
        /// Check to show translation handles for the Z axis
        /// </summary>
        public bool ShowTranslationHandleForZ
        {
            get
            {
                return showTranslationHandleForZ;
            }
            set
            {
                if (showTranslationHandleForZ != value)
                {
                    showTranslationHandleForZ = value;
                    handlesChanged.Invoke(HandlesChangedEventType.Visibility);
                }
            }
        }

        internal UnityEvent colliderTypeChanged = new UnityEvent();

        /// <summary>
        /// Fabricates an instance of TranslationHandles, applying
        /// this config to it whilst creating it.
        /// </summary>
        /// <returns>New TranslationHandles</returns>
        internal virtual TranslationHandles ConstructInstance()
        {
            // Return a new TranslationHandles, using this config as the active config.
            return new TranslationHandles(this);
        }
    }
}
