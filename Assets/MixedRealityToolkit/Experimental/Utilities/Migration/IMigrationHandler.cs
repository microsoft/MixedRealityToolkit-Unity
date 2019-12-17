// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    /// <summary>
    /// Interface defining a migration handler, which is used to migrate assets as they
    /// upgrade to new versions of MRTK.
    /// </summary>
    public interface IMigrationHandler
    {
        /// <summary>
        /// Returns true if this migration handler can apply a migration to gameObject
        /// </summary>
        bool CanMigrate(GameObject gameObject);

        /// <summary>
        /// Applies migration to gameObject
        /// </summary>
        void Migrate(GameObject gameObject);
    }
}