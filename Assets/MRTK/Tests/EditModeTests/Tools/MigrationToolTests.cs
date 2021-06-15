// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode
{
    public class MigrationToolTests
    {
        private readonly MigrationTool migrationTool = new MigrationTool();
        private readonly HashSet<string> assetsForDeletion = new HashSet<string>();
        private readonly string scenePath = "Assets/_migration.unity";
        private readonly string prefabPath = "Assets/_migration.prefab";

        private struct MigrationTypes
        {
            public Type oldType;
            public Type newType;
            public Type handler;

            public MigrationTypes(Type oldT, Type newT, Type mHandler)
            {
                oldType = oldT;
                newType = newT;
                handler = mHandler;
            }
        }

        private List<MigrationTypes> migrationList = new List<MigrationTypes>();

        [SetUp]
        public void Setup()
        {
            migrationList.Add(new MigrationTypes(typeof(ManipulationHandler), typeof(ObjectManipulator), typeof(ObjectManipulatorMigrationHandler)));
            migrationList.Add(new MigrationTypes(typeof(BoundingBox), typeof(BoundsControl), typeof(BoundsControlMigrationHandler)));
        }

        /// <summary>
        /// Deletes all assets created during tests
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            foreach (var assetPath in assetsForDeletion)
            {
                if (AssetDatabase.LoadMainAssetAtPath(assetPath))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Tests that the Button Migration tool works properly
        /// </summary>
        [Test]
        public void ButtonMigrationTest()
        {
            Type migrationHandlerType = typeof(ButtonConfigHelperMigrationHandler);
            Material testMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/MRTK/SDK/Features/UX/Interactable/Materials/HolographicButtonIconHome.mat");
            Material testDefaultMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/MRTK/SDK/Features/UX/Interactable/Materials/HolographicButtonIconStar.mat");

            GameObject buttonGameObject = SetUpGameObjectWithComponentOfType(typeof(ButtonConfigHelper));
            GameObject buttonQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            buttonQuad.transform.parent = buttonGameObject.transform;

            MeshRenderer quadRenderer = buttonQuad.GetComponent<MeshRenderer>();
            quadRenderer.sharedMaterial = testMat;

            ButtonConfigHelper buttonConfig = buttonGameObject.GetComponent<ButtonConfigHelper>();
            ButtonIconSet testIconSet = ScriptableObject.CreateInstance<ButtonIconSet>();
            buttonConfig.IconStyle = ButtonIconStyle.Quad;
            buttonConfig.IconSet = testIconSet;
            buttonConfig.EditorSetDefaultIconSet(testIconSet);
            buttonConfig.EditorSetIconQuadRenderer(quadRenderer);
            buttonConfig.EditorSetDefaultQuadMaterial(testDefaultMat);

            migrationTool.TryAddObjectForMigration(migrationHandlerType, buttonGameObject);

            string testCustomIconSetFolder = System.IO.Path.Combine("Assets", "MixedRealityToolkit.Generated.Test");
            AssetDatabase.DeleteAsset(testCustomIconSetFolder);
            AssetDatabase.CreateFolder("Assets", "MixedRealityToolkit.Generated.Test");

            buttonConfig.EditorUpgradeCustomIcon(null, testCustomIconSetFolder, true);

            AssetDatabase.Refresh();
            ButtonIconSet generatedIconSet = AssetDatabase.LoadAssetAtPath<ButtonIconSet>(System.IO.Path.Combine("Assets", "MixedRealityToolkit.Generated.Test", "CustomIconSets", "CustomIconSet.asset"));
            Assert.IsNotNull(generatedIconSet);
            Assert.IsTrue(generatedIconSet.QuadIcons.Length == 1);

            AssetDatabase.DeleteAsset(testCustomIconSetFolder);
        }

        /// <summary>
        /// Checks if MigrationTool can process migration on a game object containing a deprecated component with a compatible migration handler.
        /// </summary>
        [Test]
        public void GameObjectCanBeMigrated()
        {
            foreach (var entry in migrationList)
            {
                Type oldType = entry.oldType;
                Type newType = entry.newType;
                Type migrationHandlerType = entry.handler;

                GameObject gameObject = SetUpGameObjectWithComponentOfType(oldType);

                migrationTool.TryAddObjectForMigration(migrationHandlerType, gameObject);
                migrationTool.MigrateSelection(migrationHandlerType, false);

                Assert.IsNull(gameObject.GetComponent(oldType), $"Migrated Component of type {oldType.Name} could not be removed");
                Assert.IsNotNull(gameObject.GetComponent(newType), $"Migrated Component of type {newType.Name} could not be added");

                Object.DestroyImmediate(gameObject);
            }
        }

        /// <summary>
        /// Checks if MigrationTool can process migration on a prefab containing a deprecated component with a compatible migration handler.
        /// </summary>
        [Test]
        public void PrefabCanBeMigrated()
        {
            foreach (var entry in migrationList)
            {
                Type oldType = entry.oldType;
                Type newType = entry.newType;
                Type migrationHandlerType = entry.handler;

                GameObject gameObject = SetUpGameObjectWithComponentOfType(oldType);
                PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
                assetsForDeletion.Add(prefabPath);

                migrationTool.TryAddObjectForMigration(migrationHandlerType, AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)));
                migrationTool.MigrateSelection(migrationHandlerType, false);

                GameObject prefabGameObject = PrefabUtility.LoadPrefabContents(prefabPath);

                Assert.IsNull(prefabGameObject.GetComponent(oldType), $"Migrated Component of type {oldType.Name} could not be removed");
                Assert.IsNotNull(prefabGameObject.GetComponent(newType), $"Migrated Component of type {newType.Name} could not be added");

                Object.DestroyImmediate(gameObject);
            }
        }

        /// <summary>
        /// Checks if MigrationTool can process migration on a scene root game object that contains a deprecated component with a compatible migration handler.
        /// </summary>
        [Test]
        public void SceneCanBeMigrated()
        {
            foreach (var entry in migrationList)
            {
                Type oldType = entry.oldType;
                Type newType = entry.newType;
                Type migrationHandlerType = entry.handler;

                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                GameObject gameObject = SetUpGameObjectWithComponentOfType(oldType);
                EditorSceneManager.SaveScene(scene, scenePath);
                assetsForDeletion.Add(scenePath);

                migrationTool.TryAddObjectForMigration(migrationHandlerType, AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset)));
                migrationTool.MigrateSelection(migrationHandlerType, false);

                var openScene = EditorSceneManager.OpenScene(scenePath);
                foreach (var sceneGameObject in openScene.GetRootGameObjects())
                {
                    Assert.IsNull(sceneGameObject.GetComponent(oldType), $"Migrated component of type {oldType.Name} could not be removed");
                    Assert.IsNotNull(sceneGameObject.GetComponent(newType), $"Migrated component of type {newType.Name} could not be added");

                    Object.DestroyImmediate(sceneGameObject);
                }
                Object.DestroyImmediate(gameObject);
            }
        }

        /// <summary>
        /// Checks if MigrationTool can process migration on a inactive scene root game object that contains an inactive deprecated component with a compatible migration handler.
        /// Active state of both game object and component must be kept.
        /// </summary>
        [Test]
        public void MigrationKeepObjectAndComponentActiveState()
        {
            foreach (var entry in migrationList)
            {
                Type oldType = entry.oldType;
                Type newType = entry.newType;
                Type migrationHandlerType = entry.handler;

                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                GameObject gameObject = SetUpGameObjectWithComponentOfType(oldType);
                MonoBehaviour oldTypeComponent = (MonoBehaviour)gameObject.GetComponent(oldType);

                oldTypeComponent.enabled = false;
                gameObject.SetActive(false);

                EditorSceneManager.SaveScene(scene, scenePath);
                assetsForDeletion.Add(scenePath);

                migrationTool.TryAddObjectForMigration(migrationHandlerType, AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset)));
                migrationTool.MigrateSelection(migrationHandlerType, false);

                var openScene = EditorSceneManager.OpenScene(scenePath);
                foreach (var sceneGameObject in openScene.GetRootGameObjects())
                {
                    Assert.IsNull(sceneGameObject.GetComponent(oldType), $"Migrated component of type {oldType.Name} could not be removed");
                    Assert.IsNotNull(sceneGameObject.GetComponent(newType), $"Migrated component of type {newType.Name} could not be added");

                    // Active state of game object and component is kept
                    Assert.IsFalse(sceneGameObject.activeSelf, $"Active state of migrated game object was not kept during migration with type {migrationHandlerType.Name}");
                    Assert.IsFalse(((MonoBehaviour)sceneGameObject.GetComponent(newType)).enabled, $"Active state of migrated component was not kept during migration with type { migrationHandlerType.Name}");

                    Object.DestroyImmediate(sceneGameObject);
                }
                Object.DestroyImmediate(gameObject);
            }
        }

        private static GameObject SetUpGameObjectWithComponentOfType(Type type)
        {
            GameObject go = new GameObject();
            if (typeof(Component).IsAssignableFrom(type))
            {
                go.AddComponent(type);
            }

            Assert.IsNotNull(go.GetComponent(type), $"Component of type {type.Name} could not be added to GameObject");

            return go;
        }
    }
}