// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental.DialogTest
{
    /// <summary>
    /// This class is used as an example controller to show how to instantiate and launch two different kind of Dialog.
    /// Each one of the public methods are called by the buttons in the scene at the OnClick event.
    /// </summary>
    public class DialogExampleController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Assign DialogLarge_192x192.prefab")]
        private GameObject dialogPrefabLarge;

        /// <summary>
        /// Large Dialog example prefab to display
        /// </summary>
        public GameObject DialogPrefabLarge
        {
            get => dialogPrefabLarge;
            set => dialogPrefabLarge = value;
        }

        [SerializeField]
        [Tooltip("Assign DialogMediume_192x128.prefab")]
        private GameObject dialogPrefabMedium;

        /// <summary>
        /// Medium Dialog example prefab to display
        /// </summary>
        public GameObject DialogPrefabMedium
        {
            get => dialogPrefabMedium;
            set => dialogPrefabMedium = value;
        }

        [SerializeField]
        [Tooltip("Assign DialogSmall_192x96.prefab")]
        private GameObject dialogPrefabSmall;

        /// <summary>
        /// Small Dialog example prefab to display
        /// </summary>
        public GameObject DialogPrefabSmall
        {
            get => dialogPrefabSmall;
            set => dialogPrefabSmall = value;
        }

        /// <summary>
        /// Opens confirmation dialog example
        /// </summary>
        public void OpenConfirmationDialogLarge()
        {
            Dialog.Open(DialogPrefabLarge, DialogButtonType.OK, "Confirmation Dialog, Large, Far", "This is an example of a large dialog with only one button, placed at far interaction range", false);
        }

        /// <summary>
        /// Opens choice dialog example
        /// </summary>
        public void OpenChoiceDialogLarge()
        {
            Dialog myDialog = Dialog.Open(DialogPrefabLarge, DialogButtonType.Yes | DialogButtonType.No, "Choice Dialog, Large, Near", "This is an example of a large dialog with a choice message for the user, placed at near interaction range", true);
            if (myDialog != null)
            {
                myDialog.OnClosed += OnClosedDialogEvent;
            }
        }

        /// <summary>
        /// Opens confirmation dialog example
        /// </summary>
        public void OpenConfirmationDialogMedium()
        {
            Dialog.Open(DialogPrefabMedium, DialogButtonType.OK, "Confirmation Dialog, Medium, Near", "This is an example of a medium dialog with only one button, placed at near interaction range", true);
        }

        /// <summary>
        /// Opens choice dialog example
        /// </summary>
        public void OpenChoiceDialogMedium()
        {
            Dialog myDialog = Dialog.Open(DialogPrefabMedium, DialogButtonType.Yes | DialogButtonType.No, "Choice Dialog, Medium, Far", "This is an example of a medium dialog with a choice message for the user, placed at far interaction range", false);
            if (myDialog != null)
            {
                myDialog.OnClosed += OnClosedDialogEvent;
            }
        }

        /// <summary>
        /// Opens confirmation dialog example
        /// </summary>
        public void OpenConfirmationDialogSmall()
        {
            Dialog.Open(DialogPrefabSmall, DialogButtonType.OK, "Confirmation Dialog, Small, Far", "This is an example of a small dialog with only one button, placed at far interaction range", false);
        }

        /// <summary>
        /// Opens choice dialog example
        /// </summary>
        public void OpenChoiceDialogSmall()
        {
            Dialog myDialog = Dialog.Open(DialogPrefabSmall, DialogButtonType.Yes | DialogButtonType.No, "Choice Dialog, Small, Near", "This is an example of a small dialog with a choice message for the user, placed at near interaction range", true);
            if (myDialog != null)
            {
                myDialog.OnClosed += OnClosedDialogEvent;
            }
        }

        private void OnClosedDialogEvent(DialogResult obj)
        {
            if (obj.Result == DialogButtonType.Yes)
            {
                Debug.Log(obj.Result);
            }
        }
    }
}
