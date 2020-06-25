// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using System;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public class BaseMixedRealityServiceInspector : IMixedRealityServiceInspector
    {
        public virtual bool DrawProfileField { get { return true; } }

        public virtual bool AlwaysDrawSceneGUI { get { return false; } }

        public virtual void DrawGizmos(object target) { }

        public virtual void DrawInspectorGUI(object target) { }

        public virtual void DrawSceneGUI(object target, SceneView sceneView) { }
    }
}