// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;

namespace Miicrosoft.MixedReality.Toolkit.SpatialObjectMeshObserver.RoomFile
{
    public class RoomObject : ScriptableObject
    {
        // todo: names?
        [SerializeField]
        private GameObject model;

        public GameObject Model
        {
            get => model;
            internal set => model = value;
        }
    }
}