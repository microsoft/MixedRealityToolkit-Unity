// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell.Editor
{
    /// <summary>
    /// Custom profile inspector for the extended dwell profile sample
    /// </summary>
    [CustomEditor(typeof(DwellProfileWithDecay))]
    [Serializable]
    public class DwellProfileWithDecayInspector : DwellProfileInspector
    {
        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(this.serializedObject, "timeToAllowDwellDecay", "timeToAllowDwellResume");
            DrawConditionalParameter("timeToAllowDwellResume", "allowDwellResume");
            DrawConditionalParameter("timeToAllowDwellDecay", "allowDwellDecayOnCancel");

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}