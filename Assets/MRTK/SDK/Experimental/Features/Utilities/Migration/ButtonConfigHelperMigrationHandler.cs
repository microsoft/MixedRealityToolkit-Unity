// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    /// <summary>
    /// Migration handler for migrating buttons with custom icons to the button config helper.
    /// </summary>
    public class ButtonConfigHelperMigrationHandler : IMigrationHandler
    {
        public bool CanMigrate(GameObject gameObject)
        {
#if UNITY_EDITOR
            ButtonConfigHelper bch = gameObject.GetComponent<ButtonConfigHelper>();
            return bch != null && bch.EditorCheckForCustomIcon();
#else
            return false;
#endif
        }

        public void Migrate(GameObject gameObject)
        {
            throw new System.NotImplementedException();
        }
    }
}