// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Used to populate service facades with content.
    /// To use, create a class that implements this interface
    /// and mark it with the MixedRealityServiceInspector attribute.
    /// </summary>
    public interface IMixedRealityServiceInspector
    {
        /// <summary>
        /// If true, inspector will include a field for the service's profile at the top (if applicable)
        /// </summary>
        bool DrawProfileField { get; }

        /// <summary>
        /// If true, DrawSceneGUI will be called even when facade object is not selected.
        /// </summary>
        bool AlwaysDrawSceneGUI { get; }

        /// <summary>
        /// Used to draw an inspector for a service facade.
        /// </summary>
        void DrawInspectorGUI(object target);

        /// <summary>
        /// Used to draw handles and visualizations in scene view.
        /// </summary>
        void DrawSceneGUI(object target, SceneView sceneView);

        /// <summary>
        /// Used to draw gizmos in the scene
        /// </summary>
        void DrawGizmos(object target);
    }
}
