// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    /// <summary>
    /// A custom class used when rendering a Unity inspector window editor for a <see cref="Microsoft.MixedReality.Toolkit.Input.InteractionModeManager">InteractionModeManager</see>
    /// class.
    /// </summary>
    [CustomEditor(typeof(InteractionModeManager))]
    public class InteractionModeManagerEditor : UnityEditor.Editor
    {
        private const string InitControllers = "Init Controllers";
        private const string InitSubtypes = "Populate Modes Definitions with Subtypes";

        /// <summary>
        /// Implemented so to make a custom inspector inside Unity's inspector window.
        /// </summary>
        public override void OnInspectorGUI()
        {
            Color prevColor = GUI.color;
            InteractionModeManager interactionModeManager = (InteractionModeManager)target;

            // Raise lots of errors if the interaction mode manager is configured incorrectly
            var duplicatedNames = interactionModeManager.GetDuplicateInteractionModes();
            if (duplicatedNames.Count > 0)
            {
                var duplicatedNameString = interactionModeManager.CompileDuplicatedNames(duplicatedNames);

                InspectorUIUtility.DrawError($"Duplicate interaction mode definitions detected in the interaction mode manager on {interactionModeManager.gameObject.name}. " +
                                    $"Please check the following interaction modes: {duplicatedNameString}");

                GUI.color = InspectorUIUtility.ErrorColor;
            }

            // Handle the rest of the UI
            base.OnInspectorGUI();
            GUI.color = prevColor;

            if (GUILayout.Button(InitControllers))
            {
                Undo.RecordObject(target, InitControllers);
                interactionModeManager.InitializeControllers();
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            }

            if (GUILayout.Button(InitSubtypes))
            {
                Undo.RecordObject(target, InitSubtypes);
                interactionModeManager.PopulateModesWithSubtypes();
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
