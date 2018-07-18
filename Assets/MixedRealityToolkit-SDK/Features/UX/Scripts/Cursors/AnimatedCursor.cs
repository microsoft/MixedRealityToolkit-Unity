// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Cursors
{
    /// <summary>
    /// Animated cursor is a cursor driven using an animator to inject state information
    /// and animate accordingly
    /// </summary>
    public class AnimatedCursor : BaseCursor
    {
        [SerializeField]
        [Header("Animated Cursor State Data")]
        [Tooltip("Cursor state data to use for its various states.")]
        private AnimatedCursorData[] cursorStateData = null;

        [SerializeField]
        [Tooltip("Animator parameter to set when input is enabled.")]
        private AnimatorParameter inputEnabledParameter = default(AnimatorParameter);

        [SerializeField]
        [Tooltip("Animator parameter to set when input is disabled.")]
        private AnimatorParameter inputDisabledParameter = default(AnimatorParameter);

        /// <summary>
        /// Link the cursor animator.
        /// </summary>
        [SerializeField]
        [Tooltip("Animator for the cursor")]
        protected Animator CursorAnimator = null;

        /// <summary>
        /// Change animation state when enabling input.
        /// </summary>
        public override void OnInputEnabled()
        {
            base.OnInputEnabled();
            SetAnimatorParameter(inputEnabledParameter);
        }

        /// <summary>
        /// Change animation state when disabling input.
        /// </summary>
        public override void OnInputDisabled()
        {
            base.OnInputDisabled();
            SetAnimatorParameter(inputDisabledParameter);
        }

        /// <summary>
        /// Override to set the cursor animation trigger.
        /// </summary>
        public override void OnFocusChanged(FocusEventData eventData)
        {
            base.OnFocusChanged(eventData);

            if (Pointer.CursorModifier != null)
            {
                if ((Pointer.CursorModifier.CursorParameters != null) && (Pointer.CursorModifier.CursorParameters.Length > 0))
                {
                    OnCursorStateChange(CursorStateEnum.Contextual);
                    foreach (var param in Pointer.CursorModifier.CursorParameters)
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
        /// Override OnCursorState change to set the correct animation state for the cursor.
        /// </summary>
        /// <param name="state"></param>
        public override void OnCursorStateChange(CursorStateEnum state)
        {
            base.OnCursorStateChange(state);
            if (state == CursorStateEnum.Contextual) { return; }

            for (int i = 0; i < cursorStateData.Length; i++)
            {
                if (cursorStateData[i].CursorState == state)
                {
                    SetAnimatorParameter(cursorStateData[i].Parameter);
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

            switch (animationParameter.ParameterType)
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