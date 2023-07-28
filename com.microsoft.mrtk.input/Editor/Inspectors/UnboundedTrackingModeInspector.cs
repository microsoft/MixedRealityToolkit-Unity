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
        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
#if UNITYXR_MANAGEMENT_PRESENT
            EditorGUILayout.HelpBox("This MRTK component works along side XROrigin to ensure that the XRInputSubsystem " +
                "has the TrackingOriginModeFlags.Unbounded flag set if this flag is supported.\n\nNote, the " +
                "TrackingOriginModeFlags.Unbounded flag is only applied if the XROrigin.RequestedTrackingOriginMode is " +
                "set to \"Not Specified\" and the device supports unbounded spaces.", MessageType.Info);

            if (target is UnboundedTrackingMode unboundedTrackingMode)
            {
                XROrigin xrOrigin = unboundedTrackingMode.GetComponent<XROrigin>();
                if (xrOrigin.RequestedTrackingOriginMode != XROrigin.TrackingOriginMode.NotSpecified)
                {
                    EditorGUILayout.HelpBox("The XROrigin's tracking origin mode is not set to \"Not Specified\". This " +
                        "component will only put the XRInputSubsystem into unbounded mode if XROrigin's tracking mode is set " +
                        "\"Not Specified\"", MessageType.Warning);
                }
            }
#else
            EditorGUILayout.HelpBox("This MRTK component is unable to put the XRInputSubsystem into unbounded mode, as the " +
                "com.unity.xr.management package hasn't been included. Please include com.unity.xr.management version 4.2 " +
                "for this component to function.", MessageType.Warning);
#endif
        }
    }
}
