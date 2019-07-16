// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UITextMeshProConverter : EditorWindow
{
    private TMP_FontAsset ReplacementFontAsset;

    [MenuItem("Mixed Reality Toolkit/Utilities/Text Mesh Converter", false, 0)]
    public static void OpenWindow()
    {
        // Dock it next to the Scene View.
        var window = GetWindow<UITextMeshProConverter>();
        window.titleContent = new GUIContent("Text Mesh Pro Converter", EditorGUIUtility.IconContent("TextMesh Icon").image);
        window.Show();
    }

    private void OnGUI()
    {
        ReplacementFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("Replacement TMP Font Asset", ReplacementFontAsset, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Convert Selected Text Mesh"))
        {
            ConvertTextMesh();
        }

        if (GUILayout.Button("Convert Selected Text GUI"))
        {
            ConvertText();
        }
    }

    //[MenuItem("Mixed Reality Toolkit/UI/Convert Text To Text Mesh Pro UGUI", false, 2000)]
    public void ConvertText()
    {
        if (!NullChecks()) return;

        List<Text> textObjects = new List<Text>();
        List<GameObject> textGameObjects = new List<GameObject>();
        foreach (var gameObject in Selection.gameObjects)
        {
            Text uiText = gameObject.GetComponent<Text>();
            if (uiText != null)
            {
                textObjects.Add(uiText);
                textGameObjects.Add(gameObject);
            }
        }

        Undo.RecordObjects(textGameObjects.ToArray(), "Convert to Text Mesh Pro");

        foreach (Text uiText in textObjects)
        {
            GameObject gameObject = uiText.gameObject;

            Font font = uiText.font;
            int fontSize = uiText.fontSize;
            FontStyle fontStyle = uiText.fontStyle;
            HorizontalWrapMode horizWrap = uiText.horizontalOverflow;
            string textValue = uiText.text;
            TextAnchor anchor = uiText.alignment;
            Color color = uiText.color;
            float lineSpacing = uiText.lineSpacing;
            var anchoredPosition3D = uiText.rectTransform.anchoredPosition3D;
            var anchorMax = uiText.rectTransform.anchorMax;
            var anchorMin = uiText.rectTransform.anchorMin;
            var localPosition = uiText.rectTransform.localPosition;
            var localRotation = uiText.rectTransform.localRotation;
            var localScale = uiText.rectTransform.localScale;
            var pivot = uiText.rectTransform.pivot;
            var sizeDelta = uiText.rectTransform.sizeDelta;

            DestroyImmediate(uiText);

            TextMeshProUGUI textMesh = gameObject.AddComponent<TextMeshProUGUI>();
            textMesh.font = ReplacementFontAsset;
            textMesh.fontSize = fontSize;
            textMesh.fontStyle = ConvertToTMPFontStyle(fontStyle);
            textMesh.enableWordWrapping = (horizWrap == HorizontalWrapMode.Wrap);
            textMesh.text = textValue;
            textMesh.alignment = ConvertTextAnchorToTMPTextAlignment(anchor);
            textMesh.color = color;
            textMesh.lineSpacing = lineSpacing;

            textMesh.rectTransform.anchoredPosition3D = anchoredPosition3D;
            textMesh.rectTransform.anchorMax = anchorMax;
            textMesh.rectTransform.anchorMin = anchorMin;
            textMesh.rectTransform.localPosition = localPosition;
            textMesh.rectTransform.localRotation = localRotation;
            textMesh.rectTransform.localScale = localScale;
            textMesh.rectTransform.pivot = pivot;
            textMesh.rectTransform.sizeDelta = sizeDelta;
        }
    }

    //[MenuItem("Mixed Reality Toolkit/UI/Convert Text To Text Mesh Pro UGUI", false, 2000)]
    public void ConvertTextMesh()
    {
        if (!NullChecks()) return;

        List<TextMesh> textMeshObjects = new List<TextMesh>();
        List<GameObject> textGameObjects = new List<GameObject>();
        foreach (var gameObject in Selection.gameObjects)
        {
            TextMesh uiTextMesh = gameObject.GetComponent<TextMesh>();

            if (uiTextMesh != null)
            {
                textMeshObjects.Add(uiTextMesh);
                textGameObjects.Add(gameObject);
            }
        }

        foreach (TextMesh uiText in textMeshObjects)
        {
            Undo.RegisterCompleteObjectUndo(uiText.gameObject, "Convert to Text Mesh Pro");
            //Undo.RecordObject(uiText.gameObject, "Convert to Text Mesh Pro");

            GameObject gameObject = uiText.gameObject;

            Font font = uiText.font;
            int fontSize = uiText.fontSize;
            FontStyle fontStyle = uiText.fontStyle;
            //HorizontalWrapMode horizWrap = uiText.wrap;
            string textValue = uiText.text;
            TextAlignment alignment = uiText.alignment;
            Color color = uiText.color;
            float lineSpacing = uiText.lineSpacing;

            /*
            var anchoredPosition3D = uiText.rectTransform.anchoredPosition3D;
            var anchorMax = uiText.rectTransform.anchorMax;
            var anchorMin = uiText.rectTransform.anchorMin;
            var localPosition = uiText.rectTransform.localPosition;
            var localRotation = uiText.rectTransform.localRotation;
            var localScale = uiText.rectTransform.localScale;
            var pivot = uiText.rectTransform.pivot;
            var sizeDelta = uiText.rectTransform.sizeDelta;
            */

            DestroyImmediate(uiText);

            TextMeshPro textMesh = gameObject.AddComponent<TextMeshPro>();

            //textMesh.font = fontMap[font.name];
            textMesh.fontSize = fontSize;
            textMesh.fontStyle = ConvertToTMPFontStyle(fontStyle);
            //textMesh.enableWordWrapping = (horizWrap == HorizontalWrapMode.Wrap);
            textMesh.text = textValue;
            textMesh.alignment = ConvertTextAlignmentToTMPTextAlignment(alignment);
            textMesh.color = color;
            textMesh.lineSpacing = lineSpacing;

            textMesh.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 40);
            /*
            textMesh.rectTransform.anchoredPosition3D = anchoredPosition3D;
            textMesh.rectTransform.anchorMax = anchorMax;
            textMesh.rectTransform.anchorMin = anchorMin;
            textMesh.rectTransform.localPosition = localPosition;
            textMesh.rectTransform.localRotation = localRotation;
            textMesh.rectTransform.localScale = localScale;
            textMesh.rectTransform.pivot = pivot;
            textMesh.rectTransform.sizeDelta = sizeDelta;
            */

            textMesh.font = ReplacementFontAsset;

            Undo.FlushUndoRecordObjects();
        }
    }

    private bool NullChecks()
    {
        if (Selection.gameObjects == null)
        {
            EditorUtility.DisplayDialog(
                "ERROR!", "You must select a Unity UI Text Object to convert.", "OK", "");
            return false;
        }

        if (ReplacementFontAsset == null)
        {
            EditorUtility.DisplayDialog("ERROR!", "You must select a Replacement Text Mesh Pro Font Asset.", "OK", "");
            return false;
        }

        return true;
    }

    /*
     *         Dictionary<string, TMP_FontAsset> fontMap = new Dictionary<string, TMP_FontAsset>();
        string[] fontAssets = UnityEditor.AssetDatabase.FindAssets("t:TMP_FontAsset");
        foreach (string fontAssetGUID in fontAssets)
        {
            string fontAssetPath = AssetDatabase.GUIDToAssetPath(fontAssetGUID);
            var tmp_fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);
            //tmp_fontAsset.material.shader

            if (tmp_fontAsset != null)
            {
                fontMap[tmp_fontAsset.name] = tmp_fontAsset;
            }
        }
        */

    /*
    [MenuItem("Utils/Select Text Components")]
    public static void SelectText(MenuCommand command)
    {
        Transform[] ts = FindObjectsOfType<Transform>();
        List<GameObject> selection = new List<GameObject>();
        foreach (Transform t in ts)
        {
            Text[] cs = t.gameObject.GetComponents<Text>();
            if (cs.Length > 0)
            {
                selection.Add(t.gameObject);
            }
        }
        Selection.objects = selection.ToArray();
    }*/

    private static TMPro.FontStyles ConvertToTMPFontStyle(UnityEngine.FontStyle uGuiFontStyle)
    {
        FontStyles tmp = FontStyles.Normal;
        switch (uGuiFontStyle)
        {
            case FontStyle.Normal:
            default:
                tmp = FontStyles.Normal;
                break;
            case FontStyle.Bold:
                tmp = FontStyles.Bold;
                break;
            case FontStyle.Italic:
                tmp = FontStyles.Italic;
                break;
            case FontStyle.BoldAndItalic:
                tmp = FontStyles.Bold | FontStyles.Italic;
                break;
        }

        return tmp;
    }

    private static TMPro.TextAlignmentOptions ConvertTextAlignmentToTMPTextAlignment(UnityEngine.TextAlignment textMeshAlignment)
    {
        switch(textMeshAlignment)
        {
            case TextAlignment.Left:
                return TextAlignmentOptions.MidlineLeft;
            case TextAlignment.Center:
                return TextAlignmentOptions.Midline;
            case TextAlignment.Right:
                return TextAlignmentOptions.MidlineRight;
        }

        return TextAlignmentOptions.Midline;
    }

    private static TMPro.TextAlignmentOptions ConvertTextAnchorToTMPTextAlignment(UnityEngine.TextAnchor uGuiAlignment)
    {
        TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft;

        switch (uGuiAlignment)
        {
            default:
            case TextAnchor.UpperLeft:
                alignment = TextAlignmentOptions.TopLeft;
                break;
            case TextAnchor.UpperCenter:
                alignment = TextAlignmentOptions.Top;
                break;
            case TextAnchor.UpperRight:
                alignment = TextAlignmentOptions.TopRight;
                break;
            case TextAnchor.MiddleLeft:
                alignment = TextAlignmentOptions.MidlineLeft;
                break;
            case TextAnchor.MiddleCenter:
                alignment = TextAlignmentOptions.Midline;
                break;
            case TextAnchor.MiddleRight:
                alignment = TextAlignmentOptions.MidlineRight;
                break;
            case TextAnchor.LowerLeft:
                alignment = TextAlignmentOptions.BottomLeft;
                break;
            case TextAnchor.LowerCenter:
                alignment = TextAlignmentOptions.Bottom;
                break;
            case TextAnchor.LowerRight:
                alignment = TextAlignmentOptions.BottomRight;
                break;
        }

        return alignment;
    }
}
