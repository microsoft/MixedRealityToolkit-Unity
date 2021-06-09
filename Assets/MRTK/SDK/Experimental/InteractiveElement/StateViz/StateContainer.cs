// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer
{
    /// <summary>
    /// Container class for an Interactive State in the StateVisualizer component. Each state container maps to an 
    /// Interactive State in an attached Interactive Element component.
    /// </summary>
    [Serializable]
    public class StateContainer
    {
        /// <summary>
        /// The state container constructor.
        /// </summary>
        /// <param name="stateName">The state name for this state container</param>
        public StateContainer(string stateName)
        {
            StateName = stateName;
        }

        [SerializeField]
        [Tooltip("The name of the state this container. This state container name is the same as the state name of the associated" +
            "Interactive State in Interactive Element.")]
        private string stateName = null;

        /// <summary>
        /// The name of the state this container. This state container name is the same as the state name of the associated 
        /// Interactive State in Interactive Element.
        /// </summary>
        public string StateName
        {
            get => stateName;
            set => stateName = value;
        }

        [SerializeField]
        [Tooltip("The list of animation targets for this state container.  Each animation target contains a list " +
            "of animatable properties. " +
            "NOTE: Once a target is added, the keyframes for the animation clip for this state container will be overwritten by the state animatable properties.")]
        private List<AnimationTarget> animationTargets = new List<AnimationTarget>();

        /// <summary>
        /// The list of animation targets for this state container.  Each animation target contains a list 
        /// of animatable properties.
        /// 
        /// NOTE:
        /// Once a target is added, the keyframes for the animation clip for this state container will be overwritten by the state animatable
        /// properties.
        /// </summary>
        public List<AnimationTarget> AnimationTargets
        {
            get => animationTargets;
            set => animationTargets = value;
        }

        [SerializeField]
        [Tooltip("The Animation Clip for this state container. Keyframes for this animation clip can be modified via the Unity Animation Window OR via " +
            "this inspector by adding a new animation target. ")]
        private AnimationClip animationClip = null;

        /// <summary>
        /// The Animation Clip for this state container. Keyframes for this animation clip can be modified via the Unity Animation Window OR via
        /// this inspector by adding a new animation target. 
        /// </summary>
        public AnimationClip AnimationClip
        {
            get => animationClip;
            set
            {

#if UNITY_EDITOR
                if (animationClip != null)
                {
                    SetAnimationClipInStateMachine(value);
                }
#endif

                animationClip = value;
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        [Tooltip("The time in seconds for the animation transition between states.")]
        private float animationTransitionDuration = 0.25f;

        /// <summary>
        /// The time in seconds for the animation transition between states.
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

        internal AnimatorStateMachine AnimatorStateMachine { get; set; }

        // Set the animation transition duration in the editor animation state machine
        internal void SetAnimationTransitionDuration(string stateName)
        {
            AnimatorStateTransition[] transitions = AnimatorStateMachine.anyStateTransitions;

            string transitionName = "To" + stateName;

            AnimatorStateTransition animationTransition = Array.Find(transitions, (transition) => transition.name == transitionName);

            animationTransition.duration = AnimationTransitionDuration;
        }

        internal void SetAnimationClipInStateMachine(AnimationClip clip)
        {
            ChildAnimatorState animatorState = Array.Find(AnimatorStateMachine.states, (state) => state.state.name == StateName);

            animatorState.state.motion = clip;
        }
#endif

        internal StateAnimatableProperty CreateAnimatablePropertyInstance(int animationTargetIndex, string animatablePropertyName, string stateName)
        {
            return AnimationTargets[animationTargetIndex].CreateAnimatablePropertyInstance(animatablePropertyName, stateName);
        }

        internal void SetKeyFrames(int animationTargetIndex)
        {
            AnimationTargets[animationTargetIndex].SetKeyFrames(AnimationClip);
        }

        internal void RemoveKeyFrames(int animationTargetIndex, string animatablePropertyName)
        {
            AnimationTargets[animationTargetIndex].RemoveKeyFrames(animatablePropertyName, AnimationClip);
        }
    }
}