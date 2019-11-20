// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.MixedReality.Toolkit
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Assembly.GetTypes() can throw in some cases.  This extension will catch that exception and return only the types which were successfully loaded from the assembly.
        /// </summary>
        public static IEnumerable<Type> GetLoadableTypes(this Assembly @this)
        {
            try
            {
                return @this.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
