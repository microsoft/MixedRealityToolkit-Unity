// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public enum MovementConstraintType
    {
        None,
        FixDistanceFromHead,

        /// <summary>
        /// This constraint type indicates that the manipulation handler's
        /// "Translation Axes" preferences will be used to restrict the set of axes
        /// that translation/movement can occur on.
        /// </summary>
        TranslationAxes,
    }
}