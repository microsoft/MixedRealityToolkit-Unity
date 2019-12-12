// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro.EditorUtilities;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.Inspectors
{
    [CustomEditor(typeof(SliderInputField))]
    public class SliderInputFieldInspector : TMP_InputFieldEditor
    {
        private const string defaultText = "This is an experimental feature.\n" +
                                           "Parts of the MRTK appear to have a lot of value even if the details " +
                                           "haven’t fully been fleshed out. For these types of features, we want " +
                                           "the community to see them and get value out of them early. Because " +
                                           "they are early in the cycle, we label them as experimental to indicate " +
                                           "that they are still evolving, and subject to change over time.";

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(defaultText, MessageType.Info);

            base.OnInspectorGUI();
        }
    }
}