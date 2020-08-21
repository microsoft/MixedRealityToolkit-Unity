// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Animated cursor is a cursor driven using an animator to inject state information
    /// and animate accordingly
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/AnimatedCursor")]
    public class AnimatedCursor : BaseCursor
    {
        [SerializeField]
        [Header("Animated Cursor State Data")]
        [Tooltip("Cursor state data to use for its various states.")]
        private AnimatedCursorStateData[] cursorStateData = null;

        [SerializeField]
        [Header("Animated Cursor Context Data")]
        [Tooltip("Cursor context data to use for its various contextual states.")]
        private AnimatedCursorContextData[] cursorContextData = null;

        [SerializeField]
        [Tooltip("Animator parameter to set when input is enabled.")]
        private AnimatorParameter inputEnabledParameter = default(AnimatorParameter);

        [SerializeField]
        [Tooltip("Animator parameter to set when input is disabled.")]
        private AnimatorParameter inputDisabledParameter = default(AnimatorParameter);

        [SerializeField]
        [Tooltip("Animator for the cursor")]
        private Animator cursorAnimator = null;

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

            if (((Pointer is UnityEngine.Object) ? ((Pointer as UnityEngine.Object) != null) : (Pointer != null)) && Pointer.CursorModifier != null)
            {
                if ((Pointer.CursorModifier.CursorParameters != null) && (Pointer.CursorModifier.CursorParameters.Length > 0))
                {
                    OnCursorStateChange(CursorStateEnum.Contextual);

                    for (var i = 0; i < Pointer.CursorModifier.CursorParameters.Length; i++)
                    {
                        SetAnimatorParameter(Pointer.CursorModifier.CursorParameters[i]);
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
        /// Override OnCursorContext change to set the correct animation state for the cursor.
        /// </summary>
        public override void OnCursorContextChange(CursorContextEnum context)
        {
            base.OnCursorContextChange(context);

            if (context == CursorContextEnum.Contextual) { return; }

            for (int i = 0; i < cursorContextData.Length; i++)
            {
                if (cursorContextData[i].CursorState == context)
                {
                    SetAnimatorParameter(cursorContextData[i].Parameter);
                }
            }
        }

        /// <summary>
        /// Based on the type of animator state info pass it through to the animator
        /// </summary>
        private void SetAnimatorParameter(AnimatorParameter animationParameter)
        {
            // Return if we do not have an animator
            if (cursorAnimator == null || !cursorAnimator.isInitialized)
            {
                return;
            }

            switch (animationParameter.ParameterType)
            {
                case AnimatorControllerParameterType.Bool:
                    cursorAnimator.SetBool(animationParameter.NameHash, animationParameter.DefaultBool);
                    break;
                case AnimatorControllerParameterType.Float:
                    cursorAnimator.SetFloat(animationParameter.NameHash, animationParameter.DefaultFloat);
                    break;
                case AnimatorControllerParameterType.Int:
                    cursorAnimator.SetInteger(animationParameter.NameHash, animationParameter.DefaultInt);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    cursorAnimator.SetTrigger(animationParameter.NameHash);
                    break;
            }
        }
    }
}