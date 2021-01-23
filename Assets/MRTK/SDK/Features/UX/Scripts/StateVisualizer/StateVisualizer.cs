// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Graphs;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.SDK.Editor")]
namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [RequireComponent(typeof(Animator))]
    public class StateVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("")]
        private List<StateContainer> stateContainers = new List<StateContainer>();

        /// <summary>
        /// The container for each state that stores the list of the state animatable properties.  
        /// </summary>
        public List<StateContainer> StateContainers
        {
            get => stateContainers;
            protected set => stateContainers = value;
        }

        [SerializeField]
        [Tooltip("")]
        private BaseInteractiveElement interactiveElement;

        /// <summary>
        /// 
        /// </summary>
        public BaseInteractiveElement InteractiveElement
        {
            get => interactiveElement;
            set => interactiveElement = value;
        }

        [SerializeField]
        [Tooltip("")]
        private Animator animator;

        /// <summary>
        /// 
        /// </summary>
        public Animator Animator
        {
            get => animator;
            set => animator = value;
        }

        // The states being tracked within an Interactive Element 
        public List<InteractionState> States => InteractiveElement != null ? InteractiveElement.States : null;

        // The state manager within the Interactive Element
        private StateManager stateManager;

        public AnimatorStateMachine RootStateMachine;

        public AnimatorController AnimatorController;

        private string defaultAnimationAssetSavePath = "Assets/MixedRealityToolkit.Generated/";

        private void OnValidate()
        {
            if (InteractiveElement == null)
            {
                if (gameObject.GetComponent<BaseInteractiveElement>() != null)
                {
                    InteractiveElement = gameObject.GetComponent<BaseInteractiveElement>();
                }
            }

            if (Animator == null)
            {
                Animator = gameObject.GetComponent<Animator>();
            }

            if (stateContainers.Count == 0)
            {
                InitializeStateContainers();
            }
        }

        private void Start()
        {
            if (InteractiveElement == null)
            {
                InteractiveElement = gameObject.AddComponent<InteractiveElement>();
            }

            stateManager = InteractiveElement.StateManager;

            InitializeStateContainers();

            if (AnimatorController == null)
            {
                InitializeAnimatorControllerAsset();
            }

            stateManager.OnStateActivated.AddListener((state) =>
            {
                Animator.SetTrigger("On" + state.Name);
            });
        }

        #region Animator State Methods

        /// <summary>
        /// Initialize the Animator State Machine by creating new animator states to match the states in Interactive Element. 
        /// </summary>
        /// <param name="animatorController">The animation controller contained in the attached Animator component</param>
        public void SetUpStateMachine(AnimatorController animatorController)
        {
            // Update Animation Clip References
            RootStateMachine = animatorController.layers[0].stateMachine;
            AnimatorController = animatorController;

            foreach (var stateContainer in StateContainers)
            {
                AddNewStateToStateMachine(stateContainer.StateName, animatorController);
            }

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Add a new state to the animator state machine and generate a new associated animation clip.
        /// </summary>
        /// <param name="stateName">The name of the new animation state</param>
        /// <param name="animatorController">The animation controller contained in the attached Animator component</param>
        /// <returns></returns>
        private AnimatorState AddNewStateToStateMachine(string stateName, AnimatorController animatorController)
        {
            // Create animation state
            AnimatorState animatorState = AddAnimatorState(RootStateMachine, stateName);

            // Add associated parameter
            AddAnimatorParameter(animatorController, "On" + stateName, AnimatorControllerParameterType.Trigger);

            // Create and attach animation clip
            AddAnimationClip(animatorState);

            AddAnyStateTransition(RootStateMachine, animatorState);

            StateContainer stateContainer = GetStateContainer(stateName);
            stateContainer.AnimatorStateMachine = RootStateMachine;

            return animatorState;
        }

        private AnimatorState AddAnimatorState(AnimatorStateMachine stateMachine, string animatorStateName)
        {
            bool doesStateExist = Array.Exists(stateMachine.states, (animatorState) => animatorState.state.name == animatorStateName);
            
            if (!doesStateExist)
            {
                return stateMachine.AddState(animatorStateName);
            }
            else
            {
                Debug.LogError($"The {animatorStateName} state already exisits in the animator state machine");
                return null;
            }
        }

        private void AddAnimatorParameter(AnimatorController animatorController, string parameterName, AnimatorControllerParameterType animatorParameterType)
        {
            animatorController.AddParameter(parameterName, animatorParameterType);
        }

        private void AddAnimationClip(AnimatorState animatorState)
        {
            AnimationClip stateAnimationClip = new AnimationClip();
            stateAnimationClip.name = gameObject.name + "_" + animatorState.name + "Clip";

            string animationClipFileName = stateAnimationClip.name + ".anim";

            AssetDatabase.CreateAsset(stateAnimationClip, GetAnimationDirectoryPath() + "/" + animationClipFileName);

            animatorState.motion = stateAnimationClip;

            StateContainer stateContainer = GetStateContainer(animatorState.name);

            stateContainer.AnimationClip = stateAnimationClip;
        }

        private void AddAnyStateTransition(AnimatorStateMachine animatorStateMachine, AnimatorState animatorState)
        {
            // Idle state
            AnimatorStateTransition transition = animatorStateMachine.AddAnyStateTransition(animatorState);
            transition.name = "To" + animatorState.name;

            // Add Trigger Parameter as a condition for the transition
            transition.AddCondition(AnimatorConditionMode.If, 0, "On" + animatorState.name);
        }

        /// <summary>
        /// Remove an animator state from the state machine.  Used in the StateVisualizerInspector
        /// </summary>
        /// <param name="stateMachine">The state machine for state removal</param>
        /// <param name="animatorStateName">The name of the animator state</param>
        internal void RemoveAnimatorState(AnimatorStateMachine stateMachine, string animatorStateName)
        {
            AnimatorState animatorStateToRemove = GetAnimatorState(animatorStateName);

            stateMachine.RemoveState(animatorStateToRemove);
        }

        /// <summary>
        /// Get the path where the animation controller and animation clips assets are located. 
        /// </summary>
        /// <returns>Returns path to the animation controller and animation clip assets</returns>
        public string GetAnimationDirectoryPath()
        {
            string animationDirectoryPath = Path.Combine(defaultAnimationAssetSavePath, "MRTK_Animations");

            // If the animation directory path does not exist, then create a new directory
            if (!Directory.Exists(animationDirectoryPath))
            {
                Directory.CreateDirectory(animationDirectoryPath);
            }

            return animationDirectoryPath;
        }

        // Create a new animator controller asset and add it to the MixedRealityToolkit.Generated folder. 
        // Then set up the state machine for the animator controller.
        internal void InitializeAnimatorControllerAsset()
        {
            // Create MRTK_Animation Directory if it does not exist
            string animationAssetDirectory = GetAnimationDirectoryPath();
            string animatorControllerName = gameObject.name + ".controller";
            string animationControllerPath = Path.Combine(animationAssetDirectory, animatorControllerName);

            // Create Animation Controller 
            AnimatorController = AnimatorController.CreateAnimatorControllerAtPath(animationControllerPath);

            // Set the runtime animation controller 
            gameObject.GetComponent<Animator>().runtimeAnimatorController = AnimatorController;

            SetUpStateMachine(AnimatorController);
        }

        #endregion

        #region State Container Methods

        private void InitializeStateContainers()
        {
            if (States != null && StateContainers.Count == 0)
            {
                foreach (InteractionState state in States)
                {
                    AddStateAnimatableContainer(state.Name);
                }
            }
        }

        private void UpdateStateAnimatableContainers(List<InteractionState> interactionStates)
        {
            if (interactionStates.Count != StateContainers.Count)
            {
                if (interactionStates.Count > StateContainers.Count)
                {
                    foreach (InteractionState state in interactionStates)
                    {
                        // Find the container that matches the state
                        StateContainer animatableContainer = GetStateContainer(state.Name);

                        if (animatableContainer == null)
                        {
                            AddStateAnimatableContainer(state.Name);
                        }
                    }
                }
                else if (interactionStates.Count < StateContainers.Count)
                {
                    foreach (StateContainer animatableContainer in StateContainers.ToList())
                    {
                        // Find the state in tracked states for this container
                        InteractionState trackedState = interactionStates.Find((state) => (state.Name == animatableContainer.StateName));

                        // Do not remove the default state
                        if (trackedState == null)
                        {
                            RemoveStateAnimatableContainer(animatableContainer.StateName);
                        }
                    }
                }
            }
        }

        private void RemoveStateAnimatableContainer(string stateName)
        {
            StateContainer containerToRemove = StateContainers.Find((container) => container.StateName == stateName);

            StateContainers.Remove(containerToRemove);
        }

        private void AddStateAnimatableContainer(string stateName)
        {
            StateContainer stateAnimatableContainer = new StateContainer(stateName);

            StateContainers.Add(stateAnimatableContainer);
        }

        /// <summary>
        /// Update the state containers in the state visualizer to match the states in InteractiveElement.  Used in the StateVisualizerInspector.
        /// </summary>
        internal void UpdateStateContainerStates()
        {
            UpdateStateAnimatableContainers(InteractiveElement.States);

            List<string> stateContainerNames = new List<string>();
            List<string> animatorStateNames = new List<string>();

            // Get state container names
            StateContainers.ForEach((stateContainer) => stateContainerNames.Add(stateContainer.StateName));

            // Get animation state names
            Array.ForEach(RootStateMachine.states, (animatorState) => animatorStateNames.Add(animatorState.state.name));

            var statesToAdd = stateContainerNames.Except(animatorStateNames);

            foreach (var state in statesToAdd)
            {
                AddNewStateToStateMachine(state, animator.runtimeAnimatorController as AnimatorController);
            }

            var statesToRemove = animatorStateNames.Except(stateContainerNames);

            foreach (var stateAni in statesToRemove)
            {
                RemoveAnimatorState(RootStateMachine, stateAni);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get state container given a state name.
        /// </summary>
        /// <param name="stateName">The name of the state container</param>
        /// <returns>The state container with given state name</returns>
        public StateContainer GetStateContainer(string stateName)
        {
            StateContainer stateContainer = StateContainers.Find((container) => container.StateName == stateName);

            return stateContainer != null ? stateContainer : null;
        }

        public void AddAnimatableProperty(string stateName, int animationTargetIndex, AnimatableProperty animatableProperty)
        {
            StateContainer stateContainer = GetStateContainer(stateName);

            CreateAnimatablePropertyInstance(animationTargetIndex, animatableProperty.ToString(), stateName);
        }

        //public T GetAnimatableProperty<T>(string stateName, int animationTargetIndex) where T : StateAnimatableProperty
        //{
        //    StateContainer stateContainer = GetStateContainer(stateName);

        //    AnimationTarget animationTarget = stateContainer.AnimationTargets[animationTargetIndex];

        //    animationTarget.StateAnimatableProperties.Find((animatableProperty) => animatableProperty.AnimatablePropertyName.);

        //    return T;
        //}

        /// <summary>
        /// Set the keyframes for a given animatable property. 
        /// </summary>
        /// <param name="stateName">The name of the state container</param>
        /// <param name="animationTargetIndex">The index of the animation target game object</param>
        /// <param name="animatablePropertyName">The name of the animatable property</param>
        public void SetKeyFrames(string stateName, int animationTargetIndex)
        {
            StateContainer stateContainer = GetStateContainer(stateName);

            stateContainer.SetKeyFrames(animationTargetIndex);
        }

        /// <summary>
        /// Remove previously set keyframes. 
        /// </summary>
        /// <param name="stateName">The name of the state container</param>
        /// <param name="animationTargetIndex">The index of the animation target game object</param>
        /// <param name="animatablePropertyName">The name of the animatable property</param>
        public void RemoveKeyFrames(string stateName, int animationTargetIndex, string animatablePropertyName)
        {
            StateContainer stateContainer = GetStateContainer(stateName);

            stateContainer.RemoveKeyFrames(animationTargetIndex, animatablePropertyName);
        }

        /// <summary>
        /// Set the AnimationTransitionDuration for a state.
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        /// <param name="transitionDurationValue">The duration of the transition in seconds</param>
        public void SetAnimationTransitionDuration(string stateName, float transitionDurationValue)
        {
            StateContainer stateContainer = GetStateContainer(stateName);

            if (stateContainer.AnimatorStateMachine == null)
            {
                stateContainer.AnimatorStateMachine = RootStateMachine;
            }

            stateContainer.AnimationTransitionDuration = transitionDurationValue;
        }

        /// <summary>
        /// Set the animation clip for a state.
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        /// <param name="animationClip">The animation clip to set</param>
        public void SetAnimationClip(string stateName, AnimationClip animationClip)
        {
            StateContainer stateContainer = GetStateContainer(stateName);
            stateContainer.AnimationClip = animationClip;
        }

        /// <summary>
        /// Get an animator state in the animator state machine by state name.
        /// </summary>
        /// <param name="animatorStateName">The name of the animator state</param>
        /// <returns></returns>
        public AnimatorState GetAnimatorState(string animatorStateName)
        {
            return Array.Find(RootStateMachine.states, (animatorState) => animatorState.state.name == animatorStateName).state;
        }

        #endregion

        internal void CreateAnimatablePropertyInstance(int animationTargetIndex, string animatablePropertyName, string stateName)
        {
            StateContainer stateContainer = GetStateContainer(stateName);

            if (stateContainer != null)
            {
                stateContainer.CreateAnimatablePropertyInstance(animationTargetIndex, animatablePropertyName, stateName);
            }
        }
    }
}
