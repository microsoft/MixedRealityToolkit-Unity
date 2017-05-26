using UnityEngine;

namespace HoloToolkit.Unity
{
    public class SingleInstance<T> : MonoBehaviour where T : SingleInstance<T>
    {
        private static T _Instance;
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    T[] objects = FindObjectsOfType<T>();
                    if (objects.Length != 1)
                    {
                        Debug.LogFormat("Expected exactly 1 {0} but found {1}", typeof(T).ToString(), objects.Length);
                    }
                    else
                    {
                        _Instance = objects[0];
                    }
                }
                return _Instance;
            }
        }
    }
}