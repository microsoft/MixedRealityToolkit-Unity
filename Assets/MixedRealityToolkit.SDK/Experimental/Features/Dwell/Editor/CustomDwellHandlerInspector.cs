// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    [CustomEditor(typeof(CustomDwellHandler))]
    public class CustomDwellHandlerInspector : DwellHandlerInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}