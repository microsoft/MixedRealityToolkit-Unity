// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ButtonConfigHelperTests : BasePlayModeTests
    {
        [UnityTest]
        /// <summary>
        /// Test adding a config helper to a game object and attempting to modify it.
        /// </summary>
        public IEnumerator TestAddButtonConfigHelperAtRuntime()
        {
            GameObject newGameObject = new GameObject("ButtonTest");
            ButtonConfigHelper bch = newGameObject.AddComponent<ButtonConfigHelper>();

            bch.MainLabelText = "Test Text";
            bch.SeeItSayItLabelText = "Test Text";
            bch.IconStyle = ButtonIconStyle.Char;
            bch.IconStyle = ButtonIconStyle.None;
            bch.IconStyle = ButtonIconStyle.Quad;
            bch.IconStyle = ButtonIconStyle.Sprite;
            bch.SeeItSayItLabelEnabled = false;
            bch.SetCharIcon(0);
            bch.SetQuadIcon(null);
            bch.SetSpriteIcon(null);
            bch.SetCharIconByName("EmptyIcon");
            bch.SetQuadIconByName("EmptyIcon");
            bch.SetSpriteIconByName("EmptyIcon");
            bch.ForceRefresh();
            bch.OnClick.AddListener(() => { Debug.Log("OnClick"); });
            bch.IconSet = null;

            yield break;
        }

        [UnityTest]
        /// <summary>
        /// Test creating a button icon set and inserting null elements.
        /// </summary>
        public IEnumerator TestBadlyConfiguredIconSet()
        {
            // Create a button icon set and insert arrays with null references
            ButtonIconSet buttonIconSet = ScriptableObject.CreateInstance<ButtonIconSet>();

            SerializedObject buttonIconSetObject = new SerializedObject(buttonIconSet);
            SerializedProperty quadIconsProp = buttonIconSetObject.FindProperty("quadIcons");
            SerializedProperty spriteIconsProp = buttonIconSetObject.FindProperty("spriteIcons");
            SerializedProperty charIconsProp = buttonIconSetObject.FindProperty("charIcons");

            quadIconsProp.InsertArrayElementAtIndex(0);
            spriteIconsProp.InsertArrayElementAtIndex(0);
            charIconsProp.InsertArrayElementAtIndex(0);
            buttonIconSetObject.ApplyModifiedProperties();

            // These calls should not fail even if we have null / empty elements.
            Assert.IsFalse(buttonIconSet.TryGetQuadIcon("EmptyIcon", out _));
            Assert.IsFalse(buttonIconSet.TryGetSpriteIcon("EmptyIcon", out _));
            Assert.IsFalse(buttonIconSet.TryGetCharIcon("EmptyIcon", out _));
            yield break;
        }

        [UnityTest]
        /// <summary>
        /// Ensure a newly created button icon set instance contains all default textures / characters.
        /// </summary>
        public IEnumerator TestDefaultIconSetInstance()
        {
            // Create a button icon set and insert arrays with null references
            ButtonIconSet buttonIconSet = ScriptableObject.CreateInstance<ButtonIconSet>();

            // The icon set will nave no quad icons.
            Assert.IsNotNull(buttonIconSet.QuadIcons);
            Assert.IsTrue(buttonIconSet.QuadIcons.Length == 0);
            Assert.IsFalse(buttonIconSet.TryGetQuadIcon("EmptyIcon", out _));

            // The icon set will nave no sprite icons.
            Assert.IsNotNull(buttonIconSet.SpriteIcons);
            Assert.IsTrue(buttonIconSet.SpriteIcons.Length == 0);
            Assert.IsFalse(buttonIconSet.TryGetSpriteIcon("EmptyIcon", out _));

            // The icon set should have the following icons by default.
            Assert.IsNotNull(buttonIconSet.CharIcons);
            Assert.IsTrue(buttonIconSet.TryGetCharIcon("AppBarAdjust", out _));
            Assert.IsTrue(buttonIconSet.TryGetCharIcon("AppBarClose", out _));
            Assert.IsTrue(buttonIconSet.TryGetCharIcon("AppBarDone", out _));
            Assert.IsTrue(buttonIconSet.TryGetCharIcon("AppBarHide", out _));
            Assert.IsTrue(buttonIconSet.TryGetCharIcon("AppBarShow", out _));
            Assert.IsTrue(buttonIconSet.TryGetCharIcon("AppBarHome", out _));
            yield break;
        }

        /// <summary>
        /// Ensure one of our default button prefabs can have its label and icon configured at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestPressableButtonHololens2Prefab()
        {
            GameObject buttonObject = InstantiateButtonFromPath(Vector3.zero, Quaternion.identity, TestButtonUtilities.PressableHoloLens2PrefabPath);
            ButtonConfigHelper bch = buttonObject.GetComponent<ButtonConfigHelper>();

            bch.MainLabelText = "MainLabelText";
            Assert.AreEqual(bch.MainLabelText, "MainLabelText");

            bch.SeeItSayItLabelText = "SeeItSayItLabelText";
            Assert.AreEqual(bch.SeeItSayItLabelText, "SeeItSayItLabelText");

            bch.IconStyle = ButtonIconStyle.Char;
            bch.IconStyle = ButtonIconStyle.None;
            bch.IconStyle = ButtonIconStyle.Quad;
            bch.IconStyle = ButtonIconStyle.Sprite;
            bch.SeeItSayItLabelEnabled = false;
            bch.SetCharIcon(0);
            bch.SetQuadIcon(null);
            bch.SetSpriteIcon(null);
            bch.SetCharIconByName("EmptyIcon");
            bch.SetQuadIconByName("EmptyIcon");
            bch.SetSpriteIconByName("EmptyIcon");
            bch.ForceRefresh();
            bch.OnClick.AddListener(() => { Debug.Log("OnClick"); });
            bch.IconSet = null;

            yield break;
        }

        private GameObject InstantiateButtonFromPath(Vector3 position, Quaternion rotation, string path)
        {
            // Load interactable prefab
            Object interactablePrefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            GameObject result = Object.Instantiate(interactablePrefab) as GameObject;
            Assert.IsNotNull(result);

            // Move the object into position
            result.transform.position = position;
            result.transform.rotation = rotation;
            return result;
        }
    }
}
#endif