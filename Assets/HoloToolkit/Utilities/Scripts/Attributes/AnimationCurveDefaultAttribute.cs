using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
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