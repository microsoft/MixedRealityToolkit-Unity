﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class DefaultPrimaryPointerSelector : IMixedRealityPrimaryPointerSelector
    {
        private readonly Dictionary<IMixedRealityPointer, PointerInfo> pointerInfos = new Dictionary<IMixedRealityPointer, PointerInfo>();

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
                    (info.IsFocusLocked && (!primaryInfo.IsFocusLocked || info.FocusLockedTimestamp < primaryInfo.FocusLockedTimestamp)) ||
                    (!primaryInfo.IsFocusLocked && info.InteractionEnabledTimestamp > primaryInfo.InteractionEnabledTimestamp)))
                {
                    primaryPointer = pointer;
                    primaryInfo = info;
                }
            }

            return primaryPointer;
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

            public long InteractionEnabledTimestamp { get; private set; }

            private bool isFocusLocked;

            public bool IsFocusLocked
            {
                get { return isFocusLocked; }
                private set
                {
                    if (value && !isFocusLocked)
                    {
                        FocusLockedTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
                    }
                    else if (!value && isFocusLocked)
                    {
                        InteractionEnabledTimestamp = System.Diagnostics.Stopwatch.GetTimestamp();
                    }

                    isFocusLocked = value;
                }
            }

            public long FocusLockedTimestamp { get; private set; }

            public PointerInfo(IMixedRealityPointer pointer)
            {
                IsInteractionEnabled = pointer.IsInteractionEnabled;
                IsFocusLocked = pointer.IsFocusLocked;
            }

            public void Update(IMixedRealityPointer pointer)
            {
                IsInteractionEnabled = pointer.IsInteractionEnabled;
                IsFocusLocked = pointer.IsFocusLocked;
            }
        }
    }
}