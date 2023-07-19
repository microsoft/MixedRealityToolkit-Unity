// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Data.Editor
{
    [CustomEditor(typeof(DataConsumerSpriteLookup.ValueToSpriteInfo))]
    public class DataConsumerSpriteInspector : UnityEditor.Editor
    {
        private SerializedProperty valueToSpriteLookup;

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary> 
        private void OnEnable()
        {
            valueToSpriteLookup = serializedObject.FindProperty("valueToSpriteLookup");
        }

        /// <summary>
        /// Called by the Unity editor to render custom inspector UI for this component.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(valueToSpriteLookup, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#pragma warning restore CS1591
