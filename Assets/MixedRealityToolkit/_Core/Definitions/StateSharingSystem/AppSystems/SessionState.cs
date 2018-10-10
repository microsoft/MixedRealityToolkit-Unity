using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems
{
    [Serializable]
    public struct SessionState : IItemState<SessionState>
    {
        public SessionState(sbyte sessionNum)
        {
            ItemNum = sessionNum;
            SessionNum = sessionNum;
            LayoutSceneName = string.Empty;
            SessionStartTime = Mathf.NegativeInfinity;
            CurrentStageNum = 0;
            CurrentStageType = StageProgressionTypeEnum.Manual;
            CurrentStageStartTime = Mathf.NegativeInfinity;
            State = SessionStateEnum.WaitingForUsers;
            CurrentStageState = StageStateEnum.NotStarted;
        }

        sbyte IItemState<SessionState>.Key { get { return ItemNum; } }
        sbyte IItemState<SessionState>.Filter { get { return SessionNum; } }

        public sbyte ItemNum;
        public sbyte SessionNum;
        public string LayoutSceneName;
        public SessionStateEnum State;
        public float SessionStartTime;
        public byte CurrentStageNum;
        public StageStateEnum CurrentStageState;
        public StageProgressionTypeEnum CurrentStageType;
        public float CurrentStageStartTime;

        public bool IsDifferent(SessionState from)
        {
            return ItemNum != from.ItemNum
                || State != from.State
                || LayoutSceneName != from.LayoutSceneName
                || CurrentStageNum != from.CurrentStageNum
                || CurrentStageState != from.CurrentStageState
                || CurrentStageType != from.CurrentStageType
                || CurrentStageStartTime != from.CurrentStageStartTime
                || SessionStartTime != from.SessionStartTime;
        }

        public SessionState Merge(SessionState clientValue, SessionState serverValue)
        {
            return serverValue;
        }

        public override string ToString()
        {
            return StateUtils.StateToString(this);
        }
    }
}