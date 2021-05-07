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
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class VisualThemeTests : BasePlayModeTests
    {
        private const string DefaultColorProperty = "_Color";

        public override IEnumerator Setup()
        {
            yield return base.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;
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
                (host, theme) => { Assert.AreEqual(true, host.activeInHierarchy); },
                (theme) => { Assert.AreEqual(state0, theme.Host.activeInHierarchy); },
                (theme) => { Assert.AreEqual(state1, theme.Host.activeInHierarchy); });
        }

        [UnityTest]
        public IEnumerator TestMaterialTheme()
        {
            // Point Cube diagonally to right
            var hostGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hostGameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Color"));
            var initMaterial = hostGameObject.GetComponent<Renderer>().material;

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

            yield return TestTheme<InteractableMaterialTheme, MeshRenderer>(hostGameObject, defaultStateValues,
                (host, theme) => { Assert.AreEqual(initMaterial, host.GetComponent<Renderer>().material); },
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
                (host, theme) => { Assert.AreEqual(Vector3.zero, host.transform.position); },
                (theme) => { Assert.AreEqual(state0, theme.Host.transform.position); },
                (theme) => { Assert.AreEqual(state1, theme.Host.transform.position); });
        }

        /// <summary>
        /// Test InteractableRotationTheme applied not as local, non-relative rotation to host target
        /// </summary>
        [UnityTest]
        public IEnumerator TestRotationTheme()
        {
            // Point Cube diagonally to right
            var hostGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var initLocalRotation = Quaternion.LookRotation(Vector3.forward + Vector3.right);
            hostGameObject.transform.localRotation = initLocalRotation;

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

            yield return TestTheme<InteractableRotationTheme, MeshRenderer>(hostGameObject, defaultStateValues,
                (host, theme) => { Assert.IsTrue(AreEulerEquals(initLocalRotation.eulerAngles, host.transform.localEulerAngles)); },
                (theme) => { Assert.IsTrue(AreEulerEquals(state0, theme.Host.transform.localEulerAngles)); },
                (theme) => { Assert.IsTrue(AreEulerEquals(state1, theme.Host.transform.localEulerAngles)); });
        }

        /// <summary>
        /// Test InteractableRotationTheme applied not as world space, relative rotation to host target
        /// </summary>
        [UnityTest]
        public IEnumerator TestRotationThemeWorldSpace()
        {
            // Point Cube diagonally to right
            var hostGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var initRotation = Quaternion.LookRotation(Vector3.forward + Vector3.right); ;
            hostGameObject.transform.rotation = initRotation;

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

            // Generate default theme properties for InteractableRotationTheme. 
            // Set Relative Rotation property (index=0) to true so theme values are applied in addition instead of absolutely set
            // Set Local Rotation property (index=1) to false so Euler angles are world space
            var defaultThemeProperties = (new InteractableRotationTheme()).GetDefaultThemeDefinition().CustomProperties;
            defaultThemeProperties[0].Value.Bool = true;
            defaultThemeProperties[1].Value.Bool = false;

            yield return TestTheme<InteractableRotationTheme, MeshRenderer>(hostGameObject, defaultStateValues, defaultThemeProperties,
                (host, theme) => { Assert.IsTrue(AreEulerEquals(initRotation.eulerAngles, host.transform.eulerAngles)); },
                (theme) => { Assert.IsTrue(AreEulerEquals(initRotation.eulerAngles + state0, theme.Host.transform.eulerAngles)); },
                (theme) => { Assert.IsTrue(AreEulerEquals(initRotation.eulerAngles + state1, theme.Host.transform.eulerAngles)); });
        }

        /// <summary>
        /// Tests that the rotation theme with "Relative Rotation" custom property keeps the initial rotation of target GameObject.
        /// </summary>
        [UnityTest]
        public IEnumerator TestRelativeRotationTheme()
        {
            GameObject target = new GameObject();
            Vector3 hostInitialRotation = new Vector3(42f, 130f, 12.5f);
            target.transform.localEulerAngles = hostInitialRotation;

            Vector3 state0 = Quaternion.LookRotation(Vector3.up).eulerAngles;
            Vector3 state1 = Quaternion.LookRotation(Vector3.down).eulerAngles;

            Vector3 expectedState0 = hostInitialRotation + state0;
            Vector3 expectedState1 = hostInitialRotation + state1;

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Vector3 = state0 },
                    new ThemePropertyValue() { Vector3 = state1 },
                }
            };

            List<ThemeProperty> defaultCustomProperties = new List<ThemeProperty>()
            {
                new ThemeProperty()
                {
                    Value = new ThemePropertyValue() { Bool = true },
                }
            };

            yield return TestTheme<InteractableRotationTheme, MeshRenderer>(target, defaultStateValues, defaultCustomProperties,
                (host, theme) => { Assert.IsTrue(AreEulerEquals(hostInitialRotation, host.transform.eulerAngles)); },
                (theme) => { Assert.IsTrue(AreEulerEquals(expectedState0, theme.Host.transform.eulerAngles)); },
                (theme) => { Assert.IsTrue(AreEulerEquals(expectedState1, theme.Host.transform.eulerAngles)); });
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
                (host, theme) => { Assert.AreEqual(Vector3.one, host.transform.localScale); },
                (theme) => { Assert.AreEqual(state0, theme.Host.transform.localScale); },
                (theme) => { Assert.AreEqual(state1, theme.Host.transform.localScale); });
        }

        /// <summary>
        /// Tests that the scale theme with "Relative Scale" custom property also takes into account the initial scale of target GameObject.
        /// </summary>
        [UnityTest]
        public IEnumerator TestRelativeScaleTheme()
        {
            GameObject target = new GameObject();
            Vector3 hostInitialScale = new Vector3(1f, 3f, 0.5f);
            target.transform.localScale = hostInitialScale;

            Vector3 state0 = Vector3.one * 4.0f;
            Vector3 state1 = Vector3.one;

            Vector3 expectedState0 = Vector3.Scale(hostInitialScale, state0);
            Vector3 expectedState1 = Vector3.Scale(hostInitialScale, state1);

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Vector3 = state0 },
                    new ThemePropertyValue() { Vector3 = state1 },
                }
            };

            List<ThemeProperty> defaultCustomProperties = new List<ThemeProperty>()
            {
                new ThemeProperty()
                {
                    Value = new ThemePropertyValue() { Bool = true },
                }
            };

            yield return TestTheme<InteractableScaleTheme, MeshRenderer>(target, defaultStateValues, defaultCustomProperties,
                (host, theme) => { Assert.AreEqual(hostInitialScale, host.transform.localScale); },
                (theme) => { Assert.AreEqual(expectedState0, theme.Host.transform.localScale); },
                (theme) => { Assert.AreEqual(expectedState1, theme.Host.transform.localScale); });
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
                new List<ThemePropertyValue>() // Color
                {
                    new ThemePropertyValue() { Color = state0Color },
                    new ThemePropertyValue() { Color = state1Color },
                }
            };

            yield return TestTheme<ScaleOffsetColorTheme, MeshRenderer>(defaultStateValues,
                (host, theme) =>
                {
                    Assert.AreEqual(Vector3.one, host.transform.localScale);
                    Assert.AreEqual(Vector3.zero, host.transform.position);
                },
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
                (host, theme) => { },
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
                (host, theme) => { Assert.AreEqual(Color.white, host.GetComponent<Renderer>().material.color); },
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
                var childCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                childCube.transform.parent = parent.transform;
                childCube.GetComponent<Renderer>().material.color = Color.white;
            }

            yield return TestTheme<InteractableColorChildrenTheme, AudioSource>(parent,
                defaultStateValues,
                (host, theme) =>
                {
                    foreach (Transform child in host.transform)
                    {
                        Assert.AreEqual(Color.white, child.GetComponent<Renderer>().material.color);
                    }
                },
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
            // Examples/Demos/StandardShader/Textures/Panel_albedo.png
            const string TexturePathState0 = "7b551659cf4349242ba72d82b4f9cdc7";
            Texture texState0 = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(TexturePathState0));

            // Examples/Demos/StandardShader/Textures/Checker_albedo.png
            const string TexturePathState1 = "e2cd08a4d181dcc4ea7beb0992656c7e";
            Texture texState1 = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(TexturePathState1));

            var defaultStateValues = new List<List<ThemePropertyValue>>()
            {
                new List<ThemePropertyValue>()
                {
                    new ThemePropertyValue() { Texture = texState0 },
                    new ThemePropertyValue() { Texture = texState1 },
                }
            };

            yield return TestShaderTheme<InteractableTextureTheme>(defaultStateValues,
                (host, theme) => { Assert.AreEqual(null, host.GetComponent<Renderer>().material.mainTexture); },
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
                (host, theme) => { Assert.AreEqual(string.Empty, host.GetComponent<Text>().text); },
                (theme) => { Assert.AreEqual(State0, theme.Host.GetComponent<Text>().text); },
                (theme) => { Assert.AreEqual(State1, theme.Host.GetComponent<Text>().text); });

            yield return TestTheme<InteractableStringTheme, TextMesh>(defaultStateValues,
                (host, theme) => { Assert.AreEqual(string.Empty, host.GetComponent<TextMesh>().text); },
                (theme) => { Assert.AreEqual(State0, theme.Host.GetComponent<TextMesh>().text); },
                (theme) => { Assert.AreEqual(State1, theme.Host.GetComponent<TextMesh>().text); });

            yield return TestTheme<InteractableStringTheme, TMPro.TextMeshPro>(defaultStateValues,
                (host, theme) => { Assert.AreEqual(null, host.GetComponent<TMPro.TextMeshPro>().text); },
                (theme) => { Assert.AreEqual(State0, theme.Host.GetComponent<TMPro.TextMeshPro>().text); },
                (theme) => { Assert.AreEqual(State1, theme.Host.GetComponent<TMPro.TextMeshPro>().text); });

            yield return TestTheme<InteractableStringTheme, TMPro.TextMeshProUGUI>(defaultStateValues,
                (host, theme) => { Assert.AreEqual(null, host.GetComponent<TMPro.TextMeshProUGUI>().text); },
                (theme) => { Assert.AreEqual(State0, theme.Host.GetComponent<TMPro.TextMeshProUGUI>().text); },
                (theme) => { Assert.AreEqual(State1, theme.Host.GetComponent<TMPro.TextMeshProUGUI>().text); });
        }

        #endregion

        #region Helpers

        private IEnumerator TestTheme<T, C>(
            List<List<ThemePropertyValue>> stateValues,
            Action<GameObject, InteractableThemeBase> resetTest,
            params Action<InteractableThemeBase>[] stateTests)
            where T : InteractableThemeBase
            where C : Component
        {
            yield return TestTheme<T, C>(new GameObject("TestObject"), stateValues, new List<ThemeProperty>() { }, resetTest, stateTests);
        }

        private IEnumerator TestTheme<T, C>(
            List<List<ThemePropertyValue>> stateValues,
            List<ThemeProperty> customProperties,
            Action<GameObject, InteractableThemeBase> resetTest,
            params Action<InteractableThemeBase>[] stateTests)
            where T : InteractableThemeBase
            where C : Component
        {
            yield return TestTheme<T, C>(new GameObject("TestObject"), stateValues, customProperties, resetTest, stateTests);
        }

        private IEnumerator TestTheme<T, C>(
            GameObject host,
            List<List<ThemePropertyValue>> stateValues,
            Action<GameObject, InteractableThemeBase> resetTest,
            params Action<InteractableThemeBase>[] stateTests)
            where T : InteractableThemeBase
            where C : Component
        {
            yield return TestTheme<T, C>(host, stateValues, new List<ThemeProperty>() { }, resetTest, stateTests);
        }

        private IEnumerator TestTheme<T, C>(
            GameObject host,
            List<List<ThemePropertyValue>> stateValues,
            List<ThemeProperty> customProperties,
            Action<GameObject, InteractableThemeBase> resetTest,
            params Action<InteractableThemeBase>[] stateTests)
            where T : InteractableThemeBase
            where C : Component
        {
            foreach (List<ThemePropertyValue> values in stateValues)
            {
                Assert.AreEqual(values.Count, stateTests.Length);
            }

            var themeDefinition = ThemeDefinition.GetDefaultThemeDefinition<T>().Value;
            for (int i = 0; i < stateValues.Count; i++)
            {
                themeDefinition.StateProperties[i].Values = stateValues[i];
            }

            for (int i = 0; i < customProperties.Count; i++)
            {
                themeDefinition.CustomProperties[i] = customProperties[i];
            }

            yield return TestTheme<C>(host, themeDefinition, resetTest, stateTests);
        }

        private IEnumerator TestTheme<C>(
            GameObject host,
            ThemeDefinition themeDefinition,
            Action<GameObject, InteractableThemeBase> resetTest,
            params Action<InteractableThemeBase>[] stateTests)
            where C : Component
        {
            host.EnsureComponent<C>();

            var theme = InteractableThemeBase.CreateAndInitTheme(themeDefinition, host);

            for (int i = 0; i < stateTests.Length; i++)
            {
                theme.OnUpdate(i);
                yield return null;
                stateTests[i](theme);
            }

            theme.Reset();
            resetTest(host, theme);

            GameObjectExtensions.DestroyGameObject(host);
        }

        private IEnumerator TestShaderTheme<T>(
            List<List<ThemePropertyValue>> stateValues,
            Action<GameObject, InteractableThemeBase> resetTest,
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

            yield return TestTheme<T, AudioSource>(targetHost, stateValues, resetTest, convertedStateTests);
        }

        private bool AreEulerEquals(Vector3 eulerA, Vector3 eulerB)
        {
            return Mathf.Abs(Quaternion.Angle(Quaternion.Euler(eulerA), Quaternion.Euler(eulerB))) < Mathf.Epsilon;
        }

        #endregion
    }
}
#endif
