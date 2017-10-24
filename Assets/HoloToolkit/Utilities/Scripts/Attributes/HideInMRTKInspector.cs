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
    // Hides a field in an MRDL inspector
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HideInMRTKInspector : ShowIfAttribute
    {
        public HideInMRTKInspector() { }

        public override bool ShouldShow(object target)
        {
            return false;
        }
    }
}