// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal static class NetworkExtensions
    {
        public static void Write(this BinaryWriter message, Guid value)
        {
            message.Write(value.ToString("D"));
        }

        public static void Write(this BinaryWriter message, Color value)
        {
            message.Write(value.r);
            message.Write(value.g);
            message.Write(value.b);
            message.Write(value.a);
        }

        public static void Write(this BinaryWriter message, Matrix4x4 value)
        {
            message.Write(value.GetColumn(0));
            message.Write(value.GetColumn(1));
            message.Write(value.GetColumn(2));
            message.Write(value.GetColumn(3));
        }

        public static void Write(this BinaryWriter message, Vector4 value)
        {
            message.Write(value.x);
            message.Write(value.y);
            message.Write(value.z);
            message.Write(value.w);
        }

        public static void Write(this BinaryWriter message, Vector3 value)
        {
            message.Write(value.x);
            message.Write(value.y);
            message.Write(value.z);
        }

        public static void Write(this BinaryWriter message, Vector2 value)
        {
            message.Write(value.x);
            message.Write(value.y);
        }

        public static void Write(this BinaryWriter message, Quaternion value)
        {
            message.Write(value.eulerAngles.x);
            message.Write(value.eulerAngles.y);
            message.Write(value.eulerAngles.z);
        }
        public static void Write(this BinaryWriter message, Color32 value)
        {
            message.Write(value.r);
            message.Write(value.g);
            message.Write(value.b);
            message.Write(value.a);
        }

        public static bool ReadBoolean(this BinaryReader message)
        {
            return message.ReadByte() != 0;
        }

        public static Color ReadColor(this BinaryReader message)
        {
            return new Color(message.ReadSingle(), message.ReadSingle(), message.ReadSingle(), message.ReadSingle());
        }

        public static Matrix4x4 ReadMatrix4x4(this BinaryReader message)
        {
            return new Matrix4x4(message.ReadVector4(), message.ReadVector4(), message.ReadVector4(), message.ReadVector4());
        }

        public static Vector4 ReadVector4(this BinaryReader message)
        {
            return new Vector4(message.ReadSingle(), message.ReadSingle(), message.ReadSingle(), message.ReadSingle());
        }

        public static Vector3 ReadVector3(this BinaryReader message)
        {
            return new Vector3(message.ReadSingle(), message.ReadSingle(), message.ReadSingle());
        }

        public static Vector2 ReadVector2(this BinaryReader message)
        {
            return new Vector2(message.ReadSingle(), message.ReadSingle());
        }

        public static Quaternion ReadQuaternion(this BinaryReader message)
        {
            return Quaternion.Euler(message.ReadVector3());
        }

        public static Color32 ReadColor32(this BinaryReader message)
        {
            return new Color32(message.ReadByte(), message.ReadByte(), message.ReadByte(), message.ReadByte());
        }

        public static ShortID ReadShortID(this BinaryReader message)
        {
            return new ShortID(message.ReadUInt16());
        }

        public static void Write(this BinaryWriter message, Vector3[] array)
        {
            WriteArray(message, array, (writer, value) => writer.Write(value));
        }

        public static void Write(this BinaryWriter message, Vector2[] array)
        {
            WriteArray(message, array, (writer, value) => writer.Write(value));
        }

        public static void Write(this BinaryWriter message, Color[] array)
        {
            WriteArray(message, array, (writer, value) => writer.Write(value));
        }

        public static void Write(this BinaryWriter message, int[] array)
        {
            WriteArray(message, array, (writer, value) => writer.Write(value));
        }

        public static void WriteArray<T>(this BinaryWriter message, T[] array, Action<BinaryWriter, T> writeAction)
        {
            bool isNull = array == null;
            message.Write(isNull);

            if (isNull)
            {
                return;
            }

            message.Write(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                writeAction(message, array[i]);
            }
        }

        public static Guid ReadGuid(this BinaryReader message)
        {
            return new Guid(message.ReadString());
        }

        public static Vector3[] ReadVector3Array(this BinaryReader message)
        {
            return ReadArray(message, reader => reader.ReadVector3());
        }

        public static Vector2[] ReadVector2Array(this BinaryReader message)
        {
            return ReadArray(message, reader => reader.ReadVector2());
        }

        public static Color[] ReadColorArray(this BinaryReader message)
        {
            return ReadArray(message, reader => reader.ReadColor());
        }

        public static int[] ReadInt32Array(this BinaryReader message)
        {
            return ReadArray(message, reader => reader.ReadInt32());
        }

        public static T[] ReadArray<T>(this BinaryReader message, Func<BinaryReader, T> readElement)
        {
            bool isNull = message.ReadBoolean();
            if (isNull)
            {
                return null;
            }

            int length = message.ReadInt32();
            if (length == 0)
            {
                return Array.Empty<T>();
            }

            T[] buffer = new T[length];

            for (int i = 0; i < length; i++)
            {
                buffer[i] = readElement(message);
            }
            return buffer;
        }
    }
}
