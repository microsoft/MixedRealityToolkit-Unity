using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateStyleProperty 
{
    string StateName { get; set; }

    string StylePropertyName { get; set; }

    GameObject Target { get; set; }

    void SetKeyFrames(AnimationClip animationClip);
    void RemoveKeyFrames(AnimationClip animationClip);
}
