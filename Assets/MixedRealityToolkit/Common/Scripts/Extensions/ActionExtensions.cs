// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.Common.Extensions
{
    /// <summary>
    /// Extensions for the action class.
    /// These methods encapsulate the null check before raising an event for an Action.
    /// </summary>
    public static class ActionExtensions
    {
        public static void RaiseEvent(this Action action)
        {
            action?.Invoke();
        }

        public static void RaiseEvent<T>(this Action<T> action, T arg)
        {
            action?.Invoke(arg);
        }

        public static void RaiseEvent<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            action?.Invoke(arg1, arg2);
        }

        public static void RaiseEvent<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            action?.Invoke(arg1, arg2, arg3);
        }

        public static void RaiseEvent<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            action?.Invoke(arg1, arg2, arg3, arg4);
        }
    }
}
