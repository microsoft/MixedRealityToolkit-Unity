// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Teleport;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [MixedRealityServiceInspector(typeof(IMixedRealityTeleportSystem))]
    public class TeleportSystemInspector : BaseMixedRealityServiceInspector
    {
        private static readonly Color enabledColor = GUI.backgroundColor;
        private static readonly Color disabledColor = Color.Lerp(enabledColor, Color.clear, 0.5f);

        public override void DrawInspectorGUI(object target)
        {
            IMixedRealityTeleportSystem teleport = (IMixedRealityTeleportSystem)target;

            EditorGUILayout.LabelField("Event Listeners", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Event listeners will be populated once you enter play mode.", MessageType.Info);
                return;
            }

            if (teleport.EventListeners.Count == 0)
            {
                EditorGUILayout.LabelField("(None found)", EditorStyles.miniLabel);
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                foreach (GameObject listener in teleport.EventListeners)
                {
                    EditorGUILayout.ObjectField(listener.name, listener, typeof(GameObject), true);
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}