// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [Serializable]
    public class StateContainer 
    {
        public StateContainer(string stateName) 
        {
            StateName = stateName;
        }

        [SerializeField]
        private string stateName = null;

        /// <summary>
        /// The name of the state this container is tracking 
        /// </summary>
        public string StateName
        {
            get => stateName;
            set => stateName = value;
        }

        [SerializeField]
        private List<AnimationTarget> animationTargets = new List<AnimationTarget>();

        /// <summary>
        /// 
        /// </summary>
        public List<AnimationTarget> AnimationTargets
        {
            get => animationTargets;
            set => animationTargets = value;
        }

        [SerializeField]
        private AnimationClip animationClip = null;

        /// <summary>
        ///
        /// </summary>
        public AnimationClip AnimationClip
        {
            get => animationClip;
            set => animationClip = value;
        }

        [SerializeField]
        private float animationTransitionDuration = 0.25f;

        /// <summary>
        ///
        /// </summary>
        public float AnimationTransitionDuration
        {
            get => animationTransitionDuration;
            set 
            {
                animationTransitionDuration = value;
                SetAnimationTransitionDuration(StateName);
            } 
        }

        public AnimatorStateMachine AnimatorStateMachine { get; internal set; }

        public void CreateStylePropertyInstance(int animationTargetIndex, string stylePropertyName, string stateName)
        {
            AnimationTargets[animationTargetIndex].CreateStylePropertyInstance(stylePropertyName, stateName);
        }

        public void SetKeyFrames(int animationTargetIndex, string stylePropertyName)
        {
            AnimationTargets[animationTargetIndex].SetKeyFrames(stylePropertyName, AnimationClip);
        }

        public void RemoveKeyFrames(int animationTargetIndex, string stylePropertyName)
        {
            AnimationTargets[animationTargetIndex].RemoveKeyFrames(stylePropertyName, AnimationClip);
        }

        internal void SetAnimationTransitionDuration(string stateName)
        {
            AnimatorStateTransition[] transitions = AnimatorStateMachine.anyStateTransitions;

            string transitionName = "To" + stateName;

            AnimatorStateTransition animationTransition = Array.Find(transitions, (transition) => transition.name == transitionName);

            animationTransition.duration = AnimationTransitionDuration;
        }
    }
}