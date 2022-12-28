// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The InteractorDwellManager is attached to an interactor GameObject to allow
    /// the interactor trigger far/gaze dwell on <see cref="StatefulInteractable"/>.
    /// </summary>
    /// <remarks>
    /// After entering the trigger selection state the interactor will keep selecting before <see cref="dwellTriggerTime"/> passes, after which the selection ends.
    /// </remarks>
    [RequireComponent(typeof(XRBaseInteractor))]
    [AddComponentMenu("MRTK/Input/Interactor Dwell Manager")]
    public class InteractorDwellManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How long does the interactor remain selecting the interactable after entering dwell? " +
            "Selection on the interactable stops after this time passes.")]
        private float dwellTriggerTime = 0.3f;

        /// <summary>
        /// How long does the interactor remain selecting the interactable after enter dwell?
        /// Selection on the interactable stops after this time passes.
        /// </summary>
        public float DwellTriggerTime
        {
            get => dwellTriggerTime;
        }

        private XRBaseInteractor interactor;

        /// <summary>
        /// A dictionary that keeps track of dwell-enabled StatefulInteractables the interactor is interacting with.
        /// </summary>
        /// <remarks>
        /// <para>The value refers to the state of the interaction:</para>
        /// <para>If the value is positive, it is the time remaining before we trigger select on the interactable.</para>
        /// <para>If the value is zero, we trigger select on the interactable (calling StartManualInteraction).</para>
        /// <para>If the value is negative but not negative infinity, it is the time passed since we triggered select on the interactable.
        /// We stop triggering select (call EndManualInteraction) when the absolute value exceeds DwellTriggerTime.</para>
        /// <para>If the value is negative infinity, we have stopped triggering select (EndManualInteraction has been called).</para>
        /// </remarks>
        private readonly Dictionary<StatefulInteractable, float> interactableDict = new Dictionary<StatefulInteractable, float>();

        private void Awake()
        {
            if (!TryGetComponent(out interactor))
            {
                Debug.LogError("Cannot locate an XRBaseInteractor on this GameObject, which is required by InteractorDwellManager.");
            }
        }

        private void Update()
        {
            if (interactableDict.Count > 0)
            {
                foreach (var pair in interactableDict.ToReadOnlyCollection())
                {
                    // We have not trigged select on this interactable and are waiting till far/gaze dwell time passes. Reduce the timer by deltaTime.
                    if (pair.Value > 0)
                    {
                        interactableDict[pair.Key] = pair.Value - Time.deltaTime > 0 ? pair.Value - Time.deltaTime : 0;
                    }
                    // The time reaches zero, so trigger select on the interactable.
                    else if (pair.Value == 0)
                    {
                        interactor.StartManualInteraction(pair.Key as IXRSelectInteractable);
                        interactableDict[pair.Key] = -Time.deltaTime;
                    }
                    // Triggering select and waiting till DwellTriggerTime passes (manual interaction started but has yet finished).
                    else if (!float.IsNegativeInfinity(interactableDict[pair.Key]))
                    {
                        float time = pair.Value - Time.deltaTime;
                        // If dwellTriggerTime has elapsed we finish the interaction (stop selecting) as we don't keep the interactable selected indefinitely
                        if (time < -DwellTriggerTime)
                        {
                            if (interactor.IsSelecting(pair.Key))
                            {
                                interactor.EndManualInteraction();
                            }
                            // Use NegativeInfinity as a marker of interaction ended
                            interactableDict[pair.Key] = float.NegativeInfinity;
                            // Remove the interactable from dictionary only if it is not being hovered by the interactor
                            // so that we don't keep triggering if the interactor stays hovering
                            if (!interactor.IsHovering(pair.Key))
                            {
                                interactableDict.Remove(pair.Key);
                            }
                        }
                        else
                        {
                            interactableDict[pair.Key] = time;
                        }
                    }
                }
            }
        }

        private void OnEnable()
        {
            interactor.hoverEntered.AddListener(OnHoverEnter);
            interactor.hoverExited.AddListener(OnHoverExit);
        }

        private void OnDisable()
        {
            interactor.hoverEntered.RemoveListener(OnHoverEnter);
            interactor.hoverExited.RemoveListener(OnHoverExit);
            if (interactableDict.Count > 0)
            {
                foreach (var pair in interactableDict)
                {
                    // The interactable is currently being selected.
                    if (pair.Value < 0 && !float.IsNegativeInfinity(interactableDict[pair.Key]) && interactor.IsSelecting(pair.Key))
                    {
                        interactor.EndManualInteraction();
                    }
                }
            }
            interactableDict.Clear();
        }

        private void OnHoverEnter(HoverEnterEventArgs eventArgs)
        {
            // if we are processing a StatefulInteractable AND the dwell is enabled for the type of interactor AND the interactable is not being tracked by the dictionary
            if (eventArgs.interactableObject is StatefulInteractable statefulInteractable &&
                (statefulInteractable.UseFarDwell && interactor is IRayInteractor || statefulInteractable.UseGazeDwell && interactor is IGazeInteractor) &&
                !interactableDict.ContainsKey(statefulInteractable))
            {
                interactableDict[statefulInteractable] = interactor is MRTKRayInteractor ? statefulInteractable.FarDwellTime : statefulInteractable.GazeDwellTime;
            }
        }

        private void OnHoverExit(HoverExitEventArgs eventArgs)
        {
            // if we are processing a StatefulInteractable AND the interactable is being tracked by the dictionary
            // AND (we either haven't selected (aka fired the manual interaction) OR the manual interaction has finished)
            if (eventArgs.interactableObject is StatefulInteractable statefulInteractable &&
                interactableDict.TryGetValue(statefulInteractable, out float time) &&
                (time > 0 || float.IsNegativeInfinity(time)))
            {
                interactableDict.Remove(statefulInteractable);
            }
        }
    }
}
