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
        /// Enabled state Data when enabling
        /// </summary>
        [Obsolete("Use InputEnabledParameter")]
        [Tooltip("Cursor State Data to use when enabling the cursor")]
        public AnimCursorDatum EnableStateData;

        /// <summary>
        /// Disabled state Data when disabled
        /// </summary>
        [Obsolete("Use InputDisabledParameter")]
        [Tooltip("Cursor State Data to use when the cursor is disabled")]
        public AnimCursorDatum DisableStateData;

        /// <summary>
        /// Serialized set of cursor state data
        /// </summary>
        [Header("Animated Cursor State Data")]
        [Tooltip("Cursor state data to use for its various states")]
        [SerializeField]
        public AnimCursorDatum[] CursorStateData;

        [Tooltip("Animator parameter to set when input is enabled.")]
        public AnimatorParameter InputEnabledParameter;

        [Tooltip("Animator parameter to set when input is disabled.")]
        public AnimatorParameter InputDisabledParameter;

        /// <summary>
        /// Link the cursor animator
        /// </summary>
        [SerializeField]
        [Tooltip("Animator for the cursor")]
        protected Animator CursorAnimator;

        /// <summary>
        /// Change animation state when enabling input
        /// </summary>
        public override void OnInputEnabled()
        {
            base.OnInputEnabled();
            SetAnimatorParameter(InputEnabledParameter);
        }

        /// <summary>
        /// Change animation state when disabling input
        /// </summary>
        public override void OnInputDisabled()
        {
            base.OnInputDisabled();
            SetAnimatorParameter(InputDisabledParameter);
        }

        /// <summary>
        /// Override to set the cursor animation trigger
        /// </summary>
        /// <param name="modifier"></param>
        protected override void OnActiveModifier(CursorModifier modifier)
        {
            base.OnActiveModifier(modifier);

            if (modifier != null)
            {
                if ((modifier.CursorParameters != null) && (modifier.CursorParameters.Length > 0))
                {
                    OnCursorStateChange(CursorStateEnum.Contextual);
                    foreach (var param in modifier.CursorParameters)
                    {
                        SetAnimatorParameter(param);
                    }
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
            if (state != CursorStateEnum.Contextual)
            {
                for (int i = 0; i < CursorStateData.Length; i++)
                {
                    if (CursorStateData[i].CursorState == state)
                    {
                        SetAnimatorParameter(CursorStateData[i].Parameter);
                    }
                }
            }
        }

        /// <summary>
        /// Based on the type of animator state info pass it through to the animator
        /// </summary>
        /// <param name="animationParameter"></param>
        protected void SetAnimatorParameter(AnimatorParameter animationParameter)
        {
            // Return if we do not have an animator
            if (CursorAnimator == null)
            {
                return;
            }

            switch (animationParameter.Type)
            {
                case AnimatorControllerParameterType.Bool:
                    CursorAnimator.SetBool(animationParameter.NameHash, animationParameter.DefaultBool);
                    break;
                case AnimatorControllerParameterType.Float:
                    CursorAnimator.SetFloat(animationParameter.NameHash, animationParameter.DefaultFloat);
                    break;
                case AnimatorControllerParameterType.Int:
                    CursorAnimator.SetInteger(animationParameter.NameHash, animationParameter.DefaultInt);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    CursorAnimator.SetTrigger(animationParameter.NameHash);
                    break;
            }
        }
    }
}