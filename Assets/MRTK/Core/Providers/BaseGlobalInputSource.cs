// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base class for input sources whose pointers are all active pointers in the scene.
    /// </summary>
    /// <remarks>
    /// <para>This base class is intended to represent input sources which raise events to all active pointers found by the FocusProvider in a scene.</para>
    /// </remarks>
    public class BaseGlobalInputSource : IMixedRealityInputSource, IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BaseGlobalInputSource(string name, IMixedRealityFocusProvider focusProvider, InputSourceType sourceType = InputSourceType.Other)
        {
            SourceId = (CoreServices.InputSystem != null) ? CoreServices.InputSystem.GenerateNewSourceId() : 0;
            SourceName = name;
            FocusProvider = focusProvider;
            SourceType = sourceType;

            UpdateActivePointers();
        }

        /// <inheritdoc />
        public uint SourceId { get; }

        /// <inheritdoc />
        public string SourceName { get; }

        /// <inheritdoc />
        public virtual IMixedRealityPointer[] Pointers { get; set; }

        /// <inheritdoc />
        public InputSourceType SourceType { get; set; }

        private IMixedRealityFocusProvider FocusProvider;

        public void UpdateActivePointers()
        {
            Pointers = FocusProvider.GetPointers<IMixedRealityPointer>().Where(x => x.IsActive).ToArray();
        }

        #region IEquality Implementation

        public static bool Equals(IMixedRealityInputSource left, IMixedRealityInputSource right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealityInputSource)obj);
        }

        private bool Equals(IMixedRealityInputSource other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public virtual void Dispose() { }

        #endregion IEquality Implementation
    }
}
