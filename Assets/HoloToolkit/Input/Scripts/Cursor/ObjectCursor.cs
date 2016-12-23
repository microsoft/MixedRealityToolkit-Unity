// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// The object cursor can switch between different game objects based on its state.
    /// It simply links the game object to set to active with its associated cursor state.
    /// </summary>
    public class ObjectCursor : Cursor
    {
        [Serializable]
        public struct ObjectCursorDatum
        {
            public string Name;
            public CursorStateEnum CursorState;
            public GameObject CursorObject;
        }

        [SerializeField]
        public ObjectCursorDatum[] CursorStateData;

        /// <summary>
        /// Sprite renderer to change.  If null find one in children
        /// </summary>
        public Transform ParentTransform;

        /// <summary>
        /// On enable look for a sprite renderer on children
        /// </summary>
        protected override void OnEnable()
        {
            if(ParentTransform == null)
            {
                ParentTransform = transform;
            }

            for (int i = 0; i < ParentTransform.childCount; i++)
            {
                ParentTransform.GetChild(i).gameObject.SetActive(false);
            }

            base.OnEnable();
        }

        /// <summary>
        /// Override OnCursorState change to set the correct animation
        /// state for the cursor
        /// </summary>
        /// <param name="state"></param>
        public override void OnCursorStateChange(CursorStateEnum state)
        {
            base.OnCursorStateChange(state);
            if (state != CursorStateEnum.Contextual)
            {
                var newActive = new ObjectCursorDatum();
                for(int i = 0; i < CursorStateData.Length; i++)
                {
                    ObjectCursorDatum c = CursorStateData[i];
                    if (c.CursorState == state)
                    {
                        newActive = c;
                        break;
                    }
                }

                if (newActive.Name == null)
                {
                    return;
                }

                for(int i = 0; i < CursorStateData.Length; i++)
                {
                    ObjectCursorDatum c = CursorStateData[i];
                    if (c.CursorObject.activeSelf)
                    {
                        c.CursorObject.SetActive(false);
                        break;
                    }
                }

                newActive.CursorObject.SetActive(true);
            }
        }
    }
}
