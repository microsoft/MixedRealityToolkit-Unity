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
    // Shows / hides based on bool value of named member
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ShowIfBoolValueAttribute : ShowIfAttribute
    {

        public ShowIfBoolValueAttribute(string boolMemberName, bool showIfConditionMet = true)
        {
            MemberName = boolMemberName;
            ShowIfConditionMet = showIfConditionMet;
        }

#if UNITY_EDITOR
        public override bool ShouldShow(object target)
        {
            bool conditionMet = (bool)GetMemberValue(target, MemberName);
            return ShowIfConditionMet ? conditionMet : !conditionMet;
        }
#endif
    }
}