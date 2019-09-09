// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class VisualThemeTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        [UnityTest]
        public IEnumerator TestActivateTheme()
        {
            bool state0 = false;
            bool state1 = true;

            var defaultStateValues = new List<ThemePropertyValue>()
            {
                new ThemePropertyValue() { Bool = state0 },
                new ThemePropertyValue() { Bool = state1 },
            };

            yield return TestTheme<InteractableActivateTheme, Component>(defaultStateValues,
                (theme) =>
                {
                    Assert.AreEqual(state0, theme.Host.activeInHierarchy);
                },
                (theme) =>
                {
                    Assert.AreEqual(state1, theme.Host.activeInHierarchy);
                });
        }

        [UnityTest]
        public IEnumerator TestMaterialTheme()
        {
            Material state0 = new Material(Shader.Find("Mixed Reality Toolkit/Standard"));
            Material state1 = new Material(Shader.Find("Standard"));

            var defaultStateValues = new List<ThemePropertyValue>()
            {
                new ThemePropertyValue() { Material = state0 },
                new ThemePropertyValue() { Material = state1 },
            };

            yield return TestTheme<InteractableMaterialTheme, MeshRenderer>(defaultStateValues,
                (theme) =>
                {
                    Assert.AreEqual(state0, theme.Host.GetComponent<Renderer>().sharedMaterial);
                },
                (theme) =>
                {
                    Assert.AreEqual(state1, theme.Host.GetComponent<Renderer>().sharedMaterial);
                });
        }

        [UnityTest]
        public IEnumerator TestColorTheme()
        {
            Color state0Color = Color.red;
            Color state1Color = Color.blue;

            var defaultStateValues = new List<ThemePropertyValue>()
            {
                new ThemePropertyValue() { Color = state0Color },
                new ThemePropertyValue() { Color = state1Color },
            };

            yield return TestShaderTheme<InteractableColorTheme>(defaultStateValues,
                (block) =>
                {
                    Assert.AreEqual(state0Color, block.GetColor("_Color"));
                },
                (block) =>
                {
                    Assert.AreEqual(state1Color, block.GetColor("_Color"));
                });
        }

        [UnityTest]
        public IEnumerator TestTextureTheme()
        {
            const string TexturePathState0 = @"Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Textures/Panel_albedo.png";
            Texture texState0 = AssetDatabase.LoadAssetAtPath<Texture>(TexturePathState0);

            const string TexturePathState1 = @"Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Textures/Checker_albedo.png";
            Texture texState1 = AssetDatabase.LoadAssetAtPath<Texture>(TexturePathState1);

            var defaultStateValues = new List<ThemePropertyValue>()
            {
                new ThemePropertyValue() { Texture = texState0 },
                new ThemePropertyValue() { Texture = texState1 },
            };

            yield return TestShaderTheme<InteractableTextureTheme>(defaultStateValues,
                (block) =>
                {
                    Assert.AreEqual(texState0, block.GetTexture("_MainTex"));
                },
                (block) =>
                {
                    Assert.AreEqual(texState1, block.GetTexture("_MainTex"));
                });
        }

        [UnityTest]
        public IEnumerator TestStringTheme()
        {
            const string State0 = "TestState0";
            const string State1 = "TestState1";

            var defaultStateValues = new List<ThemePropertyValue>()
            {
                new ThemePropertyValue() { String = State0 },
                new ThemePropertyValue() { String = State1 },
            };

            yield return TestTheme<InteractableStringTheme, Text>(defaultStateValues,
                (theme) =>
                {
                    Assert.AreEqual(State0, theme.Host.GetComponent<Text>().text);
                },
                (theme) =>
                {
                    Assert.AreEqual(State1, theme.Host.GetComponent<Text>().text);
                });

            yield return TestTheme<InteractableStringTheme, TextMesh>(defaultStateValues,
                (theme) =>
                {
                    Assert.AreEqual(State0, theme.Host.GetComponent<TextMesh>().text);
                },
                (theme) =>
                {
                    Assert.AreEqual(State1, theme.Host.GetComponent<TextMesh>().text);
                });

            yield return TestTheme<InteractableStringTheme, TMPro.TextMeshPro>(defaultStateValues,
                (theme) =>
                {
                    Assert.AreEqual(State0, theme.Host.GetComponent<TMPro.TextMeshPro>().text);
                },
                (theme) =>
                {
                    Assert.AreEqual(State1, theme.Host.GetComponent<TMPro.TextMeshPro>().text);
                });

            yield return TestTheme<InteractableStringTheme, TMPro.TextMeshProUGUI>(defaultStateValues,
                (theme) =>
                {
                    Assert.AreEqual(State0, theme.Host.GetComponent<TMPro.TextMeshProUGUI>().text);
                },
                (theme) =>
                {
                    Assert.AreEqual(State1, theme.Host.GetComponent<TMPro.TextMeshProUGUI>().text);
                });
        }

        private IEnumerator TestTheme<T, C>(
            List<ThemePropertyValue> stateValues,
            Action<InteractableThemeBase> state0Test,
            Action<InteractableThemeBase> state1Test) 
            where T : InteractableThemeBase
            where C : UnityEngine.Component
        {
            var targetHost = new GameObject("TestObject");
            targetHost.AddComponent<C>();

            var themeDefinition = ThemeDefinition.GetDefaultThemeDefinition<T>().Value;
            themeDefinition.StateProperties[0].Values = stateValues;

            var theme = InteractableThemeBase.CreateAndInitTheme(themeDefinition, targetHost);

            theme.OnUpdate(0);
            yield return null;
            state0Test(theme);

            theme.OnUpdate(1);
            yield return null;
            state1Test(theme);

            // Cleanup
            GameObjectExtensions.DestroyGameObject(targetHost);
        }

        private IEnumerator TestShaderTheme<T>(List<ThemePropertyValue> stateValues, 
            Action<MaterialPropertyBlock> state0Test, 
            Action<MaterialPropertyBlock> state1Test) where T : InteractableThemeBase
        {
            GameObject targetHost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Renderer targetRenderer = targetHost.GetComponent<Renderer>();
            var block = new MaterialPropertyBlock();

            var themeDefinition = ThemeDefinition.GetDefaultThemeDefinition<T>().Value;
            themeDefinition.StateProperties[0].Values = stateValues;

            var theme = InteractableThemeBase.CreateAndInitTheme(themeDefinition, targetHost);

            theme.OnUpdate(0);
            yield return null;
            targetRenderer.GetPropertyBlock(block);
            state0Test(block);

            theme.OnUpdate(1);
            yield return null;
            targetRenderer.GetPropertyBlock(block);
            state1Test(block);

            // Cleanup
            GameObjectExtensions.DestroyGameObject(targetHost);
        }
    }
}
#endif