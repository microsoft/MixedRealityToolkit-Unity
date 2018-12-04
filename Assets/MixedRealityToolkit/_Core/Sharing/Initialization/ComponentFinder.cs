using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pixie.Initialization
{
    public class ComponentFinder
    {
        public enum SearchTypeEnum
        {
            RootGameObjects,
            Recursive,
        }

        public enum FailModeEnum
        {
            Exception,
            Error,
            Silent
        }

        public static bool FindInScenes<T>(out T component, FailModeEnum failMode) where T : class
        {
            return FindInScenes<T>(out component, SearchTypeEnum.RootGameObjects, -1, failMode);
        }


        public static bool FindInScenes<T> (out T component, SearchTypeEnum searchType = SearchTypeEnum.RootGameObjects, int maxDepth = -1, FailModeEnum failMode = FailModeEnum.Silent) where T : class
        {
            component = null;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (FindInScene<T>(scene, out component, searchType, maxDepth, FailModeEnum.Silent))
                    return true;
            }

            switch (failMode)
            {
                case FailModeEnum.Exception:
                    throw new System.Exception("Component type " + typeof(T).ToString() + " not found in loaded scenes");


                case FailModeEnum.Error:
                    Debug.LogError("Component type " + typeof(T).ToString() + " not found in loaded scenes");
                    break;

                case FailModeEnum.Silent:
                default:
                    break;
            }

            return false;
        }

        public static bool FindInScene<T>(Scene scene, out T component, SearchTypeEnum searchType = SearchTypeEnum.RootGameObjects, int maxDepth = -1, FailModeEnum failMode = FailModeEnum.Silent) where T : class
        {
            component = null;

            if (!scene.IsValid())
                return false;

            if (!scene.isLoaded)
                return false;
            
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                if (CheckGameObject(rootGameObject, out component, searchType, 0, maxDepth))
                    break;
            }

            if (component != null)
                return true;

            switch (failMode)
            {
                case FailModeEnum.Exception:
                    throw new System.Exception("Component type " + typeof(T).ToString() + " not found in scene " + scene.name);

                case FailModeEnum.Error:
                    Debug.LogError("Component type " + typeof(T).ToString() + " not found in scene " + scene.name);
                    break;

                case FailModeEnum.Silent:
                default:
                    break;
            }

            return false;
        }

        private static bool CheckGameObject<T>(GameObject gameObject, out T component, SearchTypeEnum searchType = SearchTypeEnum.RootGameObjects, int depth = 0, int maxDepth = -1) where T : class
        {
            component = gameObject.GetComponent<T>();

            if (component != null)
                return true;

            switch (searchType)
            {
                case SearchTypeEnum.Recursive:

                    if (maxDepth >= 0)
                    {
                        depth++;
                        if (depth >= maxDepth)
                            return false;
                    }

                    foreach (Transform child in gameObject.transform)
                    {
                        if (CheckGameObject<T>(child.gameObject, out component, searchType, depth, maxDepth))
                            return true;
                    }
                    break;

                case SearchTypeEnum.RootGameObjects:
                default:
                    break;
            }

            return false;
        }

        public static void FindAllInScenes<T>(List<T> components, SearchTypeEnum searchType = SearchTypeEnum.RootGameObjects, int maxDepth = -1) where T : class
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                FindAllInScene<T>(scene, components, searchType, maxDepth);
            }
        }

        private static void FindAllInScene<T>(Scene scene, List<T> components, SearchTypeEnum searchType = SearchTypeEnum.RootGameObjects, int maxDepth = -1) where T : class
        {
            if (!scene.IsValid())
                return;

            if (!scene.isLoaded)
                return;

            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                CheckGameObject<T>(rootGameObject, components, searchType, 0, maxDepth);
        }

        private static void CheckGameObject<T>(GameObject gameObject, List<T> components, SearchTypeEnum searchType = SearchTypeEnum.RootGameObjects, int depth = 0, int maxDepth = -1) where T : class
        {
            foreach (Component goComponent in gameObject.GetComponents(typeof(T)))
                components.Add(goComponent as T);

            switch (searchType)
            {
                case SearchTypeEnum.Recursive:

                    if (maxDepth >= 0)
                    {
                        depth++;
                        if (depth >= maxDepth)
                            return;
                    }

                    foreach (Transform child in gameObject.transform)
                        CheckGameObject(child.gameObject, components, searchType, depth, maxDepth);

                    break;

                case SearchTypeEnum.RootGameObjects:
                default:
                    break;
            }
        }

        /// <summary>
        /// Finds all objects of type T attached to root gameobjects.
        /// Returns all objects NOT found in existingComponents list.
        /// Adds those components to the existing existingComponents list.
        /// </summary>
        public static IEnumerable<T> FindAllNewInScenes<T>(List<T> existingComponents, SearchTypeEnum searchType = SearchTypeEnum.RootGameObjects, int maxDepth = -1) where T : class
        {
            List<T> newComponents = new List<T>();
            HashSet<T> existing = new HashSet<T>(existingComponents);

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                foreach (T newComponent in FindAllNewInScene<T>(scene, existing, searchType, maxDepth))
                    newComponents.Add(newComponent);
            }

            existingComponents.AddRange(newComponents);

            return newComponents;
        }

        public static IEnumerable<T> FindAllNewInScene<T>(Scene scene, List<T> existingComponents, SearchTypeEnum searchType = SearchTypeEnum.RootGameObjects, int maxDepth = -1) where T : class
        {
            List<T> newComponents = new List<T>();

            if (!scene.IsValid())
                return newComponents;

            if (!scene.isLoaded)
                return newComponents;

            HashSet<T> existing = new HashSet<T>(existingComponents);

            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                foreach (T newComponent in CheckGameObjectForNew<T>(rootGameObject, existing, searchType, 0, maxDepth))
                    newComponents.Add(newComponent);
            }

            existingComponents.AddRange(newComponents);

            return newComponents;
        }

        private static IEnumerable<T> FindAllNewInScene<T>(Scene scene, HashSet<T> existingComponents, SearchTypeEnum searchType = SearchTypeEnum.RootGameObjects, int maxDepth = -1) where T : class
        {
            if (!scene.IsValid())
                yield break;

            if (!scene.isLoaded)
                yield break;
                        
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                foreach (T newComponent in CheckGameObjectForNew<T>(rootGameObject, existingComponents, searchType, 0, maxDepth))
                    yield return newComponent;
            }
        }

        private static IEnumerable<T> CheckGameObjectForNew<T>(GameObject gameObject, HashSet<T> existingComponents, SearchTypeEnum searchType = SearchTypeEnum.RootGameObjects, int depth = 0, int maxDepth = -1) where T : class
        {
            foreach (Component goComponent in gameObject.GetComponents(typeof(T)))
            {
                T component = goComponent as T;
                if (!existingComponents.Contains(component))
                    yield return component;
            }

            switch (searchType)
            {
                case SearchTypeEnum.Recursive:

                    if (maxDepth >= 0)
                    {
                        depth++;
                        if (depth >= maxDepth)
                            yield break;
                    }

                    foreach (Transform child in gameObject.transform)
                    {
                        foreach (T newComponent in CheckGameObjectForNew<T>(child.gameObject, existingComponents, searchType, depth, maxDepth))
                            yield return newComponent;
                    }
                    break;

                case SearchTypeEnum.RootGameObjects:
                default:
                    break;
            }
        }
    }
}