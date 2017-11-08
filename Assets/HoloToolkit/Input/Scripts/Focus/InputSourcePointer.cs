// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Class implementing IPointingSource to demonstrate how to create a pointing source.
    /// This is consumed by SimpleSinglePointerSelector.
    /// </summary>
    public class InputSourcePointer : IPointingSource
    {
        public IInputSource InputSource { get; set; }

        public uint InputSourceId { get; set; }

        public BaseRayStabilizer RayStabilizer { get; set; }

        public bool OwnAllInput { get; set; }
        
        [Obsolete]
        public Ray Ray { get { return Rays[0]; } }

        public RayStep[] Rays {
            get
            {
                return new RayStep(rawRay.origin, rawRay.origin + rawRay.direction);
            }
        }

        public float? ExtentOverride { get; set; }

        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        private Ray rawRay = default(Ray);

        public void UpdatePointer()
        {
            if (InputSource == null)
            {
                rawRay = default(Ray);
            }
            else
            {
                Debug.Assert(InputSource.SupportsInputInfo(InputSourceId, SupportedInputInfo.Pointing));

                InputSource.TryGetPointingRay(InputSourceId, out rawRay);
            }

            if (RayStabilizer != null)
            {
                RayStabilizer.UpdateStability(rawRay.origin, rawRay.direction);
            }
        }

        public bool OwnsInput(BaseEventData eventData)
        {
            return (OwnAllInput || InputIsFromSource(eventData));
        }

        public bool InputIsFromSource(BaseEventData eventData)
        {
            var inputData = (eventData as IInputSourceInfoProvider);

            return (inputData != null)
                && (inputData.InputSource == InputSource)
                && (inputData.SourceId == InputSourceId);
        }
    }
}
