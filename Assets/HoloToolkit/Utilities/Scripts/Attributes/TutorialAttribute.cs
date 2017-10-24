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
    // Provides a clickable link to a tuturoial in the inspector header
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class TutorialAttribute : Attribute
    {

        public string TutorialURL { get; private set; }
        public string Description { get; private set; }

        public TutorialAttribute(string tutorialURL, string description = null)
        {
            TutorialURL = tutorialURL;
            Description = description;
        }
    }
}