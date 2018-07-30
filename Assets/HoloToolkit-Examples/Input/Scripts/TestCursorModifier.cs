using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// Component that can be added to any game object with a collider to modify 
    /// how a cursor reacts when on that collider.
    /// </summary>
    public class TestCursorModifier : MonoBehaviour, ICursorModifier
    {
        [SerializeField]
        private GameObject cursorPrefab = null;
        private GameObject cursor = null;

        public bool GetCursorVisibility()
        {
            return true;
        }

        public Vector3 GetModifiedPosition(ICursor cursor)
        {
            return cursor.Position;
        }

        public Quaternion GetModifiedRotation(ICursor cursor)
        {
            return cursor.Rotation;
        }

        public Vector3 GetModifiedScale(ICursor cursor)
        {
            return cursor.LocalScale;
        }

        public void GetModifiedTransform(ICursor cursor, out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            position = GetModifiedPosition(cursor);
            rotation = GetModifiedRotation(cursor);
            scale = GetModifiedScale(cursor);
        }

        /// <summary>
        /// Instantiates the cursor if it is null and returns it
        /// </summary>
        /// <returns>Instantiated cursor game object</returns>
        public GameObject GetCursor()
        {
            if (cursor == null)
            {
                cursor = Instantiate(cursorPrefab, Vector3.zero, Quaternion.identity);
            }
            return cursor;
        }
    }
}