// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.using System;

#if UNITY_EDITOR
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
        public List<Type> MigrationHandlerTypes => new List<Type>(migrationHandlerTypes);

        private List<Object> migrationObjects = new List<Object>();
        /// <summary>
        /// Returns a copy of all game objects, prefabs and scene assets selected for migration
        /// </summary>
        public List<Object> MigrationObjects => new List<Object>(migrationObjects);

        private IMigrationHandler migrationHandlerInstance;

        public MigrationTool()
        {
            RefreshAvailableTypes();
        }

        /// <summary>
        /// Adds selectedObject to the list of objects to be migrated. Return false if the object is not of type GameObject, or SceneAsset.
        /// </summary>
        /// <param name="selectedObject">Unity Object of type GameObject or SceneAsset</param>
        public bool TryAddObjectForMigration(Object selectedObject)
        {
            if (!selectedObject)
            {
                Debug.LogError("Selection is Empty. Could not add for migration.");
                return false;
            }

            if (selectedObject is GameObject || selectedObject is SceneAsset)
            {
                if (!migrationObjects.Contains(selectedObject))
                {
                    migrationObjects.Add(selectedObject);
                    Debug.Log($"{selectedObject.name} object added for migration");
                }
                return true;
            }
            Debug.LogError("Object must be a GameObject, Prefab or SceneAsset. Could not add object for migration");
            return false;
        }

        /// <summary>
        /// Adds all prefabs and scene assets found on the assets folder to the list of objects to be migrated
        /// </summary>
        public void TryAddProjectForMigration()
        {
            AddAllAssetsOfTypeForMigration(new Type[] { typeof(GameObject), typeof(SceneAsset) });
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
        public bool MigrateSelection(Type type, bool askToSaveCurrentScene)
        {
            if (type == null || !SetMigrationHandlerInstance(type))
            {
                return false;
            }

            if (askToSaveCurrentScene && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return false;
            }
            var previousScenePath = EditorSceneManager.GetActiveScene().path;

            for (int i = 0; i < migrationObjects.Count; i++)
            {
                var progress = (float)i / migrationObjects.Count;
                if (EditorUtility.DisplayCancelableProgressBar("Migration Tool", $"Migrating all {type.Name} components from selection", progress))
                {
                    break;
                }
                string assetPath = AssetDatabase.GetAssetPath(migrationObjects[i]);

                // Object migrationObject is a scene game object
                if (String.IsNullOrEmpty(assetPath))
                {
                    MigrateGameObjectHierarchy((GameObject)migrationObjects[i]);
                }
                else
                {
                    if (migrationObjects[i] is GameObject)
                    {
                        MigratePrefab(assetPath);
                    }
                    else if (migrationObjects[i] is SceneAsset)
                    {
                        MigrateScene(assetPath);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            migrationObjects.Clear();

            if (!String.IsNullOrEmpty(previousScenePath) && previousScenePath != EditorSceneManager.GetActiveScene().path)
            {
                EditorSceneManager.OpenScene(Path.Combine(Directory.GetCurrentDirectory(), previousScenePath));
            }
            return true;
        }

        private void AddAllAssetsOfTypeForMigration(Type[] types)
        {
            var assetPaths = FindAllAssetsOfType(types);
            if (assetPaths != null)
            {
                foreach (var path in assetPaths)
                {
                    TryAddObjectForMigration(AssetDatabase.LoadMainAssetAtPath(path));
                }
            }
        }

        private bool SetMigrationHandlerInstance(Type type)
        {
            if (!typeof(IMigrationHandler).IsAssignableFrom(type))
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
                Debug.LogError("Selected MigrationHandler implementation could not be instanciated");
                return false;
            }
            return true;
        }

        private void RefreshAvailableTypes()
        {
            var type = typeof(IMigrationHandler);
            migrationHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetLoadableTypes())
                .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
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
                    Debug.LogError($"{e.Message}: GameObject {parent.name} could not be migrated");
                }
            }
        }

        private static List<string> FindAllAssetsOfType(Type[] types)
        {
            var filter = string.Join(" ", types
                                          .Select(x => string.Format("t{0}", x.Name))
                                          .ToArray());
            return AssetDatabase.FindAssets(filter).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
        }
    }
}
#endif

