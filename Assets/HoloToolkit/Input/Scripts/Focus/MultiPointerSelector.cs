// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class MultiPointerSelector : MonoBehaviour
    {
        #region Settings

        [Tooltip("The default cursor which should follow active pointers.")]
        public Cursor CursorPrefab;

        #endregion

        #region Data

        private bool started;
        private bool addedInputManagerListener;
        private Dictionary<IPointingSource, Cursor> cursors = new Dictionary<IPointingSource, Cursor>();
        private List<KeyValuePair<IPointingSource, Cursor>> destroyedCursors = new List<KeyValuePair<IPointingSource, Cursor>>();

        #endregion

        #region MonoBehaviour Implementation

        private void Start()
        {
            started = true;

            InputManager.AssertIsInitialized();
            FocusManager.AssertIsInitialized();
        }

        #endregion

        private void Update()
        {
            UpdateActivePointers();
        }

        private void UpdateActivePointers()
        {
            var pointingSources = FocusManager.Instance.ActivePointingSources;

            // Make sure our pointing sources have the correct cursors
            for (int i = 0; i < pointingSources.Count; i++)
            {
                IPointingSource pointingSource = pointingSources[i];
                Cursor associatedCursor = null;
                Cursor cursorPrefab = pointingSource.CursorOverride != null ? pointingSource.CursorOverride : CursorPrefab;

                // See if there's an associated cursor of the correct type associated with this pointer
                // If one doesn't exist, or if the existing cursor was destroyed, create one
                if (!cursors.TryGetValue(pointingSource, out associatedCursor) || associatedCursor == null)
                {
                    associatedCursor = CreateCursor(pointingSource, cursorPrefab);
                }
                else if (associatedCursor.name != cursorPrefab.name)
                {
                    // The desired cursor prefab has changed - destroy the existing cursor and instantiate a new one
                    cursors.Remove(pointingSource);
                    GameObject.Destroy(associatedCursor.gameObject);
                    associatedCursor = CreateCursor(pointingSource, cursorPrefab);
                }
            }

            // Now check to see whether our cursors need to be culled or deactivated
            destroyedCursors.Clear();
            foreach (KeyValuePair<IPointingSource, Cursor> cursor in cursors)
            {
                // If the pointer has been destroyed, destroy the cursor as well
                if (cursor.Key == null)
                {
                    destroyedCursors.Add(cursor);
                }
                else
                {
                    // Disable cursor if interaction for this pointer is disabled
                    cursor.Value.gameObject.SetActive(cursor.Key.InteractionEnabled);
                }
            }

            // Remove all destroyed pointers and destroy unneeded cursors
            for (int i = 0; i < destroyedCursors.Count; i++)
            {
                cursors.Remove(destroyedCursors[i].Key);
                GameObject.Destroy(destroyedCursors[i].Value.gameObject);
            }
        }

        private Cursor CreateCursor(IPointingSource forPointingSource, Cursor usingPrefab)
        {
            Cursor associatedCursor = null;
            GameObject newCursorGo = GameObject.Instantiate(usingPrefab.gameObject) as GameObject;
            // Make sure to re-name the cursor gameobject to match its prefab for later checks
            newCursorGo.name = usingPrefab.name;
            associatedCursor = newCursorGo.GetComponent<Cursor>();
            associatedCursor.Pointer = forPointingSource;
            cursors.Add(forPointingSource, associatedCursor);
            return associatedCursor;
        }
    }
}