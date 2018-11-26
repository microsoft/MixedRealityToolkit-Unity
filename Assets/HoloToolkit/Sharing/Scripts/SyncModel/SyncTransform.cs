// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using HoloToolkit.Unity;

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// This class implements the Transform object primitive for the syncing system.
    /// It does the heavy lifting to make adding new transforms to a class easy.
    /// A transform defines the position, rotation and scale of an object.
    /// </summary>
    public class SyncTransform : SyncObject
    {
        [SyncData] public SyncVector3 Position = new SyncVector3("Position");
        [SyncData] public SyncQuaternion Rotation = new SyncQuaternion("Rotation");
        [SyncData] public SyncVector3 Scale = new SyncVector3("Scale");

        public event Action PositionChanged;
        public event Action RotationChanged;
        public event Action ScaleChanged;

        public SyncTransform(string field)
            : base(field)
        {
            Position.ObjectChanged += OnPositionChanged;
            Rotation.ObjectChanged += OnRotationChanged;
            Scale.ObjectChanged += OnScaleChanged;
        }

        private void OnPositionChanged(SyncObject obj)
        {
            PositionChanged.RaiseEvent();
        }

        private void OnRotationChanged(SyncObject obj)
        {
            RotationChanged.RaiseEvent();
        }

        private void OnScaleChanged(SyncObject obj)
        {
            ScaleChanged.RaiseEvent();
        }
    }
}
