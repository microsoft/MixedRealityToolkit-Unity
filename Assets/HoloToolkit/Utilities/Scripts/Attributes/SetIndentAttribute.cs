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
    // Sets the indent level for custom formatting
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SetIndentAttribute : Attribute
    {
        public int Indent { get; private set; }

        public SetIndentAttribute(int indent)
        {
            Indent = indent;
        }
    }
}