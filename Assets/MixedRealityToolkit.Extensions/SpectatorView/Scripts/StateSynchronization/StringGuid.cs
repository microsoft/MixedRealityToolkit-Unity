// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    internal class StringGuid
    {
        [SerializeField]
        private string m_storage;

        public static implicit operator StringGuid(Guid rhs)
        {
            return new StringGuid { m_storage = rhs.ToString("D") };
        }

        public static implicit operator Guid(StringGuid rhs)
        {
            if (rhs.m_storage == null) return Guid.Empty;
            try
            {
                return new Guid(rhs.m_storage);
            }
            catch (FormatException)
            {
                return System.Guid.Empty;
            }
        }
    }
}