// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Manages InteractablePointerData for Interactive
    /// Converts HandTrackingInputEventData, input data and pointer data to InteractablePointerData
    /// </summary>
    public class InteractablePointerDataManager
    {
        protected Dictionary<uint, InteractablePointerData> pointerData = new Dictionary<uint, InteractablePointerData>();

        /// <summary>
        /// Get the current list of pointer data
        /// </summary>
        /// <returns></returns>
        public Dictionary<uint, InteractablePointerData> PointerData => pointerData;

        /// <summary>
        /// add pointerData, interaction started - focus
        /// </summary>
        /// <param name="pointer"></param>
        public void AddPointerData(IMixedRealityPointer pointer)
        {
            if (!pointerData.ContainsKey(pointer.PointerId))
            {
                InteractablePointerData data = new InteractablePointerData() { Pointer = pointer };
                data.SetFocus(true);
                data.SetStartTransfrom(pointer);
                pointerData.Add(pointer.PointerId, data);
            }
        }

        public static InteractablePressData GetPressData(Interactable source, InteractablePressData data)
        {
            Dictionary<uint, InteractablePointerData> pointers = source.PointerManager.PointerData;
            Vector3 position = Vector3.zero;

            foreach (InteractablePointerData pointer in pointers.Values)
            {
                if (pointer.Hand != null)
                {
                    IMixedRealityHand hand = (IMixedRealityHand)pointer.Hand.Controller;
                    MixedRealityPose pose = new MixedRealityPose();
                    Debug.Log(hand);
                    if (hand != null && hand.TryGetJoint(TrackedHandJoint.IndexTip, out pose))
                    {
                        position = pose.Position;
                    }
                }
            }

            if (!data.HasPress)
            {
                // starting
                data.StartPosition = position;
            }

            data.Direction = position - data.StartPosition;
            float distance = Vector3.Dot(data.Direction, data.ProjectedDirection.normalized);
            distance = Mathf.Clamp(distance, 0, data.MaxDistance);
            data.Distance = distance;
            data.Percentage = distance / data.MaxDistance;
            return data;
        }

        public static InteractablePressData GetPointerPressData(Interactable source, InteractablePressData data)
        {
            Dictionary<uint, InteractablePointerData> pointers = source.PointerManager.PointerData;
            Vector3 position = Vector3.zero;

            foreach (InteractablePointerData pointer in pointers.Values)
            {
                position = pointer.Pointer.Position;
            }

            if (!data.HasPress)
            {
                // starting
                data.StartPosition = position;
            }

            data.Direction = position - data.StartPosition;
            float distance = Vector3.Dot(data.Direction, data.ProjectedDirection.normalized);
            distance = Mathf.Clamp(distance, 0, data.MaxDistance);
            data.Distance = distance;
            data.Percentage = distance / data.MaxDistance;
            return data;
        }

        /// <summary>
        /// add pointerData to handData, lost focus
        /// </summary>
        /// <param name="pointer"></param>
        public void AddPointerData(HandTrackingInputEventData pointer)
        {
            if (!pointerData.ContainsKey(pointer.SourceId))
            {
                InteractablePointerData data = new InteractablePointerData() { Hand = pointer };
                data.SetFocus(true);
                data.SetStartTransfrom(pointer);
                pointerData.Add(pointer.SourceId, data);
            }
        }

        /// <summary>
        /// Removes pointerData, lost focus
        /// </summary>
        /// <param name="pointer"></param>
        public void RemovePointerData(IMixedRealityPointer pointer)
        {
            if (pointerData.ContainsKey(pointer.PointerId))
            {
                pointerData.Remove(pointer.PointerId);
            }
        }

        /// <summary>
        /// Removes pointerData from handData, lost focus
        /// </summary>
        /// <param name="pointer"></param>
        public void RemovePointerData(HandTrackingInputEventData pointer)
        {
            if (pointerData.ContainsKey(pointer.SourceId))
            {
                pointerData.Remove(pointer.SourceId);
            }
        }

        /// <summary>
        /// Removes pointerData by sourceId
        /// </summary>
        /// <param name="pointer"></param>
        public void RemovePointerDataById(uint id)
        {
            if (pointerData.ContainsKey(id))
            {
                pointerData.Remove(id);
            }
        }

        public void ClearPointerData()
        {
            pointerData = new Dictionary<uint, InteractablePointerData>();
        }

        /// <summary>
        /// Update IMixedRealityPointerPositions CurrentTransform data
        /// </summary>
        public void UpdatePointerPositions()
        {
            List<uint> cleanUpIds = new List<uint>();
            foreach (InteractablePointerData data in pointerData.Values)
            {
                if (data.Pointer != null)
                {
                    if(data.Pointer.ToString() != "null")
                    {
                        data.SetCurrentTransform(data.Pointer);
                    }
                    else
                    {
                        cleanUpIds.Add(data.Pointer.PointerId);
                    }
                    
                }
            }

            for (int i = 0; i < cleanUpIds.Count; i++)
            {
                pointerData.Remove(cleanUpIds[i]);
            }
        }

        /// <summary>
        /// Update pointerData, a way to check if the pointer has changed during a gesture
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="hasPress"></param>
        public bool UpdatePointerData(IMixedRealityPointer pointer, MixedRealityInputAction action, bool hasPress)
        {
            InteractablePointerData data;
            pointerData.TryGetValue(pointer.PointerId, out data);
            
            bool stillActive = true;
            if (data != null)
            {
                data.Action = action;
                data.SetCurrentTransform(pointer);

                if (hasPress != data.HasPress)
                {
                    stillActive = data.SetPress(pointer, hasPress);

                    if (!hasPress && !data.HasFocus)
                    {
                        stillActive = false;
                    }
                }
            }
            else
            {
                stillActive = false;
            }

            return stillActive;
        }

        /// <summary>
        /// update pointerData in handData
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="hasPress"></param>
        public bool UpdatePointerData(HandTrackingInputEventData pointer, MixedRealityInputAction action, bool hasPress)
        {
            InteractablePointerData data;
            pointerData.TryGetValue(pointer.SourceId, out data);
            
            bool stillActive = true;
            if (data != null)
            {
                data.Action = action;
                data.SetCurrentTransform(pointer);

                if (hasPress != data.HasPress)
                {
                    stillActive = data.SetPress(pointer, hasPress);
                    if (!hasPress && !data.HasFocus)
                    {
                        stillActive = false;
                    }
                }
            }
            else
            {
                stillActive = false;
            }

            return stillActive;
        }

        /// <summary>
        /// Update pointer focus
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="hasFocus"></param>
        public void UpdatePointerFocus(IMixedRealityPointer pointer, bool hasFocus)
        {
            InteractablePointerData data;
            pointerData.TryGetValue(pointer.PointerId, out data);
            if (data != null)
            {
                data.SetFocus(hasFocus);
            }
        }

        /// <summary>
        /// Update pointer focus
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="hasFocus"></param>
        public void UpdatePointerFocus(HandTrackingInputEventData pointer, bool hasFocus)
        {
            InteractablePointerData data;
            pointerData.TryGetValue(pointer.SourceId, out data);
            if (data != null)
            {
                data.SetFocus(hasFocus);
            }
        }

    }
}
