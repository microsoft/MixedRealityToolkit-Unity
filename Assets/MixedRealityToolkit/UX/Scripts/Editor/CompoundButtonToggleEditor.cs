using MixedRealityToolkit.Utilities.Inspectors;

namespace MixedRealityToolkit.UX.Buttons
{
        [UnityEditor.CustomEditor(typeof(CompoundButtonToggle))]
        public class CompoundButtonToggleEditor : MRTKEditor
        {
            protected override void DrawCustomFooter() {
                CompoundButtonToggle toggle = (CompoundButtonToggle)target;

                FieldInfo fieldInfo = null;
                Type profileType = null;
                if (toggle.Target == null) {
                    DrawError("Target must be set.");
                    return;
                } else {

                    fieldInfo = toggle.Target.GetType().GetField("Profile");

                    if (fieldInfo == null) {
                        DrawError("Target component has no 'Profile' field - are you use this class inherits from ProfileButtonBase?");
                        return;
                    }

                    GUIStyle labelStyle = new GUIStyle(UnityEditor.EditorStyles.label);
                    labelStyle.fontSize = 18;
                    labelStyle.fontStyle = FontStyle.Bold;

                    profileType = fieldInfo.FieldType;
                    UnityEditor.EditorGUILayout.LabelField("Type: " + toggle.Target.GetType().Name + " / " + fieldInfo.FieldType.Name, labelStyle, GUILayout.MinHeight(24));

                }

                UnityEditor.EditorGUILayout.LabelField("Select on/off profiles of the type " + profileType.Name);
                if (toggle.OnProfile == null) {
                    toggle.OnProfile = (ButtonProfile)fieldInfo.GetValue(toggle.Target);
                }
                if (toggle.OffProfile == null) {
                    toggle.OffProfile = toggle.OnProfile;
                }
                ButtonProfile onProfile = (ButtonProfile)UnityEditor.EditorGUILayout.ObjectField("On Profile", toggle.OnProfile, typeof(ButtonProfile), false);
                ButtonProfile offProfile = (ButtonProfile)UnityEditor.EditorGUILayout.ObjectField("Off Profile", toggle.OffProfile, typeof(ButtonProfile), false);
                if (onProfile.GetType() == profileType) {
                    toggle.OnProfile = onProfile;
                }
                if (offProfile.GetType() == profileType) {
                    toggle.OffProfile = offProfile;
                }

                if (toggle.OnProfile.GetType() != profileType) {
                    DrawError("On profile object does not match type " + profileType.Name);
                }
                if (toggle.OffProfile.GetType() != profileType) {
                    DrawError("Off profile object does not match type " + profileType.Name);
                }

                if (onProfile == offProfile) {
                    DrawWarning("Profiles are the same - toggle will have no effect");
                }

                toggle.Behavior = (ToggleBehaviorEnum)UnityEditor.EditorGUILayout.EnumPopup("Toggle behavior", toggle.Behavior);
            }
        }
}