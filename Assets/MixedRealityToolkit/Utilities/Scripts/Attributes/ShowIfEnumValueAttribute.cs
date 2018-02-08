// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common.Extensions;
using System;

namespace MixedRealityToolkit.Utilities.Attributes
{
    // Shows / hides based on enum value of a named member
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ShowIfEnumValueAttribute : ShowIfAttribute
    {
        public int[] ShowValues { get; private set; }

        // IL2CPP doesn't support attributes with object arguments that are array types
        public ShowIfEnumValueAttribute(string enumVariableName, object enumValue, bool showIfConditionMet = true)
        {
            if (!enumValue.GetType().IsEnum())
                throw new Exception("Value must be of type Enum");

            ShowValues = new int[] { Convert.ToInt32(enumValue) };
            MemberName = enumVariableName;
            ShowIfConditionMet = showIfConditionMet;
        }

        public ShowIfEnumValueAttribute(string enumVariableName, object enumValue1, object enumValue2, bool showIfConditionMet = true)
        {
            if (!enumValue1.GetType().IsEnum() || !enumValue2.GetType().IsEnum())
                throw new Exception("Values must be of type Enum");

            ShowValues = new int[] { Convert.ToInt32(enumValue1), Convert.ToInt32(enumValue2) };
            MemberName = enumVariableName;
            ShowIfConditionMet = showIfConditionMet;
        }

        public ShowIfEnumValueAttribute(string enumVariableName, object enumValue1, object enumValue2, object enumValue3, bool showIfConditionMet = true)
        {
            if (!enumValue1.GetType().IsEnum() || !enumValue2.GetType().IsEnum() || !enumValue3.GetType().IsEnum())
                throw new Exception("Values must be of type Enum");

            ShowValues = new int[] { Convert.ToInt32(enumValue1), Convert.ToInt32(enumValue2), Convert.ToInt32(enumValue3) };
            MemberName = enumVariableName;
            ShowIfConditionMet = showIfConditionMet;
        }

        public ShowIfEnumValueAttribute(string enumVariableName, object enumValue1, object enumValue2, object enumValue3, object enumValue4, bool showIfConditionMet = true)
        {
            if (!enumValue1.GetType().IsEnum() || !enumValue2.GetType().IsEnum() || !enumValue3.GetType().IsEnum() || !enumValue4.GetType().IsEnum())
                throw new Exception("Values must be of type Enum");

            ShowValues = new int[] { Convert.ToInt32(enumValue1), Convert.ToInt32(enumValue2), Convert.ToInt32(enumValue3), Convert.ToInt32(enumValue4) };
            MemberName = enumVariableName;
            ShowIfConditionMet = showIfConditionMet;
        }

#if UNITY_EDITOR
        public override bool ShouldShow(object target)
        {
            bool conditionMet = false;
            int memberValue = Convert.ToInt32(GetMemberValue(target, MemberName));
            for (int i = 0; i < ShowValues.Length; i++)
            {
                if (ShowValues[i] == memberValue)
                {
                    conditionMet = true;
                    break;
                }
            }
            return ShowIfConditionMet ? conditionMet : !conditionMet;
        }
#endif

        private static object GetAsUnderlyingType(Enum enval)
        {
            Type entype = enval.GetType();
            Type undertype = Enum.GetUnderlyingType(entype);
            return Convert.ChangeType(enval, undertype);
        }
    }
}