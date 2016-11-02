//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Animated cursor is cursor driven using an animator to inject state information
    /// and animate accordingly
    /// </summary>
    public class AnimatedCursor : Cursor
    {
        /// <summary>
        /// Data struct for cursor state information for the Animated Cursor
        /// </summary>
        [Serializable]
        public struct AnimCursorDatum
        {
            public string Name;
            public CursorStateEnum CursorState;

            public enum AnimInputTypeEnum
            {
                Int,
                Trigger,
                Bool,
                Float
            }

            public AnimInputTypeEnum AnimInputType;
            public string AnimParameterName;
            public bool AnimBoolValue;
            public int AnimIntValue;
            public float AnimFloatValue;
        }

        [Header("Animated Cursor State Data")]
        /// <summary>
        /// Serialized set of cursor state data
        /// </summary>
        [SerializeField]
        public AnimCursorDatum[] CursorStateData;

        /// <summary>
        /// Enabled state Data when enabling
        /// </summary>
        public AnimCursorDatum EnableStateData;

        /// <summary>
        /// Disabled state Data when disabled
        /// </summary>
        public AnimCursorDatum DisableStateData;

        /// <summary>
        /// Link the the curaor animator
        /// </summary>
        [SerializeField]
        protected Animator CursorAnimator = null;

        /// <summary>
        /// Change anim stage when enabled
        /// </summary>
        public override void EnableInput()
        {
            base.EnableInput();
            SetCursorState(EnableStateData);
        }

        /// <summary>
        /// Change anim stage when disabled
        /// </summary>
        public override void DisableInput()
        {
            base.DisableInput();
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