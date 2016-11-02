// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Object that represents a cursor in 3D space controlled by gaze.
    /// </summary>
    public class SpriteCursor : Cursor
    {
        [Serializable]
        public struct SpriteCursorDatum
        {
            public string Name;
            public CursorStateEnum CursorState;
            public Sprite CursorSprite;
            public Color CursorColor;
        }

        [SerializeField]
        public SpriteCursorDatum[] CursorStateData;

        /// <summary>
        /// Sprite renderer to change.  If null find one in children
        /// </summary>
        public SpriteRenderer TargetRenderer;

        /// <summary>
        /// On enable look for a sprite renderer on children
        /// </summary>
        protected override void OnEnable()
        {
            if(TargetRenderer == null)
            {
                TargetRenderer = GetComponentInChildren<SpriteRenderer>();
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
                for (int i = 0; i < CursorStateData.Length; i++)
                {
                    if (CursorStateData[i].CursorState == state)
                    {
                        SetCursorState(CursorStateData[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Based on the type of animator state info pass it through to the animator
        /// </summary>
        /// <param name="stateDatum"></param>
        private void SetCursorState(SpriteCursorDatum stateDatum)
        {
            // Return if we do not have an animator
            if (TargetRenderer != null)
            {
                TargetRenderer.sprite = stateDatum.CursorSprite;
                TargetRenderer.color = stateDatum.CursorColor;
            }
        }

    }

}