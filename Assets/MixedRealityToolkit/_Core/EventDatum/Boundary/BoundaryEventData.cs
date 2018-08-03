// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Boundary
{
    /// <summary>
    /// Describes a boundary event.
    /// </summary>
    public class BoundaryEventData : GenericBaseEventData
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public BoundaryEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }
    }
}
