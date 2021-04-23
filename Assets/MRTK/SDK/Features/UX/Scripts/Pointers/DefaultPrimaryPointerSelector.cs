// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.Profiling;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Default primary pointer selector. The primary pointer is chosen among all interaction enabled ones using the following rules in order:
    ///   1. Currently pressed pointer that has been pressed for the longest
    ///   2. Pointer that was released most recently
    ///   3. Pointer that became interaction enabled most recently
    /// </summary>
    public class DefaultPrimaryPointerSelector : IMixedRealityPrimaryPointerSelector, IMixedRealityPointerHandler
    {
        private readonly Dictionary<IMixedRealityPointer, PointerInfo> pointerInfos = new Dictionary<IMixedRealityPointer, PointerInfo>();

        public void Initialize()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
        }

        public void Destroy()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
        }

        #region IMixedRealityPrimaryPointerSelector

        public void RegisterPointer(IMixedRealityPointer pointer)
        {
            var pointerInfo = new PointerInfo(pointer);
            pointerInfos.Add(pointer, pointerInfo);
        }

        public void UnregisterPointer(IMixedRealityPointer pointer)
        {
            pointerInfos.Remove(pointer);
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] DefaultPrimaryPointerSelector.Update");

        virtual public IMixedRealityPointer Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                IMixedRealityPointer primaryPointer = null;
                PointerInfo primaryInfo = null;

                foreach (var keyValue in pointerInfos)
                {
                    var pointer = keyValue.Key;
                    var info = keyValue.Value;
                    info.Update(pointer);

                    if (info.IsInteractionEnabled &&
                        (primaryInfo == null ||
                        (info.IsPressed && (!primaryInfo.IsPressed || info.PressedTimestamp < primaryInfo.PressedTimestamp)) ||
                        (!primaryInfo.IsPressed && info.ReleasedTimestamp > primaryInfo.ReleasedTimestamp)))
                    {
                        primaryPointer = pointer;
                        primaryInfo = info;
                    }
                }

                return primaryPointer;
            }
        }

        #endregion IMixedRealityPrimaryPointerSelector

        #region IMixedRealityPointerHandler

        private static readonly ProfilerMarker OnPointerDownPerfMarker = new ProfilerMarker("[MRTK] DefaultPrimaryPointerSelector.OnPointerDown");

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            using (OnPointerDownPerfMarker.Auto())
            {
                PointerInfo info = null;
                if (pointerInfos.TryGetValue(eventData.Pointer, out info))
                {
                    info.IsPressed = true;
                }
            }
        }

        private static readonly ProfilerMarker OnPointerUpPerfMarker = new ProfilerMarker("[MRTK] DefaultPrimaryPointerSelector.OnPointerUp");

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            using (OnPointerUpPerfMarker.Auto())
            {
                PointerInfo info = null;
                if (pointerInfos.TryGetValue(eventData.Pointer, out info))
                {
                    info.IsPressed = false;
                }
            }
        }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }

        #endregion IMixedRealityPointerHandler

        private class PointerInfo
        {
            private bool isInteractionEnabled;

            public bool IsInteractionEnabled
            {
                get { return isInteractionEnabled; }
                private set
                {
                    if (value && !isInteractionEnabled)
                    {
                        // We count becoming interaction enabled as a pointer released event
                        ReleasedTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
                    }
                    isInteractionEnabled = value;
                }
            }

            // Last time the pointer was released
            public long ReleasedTimestamp { get; private set; }

            private bool isPressed;

            public bool IsPressed
            {
                get { return isPressed; }
                set
                {
                    if (value && !isPressed)
                    {
                        PressedTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
                    }
                    else if (!value && isPressed)
                    {
                        ReleasedTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
                    }

                    isPressed = value;
                }
            }

            // Last time the pointer was pressed
            public long PressedTimestamp { get; private set; }

            public PointerInfo(IMixedRealityPointer pointer)
            {
                IsInteractionEnabled = pointer.IsInteractionEnabled;
            }

            public void Update(IMixedRealityPointer pointer)
            {
                IsInteractionEnabled = pointer.IsInteractionEnabled;
            }
        }
    }
}