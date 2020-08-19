// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Class which handles displaying and hiding the input simulation help guide
    /// </summary>
    public class InputSimulationHelpGuide : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Keys required to bring up the input display tips")]
        public List<KeyCode> HelpGuideShortcutKeys;

        [SerializeField]
        [Tooltip("The gameobject that displays the shortcut for bringing up the input simulation help guide")]
        public GameObject HelpGuideShortcutTip = null;

        [SerializeField]
        [Tooltip("Whether or not to show the help guide shortcut on startup")]
        public bool DisplayHelpGuideShortcutTipOnStart = true;

        [SerializeField]
        [Tooltip("The game object containing the input simulation help guide")]
        public GameObject HelpGuideVisual = null;

        // Start is called before the first frame update
        void Start()
        {
            string HelpGuideShortcutString = "";
            for(int i = 0; i < HelpGuideShortcutKeys.Count; i++)
            {
                string key = HelpGuideShortcutKeys[i].ToString();
                if (i > 0)
                    HelpGuideShortcutString += " + ";
                HelpGuideShortcutString += key;
            }

            HelpGuideShortcutTip.GetComponentInChildren<TextMeshProUGUI>().text = "Press " + HelpGuideShortcutString + " to open up the input simulation guide";
            if (DisplayHelpGuideShortcutTipOnStart)
            {
                HelpGuideShortcutTip.SetActive(true);
            }
            HelpGuideVisual.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            bool shortcutPressed = true;
            bool shortcutDown = false;

            // Checks to make sure that all keys are pressed and that one of the required shortcut keys was pressed on this frame
            // before bringing up the shortcut
            foreach (KeyCode key in HelpGuideShortcutKeys)
            {
                shortcutPressed &= KeyInputSystem.GetKey(KeyBinding.FromKey(key));
                shortcutDown |= KeyInputSystem.GetKeyDown(KeyBinding.FromKey(key));
            }

            if (shortcutPressed && shortcutDown)
            {
                HelpGuideVisual.SetActive(!HelpGuideVisual.activeSelf);
                HelpGuideShortcutTip.SetActive(false);
            }
        }
    }
}