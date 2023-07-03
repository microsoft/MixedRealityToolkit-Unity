// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extended version of XRBaseInteractable to enable multi-hand interactions.
    /// </summary>
    [AddComponentMenu("MRTK/Core/MRTK Base Interactable")]
    public class MRTKBaseInteractable : XRBaseInteractable
    {
        #region Gaze

        readonly List<IGazeInteractor> hoveringGazeInteractors = new List<IGazeInteractor>();

        /// <summary>
        /// (Read Only) The list of <see cref="IGazeInteractor"/> components currently gazing this object.
        /// </summary>
        public List<IGazeInteractor> HoveringGazeInteractors => hoveringGazeInteractors;

        #endregion Gaze

        #region GazePinch

        readonly List<IGazePinchInteractor> hoveringGazePinchInteractors = new List<IGazePinchInteractor>();

        /// <summary>
        /// (Read Only) The list of <see cref="IGazePinchInteractor"/> components currently hovering this object.
        /// </summary>
        public List<IGazePinchInteractor> HoveringGazePinchInteractors => hoveringGazePinchInteractors;

        readonly List<IGazePinchInteractor> selectingGazePinchInteractors = new List<IGazePinchInteractor>();

        /// <summary>
        /// (Read Only) The list of <see cref="IGazePinchInteractor"/> components currently selecting this object.
        /// </summary>
        public List<IGazePinchInteractor> SelectingGazePinchInteractors => selectingGazePinchInteractors;

        #endregion GazePinch

        #region Poke

        readonly List<IPokeInteractor> hoveringPokeInteractors = new List<IPokeInteractor>();

        /// <summary>
        /// (Read Only) The list of <see cref="IPokeInteractor"/> components currently hovering this object.
        /// </summary>
        public List<IPokeInteractor> HoveringPokeInteractors => hoveringPokeInteractors;

        #endregion Poke

        #region Grab

        readonly List<IGrabInteractor> hoveringGrabInteractors = new List<IGrabInteractor>();

        /// <summary>
        /// (Read Only) The list of <see cref="IGrabInteractor"/> components currently hovering this object.
        /// </summary>]
        public List<IGrabInteractor> HoveringGrabInteractors => hoveringGrabInteractors;

        readonly List<IGrabInteractor> selectingGrabInteractors = new List<IGrabInteractor>();

        /// <summary>
        /// (Read Only) The list of <see cref="IGrabInteractor"/> components currently selecting this object.
        /// </summary>
        public List<IGrabInteractor> SelectingGrabInteractors => selectingGrabInteractors;

        #endregion Grab

        #region Ray

        readonly List<IRayInteractor> hoveringRayInteractors = new List<IRayInteractor>();

        /// <summary>
        /// (Read Only) The list of <see cref="IRayInteractor"/> components currently hovering this object.
        /// </summary>]
        public List<IRayInteractor> HoveringRayInteractors => hoveringRayInteractors;

        #endregion Ray

        #region TimedFlags

        [SerializeField]
        [Tooltip("Is this object selected by a gaze-pinch interactor?")]
        private TimedFlag isGazePinchSelected = new TimedFlag();

        /// <summary>
        /// Is this object selected by a gaze-pinch interactor?
        /// </summary>
        public TimedFlag IsGazePinchSelected { get => isGazePinchSelected; }

        [SerializeField]
        [Tooltip("Is this object selected by a non-gaze ray interactor?")]
        private TimedFlag isRaySelected = new TimedFlag();

        /// <summary>
        /// Is this object selected by a non-gaze ray interactor?
        /// </summary>
        public TimedFlag IsRaySelected { get => isRaySelected; }

        [SerializeField]
        [Tooltip("Is this object selected by a poke interactor?")]
        private TimedFlag isPokeSelected = new TimedFlag();

        /// <summary>
        /// Is this object selected by a poke interactor?
        /// </summary>
        public TimedFlag IsPokeSelected { get => isPokeSelected; }

        [SerializeField]
        [Tooltip("Is this object selected by a grab interactor?")]
        private TimedFlag isGrabSelected = new TimedFlag();

        /// <summary>
        /// Is this object selected by a grab interactor?
        /// </summary>
        public TimedFlag IsGrabSelected { get => isGrabSelected; }

        [SerializeField]
        [Tooltip("Is this object hovered by any gaze interactor?")]
        private TimedFlag isGazeHovered = new TimedFlag();

        /// <summary>
        /// Is this object hovered by any gaze interactor?
        /// </summary>
        public TimedFlag IsGazeHovered { get => isGazeHovered; }

        [SerializeField]
        [Tooltip("Is this object hovered by a gaze-pinch interactor?")]
        private TimedFlag isGazePinchHovered = new TimedFlag();

        /// <summary>
        /// Is this object hovered by a gaze-pinch interactor?
        /// </summary>
        public TimedFlag IsGazePinchHovered { get => isGazePinchHovered; }

        [SerializeField]
        [Tooltip("Is this object hovered by a non-gaze ray interactor?")]
        private TimedFlag isRayHovered = new TimedFlag();

        /// <summary>
        /// Is this object hovered by a non-gaze ray interactor?
        /// </summary>
        public TimedFlag IsRayHovered { get => isRayHovered; }

        [SerializeField]
        [Tooltip("Is this object hovered by a grab interactor?")]
        private TimedFlag isGrabHovered = new TimedFlag();

        /// <summary>
        /// Is this object hovered by a grab interactor?
        /// </summary>
        public TimedFlag IsGrabHovered { get => isGrabHovered; }

        [SerializeField]
        [Tooltip("Is this object hovered by a near touch/poke interactor?")]
        [FormerlySerializedAs("isTouchHovered")]
        private TimedFlag isPokeHovered = new TimedFlag();

        /// <summary>
        /// Is this object hovered by a near touch/poke interactor?
        /// </summary>
        public TimedFlag IsPokeHovered { get => isPokeHovered; }

        /// <summary>
        /// Is this object hovered by any interactor other than passive targeting interactors?
        /// </summary>
        public TimedFlag IsActiveHovered { get => isActiveHovered; }

        [SerializeField]
        [Tooltip("Is this object hovered by any interactor other than only passive targeting interactors?")]
        private TimedFlag isActiveHovered = new TimedFlag();

        #endregion

        #region Interactor Filtering

        // private field to ensure serialization
        // todo: can we rework/get rid of this? in the NEAR FUTURE??
        [SerializeField]
        [Tooltip("Subtractively specifies the set of interactors allowed to select this interactable")]
        [Implements(typeof(IXRInteractor), TypeGrouping.ByNamespaceFlat, AllowAbstract = true)]
        private List<SystemInterfaceType> disabledInteractorTypes = new List<SystemInterfaceType>();

        /// <summary>
        /// Adds the specified type to the set of interactors which cannot select this interactable
        /// </summary>
        public void DisableInteractorType(SystemInterfaceType type)
        {
            if (!disabledInteractorTypes.Contains(type))
            {
                disabledInteractorTypes.Add(type);
            }
        }
        /// <summary>
        /// Removes the specified type to the set of interactors which cannot select this interactable
        /// </summary>
        public void EnableInteractorType(SystemInterfaceType type)
        {
            disabledInteractorTypes.Remove(type);
        }

        /// <summary>
        /// Is the given type of interactor permitted to interact with this interactable?
        /// </summary>
        public bool IsInteractorTypeValid(IXRInteractor interactor)
        {
            // Cache queried interactor type to extract from hot loop.
            Type interactorType = interactor.GetType();

            foreach (SystemInterfaceType disabledType in disabledInteractorTypes)
            {
                if (disabledType.Type.IsAssignableFrom(interactorType))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion Interactor Filtering

        // Refcount-style trackers to avoid LINQs to query
        // selection types.
        private int raySelectCount = 0;
        private int gazePinchSelectCount = 0;
        private int grabSelectCount = 0;
        private int pokeSelectCount = 0;
        private int activeHoverCount = 0;

        private void UpdateHoverFlags()
        {
            IsGazeHovered.Active = hoveringGazeInteractors.Count > 0;
            IsGazePinchHovered.Active = hoveringGazePinchInteractors.Count > 0;
            IsRayHovered.Active = hoveringRayInteractors.Count > 0;
            IsGrabHovered.Active = hoveringGrabInteractors.Count > 0;
            IsPokeHovered.Active = hoveringPokeInteractors.Count > 0;
            IsActiveHovered.Active = activeHoverCount > 0;
        }

        private void UpdateSelectFlags(bool increment, IXRInteractor interactor)
        {
            if (interactor is IRayInteractor) { raySelectCount += increment ? 1 : -1; }
            if (interactor is IGazePinchInteractor) { gazePinchSelectCount += increment ? 1 : -1; }
            if (interactor is IGrabInteractor) { grabSelectCount += increment ? 1 : -1; }
            if (interactor is IPokeInteractor) { pokeSelectCount += increment ? 1 : -1; }

            isRaySelected.Active = raySelectCount > 0;
            isGazePinchSelected.Active = gazePinchSelectCount > 0;
            isGrabSelected.Active = grabSelectCount > 0;
            isPokeSelected.Active = pokeSelectCount > 0;
        }

        #region XRI methods

        /// <inheritdoc />
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return base.IsSelectableBy(interactor) && IsInteractorTypeValid(interactor);
        }

        /// <inheritdoc />
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return base.IsHoverableBy(interactor) && IsInteractorTypeValid(interactor);
        }

        /// <inheritdoc />
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            UpdateSelectFlags(true, args.interactorObject);

            if (args.interactorObject is IGazePinchInteractor gazePinchInteractor)
            {
                selectingGazePinchInteractors.Add(gazePinchInteractor);
            }
        }

        /// <inheritdoc />
        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);

            UpdateSelectFlags(false, args.interactorObject);

            if (args.interactorObject is IGazePinchInteractor gazePinchInteractor)
            {
                selectingGazePinchInteractors.Remove(gazePinchInteractor);
            }
        }

        /// <inheritdoc />
        protected override void OnHoverEntering(HoverEnterEventArgs args)
        {
            base.OnHoverEntering(args);

            if (args.interactorObject is IGazeInteractor gazeInteractor)
            {
                hoveringGazeInteractors.Add(gazeInteractor);
            }
            else // TODO: possibly move to IPassiveInteractor instead of hardcoding on gaze
            {
                activeHoverCount++;
            }

            if (args.interactorObject is IGazePinchInteractor gazePinchInteractor)
            {
                hoveringGazePinchInteractors.Add(gazePinchInteractor);
            }

            if (args.interactorObject is IGrabInteractor grabInteractor)
            {
                hoveringGrabInteractors.Add(grabInteractor);
            }

            if (args.interactorObject is IPokeInteractor pokeInteractor)
            {
                hoveringPokeInteractors.Add(pokeInteractor);
            }

            if (args.interactorObject is IRayInteractor rayInteractor)
            {
                hoveringRayInteractors.Add(rayInteractor);
            }

            UpdateHoverFlags();
        }

        /// <inheritdoc />
        protected override void OnHoverExiting(HoverExitEventArgs args)
        {
            base.OnHoverExiting(args);

            if (args.interactorObject is IGazeInteractor gazeInteractor)
            {
                hoveringGazeInteractors.Remove(gazeInteractor);
            }
            else // TODO: possibly move to IPassiveInteractor instead of hardcoding on gaze
            {
                if (activeHoverCount > 0) { activeHoverCount--; }
            }

            if (args.interactorObject is IGazePinchInteractor gazePinchInteractor)
            {
                hoveringGazePinchInteractors.Remove(gazePinchInteractor);
            }

            if (args.interactorObject is IGrabInteractor grabInteractor)
            {
                hoveringGrabInteractors.Remove(grabInteractor);
            }

            if (args.interactorObject is IPokeInteractor pokeInteractor)
            {
                hoveringPokeInteractors.Remove(pokeInteractor);
            }

            if (args.interactorObject is IRayInteractor rayInteractor)
            {
                hoveringRayInteractors.Remove(rayInteractor);
            }

            UpdateHoverFlags();
        }

        #endregion
    }
}
