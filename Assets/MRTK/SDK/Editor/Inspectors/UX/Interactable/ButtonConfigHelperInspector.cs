// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(ButtonConfigHelper)), CanEditMultipleObjects]
    public class ButtonConfigHelperInspector : UnityEditor.Editor
    {
        private const string LabelFoldoutKey = "MRTK.ButtonConfigHelper.Label";
        private const string BasicEventsFoldoutKey = "MRTK.ButtonConfigHelper.BasicEvents";
        private const string IconFoldoutKey = "MRTK.ButtonConfigHelper.Icon";
        private const string ShowComponentsKey = "MRTK.ButtonConfigHelper.ShowComponents";
        private const string ShowSpeechCommandKey = "MRTK.ButtonConfigHelper.DisplayInteractableSpeechCommand";

        private const string generatedIconSetName = "CustomIconSet";
        private const string customIconSetsFolderName = "CustomIconSets";
        private const string customIconUpgradeMessage = "This button appears to have a custom icon material. This is no longer required for custom icons.\n\n" +
            "We recommend upgrading the buttons in your project by installing the Microsoft.MixedRealityToolkit.Unity.Tools package and using the Migration Tool.";
        private const string missingIconWarningMessage = "The icon used by this button was not found in the icon set. You can see the icon currently being used is in the field below:";
        private const string missingCharIconWarningMessage = "The icon used by this button was not found in the icon set. It may be part of another char icon font that was previously part of this icon set";
        private const string customIconSetCreatedMessage = "A new icon set has been created to hold your button's custom icons. It has been saved to:\n\n{0}";
        private const string upgradeDocUrl = "https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/button#how-to-change-the-icon-and-text";

        private SerializedProperty mainLabelTextProp;
        private SerializedProperty seeItSayItLabelProp;
        private SerializedProperty displayInteractableSpeechCommand;
        private SerializedProperty seeItSayItLabelTextProp;

        private SerializedProperty interactableProp;

        private SerializedProperty iconStyleProp;
        private SerializedProperty iconSetProp;

        private SerializedProperty iconCharLabelProp;
        private SerializedProperty iconCharProp;
        private SerializedProperty iconFontProp;

        private SerializedProperty iconSpriteRendererProp;
        private SerializedProperty iconSpriteProp;

        private SerializedProperty iconQuadRendererProp;
        private SerializedProperty iconQuadTextureNameIDProp;
        private SerializedProperty iconQuadTextureProp;

        private ButtonConfigHelper cb = null;

        private void OnEnable()
        {
            mainLabelTextProp = serializedObject.FindProperty("mainLabelText");
            seeItSayItLabelProp = serializedObject.FindProperty("seeItSayItLabel");
            displayInteractableSpeechCommand = serializedObject.FindProperty("displayInteractableSpeechCommand");
            seeItSayItLabelTextProp = serializedObject.FindProperty("seeItSayItLabelText");

            interactableProp = serializedObject.FindProperty("interactable");

            iconStyleProp = serializedObject.FindProperty("iconStyle");
            iconSetProp = serializedObject.FindProperty("iconSet");

            iconCharLabelProp = serializedObject.FindProperty("iconCharLabel");
            iconCharProp = serializedObject.FindProperty("iconChar");
            iconFontProp = serializedObject.FindProperty("iconCharFont");

            iconSpriteRendererProp = serializedObject.FindProperty("iconSpriteRenderer");
            iconSpriteProp = serializedObject.FindProperty("iconSprite");

            iconQuadRendererProp = serializedObject.FindProperty("iconQuadRenderer");
            iconQuadTextureNameIDProp = serializedObject.FindProperty("iconQuadTextureNameID");
            iconQuadTextureProp = serializedObject.FindProperty("iconQuadTexture");
        }

        public override void OnInspectorGUI()
        {
            cb = (ButtonConfigHelper)target;

            bool labelFoldout = SessionState.GetBool(LabelFoldoutKey, true);
            bool basicEventsFoldout = SessionState.GetBool(BasicEventsFoldoutKey, true);
            bool iconFoldout = SessionState.GetBool(IconFoldoutKey, true);
            bool showComponents = SessionState.GetBool(ShowComponentsKey, false);
            bool showSpeechCommand = SessionState.GetBool(ShowSpeechCommandKey, true);

            if (cb.EditorCheckForCustomIcon())
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("Custom Icon Migration", EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox(customIconUpgradeMessage, MessageType.Error);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Use migration tool to upgrade buttons"))
                        {
                            if (!EditorApplication.ExecuteMenuItem("Mixed Reality/Toolkit/Utilities/Migration Window"))
                            {
                                EditorUtility.DisplayDialog("Package Required", "You need to install the MRTK tools (Microsoft.MixedRealityToolkit.Unity.Tools) package to use the Migration Tool", "OK");
                            }
                        }

                        InspectorUIUtility.RenderDocumentationButton(upgradeDocUrl);
                    }
                }
            }

            showComponents = EditorGUILayout.Toggle("Show Component References", showComponents);

            ButtonIconStyle oldStyle = (ButtonIconStyle)iconStyleProp.intValue;

            using (new EditorGUI.IndentLevelScope(1))
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    labelFoldout = EditorGUILayout.Foldout(labelFoldout, "Labels", true);

                    if (labelFoldout)
                    {
                        EditorGUI.BeginChangeCheck();

                        if (showComponents)
                        {
                            EditorGUILayout.PropertyField(mainLabelTextProp);
                        }

                        if (mainLabelTextProp.objectReferenceValue != null)
                        {
                            Component mainLabelText = (Component)mainLabelTextProp.objectReferenceValue;
                            bool mainLabelTextActive = EditorGUILayout.Toggle("Enable Main Label", mainLabelText.gameObject.activeSelf);
                            if (mainLabelText.gameObject.activeSelf != mainLabelTextActive)
                            {
                                mainLabelText.gameObject.SetActive(mainLabelTextActive);
                                EditorUtility.SetDirty(mainLabelText.gameObject);
                            }
                            if (mainLabelText.gameObject.activeSelf)
                            {
                                SerializedObject labelTextObject = new SerializedObject(mainLabelText);
                                SerializedProperty textProp = labelTextObject.FindProperty("m_text");
                                EditorGUILayout.PropertyField(textProp, new GUIContent("Main Label Text"));
                                EditorGUILayout.Space();

                                if (EditorGUI.EndChangeCheck())
                                {
                                    labelTextObject.ApplyModifiedProperties();
                                }
                            }
                        }

                        if (showComponents)
                        {
                            EditorGUILayout.PropertyField(seeItSayItLabelProp);
                        }

                        if (seeItSayItLabelProp.objectReferenceValue != null)
                        {
                            GameObject seeItSayItLabel = (GameObject)seeItSayItLabelProp.objectReferenceValue;
                            bool seeItSayItLabelActive = EditorGUILayout.Toggle("Enable See it / Say it Label", seeItSayItLabel.activeSelf);
                            if (seeItSayItLabel.activeSelf != seeItSayItLabelActive)
                            {
                                seeItSayItLabel.SetActive(seeItSayItLabelActive);
                                EditorUtility.SetDirty(seeItSayItLabel);
                            }

                            if (seeItSayItLabel.activeSelf)
                            {
                                var sisiChanged = false;
                                EditorGUI.BeginChangeCheck();

                                if (showComponents)
                                {
                                    EditorGUILayout.PropertyField(seeItSayItLabelTextProp);
                                }

                                showSpeechCommand = EditorGUILayout.Toggle("Display Speech Command", showSpeechCommand);

                                SerializedObject sisiLabelTextObject = new SerializedObject(seeItSayItLabelTextProp.objectReferenceValue);
                                SerializedProperty sisiTextProp = sisiLabelTextObject.FindProperty("m_text");
                                if (!showSpeechCommand)
                                {
                                    EditorGUILayout.PropertyField(sisiTextProp, new GUIContent("See it / Say it Label"));
                                    EditorGUILayout.Space();
                                }
                                else
                                {
                                    if (interactableProp.objectReferenceValue != null)
                                    {
                                        SerializedObject interactableObject = new SerializedObject(interactableProp.objectReferenceValue);
                                        SerializedProperty voiceCommandProperty = interactableObject.FindProperty("voiceCommand");

                                        if (string.IsNullOrEmpty(voiceCommandProperty.stringValue))
                                        {
                                            EditorGUILayout.HelpBox("No valid speech command provided to the interactable", MessageType.Warning);
                                        }
                                        else
                                        {
                                            string sisiText = string.Format("Say \"{0}\"", voiceCommandProperty.stringValue);
                                            if (sisiTextProp.stringValue != sisiText)
                                            {
                                                sisiTextProp.stringValue = sisiText;
                                                sisiChanged = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        EditorGUILayout.HelpBox("There is no interactable linked to the button config helper. One is needed to display the appropriate speech command", MessageType.Warning);
                                    }
                                }
                                sisiChanged |= EditorGUI.EndChangeCheck();

                                if (sisiChanged)
                                {
                                    sisiLabelTextObject.ApplyModifiedProperties();
                                }
                            }
                        }
                    }
                }
            }

            using (new EditorGUI.IndentLevelScope(1))
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    basicEventsFoldout = EditorGUILayout.Foldout(basicEventsFoldout, "Basic Events", true);

                    if (basicEventsFoldout)
                    {
                        EditorGUI.BeginChangeCheck();

                        if (showComponents)
                        {
                            EditorGUILayout.PropertyField(interactableProp);
                        }

                        if (interactableProp.objectReferenceValue != null)
                        {
                            SerializedObject interactableObject = new SerializedObject(interactableProp.objectReferenceValue);
                            SerializedProperty onClickProp = interactableObject.FindProperty("OnClick");
                            EditorGUILayout.PropertyField(onClickProp);

                            if (EditorGUI.EndChangeCheck())
                            {
                                interactableObject.ApplyModifiedProperties();
                            }
                        }
                    }
                }
            }

            using (new EditorGUI.IndentLevelScope(1))
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    iconFoldout = EditorGUILayout.Foldout(iconFoldout, "Icon", true);
                    ButtonIconSet iconSet = (ButtonIconSet)iconSetProp.objectReferenceValue;

                    if (iconFoldout)
                    {
                        EditorGUILayout.PropertyField(iconStyleProp);

                        switch (cb.IconStyle)
                        {
                            case ButtonIconStyle.Char:
                                DrawIconCharEditor(showComponents, iconSet);
                                break;

                            case ButtonIconStyle.Quad:
                                DrawIconQuadEditor(showComponents, iconSet);
                                break;

                            case ButtonIconStyle.Sprite:
                                DrawIconSpriteEditor(showComponents, iconSet);
                                break;
                        }

                        EditorGUILayout.Space();
                    }
                }
            }

            SessionState.SetBool(LabelFoldoutKey, labelFoldout);
            SessionState.SetBool(BasicEventsFoldoutKey, basicEventsFoldout);
            SessionState.SetBool(IconFoldoutKey, iconFoldout);
            SessionState.SetBool(ShowComponentsKey, showComponents);
            SessionState.SetBool(ShowSpeechCommandKey, showSpeechCommand);

            serializedObject.ApplyModifiedProperties();

            if (oldStyle != (ButtonIconStyle)iconStyleProp.intValue)
            {
                cb.ForceRefresh();
            }
        }

        private void DrawIconSpriteEditor(bool showComponents, ButtonIconSet iconSet)
        {
            if (showComponents)
            {
                EditorGUILayout.PropertyField(iconSpriteRendererProp);
            }

            Sprite currentIconSprite = null;

            if (iconQuadTextureProp.objectReferenceValue != null)
            {
                currentIconSprite = iconSpriteProp.objectReferenceValue as Sprite;
            }
            else
            {
                if (iconSpriteRendererProp.objectReferenceValue != null)
                {
                    currentIconSprite = ((SpriteRenderer)iconSpriteRendererProp.objectReferenceValue).sprite;
                }
                else
                {
                    EditorGUILayout.HelpBox("This button has no icon quad renderer assigned.", MessageType.Warning);
                    return;
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(iconSetProp);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh"), EditorStyles.miniButtonRight, GUILayout.Width(24f)))
            {
                iconSet.UpdateSpriteIconTextures();
            }
            EditorGUILayout.EndHorizontal();
            if (iconSet == null)
            {
                EditorGUILayout.HelpBox("No icon set assigned. You can specify custom icons manually by assigning them to the field below:", MessageType.Info);
                EditorGUILayout.PropertyField(iconQuadTextureProp);
                return;
            }
            if (iconSet.SpriteIcons == null || iconSet.SpriteIcons.Length == 0)
            {
                EditorGUILayout.HelpBox("No sprite icons assigned to the icon set. You can specify custom icons manually by assigning them to the field below:", MessageType.Info);
                EditorGUILayout.PropertyField(iconQuadTextureProp);
                return;
            }

            Sprite newIconSprite;
            bool foundSprite;
            if (iconSet.EditorDrawSpriteIconSelector(currentIconSprite, out foundSprite, out newIconSprite, 1))
            {
                iconSpriteProp.objectReferenceValue = newIconSprite;
                cb.SetSpriteIcon(newIconSprite);
            }

            if (!foundSprite)
            {
                EditorGUILayout.HelpBox(missingIconWarningMessage, MessageType.Warning);
                EditorGUILayout.PropertyField(iconSpriteProp);
            }
        }

        private void DrawIconQuadEditor(bool showComponents, ButtonIconSet iconSet)
        {
            if (showComponents)
            {
                EditorGUILayout.PropertyField(iconQuadRendererProp);
            }

            Texture currentIconTexture = null;

            if (iconQuadTextureProp.objectReferenceValue != null)
            {
                currentIconTexture = iconQuadTextureProp.objectReferenceValue as Texture;
            }
            else
            {
                if (iconQuadRendererProp.objectReferenceValue != null)
                {
                    currentIconTexture = ((Renderer)iconQuadRendererProp.objectReferenceValue).sharedMaterial.GetTexture(iconQuadTextureNameIDProp.stringValue);
                }
                else
                {
                    EditorGUILayout.HelpBox("This button has no icon quad renderer assigned.", MessageType.Warning);
                    return;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(iconSetProp);

            EditorGUILayout.Space();
            if (iconSet == null)
            {
                EditorGUILayout.HelpBox("No icon set assigned. You can specify custom icons manually by assigning them to the field below:", MessageType.Info);
                EditorGUILayout.PropertyField(iconQuadTextureProp);
                return;
            }
            if (iconSet.QuadIcons == null || iconSet.QuadIcons.Length == 0)
            {
                EditorGUILayout.HelpBox("No quad icons assigned to the icon set. You can specify custom icons manually by assigning them to the field below:", MessageType.Info);
                EditorGUILayout.PropertyField(iconQuadTextureProp);
                return;
            }

            Texture newIconTexture;
            bool foundTexture;
            if (iconSet.EditorDrawQuadIconSelector(currentIconTexture, out foundTexture, out newIconTexture, 1))
            {
                iconQuadTextureProp.objectReferenceValue = newIconTexture;
                cb.SetQuadIcon(newIconTexture);
            }

            if (!foundTexture)
            {
                EditorGUILayout.HelpBox(missingIconWarningMessage, MessageType.Warning);
                EditorGUILayout.PropertyField(iconQuadTextureProp);
            }
        }

        private void DrawIconCharEditor(bool showComponents, ButtonIconSet iconSet)
        {
            if (showComponents)
            {
                EditorGUILayout.PropertyField(iconCharLabelProp);
            }

            uint currentIconChar = 0;

            if (iconCharProp.longValue > 0)
            {
                currentIconChar = (uint)iconCharProp.longValue;
            }
            else
            {
                if (iconCharLabelProp != null)
                {
                    SerializedObject tmpObject = new SerializedObject(iconCharLabelProp.objectReferenceValue);
                    SerializedProperty tmpTextProp = tmpObject.FindProperty("m_text");
                    string iconCharString = tmpTextProp.stringValue;
                    currentIconChar = ButtonIconSet.ConvertCharStringToUInt32(iconCharString);
                }
                else
                {
                    EditorGUILayout.HelpBox("This button has no icon char renderer assigned.", MessageType.Warning);
                    return;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(iconSetProp);
            if (iconSet == null)
            {
                EditorGUILayout.HelpBox("No icon set assigned. You can specify custom icons manually by assigning them to the field below:", MessageType.Info);
                EditorGUILayout.PropertyField(iconQuadTextureProp);
                return;
            }

            uint newIconChar;
            bool foundChar;
            if (iconSet.EditorDrawCharIconSelector(currentIconChar, out foundChar, out newIconChar, 1))
            {
                iconCharProp.longValue = newIconChar;
                SerializedObject iconSetObject = new SerializedObject(iconSet);
                SerializedProperty charIconFontProp = iconSetObject.FindProperty("charIconFont");
                iconFontProp.objectReferenceValue = charIconFontProp.objectReferenceValue;
                cb.SetCharIcon(newIconChar);

                if (!foundChar)
                {
                    EditorGUILayout.HelpBox(missingCharIconWarningMessage, MessageType.Warning);
                }
            }
        }
    }
}
