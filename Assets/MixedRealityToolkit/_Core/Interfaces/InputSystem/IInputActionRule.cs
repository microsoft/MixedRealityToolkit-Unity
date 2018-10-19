// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem
{
    /// <summary>
    /// Interface for defining Input Action Rules
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInputActionRule<T>
    {
        /// <summary>
        /// The Base Action that the rule will listen to.
        /// </summary>
        MixedRealityInputAction BaseAction { get; }

        /// <summary>
        /// The Action to raise if the criteria is met.
        /// </summary>
        MixedRealityInputAction RuleAction { get; }

        /// <summary>
        /// The criteria to check against for determining if the action should be raised.
        /// </summary>
        T Criteria { get; }
    }
}