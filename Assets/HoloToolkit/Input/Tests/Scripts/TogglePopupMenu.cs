//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TogglePopupMenu : MonoBehaviour
    {

        [SerializeField]
        private PopupMenu popupMenu = null;

        [SerializeField]
        private Button button = null;

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

        private void ShowPopup(Button source)
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