// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Migration handler for migrating buttons with custom icons to the button config helper.
    /// </summary>
    public class ButtonConfigHelperMigrationHandler : IMigrationHandler
    {
        /// <inheritdoc />
        public bool CanMigrate(GameObject gameObject)
        {
            ButtonConfigHelper bch = gameObject.GetComponent<ButtonConfigHelper>();
            return bch != null && bch.EditorCheckForCustomIcon();
        }

        /// <inheritdoc />
        public void Migrate(GameObject gameObject)
        {
            ButtonConfigHelper bch = gameObject.GetComponent<ButtonConfigHelper>();
            bch.EditorUpgradeCustomIcon();
        }
    }
}