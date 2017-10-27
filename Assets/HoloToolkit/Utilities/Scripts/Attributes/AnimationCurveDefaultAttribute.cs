// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    // Adds a 'default' button to an animation curve that will supply default curve values
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class AnimationCurveDefaultAttribute : DrawOverrideAttribute
    {
        public WrapMode PostWrap { get; private set; }
        public Keyframe StartVal { get; private set; }
        public Keyframe EndVal { get; private set; }

        public AnimationCurveDefaultAttribute(Keyframe startVal, Keyframe endVal, WrapMode postWrap = WrapMode.Loop)
        {
            PostWrap = postWrap;
            StartVal = startVal;
            EndVal = endVal;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            throw new NotImplementedException();
        }
#endif

    }
}