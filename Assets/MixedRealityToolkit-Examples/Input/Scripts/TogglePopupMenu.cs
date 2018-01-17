// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace MixedRealityToolkit.Examples.InputModule
{
    public class TogglePopupMenu : MonoBehaviour
    {

        [SerializeField]
        private PopupMenu popupMenu = null;

        [SerializeField]
        private TestButton button = null;

        private void Awake()
        {
            if (button)
            {
                button.Activated += ShowPopup;
            }
        }

        private void OnDisable()
        {
            if (button)
            {
                button.Activated -= ShowPopup;
            }
        }

        private void ShowPopup(TestButton source)
        {
            if (popupMenu != null)
            {
                if (popupMenu.CurrentPopupState == PopupMenu.PopupState.Closed)
                {
                    popupMenu.Show();

                    StartCoroutine(WaitForPopupToClose());
                }
            }
        }

        private IEnumerator WaitForPopupToClose()
        {
            if (popupMenu)
            {
                while (popupMenu.CurrentPopupState == PopupMenu.PopupState.Open)
                {
                    yield return null;
                }
            }

            if (button)
            {
                button.Selected = false;
            }
        }
    }
}