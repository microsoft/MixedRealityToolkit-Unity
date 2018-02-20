// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Buttons.Utilities;
using MixedRealityToolkit.UX.Dialog;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons
{
    public class DialogButton : MonoBehaviour
    {
        public DialogShell ParentDialog { get; set; }

        public Dialog.Dialog.ButtonTypeEnum ButtonTypeEnum;

        private void OnEnable()
        {
            GetComponent<Button>().OnButtonClicked += OnButtonClicked;
        }

        public void OnButtonClicked(GameObject obj)
        {
            if (ParentDialog != null)
            {
                ParentDialog.Result.Result = ButtonTypeEnum;
                ParentDialog.DismissDialog();
            }
        }

        public void SetTitle(string title)
        {
            CompoundButtonText c = GetComponent<CompoundButtonText>();
            c.Text = title;
        }
    }
}