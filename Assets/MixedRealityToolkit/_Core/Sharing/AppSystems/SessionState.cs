using Pixie.Core;
using System;
using UnityEngine;

namespace Pixie.AppSystems
{
    [Serializable]
    public struct SessionState : IItemState, IItemStateComparer<SessionState>
    {
        public SessionState(sbyte sessionID)
        {
            ItemID = sessionID;
            LayoutSceneName = string.Empty;
            SessionStartTime = Mathf.NegativeInfinity;
            CurrentStageNum = 0;
            CurrentStageType = StageProgressionTypeEnum.Manual;
            CurrentStageStartTime = Mathf.NegativeInfinity;
            State = SessionStateEnum.WaitingForApp;
            CurrentStageState = StageStateEnum.NotStarted;
        }

        short IItemState.Key { get { return ItemID; } }

        public short ItemID;
        public string LayoutSceneName;
        public SessionStateEnum State;
        public float SessionStartTime;
        public byte CurrentStageNum;
        public StageStateEnum CurrentStageState;
        public StageProgressionTypeEnum CurrentStageType;
        public float CurrentStageStartTime;

        public bool IsDifferent(SessionState from)
        {
            return ItemID != from.ItemID
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