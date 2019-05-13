// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedCanvasGroup : SynchronizedComponent<CanvasGroupService, SynchronizedCanvasGroup.ChangeType>
    {
        [Flags]
        public enum ChangeType : byte
        {
            None = 0x0,
            Properties = 0x1,
        }

        private CanvasGroup synchronizedCanvasGroup;
        private CanvasGroupProperties previousValues;

        protected override void Awake()
        {
            base.Awake();

            this.synchronizedCanvasGroup = GetComponent<CanvasGroup>();
        }

        public static bool HasFlag(ChangeType changeType, ChangeType flag)
        {
            return (changeType & flag) == flag;
        }

        protected override bool HasChanges(ChangeType changeFlags)
        {
            return changeFlags != ChangeType.None;
        }

        protected override ChangeType CalculateDeltaChanges()
        {
            ChangeType changeType = ChangeType.None;

            CanvasGroupProperties newValues = new CanvasGroupProperties(synchronizedCanvasGroup);
            if (previousValues != newValues)
            {
                changeType |= ChangeType.Properties;
                previousValues = newValues;
            }

            return changeType;
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            SendDeltaChanges(endpoints, ChangeType.Properties);
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, ChangeType changeFlags)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                SynchronizedComponentService.WriteHeader(message, this);

                message.Write((byte)changeFlags);

                if (HasFlag(changeFlags, ChangeType.Properties))
                {
                    message.Write(previousValues.alpha);
                    message.Write(previousValues.ignoreParentGroups);
                }

                message.Flush();
                SynchronizedSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
            }
        }

        private struct CanvasGroupProperties
        {
            public CanvasGroupProperties(CanvasGroup canvasGroup)
            {
                alpha = canvasGroup.alpha;
                ignoreParentGroups = canvasGroup.ignoreParentGroups;
            }

            public float alpha;
            public bool ignoreParentGroups;

            public static bool operator ==(CanvasGroupProperties first, CanvasGroupProperties second)
            {
                return first.Equals(second);
            }

            public static bool operator !=(CanvasGroupProperties first, CanvasGroupProperties second)
            {
                return !first.Equals(second);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is CanvasGroupProperties))
                {
                    return false;
                }

                CanvasGroupProperties other = (CanvasGroupProperties)obj;
                return
                    other.alpha == alpha &&
                    other.ignoreParentGroups == ignoreParentGroups;
            }

            public override int GetHashCode()
            {
                return alpha.GetHashCode();
            }
        }
    }
}
