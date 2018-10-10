using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Patterns.Targeting
{
    [Serializable]
    public struct TargetState : IItemState<TargetState>
    {
        public TargetState(sbyte sessionNum, sbyte userNum)
        {
            UserNum = userNum;
            SessionNum = sessionNum;
            TargetNum = -1;
            TimeTargetChanged = Mathf.NegativeInfinity;
            TargetType = TargetTypeEnum.None;
        }

        public sbyte Key { get { return UserNum; } }
        public sbyte Filter { get { return SessionNum; } }

        public sbyte UserNum;
        public sbyte SessionNum;
        public sbyte TargetNum;
        public float TimeTargetChanged;
        public TargetTypeEnum TargetType;

        public bool IsDifferent(TargetState from)
        {
            return UserNum != from.UserNum
                || SessionNum != from.SessionNum
                || TargetNum != from.TargetNum
                || TargetType != from.TargetType
                || TimeTargetChanged != from.TimeTargetChanged;
        }

        public TargetState Merge(TargetState clientValue, TargetState serverValue)
        {
            return serverValue;
        }

        public static bool HasTarget(TargetState state)
        {
            return state.TargetType != TargetTypeEnum.None;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }

        public static class Events
        {
            public static bool LostTarget(TargetState oldState, TargetState newState)
            {
                return oldState.TargetType != TargetTypeEnum.None && newState.TargetType == TargetTypeEnum.None;
            }

            public static bool GainedTarget(TargetState oldState, TargetState newState)
            {
                return oldState.TargetType == TargetTypeEnum.None && newState.TargetType != TargetTypeEnum.None;
            }
        }

    }
}