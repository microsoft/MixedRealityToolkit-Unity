// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A SpeechInteractor that is driven by KeywordRecognitionSubsystem.
    /// StatefulInteractables register themselves with this interactor to receive
    /// events when the associated keyword is heard by KeywordRecognitionSubsystem.
    /// </summary>
    /// <remarks>
    /// <para>As XRI does not support selecting more than one interactable at a time,
    /// drop part of the selection lifecycle management provided by XRI and manually tell the interaction manager to enter/exit selection</para>
    /// </remarks>
    [AddComponentMenu("MRTK/Input/Speech Interactor")]
    public class SpeechInteractor : XRBaseInteractor, ISpeechInteractor
    {
        [SerializeField]
        [Tooltip("How long does the interactor remain selecting the interactable after recognizing a voice command?")]
        private float voiceCommandTriggerTime = 0.3f;

        /// <summary>
        /// How long does the interactor remain selecting the interactable after recognizing a voice command?
        /// </summary>
        public float VoiceCommandTriggerTime
        {
            get => voiceCommandTriggerTime;
        }

        private Dictionary<string, List<StatefulInteractable>> keywordDictionary = new Dictionary<string, List<StatefulInteractable>>();
        private List<(StatefulInteractable, float)> selectedInteractables = new List<(StatefulInteractable, float)>();

        /// <summary>
        /// A bool flag to mark whether OnSelectExiting is being triggered by ProcessInteractor of this class.
        /// OnSelectExiting is ignored unless this flag is true.
        /// </summary>
        private bool exitingSelect = false;

        public void RegisterInteractable(StatefulInteractable interactable, string keyword)
        {
            keyword = keyword.ToLower();
            if (keywordDictionary.TryGetValue(keyword, out List<StatefulInteractable> interactableList))
            {
                interactableList.Add(interactable);
            }
            else
            {
                keywordDictionary.Add(keyword, new List<StatefulInteractable> { interactable });
                var subsystem = XRSubsystemHelpers.KeywordRecognitionSubsystem;
                if (subsystem != null)
                {
                    subsystem.CreateOrGetEventForKeyword(keyword).AddListener(() => OnKeywordRecognized(keyword));
                }
                else
                {
                    Debug.LogError("Failed to retrieve a running KeywordRecognitionSubsystem while registering an interactable. " +
                        "Please make sure the subsystem is correctly set up or disable this speech interactor.");
                }
            }
        }

        public void UnregisterInteractable(StatefulInteractable interactable, string keyword)
        {
            keyword = keyword.ToLower();
            if (keywordDictionary.TryGetValue(keyword, out List<StatefulInteractable> interactableList) && interactableList.Remove(interactable))
            {
                return;
            }
            else
            {
                Debug.LogError("The interactable to unregister is not registered with the speech interactor");
            }
        }

        private static readonly ProfilerMarker OnKeywordRecognizedPerfMarker =
            new ProfilerMarker("[MRTK] SpeechInteractor.OnKeywordRecognized");

        public void OnKeywordRecognized(string keyword)
        {
            using (OnKeywordRecognizedPerfMarker.Auto())
            {
                if (keywordDictionary.TryGetValue(keyword, out List<StatefulInteractable> interactableList))
                {
                    if (interactableList.Count > 0 && interactionManager == null)
                    {
                        Debug.LogError("The speech interactor does not have an Interaction Manager.");
                        return;
                    }

                    foreach (var interactable in interactableList)
                    {
                        if (!interactable.VoiceRequiresFocus || interactable.isHovered)
                        {
                            selectedInteractables.Insert(0, (interactable, VoiceCommandTriggerTime));
                            interactionManager.SelectEnter(this, interactable as IXRSelectInteractable);
                        }
                    }
                }
            }
        }

        private static readonly ProfilerMarker ProcessInteractorPerfMarker =
            new ProfilerMarker("[MRTK] SpeechInteractor.ProcessInteractor");

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            using (ProcessInteractorPerfMarker.Auto())
            {
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    for (int i = selectedInteractables.Count - 1; i >= 0; i--)
                    {
                        float selectionTimeRemaining = selectedInteractables[i].Item2 - Time.deltaTime;
                        if (selectionTimeRemaining < 0)
                        {
                            exitingSelect = true;
                            interactionManager.SelectExit(this, selectedInteractables[i].Item1 as IXRSelectInteractable);
                            selectedInteractables.RemoveAt(i);
                        }
                        else
                        {
                            selectedInteractables[i] = (selectedInteractables[i].Item1, selectionTimeRemaining);
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnRegistered(InteractorRegisteredEventArgs args)
        {
            base.OnRegistered(args);
            args.manager.interactableRegistered += OnInteractableRegistered;
            args.manager.interactableUnregistered += OnInteractableUnregistered;

            // Try register all interactables that are registered with the interaction manager
            List<IXRInteractable> interactables = new List<IXRInteractable>();
            interactionManager.GetRegisteredInteractables(interactables);
            foreach (var interactable in interactables)
            {
                if (interactable is StatefulInteractable statefulInteractable && statefulInteractable.AllowSelectByVoice)
                {
                    RegisterInteractable(statefulInteractable, statefulInteractable.SpeechRecognitionKeyword);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnUnregistered(InteractorUnregisteredEventArgs args)
        {
            base.OnUnregistered(args);
            args.manager.interactableRegistered -= OnInteractableRegistered;
            args.manager.interactableUnregistered -= OnInteractableUnregistered;
            keywordDictionary.Clear();
        }

        protected void OnInteractableRegistered(InteractableRegisteredEventArgs args)
        {
            if (args.interactableObject is StatefulInteractable interactable && interactable.AllowSelectByVoice)
            {
                RegisterInteractable(interactable, interactable.SpeechRecognitionKeyword);
            }
        }

        protected void OnInteractableUnregistered(InteractableUnregisteredEventArgs args)
        {
            if (args.interactableObject is StatefulInteractable interactable && interactable.AllowSelectByVoice)
            {
                UnregisterInteractable(interactable, interactable.SpeechRecognitionKeyword);
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// <para>Special workaround: as XRI does not support selecting more than one interactable at a time,
        /// drop part of the selection lifecycle management provided by XRI and manually tell the interaction manager to enter/exit selection</para>
        /// </remarks>
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
        }

        /// <inheritdoc />
        /// <remarks>
        /// <para>Special workaround: as XRI does not support selecting more than one interactable at a time,
        /// drop part of the selection lifecycle management provided by XRI and manually tell the interaction manager to enter/exit selection</para>
        /// <para>When this SpeechInteractor is initiating SelectExit from ProcessInteractor (i.e. exitingSelect is true)
        /// we still call into the base to exit the select.</para>
        /// </remarks>
        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            if (exitingSelect)
            {
                base.OnSelectExiting(args);
                exitingSelect = false;
            }
        }
    }
}
