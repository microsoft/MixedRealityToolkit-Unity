// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Created a copy of the AnimatorControllerParameter because that class is not Serializable
    /// and cannot be modified in the editor.
    /// </summary>
    [Serializable]
    public struct AnimatorParameter
    {
        [Tooltip("Type of the animation parameter to modify.")]
        public AnimatorControllerParameterType type;

        [Tooltip("If the animation parameter type is an int, value to set. Ignored otherwise.")]
        public int defaultInt;

        [Tooltip("If the animation parameter type is a float, value to set. Ignored otherwise.")]
        public float defaultFloat;

        [Tooltip("If the animation parameter type is a bool, value to set. Ignored otherwise.")]
        public bool defaultBool;

        [Tooltip("Name of the animation parameter to modify.")]
        public string name;

        private int? nameStringHash;
        public int nameHash
        {
            get
            {
                if (!nameStringHash.HasValue && !String.IsNullOrEmpty(name))
                {
                    nameStringHash = Animator.StringToHash(name);
                }
                return nameStringHash.Value;
            }
        }
    }

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
            public AnimatorParameter Parameter;
        }

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
        /// Link the the cursor animator
        /// </summary>
        [SerializeField]
        [Tooltip("Animator for the cursor")]
        protected Animator CursorAnimator = null;

        /// <summary>
        /// Change anim state when enabling input
        /// </summary>
        public override void OnInputEnabled()
        {
            base.OnInputEnabled();
            SetAnimatorParameter(InputEnabledParameter);
        }

        /// <summary>
        /// Change anim state when disabling input
        /// </summary>
        public override void OnInputDisabled()
        {
            base.OnInputDisabled();
            SetAnimatorParameter(InputDisabledParameter);
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
        /// <param name="stateDatum"></param>
        protected void SetAnimatorParameter(AnimatorParameter param)
        {
            // Return if we do not have an animator
            if (CursorAnimator == null)
            {
                return;
            }
            
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    CursorAnimator.SetBool(param.nameHash, param.defaultBool);
                    break;
                case AnimatorControllerParameterType.Float:
                    CursorAnimator.SetFloat(param.nameHash, param.defaultFloat);
                    break;
                case AnimatorControllerParameterType.Int:
                    CursorAnimator.SetInteger(param.nameHash, param.defaultInt);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    CursorAnimator.SetTrigger(param.nameHash);
                    break;
            }
        }

    }

}