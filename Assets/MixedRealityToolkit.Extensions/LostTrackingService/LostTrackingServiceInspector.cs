// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking.Editor
{
    /// <summary>
    /// The custom inspector for an <see cref="ILostTrackingService"/>.
    /// </summary>
    [MixedRealityServiceInspector(typeof(ILostTrackingService))]
    public class LostTrackingServiceInspector : BaseMixedRealityServiceInspector
    {
        public override void DrawInspectorGUI(object target)
        {
            LostTrackingService service = (LostTrackingService)target;

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            EditorGUILayout.Toggle("Tracking Lost", service.TrackingLost);

            EditorGUILayout.LabelField("Editor Testing", EditorStyles.boldLabel);
            if (service.TrackingLost)
            {
                if (GUILayout.Button("Set Tracking Restored"))
                {
                    service.EditorSetTrackingLost(false);
                }
            }
            else
            {
                if (GUILayout.Button("Set Tracking Lost"))
                {
                    service.EditorSetTrackingLost(true);
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif