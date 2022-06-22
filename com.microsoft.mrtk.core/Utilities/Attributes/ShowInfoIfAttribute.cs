// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Conditionally draws an info box
    /// </summary>
    /// <remarks>
    /// Based on https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ShowInfoIfAttribute : PropertyAttribute
    {
        /// <summary>
        /// Types of comperisons.
        /// </summary>
        public enum ComparisonType
        {
            Equal = 1,
            NotEqual = 2,
        }

        /// <summary>
        /// The type of info to show if the property is true
        /// </summary>
        public MessageType MessageType;

        /// <summary>
        /// The message to show if the property is true
        /// </summary>
        public string Message;

        /// <summary>
        /// The serialized name of the property to compare.
        /// </summary>
        public string ComparedPropertyName { get; private set; }

        /// <summary>
        /// The value to compare against.
        /// </summary>
        public object CompareAgainst { get; private set; }

        /// <summary>
        /// The comparison to perform.
        /// </summary>
        public ComparisonType ComparisonMode { get; private set; }

        /// <summary>
        /// Only draws the field only if the provided property's boolValue matches the compared value.
        /// </summary>
        /// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
        /// <param name="compareAgainst">The value the property is being compared to.</param>
        /// <param name="comparisonMode">Whether to check equality or inequality.</param>
        public ShowInfoIfAttribute(MessageType messageType, string message, string comparedPropertyName, object compareAgainst = null, ComparisonType comparisonMode = ComparisonType.Equal)
        {
            // Because we can't use default parameters for reference types other than string.
            if (compareAgainst == null)
            {
                compareAgainst = true;
            }

            MessageType = messageType;
            Message = message;
            ComparedPropertyName = comparedPropertyName;
            CompareAgainst = compareAgainst;
            ComparisonMode = comparisonMode;
        }
    }
}
