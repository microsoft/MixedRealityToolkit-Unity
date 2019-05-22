// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Default primary pointer selector. The primary pointer is chosen among all interaction enabled ones using the following rules in order:
    ///   1. Focus locked pointer that has been locked for the longest
    ///   2. Pointer that was focus locked most recently
    ///   3. Pointer that became interaction enabled most recently
    /// </summary>
    public class DefaultPrimaryPointerSelector : IMixedRealityPrimaryPointerSelector
    {
        private readonly Dictionary<IMixedRealityPointer, PointerInfo> pointerInfos = new Dictionary<IMixedRealityPointer, PointerInfo>();

        public DefaultPrimaryPointerSelector()
        {
            MixedRealityToolkit.InputSystem.PointerEvent += OnPointerEvent;
        }

        ~DefaultPrimaryPointerSelector()
        {
            MixedRealityToolkit.InputSystem.PointerEvent -= OnPointerEvent;
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

        public IMixedRealityPointer Update()
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
                    (info.IsDown && (!primaryInfo.IsDown || info.FocusLockedTimestamp < primaryInfo.FocusLockedTimestamp)) ||
                    (!primaryInfo.IsDown && info.InteractionEnabledTimestamp > primaryInfo.InteractionEnabledTimestamp)))
                {
                    primaryPointer = pointer;
                    primaryInfo = info;
                }
            }

            return primaryPointer;
        }

        #endregion IMixedRealityPrimaryPointerSelector

        public void OnPointerEvent(MixedRealityPointerEventData eventData, PointerEventType eventType)
        {
            if (eventType == PointerEventType.Down || eventType == PointerEventType.Up)
            {
                PointerInfo info = null;
                if (pointerInfos.TryGetValue(eventData.Pointer, out info))
                {
                    info.IsDown = eventType == PointerEventType.Down;
                }
            }
        }

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
                        InteractionEnabledTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
                    }
                    isInteractionEnabled = value;
                }
            }

            // This doubles as interaction enabled and focus locked lost timestamp. See IsFocusLocked setter below.
            public long InteractionEnabledTimestamp { get; private set; }

            private bool isDown;

            public bool IsDown
            {
                get { return isDown; }
                set
                {
                    if (value && !isDown)
                    {
                        FocusLockedTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
                    }
                    else if (!value && isDown)
                    {
                        // We take shortcut here and refresh the interaction enabled timestamp instead of keeping a separate focus locked lost one
                        InteractionEnabledTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
                    }

                    isDown = value;
                }
            }

            public long FocusLockedTimestamp { get; private set; }

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