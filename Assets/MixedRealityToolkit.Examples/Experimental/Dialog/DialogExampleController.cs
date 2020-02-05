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
        private GameObject dialogPrefab;
        public GameObject DialogPrefab
        {
            get => dialogPrefab;
            set => dialogPrefab = value;
        }

        public void OpenConfirmationDialog()
        {
            Dialog confDialog = Dialog.Open(DialogPrefab, DialogButtonType.OK, "Success", "This is an example of a dialog with only one button");
        }

        public void OpenChoiceDialog()
        {
            Dialog myDialog = Dialog.Open(DialogPrefab, DialogButtonType.Yes | DialogButtonType.Cancel, "Are You Sure?", "This is an example of a dialog with a choice message for the user");
            if (myDialog != null)
            {
                myDialog.OnClosed += OnClosedDialogEvent;
            }
        }

        private void OnClosedDialogEvent(DialogResult obj)
        {
            if (obj.Result == DialogButtonType.Yes)
            {
                //The user chose the "Yes" button            
            }
        }
    }
}
