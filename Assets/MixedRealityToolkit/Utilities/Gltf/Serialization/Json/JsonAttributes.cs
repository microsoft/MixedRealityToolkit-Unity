// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

// Attributes for serializing structs with the JsonBuilder utility.
// Plain structs do not carry enough information to produce valid JSON depending on the schema.
// These optional attributes can be used to guide the JsonBuilder tool.

namespace Microsoft.MixedReality.Toolkit.Utilities.Json
{
    /// <summary>
    /// Base class for all JSON attributes.
    /// </summary>
    public abstract class JsonAttribute : System.Attribute
    {
    }

    /// <summary>
    /// Details of converting enums to JSON values.
    /// </summary>
    [System.Serializable]
    [AttributeUsage(AttributeTargets.All)]
    public class JSONEnumAttribute : JsonAttribute
    {
        private bool useIntValue;
        /// <summary>
        /// Serialize as the integer value of the enum rather than a string.
        /// </summary>
        public bool UseIntValue => useIntValue;

        private object[] ignoreValues;
        /// <summary>
        /// Don't serialize the enum when it has one of the values in this list.
        /// </summary>
        public object[] IgnoreValues => ignoreValues;

        public JSONEnumAttribute(bool useIntValue, object[] ignoreValues = null)
        {
            this.useIntValue = useIntValue;
            this.ignoreValues = ignoreValues;
        }
    }

    /// <summary>
    /// Attribute for JSON integer schema details.
    /// </summary>
    [System.Serializable]
    [AttributeUsage(AttributeTargets.Field)]
    public class JSONIntegerAttribute : JsonAttribute
    {
        private int minimum;
        /// <summary>
        /// Values below the minimum will not be serialized.
        /// </summary>
        public int Minimum => minimum;

        public JSONIntegerAttribute(int minimum)
        {
            this.minimum = minimum;
        }
    }

    /// <summary>
    /// Attribute for JSON array schema details.
    /// </summary>
    [System.Serializable]
    [AttributeUsage(AttributeTargets.Field)]
    public class JSONArrayAttribute : JsonAttribute
    {
        private int minItems;
        /// <summary>
        /// Arrays with fewer items than this will not be serialized.
        /// </summary>
        public int MinItems => minItems;

        public JSONArrayAttribute(int minItems)
        {
            this.minItems = minItems;
        }
    }
}