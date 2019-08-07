//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(ManipulationHandler))]
    [CanEditMultipleObjects]
    public class ManipulationHandlerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (target != null)
            {
                InspectorUIUtility.RenderHelpURL(target.GetType());
            }

            DrawDefaultInspector();
        }
    }
}
