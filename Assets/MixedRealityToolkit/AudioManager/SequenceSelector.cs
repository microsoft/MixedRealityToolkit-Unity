using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// Iterates over each of its connections when the event is played
    /// </summary>
    public class SequenceSelector : AudioNode
    {
        /// <summary>
        /// The index of the connected node that will be chosen next time the event is played
        /// </summary>
        private int currentNode = 0;

        /// <summary>
        /// Process the node on the current index and increase the index for next playback
        /// </summary>
        /// <param name="activeEvent">The existing runtime event to set properties on</param>
        public override void ProcessNode(ActiveEvent activeEvent)
        {
            ProcessConnectedNode(this.currentNode, activeEvent);

            this.currentNode++;

            if (this.currentNode >= this.input.ConnectedNodes.Length)
            {
                this.currentNode = 0;
            }
        }

        /// <summary>
        /// Reset the current node index back to 0
        /// </summary>
        public override void Reset()
        {
            this.currentNode = 0;
        }

#if UNITY_EDITOR

        /// <summary>
        /// EDITOR: Set the initial values for the node's properties
        /// </summary>
        /// <param name="position">The position of the node's window in the graph</param>
        public override void InitializeNode(Vector2 position)
        {
            this.name = "Sequence Selector";
            this.nodeRect.height = 50;
            this.nodeRect.width = 150;
            this.nodeRect.position = position;
            AddInput();
            AddOutput();
        }

#endif
    }
}