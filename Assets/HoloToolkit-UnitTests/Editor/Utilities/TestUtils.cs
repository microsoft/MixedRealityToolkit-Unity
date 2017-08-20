// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public static class TestUtils
    {
        /// <summary>
        /// Objects created through test utils. The reason to keep them in a list is that disabled objects can't be found at all.
        /// So to be able to clear the whole scene we need to keep references.
        /// Instantiated disabled objects will still not be found.
        /// </summary>
        private static readonly List<Transform> CreatedGameObjects = new List<Transform>();

        /// <summary>
        /// Deletes all active objects in the scene
        /// </summary>
        public static void ClearScene()
        {
            ClearCreated();
            ClearUnreferencedActive();
        }

        private static void ClearUnreferencedActive()
        {
            foreach (var transform in Object.FindObjectsOfType<Transform>().Select(t => t.root).Distinct().ToList())
            {
                Object.DestroyImmediate(transform.gameObject);
            }
        }

        private static void ClearCreated()
        {
            foreach (var transform in CreatedGameObjects.Where(t => t).Select(t => t.root).Distinct().ToList())
            {
                Object.DestroyImmediate(transform.gameObject);
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
        /// Calls Awake on all monobehaviours on the given gameObject through reflection.
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
        /// Calls Start on all monobehaviours on the given gameObject through reflection.
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
        /// Calls Update on all monobehaviours on the given gameObject through reflection.
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
