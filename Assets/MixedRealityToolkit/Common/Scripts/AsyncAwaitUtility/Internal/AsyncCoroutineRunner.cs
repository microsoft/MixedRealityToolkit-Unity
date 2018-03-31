using UnityEngine;

namespace MixedRealityToolkit.Common.AsyncAwaitUtilities.Internal
{
    /// <summary>
    /// This Async Coroutine Runner is just a helper object to
    /// ensure that coroutines run properly with async/await.
    /// </summary>
    public class AsyncCoroutineRunner : MonoBehaviour
    {
        private static AsyncCoroutineRunner instance;

        public static AsyncCoroutineRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AsyncCoroutineRunner>();
                }

                if (instance == null)
                {
                    instance = new GameObject("AsyncCoroutineRunner").AddComponent<AsyncCoroutineRunner>();
                    instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
#if !UNITY_EDITOR
                    DontDestroyOnLoad(instance);
#endif
                }

                return instance;
            }
        }

        private void Update()
        {
            Debug.Assert(Instance != null);
        }
    }
}
