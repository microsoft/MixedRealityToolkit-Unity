// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities
{
    public class PluginHelper
    {
        public static bool ValidateExists(string file)
        {
            if (!File.Exists(file))
            {
                throw new Exception("Plugin file not found: " + file);
            }

            return true;
        }
    }
}
