// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    /// <summary>
    /// A custom inspector for the <see cref="UnboundedTrackingMode"/> component.
    /// </summary>
    [CustomEditor(typeof(UnboundedTrackingMode))]
    internal class UnboundedTrackingModeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component works along side XROrigin to ensure that the XRInputSubsystem " +
                "has the TrackingOriginModeFlags.Unbounded flag set on its tracking origin mode if this flag is " +
                "supported. Note, the TrackingOriginModeFlags.Unbounded flag is only applied if the " +
                "XROrigin.RequestedTrackingOriginMode is set to \"Not Specified\" and the device supports unbounded " +
                "spaces.", MessageType.Info);

            if (target is UnboundedTrackingMode unboundedTrackingMode)
            {
                XROrigin xrOrigin = unboundedTrackingMode.GetComponent<XROrigin>();
            }
        }
    }
}
