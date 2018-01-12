// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;

namespace MixedRealityToolkit.Utilities.Attributes
{
    // Validates object and displays an error or warning if validation fails
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ValidateUnityObjectAttribute : Attribute
    {
        public enum ActionEnum
        {
            Success,
            Warn,
            Error,
            HaltError,
        }

        public ActionEnum FailAction { get; private set; }
        public string MethodName { get; private set; }
        public string MessageOnFail { get; private set; }

        public ValidateUnityObjectAttribute(string methodName, string messageOnError, ActionEnum failAction = ActionEnum.Warn)
        {
            MethodName = methodName;
            MessageOnFail = messageOnError;
            FailAction = failAction;
        }

        public ActionEnum Validate(UnityEngine.Object target, System.Object source, out string messageOnFail)
        {
            if (source == null)
                throw new NullReferenceException("Source cannot be null.");

            MethodInfo m = source.GetType().GetMethod(MethodName);
            if (m == null)
                throw new MissingMethodException("Method " + MethodName + " not found in type " + source.GetType().ToString());

            bool result = (bool)m.Invoke(source, new System.Object[] { target });

            if (result)
            {
                messageOnFail = string.Empty;
                return ActionEnum.Success;
            }

            messageOnFail = MessageOnFail;
            return FailAction;
        }
    }
}