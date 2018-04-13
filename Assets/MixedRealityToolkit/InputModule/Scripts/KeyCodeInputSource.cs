// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.InputModule.InputSources;
using MixedRealityToolkit.InputModule.Pointers;
using MixedRealityToolkit.InputModule.Utilities;
using System.Collections;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Keyboard
{
    /// <summary>
    /// KeyCodeInputSource gets all data from <see cref="Input"/> using <see cref="KeyCode"/> values.
    /// <para/>
    /// <remarks><see cref="KeyCode"/> values can come from the Mouse, Keyboard, Joysticks, or other types of hardware.</remarks>
    /// </summary>
    public class KeyCodeInputSource : Singleton<KeyCodeInputSource>, IInputSource
    {
        /// <summary>
        /// Always true initially so we only initialize our interaction sources 
        /// after all <see cref="Singleton{T}"/> Instances have been properly initialized.
        /// </summary>
        private bool delayInitialization = true;

        #region IInputSource Implementation

        public uint SourceId { get; protected set; }

        public string SourceName { get { return "Unity.Input.KeyCode"; } }

        public IPointer[] Pointers { get { return null; } }

        public SupportedInputInfo GetSupportedInputInfo()
        {
            return SupportedInputInfo.None;
        }

        public bool SupportsInputInfo(SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo() & inputInfo) == inputInfo;
        }

        #endregion IInputSource Implementation

        #region IEquality Implementation

        private bool Equals(IInputSource other)
        {
            return base.Equals(other) && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IInputSource)obj);
        }

        public static bool Equals(IInputSource left, IInputSource right)
        {
            return left.SourceId == right.SourceId;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            var left = (IInputSource)x;
            var right = (IInputSource)y;
            if (left != null && right != null)
            {
                return Equals(left, right);
            }

            return false;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        #region Monobehaiour Implementation

        private void OnEnable()
        {
            if (!delayInitialization)
            {
                // The first time we call OnEnable we skip this.
                InputManager.Instance.RaiseSourceDetected(this);
            }
        }

        private void Start()
        {
            InputManager.AssertIsInitialized();

            SourceId = InputManager.GenerateNewSourceId();

            if (delayInitialization)
            {
                delayInitialization = false;
                InputManager.Instance.RaiseSourceDetected(this);
            }
        }

        private void Update()
        {
            for (int i = (int)KeyCode.Backspace; i < (int)KeyCode.Joystick8Button19; i++)
            {
                var keyCode = (KeyCode)i;
                if (Input.GetKeyDown(keyCode))
                {
                    InputManager.Instance.RaiseOnInputDown(this, keyCode);
                }

                if (Input.GetKey(keyCode))
                {
                    InputManager.Instance.RaiseOnInputPressed(this, keyCode);
                }

                if (Input.GetKeyUp(keyCode))
                {
                    InputManager.Instance.RaiseOnInputUp(this, keyCode);
                }
            }
        }

        private void OnDisable()
        {
            InputManager.Instance.RaiseSourceLost(this);
        }

        #endregion Monobehaiour Implementation
    }
}