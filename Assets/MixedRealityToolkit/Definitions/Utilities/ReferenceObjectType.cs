// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public enum ReferenceObjectType
    {
        /// <summary>
        /// Reference an already tracked object
        /// </summary>
        TrackedObject,

        /// <summary>
        /// Reference a body part (head, controller, hand)
        /// </summary>
        BodyPart,

        /// <summary>
        /// Reference an object in the scene
        /// </summary>
        SceneObject
    }
}
