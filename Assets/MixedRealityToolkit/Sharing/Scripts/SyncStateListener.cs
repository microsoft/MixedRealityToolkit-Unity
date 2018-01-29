// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.Sharing
{
    /// <summary>
    /// C# wrapper for the Sharing SyncListener, making changes available through the Action class.
    /// </summary>
    public class SyncStateListener : SyncListener
    {
        /// <summary>
        /// Event sent when
        /// </summary>
        public event Action SyncChangesBeginEvent;
        public event Action SyncChangesEndEvent;

        public override void OnSyncChangesBegin()
        {
            if (SyncChangesBeginEvent != null)
            {
                SyncChangesBeginEvent();
            }
        }

        public override void OnSyncChangesEnd()
        {
            if (SyncChangesEndEvent != null)
            {
                SyncChangesEndEvent();
            }
        }
    }
}
