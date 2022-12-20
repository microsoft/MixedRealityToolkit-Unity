// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Microsoft.MixedReality.GraphicsTools;
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

        // CanvasBackplate.mat
        // Backplate material for menu plates.
        private static readonly string PlateMaterialPath = AssetDatabase.GUIDToAssetPath("65972ebbfd5c529479f9c30fd3ec3f6a");

        // Reflection into internal UGUI editor utilities.
        private static System.Reflection.MethodInfo PlaceUIElementRoot = null;

        private static GameObject SetupElement(GameObject gameObject, MenuCommand menuCommand)
        {

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

            // If the canvas's only child is us; let's make sure the Canvas has reasonable starting defaults.
            // Otherwise, it was probably an existing canvas we were added to, so we shouldn't mess with it.
            if (rt.childCount == 1 && rt.GetChild(0) == gameObject.transform)
            {
                SetReasonableCanvasDefaults(canvas);
                
                // Reset our own object to zero-position relative to the parent canvas.
                gameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }

            return gameObject;
        }

        private static GameObject CreateElementFromPath(string path, MenuCommand menuCommand)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            GameObject gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);

            return SetupElement(gameObject, menuCommand);
        }

        private static void SetReasonableCanvasDefaults(Canvas canvas)
        {
            RectTransform rt = canvas.GetComponent<RectTransform>();

            // 1mm : 1 unit measurement ratio.
            if (rt.lossyScale != Vector3.one * 0.001f)
            {
                rt.localScale = Vector3.one * 0.001f;
            }
            // 150mm x 150mm.
            rt.sizeDelta = Vector2.one * 150.0f;

            // All our canvases will be worldspace (by default.)
            canvas.renderMode = RenderMode.WorldSpace;
            Undo.RecordObject(canvas, "Set Canvas RenderMode to WorldSpace");

            // 30cm in front of the camera.
            rt.position = Camera.main.transform.position + Camera.main.transform.forward * 0.3f;
            Undo.RecordObject(rt, "Set Canvas Position");

            // No GraphicRaycaster by default. Users can add one, if they like.
            if (canvas.TryGetComponent(out GraphicRaycaster raycaster))
            {
                Undo.DestroyObjectImmediate(raycaster);
            }

            // CanvasScaler should be there by default.
            if (!canvas.TryGetComponent(out CanvasScaler _))
            {
                Undo.AddComponent<CanvasScaler>(canvas.gameObject);
            }
        }

        [MenuItem("GameObject/UI/MRTK/Canvas", false, 0)]
        private static void CreateEmptyCanvas(MenuCommand menuCommand)
        {
            Undo.SetCurrentGroupName("Create Canvas");
            int group = Undo.GetCurrentGroup();

            GameObject gameObject = new GameObject("Canvas");
            Undo.RegisterCreatedObjectUndo(gameObject, "Create blank MRTK Canvas");

            Canvas canvas = Undo.AddComponent<Canvas>(gameObject);
            SetReasonableCanvasDefaults(canvas);

            Undo.CollapseUndoOperations(group);
        }

        [MenuItem("GameObject/UI/MRTK/Canvas + Graphic Raycasting", false, 0)]
        private static void CreateGraphicRaycastingCanvas(MenuCommand menuCommand)
        {
            Undo.SetCurrentGroupName("Create Canvas (Raycasting-enabled)");
            int group = Undo.GetCurrentGroup();

            GameObject gameObject = new GameObject("Canvas");
            Undo.RegisterCreatedObjectUndo(gameObject, "Create MRTK Canvas with Graphic Raycasting");

            Canvas canvas = Undo.AddComponent<Canvas>(gameObject);
            SetReasonableCanvasDefaults(canvas);

            Undo.AddComponent<GraphicRaycaster>(gameObject);
            Undo.AddComponent<TrackedDeviceGraphicRaycaster>(gameObject);

            Undo.CollapseUndoOperations(group);
        }

        // TODO: This may end up being prefabified at some point. Also TODO,
        // ensure this gets theming scripts when that system is ready.
        [MenuItem("GameObject/UI/MRTK/Plate", false, 0)]
        private static GameObject CreatePlate(MenuCommand menuCommand)
        {   
            Undo.SetCurrentGroupName("Create Plate");
            int group = Undo.GetCurrentGroup();

            GameObject gameObject = new GameObject("Plate", typeof(CanvasElementRoundedRect));
            

            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);

            // gameObject.transform.SetParent((menuCommand.context as GameObject).transform, false);
            
            SetupElement(gameObject, menuCommand);

            CanvasElementRoundedRect roundedRect = gameObject.GetComponent<CanvasElementRoundedRect>();

            roundedRect.raycastTarget = false;
            roundedRect.material = AssetDatabase.LoadAssetAtPath(PlateMaterialPath, typeof(Material)) as Material;
            roundedRect.Radius = 13.0f;
            roundedRect.Thickness = 2.0f;
            roundedRect.Wedges = 8;
            roundedRect.SmoothEdges = true;
            Undo.RecordObject(roundedRect, "Set Plate RoundedRect properties");


            Undo.CollapseUndoOperations(group);

            return gameObject;
        }
        
        [MenuItem("GameObject/UI/MRTK/Action Button", false, 0)]
        private static void CreateActionButton(MenuCommand menuCommand)
        {
            CreateElementFromPath(ActionButtonPath, menuCommand);
        }

        [MenuItem("GameObject/UI/MRTK/Action Button (Wide)", false, 1)]
        private static GameObject CreateActionButtonWide(MenuCommand menuCommand)
        {
            Undo.SetCurrentGroupName("Create Action Button (wide)");
            int group = Undo.GetCurrentGroup();

            GameObject gameObject = CreateElementFromPath(ActionButtonPath, menuCommand);

            RectTransform rt = gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(128.0f, 32.0f);
            Undo.RecordObject(rt, "Set Action Button (Wide) size");
            LayoutElement le = gameObject.GetComponent<LayoutElement>();
            le.minWidth = 128.0f;
            Undo.RecordObject(le, "Set Action Button (Wide) min width");

            var text = gameObject.GetComponentsInChildren<TMP_Text>(true).Where(t => t.name == "Text").First();
            text.gameObject.SetActive(true);
            text.alignment = TextAlignmentOptions.Left;
            text.text = "<size=8>Header</size><size=6>\n<alpha=#88>Meta text goes here</size>";
            Undo.RecordObject(text, "Set Action Button (Wide) text");

            PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);

            return gameObject;
        }

        [MenuItem("GameObject/UI/MRTK/Empty Button", false, 2)]
        private static void CreateEmptyButton(MenuCommand menuCommand)
        {
            CreateElementFromPath(EmptyButtonPath, menuCommand);
        }

        [MenuItem("GameObject/UI/MRTK/List Menu", false, 0)]
        private static void CreateListMenu(MenuCommand menuCommand)
        {   
            Undo.SetCurrentGroupName("Create ListMenu");
            int group = Undo.GetCurrentGroup();

            var plate = CreatePlate(menuCommand);

            var layout = Undo.AddComponent<VerticalLayoutGroup>(plate);
            layout.padding = new RectOffset(4, 4, 4, 4);
            Undo.RecordObject(layout, "Set ListMenu VerticalLayoutGroup properties");

            var fitter = Undo.AddComponent<ContentSizeFitter>(plate);
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            Undo.RecordObject(fitter, "Set ListMenu ContentSizeFitter properties");

            for (int i = 0; i < 4; i++)
            {
                var button = CreateActionButtonWide(menuCommand);
                Undo.SetTransformParent(button.transform, plate.transform, "Reparent button to plate");

            }

            Undo.CollapseUndoOperations(group);
        }
    }
}
