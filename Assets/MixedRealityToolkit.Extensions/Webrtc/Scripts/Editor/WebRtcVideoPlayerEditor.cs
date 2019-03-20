using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Webrtc.Editor
{
#if UNITY_EDITOR
    /// <summary>
    /// A lightweight custom editor to include stat data for <see cref="WebRtcVideoPlayer"/>
    /// in the inspector
    /// </summary>
    [CustomEditor(typeof(WebRtcVideoPlayer))]
    public class WebRtcVideoPlayerEditor : UnityEditor.Editor
    {
        private bool needsConstantRepaint = false;

        public override bool RequiresConstantRepaint()
        {
            return needsConstantRepaint;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var src = target as WebRtcVideoPlayer;

            string loadFps;
            string presentFps;
            string skipFps;

            if (src.FrameQueue == null)
            {
                var unk = "Unknown";
                loadFps = unk;
                presentFps = unk;
                skipFps = unk;

                GUI.enabled = false;
                needsConstantRepaint = false;
            }
            else
            {
                loadFps = src.FrameQueue.FrameLoad.Value.ToString();
                presentFps = src.FrameQueue.FramePresent.Value.ToString();
                skipFps = src.FrameQueue.FrameSkip.Value.ToString();

                GUI.enabled = true;
                needsConstantRepaint = true;
            }

            EditorGUILayout.LabelField("Load FPS", loadFps);
            EditorGUILayout.LabelField("Present FPS", presentFps);
            EditorGUILayout.LabelField("Skip FPS", skipFps);
        }
    }
#endif
}
