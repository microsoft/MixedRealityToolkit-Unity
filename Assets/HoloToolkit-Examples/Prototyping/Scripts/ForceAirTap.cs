// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Fools the input system into thinking an air tap fired.
/// </summary>

namespace HoloToolkit.Examples.Prototyping
{
    public class ForceAirTap : BaseInputSource
    {
        private class EditorHandData
        {
            public EditorHandData(IInputSource inputSource, uint handId)
            {
                HandId = handId;
                HandPosition = Vector3.zero;
                HandDelta = Vector3.zero;
                IsFingerDown = false;
                IsFingerDownPending = false;
                FingerStateChanged = false;
                FingerStateUpdateTimer = -1;
                EditorHandData editorHandData = new EditorHandData(inputSource, handId);
            }

            public readonly uint HandId;
            public Vector3 HandPosition;
            public Vector3 HandDelta;
            public bool IsFingerDown;
            public bool IsFingerDownPending;
            public bool FingerStateChanged;
            public float FingerStateUpdateTimer;
            public readonly EditorHandData editorHandData;
        }

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetOrientation(uint sourceId, out Quaternion orientation)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetPosition(uint sourceId, out Vector3 position)
        {
            throw new NotImplementedException();
        }

        public void FireClickEvent()
        {
            EditorHandData editorHandData = new EditorHandData(this, 1);
            inputManager.RaiseInputClicked(this, editorHandData.HandId, 1);
        }

    }
}
