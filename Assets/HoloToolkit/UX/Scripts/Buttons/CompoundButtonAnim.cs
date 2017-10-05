//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using System.Collections;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Anim controller button offers as simple way to link button states to animation controller parameters
    /// </summary>
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonAnim : MonoBehaviour
    {
        [Serializable]
        public struct AnimatorControllerAction
        {
            public Button.ButtonStateEnum ButtonState;
            public string ParamName;
            public AnimatorControllerParameterType ParamType;
            public bool BoolValue;
            public int IntValue;
            public float FloatValue;
            public bool InvalidParam;
        }

        /// <summary>
        /// List of animation actions
        /// </summary>
        [HideInInspector]
        public AnimatorControllerAction[] AnimActions;

        /// <summary>
        /// Animator
        /// </summary>
        public Animator TargetAnimator;

        void Awake ()
        {
            GetComponent<Button>().StateChange += StateChange;
            if (TargetAnimator == null)
            {
                TargetAnimator = GetComponent<Animator>();
            }
        }

        /// <summary>
        /// State change
        /// </summary>
        void StateChange(Button.ButtonStateEnum newState)
        {
            if (TargetAnimator == null)
            {
                return;
            }

            if (AnimActions == null)
            {
                //TODO error?
                return;
            }
            
            for (int i = 0; i < AnimActions.Length; i++)
            {
                if (AnimActions[i].ButtonState == newState)
                {
                    switch (AnimActions[i].ParamType)
                    {
                        case AnimatorControllerParameterType.Bool:
                            TargetAnimator.SetBool(AnimActions[i].ParamName, AnimActions[i].BoolValue);
                            break;

                        case AnimatorControllerParameterType.Float:
                            TargetAnimator.SetFloat(AnimActions[i].ParamName, AnimActions[i].FloatValue);
                            break;

                        case AnimatorControllerParameterType.Int:
                            TargetAnimator.SetInteger(AnimActions[i].ParamName, AnimActions[i].IntValue);
                            break;

                        case AnimatorControllerParameterType.Trigger:
                            TargetAnimator.SetTrigger(AnimActions[i].ParamName);
                            break;

                        default:
                            break;
                    }
                    break;
                }
            }
        }
    }
}