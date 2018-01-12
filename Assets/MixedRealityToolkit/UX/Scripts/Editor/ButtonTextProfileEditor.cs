using MixedRealityToolkit.Utilities.Inspectors;

namespace MixedRealityToolkit.UX.Buttons
{
        [UnityEditor.CustomEditor(typeof(ButtonTextProfile))]
        public class ButtonTextProfileEditor : ProfileInspector {
            protected override void DrawCustomFooter() {
                ButtonTextProfile textProfile = (ButtonTextProfile)target;
                CompoundButtonText textButton = (CompoundButtonText)targetComponent;

                if (textButton == null || !textButton.OverrideOffset) {
                    switch (textProfile.Anchor) {
                        case TextAnchor.LowerCenter:
                            textProfile.AnchorLowerCenterOffset = UnityEditor.EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorLowerCenterOffset);
                            break;

                        case TextAnchor.LowerLeft:
                            textProfile.AnchorLowerLeftOffset = UnityEditor.EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorLowerLeftOffset);
                            break;

                        case TextAnchor.LowerRight:
                            textProfile.AnchorLowerRightOffset = UnityEditor.EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorLowerRightOffset);
                            break;

                        case TextAnchor.MiddleCenter:
                            textProfile.AnchorMiddleCenterOffset = UnityEditor.EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorMiddleCenterOffset);
                            break;

                        case TextAnchor.MiddleLeft:
                            textProfile.AnchorMiddleLeftOffset = UnityEditor.EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorMiddleLeftOffset);
                            break;

                        case TextAnchor.MiddleRight:
                            textProfile.AnchorMiddleRightOffset = UnityEditor.EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorMiddleRightOffset);
                            break;

                        case TextAnchor.UpperCenter:
                            textProfile.AnchorUpperCenterOffset = UnityEditor.EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorUpperCenterOffset);
                            break;

                        case TextAnchor.UpperLeft:
                            textProfile.AnchorUpperLeftOffset = UnityEditor.EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorUpperLeftOffset);
                            break;

                        case TextAnchor.UpperRight:
                            textProfile.AnchorUpperRightOffset = UnityEditor.EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorUpperRightOffset);
                            break;
                    }
                }
            }
        }
}