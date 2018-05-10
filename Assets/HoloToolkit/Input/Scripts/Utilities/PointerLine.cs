// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.UX;
using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    [RequireComponent(typeof(DistorterGravity))]
    [RequireComponent(typeof(LineBase))]
    [RequireComponent(typeof(LineRendererBase))]
    public class PointerLine : BasePointer, IInputHandler
    {
        [Header("Colors")]
        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Blue, GradientDefaultAttribute.ColorEnum.White, 1f, 0.25f)]
        protected Gradient LineColorSelected;

        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Blue, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient LineColorValid;

        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Gray, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient LineColorNoTarget;

        [Range(5, 100)]
        [SerializeField]
        protected int LineCastResolution = 25;

        [SerializeField]
        protected LineBase LineBase;

        [SerializeField]
        [Tooltip("If no line renderers are specified, this array will be auto-populated on startup.")]
        protected LineRendererBase[] LineRenderers;

        protected DistorterGravity DistorterGravity;

        /// <summary>
        /// True if select is pressed right now
        /// </summary>
        protected bool SelectPressed = false;

        /// <summary>
        /// True if select has been pressed once since startup
        /// </summary>
        protected bool ButtonPressedOnce = false;

        /// <summary>
        /// Line pointer stays inactive until select is pressed for first time
        /// </summary>
        public bool InteractionEnabled
        {
            get
            {
                return ControllerInfo != null;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            LineBase = GetComponent<LineBase>();
            DistorterGravity = GetComponent<DistorterGravity>();
            LineBase.AddDistorter(DistorterGravity);
            if (LineRenderers == null || LineRenderers.Length == 0)
            {
                LineRenderers = LineBase.GetComponentsInChildren<LineRendererBase>();
            }

            LineBase.enabled = false;

            SelectPressed = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SelectPressed = false;
        }

        public void UpdateRenderedLine(RayStep[] lines, PointerResult result)
        {
            if (LineBase == null) { return; }

            Gradient lineColor = LineColorNoTarget;

            if (InteractionEnabled)
            {
                LineBase.enabled = true;

                // If we hit something
                if (result.End.Object != null)
                {
                    lineColor = LineColorValid;
                    LineBase.LastPoint = result.End.Point;
                }
                else
                {
                    LineBase.LastPoint = RayStep.GetPointByDistance(lines, ExtentOverride.Value);
                }

                if (SelectPressed)
                {
                    lineColor = LineColorSelected;
                }
            }
            else
            {
                LineBase.enabled = false;
            }

            for (int i = 0; i < LineRenderers.Length; i++)
            {
                LineRenderers[i].LineColor = lineColor;
            }
        }

        protected override void OnAttachToController()
        {
            // Subscribe to input now that we're parented under the controller
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        protected override void OnDetachFromController()
        {
            // Unsubscribe from input now that we've detached from the controller
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            if (eventData.PressType == InteractionSourcePressInfo.Select)
            {
                SelectPressed = true;
            }
            ButtonPressedOnce = true;
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            if (eventData.PressType == InteractionSourcePressInfo.Select)
            {
                SelectPressed = false;
            }
        }
    }
}
