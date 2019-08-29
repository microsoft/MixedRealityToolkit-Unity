// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;

public static class ThemeDefinitionCache
{
    private static Dictionary<string, List<Guid>> cache = new Dictionary<string, List<Guid>>();

    public static ThemeDefinition GetEntry(string fileName, int index, Type type)
    {
        return new ThemeDefinition();
    }

    public static void SetEntry(string fileName, int index, Type type, ThemeDefinition definition)
    {
    }

    public static void RemoveEntry(string fileName, int index)
    {

    }
}
