// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This attaches a Unity Inspector drawer that will enable
    /// selection and instantiation of concrete classes that are assignable to
    /// this field. This pairs best with an interface and the
    /// [SerializeReference] attribute, though technically any parent class
    /// type will work!
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InterfaceSelectorAttribute : PropertyAttribute
    {
        /// <summary>
        /// Should the inspector add an option to set this field to
        /// null, or is null a bad idea for this field? This does _not_ mean
        /// it's impossible for this field to be null if 'false'.
        /// </summary>
        public bool AllowNull { get; private set; }

        /// <summary>
        /// This attaches a Unity Inspector drawer that will enable
        /// selection and instantiation of concrete classes that are assignable
        /// to this field. This pairs best with an interface and the
        /// [SerializeReference] attribute, though technically any parent
        /// class type will work!
        /// </summary>
        /// <param name="allowNull">
        /// Should the inspector add an option to set
        /// this field to null, or is null a bad idea for this field? This does
        /// _not_ mean it's impossible for this field to be null if 'false'.
        /// </param>
        public InterfaceSelectorAttribute(bool allowNull = false)
        {
            AllowNull = allowNull;
        }
    }
}
