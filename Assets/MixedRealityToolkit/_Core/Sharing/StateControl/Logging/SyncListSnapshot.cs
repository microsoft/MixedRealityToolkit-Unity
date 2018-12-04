using System;
using System.Collections.Generic;

namespace Pixie.StateControl.Logging
{
    [Serializable]
    public struct SyncListSnapshot
    {
        public float NetworkTime;
        public string StateType;
        public List<object> ChangedStates;
    }
}