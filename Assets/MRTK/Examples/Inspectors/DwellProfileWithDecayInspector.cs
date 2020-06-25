// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell.Editor
{
    /// <summary>
    /// Custom profile inspector for the extended dwell profile sample
    /// </summary>
    [CustomEditor(typeof(DwellProfileWithDecay))]
    [Serializable]
    public class DwellProfileWithDecayInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(this.serializedObject, "timeToAllowDwellDecay", "timeToAllowDwellResume");
            DrawConditionalParameter("timeToAllowDwellDecay", "allowDwellDecayOnCancel");

            this.serializedObject.ApplyModifiedProperties();
        }

        public void DrawConditionalParameter(string propertyToDraw, string conditionalProperty)
        {
            var propertyRef = serializedObject.FindProperty(conditionalProperty);
            if (propertyRef.boolValue)
            {
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty(propertyToDraw));
            }
        }
    }
}
