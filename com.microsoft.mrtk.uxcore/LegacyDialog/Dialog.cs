// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Deprecated
{
    /// <summary>
    /// Abstract class that presents a Dialog object.
    /// </summary>
    /// <remarks>
    /// The <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> is deprecated. Please migrate to the 
    /// new <see cref="Microsoft.MixedReality.Toolkit.UX.Dialog">Dialog</see>. If you'd like to continue using the 
    /// <see cref="Microsoft.MixedReality.Toolkit.UX.Deprecated.Dialog">Legacy Dialog</see> implementation, it is recommended that the legacy code 
    /// be copied to the application's code base, and maintained independently by the application developer. Otherwise, it is strongly recommended 
    /// that the application be updated to use the new <see cref="Microsoft.MixedReality.Toolkit.UX.DialogPool">DialogPool</see> system.
    /// </remarks>
    [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
    public abstract class Dialog : MonoBehaviour
    {
        /// <summary>
        /// The current state of the dialog.
        /// </summary>
        [Obsolete("This legacy dialog system has been deprecated. Please migrate to the new dialog system, see Microsoft.MixedReality.Toolkit.UX.DialogPool for more details.")]
        public DialogState State { get; protected set; } = DialogState.Uninitialized;

        /// <summary>
        /// Called after user has clicked a button and the dialog has finished closing.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public Action<DialogProperty> OnClosed { get; set; }

        /// <summary>
        /// Retrieve the properties of the dialog (including the result).
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public DialogProperty Property { get; protected set; }

        [SerializeField]
        [HideInInspector]
        private float followMinDistanceNear = 0.25f;

        /// <summary>
        /// The min distance setting on the follow solver attached to this dialog in near interaction placement mode.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public float FollowMinDistanceNear
        {
            get => followMinDistanceNear;
            set => followMinDistanceNear = value;
        }

        [SerializeField]
        [HideInInspector]
        private float followMaxDistanceNear = 0.6f;

        /// <summary>
        /// The max distance setting on the follow solver attached to this dialog in near interaction placement mode.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public float FollowMaxDistanceNear
        {
            get => followMaxDistanceNear;
            set => followMaxDistanceNear = value;
        }

        [SerializeField]
        [HideInInspector]
        private float followDefaultDistanceNear = 0.4f;

        /// <summary>
        /// The default distance setting on the follow solver attached to this dialog in near interaction placement mode.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public float FollowDefaultDistanceNear
        {
            get => followDefaultDistanceNear;
            set => followDefaultDistanceNear = value;
        }

        [SerializeField]
        [HideInInspector]
        private float followMinDistanceFar = 1f;

        /// <summary>
        /// The min distance setting on the follow solver attached to this dialog in far interaction placement mode.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public float FollowMinDistanceFar
        {
            get => followMinDistanceFar;
            set => followMinDistanceFar = value;
        }

        [SerializeField]
        [HideInInspector]
        private float followMaxDistanceFar = 1.5f;

        /// <summary>
        /// The max distance setting on the follow solver attached to this dialog in far interaction placement mode.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public float FollowMaxDistanceFar
        {
            get => followMaxDistanceFar;
            set => followMaxDistanceFar = value;
        }

        [SerializeField]
        [HideInInspector]
        private float followDefaultDistanceFar = 1.2f;

        /// <summary>
        /// The default distance setting on the follow solver attached to this dialog in far interaction placement mode.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public float FollowDefaultDistanceFar
        {
            get => followDefaultDistanceFar;
            set => followDefaultDistanceFar = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool placeForNearInteraction = true;

        /// <summary>
        /// Use placement optimized for near interaction instead of far interaction.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public bool PlaceForNearInteraction
        {
            get => placeForNearInteraction;
            set
            {
                if (placeForNearInteraction != value)
                {
                    SetInteractionMode(value);
                    placeForNearInteraction = value;
                }
            }
        }

        /// <summary>
        /// Generates buttons - Must parent them under buttonParent!
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        protected abstract void GenerateButtons();

        /// <summary>
        /// This is called after the buttons are generated and
        /// the title and message have been set.
        /// Perform here any operations that you'd like
        /// Lays out the buttons on the dialog
        /// E.g. using an ObjectCollection
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        protected abstract void FinalizeLayout();

        /// <summary>
        /// Set the title and message using the result
        /// E.g. using TextMesh components 
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        protected abstract void SetTitleAndMessage();

        /// <summary>
        /// Closes the dialog - state is set to Closed
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        protected virtual void Close() { }

        /// <summary>
        /// Dismisses the Dialog.
        /// </summary>
        /// <param name="destroyDialog">If false the dialog will not be destroyed after being dismissed but instead the GameObject will be disabled</param>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public void Dismiss(bool destroyDialog = true)
        {
            Close();
            OnClosed?.Invoke(Property);
            State = DialogState.Closed;
            if (destroyDialog)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Opens a dialog
        /// </summary>
        /// <param name="property">DialogProperty class object which contains information such as title and description text</param>
        /// <param name="placeForNearInteraction">Use placement optimized for near interaction instead of far interaction</param>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public virtual void Open(DialogProperty property = null, bool? placeForNearInteraction = null)
        {
            State = DialogState.Opening;
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            if (property != null)
            {
                Property = property;
                Property.TargetDialog = this;
            }
            else if (Property == null)
            {
                Debug.LogError("Cannot open the dialog because the property is not specified");
                return;
            }
            if (placeForNearInteraction != null)
            {
                PlaceForNearInteraction = placeForNearInteraction.GetValueOrDefault();
            }
            // Create buttons and set up message
            GenerateButtons();
            SetTitleAndMessage();
            FinalizeLayout();
            State = DialogState.WaitingForInput;
        }

        /// <summary>
        /// Instantiates a dialog using the parameters
        /// </summary>
        /// <param name="dialogComponentOnPrefab">The dialog component on a Dialog prefab</param>
        /// <param name="property">DialogProperty class object which contains information such as title and description text</param>
        /// <param name="placeForNearInteraction">Use placement optimized for near interaction instead of far interaction</param>
        /// <param name="openOnInstantiate">Whether the dialog should be opened now</param>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        public static Dialog InstantiateFromPrefab(Dialog dialogComponentOnPrefab, DialogProperty property = null, bool placeForNearInteraction = true, bool openOnInstantiate = false)
        {
            GameObject dialogGameObject = Instantiate(dialogComponentOnPrefab.gameObject);

            Dialog dialog = dialogGameObject.GetComponent<Dialog>();
            if (property != null)
            {
                dialog.Property = property;
                dialog.Property.TargetDialog = dialog;
            }

            dialog.PlaceForNearInteraction = placeForNearInteraction;

            if (openOnInstantiate)
            {
                if (property == null)
                {
                    Debug.LogError("Cannot open the dialog because the property is not specified");
                    dialogGameObject.SetActive(false);
                    return dialog;
                }
                dialog.Open();
            }
            else
            {
                dialogGameObject.SetActive(false);
            }

            return dialog;
        }
        
        protected virtual void Awake()
        {
            SetInteractionMode(PlaceForNearInteraction);
        }

        /// <summary>
        /// Apply settings to the follow solver based on the selected interaction mode.
        /// </summary>
        [Obsolete("Legacy Dialog is deprecated. Please migrate to the new Dialog. See uxcore/LegacyDialog/README.md")]
        protected virtual void SetInteractionMode(bool useNearInteractionPlacement) { }
    }
}
