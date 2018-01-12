using MixedRealityToolkit.Utilities.Inspectors;

namespace MixedRealityToolkit.UX.Buttons
{
        [UnityEditor.CustomEditor(typeof(ButtonIconProfileTexture))]
        public class ButtonIconProfileTextureEditor : ProfileInspector {
            protected override void DrawCustomFooter() {

                ButtonIconProfileTexture iconProfile = (ButtonIconProfileTexture)target;
                UnityEditor.EditorGUILayout.LabelField("Custom Icons", UnityEditor.EditorStyles.boldLabel);

                for (int i = 0; i < iconProfile.CustomIcons.Length; i++) {
                    Texture2D icon = iconProfile.CustomIcons[i];
                    icon = (Texture2D)UnityEditor.EditorGUILayout.ObjectField(icon != null ? icon.name : "(Empty)", icon, typeof(Texture2D), false, GUILayout.MaxHeight(textureSize));
                    iconProfile.CustomIcons[i] = icon;
                }

                if (GUILayout.Button("Add custom icon")) {
                    System.Array.Resize<Texture2D>(ref iconProfile.CustomIcons, iconProfile.CustomIcons.Length + 1);
                }
            }
			
			        public override string DrawIconSelectField(string iconName)
        {
            int selectedIconIndex = -1;
            List<string> iconKeys = GetIconKeys();
            for (int i = 0; i < iconKeys.Count; i++)
            {
                if (iconName == iconKeys[i])
                {
                    selectedIconIndex = i;
                    break;
                }
            }
            int newIconIndex = UnityEditor.EditorGUILayout.Popup("Icon", selectedIconIndex, iconKeys.ToArray());
            // This will automatically set the icon in the editor view
            iconName = (newIconIndex < 0 ? string.Empty : iconKeys[newIconIndex]);
            return iconName;
        }
        }
}