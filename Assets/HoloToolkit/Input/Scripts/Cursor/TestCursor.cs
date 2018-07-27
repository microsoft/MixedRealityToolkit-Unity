using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// The test cursor can switch between different game objects based on its state.
    /// It simply links the game object to set to active with its associated cursor state.
    /// It also handles cursor modification with TestCursorModifier.
    /// </summary>
    public class TestCursor : Cursor
    {
        [System.Serializable]
        public struct ObjectCursor
        {
            public CursorStateEnum CursorState;
            public GameObject CursorObject;
        }

        [SerializeField]
        public ObjectCursor[] Cursors;

        private GameObject userDefinedCursor = null;

        protected override void OnActiveModifier(ICursorModifier modifier)
        {
            if (modifier != null)
            {
                if (modifier is TestCursorModifier)
                {
                    TestCursorModifier cursorModifier = modifier as TestCursorModifier;
                    userDefinedCursor = cursorModifier.GetCursor();
                    userDefinedCursor.transform.SetParent(transform, false);
                    HideDefaultCursors();
                }
                else if (modifier is CursorModifier)
                {
                    base.OnActiveModifier(modifier);
                }
            }
            else
            {
                if (userDefinedCursor != null)
                {
                    Destroy(userDefinedCursor);
                    userDefinedCursor = null;
                }
            }
        }

        /// <summary>
        /// Override OnCursorStateChange to set the correct cursor
        /// state for the cursor
        /// </summary>
        /// <param name="state"></param>
        public override void OnCursorStateChange(CursorStateEnum state)
        {
            base.OnCursorStateChange(state);

            if(userDefinedCursor != null)
            {
                return;
            }

            if (Cursors != null)
            {
                for (int i = 0; i < Cursors.Length; i++)
                {
                    Cursors[i].CursorObject.SetActive(false);
                }
                for (int i = 0; i < Cursors.Length; i++)
                {
                    if (Cursors[i].CursorState == state)
                    {
                        Cursors[i].CursorObject.SetActive(true);
                    }
                }
            }
        }

        /// <summary>
        /// Hides the default cursors
        /// </summary>
        private void HideDefaultCursors()
        {
            if (Cursors != null)
            {
                for (int i = 0; i < Cursors.Length; i++)
                {
                    Cursors[i].CursorObject.SetActive(false);
                }
            }
        }
    }
}