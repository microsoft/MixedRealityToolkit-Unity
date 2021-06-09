// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement.Editor
{
    /// <summary>
    /// The state selection menu for an Interactive Element. Utilized by the BaseInteractiveElementInspector class. 
    /// </summary>
    internal class StateSelectionMenu : EditorWindow
    {
        internal bool stateSelected;
        internal string state;

        private string Near = InteractionType.Near.ToString();
        private string Far = InteractionType.Far.ToString();
        private string NearAndFar = InteractionType.NearAndFar.ToString();
        private string Other = InteractionType.Other.ToString();
        private const string SelectStateButtonLabel = "Select State";

        private string touchStateName = CoreInteractionState.Touch.ToString();
        private string focusStateName = CoreInteractionState.Focus.ToString();

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
                else if (stateName == focusStateName)
                {
                    // Focus is a special case state because it supports both near and far interaction
                    AddStateToMenu(statesMenu, NearAndFar + "/" + stateName, stateName);
                }
                else
                {
                    AddStateToMenu(statesMenu, Other + "/" + stateName, stateName);
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
