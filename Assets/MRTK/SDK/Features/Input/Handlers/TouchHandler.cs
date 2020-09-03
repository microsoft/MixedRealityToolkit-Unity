// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [AddComponentMenu("Scripts/MRTK/SDK/TouchHandler")]
    public class TouchHandler : MonoBehaviour, IMixedRealityTouchHandler
    {
        #region Event handlers
        public TouchEvent OnTouchStarted = new TouchEvent();
        public TouchEvent OnTouchCompleted = new TouchEvent();
        public TouchEvent OnTouchUpdated = new TouchEvent();
        #endregion


        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            OnTouchCompleted.Invoke(eventData);
        }

        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            OnTouchStarted.Invoke(eventData);
        }

        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            OnTouchUpdated.Invoke(eventData);
        }
    }
}
