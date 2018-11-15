using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// An AudioNode containing a reference to a voice-over AudioClip
    /// </summary>
    public class VoiceFile : AudioNode
    {
        /// <summary>
        /// The voice clip to be set on the AudioSource if this node is processed
        /// </summary>
        [SerializeField]
        private AudioClip file = null;

        /// <summary>
        /// The amount of volume change to apply if this node is processed
        /// </summary>
        [SerializeField, Range(-1, 1)]
        private float volumeOffset = 0;

        /// <summary>
        /// The amount of pitch change to apply if this node is processed
        /// </summary>
        [SerializeField, Range(-3, 3)]
        private float pitchOffset = 0;

        /// <summary>
        /// The language the voice file is spoken in
        /// </summary>
        [SerializeField]
        private AudioLanguage language = AudioLanguage.English;

        /// <summary>
        /// The text associated with this voice clip, usually for subtitles
        /// </summary>
        [SerializeField]
        private string text = null;

        /// <summary>
        /// Accessor for the AudioLanguage this voice clip is set to
        /// </summary>
        public AudioLanguage Language
        {
            get { return this.language; }
        }

        /// <summary>
        /// Apply all modifications to the ActiveEvent before it gets played
        /// </summary>
        /// <param name="activeEvent">The runtime event being prepared for playback</param>
        public override void ProcessNode(ActiveEvent activeEvent)
        {
            if (file == null)
            {
                Debug.LogWarningFormat("Empty Voice File node in event {0}", activeEvent.rootEvent.name);
            }

            activeEvent.ModulateVolume(this.volumeOffset);
            activeEvent.ModulatePitch(this.pitchOffset);
            activeEvent.clip = this.file;
            activeEvent.text = this.text;
        }

#if UNITY_EDITOR

        /// <summary>
        /// The width in pixels for the node's window in the graph
        /// </summary>
        private const float NodeWidth = 300;
        /// <summary>
        /// The height in pixels for the node's window in the graph
        /// </summary>
        private const float NodeHeight = 130;

        /// <summary>
        /// Public accessor for the voice clip in the node
        /// </summary>
        public AudioClip File
        {
            get { return this.file; }
        }

        /// <summary>
        /// EDITOR: Initialize the node's properties when it is first created
        /// </summary>
        /// <param name="position">The position of the new node in the graph</param>
        public override void InitializeNode(Vector2 position)
        {
            this.name = "Voice File";
            this.nodeRect.position = position;
            this.nodeRect.width = NodeWidth;
            this.nodeRect.height = NodeHeight;
            AddOutput();
            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// EDITOR: Display the node's properties in the graph
        /// </summary>
        protected override void DrawProperties()
        {
            this.file = EditorGUILayout.ObjectField(this.file, typeof(AudioClip), false) as AudioClip;
            this.volumeOffset = EditorGUILayout.Slider("Volume Offset", this.volumeOffset, -1, 1);
            this.pitchOffset = EditorGUILayout.Slider("Pitch Offset", this.pitchOffset, -3, 3);
            this.text = EditorGUILayout.TextField("Text", this.text);
            this.language = (AudioLanguage)EditorGUILayout.EnumPopup(this.language);
        }
#endif
    }
}