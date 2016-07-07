// Copyright Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

public class PlaneTargetGroup : MonoBehaviour
{
    [Tooltip("Enter in same target consecutively to turn on velocity tracking for that target.")]
    public Transform[] Targets;
    
    public bool UseVelocity
    {
        get;
        private set;
    }

    [HideInInspector]
    public Transform CurrentTarget;
    private int currentTargetIndex;

    private void Start()
    {
        currentTargetIndex = 0;
        if (Targets.Length > 0)
        {
            CurrentTarget = Targets[currentTargetIndex];
        }
    }

    // Pick a new target within this group. Targets are cycled through in 
    // the order in which they exist in the Targets property. Velocity can
    // be tracked for targets if they exist in Targets array twice and
    // consecutively
    public void PickNewTarget()
    {
        if (Targets.Length == 0)
        {
            return;
        }

        UseVelocity = false;

        ++currentTargetIndex;
        currentTargetIndex %= Targets.Length;

        // Track velocity for consecutive duplicate entries
        Transform newTarget = Targets[currentTargetIndex];
        if (CurrentTarget == newTarget)
        {
            UseVelocity = true;
        }
        CurrentTarget = newTarget;
    }
}
