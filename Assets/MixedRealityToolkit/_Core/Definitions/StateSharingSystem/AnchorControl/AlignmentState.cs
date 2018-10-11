using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AnchorControl
{
    [Serializable]
    public struct AlignmentState : IItemState<AlignmentState>
    {
        const float MaxPositionRange = 20;

        public AlignmentState(sbyte sessionNum, sbyte userNum)
        {
            UserNum = userNum;
            SessionNum = sessionNum;
            StateUtils.ShortPosIn(Vector3.zero, out PositionX, out PositionY, out PositionZ, MaxPositionRange);
            StateUtils.ShortRotIn(Vector3.zero, out RotationX, out RotationY, out RotationZ);
        }

        sbyte IItemState<AlignmentState>.Key { get { return UserNum; } }
        sbyte IItemState<AlignmentState>.Filter { get { return SessionNum; } }

        public sbyte UserNum;
        public sbyte SessionNum;

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

        public bool IsDifferent(AlignmentState from)
        {
            return UserNum != from.UserNum
                || SessionNum != from.SessionNum
                || PositionX != from.PositionX
                || PositionY != from.PositionY
                || PositionZ != from.PositionZ
                || RotationX != from.RotationX
                || RotationY != from.RotationY
                || RotationZ != from.RotationZ;
        }

        public AlignmentState Merge(AlignmentState clientValue, AlignmentState serverValue)
        {
            return serverValue;
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