// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// pointer information for tracking gestures and pokes
    /// Abstacts different input types into a single basic set of data
    /// These values can be accessed from themes or events to
    /// to provide more granular feedback
    /// </summary>
    public struct InteractablePointerTransform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Ray[] Rays;
    }

    public struct InteractablePressData
    {
        public Vector3 StartPosition;
        public Vector3 Position;
        public Vector3 Direction;
        public float Distance;
        public float Percentage;
        public bool HasTouch;
        public float MaxDistance;
        public Vector3 ProjectedDirection;
        public Vector3 PressedValue;
    }

    public class InteractablePointerData
    {
        public IMixedRealityPointer Pointer;
        public HandTrackingInputEventData Hand;
        public InteractablePointerTransform CurrentTransform;
        public InteractablePointerTransform StartTransform;
        public bool HasPress { get; protected set; }
        public bool HasFocus { get; protected set; }
        public int ActionScore { get; protected set; }
        public MixedRealityInputAction Action;

        public void SetFocus(bool hasFocus)
        {
            HasFocus = hasFocus;
        }

        public void SetStartTransfrom(IMixedRealityPointer pointer)
        {
            StartTransform = new InteractablePointerTransform();
            StartTransform.Position = pointer.Position;
            StartTransform.Rotation = pointer.Rotation;
        }

        public void SetStartTransfrom(HandTrackingInputEventData pointer)
        {
            StartTransform = new InteractablePointerTransform();
            StartTransform.Position = pointer.InputData;
            StartTransform.Rotation = Quaternion.identity;

            if (pointer.InputSource.Pointers.Length > 0)
            {
                StartTransform.Rotation = pointer.InputSource.Pointers[0].Rotation;
            }
        }

        public void SetCurrentTransform(IMixedRealityPointer pointer)
        {
            CurrentTransform = new InteractablePointerTransform();
            CurrentTransform.Position = pointer.Position;
            CurrentTransform.Rotation = pointer.Rotation;
            
        }

        public void SetCurrentTransform(HandTrackingInputEventData pointer)
        {
            CurrentTransform = new InteractablePointerTransform();
            CurrentTransform.Position = pointer.InputData;
            CurrentTransform.Rotation = Quaternion.identity;

            if (pointer.InputSource.Pointers.Length > 0)
            {
                CurrentTransform.Rotation = pointer.InputSource.Pointers[0].Rotation;
            }
        }

        public bool SetPress(IMixedRealityPointer pointer, bool hasPress)
        {
            bool stillHasPress = true;
            if (hasPress)
            {
                stillHasPress = UpdateActionScore(pointer);
            }

            HasPress = hasPress;

            return stillHasPress;
        }

        public bool SetPress(HandTrackingInputEventData pointer, bool hasPress)
        {
            HasPress = hasPress;
            return hasPress;
        }

        /// <summary>
        /// Udate the gesture score to see if the action that started the gesture has released
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        protected bool UpdateActionScore(IMixedRealityPointer pointer)
        {
            if (pointer.Controller != null)
            {
                // check to see if pointer button has changed since the gesture started
                // actionScore stores a value representing the interactions mapping data
                // if actionScore reduces below the amount that started the gesture, we assume the gesture should stop.
                MixedRealityInteractionMapping[] mappings = pointer.Controller.Interactions;
                int count = 0;
                for (int j = 0; j < mappings.Length; j++)
                {
                    if(Action == mappings[j].MixedRealityInputAction)
                    {
                        count += Mathf.RoundToInt(mappings[j].FloatData);

                        if (mappings[j].BoolData)
                        {
                            count += 1;
                        } 
                    }
                }

                if (!HasPress)
                {
                    ActionScore = count;
                }
                else if (count < ActionScore)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
