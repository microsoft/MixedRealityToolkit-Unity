// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Abstract base class for defining file or stream logging structure types.
    /// </summary>
    public abstract class LogStructure : MonoBehaviour
    {
        /// <summary>
        /// Returns the headers of the log structure.
        /// </summary>
        public virtual string[] GetHeaderColumns()
        {
            return Array.Empty<string>();
        }

        /// <summary>
        /// Returns a row of logging data.
        /// </summary>
        public virtual object[] GetData()
        {
            return Array.Empty<object>();
        }
    }
}
