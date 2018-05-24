// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_WSA && !UNITY_EDITOR && !ENABLE_IL2CPP

using System;

namespace System.Runtime.InteropServices
{
    [ComVisible (true)]
    public struct HandleRef
    {
        object wrapper;
        IntPtr handle;

        public HandleRef (object wrapper, IntPtr handle)
        {
            this.wrapper = wrapper;
            this.handle = handle;
        }

        public IntPtr Handle
        {
            get { return handle; }
        }

        public object Wrapper
        {
            get { return wrapper; }
        }

        public static explicit operator IntPtr (HandleRef value)
        {
            return value.Handle;
        }

        public static IntPtr ToIntPtr(HandleRef value)
        {
            return value.Handle;
        }
    }
}
#endif
