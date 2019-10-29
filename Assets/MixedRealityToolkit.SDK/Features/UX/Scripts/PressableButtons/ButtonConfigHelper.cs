using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Data;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.UI
{
    public enum ButtonIconStyle
    {
        Quad,
        Sprite,
        Char,
        None,
    }

    [ExecuteAlways]
    public class ButtonConfigHelper : MonoBehaviour
    {
        const string defaultIconChar = "E700";
        const string defaultIconTextureNameID = "_MainTex";

        public string Label
        {
            get { return labelText.text; }
            set { labelText.text = value; }
        }

        public UnityEvent OnClick => interactable?.OnClick;

        public ButtonIconStyle IconStyle
        {
            get { return iconStyle; }
            set
            {
                if (iconStyle != value)
                {
                    SetIconStyle(value);
                }
            }
        }

        [SerializeField]
        private TextMeshPro labelText = null;
        [SerializeField]
        private Interactable interactable = null;
        [SerializeField]
        private ButtonIconSet iconSet = null;
        [SerializeField]
        private GameObject seeItSayItLabel = null;
        [SerializeField]
        private TextMeshPro seeItSatItLabelText = null;
        [SerializeField]
        private ButtonIconStyle iconStyle = ButtonIconStyle.Quad;

        [SerializeField]
        private TextMeshPro iconCharLabel = null;
        [SerializeField]
        private TMP_FontAsset iconCharFont = null;
        [SerializeField]
        private string iconChar = defaultIconChar;

        [SerializeField]
        private SpriteRenderer iconSpriteRenderer = null;
        [SerializeField]
        private Sprite iconSprite = null;

        [SerializeField]
        private MeshRenderer iconQuadRenderer = null;
        [SerializeField]
        private string iconQuadTextureNameID = defaultIconTextureNameID;
        [SerializeField]
        private Texture iconQuadTexture = null;

        private MaterialPropertyBlock iconTexturePropertyBlock;

        public void SetCharIcon(string newIconChar, TMP_FontAsset newIconCharFont = null)
        {
            if (string.IsNullOrEmpty(newIconChar))
            {
                return;
            }

            if (newIconCharFont != null && newIconCharFont != iconCharFont)
            {
                iconCharFont = newIconCharFont;
            }

            if (iconCharLabel == null)
            {
                Debug.LogWarning("No icon char label in " + name + " - not setting custom icon char.");
                return;
            }

            iconChar = newIconChar;
            if (iconCharFont != null)
            {
                iconCharLabel.font = iconCharFont;
            }

            if (iconCharLabel.text != iconChar || iconCharLabel.font != iconCharFont)
            {
                iconCharLabel.text = newIconChar;
            }

            SetIconStyle(ButtonIconStyle.Char);
        }

        public void SetSpriteIcon(Sprite newIconSprite)
        {
            if (newIconSprite == null)
            {
                return;
            }

            if (iconSpriteRenderer == null)
            {
                Debug.LogWarning("No icon sprite renderer in " + name + " - not setting custom icon sprite.");
                return;
            }

            iconSprite = newIconSprite;

            if (iconSpriteRenderer.sprite != iconSprite)
            {
                iconSpriteRenderer.sprite = newIconSprite;
            }

            SetIconStyle(ButtonIconStyle.Sprite);
        }

        public void SetQuadIcon(Texture newIconTexture)
        {
            if (newIconTexture == null)
            {
                return;
            }

            if (iconQuadRenderer == null)
            {
                Debug.LogWarning("No icon quad renderer in " + name + " - not setting custom icon texture.");
                return;
            }

            iconQuadTexture = newIconTexture;

            if (iconTexturePropertyBlock == null)
            {
                iconTexturePropertyBlock = new MaterialPropertyBlock();
            }

            iconQuadRenderer.GetPropertyBlock(iconTexturePropertyBlock);
            iconTexturePropertyBlock.SetTexture("_MainTex", newIconTexture);
            iconQuadRenderer.SetPropertyBlock(iconTexturePropertyBlock);

            SetIconStyle(ButtonIconStyle.Quad);
        }

        private void SetIconStyle(ButtonIconStyle newStyle)
        {
            iconStyle = newStyle;
            switch (iconStyle)
            {
                case ButtonIconStyle.Char:
                    iconCharLabel?.gameObject.SetActive(true);
                    iconSpriteRenderer?.gameObject.SetActive(false);
                    iconQuadRenderer?.gameObject.SetActive(false);
                    break;

                case ButtonIconStyle.Sprite:
                    iconCharLabel?.gameObject.SetActive(false);
                    iconSpriteRenderer?.gameObject.SetActive(true);
                    iconQuadRenderer?.gameObject.SetActive(false);
                    break;

                case ButtonIconStyle.Quad:
                    iconCharLabel?.gameObject.SetActive(false);
                    iconSpriteRenderer?.gameObject.SetActive(false);
                    iconQuadRenderer?.gameObject.SetActive(true);
                    break;

                case ButtonIconStyle.None:
                    iconCharLabel?.gameObject.SetActive(false);
                    iconSpriteRenderer?.gameObject.SetActive(false);
                    iconQuadRenderer?.gameObject.SetActive(false);
                    break;
            }
        }

        private void ForceRefresh()
        {
            SetIconStyle(iconStyle);

            switch (iconStyle)
            {
                case ButtonIconStyle.Quad:
                    SetQuadIcon(iconQuadTexture);
                    break;

                case ButtonIconStyle.Char:
                    SetCharIcon(iconChar, iconCharFont);
                    break;

                case ButtonIconStyle.Sprite:
                    SetSpriteIcon(iconSprite);
                    break;
            }
        }

        private void OnEnable()
        {
            ForceRefresh();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(ButtonConfigHelper))]
        public class ConfigureButtonInspector : UnityEditor.Editor
        {
            const string LabelFoldoutKey = "MRTK.ButtonConfigHelper.Label";
            const string BasicEventsFoldoutKey = "MRTK.ButtonConfigHelper.BasicEvents";
            const string IconFoldoutKey = "MRTK.ButtonConfigHelper.Icon";
            const string ShowComponentsKey = "MRTK.ButtonConfigHelper.ShowComponents";

            const float iconPreviewWidth = 60f;
            const float iconPreviewHeight = 60f;

            private SerializedProperty labelTextProp;
            private SerializedProperty seeItSayItLabelProp;
            private SerializedProperty seeItSatItLabelTextProp;

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

            private ButtonConfigHelper cb;

            private void OnEnable()
            {
                labelTextProp = serializedObject.FindProperty("labelText");
                seeItSayItLabelProp = serializedObject.FindProperty("seeItSayItLabel");
                seeItSatItLabelTextProp = serializedObject.FindProperty("seeItSatItLabelText");

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

                EditorGUILayout.HelpBox("This component gathers commonly modified button settings in one place.", MessageType.Info);

                bool labelFoldout = SessionState.GetBool(LabelFoldoutKey, true);
                bool basicEventsFoldout = SessionState.GetBool(BasicEventsFoldoutKey, true);
                bool iconFoldout = SessionState.GetBool(IconFoldoutKey, true);
                bool showComponents = SessionState.GetBool(ShowComponentsKey, false);

                showComponents = EditorGUILayout.Toggle("Show Component References", showComponents);

                ButtonIconStyle oldStyle = (ButtonIconStyle)iconStyleProp.enumValueIndex;

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
                                EditorGUILayout.PropertyField(labelTextProp);
                            }

                            if (cb.labelText != null)
                            {
                                cb.labelText.gameObject.SetActive(EditorGUILayout.Toggle("Enable Main Label", cb.labelText.gameObject.activeSelf));
                                if (cb.labelText.gameObject.activeSelf)
                                {
                                    SerializedObject labelTextObject = new SerializedObject(cb.labelText);
                                    SerializedProperty textProp = labelTextObject.FindProperty("m_text");
                                    EditorGUILayout.PropertyField(textProp, new GUIContent("Main Label Text"));
                                    EditorGUILayout.Space();

                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        labelTextObject.ApplyModifiedProperties();
                                    }
                                }
                            }

                            if (cb.seeItSayItLabel != null)
                            {
                                cb.seeItSayItLabel.SetActive(EditorGUILayout.Toggle("Enable See it / Say it Label", cb.seeItSayItLabel.activeSelf));
                                if (cb.seeItSayItLabel.activeSelf && cb.seeItSatItLabelText != null)
                                {
                                    if (showComponents)
                                    {
                                        EditorGUILayout.PropertyField(seeItSayItLabelProp);
                                    }

                                    if (cb.seeItSayItLabel.activeSelf)
                                    {
                                        if (showComponents)
                                        {
                                            EditorGUILayout.PropertyField(seeItSatItLabelTextProp);
                                        }

                                        EditorGUI.BeginChangeCheck();

                                        SerializedObject sisiLabelTextObject = new SerializedObject(cb.seeItSatItLabelText);
                                        SerializedProperty sisiTextProp = sisiLabelTextObject.FindProperty("m_text");
                                        EditorGUILayout.PropertyField(sisiTextProp, new GUIContent("See it / Say it Label"));
                                        EditorGUILayout.Space();

                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            sisiLabelTextObject.ApplyModifiedProperties();
                                        }
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

                            SerializedObject interactableObject = new SerializedObject(cb.interactable);
                            SerializedProperty onClickProp = interactableObject.FindProperty("OnClick");
                            EditorGUILayout.PropertyField(onClickProp);

                            if (EditorGUI.EndChangeCheck())
                            {
                                interactableObject.ApplyModifiedProperties();
                            }
                        }
                    }
                }

                using (new EditorGUI.IndentLevelScope(1))
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        iconFoldout = EditorGUILayout.Foldout(iconFoldout, "Icon", true);

                        if (iconFoldout)
                        {
                            EditorGUILayout.PropertyField(iconStyleProp);

                            switch (cb.iconStyle)
                            {
                                case ButtonIconStyle.Char:
                                    DrawIconCharEditor(showComponents);
                                    break;

                                case ButtonIconStyle.Quad:
                                    DrawIconQuadEditor(showComponents);
                                    break;

                                case ButtonIconStyle.Sprite:
                                    DrawIconSpriteEditor(showComponents);
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

                serializedObject.ApplyModifiedProperties();

                if (oldStyle != (ButtonIconStyle)iconStyleProp.enumValueIndex)
                {
                    cb.ForceRefresh();
                }
            }

            private void DrawIconSpriteEditor(bool showComponents)
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
                    if (cb.iconSpriteRenderer != null)
                    {
                        currentIconSprite = cb.iconSpriteRenderer.sprite;
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("This button has no icon quad renderer assigned.", MessageType.Warning);
                        return;
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(iconSetProp);
                if (cb.iconSet != null)
                {
                    Sprite newIconSprite;
                    if (cb.iconSet.DrawSpriteIconSelector(currentIconSprite, out newIconSprite, 1))
                    {
                        iconSpriteProp.objectReferenceValue = newIconSprite;
                        cb.SetSpriteIcon(newIconSprite);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No icon set assigned. You can specify custom icons manually by assigning them to the field below:", MessageType.Info);
                    EditorGUILayout.PropertyField(iconSpriteProp);
                }
            }

            private void DrawIconQuadEditor(bool showComponents)
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
                    if (cb.iconQuadRenderer != null)
                    {
                        currentIconTexture = cb.iconQuadRenderer.sharedMaterial.GetTexture(cb.iconQuadTextureNameID);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("This button has no icon quad renderer assigned.", MessageType.Warning);
                        return;
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(iconSetProp);
                if (cb.iconSet != null)
                {
                    Texture newIconTexture;
                    if (cb.iconSet.DrawQuadIconSelector(currentIconTexture, out newIconTexture, 1))
                    {
                        iconQuadTextureProp.objectReferenceValue = newIconTexture;
                        cb.SetQuadIcon(newIconTexture);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No icon set assigned. You can specify custom icons manually by assigning them to the field below:", MessageType.Info);
                    EditorGUILayout.PropertyField(iconQuadTextureProp);
                }
            }

            private void DrawIconCharEditor(bool showComponents)
            {
                if (showComponents)
                {
                    EditorGUILayout.PropertyField(iconCharLabelProp);
                }

                string currentIconChar = null;

                if (!string.IsNullOrEmpty(iconCharProp.stringValue))
                {
                    currentIconChar = iconCharProp.stringValue;
                }
                else
                {
                    if (cb.iconCharLabel != null)
                    {
                        currentIconChar = cb.iconCharLabel.text;
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("This button has no icon char renderer assigned.", MessageType.Warning);
                        return;
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(iconSetProp);
                if (cb.iconSet != null)
                {
                    string newIconChar;
                    if (cb.iconSet.DrawCharIconSelector(currentIconChar, out newIconChar, 1))
                    {
                        iconCharProp.stringValue = newIconChar;
                        iconFontProp.objectReferenceValue = cb.iconSet.CharIconFont;
                        cb.SetCharIcon(newIconChar, cb.iconSet.CharIconFont);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No icon set assigned. You can specify custom icons manually by assigning them to the field below:", MessageType.Info);
                    EditorGUILayout.PropertyField(iconQuadTextureProp);
                }
            }
        }
#endif
    }
}