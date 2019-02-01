// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem
{
    public interface IMixedRealitySpatialAwarenessObject
    {
        //todo
        /// <summary>
        /// 
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        GameObject GameObject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        MeshRenderer Renderer { get; set; }

        ///<summary>
        ///
        /// </summary>
        void CleanObject();


    }
}