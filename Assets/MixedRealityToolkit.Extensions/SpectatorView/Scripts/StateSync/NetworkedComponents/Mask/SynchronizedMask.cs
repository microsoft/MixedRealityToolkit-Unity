using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedMask : SynchronizedComponent<MaskService, SynchronizedMask.ChangeType>
    {
        [Flags]
        public enum ChangeType : byte
        {
            None = 0x0,
            Properties = 0x1,
        }

        private Mask synchronizedMask;
        private MaskProperties previousValues;

        protected override void Awake()
        {
            base.Awake();

            this.synchronizedMask = GetComponent<Mask>();
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

            MaskProperties newValues = new MaskProperties(synchronizedMask);
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
                    message.Write(previousValues.enabled);
                }

                message.Flush();
                SynchronizedSceneManager.Instance.Send(endpoints, memoryStream.ToArray());
            }
        }

        private struct MaskProperties
        {
            public MaskProperties(Mask mask)
            {
                enabled = mask.enabled;
            }

            public bool enabled;

            public static bool operator ==(MaskProperties first, MaskProperties second)
            {
                return first.Equals(second);
            }

            public static bool operator !=(MaskProperties first, MaskProperties second)
            {
                return !first.Equals(second);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is MaskProperties))
                {
                    return false;
                }

                MaskProperties other = (MaskProperties)obj;
                return
                    other.enabled == enabled;
            }

            public override int GetHashCode()
            {
                return enabled.GetHashCode();
            }
        }
    }
}
