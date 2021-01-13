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
    internal class StylePropertyMenu : EditorWindow
    {
        public bool stylePropertySelected;

        internal CoreStyleProperty stylePropertyNameSelected;

        private const string AddStylePropertyButtonLabel = "Add Style Property";

        private static GUIContent AddButtonLabel;

        /// <summary>
        /// 
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

        /// <summary>
        /// 
        /// </summary>
        public void CreateStateSelectionMenu(GenericMenu stylePropertyMenu)
        {
            List<string> coreStylePropertyNames = Enum.GetNames(typeof(CoreStyleProperty)).ToList();

            foreach (var stylePropertyName in coreStylePropertyNames)
            {
                AddStylePropertyToMenu(stylePropertyMenu, stylePropertyName, stylePropertyName);
            }
        }

        private void AddStylePropertyToMenu(GenericMenu menu, string menuPath, string stylePropertyName)
        {
            menu.AddItem(new GUIContent(menuPath), false, OnStylePropertySelected, stylePropertyName);
        }

        private void OnStylePropertySelected(object stylePropertyName)
        {
            stylePropertySelected = true;

            var enumValues = Enum.GetValues(typeof(CoreStyleProperty)).Cast<CoreStyleProperty>();

            foreach(var val in enumValues)
            {
                if (val.ToString() == stylePropertyName.ToString())
                {
                    stylePropertyNameSelected = val;
                }
            }
        }
    }
}
