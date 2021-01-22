// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    public interface IStateAnimatableProperty
    {
        string StateName { get; set; }

        string AnimatablePropertyName { get; set; }

        GameObject Target { get; set; }

        void SetKeyFrames(AnimationClip animationClip);
        void RemoveKeyFrames(AnimationClip animationClip);
    }
}
