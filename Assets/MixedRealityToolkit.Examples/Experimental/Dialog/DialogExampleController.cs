using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.Dialog;

namespace Microsoft.MixedReality.Examples.Experimental.DialogTest
{
    public class DialogExampleController : MonoBehaviour
    {

        public GameObject dialogPrefab;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OpenConfirmationDialog()
        {
            Dialog confDialog = Dialog.Open(dialogPrefab, DialogButtonType.OK, "Success", "This is an example of a dialog with only one button");
        }

        public void OpenChoiceDialog()
        {
            var myVariable = new Tuple<string, GameObject>(gameObject.name, gameObject);
            Dialog myDialog = Dialog.Open(dialogPrefab, DialogButtonType.Yes | DialogButtonType.Cancel, "Are You Sure?", "This is an example of a dialog with a choice message for the user", myVariable);
            if (myDialog != null)
            {
                myDialog.OnClosed += OnClosedDialogEvent;
            }
        }

        private void OnClosedDialogEvent(DialogResult obj)
        {
            if (obj.Result == DialogButtonType.Yes)
            {
                //The user choise the "Yes" button            
            }
        }
    }
}