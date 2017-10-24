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
    // Shows / hides based on whether named member is null
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ShowIfNullAttribute : ShowIfAttribute
    {
        public ShowIfNullAttribute(string nullableMemberName, bool showIfConditionMet = false)
        {
            MemberName = nullableMemberName;
            ShowIfConditionMet = showIfConditionMet;
        }

#if UNITY_EDITOR
        public override bool ShouldShow(object target)
        {
            bool isNullable = true;
            if (target != null)
                isNullable = IsNullable(target, MemberName);

            if (!isNullable)
                throw new InvalidCastException("Member " + MemberName + " is not nullable.");

            UnityEngine.Object memberValue = (UnityEngine.Object)GetMemberValue(target, MemberName);
            bool conditionMet = memberValue == null;
            return ShowIfConditionMet ? conditionMet : !conditionMet;
        }
#endif
    }
}