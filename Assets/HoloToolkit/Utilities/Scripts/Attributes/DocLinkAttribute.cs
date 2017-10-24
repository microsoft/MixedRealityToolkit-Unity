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
    // Provides a clickable link to documentation in the inspector header
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DocLinkAttribute : Attribute
    {

        public string DocURL { get; private set; }
        public string Description { get; private set; }

        public DocLinkAttribute(string docURL, string description = null)
        {
            DocURL = docURL;
            Description = description;
        }
    }
}