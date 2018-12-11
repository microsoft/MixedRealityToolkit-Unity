// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.CustomExtensionServices.Inspectors
{
    /// <summary>
    /// The custom inspector for your <see cref="DemoCustomExtensionServiceProfile"/>.
    /// This is where you can create your own inspector for what you see in the profile.
    /// </summary>
    [CustomEditor(typeof(DemoCustomExtensionServiceProfile))]
    public class DemoCustomExtensionServiceProfileInspector : BaseMixedRealityProfileInspector
    {
        private SerializedProperty myCustomStringData;

        protected override void OnEnable()
        {
            // Call base on enable so we can get proper
            // copy/paste functionality of the profile
            base.OnEnable();

            // We check to make sure the MRTK is configured here.
            // Pass false so we don't get an error for showing the help box.
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            myCustomStringData = serializedObject.FindProperty("myCustomStringData");
        }

        public override void OnInspectorGUI()
        {
            // We check to make sure the MRTK is configured here.
            // This will show an error help box if it's not.
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(myCustomStringData);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
