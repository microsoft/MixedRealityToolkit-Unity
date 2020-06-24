// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Tools.ContinuousIntegration
{
    /// <summary>
    /// The base CI validator interface definition.
    /// </summary>
    /// <remarks>
    /// In order to add a new validator to the CI system, implement this interface and add
    /// the new class to ValidatorRunner's validators list.
    /// </remarks>
    public interface IBaseValidator
    {
        /// <summary>
        /// Implementors should return true if no issues/errors are found.
        /// </summary>
        /// <remarks>
        /// Implementors should also log specific errors (using Debug.LogError)
        /// when issues are found so that they are readily diagnosable from logs
        /// </remarks>
        bool Validate();
    }
}