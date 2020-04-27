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

        private Dictionary<Object, MigrationStatus> migrationObjects = new Dictionary<Object, MigrationStatus>();
        /// <summary>
        /// Returns a copy of all game objects, prefabs and scene assets selected for migration and their migration status
        /// </summary>
        public Dictionary<Object, MigrationStatus> MigrationObjects => new Dictionary<Object, MigrationStatus>(migrationObjects);

        private IMigrationHandler migrationHandlerInstance;
        private Type migrationHandlerInstanceType;

        /// <summary>
        /// Possible states for the migration tool
        /// </summary>
        public enum MigrationState
        {
            PreMigration, // New object selection can be added to migration objects collection
            Migrating, // Processing migration objects
            PostMigration // New objects should not be added to migration objects collection
        };
        public MigrationState migrationState { get; private set; }

        public MigrationTool()
        {
            RefreshAvailableTypes();
        }

        /// <summary>
        /// Adds selectedObject to the list of objects to be migrated. Return false if the object is not of type GameObject, or SceneAsset.
        /// </summary>
        public bool TryAddObjectForMigration(Type type, Object selectedObject)
        {
            if(migrationState == MigrationState.Migrating)
            {
                Debug.LogError("Objects cannot be added during migration process.");
                return false;
            }
            else if(migrationState == MigrationState.PostMigration)
            {
                Debug.Log("Cleaning list of processed objects.");
                ClearMigrationList();
                migrationState = MigrationState.PreMigration;
            }

            if (type == null)
            {
                Debug.LogError("Migration type needs to be selected before migration.");
                return false;
            }

            if (type != migrationHandlerInstanceType)
            {
                ClearMigrationList();
                Debug.Log("New migration type selected for migration. Clearing previous selection.");

                if (!SetMigrationHandlerInstance(type))
                {
                    return false;
                }
            }

            if (!selectedObject)
            {
                Debug.LogWarning("Selection is empty. Please select object for migration.");
                return false;
            }

            if (selectedObject is GameObject || selectedObject is SceneAsset)
            {
                if (CheckIfCanMigrate(type, selectedObject) && !migrationObjects.ContainsKey(selectedObject))
                {
                    migrationObjects.Add(selectedObject, new MigrationStatus());
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
            bool canMigrate = false;
            string objectPath = AssetDatabase.GetAssetPath(selectedObject);

            if (IsSceneGameObject(selectedObject))
            {
                var objectHierarchy = ((GameObject)selectedObject).GetComponentsInChildren<Transform>();
                for (int i = 0; i < objectHierarchy.Length; i++)
                {
                    if (migrationHandlerInstance.CanMigrate(objectHierarchy[i].gameObject))
                    { 
                        return true;
                    }
                }
            }
            else if (IsPrefabAsset(selectedObject))
            {
                PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(selectedObject);
                if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
                {
                    var parent = UnityEditor.PrefabUtility.LoadPrefabContents(objectPath);
                    canMigrate = CheckIfCanMigrate(type, parent);
                    PrefabUtility.UnloadPrefabContents(parent);
                }
            }
            else if (IsSceneAsset(selectedObject))
            {
                Scene scene = EditorSceneManager.OpenScene(objectPath);

                foreach (var parent in scene.GetRootGameObjects())
                {
                    if (CheckIfCanMigrate(type, parent))
                    {
                        return true;
                    }
                }
            }
            return canMigrate;
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
            if (migrationObjects.Count == 0)
            {
                Debug.LogError($"List of objects for migration is empty.");
                return false;
            }

            if (migrationHandlerInstanceType == null)
            {
                Debug.LogError($"Please select type for migration.");
                return false;
            }

            if (type == null || migrationHandlerInstanceType != type)
            {
                Debug.LogError($"Selected objects should be migrated with type: {migrationHandlerInstanceType}");
                return false;
            }

            if(!EditorUtility.DisplayDialog("Migration Window",
                "Migration operation cannot be reverted.\n\nDo you want to continue?", "Continue", "Cancel"))
            {
                return false;
            }

            if (askToSaveCurrentScene && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return false;
            }
            var previousScenePath = EditorSceneManager.GetActiveScene().path;
            migrationState = MigrationState.Migrating;

            for (int i = 0; i < migrationObjects.Count; i++)
            {
                var progress = (float)i / migrationObjects.Count;
                if (EditorUtility.DisplayCancelableProgressBar("Migration Tool", $"Migrating all {type.Name} components from selection", progress))
                {
                    break;
                }
                string assetPath = AssetDatabase.GetAssetPath(migrationObjects.ElementAt(i).Key);

                if (IsSceneGameObject(migrationObjects.ElementAt(i).Key))
                {
                    MigrateGameObjectHierarchy((GameObject)migrationObjects.ElementAt(i).Key, migrationObjects.ElementAt(i).Value);
                }
                else if (IsPrefabAsset(migrationObjects.ElementAt(i).Key))
                {
                    PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(migrationObjects.ElementAt(i).Key);
                    if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
                    {
                        // there's currently 5 types of prefab asset types - we're supporting the following:
                        // - Regular: a regular prefab object
                        // - Variant: a prefab derived from another prefab which could be a model, regular or variant prefab
                        // we won't support the following types:
                        // - Model: we can't migrate fbx or other mesh files
                        // - MissingAsset: we can't migrate missing data
                        // - NotAPrefab: we can't migrate as prefab if the given asset isn't a prefab
                        MigratePrefab(assetPath, migrationObjects.ElementAt(i).Value);
                    }
                }
                else if (IsSceneAsset(migrationObjects.ElementAt(i).Key))
                {
                    MigrateScene(assetPath, migrationObjects.ElementAt(i).Value);
                }
                migrationObjects.ElementAt(i).Value.isProcessed = true;
            }
            EditorUtility.ClearProgressBar();
            //migrationObjects.Clear();

            if (!String.IsNullOrEmpty(previousScenePath) && previousScenePath != EditorSceneManager.GetActiveScene().path)
            {
                EditorSceneManager.OpenScene(Path.Combine(Directory.GetCurrentDirectory(), previousScenePath));
            }
            EditorUtility.DisplayDialog("Migration Window",
                "Migration completed successfully!", "Close");
            migrationState = MigrationState.PostMigration;
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

        private void MigrateScene(String path, MigrationStatus status)
        {
            if (!AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset)))
            {
                return;
            }
            Scene scene = EditorSceneManager.OpenScene(path);

            bool didAnySceneObjectChange = false;
            foreach (var parent in scene.GetRootGameObjects())
            {
                didAnySceneObjectChange |= MigrateGameObjectHierarchy(parent, status);
            }

            if (didAnySceneObjectChange)
            {
                EditorSceneManager.SaveScene(scene);
            }
        }

        private void MigratePrefab(String path, MigrationStatus status)
        {
            if (!AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)))
            {
                return;
            }
            var parent = UnityEditor.PrefabUtility.LoadPrefabContents(path);

            if (MigrateGameObjectHierarchy(parent, status))
            {
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(parent, path);
            }

            PrefabUtility.UnloadPrefabContents(parent);
        }

        private bool MigrateGameObjectHierarchy(GameObject parent, MigrationStatus status)
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

                        status.log += $"Successfully migrated {parent.name} object \n";
                        Debug.Log(status.log);
                    }
                }
                catch (Exception e)
                {
                    status.failures++;
                    status.log += $"{e.Message}: GameObject {parent.name} could not be migrated \n";
                    Debug.LogError(status.log);
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

        private static bool IsSceneGameObject(Object selectedObject)
        {
            string objectPath = AssetDatabase.GetAssetPath(selectedObject);
            return String.IsNullOrEmpty(objectPath) && selectedObject is GameObject;
        }

        private static bool IsPrefabAsset(Object selectedObject)
        {
            string objectPath = AssetDatabase.GetAssetPath(selectedObject);
            return !String.IsNullOrEmpty(objectPath) && selectedObject is GameObject;
        }

        private static bool IsSceneAsset(Object selectedObject)
        {
            return selectedObject is SceneAsset;
        }

        /// <summary>
        /// Utility class to keep migration status of each object
        /// </summary>
        public class MigrationStatus
        {
            public bool isProcessed;
            public int failures;
            public String log;

            public MigrationStatus()
            {
                isProcessed = false;
                failures = 0;
                log = "";
            }
        }
    }
}
#endif

