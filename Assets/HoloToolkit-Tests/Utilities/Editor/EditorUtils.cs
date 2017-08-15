using UnityEngine;

namespace HoloToolkit.Unity
{
    public static class EditorUtils
    {
        /// <summary>
        /// Deletes all objects in the scene
        /// </summary>
        public static void ClearScene()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                //only destroy root objects
                if (gameObject.transform.parent == null)
                {
                    Object.DestroyImmediate(gameObject);
                }
            }
        }
    }
}
