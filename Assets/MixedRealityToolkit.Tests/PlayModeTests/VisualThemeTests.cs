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
        public IEnumerator TestTextureTheme()
        {
            const string TexturePathState0 = @"Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Textures/Panel_albedo.png";
            Texture texState0 = AssetDatabase.LoadAssetAtPath<Texture>(TexturePathState0);
            const string TexturePathState1 = @"Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Textures/Checker_albedo.png";
            Texture texState1 = AssetDatabase.LoadAssetAtPath<Texture>(TexturePathState1);

            GameObject targetHost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Renderer targetRenderer = targetHost.GetComponent<Renderer>();
            var block = new MaterialPropertyBlock();

            var textureThemeDefinition = ThemeDefinition.GetDefaultThemeDefinition<InteractableTextureTheme>().Value;
            textureThemeDefinition.StateProperties[0].Values = new List<ThemePropertyValue>()
            {
                new ThemePropertyValue() { Texture = texState0 },
                new ThemePropertyValue() { Texture = texState1 },
            };

            var textureTheme = InteractableThemeBase.CreateAndInitTheme(textureThemeDefinition, targetHost);

            textureTheme.OnUpdate(0);
            yield return null;
            targetRenderer.GetPropertyBlock(block);
            Assert.AreEqual(texState0, block.GetTexture("_MainTex"));

            textureTheme.OnUpdate(1);
            yield return null;
            targetRenderer.GetPropertyBlock(block);
            Assert.AreEqual(texState1, block.GetTexture("_MainTex"));

            // Cleanup
            GameObjectExtensions.DestroyGameObject(targetHost);
        }

        [UnityTest]
        public IEnumerator TestStringTheme()
        {
            yield return TestStringTheme<Text>((theme) =>
            {
                return theme.Host.GetComponent<Text>().text;
            });

            yield return TestStringTheme<TextMesh>((theme) =>
            {
                return theme.Host.GetComponent<TextMesh>().text;
            });

            yield return TestStringTheme<TMPro.TextMeshPro>((theme) =>
            {
                return theme.Host.GetComponent<TMPro.TextMeshPro>().text;
            });

            yield return TestStringTheme<TMPro.TextMeshProUGUI>((theme) =>
            {
                return theme.Host.GetComponent<TMPro.TextMeshProUGUI>().text;
            });
        } 

        private IEnumerator TestStringTheme<T>(Func<InteractableThemeBase, string> resultTest) where T : UnityEngine.Component
        {
            const string State0 = "TestState0";
            const string State1 = "TestState1";

            var targetHost = new GameObject("TestObject");
            T textMesh = targetHost.AddComponent<T>();

            var stringThemeDefinition = ThemeDefinition.GetDefaultThemeDefinition<InteractableStringTheme>().Value;
            stringThemeDefinition.StateProperties[0].Values = new List<ThemePropertyValue>()
            {
                new ThemePropertyValue() { String = State0 },
                new ThemePropertyValue() { String = State1 },
            };

            var stringTheme = InteractableThemeBase.CreateAndInitTheme(stringThemeDefinition, targetHost);

            stringTheme.OnUpdate(0);
            yield return null;
            Assert.AreEqual(State0, resultTest(stringTheme));

            stringTheme.OnUpdate(1);
            yield return null;
            Assert.AreEqual(State1, resultTest(stringTheme));

            // Cleanup
            GameObjectExtensions.DestroyGameObject(targetHost);
        }
    }
}
#endif