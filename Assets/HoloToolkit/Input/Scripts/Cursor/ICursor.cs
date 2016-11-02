//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//
using UnityEngine.EventSystems;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Cursor Interface for handling input events and enable or disable inputs and setting visiblity.
    /// </summary>
    public interface ICursor : IInputHandler, ISourceStateHandler
    {
        void SetVisiblity(bool visible);
        void DisableInput();
        void EnableInput();

        Vector3 GetPosition();
        Quaternion GetRotation();
        Vector3 GetScale();
    }
}
