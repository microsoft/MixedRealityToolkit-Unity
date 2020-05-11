// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CustomEditor(typeof(NearInteractionTouchableVolume))]
    public class NearInteractionTouchableVolumeInspector : UnityEditor.Editor
    {
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            NearInteractionTouchableVolume t = target as NearInteractionTouchableVolume;
            Collider c = t.GetComponent<Collider>();
            if (c == null)
            {
                EditorGUILayout.HelpBox("A collider is required in order to compute the touchable volume.", MessageType.Warning);
            }
        }
    }
}
