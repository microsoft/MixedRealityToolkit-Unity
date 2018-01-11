// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.Utilities.Attributes
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