// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Abstract class that presents a Dialog object.
    /// </summary>
    [AddComponentMenu("MRTK/UX/Dialog")]
    public abstract class Dialog : MonoBehaviour
    {
        /// <summary>
        /// The current state of the dialog.
        /// </summary>
        public DialogState State { get; protected set; } = DialogState.Uninitialized;

        /// <summary>
        /// Called after user has clicked a button and the dialog has finished closing.
        /// </summary>
        public Action<DialogProperty> OnClosed { get; set; }

        /// <summary>
        /// Retrieve the properties of the dialog (including the result).
        /// </summary>
        public DialogProperty Property { get; protected set; }

        [SerializeField]
        [HideInInspector]
        private float followMinDistanceNear = 0.25f;

        /// <summary>
        /// The min distance setting on the follow solver attached to this dialog in near interaction placement mode.
        /// </summary>
        [Obsolete("Dialog's ConstantViewSize/variable follow distance is deprecated until Dialog is refactored to render correctly when scaled.")]
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
        [Obsolete("Dialog's ConstantViewSize/variable follow distance is deprecated until Dialog is refactored to render correctly when scaled.")]
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
        [Obsolete("Dialog's ConstantViewSize/variable follow distance is deprecated until Dialog is refactored to render correctly when scaled.")]
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
        [Obsolete("Dialog's ConstantViewSize/variable follow distance is deprecated until Dialog is refactored to render correctly when scaled.")]
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
        [Obsolete("Dialog's ConstantViewSize/variable follow distance is deprecated until Dialog is refactored to render correctly when scaled.")]
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
        [Obsolete("Dialog's ConstantViewSize/variable follow distance is deprecated until Dialog is refactored to render correctly when scaled.")]
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
        protected abstract void GenerateButtons();

        /// <summary>
        /// This is called after the buttons are generated and
        /// the title and message have been set.
        /// Perform here any operations that you'd like
        /// Lays out the buttons on the dialog
        /// E.g. using an ObjectCollection
        /// </summary>
        protected abstract void FinalizeLayout();

        /// <summary>
        /// Set the title and message using the result
        /// E.g. using TextMesh components 
        /// </summary>
        protected abstract void SetTitleAndMessage();

        /// <summary>
        /// Closes the dialog - state is set to Closed
        /// </summary>
        protected virtual void Close() { }

        /// <summary>
        /// Dismisses the Dialog.
        /// </summary>
        /// <param name="destroyDialog">If false the dialog will not be destroyed after being dismissed but instead the GameObject will be disabled</param>
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
        [Obsolete("Dialog's ConstantViewSize/variable follow distance is deprecated until Dialog is refactored to render correctly when scaled.")]
        protected virtual void SetInteractionMode(bool useNearInteractionPlacement) { }
    }
}
