using MixedRealityToolkit.Utilities.Inspectors;

namespace MixedRealityToolkit.UX.Buttons
{
        [UnityEditor.CustomEditor(typeof(CompoundButtonIcon))]
        public class CompoundButtonIconEditor : MRTKEditor {
            protected override void DrawCustomFooter() {
                CompoundButtonIcon iconButton = (CompoundButtonIcon)target;
                if (iconButton != null && iconButton.Profile != null)
                {
                    iconButton.IconName = iconButton.Profile.DrawIconSelectField(iconButton.iconName);
                }

            }
        }
}