// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Example of using InspectorFields attributes in a class to create custom inspectors
    /// This is on approach for building complex inspectors that need to be customized or the need to overcome lack of polymorphism support
    /// They provide a way to create one inspector for multiple classes
    /// Example: Create a MonoBehaviour or scriptable object with a custom inspector.
    /// The functionality or settings can be changed by assigning a custom script to the object
    /// Use InspectorFields to render the custom properties inside the custom script in the inspector
    /// When the app launches, copy the properties to the new instance of the script
    /// An example of this can be found in Interactables Receivers.
    /// Each Receiver is a custom class that renders their properties in the Interactables custom inspector
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/InspectorFieldsExample")]
    public class InspectorFieldsExample : MonoBehaviour
    {
        [InspectorField(Label = "Component Name", Tooltip = "The name of the component", Type = InspectorField.FieldTypes.String)]
        public string ComponentName = "My Name";

        [InspectorField(Label = "Enabled", Tooltip = "Is the component enabled?", Type = InspectorField.FieldTypes.Bool)]
        public bool Enabled;

        [InspectorField(Label = "Component Option", Tooltip = "Select an option", Type = InspectorField.FieldTypes.DropdownString, Options = new string[] { "Option 1", "Option 2", "Option 3", "Option 4" })]
        public string ComponentOption = "Option 3";

        [InspectorField(Label = "Component Index", Tooltip = "A index value of the component", Type = InspectorField.FieldTypes.DropdownInt, Options = new string[] { "Index 0", "Index 1", "Index 2", "Index 3", "Index 4" })]
        public int ComponentIndex = 2;

        /// <summary>
        /// A holder for the InpsectorFields as a list
        /// The inspector will update these settings while in the editor
        /// </summary>
        [HideInInspector]
        public List<InspectorPropertySetting> Settings;

        private void Awake()
        {
            // copy the virtual property settings values to the actual properties
            // this can be done on awake or when creating a new instance of a class
            InspectorGenericFields<InspectorFieldsExample>.LoadSettings(this, Settings);
        }

        private void Update()
        {
            print("Name: " + ComponentName + ", Enabled: " + Enabled + ", Option: " + ComponentOption + ",  Index: " + ComponentIndex);
        }
    }
}
