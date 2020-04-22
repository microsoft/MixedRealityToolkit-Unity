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

namespace Microsoft.MixedReality.Toolkit.Utilities
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
        private Type migrationHandlerInstanceType;

        public MigrationTool()
        {
            RefreshAvailableTypes();
        }

        /// <summary>
        /// Adds selectedObject to the list of objects to be migrated. Return false if the object is not of type GameObject, or SceneAsset.
        /// </summary>
        /// <param name="selectedObject">Unity Object of type GameObject or SceneAsset</param>
        public bool TryAddObjectForMigration(Type type, Object selectedObject)
        {
            if(type == null)
            {
                Debug.LogError("Migration type needs to be selected before migration.");
                return false;
            }

            if(type != migrationHandlerInstanceType)
            {
                ClearMigrationList();
                Debug.LogWarning("New migration type selected for migration. Clearing previous selection.");

                if (!SetMigrationHandlerInstance(type))
                {
                    return false;
                }
            }

            if (!selectedObject)
            {
                Debug.LogError("Selection is empty. Please select object for migration.");
                return false;
            }

            if (selectedObject is GameObject || selectedObject is SceneAsset)
            {
                if (CheckIfCanMigrate(type, selectedObject) && !migrationObjects.Contains(selectedObject))
                {
                    migrationObjects.Add(selectedObject);
                    return true;
                }
                else
                {
                    Debug.Log($"{selectedObject.name} does not support {type.Name} migration. Could not add object for migration");
                    return false;
                }
            }
            Debug.LogError("Object must be a GameObject, Prefab or SceneAsset. Could not add object for migration");
            return false;
        }

        private bool CheckIfCanMigrate(Type type, Object selectedObject)
        {
            string objectPath = AssetDatabase.GetAssetPath(selectedObject);

            // Object migrationObject is a scene game object
            if (String.IsNullOrEmpty(objectPath) && selectedObject is GameObject)
            {
                var objectHierarchy = ((GameObject)selectedObject).GetComponentsInChildren<Transform>();
                for (int i = 0; i < objectHierarchy.Length; i++)
                {
                    try
                    {
                        if (migrationHandlerInstance.CanMigrate(objectHierarchy[i].gameObject))
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"{e.Message}: Could not check if GameObject {objectHierarchy[i].name} can be migrated");
                    }
                }
            }
            else
            {
                // Selected object is prefab asset
                if (selectedObject is GameObject)
                {
                    PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(selectedObject);
                    if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
                    {
                        var parent = UnityEditor.PrefabUtility.LoadPrefabContents(objectPath);

                        if (CheckIfCanMigrate(type, parent))
                        {
                            PrefabUtility.UnloadPrefabContents(parent);
                            return true;
                        }
                        PrefabUtility.UnloadPrefabContents(parent);
                    }                   
                }
                // Selected object is scene asset
                else if (selectedObject is SceneAsset)
                {
                    Scene scene = EditorSceneManager.OpenScene(objectPath);

                    foreach (var parent in scene.GetRootGameObjects())
                    {
                        if(CheckIfCanMigrate(type, parent))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Adds all prefabs and scene assets found on the assets folder to the list of objects to be migrated
        /// </summary>
        public void TryAddProjectForMigration(Type migrationType)
        {
            AddAllAssetsOfTypeForMigration(migrationType, new Type[] { typeof(GameObject), typeof(SceneAsset) });
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
            if(migrationObjects.Count == 0)
            {
                Debug.LogError($"List of objects for migration is empty.");
                return false;
            }

             if(migrationHandlerInstanceType == null)
            {
                Debug.LogError($"Please select type for migration.");
                return false;
            }
            
            if (type == null || migrationHandlerInstanceType != type)
            {
                Debug.LogError($"Selected objects should be migrated with type: {migrationHandlerInstanceType}");
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
                        PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(migrationObjects[i]);
                        if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
                        {
                            // there's currently 5 types of prefab asset types - we're supporting the following:
                            // - Regular: a regular prefab object
                            // - Variant: a prefab derived from another prefab which could be a model, regular or variant prefab
                            // we won't support the following types:
                            // - Model: we can't migrate fbx or other mesh files
                            // - MissingAsset: we can't migrate missing data
                            // - NotAPrefab: we can't migrate as prefab if the given asset isn't a prefab
                            MigratePrefab(assetPath);
                        }
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

        private void AddAllAssetsOfTypeForMigration(Type migrationType, Type[] assetTypes)
        {
            var assetPaths = FindAllAssetsOfType(assetTypes);
            if (assetPaths != null)
            {                               
                for (int i = 0; i < assetPaths.Count; i++)
                {
                    var progress = (float)i / assetPaths.Count;
                    if (EditorUtility.DisplayCancelableProgressBar("Migration Tool", $"Selecting all assets that support {migrationType.Name} migration.", progress))
                    {
                        break;
                    }
                    TryAddObjectForMigration(migrationType, AssetDatabase.LoadMainAssetAtPath(assetPaths[i]));
                }
                EditorUtility.ClearProgressBar();
            }
        }

        private bool SetMigrationHandlerInstance(Type type)
        {
            if (!typeof(IMigrationHandler).IsAssignableFrom(type))
            {
                Debug.LogError($"{type.Name} is not a valid implementation of IMigrationHandler.");
                return false;
            }

            if (!migrationHandlerTypes.Contains(type))
            {
                Debug.LogError($"{type.Name} might not be a valid implementation of IMigrationHandler.");
                return false;
            }

            try
            {
                migrationHandlerInstance = Activator.CreateInstance(type) as IMigrationHandler;
                migrationHandlerInstanceType = type;
                Debug.LogWarning($"Migration tool will use {type.Name} type for next migration.");
            }
            catch (Exception)
            {
                Debug.LogError("Selected MigrationHandler implementation could not be instanciated.");
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

            bool didAnySceneObjectChange = false;
            foreach (var parent in scene.GetRootGameObjects())
            {
                didAnySceneObjectChange |= MigrateGameObjectHierarchy(parent);
            }

            if (didAnySceneObjectChange)
            {
                EditorSceneManager.SaveScene(scene);
            }
        }

        private void MigratePrefab(String path)
        {
            if (!AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)))
            {
                return;
            }
            var parent = UnityEditor.PrefabUtility.LoadPrefabContents(path);

            if (MigrateGameObjectHierarchy(parent))
            {
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(parent, path);
            }

            PrefabUtility.UnloadPrefabContents(parent);
        }

        private bool MigrateGameObjectHierarchy(GameObject parent)
        {
            bool changedAnyGameObject = false;
            foreach (var child in parent.GetComponentsInChildren<Transform>())
            {
                try
                {
                    if (migrationHandlerInstance.CanMigrate(child.gameObject))
                    {
                        changedAnyGameObject = true;
                        migrationHandlerInstance.Migrate(child.gameObject);
                        Debug.Log($"Successfully migrated {parent.name} object");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e.Message}: GameObject {parent.name} could not be migrated");
                }
            }

            return changedAnyGameObject;
        }

        private static List<string> FindAllAssetsOfType(Type[] types)
        {
            var filter = string.Join(" ", types
                                          .Select(x => string.Format("t:{0}", x.Name))
                                          .ToArray());
            return AssetDatabase.FindAssets(filter).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
        }
    }
}
#endif

