// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.UX;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    [RequireComponent(typeof(DistorterGravity))]
    [RequireComponent(typeof(LineBase))]
    [RequireComponent(typeof(LineRendererBase))]
    public class PointerLine : BaseControllerPointer
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
        /// Line pointer stays inactive until it's attached to a controller.
        /// </summary>
        public bool InteractionEnabled
        {
            get
            {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
                return ControllerInfo != null;
#else
                return false;
#endif
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
        }

        public void UpdateRenderedLine(RayStep[] lines, PointerResult result, bool selectPressed, float extent)
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
                    LineBase.LastPoint = RayStep.GetPointByDistance(lines, extent);
                }

                if (selectPressed)
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
    }
}
