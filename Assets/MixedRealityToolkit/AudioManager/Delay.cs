using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// Adds a time delay in the playback of an event
    /// </summary>
    public class Delay : AudioNode
    {
        /// <summary>
        /// The amount of time in seconds to delay the playback of the audio file
        /// </summary>
        [SerializeField]
        private float delaySeconds = 0;

        /// <summary>
        /// Add the node's delay to the total delay of the event
        /// </summary>
        /// <param name="activeEvent"></param>
        public override void ProcessNode(ActiveEvent activeEvent)
        {
            activeEvent.initialDelay += this.delaySeconds;

            ProcessConnectedNode(0, activeEvent);
        }

#if UNITY_EDITOR

        /// <summary>
        /// Set the initial properties on the node when it is first created
        /// </summary>
        /// <param name="position"></param>
        public override void InitializeNode(Vector2 position)
        {
            this.name = "Delay";
            this.nodeRect.height = 50;
            this.nodeRect.width = 200;
            this.nodeRect.position = position;
            AddInput();
            AddOutput();
        }

        /// <summary>
        /// Draw the float field property for the node in the graph
        /// </summary>
        protected override void DrawProperties()
        {
            this.delaySeconds = EditorGUILayout.FloatField("Seconds", this.delaySeconds);
        }

#endif
    }
}