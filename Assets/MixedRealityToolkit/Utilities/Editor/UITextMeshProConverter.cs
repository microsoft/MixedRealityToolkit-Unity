// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UITextMeshProConverter : Editor
{
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

    [MenuItem("Mixed Reality Toolkit/UI/Convert Text To Text Mesh Pro UGUI", false, 2000)]
    public static void ConvertTextToTextMeshPro()
    {
        if (Selection.gameObjects == null)
        {
            EditorUtility.DisplayDialog(
                "ERROR!", "You must select a Unity UI Text Object to convert.", "OK", "");
            return;
        }


        Material MRTKTextMeshProMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/StandardAssets/Materials/MRTKTextMeshPro.mat", typeof(Material));
        if (MRTKTextMeshProMaterial == null)
        {
            EditorUtility.DisplayDialog("ERROR!", "Could not find MRTKTextMeshPro.mat", "OK", "");
            return;
        }

        Dictionary<string, TMP_FontAsset> fontMap = new Dictionary<string, TMP_FontAsset>();
        string[] fontAssets = UnityEditor.AssetDatabase.FindAssets("t:TMP_FontAsset");
        foreach(string fontAssetGUID in fontAssets)
        {
            string fontAssetPath = AssetDatabase.GUIDToAssetPath(fontAssetGUID);
            var tmp_fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);
            //tmp_fontAsset.material.shader

            if (tmp_fontAsset != null)
            {
                fontMap[tmp_fontAsset.name] = tmp_fontAsset;
            }
        }

        List<Text> textGameObjects = new List<Text>();
        foreach (var gameObject in Selection.gameObjects)
        {
            Text uiText = gameObject.GetComponent<Text>();

            /*
            if (!fontMap.ContainsKey(uiText.font.name))
            {
                TMPro_FontAssetCreatorWindow.ShowFontAtlasCreatorWindow(uiText.font);
                return;
            }*/

            textGameObjects.Add(uiText);
        }

        Resources.Load<TMP_FontAsset>("Fonts & Materials/Anton SDF");
        foreach (Text uiText in textGameObjects)
        {
            //Text uiText = gameObject.GetComponent<Text>();
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

            //textMesh.font = fontMap[font.name];
            textMesh.fontSize = fontSize;
            textMesh.fontStyle = GetTmpFontStyle(fontStyle);
            textMesh.enableWordWrapping = (horizWrap == HorizontalWrapMode.Wrap);
            textMesh.text = textValue;
            textMesh.alignment = GetTmpAlignment(anchor);
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

            textMesh.material = new Material(textMesh.material);
            //Material test = new Material(textMesh.fontSharedMaterial);
            //MRTKTextMeshProMaterial.CopyPropertiesFromMaterial(textMesh.fontSharedMaterial);
            //textMesh.fontSharedMaterial = test;
            //MRTKTextMeshProMaterial.CopyPropertiesFromMaterial(textMesh.material);
            //textMesh.fontSharedMaterial = MRTKTextMeshProMaterial;
        }

        /*
        Text uiText = Selection.activeGameObject.GetComponent<Text>();
        if (uiText == null)
        {
            EditorUtility.DisplayDialog(
                "ERROR!", "You must select a Unity UI Text Object to convert.", "OK", "");
            return;
        }

        MenuCommand command = new MenuCommand(uiText);

        tmp.fontStyle = GetTmpFontStyle(uiText.fontStyle);

        tmp.fontSize = uiText.fontSize;
        tmp.fontSizeMin = uiText.resizeTextMinSize;
        tmp.fontSizeMax = uiText.resizeTextMaxSize;
        tmp.enableAutoSizing = uiText.resizeTextForBestFit;
        tmp.alignment = GetTmpAlignment(uiText.alignment);
        tmp.text = uiText.text;
        tmp.color = uiText.color;

        tmp.transform.SetParent(uiText.transform.parent);
        tmp.name = uiText.name;

        tmp.rectTransform.anchoredPosition3D = uiText.rectTransform.anchoredPosition3D;
        tmp.rectTransform.anchorMax = uiText.rectTransform.anchorMax;
        tmp.rectTransform.anchorMin = uiText.rectTransform.anchorMin;
        tmp.rectTransform.localPosition = uiText.rectTransform.localPosition;
        tmp.rectTransform.localRotation = uiText.rectTransform.localRotation;
        tmp.rectTransform.localScale = uiText.rectTransform.localScale;
        tmp.rectTransform.pivot = uiText.rectTransform.pivot;
        tmp.rectTransform.sizeDelta = uiText.rectTransform.sizeDelta;

        tmp.transform.SetSiblingIndex(uiText.transform.GetSiblingIndex());

        // Copy all other components
        Component[] components = uiText.GetComponents<Component>();
        int componentsCopied = 0;
        for (int i = 0; i < components.Length; i++)
        {
            var thisType = components[i].GetType();
            if (thisType == typeof(Text) ||
                thisType == typeof(RectTransform) ||
                thisType == typeof(Transform) ||
                thisType == typeof(CanvasRenderer))
                continue;

            UnityEditorInternal.ComponentUtility.CopyComponent(components[i]);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(tmp.gameObject);

            componentsCopied++;
        }

        if (componentsCopied == 0)
            Undo.DestroyObjectImmediate((Object)uiText.gameObject);
        else
        {
            EditorUtility.DisplayDialog(
                "uGUI to TextMesh Pro",
                string.Format(
                    "{0} components copied. Please check for accuracy as some references may not transfer properly.",
                    componentsCopied),
                "OK",
                "");
            uiText.name += " OLD";
            uiText.gameObject.SetActive(false);
        }
        */
    }


    private static FontStyles GetTmpFontStyle(FontStyle uGuiFontStyle)
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


    private static TextAlignmentOptions GetTmpAlignment(TextAnchor uGuiAlignment)
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
