// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    // Base class for show / hide - shows or hides fields & properties in the editor based on the value of a member in the target object
    public abstract class ShowIfAttribute : Attribute
    {
        public string MemberName { get; protected set; }
        public bool ShowIfConditionMet { get; protected set; }

#if UNITY_EDITOR
        public abstract bool ShouldShow(object target);

        protected static object GetMemberValue(object target, string memberName)
        {
            if (target == null)
                throw new NullReferenceException("Target cannot be null.");

            if (string.IsNullOrEmpty(memberName))
                throw new NullReferenceException("MemberName cannot be null.");

            Type targetType = target.GetType();

            MemberInfo[] members = targetType.GetMember(memberName);
            if (members.Length == 0)
                throw new MissingMemberException("Couldn't find member '" + memberName + "'");

            object memberValue;

            switch (members[0].MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = targetType.GetField(memberName);
                    memberValue = fieldInfo.GetValue(target);
                    break;

                case MemberTypes.Property:
                    PropertyInfo propertyInfo = targetType.GetProperty(memberName);
                    memberValue = propertyInfo.GetValue(target, null);
                    break;

                default:
                    throw new MissingMemberException("Member '" + memberName + "' must be a field or property");
            }
            return memberValue;
        }

        protected static bool IsNullable(object target, string memberName)
        {
            if (target == null)
                throw new NullReferenceException("Target cannot be null.");

            if (string.IsNullOrEmpty(memberName))
                throw new NullReferenceException("MemberName cannot be null.");

            Type targetType = target.GetType();

            MemberInfo[] members = targetType.GetMember(memberName);
            if (members.Length == 0)
                throw new MissingMemberException("Couldn't find member '" + memberName + "'");

            Type memberType = members[0].DeclaringType;

            if (!memberType.IsValueType)
                return true;

            if (Nullable.GetUnderlyingType(memberType) != null)
                return true;

            return false;
        }
#endif
    }
}