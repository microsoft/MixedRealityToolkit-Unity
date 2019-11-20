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
        private const string DefaultColorProperty = "_Color";

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

        #region Tests

        [UnityTest]
        public IEnumerator TestActivateTheme()
        {
            bool state0 = false;
            bool state1 = true;

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Bool = state0 },
                    new ThemePropertyValue() { Bool = state1 },
                }
            };

            yield return TestTheme<InteractableActivateTheme, MeshRenderer>(defaultStateValues,
                (theme) => { Assert.AreEqual(state0, theme.Host.activeInHierarchy); },
                (theme) => { Assert.AreEqual(state1, theme.Host.activeInHierarchy); });
        }

        [UnityTest]
        public IEnumerator TestMaterialTheme()
        {
            Material state0 = new Material(StandardShaderUtility.MrtkStandardShader);
            Material state1 = new Material(Shader.Find("Standard"));

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Material = state0 },
                    new ThemePropertyValue() { Material = state1 },
                }
            };

            yield return TestTheme<InteractableMaterialTheme, MeshRenderer>(defaultStateValues,
                (theme) => { Assert.AreEqual(state0, theme.Host.GetComponent<Renderer>().sharedMaterial); },
                (theme) => { Assert.AreEqual(state1, theme.Host.GetComponent<Renderer>().sharedMaterial); });
        }

        [UnityTest]
        public IEnumerator TestOffsetTheme()
        {
            Vector3 state0 = Vector3.forward;
            Vector3 state1 = Vector3.down;

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Vector3 = state0 },
                    new ThemePropertyValue() { Vector3 = state1 },
                }
            };

            yield return TestTheme<InteractableOffsetTheme, MeshRenderer>(defaultStateValues,
                (theme) => { Assert.AreEqual(state0, theme.Host.transform.position); },
                (theme) => { Assert.AreEqual(state1, theme.Host.transform.position); });
        }

        [UnityTest]
        public IEnumerator TestRotationTheme()
        {
            Vector3 state0 = Quaternion.LookRotation(Vector3.up).eulerAngles;
            Vector3 state1 = Quaternion.LookRotation(Vector3.down).eulerAngles;

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Vector3 = state0 },
                    new ThemePropertyValue() { Vector3 = state1 },
                }
            };

            yield return TestTheme<InteractableRotationTheme, MeshRenderer>(defaultStateValues,
                (theme) => { Assert.AreEqual(state0, theme.Host.transform.rotation.eulerAngles); },
                (theme) => { Assert.AreEqual(state1, theme.Host.transform.rotation.eulerAngles); });
        }

        [UnityTest]
        public IEnumerator TestScaleTheme()
        {
            Vector3 state0 = Vector3.one * 4.0f;
            Vector3 state1 = Vector3.one * 0.5f;

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Vector3 = state0 },
                    new ThemePropertyValue() { Vector3 = state1 },
                }
            };

            yield return TestTheme<InteractableScaleTheme, MeshRenderer>(defaultStateValues,
                (theme) => { Assert.AreEqual(state0, theme.Host.transform.localScale); },
                (theme) => { Assert.AreEqual(state1, theme.Host.transform.localScale); });
        }

        [UnityTest]
        public IEnumerator TestScaleOffsetColorTheme()
        {
            Vector3 state0 = Vector3.one * 4.0f;
            Vector3 state1 = Vector3.one * 0.5f;
            Vector3 state0Offset = Vector3.up;
            Vector3 state1Offset = Vector3.down;
            Color state0Color = Color.red;
            Color state1Color = Color.blue;

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>() // Scale
                {
                    new ThemePropertyValue() { Vector3 = state0 },
                    new ThemePropertyValue() { Vector3 = state1 },
                },
                new List<ThemePropertyValue>() // Offset
                {
                    new ThemePropertyValue() { Vector3 = state0Offset },
                    new ThemePropertyValue() { Vector3 = state1Offset },
                },
                new List<ThemePropertyValue>() //Color
                {
                    new ThemePropertyValue() { Color = state0Color },
                    new ThemePropertyValue() { Color = state1Color },
                }
            };

            yield return TestTheme<ScaleOffsetColorTheme, MeshRenderer>(defaultStateValues,
                (theme) => 
                {
                    Assert.AreEqual(state0, theme.Host.transform.localScale);
                    Assert.AreEqual(state0Offset, theme.Host.transform.position);
                },
                (theme) => 
                {
                    Assert.AreEqual(state1, theme.Host.transform.localScale);
                    Assert.AreEqual(state1Offset, theme.Host.transform.position);
                });

            yield return TestShaderTheme<ScaleOffsetColorTheme>(defaultStateValues,
                (block) => { Assert.AreEqual(state0Color, block.GetColor(DefaultColorProperty)); },
                (block) => { Assert.AreEqual(state1Color, block.GetColor(DefaultColorProperty)); });
        }

        [UnityTest]
        public IEnumerator TestColorTheme()
        {
            Color state0Color = Color.red;
            Color state1Color = Color.blue;

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Color = state0Color },
                    new ThemePropertyValue() { Color = state1Color },
                }
            };

            yield return TestShaderTheme<InteractableColorTheme>(defaultStateValues,
                (block) => { Assert.AreEqual(state0Color, block.GetColor(DefaultColorProperty)); },
                (block) => { Assert.AreEqual(state1Color, block.GetColor(DefaultColorProperty)); });
        }

        [UnityTest]
        public IEnumerator TestColorChildrenTheme()
        {
            Color state0Color = Color.red;
            Color state1Color = Color.blue;

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Color = state0Color },
                    new ThemePropertyValue() { Color = state1Color },
                }
            };

            const int numOfChildren = 3;
            var parent = new GameObject("Parent");
            for (int i = 0; i < numOfChildren; i++)
            {
                GameObject.CreatePrimitive(PrimitiveType.Cube).transform.parent = parent.transform;
            }

            yield return TestTheme<InteractableColorChildrenTheme, AudioSource>(parent,
                defaultStateValues,
                (theme) =>
                {
                    var block = new MaterialPropertyBlock();
                    foreach (Transform child in theme.Host.transform)
                    {
                        child.GetComponent<Renderer>().GetPropertyBlock(block);
                        Assert.AreEqual(state0Color, block.GetColor(DefaultColorProperty));
                    }
                },
                (theme) =>
                {
                    var block = new MaterialPropertyBlock();
                    foreach (Transform child in theme.Host.transform)
                    {
                        child.GetComponent<Renderer>().GetPropertyBlock(block);
                        Assert.AreEqual(state1Color, block.GetColor(DefaultColorProperty));
                    }
                });
        }

        [UnityTest]
        public IEnumerator TestTextureTheme()
        {
            const string TexturePathState0 = @"Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Textures/Panel_albedo.png";
            Texture texState0 = AssetDatabase.LoadAssetAtPath<Texture>(TexturePathState0);

            const string TexturePathState1 = @"Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Textures/Checker_albedo.png";
            Texture texState1 = AssetDatabase.LoadAssetAtPath<Texture>(TexturePathState1);

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Texture = texState0 },
                    new ThemePropertyValue() { Texture = texState1 },
                }
            };

            yield return TestShaderTheme<InteractableTextureTheme>(defaultStateValues,
                (block) => { Assert.AreEqual(texState0, block.GetTexture("_MainTex")); },
                (block) => { Assert.AreEqual(texState1, block.GetTexture("_MainTex")); });
        }

        [UnityTest]
        public IEnumerator TestStringTheme()
        {
            const string State0 = "TestState0";
            const string State1 = "TestState1";

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { String = State0 },
                    new ThemePropertyValue() { String = State1 },
                }
            };

            yield return TestTheme<InteractableStringTheme, Text>(defaultStateValues,
                (theme) => { Assert.AreEqual(State0, theme.Host.GetComponent<Text>().text); },
                (theme) => { Assert.AreEqual(State1, theme.Host.GetComponent<Text>().text); });

            yield return TestTheme<InteractableStringTheme, TextMesh>(defaultStateValues,
                (theme) => { Assert.AreEqual(State0, theme.Host.GetComponent<TextMesh>().text); },
                (theme) => { Assert.AreEqual(State1, theme.Host.GetComponent<TextMesh>().text); });

            yield return TestTheme<InteractableStringTheme, TMPro.TextMeshPro>(defaultStateValues,
                (theme) => { Assert.AreEqual(State0, theme.Host.GetComponent<TMPro.TextMeshPro>().text); },
                (theme) => { Assert.AreEqual(State1, theme.Host.GetComponent<TMPro.TextMeshPro>().text); });

            yield return TestTheme<InteractableStringTheme, TMPro.TextMeshProUGUI>(defaultStateValues,
                (theme) => { Assert.AreEqual(State0, theme.Host.GetComponent<TMPro.TextMeshProUGUI>().text); },
                (theme) => { Assert.AreEqual(State1, theme.Host.GetComponent<TMPro.TextMeshProUGUI>().text); });
        }

        #endregion

        #region Helpers

        private IEnumerator TestTheme<T, C>(
            List<List<ThemePropertyValue>> stateValues,
            params Action<InteractableThemeBase>[] stateTests)
            where T : InteractableThemeBase
            where C : UnityEngine.Component
        {
            yield return TestTheme<T,C>(new GameObject("TestObject"), stateValues, stateTests);
        }

        private IEnumerator TestTheme<T, C>(
            GameObject host,
            List<List<ThemePropertyValue>> stateValues,
            params Action<InteractableThemeBase>[] stateTests)
            where T : InteractableThemeBase
            where C : UnityEngine.Component
        {
            foreach (var values in stateValues)
            {
                Assert.AreEqual(values.Count, stateTests.Length);
            }

            var themeDefinition = ThemeDefinition.GetDefaultThemeDefinition<T>().Value;
            for (int i = 0; i < stateValues.Count; i++)
            {
                themeDefinition.StateProperties[i].Values = stateValues[i];
            }

            yield return TestTheme<C>(host, themeDefinition, stateTests);
        }

        private IEnumerator TestTheme<C>(
            GameObject host,
            ThemeDefinition themeDefinition,
            params Action<InteractableThemeBase>[] stateTests)
            where C : UnityEngine.Component
        {
            host.AddComponent<C>();

            var theme = InteractableThemeBase.CreateAndInitTheme(themeDefinition, host);

            for (int i = 0; i < stateTests.Length; i++)
            {
                theme.OnUpdate(i);
                yield return null;
                stateTests[i](theme);
            }
            GameObjectExtensions.DestroyGameObject(host);
        }

        private IEnumerator TestShaderTheme<T>(
            List<List<ThemePropertyValue>> stateValues, 
            params Action<MaterialPropertyBlock>[] stateTests)
            where T : InteractableThemeBase
        {
            GameObject targetHost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Renderer targetRenderer = targetHost.GetComponent<Renderer>();
            var block = new MaterialPropertyBlock();

            Action<InteractableThemeBase>[] convertedStateTests = new Action<InteractableThemeBase>[stateTests.Length];
            for (int i = 0; i < stateTests.Length; i++)
            {
                var index = i;
                convertedStateTests[i] = (theme) =>
                {
                    targetRenderer.GetPropertyBlock(block);
                    stateTests[index](block);
                };
            }

            yield return TestTheme<T, AudioSource>(targetHost, stateValues,convertedStateTests);
        }

        #endregion
    }
}
#endif