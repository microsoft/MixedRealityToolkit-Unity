//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using System;

namespace HoloToolkit.Unity
{ 
    /// <summary>
    /// Extensions for the action class.
    /// These methods encapsulate the null check before raising an event for an Action.
    /// </summary>
    public static class ActionExtensions
    {
        public static void RaiseEvent(this Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        public static void RaiseEvent<T>(this Action<T> action, T arg)
        {
            if (action != null)
            {
                action(arg);
            }
        }

        public static void RaiseEvent<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action != null)
            {
                action(arg1, arg2);
            }
        }

        public static void RaiseEvent<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3);
            }
        }

        public static void RaiseEvent<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (action != null)
            {
                action(arg1, arg2, arg3, arg4);
            }
        }
    }
}
