using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Patterns.Proximity
{
    public interface ITargetIndicator
    {
        Action OnFeedbackPulse { get; set; }

        bool FeedbackEnabled { get; set; }
        Transform SearchOrigin { get; set; }
        bool HasActiveTarget { get; }
        int MaxActiveTargets { get; set; }
        float MaxActiveDistance { get; set; }
        TargetIndicatorInfo ClosestTarget { get; }
        IEnumerable<TargetIndicatorInfo> ActiveTargets { get; }

        void SetTargets(IEnumerable<Transform> targets);
        void ClearTargets();
    }
}