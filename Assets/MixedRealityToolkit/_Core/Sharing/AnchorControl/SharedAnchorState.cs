using Pixie.Core;
using System;
using UnityEngine;

namespace Pixie.AnchorControl
{
    [Serializable]
    [AppStateType]
    public struct SharedAnchorState : IItemState, IItemStateComparer<SharedAnchorState>
    {
        const float MaxPositionRange = 20;

        public enum StateEnum : byte
        {
            Unknown,
            Known,
        }

        public SharedAnchorState(short stateKey)
        {
            StateKey = stateKey;
            AnchorID = string.Empty;
            StateUtils.ShortPosIn(Vector3.zero, out PositionX, out PositionY, out PositionZ, MaxPositionRange);
            StateUtils.ShortRotIn(Vector3.zero, out RotationX, out RotationY, out RotationZ);
        }

        public bool IsDifferent(SharedAnchorState from)
        {
            return StateKey != from.StateKey
                || AnchorID != from.AnchorID
                || PositionX != from.PositionX
                || PositionY != from.PositionY
                || PositionZ != from.PositionZ
                || RotationX != from.RotationX
                || RotationY != from.RotationY
                || RotationZ != from.RotationZ;
        }

        public SharedAnchorState Merge(SharedAnchorState clientValue, SharedAnchorState serverValue)
        {
            return serverValue;
        }

        short IItemState.Key { get { return StateKey; } }

        public short StateKey;
        public string AnchorID;
        public Vector3 Position
        {
            get { return StateUtils.ShortPosOut(PositionX, PositionY, PositionZ, MaxPositionRange); }
            set { StateUtils.ShortPosIn(value, out PositionX, out PositionY, out PositionZ, MaxPositionRange); }
        }
        public Vector3 Rotation
        {
            get { return StateUtils.ShortRotOut(RotationX, RotationY, RotationZ); }
            set { StateUtils.ShortRotIn(value, out RotationX, out RotationY, out RotationZ); }
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }

        #region raw values
        public short PositionX;
        public short PositionY;
        public short PositionZ;

        public short RotationX;
        public short RotationY;
        public short RotationZ;
        #endregion
    }
}