using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Singleton behaviour class, used for components that should only have one instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
        private static List<Action> actionsWaitingForInitialization = new List<Action>();

        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Returns whether the instance has been initialized or not.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                return instance != null;
            }
        }

        /// <summary>
        /// Base awake method that sets the singleton's unique instance.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Debug.LogErrorFormat("Trying to instantiate a second instance of singleton class {0}", GetType().Name);
            }
            else
            {
                instance = (T)this;
                actionsWaitingForInitialization.ForEach(action => action());
                actionsWaitingForInitialization = null;
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Performs an action when the singleton is initialized.
        /// If the singleton is already initialized, the action will be called instantanly.
        /// Actions can be registered multiple times. In this case, the actions will also be called multiple times.
        /// </summary>
        /// <param name="action">Action to perform when the singleton is initialized.</param>
        public static void WhenInitialized(Action action)
        {
            if (IsInitialized)
            {
                action();
            }
            else
            {
                actionsWaitingForInitialization.Add(action);
            }
        }
    }
}
