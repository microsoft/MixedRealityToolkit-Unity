// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An interface indicating that this interactor belongs to an GameObject that is governed by the
    /// interaction mode manager. This GameObject represents the 'controller' that this interactor belongs to.
    /// </summary>
    public interface IModeManagedInteractor
    {
        /// <summary>
        /// Returns the GameObject that this interactor belongs to. This GameObject is governed by the
        /// interaction mode manager and is assigned an interaction mode. This GameObject represents the 'controller' that this interactor belongs to.
        /// </summary>
        public GameObject GetModeManagedController();
    }
}
