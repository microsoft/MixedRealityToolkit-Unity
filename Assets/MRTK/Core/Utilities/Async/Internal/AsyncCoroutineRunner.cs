// MIT License

// Copyright(c) 2016 Modest Tree Media Inc

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// This Async Coroutine Runner is just an object to
    /// ensure that coroutines run properly with async/await.
    /// </summary>
    /// <remarks>
    /// <para>The object that this MonoBehavior is attached to must be a root object in the
    /// scene, as it will be marked as DontDestroyOnLoad (so that when scenes are changed,
    /// it will persist instead of being destroyed). The runner will force itself to
    /// the root of the scene if it's rooted elsewhere.</para>
    /// </remarks>
    [AddComponentMenu("Scripts/MRTK/Core/AsyncCoroutineRunner")]
    internal sealed class AsyncCoroutineRunner : MonoBehaviour
    {
        private static AsyncCoroutineRunner instance;

        private static bool isInstanceRunning = false;

        private static readonly Queue<Action> Actions = new Queue<Action>();

        internal static AsyncCoroutineRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AsyncCoroutineRunner>();
                }

                // FindObjectOfType() only search for objects attached to active GameObjects. The FindObjectOfType(bool includeInactive) variant is not available to Unity 2019.4 and earlier so cannot be used.
                // We instead search for GameObject called AsyncCoroutineRunner and see if it has the component attached.
                if (instance == null)
                {
                    var instanceGameObject = GameObject.Find("AsyncCoroutineRunner");

                    if (instanceGameObject != null)
                    {
                        instance = instanceGameObject.GetComponent<AsyncCoroutineRunner>();

                        if (instance == null)
                        {
                            Debug.Log("[AsyncCoroutineRunner] Found a \"AsyncCoroutineRunner\" GameObject but didn't have the AsyncCoroutineRunner component attached. Attaching the script.");
                            instance = instanceGameObject.AddComponent<AsyncCoroutineRunner>();
                        }
                    }
                }

                if (instance == null)
                {
                    Debug.Log("[AsyncCoroutineRunner] There is no AsyncCoroutineRunner in the scene. Adding a GameObject with AsyncCoroutineRunner attached at the root of the scene.");
                    instance = new GameObject("AsyncCoroutineRunner").AddComponent<AsyncCoroutineRunner>();
                }
                else if (!instance.isActiveAndEnabled)
                {
                    if (!instance.enabled)
                    {
                        Debug.LogWarning("[AsyncCoroutineRunner] Found a disabled AsyncCoroutineRunner component. Enabling the component.");
                        instance.enabled = true;
                    }
                    if (!instance.gameObject.activeSelf)
                    {
                        Debug.LogWarning("[AsyncCoroutineRunner] Found an AsyncCoroutineRunner attached to an inactive GameObject. Setting the GameObject active.");
                        instance.gameObject.SetActive(true);
                    }
                }

                instance.gameObject.hideFlags = HideFlags.None;

                // AsyncCoroutineRunner must be at the root so that we can call DontDestroyOnLoad on it.
                // This is ultimately to ensure that it persists across scene loads/unloads.
                if (instance.transform.parent != null)
                {
                    Debug.LogWarning($"[AsyncCoroutineRunner] AsyncCoroutineRunner was found as a child of another GameObject {instance.transform.parent}, " +
                        "it must be a root object in the scene. Moving the AsyncCoroutineRunner to the root.");
                    instance.transform.parent = null;
                }

#if !UNITY_EDITOR
                DontDestroyOnLoad(instance);
#endif
                return instance;
            }
        }

        internal static void Post(Action task)
        {
            lock (Actions)
            {
                Actions.Enqueue(task);
            }
        }

        internal static bool IsInstanceRunning => isInstanceRunning;

        private void Update()
        {
            if (Instance != this)
            {
                Debug.Log("[AsyncCoroutineRunner] Multiple active AsyncCoroutineRunners is present in the scene. Disabling duplicate ones.");
                enabled = false;
                return;
            }
            isInstanceRunning = true;

            int actionCount;

            lock (Actions)
            {
                actionCount = Actions.Count;
            }

            for (int i = 0; i < actionCount; i++)
            {
                Action next;

                lock (Actions)
                {
                    next = Actions.Dequeue();
                }

                next();
            }
        }

        private void OnDisable()
        {
            if (instance == this)
            {
                isInstanceRunning = false;
            }
        }

        private void OnEnable()
        {
            if (Instance != this)
            {
                Debug.Log("[AsyncCoroutineRunner] Multiple active AsyncCoroutineRunners is present in the scene. Disabling duplicate ones.");
                enabled = false;
            }
            else
            {
                isInstanceRunning = true;
            }
        }
    }
}
