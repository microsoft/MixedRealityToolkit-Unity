// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Class which handles displaying and hiding the input simulation help guide
    /// </summary>
    internal class InputSimulationHelpGuide : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("HelpGuideShortcutKeys")]
        [Tooltip("Keys required to bring up the input display tips")]
        private List<KeyCode> helpGuideShortcutKeys = new List<KeyCode>(0);

        [SerializeField, FormerlySerializedAs("HelpGuideShortcutTip")]
        [Tooltip("The GameObject that displays the shortcut for bringing up the input simulation help guide")]
        private GameObject helpGuideShortcutTip = null;

        [SerializeField, FormerlySerializedAs("DisplayHelpGuideShortcutTipOnStart")]
        [Tooltip("Whether or not to show the help guide shortcut on startup")]
        private bool displayHelpGuideShortcutTipOnStart = true;

        [SerializeField, FormerlySerializedAs("HelpGuideVisual")]
        [Tooltip("The GameObject containing the input simulation help guide")]
        private GameObject helpGuideVisual = null;

        private void Start()
        {
            if (DeviceUtility.IsPresent)
            {
                gameObject.SetActive(false);
                return;
            }

            string HelpGuideShortcutString = "";
            for (int i = 0; i < helpGuideShortcutKeys.Count; i++)
            {
                string key = helpGuideShortcutKeys[i].ToString();
                if (i > 0)
                {
                    HelpGuideShortcutString += " + ";
                }
                HelpGuideShortcutString += key;
            }

            helpGuideShortcutTip.GetComponentInChildren<TextMeshProUGUI>().text = "Press " + HelpGuideShortcutString + " to open up the input simulation guide";
            if (displayHelpGuideShortcutTipOnStart)
            {
                helpGuideShortcutTip.SetActive(true);
            }
            helpGuideVisual.SetActive(false);
        }

        private void Update()
        {
            bool shortcutPressed = true;
            bool shortcutDown = false;

            // Checks to make sure that all keys are pressed and that one of the required shortcut keys was pressed on this frame
            // before bringing up the shortcut
            foreach (KeyCode key in helpGuideShortcutKeys)
            {
                shortcutPressed &= KeyInputSystem.GetKey(KeyBinding.FromKey(key));
                shortcutDown |= KeyInputSystem.GetKeyDown(KeyBinding.FromKey(key));
            }

            if (shortcutPressed && shortcutDown)
            {
                helpGuideVisual.SetActive(!helpGuideVisual.activeSelf);
                helpGuideShortcutTip.SetActive(false);
            }
        }
    }
}