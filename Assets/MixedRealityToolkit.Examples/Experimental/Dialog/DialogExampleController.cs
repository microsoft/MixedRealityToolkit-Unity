using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.Dialog;

namespace Microsoft.MixedReality.Examples.Experimental.DialogTest
{
    /// <summary>
    /// This class is used as an example controller to show how to instantiate and launch two different kind of Dialog.
    /// Each one of the public methods are called by the buttons in the scene at the OnClick event.
    /// </summary>
    public class DialogExampleController : MonoBehaviour
    {
        [SerializeField]        
        private GameObject dialogPrefabLarge;
        public GameObject DialogPrefabLarge
        {
            get => dialogPrefabLarge;
            set => dialogPrefabLarge = value;
        }

        [SerializeField]
        private GameObject dialogPrefabMedium;
        public GameObject DialogPrefabMedium
        {
            get => dialogPrefabMedium;
            set => dialogPrefabMedium = value;
        }

        /// <summary>
        /// Opens confirmation dialog example
        /// </summary>
        public void OpenConfirmationDialogLarge()
        {
            Dialog confDialog = Dialog.Open(DialogPrefabLarge, DialogButtonType.OK, "Success", "This is an example of a dialog with only one button", true);
        }

        /// <summary>
        /// Opens choice dialog example
        /// </summary>
        public void OpenChoiceDialogLarge()
        {
            Dialog myDialog = Dialog.Open(DialogPrefabLarge, DialogButtonType.Yes | DialogButtonType.No, "Are You Sure?", "This is an example of a dialog with a choice message for the user", false);
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
            Dialog confDialog = Dialog.Open(DialogPrefabMedium, DialogButtonType.OK, "Success", "This is an example of a dialog with only one button", true);
        }

        /// <summary>
        /// Opens choice dialog example
        /// </summary>
        public void OpenChoiceDialogMedium()
        {
            Dialog myDialog = Dialog.Open(DialogPrefabMedium, DialogButtonType.Yes | DialogButtonType.No, "Are You Sure?", "This is an example of a dialog with a choice message for the user", false);
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
