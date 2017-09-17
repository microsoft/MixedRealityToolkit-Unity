// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Animated cursor is a cursor driven using an animator to inject state information
    /// and animate accordingly
    /// </summary>
    public class AnimatedCursor : Cursor
    {
        /// <summary>
        /// Data struct for cursor state information for the Animated Cursor, which leverages the Unity animation system..
        /// This defines a modification to an Unity animation parameter, based on cursor state.
        /// </summary>
        [Serializable]
        public struct AnimCursorDatum
        {
            public string Name;
            public CursorStateEnum CursorState;

            /// <summary>
            /// Types that an animation parameter can have in the Unity animation system.
            /// </summary>
            public enum AnimInputTypeEnum
            {
                Int,
                Trigger,
                Bool,
                Float
            }

            [Tooltip("Type of the animation parameter to modify.")]
            public AnimInputTypeEnum AnimInputType;

            [Tooltip("Name of the animation parameter to modify.")]
            public string AnimParameterName;

            [Tooltip("If the animation parameter type is a bool, value to set. Ignored otherwise.")]
            public bool AnimBoolValue;

            [Tooltip("If the animation parameter type is an int, value to set. Ignored otherwise.")]
            public int AnimIntValue;

            [Tooltip("If the animation parameter type is a float, value to set. Ignored otherwise.")]
            public float AnimFloatValue;
        }

        /// <summary>
        /// Serialized set of cursor state data
        /// </summary>
        [Header("Animated Cursor State Data")]
        [Tooltip("Cursor state data to use for its various states")]
        [SerializeField]
        public AnimCursorDatum[] CursorStateData;

        /// <summary>
        /// Enabled state Data when enabling
        /// </summary>
        [Tooltip("Cursor State Data to use when enabling the cursor")]
        public AnimCursorDatum EnableStateData;

        /// <summary>
        /// Disabled state Data when disabled
        /// </summary>
        [Tooltip("Cursor State Data to use when the cursor is disabled")]
        public AnimCursorDatum DisableStateData;

        /// <summary>
        /// Link the the cursor animator
        /// </summary>
        [SerializeField]
        [Tooltip("Animator for the cursor")]
        protected Animator CursorAnimator = null;

        /// <summary>
        /// Change anim stage when enabled
        /// </summary>
        public override void OnInputEnabled()
        {
            base.OnInputEnabled();
            SetCursorState(EnableStateData);
        }

        /// <summary>
        /// Change anim stage when disabled
        /// </summary>
        public override void OnInputDisabled()
        {
            base.OnInputDisabled();
            SetCursorState(DisableStateData);
        }

        /// <summary>
        /// Override to set the cursor anim trigger
        /// </summary>
        /// <param name="modifier"></param>
        protected override void OnActiveModifier(CursorModifier modifier)
        {
            base.OnActiveModifier(modifier);

            if (modifier != null)
            {
                if(!string.IsNullOrEmpty(modifier.CursorTriggerName))
                {
                    OnCursorStateChange(CursorStateEnum.Contextual);
                    CursorAnimator.SetTrigger(modifier.CursorTriggerName);
                }
            }
            else
            {
                OnCursorStateChange(CursorStateEnum.None);
            }
        }

        /// <summary>
        /// Override OnCursorState change to set the correct animation
        /// state for the cursor
        /// </summary>
        /// <param name="state"></param>
        public override void OnCursorStateChange(CursorStateEnum state)
        {
            base.OnCursorStateChange(state);
            if(state != CursorStateEnum.Contextual)
            {
                for(int i = 0; i < CursorStateData.Length; i++)
                {
                    if(CursorStateData[i].CursorState == state)
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
        private void SetCursorState(AnimCursorDatum stateDatum)
        {
            // Return if we do not have an animator
            if (CursorAnimator == null)
            {
                return;
            }

            switch (stateDatum.AnimInputType)
            {
                case AnimCursorDatum.AnimInputTypeEnum.Bool:
                    CursorAnimator.SetBool(stateDatum.AnimParameterName, stateDatum.AnimBoolValue);
                    break;
                case AnimCursorDatum.AnimInputTypeEnum.Float:
                    CursorAnimator.SetFloat(stateDatum.AnimParameterName, stateDatum.AnimFloatValue);
                    break;
                case AnimCursorDatum.AnimInputTypeEnum.Int:
                    CursorAnimator.SetInteger(stateDatum.AnimParameterName, stateDatum.AnimIntValue);
                    break;
                case AnimCursorDatum.AnimInputTypeEnum.Trigger:
                    CursorAnimator.SetTrigger(stateDatum.AnimParameterName);
                    break;
            }
        }

    }

}