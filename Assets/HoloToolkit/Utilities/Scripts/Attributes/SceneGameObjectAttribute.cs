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
    // Displays a drop-down menu of GameObjects that are limited to the target object
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SceneGameObjectAttribute : DrawOverrideAttribute
    {
        public string CustomLabel { get; private set; }

        public SceneGameObjectAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
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