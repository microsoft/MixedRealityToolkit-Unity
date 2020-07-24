// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    /// <summary>
    /// Scriptable object that wraps the <see cref="ElasticProperties"/> struct, allowing for easily reusable spring configs.
    /// </summary>
    [CreateAssetMenu(fileName = "ElasticConfiguration", menuName = "Mixed Reality Toolkit/Experimental/Elastic/Elastic Configuration")]
    public class ElasticConfiguration : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Physical properties of the elastic simulation system.")]
        protected ElasticProperties elasticProperties = new ElasticProperties
        {
            // Reasonable default values that should work sufficiently for
            // many simple use cases.
            Mass = 0.02f,
            HandK = 3.0f,
            EndK = 4.0f,
            SnapK = 2.0f,
            Drag = 0.1f
        };

        // <summary>
        // Physical properties of the elastic simulation system.
        // </summary>
        public ElasticProperties ElasticProperties
        {
            get => elasticProperties;
            set => elasticProperties = value;
        }
    }
}

