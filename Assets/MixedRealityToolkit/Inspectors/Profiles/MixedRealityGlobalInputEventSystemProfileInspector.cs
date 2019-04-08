using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CustomEditor(typeof(MixedRealityGlobalInputEventSystemProfile))]
    class MixedRealityGlobalInputEventSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false)) { return; }
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();

            if (GUILayout.Button("Back to Input System Profile Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
            }
            EditorGUILayout.Space();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured()) { return; }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Global Input Event System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Global Input Event Systems allow you to register to input events, without concern of what's in focus.\n\nThis profile is an optional one, and should only be used when trying to create cusom interaction logic.", MessageType.Info);
            EditorGUILayout.Space();

            CheckProfileLock(target);

            serializedObject.Update();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
