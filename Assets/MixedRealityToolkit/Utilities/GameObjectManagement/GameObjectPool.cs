// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Utilities.GameObjectManagement
{
    /// <summary>
    /// Used to recycle Unity GameObjects. When ever you create GameObjects during runtime some overhead is incurred. Additionally
    /// memory can become highly fragment as well as possibly causing the garbage collector to perform a collection (which is also
    /// a performance hit). This is especially prevelant when you are spawning and destroying GameObjects of the same type
    /// very quickly in large quantities (such as bullets). The GameObject pool allows you to recycle objects so they can be
    /// reused upon request.
    /// </summary>
    /// <example>
    /// Setup code for using the generic prefab instance creator:
    /// <code>
    /// GameObjectPool pool = new GameObjectPool();
    /// GenericPrefabInstanceCreator creator = new GenericPrefabInstanceCreator();
    /// creator.Prefab = MyProjectilePrefab;
    /// pool.AddCreator(creator, "projectile1");
    /// </code>
    /// Requesting a game object from the pool:
    /// <code>
    /// var myProjectileObj = pool.GetGameObject("projectile1");
    /// </code>
    /// Recycling the game object:
    /// <code>
    /// pool.Recycle(myProjectileObj, "projectile1");
    /// </code>
    /// </example>
    /// <remarks>Note that the GameObjectPool is not thread safe. It should only be used in Unity's main thread.</remarks>
    public class GameObjectPool
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameObjectPool"/> class.
        /// </summary>
        public GameObjectPool()
        {
            _pool = new Dictionary<string, Queue<GameObject>>();
            _creators = new Dictionary<string, GameObjectCreator>();
        }

        /// <summary>
        /// GameObjects are created by an implementation of IGameObjectCreator in this GameObjectPool. This
        /// method adds your implemenation of the IGameObjectCreator to use for objects that share a specific
        /// object identifier.
        /// </summary>
        /// <param name="creator">The implementation of IGameObjectCreator to use for GameObjects associated with the objectIdentifier.</param>
        /// <param name="objectIdentifier">The identifier you want to use to identify the kind of game objects you want to create.</param>
        public void AddCreator(GameObjectCreator creator, string objectIdentifier)
        {
            _creators.Add(objectIdentifier, creator);
        }

        /// <summary>
        /// Adds a game object under a specific object identifier to the GameObjectPool.
        /// </summary>
        /// <param name="gameObject">The GameObject to recycle.</param>
        /// <param name="objectIdentifier">The identifier you want to use to identify the kind of game object you are recycling.</param>
        public void Recycle(GameObject gameObject, string objectIdentifier)
        {
            EnsureListForObjectID(objectIdentifier);

            if (_creators.ContainsKey(objectIdentifier))
            {
                _creators[objectIdentifier].PrepareForRecycle(gameObject);
            }

            gameObject.SetActive(false);
            _pool[objectIdentifier].Enqueue(gameObject);
        }

        /// <summary>
        /// Gets a game object for a specific object identifier from the GameObjectPool. If the kind of game object
        /// being requested is not in the pool, then it will get created by a IGameObjectCreator that was
        /// added to the pool for handling objects associated with the objectIdentifier.
        /// </summary>
        /// <param name="objectIdentifier">The identifier you want to use to identifiy the kind of game object you want to retrieve.</param>
        /// <param name="position">The position that the game object should have before it is activated.</param>
        /// <param name="rotation">The rotation that the game object should have before it is activated.</param>
        /// <returns></returns>
        public GameObject GetGameObject(string objectIdentifier, Vector3 position, Quaternion rotation)
        {
            GameObject obj = null;
            GameObjectCreator creator = null;
            if (_creators.ContainsKey(objectIdentifier))
            {
                creator = _creators[objectIdentifier];
            }
            else
            {
                Debug.Log("Unable to create a GameObject for object ID '" + objectIdentifier + "' because no IGameObjectCreator implementation can be found for it.");
                return null;
            }

            EnsureListForObjectID(objectIdentifier);

            Queue<GameObject> objects = _pool[objectIdentifier];

            if (objects.Count > 0)
            {
                obj = objects.Dequeue();
            }
            else
            {
                obj = creator.Instantiate();
            }

            if (obj != null)
            {
                creator.PrepareForUse(obj);
                obj.SetActive(true);
            }

            return obj;
        }

        /// <summary>
        /// Same as calling GetGameObject(objectIdentifier, Vector3.zero, Quaternion.identity)
        /// </summary>
        /// <param name="objectIdentifier">The identifier you want to use to identifiy the kind of game object you want to retrieve.</param>
        /// <returns></returns>
        public GameObject GetGameObject(string objectIdentifier)
        {
            return GetGameObject(objectIdentifier, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Gets the number of game objects in the pool for a specific identifier.
        /// </summary>
        /// <param name="objectIdentifier"></param>
        /// <returns></returns>
        public int Count(string objectIdentifier)
        {
            EnsureListForObjectID(objectIdentifier);

            return _pool[objectIdentifier].Count;
        }

        /// <summary>
        /// Removes and destroys all game objects in the pool associated with the specified objectIdentifier.
        /// </summary>
        /// <param name="objectIdentifier">The identifier you want to use to identify the kind of game objects to remove from the pool.</param>
        public void EmptyPool(string objectIdentifier)
        {
            EnsureListForObjectID(objectIdentifier);

            Queue<GameObject> objects = _pool[objectIdentifier];
            foreach (GameObject obj in objects)
            {
                GameObject.Destroy(obj);
            }
            objects.Clear();
        }

        /// <summary>
        /// Removes and destroys all game objects in the pool.
        /// </summary>
        public void EmptyPool()
        {
            foreach (string objectID in _pool.Keys)
            {
                EmptyPool(objectID);
            }
            _pool.Clear();
        }

        #region Private

        /// <summary>
        /// The pool for game objects
        /// </summary>
        private Dictionary<string, Queue<GameObject>> _pool;

        /// <summary>
        /// All creators that this pool can handle by identifier
        /// </summary>
        private Dictionary<string, GameObjectCreator> _creators;

        /// <summary>
        /// Ensures there is a list for the specified identifier
        /// </summary>
        /// <param name="objectIdentifier"></param>
        private void EnsureListForObjectID(string objectIdentifier)
        {
            if (!_pool.ContainsKey(objectIdentifier))
            {
                _pool.Add(objectIdentifier, new Queue<GameObject>());
            }
        }

        #endregion
    }
}
