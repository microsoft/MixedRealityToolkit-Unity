// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines extension methods for the <see cref="Assembly"/> type.
    /// </summary>
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
