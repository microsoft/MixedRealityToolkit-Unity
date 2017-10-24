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
    // Class used to send members to bottom of drawing queue
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DrawLastAttribute : Attribute { }
}