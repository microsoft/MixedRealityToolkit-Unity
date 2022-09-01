// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Reflection;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Add-GameObject-menus for creating common MRTK UX components. Most of these
    /// spawn prefabs, but some are just ad-hoc objects created in code (like panels,
    /// backplates, etc).
    /// </summary>
    static internal class CreateElementMenus
    {
        // EmptyButton.prefab
        // A completely empty button, with no icon, text, label, etc.
        private static readonly string EmptyButtonPath = AssetDatabase.GUIDToAssetPath("b85e005d231192249b7077b40a4d4e45");

        // ActionButton.prefab
        // The basic building block button; contains an icon, text, and label.
        private static readonly string ActionButtonPath = AssetDatabase.GUIDToAssetPath("c6b351a67ceb69140b199996bbbea156");

        // Reflection into internal UGUI editor utilities.
        private static System.Reflection.MethodInfo PlaceUIElementRoot = null;

        private static GameObject CreateElement(string path, MenuCommand menuCommand)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            GameObject gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);

            // This is evil :)
            // UGUI contains plenty of helper utilities for spawning and managing new Canvas objects
            // at edit-time. Unfortunately, they're all internal, so we have to use reflection to
            // access them.
            if (PlaceUIElementRoot == null)
            {
                // We're using SelectableEditor type here to grab the assembly instead of going
                // and hunting down the assembly ourselves. It's a bit more convenient and durable.
                PlaceUIElementRoot = typeof(SelectableEditor).Assembly.GetType("UnityEditor.UI.MenuOptions")?.GetMethod(
                                                "PlaceUIElementRoot",
                                                System.Reflection.BindingFlags.NonPublic |
                                                System.Reflection.BindingFlags.Static );
                if (PlaceUIElementRoot == null)
                {
                    Debug.LogError("Whoops! Looks like Unity changed the internals of their UGUI editor utilities. Please file a bug!");
                    // Return early; we can't do anything else.
                    return gameObject;
                }
            }

            PlaceUIElementRoot.Invoke(null, new object[] { gameObject, menuCommand});

            // The above call will create a new Canvas for us (if we don't have one),
            // but it won't have optimal settings for MRTK UX. Let's fix that!
            Canvas canvas = gameObject.GetComponentInParent<Canvas>();
            RectTransform rt = canvas.GetComponent<RectTransform>();

            // If the canvas's only child is us; let's resize the canvas to be a decent starting size.
            // Otherwise, it was probably an existing canvas we were added to, so we shouldn't mess with it.
            if (rt.childCount == 1 && rt.GetChild(0) == gameObject.transform)
            {
                // 1mm : 1 unit measurement ratio.
                if (rt.lossyScale != Vector3.one * 0.001f)
                {
                    rt.localScale = Vector3.one * 0.001f;
                }

                // 150mm x 150mm.
                rt.sizeDelta = Vector2.one * 150.0f;

                // All our canvases will be worldspace (by default.)
                canvas.renderMode = RenderMode.WorldSpace;

                // 30cm in front of the camera.
                rt.position = Camera.main.transform.position + Camera.main.transform.forward * 0.3f;
                gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

                // PlaceUIElementRoot will have created a GraphicRaycaster for us.
                // We don't want that (at least by default)
                GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
                if (raycaster == null)
                {
                    UnityEngine.Object.Destroy(raycaster);
                }
            }

            return gameObject;
        }
        
        [MenuItem("GameObject/UI/MRTK/Action Button", false, 0)]
        private static void CreateActionButton(MenuCommand menuCommand)
        {
            CreateElement(ActionButtonPath, menuCommand);
        }

        [MenuItem("GameObject/UI/MRTK/Action Button (Wide)", false, 1)]
        private static void CreateActionButtonWide(MenuCommand menuCommand)
        {
            GameObject gameObject = CreateElement(ActionButtonPath, menuCommand);

            RectTransform rt = gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(128.0f, 32.0f);
            LayoutElement le = gameObject.GetComponent<LayoutElement>();
            le.minWidth = 128.0f;

            var text = gameObject.GetComponentsInChildren<TMP_Text>(true).Where(t => t.name == "Text").First();
            text.gameObject.SetActive(true);
            text.alignment = TextAlignmentOptions.Left;
            text.text = "<size=8>Header</size><size=6>\n<alpha=#88>Meta text goes here</size>";

            PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
        }

        [MenuItem("GameObject/UI/MRTK/Empty Button", false, 2)]
        private static void CreateEmptyButton(MenuCommand menuCommand)
        {
            CreateElement(EmptyButtonPath, menuCommand);
        }
    }
}