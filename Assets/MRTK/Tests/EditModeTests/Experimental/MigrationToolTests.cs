// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Experimental
{
    public class MigrationToolTests
    {
        private readonly MigrationTool migrationTool = new MigrationTool();
        private readonly HashSet<string> assetsForDeletion = new HashSet<string>();

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

        // internal test version of bounds control migration handler that overrides the
        // output directory of the generated config files so we can clean them up properly 
        // after each test run.
        internal class TestBoundsControlMigrationHandler : BoundsControlMigrationHandler
        {
            const string migrationTestFolder = "MigrationTests";
            const string assetPath = "Assets";
            protected override string GetBoundsControlConfigDirectory(BoundingBox boundingBox)
            {
                string configPath = assetPath + "/" + migrationTestFolder;
                if (AssetDatabase.IsValidFolder(configPath))
                {
                    return configPath;
                }
                else
                {
                    string guid = AssetDatabase.CreateFolder(assetPath, migrationTestFolder);
                    return AssetDatabase.GUIDToAssetPath(guid);
                }
            }

            static public void AddMigratedFilesToCleanup(HashSet<string> filesToDelete)
            {
                string configDir = assetPath + "/" + migrationTestFolder;
                if (AssetDatabase.IsValidFolder(configDir))
                {
                    filesToDelete.Add(configDir);
                }
            }
        }

        [SetUp]
        public void Setup()
        {
            migrationList.Add(new MigrationTypes(typeof(ManipulationHandler), typeof(ObjectManipulator), typeof(ObjectManipulatorMigrationHandler)));
            migrationList.Add(new MigrationTypes(typeof(BoundingBox), typeof(BoundsControl), typeof(TestBoundsControlMigrationHandler)));
        }

        /// <summary>
        /// Deletes all assets created during tests
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            TestBoundsControlMigrationHandler.AddMigratedFilesToCleanup(assetsForDeletion);
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
        /// Checks if MigrationTool can process migration on a game object containing a deprecated ManipulationHandler component.
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

                migrationTool.TryAddObjectForMigration(gameObject);
                migrationTool.MigrateSelection(migrationHandlerType, false);

                Assert.IsNull(gameObject.GetComponent(oldType), $"Migrated Component of type {oldType.Name} could not be removed");
                Assert.IsNotNull(gameObject.GetComponent(newType), $"Migrated Component of type {newType.Name} could not be added");

                Object.DestroyImmediate(gameObject);
            }
        }

        /// <summary>
        /// Checks if MigrationTool can process migration on a prefab containing a deprecated ManipulationHandler component.
        /// </summary>
        [Test]
        public void PrefabCanBeMigrated()
        {
            foreach (var entry in migrationList)
            {
                Type oldType = entry.oldType;
                Type newType = entry.newType;
                Type migrationHandlerType = entry.handler;
                string prefabPath = "Assets/_migration.prefab";

                GameObject gameObject = SetUpGameObjectWithComponentOfType(oldType);
                PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
                assetsForDeletion.Add(prefabPath);

                migrationTool.TryAddObjectForMigration(AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)));
                migrationTool.MigrateSelection(migrationHandlerType, false);

                GameObject prefabGameObject = PrefabUtility.LoadPrefabContents(prefabPath);

                Assert.IsNull(prefabGameObject.GetComponent(oldType), $"Migrated Component of type {oldType.Name} could not be removed");
                Assert.IsNotNull(prefabGameObject.GetComponent(newType), $"Migrated Component of type {newType.Name} could not be added");

                Object.DestroyImmediate(gameObject);
            }
        }

        /// <summary>
        /// Checks if MigrationTool can process migration on a scene root game object that contains a deprecated ManipulationHandler component.
        /// </summary>
        [Test]
        public void SceneCanBeMigrated()
        {
            foreach (var entry in migrationList)
            {
                Type oldType = entry.oldType;
                Type newType = entry.newType;
                Type migrationHandlerType = entry.handler;
                string scenePath = "Assets/_migration.unity";

                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                GameObject gameObject = SetUpGameObjectWithComponentOfType(oldType);
                EditorSceneManager.SaveScene(scene, scenePath);
                assetsForDeletion.Add(scenePath);

                migrationTool.TryAddObjectForMigration(AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset)));
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

