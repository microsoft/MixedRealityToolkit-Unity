// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Extensions
{
    public static class ConverterExtensions
    {
        /// <summary>
        /// Get the bytes of a built in type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns>The byte array value of a built-in type.</returns>
        public static byte[] GetBuiltInTypeBytes<T>(this T input)
        {
            if (!typeof(T).IsPrimitive ||
                typeof(T) != typeof(string))
            {
                Debug.LogError($"{typeof(T).Name} must be a built-in type!");
                return null;
            }

            // https://stackoverflow.com/questions/39005931/generic-method-using-bitconverter-getbytes
            int size = Marshal.SizeOf(typeof(T));
            var result = new byte[size];
            var gcHandle = GCHandle.Alloc(input, GCHandleType.Pinned);
            Marshal.Copy(gcHandle.AddrOfPinnedObject(), result, 0, size);
            gcHandle.Free();
            return result;
        }

        /// <summary>
        /// Get the 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <exception cref="ArgumentException">the <see cref="T"/> parameter is not a built in type.</exception>
        /// <returns>The value of <see cref="T"/></returns>
        public static T GetBuiltInTypeValueFromBytes<T>(this byte[] input)
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)Convert.ChangeType(BitConverter.ToBoolean(input, 0), typeof(T));
            }

            if (typeof(T) == typeof(char))
            {
                return (T)Convert.ChangeType(BitConverter.ToChar(input, 0), typeof(T));
            }

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(BitConverter.ToString(input, 0), typeof(T));
            }

            if (typeof(T) == typeof(double))
            {
                return (T)Convert.ChangeType(BitConverter.ToDouble(input, 0), typeof(T));
            }

            if (typeof(T) == typeof(short))
            {
                return (T)Convert.ChangeType(BitConverter.ToInt16(input, 0), typeof(T));
            }

            if (typeof(T) == typeof(int))
            {
                return (T)Convert.ChangeType(BitConverter.ToInt32(input, 0), typeof(T));
            }

            if (typeof(T) == typeof(long))
            {
                return (T)Convert.ChangeType(BitConverter.ToInt64(input, 0), typeof(T));
            }

            if (typeof(T) == typeof(float))
            {
                return (T)Convert.ChangeType(BitConverter.ToSingle(input, 0), typeof(T));
            }

            if (typeof(T) == typeof(ushort))
            {
                return (T)Convert.ChangeType(BitConverter.ToUInt16(input, 0), typeof(T));
            }

            if (typeof(T) == typeof(uint))
            {
                return (T)Convert.ChangeType(BitConverter.ToUInt32(input, 0), typeof(T));
            }

            if (typeof(T) == typeof(ulong))
            {
                return (T)Convert.ChangeType(BitConverter.ToUInt64(input, 0), typeof(T));
            }

            throw new ArgumentException($"{typeof(T).Name} is not a built in type");
        }
    }
}