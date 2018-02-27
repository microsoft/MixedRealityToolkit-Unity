// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.UX.Dialog;
using System.Collections;
using UnityEngine;


namespace MixedRealityToolkit.Examples.UX
{
    public class LaunchDialogScript : Interactive
    {
        [SerializeField]
        private Dialog dialogPrefab;
        private bool isDialogLaunched;
        private GameObject resultText;
        private int numButtons;

        public GameObject ResultText
        {
            get
            {
                return resultText;
            }

            set
            {
                resultText = value;
            }
        }

        public int NumButtons { get; set; }

        protected IEnumerator LaunchDialog(Dialog.ButtonTypeEnum buttons, string title, string message)
        {
            isDialogLaunched = true;

            //Open Dialog by sending in prefab...
            Dialog dialog = Dialog.Open(dialogPrefab.gameObject, buttons, title, message);

            //listen for OnClosed Event
            dialog.OnClosed += OnClosed;

            // Wait for dialog to close
            while (dialog.State < Dialog.StateEnum.InputReceived)
            {
                yield return null;
            }

            //only let one dialog be created at a time
            isDialogLaunched = false;

            yield break;
        }

        public override void OnInputClicked(InputClickedEventData eventData)
        {
            if (isDialogLaunched == false)
            {
                if (NumButtons == 1)
                {
                    // Launch Dialog with single button
                    StartCoroutine(LaunchDialog(Dialog.ButtonTypeEnum.OK, "Single Button Dialog", "Dialogs and flyouts are transient UI elements that appear when something happens that requires notification, approval, or additional information from the user."));
                }
                else if (NumButtons == 2)
                {
                    // Launch Dialog with two buttons
                    StartCoroutine(LaunchDialog(Dialog.ButtonTypeEnum.Yes | Dialog.ButtonTypeEnum.No, "Two Buttons Dialog", "Dialogs and flyouts are transient UI elements that appear when something happens that requires notification, approval, or additional information from the user."));
                }
            }
        }

        protected void OnClosed(DialogResult result)
        {
            ResultText.GetComponent<TextMesh>().text = result.Result.ToString();
        }
    }
}
