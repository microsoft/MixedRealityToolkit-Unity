// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public class AnimatablePropertyMenu : EditorWindow
    {
        public bool animatablePropertySelected;

        internal AnimatableProperty animatablePropertyNameSelected;

        private static GUIContent AddButtonLabel;

        /// <summary>
        /// Display the menu if the button is clicked.
        /// </summary>
        public void DisplayMenu()
        {
            AddButtonLabel = new GUIContent(InspectorUIUtility.Plus, "Add");

            if (InspectorUIUtility.FlexButton(AddButtonLabel))
            {
                GenericMenu menu = new GenericMenu();

                CreateStateSelectionMenu(menu);

                menu.ShowAsContext();
            }
        }

        private void CreateStateSelectionMenu(GenericMenu animatablePropertyMenu)
        {
            List<string> coreStylePropertyNames = Enum.GetNames(typeof(AnimatableProperty)).ToList();

            foreach (var animatablePropertyName in coreStylePropertyNames)
            {
                AddAnimatablePropertyToMenu(animatablePropertyMenu, animatablePropertyName, animatablePropertyName);
            }
        }

        private void AddAnimatablePropertyToMenu(GenericMenu menu, string menuPath, string animatablePropertyName)
        {
            menu.AddItem(new GUIContent(menuPath), false, OnStylePropertySelected, animatablePropertyName);
        }

        private void OnStylePropertySelected(object animatablePropertyName)
        {
            animatablePropertySelected = true;

            var enumValues = Enum.GetValues(typeof(AnimatableProperty)).Cast<AnimatableProperty>();

            foreach (var value in enumValues)
            {
                if (value.ToString() == animatablePropertyName.ToString())
                {
                    animatablePropertyNameSelected = value;
                }
            }
        }
    }
}
