// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.UI.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// The state selection menu for an Interactive Element. Utilized by the BaseInteractiveElementInspector class. 
    /// </summary>
    internal class StateSelectionMenu : EditorWindow
    {
        internal bool stateSelected;
        internal string state;

        private const string Near = "Near";
        private const string Far = "Far";
        private const string Both = "Both";
        private const string SelectStateButtonLabel = "Select State";

        private string touchStateName = CoreInteractionState.Touch.ToString();

        /// <summary>
        /// Display the state selection menu.
        /// </summary>
        public void DisplayMenu(BaseInteractiveElement instance)
        {
            if (GUILayout.Button(SelectStateButtonLabel))
            {
                GenericMenu menu = new GenericMenu();

                CreateStateSelectionMenu(instance, menu);

                menu.ShowAsContext();
            }
        }

        /// <summary>
        /// Add state menu items and sort them by interaction type (Near, Far, Both).
        /// </summary>
        public void CreateStateSelectionMenu(BaseInteractiveElement instance, GenericMenu statesMenu)
        {
            List<string> coreInteractionStateNames = Enum.GetNames(typeof(CoreInteractionState)).ToList();

            // If the state is already being tracked then do not display the state name as an option to add
            foreach (string coreState in coreInteractionStateNames.ToList())
            {
                if (instance.IsStatePresentEditMode(coreState))
                {
                    coreInteractionStateNames.Remove(coreState);
                }
            }

            // Sort the states in the menu based on name
            foreach (var stateName in coreInteractionStateNames)
            {
                // Add special case for touch because it is a near interaction state that does not contain "Near" in the name
                if (stateName.Contains(Near) || stateName == touchStateName)
                {
                    // Near Interaction States
                    AddStateToMenu(statesMenu, Near + "/" + stateName, stateName);
                }
                else if (stateName.Contains(Far))
                {
                    // Far Interaction States
                    AddStateToMenu(statesMenu, Far + "/" + stateName, stateName);
                }
                else if (!stateName.Contains(Far) && !stateName.Contains(Near))
                {
                    // Both Near and Far Interaction States
                    AddStateToMenu(statesMenu, Both + "/" + stateName, stateName);
                }
            }
        }

        // Add a single item to the state selection menu
        private void AddStateToMenu(GenericMenu menu, string menuPath, string stateName)
        {
            menu.AddItem(new GUIContent(menuPath), false, OnStateSelected, stateName);
        }

        // Set internal properties when a state is selected from the menu
        private void OnStateSelected(object stateName)
        {
            stateSelected = true;
            state = stateName.ToString();
        }
    }
}
