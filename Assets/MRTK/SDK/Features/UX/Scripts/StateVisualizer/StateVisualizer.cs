// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
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
    [RequireComponent(typeof(InteractiveElement))]
    [RequireComponent(typeof(Animator))]
    public class StateVisualizer : MonoBehaviour
    {
        [SerializeField]
        private List<StateContainer> stateContainers = new List<StateContainer>();

        /// <summary>
        /// The container for each state that stores the list of the state style properties.  
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

        // List of the core style propery names
        private string[] coreStyleProperties = Enum.GetNames(typeof(CoreStyleProperty)).ToArray();

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


        public void OnValidate()
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

        /// <summary>
        /// 
        /// </summary>
        public void UpdateStateContainerStates()
        {
            UpdateStateStyleContainers(InteractiveElement.States);

            List<string> stateContainerNames = new List<string>();
            List<string> animatorStateNames = new List<string>();

            // Get state container names
            StateContainers.ForEach((stateContainer) => stateContainerNames.Add(stateContainer.StateName));

            // Get animation state names
            Array.ForEach(RootStateMachine.states, (animatorState) => animatorStateNames.Add(animatorState.state.name));

            var statesToAdd = stateContainerNames.Except(animatorStateNames);

            foreach(var state in statesToAdd)
            {
                AddNewStateToStateMachine(state, animator.runtimeAnimatorController as AnimatorController);
            }

            var statesToRemove = animatorStateNames.Except(stateContainerNames);

            foreach (var stateAni in statesToRemove)
            {
                RemoveAnimatorState(RootStateMachine, stateAni);
            }
        }

        private void Start()
        {
            stateManager = InteractiveElement.StateManager;

            InitializeStateContainers();

            stateManager.OnStateActivated.AddListener(
                (state) =>
                {
                    Animator.SetTrigger("On" + state.Name);
                });            
        }

        #region Animator State Methods

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

        public AnimatorState AddNewStateToStateMachine(string stateName, AnimatorController animatorController)
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

        public void RemoveAnimatorState(AnimatorStateMachine stateMachine, string animatorStateName)
        {
            AnimatorState animatorStateToRemove = GetAnimatorState(animatorStateName);

            stateMachine.RemoveState(animatorStateToRemove);
        }

        public AnimatorState GetAnimatorState(string animatorStateName)
        {
            return Array.Find(RootStateMachine.states, (animatorState) => animatorState.state.name == animatorStateName).state;
        }

        public void SetKeyFrames(string stateName, int animationTargetIndex, string stylePropertyName)
        {
            StateContainer stateContainer = GetStateContainer(stateName);

            stateContainer.SetKeyFrames(animationTargetIndex, stylePropertyName);
        }

        public void RemoveKeyFrames(string stateName, int animationTargetIndex, string stylePropertyName)
        {
            StateContainer stateContainer = GetStateContainer(stateName);

            stateContainer.RemoveKeyFrames(animationTargetIndex, stylePropertyName);
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

        public void SetAnimationTransition(string stateName, float value)
        {
            StateContainer stateContainer = GetStateContainer(stateName);

            stateContainer.AnimationTransitionDuration = value;
        }


        #endregion


        #region State Container Methods

        public void InitializeStateContainers()
        {
            if (States != null && StateContainers.Count == 0)
            {
                foreach (InteractionState state in States)
                {
                    AddStateStyleContainer(state.Name);
                }
            }
        }

        public void UpdateStateStyleContainers(List<InteractionState> interactionStates)
        {
            if (interactionStates.Count != StateContainers.Count)
            {
                if (interactionStates.Count > StateContainers.Count)
                {
                    foreach (InteractionState state in interactionStates)
                    {
                        // Find the container that matches the state
                        StateContainer styleContainer = GetStateContainer(state.Name);

                        if (styleContainer == null)
                        {
                            AddStateStyleContainer(state.Name);
                        }
                    }
                }
                else if (interactionStates.Count < StateContainers.Count)
                {
                    foreach (StateContainer styleContainer in StateContainers.ToList())
                    {
                        // Find the state in tracked states for this container
                        InteractionState trackedState = interactionStates.Find((state) => (state.Name == styleContainer.StateName));

                        // Do not remove the default state
                        if (trackedState == null)
                        {
                            RemoveStateStyleContainer(styleContainer.StateName);
                        }
                    }
                }
            }
        }

        public StateContainer GetStateContainer(string stateName)
        {
            StateContainer stateContainer = StateContainers.Find((container) => container.StateName == stateName);

            if (stateContainer != null)
            {
                return stateContainer;
            }
            else
            {
                Debug.LogError($"The {stateName} state does not have an existing state container for state style properties");
                return null;
            }
        }

        private void RemoveStateStyleContainer(string stateName)
        {
            StateContainer containerToRemove = StateContainers.Find((container) => container.StateName == stateName);

            StateContainers.Remove(containerToRemove);
        }

        private void AddStateStyleContainer(string stateName)
        {
            StateContainer stateStyleContainer = new StateContainer(stateName);

            StateContainers.Add(stateStyleContainer);
        }

        #endregion


        public void CreateStylePropertyInstance(int animationTargetIndex, string stylePropertyName, string stateName)
        {
            StateContainer stateContainer = GetStateContainer(stateName);

            if (stateContainer != null)
            {
                stateContainer.CreateStylePropertyInstance(animationTargetIndex, stylePropertyName, stateName);
            }
        }
    }
}
