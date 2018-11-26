// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// An AudioNode for randomly choosing one of its connected nodes
    /// </summary>
    public class RandomSelector : AudioNode
    {
        /// <summary>
        /// Randomly select a connected node
        /// </summary>
        /// <param name="activeEvent">The existing runtime audio event</param>
        public override void ProcessNode(ActiveEvent activeEvent)
        {
            if (this.input.ConnectedNodes == null || this.input.ConnectedNodes.Length == 0)
            {
                Debug.LogWarningFormat("No connected nodes for {0}", this.name);
                return;
            }

            int nodeNum = Random.Range(0, this.input.ConnectedNodes.Length);

            ProcessConnectedNode(nodeNum, activeEvent);
        }

#if UNITY_EDITOR

        /// <summary>
        /// EDITOR: Set the initial values for the node's properties
        /// </summary>
        /// <param name="position">The position of the node on the graph</param>
        public override void InitializeNode(Vector2 position)
        {
            this.name = "Random Selector";
            this.nodeRect.height = 50;
            this.nodeRect.width = 150;
            this.nodeRect.position = position;
            AddInput();
            AddOutput();
        }

#endif
    }
}