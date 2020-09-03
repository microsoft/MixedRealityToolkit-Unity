// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.Utilities.InspectorFields.Inspectors
{
    [CustomEditor(typeof(InspectorFieldsExample))]
    public class InspectorFieldsExampleInspector : UnityEditor.Editor
    {
        private SerializedProperty settings;
        private InspectorFieldsExample example;

        private void OnEnable()
        {
            example = (InspectorFieldsExample)target;
            if (example.Settings == null || example.Settings.Count < 1)
            {
                // copy the fields from the class to the virtual list of settings
                example.Settings = InspectorGenericFields<InspectorFieldsExample>.GetSettings(example);
            }

            settings = serializedObject.FindProperty("Settings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // display the fields virtually as a custom inspector
            for (int i = 0; i < settings.arraySize; i++)
            {
                SerializedProperty prop = settings.GetArrayElementAtIndex(i);
                InspectorFieldsUtility.DisplayPropertyField(prop);
            }

            serializedObject.ApplyModifiedProperties();

            // to apply during runtime - only needed for MonoBehaviours
            InspectorGenericFields<InspectorFieldsExample>.LoadSettings(example, example.Settings);
        }
    }
}
