// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// The output connector for an AudioNode
    /// </summary>
    public class AudioNodeOutput : ScriptableObject
    {
        /// <summary>
        /// Perimeter of the connector object in the graph
        /// </summary>
        public Rect Window = new Rect(0, 0, ConnectorSize, ConnectorSize);
        /// <summary>
        /// The node that this connector is an output for
        /// </summary>
        [SerializeField]
        private AudioNode parentNode = null;

        /// <summary>
        /// EDTIOR: The position of the center of the connector's Rect
        /// </summary>
        public Vector2 Center
        {
            get
            {
                Vector2 tempPos = this.Window.position;
                tempPos.x += ConnectorSize / 2;
                tempPos.y += ConnectorSize / 2;
                return tempPos;
            }
        }

        /// <summary>
        /// The size in pixels for the connector in the graph
        /// </summary>
        private const float ConnectorSize = 20;

        /// <summary>
        /// Public accessor for the node that this connector is an output for
        /// </summary>
        public AudioNode ParentNode
        {
            get { return this.parentNode; }
            set { this.parentNode = value; }
        }
    }
}