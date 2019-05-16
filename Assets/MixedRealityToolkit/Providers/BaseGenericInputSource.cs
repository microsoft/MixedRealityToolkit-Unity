// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base class for input sources that don't inherit from MonoBehaviour.
    /// </summary>
    /// <remarks>This base class does not support adding or removing pointers, because many will never
    /// pass pointers in their constructors and will fall back to either the Gaze or Mouse Pointer.</remarks>
    public class BaseGenericInputSource : IMixedRealityInputSource, IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pointers"></param>
        public BaseGenericInputSource(string name, IMixedRealityPointer[] pointers = null, InputSourceType sourceType = InputSourceType.Other)
        {
            SourceId = (InputSystem != null) ? InputSystem.GenerateNewSourceId() : 0;
            SourceName = name;
            Pointers = pointers ?? new[] { InputSystem?.GazeProvider.GazePointer };
            SourceType = sourceType;
        }

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        /// <inheritdoc />
        public uint SourceId { get; }

        /// <inheritdoc />
        public string SourceName { get; }

        /// <inheritdoc />
        public virtual IMixedRealityPointer[] Pointers { get; }

        /// <inheritdoc />
        public InputSourceType SourceType { get; set; }

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

        /// <inheritdoc />
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

        /// <inheritdoc />
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
