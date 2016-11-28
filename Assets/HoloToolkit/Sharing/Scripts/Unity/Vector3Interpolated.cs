//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using HoloToolkit.Unity;
using UnityEngine;

/// <summary>
/// Class to encapsulate an interpolating Vector3 property.
/// </summary>
public class Vector3Interpolated
{
    /// <summary>
    /// Half-life used to control how fast values are interpolated.
    /// </summary>
    public float HalfLife = 0.08f;

    /// <summary>
    /// Current value of the property.
    /// </summary>
    public Vector3 Value { get; private set; }
    /// <summary>
    /// Target value of the property.
    /// </summary>
    public Vector3 TargetValue { get; private set; }

    public Vector3Interpolated()
    {
        Reset(Vector3.zero);
    }

    public Vector3Interpolated(Vector3 initialValue)
    {
        Reset(initialValue);
    }

    /// <summary>
    /// Resets property to zero interpolation and set value.
    /// </summary>
    /// <param name="value">Desired value to reset</param>
    public void Reset(Vector3 value)
    {
        Value = value;
        TargetValue = value;
    }

    /// <summary>
    /// Set a target for property to interpolate to.
    /// </summary>
    /// <param name="targetValue">Targeted value.</param>
    public void SetTarget(Vector3 targetValue)
    {
        TargetValue = targetValue;
    }

    /// <summary>
    /// Returns whether there are further updates required to get the target value.
    /// </summary>
    /// <returns>True if updates are required. False otherwise.</returns>
    public bool HasUpdate()
    {
        return TargetValue != Value;
    }

    /// <summary>
    /// Performs and gets the updated value.
    /// </summary>
    /// <param name="deltaTime">Tick delta.</param>
    /// <returns>Updated value.</returns>
    public Vector3 GetUpdate(float deltaTime)
    {
        Vector3 distance = (TargetValue - Value);

        if (distance.sqrMagnitude <= Mathf.Epsilon)
        {
            // When close enough, jump to the target
            Value = TargetValue;
        }
        else
        {
            Value = InterpolationUtilities.ExpDecay(Value, TargetValue, HalfLife, deltaTime);
        }


        return Value;
    }
}
