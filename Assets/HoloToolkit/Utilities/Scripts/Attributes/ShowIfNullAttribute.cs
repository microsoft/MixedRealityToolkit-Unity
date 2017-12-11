// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
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