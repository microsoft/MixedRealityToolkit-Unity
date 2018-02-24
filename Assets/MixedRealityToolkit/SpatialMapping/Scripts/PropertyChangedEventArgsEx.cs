// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using UnityEngine;

namespace MixedRealityToolkit.SpatialMapping
{
    public static class PropertyChangedEventArgsEx
    {
        public static PropertyChangedEventArgsEx<TProperty> Create<TProperty>(string propertyName, TProperty oldValue, TProperty newValue)
        {
            return new PropertyChangedEventArgsEx<TProperty>(propertyName, oldValue, newValue);
        }

        public static PropertyChangedEventArgsEx<TProperty> Create<TProperty>(Expression<Func<TProperty>> memberGetter, TProperty oldValue, TProperty newValue)
        {
            return new PropertyChangedEventArgsEx<TProperty>(memberGetter, oldValue, newValue);
        }
    }

    [Serializable]
    public class PropertyChangedEventArgsEx<TProperty> : PropertyChangedEventArgs
    {
        public TProperty OldValue { get; private set; }
        public TProperty NewValue { get; private set; }

        public PropertyChangedEventArgsEx(string inPropertyName, TProperty inOldValue, TProperty inNewValue) :
            base(inPropertyName)
        {
            OldValue = inOldValue;
            NewValue = inNewValue;
        }

        public PropertyChangedEventArgsEx(Expression<Func<TProperty>> memberGetter, TProperty inOldValue, TProperty inNewValue) :
            this(GetMemberName(memberGetter), inOldValue, inNewValue)
        {
            // Nothing.
        }

        private static string GetMemberName(Expression<Func<TProperty>> memberGetter)
        {
            Debug.Assert(memberGetter.Body is MemberExpression);

            string memberName = ((MemberExpression)memberGetter.Body).Member.Name;
            return memberName;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} -> {2}",
                PropertyName,
                OldValue,
                NewValue
                );
        }
    }
}
