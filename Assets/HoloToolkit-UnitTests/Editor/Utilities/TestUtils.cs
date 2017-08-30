// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Unity
{
    public static class TestUtils
    {
        /// <summary>
        /// Objects created through test utils. The reason to keep them in a list is that disabled objects can't be found at all, or at least only in the active scene.
        /// So to be able to clear the whole scene we need to keep references.
        /// Instantiated disabled objects will only be found when they are in the test scene but not in the additive loaded original scene.
        /// </summary>
        private static readonly List<Transform> CreatedGameObjects = new List<Transform>();

        /// <summary>
        /// Deletes all objects in the scene
        /// </summary>
        public static void ClearScene()
        {
            ClearCreated();
            ClearUnreferencedActive();
            ClearUnreferencedDisabledInTestScene();
        }

        private static void ClearUnreferencedDisabledInTestScene()
        {
            DestroyGameObjects(SceneManager.GetActiveScene().GetRootGameObjects());
        }

        private static void ClearUnreferencedActive()
        {
            DestroyTransforms(Object.FindObjectsOfType<Transform>());
        }

        private static void ClearCreated()
        {
            DestroyTransforms(CreatedGameObjects);
        }

        private static void DestroyTransforms(IEnumerable<Transform> transforms)
        {
            DestroyGameObjects(transforms.Where(t => t).Select(t => t.root.gameObject).Distinct());
        }

        private static void DestroyGameObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                Object.DestroyImmediate(gameObject);
            }
        }

        public static GameObject CreateGameObject()
        {
            var gameObject = new GameObject();
            CreatedGameObjects.Add(gameObject.transform);
            return gameObject;
        }

        /// <summary>
        /// Creates a new primitive<see cref="GameObject"/> and saves a reference internally to be able to delete it in case it gets disabled.
        /// </summary>
        /// <param name="type">Desired type of the new object</param>
        /// <returns>The created primitive <see cref="GameObject"/></returns>
        public static GameObject CreatePrimitive(PrimitiveType type)
        {
            var gameObject = GameObject.CreatePrimitive(type);
            CreatedGameObjects.Add(gameObject.transform);
            return gameObject;
        }

        /// <summary>
        /// Creates a camera and adds the MainCamera tag to it
        /// </summary>
        /// <returns>The created camera</returns>
        public static Camera CreateMainCamera()
        {
            var camera = CreateGameObject().AddComponent<Camera>();
            camera.gameObject.tag = "MainCamera";
            return camera;
        }

        /// <summary>
        /// Calls Awake on all MonoBehaviours on the given gameObject through reflection.
        /// </summary>
        /// <param name="gameObject">The gameObject to be awoken</param>
        /// <returns>The given gameObject to be able to chain call</returns>
        internal static GameObject CallAwake(this GameObject gameObject)
        {
            foreach (var script in gameObject.GetComponents<MonoBehaviour>())
            {
                script.Call("Awake");
            }
            return gameObject;
        }

        /// <summary>
        /// Calls Start on all MonoBehaviours on the given gameObject through reflection.
        /// </summary>
        /// <param name="gameObject">The gameObject to be started</param>
        /// <returns>The given gameObject to be able to chain call</returns>
        internal static GameObject CallStart(this GameObject gameObject)
        {
            foreach (var script in gameObject.GetComponents<MonoBehaviour>())
            {
                script.Call("Start");
            }
            return gameObject;
        }

        /// <summary>
        /// Calls Update on all MonoBehaviours on the given gameObject through reflection.
        /// </summary>
        /// <param name="gameObject">The gameObject to be updated</param>
        /// <returns>The given gameObject to be able to chain call</returns>
        internal static GameObject CallUpdate(this GameObject gameObject)
        {
            foreach (var script in gameObject.GetComponents<MonoBehaviour>())
            {
                script.Call("Update");
            }
            return gameObject;
        }

        /// <summary>
        /// Call a method with the given name from the given object through reflection
        /// </summary>
        /// <param name="obj">Object to call the method on</param>
        /// <param name="methodName">The method that should be called</param>
        private static void Call(this object obj, string methodName)
        {
            const BindingFlags findFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var methodInfo = obj.GetType().GetMethod(methodName, findFlags);
            if (methodInfo == null) return;
            methodInfo.Invoke(obj, null);
        }
    }
}
