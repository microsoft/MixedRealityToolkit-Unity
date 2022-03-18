// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode
{
    /// <summary>
    /// Tests for edit mode behavior of bounds control
    /// </summary>
    public class BoundsControlTests
    {
        [Test]
        /// <summary>
        /// Tests configuring every property of bounds control in edit mode
        /// </summary>
        public void TestConfiguration()
        {
            GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            int testInstanceId = testCube.GetInstanceID();
            GameObject childSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var collider = childSphere.AddComponent<BoxCollider>();
            GameObject textObj = new GameObject();
            textObj.transform.parent = testCube.transform;
            var textMesh = textObj.AddComponent<TextMesh>();
            childSphere.transform.parent = testCube.transform;
            var boundsControl = testCube.AddComponent<BoundsControl>();
            boundsControl.Target = childSphere;
            boundsControl.BoundsOverride = collider;
            boundsControl.CalculationMethod = BoundsCalculationMethod.ColliderOverRenderer;
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateByProximityAndPointer;
            boundsControl.FlattenAxis = FlattenModeType.FlattenAuto;
            boundsControl.BoxPadding = Vector3.one;

            // debug properties
            boundsControl.DebugText = textMesh;
            boundsControl.HideElementsInInspector = false;

            // box display
            List<string> assetsToDestroy = new List<string>();
            BoxDisplayConfiguration boxDisplayConfig = ScriptableObject.CreateInstance<BoxDisplayConfiguration>();
            string path = GeneratePath("BoxDisplay", testInstanceId);
            assetsToDestroy.Add(path);
            AssetDatabase.CreateAsset(boxDisplayConfig, path);
            var testMaterial = new Material(Shader.Find("Specular"));
            boxDisplayConfig.BoxMaterial = testMaterial;
            boxDisplayConfig.BoxGrabbedMaterial = testMaterial;
            boxDisplayConfig.FlattenAxisDisplayScale = 5.0f;
            boundsControl.BoxDisplayConfig = boxDisplayConfig;

            // links
            LinksConfiguration linksConfig = ScriptableObject.CreateInstance<LinksConfiguration>();
            path = GeneratePath("Links", testInstanceId);
            assetsToDestroy.Add(path);
            AssetDatabase.CreateAsset(linksConfig, path);
            linksConfig.WireframeMaterial = testMaterial;
            linksConfig.WireframeEdgeRadius = 1.0f;
            linksConfig.WireframeShape = WireframeType.Cylindrical;
            linksConfig.ShowWireFrame = false;
            boundsControl.LinksConfig = linksConfig;

            // scale handles
            ScaleHandlesConfiguration scaleConfig = ScriptableObject.CreateInstance<ScaleHandlesConfiguration>();
            path = GeneratePath("ScaleHandles", testInstanceId);
            assetsToDestroy.Add(path);
            AssetDatabase.CreateAsset(scaleConfig, path);

            scaleConfig.HandleSlatePrefab = childSphere;
            scaleConfig.ShowScaleHandles = true;
            scaleConfig.HandleMaterial = testMaterial;
            scaleConfig.HandleGrabbedMaterial = testMaterial;
            scaleConfig.HandlePrefab = testCube;
            scaleConfig.HandleSize = 0.05f;
            scaleConfig.ColliderPadding = Vector3.one;
            scaleConfig.DrawTetherWhenManipulating = false;
            scaleConfig.HandlesIgnoreCollider = collider;

            boundsControl.ScaleHandlesConfig = scaleConfig;

            // rotation handles
            RotationHandlesConfiguration rotationHandles = ScriptableObject.CreateInstance<RotationHandlesConfiguration>();
            path = GeneratePath("RotationHandles", testInstanceId);
            assetsToDestroy.Add(path);
            AssetDatabase.CreateAsset(rotationHandles, path);

            rotationHandles.HandlePrefabColliderType = HandlePrefabCollider.Box;
            rotationHandles.ShowHandleForX = false;
            rotationHandles.ShowHandleForY = true;
            rotationHandles.ShowHandleForZ = true;
            rotationHandles.HandleMaterial = testMaterial;
            rotationHandles.HandleGrabbedMaterial = testMaterial;
            rotationHandles.HandlePrefab = childSphere;
            rotationHandles.HandleSize = 0.05f;
            rotationHandles.ColliderPadding = Vector3.zero;
            rotationHandles.DrawTetherWhenManipulating = false;
            rotationHandles.HandlesIgnoreCollider = collider;

            boundsControl.RotationHandlesConfig = rotationHandles;

            // proximity effect
            ProximityEffectConfiguration proximityConfig = ScriptableObject.CreateInstance<ProximityEffectConfiguration>();
            path = GeneratePath("ProximityEffect", testInstanceId);
            assetsToDestroy.Add(path);
            AssetDatabase.CreateAsset(proximityConfig, path);

            proximityConfig.ProximityEffectActive = true;
            proximityConfig.ObjectMediumProximity = 0.2f;
            proximityConfig.ObjectCloseProximity = 0.02f;
            proximityConfig.FarScale = 10.0f;
            proximityConfig.MediumScale = 3.0f;
            proximityConfig.CloseScale = 5.0f;
            proximityConfig.FarGrowRate = 1.0f;
            proximityConfig.MediumGrowRate = 0.1f;
            proximityConfig.CloseGrowRate = 0.5f;

            boundsControl.HandleProximityEffectConfig = proximityConfig;

            // clean up created assets
            foreach (string assetPath in assetsToDestroy)
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            // destroy test cube
            Object.DestroyImmediate(testCube);
        }

        /// <summary>
        /// Generates an asset path out of the given config name and test instance id
        /// </summary>
        /// <param name="configName">Name of the config to be saved</param>
        /// <param name="testInstanceId">Test instance id that makes sure this path is unique</param>
        /// <returns>Asset path</returns>
        private string GeneratePath(string configName, int testInstanceId)
        {
            return "Assets/" + configName + testInstanceId + ".asset";
        }
    }
}
