using Pixie.Core;
using System;
using UnityEngine;

namespace Pixie.AnchorControl
{
    [Serializable]
    [AppStateType]
    public struct UserAnchorState : IItemState, IItemStateComparer<UserAnchorState>
    {
        const float MaxPositionRange = 20;

        public enum StateEnum : byte
        {
            Unknown,
            Known,
        }

        public UserAnchorState(short stateKey)
        {
            StateKey = stateKey;
            UserNum = -1;
            TargetNum = -1;
            State = StateEnum.Unknown;
            StateUtils.ShortPosIn(Vector3.zero, out PositionX, out PositionY, out PositionZ, MaxPositionRange);
            StateUtils.ShortRotIn(Vector3.zero, out RotationX, out RotationY, out RotationZ);
        }

        public bool IsDifferent(UserAnchorState from)
        {
            return StateKey != from.StateKey
                 || State != from.State
                 || UserNum != from.UserNum
                 || TargetNum != from.TargetNum
                 || PositionX != from.PositionX
                 || PositionY != from.PositionY
                 || PositionZ != from.PositionZ
                 || RotationX != from.RotationX
                 || RotationY != from.RotationY
                 || RotationZ != from.RotationZ;
        }

        public UserAnchorState Merge(UserAnchorState clientValue, UserAnchorState serverValue)
        {
            return serverValue;
        }

        short IItemState.Key { get { return StateKey; } }

        public short StateKey;
        public short TargetNum;
        public short UserNum;
        public StateEnum State;
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