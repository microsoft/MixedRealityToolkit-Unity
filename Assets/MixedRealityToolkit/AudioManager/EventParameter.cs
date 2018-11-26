// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// An AudioParameter with a response curve and audio property to apply changes to
    /// </summary>
    [System.Serializable]
    public class EventParameter
    {
        /// <summary>
        /// The root parameter that the event is using
        /// </summary>
        public AudioParameter parameter = null;
        /// <summary>
        /// The curve to evaluate the parameter's value on
        /// </summary>
        public AnimationCurve responseCurve = new AnimationCurve();
        /// <summary>
        /// Which audio property the parameter affects
        /// </summary>
        public ParameterType paramType;

        /// <summary>
        /// The current value of the parameter before being evaluated by the response curve
        /// </summary>
        public float CurrentValue { get; private set; }
        /// <summary>
        /// The resulting value from evaluating the CurrentValue on the response curve
        /// </summary>
        public float CurrentResult { get; private set; }

        /// <summary>
        /// Reset the parameter to sync it back with the root parameter
        /// </summary>
        public void SyncParameter()
        {
            this.CurrentValue = this.parameter.CurrentValue;
            ProcessParameter();
        }

        /// <summary>
        /// Evaluate the result of the current value on the response curve
        /// </summary>
        public void ProcessParameter()
        {
            this.CurrentResult = this.responseCurve.Evaluate(this.parameter.CurrentValue);
        }

        /// <summary>
        /// Evaluate a custom value on the parameter's response curve
        /// </summary>
        /// <param name="newValue">The custom value to evaluate</param>
        /// <returns>The result of the newValue on the parameter's response curve</returns>
        public float ProcessParameter(float newValue)
        {
            return this.responseCurve.Evaluate(newValue);
        }
    }

    /// <summary>
    /// The audio properties that a parameter can affect
    /// </summary>
    public enum ParameterType
    {
        Volume,
        Pitch
    }
}