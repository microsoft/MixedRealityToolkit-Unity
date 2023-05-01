// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Playables;

using WeightType = Microsoft.MixedReality.Toolkit.UX.IAnimationMixableEffect.WeightType;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A high-performance Playables API-powered interaction feedback system with extensible effects.
    /// Requires <see cref="StatefulInteractable"/> and an <see cref="Animator"/>.
    /// </summary>
    [AddComponentMenu("MRTK/UX/State Visualizer")]
    [RequireComponent(typeof(Animator))]
    public class StateVisualizer : MonoBehaviour
    {
        // How long to wait after effects report they're done
        // before going to sleep. Some tolerance is needed as animators
        // can "lag" behind their intended motion in some instances.
        private const float keepAliveTime = 0.2f;

        // Number of wakeup events we subscribe to by default.
        // Used for nicer list initialization.
        private const int defaultWakeupEventCount = 8;

        [Serializable]
        /// <summary>
        /// A container that holds a list of effects, as well as the
        /// current value of the state.
        /// </summary>
        /// <remarks>
        /// All effects in a state share the same state value. However,
        /// each effect can respond to the value in different ways.
        /// Consider using a more appropriate Effect rather than adjusting how
        /// the value is submitted.
        /// </remarks>
        internal class State
        {
            [SerializeReference]
            [Tooltip("The list of effects to apply.")]
            private List<IEffect> effects = new List<IEffect>();

            /// <summary>
            /// The list of effects to apply.
            /// </summary>
            public List<IEffect> Effects => effects;

            /// <summary>
            /// The value [0,1] that controls the effects within this state.
            /// </summary>
            /// <remarks>
            /// See the documentation of each <see cref="IEffect"/> for how
            /// they respond to the state value.
            /// </remarks>
            public float Value { get; set; }

            /// <summary>
            /// The value from last frame. Used to detect
            /// changes in the state's value between frames.
            /// </summary>
            public float PreviousValue { get; set; }

            [SerializeField]
            // Used internally to hint to the editor that this is a variable/float-based state.
            // Has no effect otherwise.
            private bool isVariable;

            /// <summary>
            /// Used internally to hint to the editor that this is a variable/float-based state.
            /// Has no effect otherwise.
            /// </summary>
            public bool IsVariable
            {
                get => isVariable;
                set => isVariable = value;
            }
        }

        /// <summary>
        /// The collection of feedback states that this <see cref="StateVisualizer"/> operates on.
        /// </summary>
        [SerializeField]
        internal SerializableDictionary<string, State> stateContainers = new SerializableDictionary<string, State>();

        // Default states that are written at validation + startup.
        private readonly Dictionary<string, State> defaultStates = new Dictionary<string, State>()
        {
            { "Disabled", new State() },
            { "PassiveHover", new State() },
            { "ActiveHover", new State() },
            { "Select", new State() { IsVariable = true } },
            { "Toggle", new State() }
        };

        [SerializeField]
        [Tooltip("The connected interactable.")]
        private StatefulInteractable interactable;

        /// <summary>
        /// The connected interactable.
        /// </summary>
        public StatefulInteractable Interactable
        {
            get
            {
                if (interactable == null)
                {
                    interactable = GetComponentInParent<StatefulInteractable>();
                }

                return interactable;
            }
            set => interactable = value;
        }

        [SerializeField]
        [Tooltip("The Animator to be used as the output for the Playable graph.")]
        private Animator animator;

        /// <summary>
        /// The Animator to be used as the output for the Playable graph.
        /// </summary>
        public Animator Animator
        {
            get => animator;
            set => animator = value;
        }

        // The PlayableGraph that injects animation data into the Animator.
        private PlayableGraph playableGraph;

        // The single animation mixer that all animation-based effects mix into.
        private AnimationLayerMixerPlayable animationMixerPlayable;

        // Set to keepAliveTime when awake. Ticked down towards
        // zero when sleep is requested.
        private float sleepTimer = 0;

        // We hold on to a list of actions we use to unsubscribe from the wakeup events.
        private List<UnityAction> unsubscribeActions = new List<UnityAction>(defaultWakeupEventCount);

        // A runtime scratchpad for recording where each IMixableEffect is connected on the mixer.
        private Dictionary<IEffect, int> mixableIndices = new Dictionary<IEffect, int>();

        void OnValidate()
        {
            EnsureDefaultStates();
            if (interactable == null)
            {
                interactable = GetComponentInParent<StatefulInteractable>();
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private UnityAction Subscribe<T>(UnityEvent<T> genericEvent, UnityAction callback)
        {
            // Wrap argumentless callback in generic lambda
            UnityAction<T> wrapper = (_) => callback();
            genericEvent.AddListener(wrapper);

            // Return lambda that removes the listener.
            return () => genericEvent.RemoveListener(wrapper);
        }

        private UnityAction Subscribe(UnityEvent evt, UnityAction callback)
        {
            evt.AddListener(callback);
            return () => evt.RemoveListener(callback);
        }

        protected virtual void Start()
        {
            OnValidate();

            if (interactable != null)
            {
                unsubscribeActions.Add(Subscribe(interactable.hoverEntered, WakeUp));
                unsubscribeActions.Add(Subscribe(interactable.hoverExited, WakeUp));
                unsubscribeActions.Add(Subscribe(interactable.selectEntered, WakeUp));
                unsubscribeActions.Add(Subscribe(interactable.selectExited, WakeUp));
                unsubscribeActions.Add(Subscribe(interactable.IsToggled.OnEntered, WakeUp));
                unsubscribeActions.Add(Subscribe(interactable.IsToggled.OnExited, WakeUp));
                unsubscribeActions.Add(Subscribe(interactable.OnEnabled, WakeUp));
                unsubscribeActions.Add(Subscribe(interactable.OnDisabled, WakeUp));
            }

            // Creates the graph, the mixer and binds them to the Animator.
            playableGraph = PlayableGraph.Create();

            // We can use a single animation output for all animation-based playables.
            var animationPlayableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());

            // We use a single master mixer for all animation-based playables.
            // Two-way animation playables mix into this mixer.
            animationMixerPlayable = AnimationLayerMixerPlayable.Create(playableGraph, 1);
            animationPlayableOutput.SetSourcePlayable(animationMixerPlayable);

            foreach (var kvp in stateContainers)
            {
                foreach (IEffect effect in kvp.Value.Effects)
                {
                    if (effect == null) { continue; }

                    effect.Setup(playableGraph, gameObject);

                    // If the effect uses Playables (not all do!)
                    // we connect it to the Mixer and set the relevant weights + settings.
                    if (effect is IPlayableEffect playableEffect)
                    {
                        // Connect all AnimationEffects to our single, reusable AnimationMixer.
                        if (effect is IAnimationMixableEffect mixableEffect)
                        {
                            // Expand the mixer's slots to fit our new playable.
                            int currentSlot = animationMixerPlayable.GetInputCount();
                            animationMixerPlayable.SetInputCount(currentSlot + 1);
                            // animationMixerPlayable.SetInputWeight(currentSlot, 1);

                            // Configure the layer in the mixer, based on the effect's settings.
                            // For now, additive mixing is blocked by a Unity bug, described at the following links.
                            // AnimationLayerMixerPlayable writes defaults into the object's state upon being disabled,
                            // and then additively blends on top of what it was animating before it was disabled.
                            // https://forum.unity.com/threads/we-need-a-way-to-disable-write-defaults-in-the-playables-api.971643/
                            // https://forum.unity.com/threads/how-to-disable-writedefaults-in-custom-playable-graph-animator.717218/
                            animationMixerPlayable.SetLayerAdditive((uint)currentSlot, false);

                            playableGraph.Connect(playableEffect.Playable, 0, animationMixerPlayable, currentSlot);

                            // Record the index for later retrieval.
                            mixableIndices.Add(mixableEffect, currentSlot);
                        }
                        else
                        {
                            // If needed, we can build custom mixer support here. This was actually implemented
                            // in an earlier version of this system, but the complexity was unnecessary for current needs.
                            // In the future; IPlayableEffect can be expanded to specify a CreateMixer function that
                            // creates a generic mixer playable that would be in charge of combining the outputs from
                            // all participating PlayableBehaviours of a given type. These would be tracked and connected here.
                            ScriptPlayableOutput output = ScriptPlayableOutput.Create(playableGraph, kvp.Value + ":" + effect.GetType().Name);
                            output.SetSourcePlayable(playableEffect.Playable);
                        }
                    }
                }
            }

            // Start awake. We'll go back to sleep if nothing happens.
            animator.enabled = true;
            playableGraph.Play();
            enabled = true;
        }

        /// <summary>
        /// Ensures that the default states are present.
        /// </summary>
        protected virtual void EnsureDefaultStates()
        {
            foreach (var kvp in defaultStates)
            {
                if (!stateContainers.ContainsKey(kvp.Key))
                {
                    stateContainers.Add(kvp.Key, kvp.Value);
                }
            }
        }

        private void WakeUp()
        {
            enabled = true;
            sleepTimer = keepAliveTime;
        }

        private void Update()
        {
            bool valueChanged = UpdateStateValues();

            // If parameters have changed but the animator is currently disabled, wake up!
            if (valueChanged && !animator.enabled)
            {
                animator.enabled = true;
                playableGraph.Play();
                sleepTimer = keepAliveTime;
            }

            // If we're asleep, quit early.
            if (!animator.enabled) { return; }

            // Returns true if all effects are done playing.
            if (EvaluateEffects())
            {
                // Have we been "done" long enough to go to sleep?
#pragma warning disable UNT0004 // Using fixedDeltaTime to avoid going to sleep too early when frames hang (like at startup)
                sleepTimer -= Time.fixedDeltaTime;
#pragma warning restore UNT0004 // Using fixedDeltaTime to avoid going to sleep too early when frames hang (like at startup)

                // Only sleep if we're not currently selected or hovered.
                // This seems counter-intuitive, but we do this because an animation may need to be
                // kicked off when the float value of a state changes. We don't have a wakeup event
                // for a "value changed", so we just stay awake while we are hovered (or selected).
                if (sleepTimer <= 0 && interactable != null && !interactable.isSelected && !interactable.isHovered)
                {
                    // All effects are done, let's go to sleep.
                    animator.enabled = false;
                    playableGraph.Stop();
                    enabled = false;
                }
            }
            else
            {
                // We're not done, so reset the sleep timer.
                sleepTimer = keepAliveTime;
            }
        }

        private void OnDestroy()
        {
            foreach (var unsubscribeAction in unsubscribeActions)
            {
                unsubscribeAction();
            }

            // Destroys all Playables and Outputs created by the graph.
            if (playableGraph.IsValid())
            {
                playableGraph.Destroy();
            }
        }

        /// <summary>
        /// Adds the provided effect to the state with name <paramref name="stateName"/>.
        /// Creates the state if it doesn't exist.
        /// </summary>
        /// <param name="stateName">The name of the state to add the effect to.</param>
        /// <param name="effect">The effect to add.</param>
        internal void AddEffect(string stateName, IEffect effect)
        {
            if (!stateContainers.ContainsKey(stateName))
            {
                stateContainers.Add(stateName, new State());
            }

            stateContainers[stateName].Effects.Add(effect);
        }

        /// <summary>
        /// Removes the specified effect from the state with name <paramref name="stateName"/>.
        /// </summary>
        /// <param name="stateName">The name of the state to remove the effect from.</param>
        /// <param name="effect">The effect to remove.</param>
        /// <returns>True if the effect was removed, false otherwise.</returns>
        internal bool RemoveEffect(string stateName, IEffect effect)
        {
            if (stateContainers.ContainsKey(stateName))
            {
                return stateContainers[stateName].Effects.Remove(effect);
            }
            return false;
        }

        private static readonly ProfilerMarker StateVisualizerUpdateStateValuesMarker =
            new ProfilerMarker("[MRTK] StateVisualizer.UpdateStateValues");

        // TODO: Custom states/effects should probably be able to set their own parameters.
        // Given that custom states can't yet be added to the StateVisualizer, this
        // is a non-issue for now.
        /// <summary>
        /// Sets the parameter value on each state. 
        /// Override + extend this method to implement custom state parameters.
        /// </summary>
        /// <returns>
        /// True if the parameter has changed, false otherwise.
        /// </returns>
        protected virtual bool UpdateStateValues()
        {
            if (interactable != null)
            {
                using (StateVisualizerUpdateStateValuesMarker.Auto())
                {
                    bool parameterChanged = false;
                    parameterChanged |= UpdateStateValue("Disabled", !interactable.enabled ? 1 : 0);
                    parameterChanged |= UpdateStateValue("PassiveHover", interactable.isHovered ? 1 : 0);
                    parameterChanged |= UpdateStateValue("ActiveHover", interactable.IsActiveHovered ? 1 : 0);
                    parameterChanged |= UpdateStateValue("Select", interactable.Selectedness());
                    parameterChanged |= UpdateStateValue("Toggle", interactable.IsToggled ? 1 : 0);
                    return parameterChanged;
                }
            }
            return false;
        }

        /// <summary>
        /// Manually sets a state to a given value.
        /// </summary>
        /// <param name="stateName">The name of the state to set.</param>
        /// <param name="value">The value to set the state to.</param>
        /// <returns>
        /// True if the parameter was changed this frame, false if it remained constant.
        /// </returns>
        internal bool UpdateStateValue(string stateName, float newValue)
        {
            if (stateContainers.TryGetValue(stateName, out var state))
            {
                state.Value = newValue;
                if (state.PreviousValue != newValue)
                {
                    state.PreviousValue = newValue;
                    return true; // The parameter changed!
                }
                state.PreviousValue = newValue;
            }

            return false;
        }

        private static readonly ProfilerMarker StateVisualizerEvaluateEffectsMarker =
            new ProfilerMarker("[MRTK] StateVisualizer.EvaluateEffects");

        /// <summary>
        /// Fires <see cref="IEffect.Evaluate"/> on all valid effects in every state.
        /// Uses the parameter currently set on the <see cref="StateVisualizer.State"/>.
        /// Call <see cref="StateVisualizer.UpdateStateValues"/> before calling this method.
        /// </summary>
        /// <returns>
        /// True if all effects are done playing, false otherwise. The <see cref="StateVisualizer"/>
        /// and connected <see cref="Animator"/> will be put to sleep if this returns true.
        /// </returns>
        private bool EvaluateEffects()
        {
            using (StateVisualizerEvaluateEffectsMarker.Auto())
            {
                bool allEffectsDone = true;

                foreach (var kvp in stateContainers)
                {
                    foreach (IEffect effect in kvp.Value.Effects)
                    {
                        if (effect == null) { continue; }

                        // If it's a mixable effect, we need to update the weighting.
                        if (effect is IAnimationMixableEffect mixableEffect)
                        {
                            allEffectsDone &= UpdateWeight(mixableEffect, kvp.Value);
                        }

                        allEffectsDone &= effect.Evaluate(kvp.Value.Value);
                    }
                }

                return allEffectsDone;
            }
        }

        // Updates weights on mixable effects, based on transition settings (or lack thereof)
        // Assumes it will be called once per frame (for transition timing)
        // Returns true if all transitions are complete, false otherwise.
        private bool UpdateWeight(IAnimationMixableEffect mixableEffect, State state)
        {
            bool done = true;

            if (mixableEffect.WeightMode == WeightType.MatchStateValue)
            {
                // Set the playable's weight directly to the state's value.
                animationMixerPlayable.SetInputWeight(mixableEffect.Playable, state.Value);
            }
            else if (mixableEffect.WeightMode == WeightType.Transition)
            {
                // Grab the current weight, using our cached mixable indices.
                float currentWeight = animationMixerPlayable.GetInputWeight(mixableIndices[mixableEffect]);

                // Compute the direction of the transition.
                bool shouldBeActive = !Mathf.Approximately(state.Value, 0.0f);
                int transitionDirection = shouldBeActive ? 1 : -1;

                // Compute and set the new weight.
                float newWeight = Mathf.Clamp01(currentWeight + transitionDirection * (Time.deltaTime / mixableEffect.TransitionDuration));
                animationMixerPlayable.SetInputWeight(mixableEffect.Playable, newWeight);

                // If we're still transitioning, make sure we don't mark the effect as done.
                if ((shouldBeActive && !Mathf.Approximately(newWeight, 1.0f)) || (!shouldBeActive && !Mathf.Approximately(newWeight, 0.0f)))
                {
                    done = false;
                }
            }
            else
            {
                // WeightType.Constant is the only remaining option; the weight is always 1.0.
                animationMixerPlayable.SetInputWeight(mixableEffect.Playable, 1.0f);
            }

            return done;
        }
    }
}
