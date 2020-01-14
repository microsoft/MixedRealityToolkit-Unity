// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.using System;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    /// <summary>
    /// This tool allows the migration of obsolete components into up-to-date versions.
    /// In order to be processed by the migration tool, deprecated components require specific implementation of the IMigrationHandler 
    /// </summary>
    public class MigrationTool
    {
        private List<Type> migrationHandlerTypes = new List<Type>();
        /// <summary>
        /// Returns a copy of all loadable implementation types of IMigrationHandler
        /// </summary>
        public List<Type> MigrationHandlerTypes { get { return new List<Type>(migrationHandlerTypes); } }

        private List<Object> migrationObjects = new List<Object>();
        /// <summary>
        /// Returns a copy of all Game Objects, Prefabs and Scene assets selected for migration
        /// </summary>
        public List<Object> MigrationObjects { get { return new List<Object>(migrationObjects); } }

        private dynamic migrationHandlerInstance;

        private bool SetMigrationHandlerInstance(Type type)
        {
            if (type == null || !typeof(IMigrationHandler).IsAssignableFrom(type))
            {
                Debug.LogError($"{type.Name} is not a valid implementation of IMigrationHandler");
                return false;
            }

            if (!migrationHandlerTypes.Contains(type))
            {
                Debug.LogError($"{type.Name} might not be a valid implementation of IMigrationHandler");
                return false;
            }

            try
            {
                migrationHandlerInstance = Activator.CreateInstance(type) as IMigrationHandler;
            }
            catch (Exception)
            {
                Debug.LogError("Selected MigrationHandler implementation could not be Instanciated");
                return false;
            }
            return true;
        }

        public MigrationTool()
        {
            RefreshAvailableTypes();
        }

        private void RefreshAvailableTypes()
        {
            var type = typeof(IMigrationHandler);
            migrationHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetLoadableTypes())
                .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
        }

        /// <summary>
        /// Adds Object to the list of objects to be migrated. Return false if Object is not of a GameObject, a Prefab or a Scene asset.
        /// </summary>
        /// <param name="selectedObject">Unity object of type of a GameObject or SceneAsset</param>
        public bool TryAddObjectForMigration(Object selectedObject)
        {
            if (!selectedObject)
            {
                Debug.LogError("Selection is Empty. Could not add for Migration.");
                return false;
            }

            if (selectedObject is GameObject || selectedObject is SceneAsset)
            {
                if (!migrationObjects.Contains(selectedObject))
                {
                    migrationObjects.Add(selectedObject);
                    Debug.Log($"{selectedObject.name} object added for Migration");
                }
                return true;
            }
            Debug.LogError("Object must be a GameObject, Prefab or SceneAsset. Could not add object for Migration");
            return false;
        }

        /// <summary>
        /// Adds all prefabs and scene assets found on the Assets folder to the list of objects to be migrated
        /// </summary>
        public void AddProjectForMigration()
        {
            AddAllAssetsOfTypeForMigration(new Type[] { typeof(GameObject), typeof(SceneAsset) });
        }

        private void AddAllAssetsOfTypeForMigration(Type[] types)
        {
            if (types != null)
            {
                return;
            }
            foreach (var type in types)
            {
                foreach (var path in FindAllAssetsOfType(type))
                {
                    TryAddObjectForMigration(AssetDatabase.LoadAssetAtPath(path, type));
                }
            }
        }

        /// <summary>
        /// Removes object from the list of objects to migrated
        /// </summary>
        /// <param name="selectedObject">Object to be removed</param>
        public void RemoveObjectForMigration(Object selectedObject)
        {
            migrationObjects.Remove(selectedObject);
        }

        /// <summary>
        /// Clears list of objects to be migrated
        /// </summary>
        public void ClearMigrationList()
        {
            migrationObjects.Clear();
        }

        /// <summary>
        /// Migrates all objects from list of objects to be migrated using the selected IMigrationHandler implementation. 
        /// </summary>
        /// <param name="type">A type that implements IMigrationhandler</param>
        /// <returns></returns>
        public bool MigrateSelection(Type type, bool askToSaveCurrentScene)
        {
            if (!SetMigrationHandlerInstance(type))
            {
                return false;
            }

            if (askToSaveCurrentScene)
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    Debug.Log("d");
                    return false;
                }
            }
            var previousScenePath = EditorSceneManager.GetActiveScene().path;

            foreach (var migrationObject in migrationObjects)
            {
                string assetPath = AssetDatabase.GetAssetPath(migrationObject);

                // Object migrationObject is a scene Game Object
                if (String.IsNullOrEmpty(assetPath))
                {
                    MigrateGameObjectHierarchy((GameObject)migrationObject);
                }
                else
                {
                    if (migrationObject is GameObject)
                    {
                        MigratePrefab(assetPath);
                    }
                    else if (migrationObject is SceneAsset)
                    {
                        MigrateScene(assetPath);
                    }
                }
            }
            migrationObjects.Clear();

            if (!String.IsNullOrEmpty(previousScenePath) && previousScenePath != EditorSceneManager.GetActiveScene().path)
            {
                EditorSceneManager.OpenScene(Directory.GetCurrentDirectory() + "/" + previousScenePath);
            }
            return true;
        }

        private void MigrateScene(String path)
        {
            if (!AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset)))
            {
                return;
            }
            Scene scene = EditorSceneManager.OpenScene(path);

            foreach (var parent in scene.GetRootGameObjects())
            {
                MigrateGameObjectHierarchy(parent);
            }
            EditorSceneManager.SaveScene(scene);
        }

        private void MigratePrefab(String path)
        {
            if (!AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)))
            {
                return;
            }
            var parent = UnityEditor.PrefabUtility.LoadPrefabContents(path);

            MigrateGameObjectHierarchy(parent);

            UnityEditor.PrefabUtility.SaveAsPrefabAsset(parent, path);
            PrefabUtility.UnloadPrefabContents(parent);
        }

        private void MigrateGameObjectHierarchy(GameObject parent)
        {
            if (!parent)
            {
                return;
            }

            foreach (var child in parent.GetComponentsInChildren<Transform>())
            {
                try
                {
                    if (migrationHandlerInstance.CanMigrate(child.gameObject))
                    {
                        migrationHandlerInstance.Migrate(child.gameObject);
                        Debug.Log($"Successfully migrated {parent.name} object");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}: GameObject {parent.name} could not be Migrated");
                }
            }
        }

        private static List<string> FindAllAssetsOfType(Type type)
        {
            return AssetDatabase.GetAllAssetPaths()
                .Select(x => x)
                .Where(x => AssetDatabase.GetMainAssetTypeAtPath(x) == type)
                .ToList();
        }
    }
}


