// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class InputSimulationHelpGuide : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Additional distance on top of sphere cast radius when pointer is considered 'near' an object and far interaction will turn off")]
        private GameObject ShortcutTip = null;

        [SerializeField]
        [Tooltip("Additional distance on top of sphere cast radius when pointer is considered 'near' an object and far interaction will turn off")]
        private bool displayShortcutTipOnStart = true;

        [SerializeField]
        [Tooltip("Additional distance on top of sphere cast radius when pointer is considered 'near' an object and far interaction will turn off")]
        private GameObject HelpGuideVisual = null;

        [SerializeField]
        [Tooltip("Additional distance on top of sphere cast radius when pointer is considered 'near' an object and far interaction will turn off")]
        public List<KeyCode> displayShortcut;

        // Start is called before the first frame update
        void Start()
        {
            if (displayShortcutTipOnStart)
            {
                ShortcutTip.SetActive(true);
            }
            HelpGuideVisual.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            bool shortcutPressed = true;
            bool shortcutDown = false;

            foreach (KeyCode key in displayShortcut)
            {
                shortcutPressed &= KeyInputSystem.GetKey(KeyBinding.FromKey(key));
                shortcutDown |= KeyInputSystem.GetKeyDown(KeyBinding.FromKey(key));
            }

            if (shortcutPressed && shortcutDown)
            {
                HelpGuideVisual.transform.parent = CameraCache.Main.transform;
                HelpGuideVisual.transform.rotation = CameraCache.Main.transform.rotation;
                HelpGuideVisual.transform.localPosition = CameraCache.Main.transform.forward * 0.5f + CameraCache.Main.transform.up * 0.1f;

                HelpGuideVisual.transform.parent = null;

                HelpGuideVisual.SetActive(!HelpGuideVisual.activeSelf);
                ShortcutTip.SetActive(false);
            }
        }
    }
}