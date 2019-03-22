using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MRTK.Core
{
    public static class StateUtils
    {
        public static Vector3 GetPredictedPos(Vector3 lastPos, Vector3 lastDir, float lastVelPerSecond, float lastTime, float currentTime, float latency)
        {
            predictedPos = lastPos;
            velocity = lastPos * lastVelPerSecond;
            if (velocity != Vector3.zero)
            {
                // Move the positon along the last known velocity by the difference in time, minus latency
                float diff = currentTime - (lastTime - latency);
                predictedPos += (velocity * diff);
            }
            return predictedPos;
        }

        public static string StateToString(object itemState)
        {
            Type type = itemState.GetType();
            string itemStateString = type.ToString();
            foreach (FieldInfo field in itemState.GetType().GetFields())
            {
                itemStateString += "\n" + field.Name + ": " + field.GetValue(itemState).ToString();
            }
            return itemStateString;
        }

        public static void ByteRotIn(Vector3 value, out byte rotX, out byte rotY, out byte rotZ)
        {
            value.x = value.x % 360;

            if (value.x < 0)
                value.x += 360;

            value.y = value.y % 360;

            if (value.y < 0)
                value.y += 360;

            value.z = value.z % 360;

            if (value.z < 0)
                value.z += 360;

            rotX = (byte)((value.x / 360) * byte.MaxValue);
            rotY = (byte)((value.y / 360) * byte.MaxValue);
            rotZ = (byte)((value.z / 360) * byte.MaxValue);
        }

        public static Vector3 ByteRotOut(byte x, byte y, byte z)
        {
            byteRotOut.x = ((float)x / byte.MaxValue) * 360f;
            byteRotOut.y = ((float)y / byte.MaxValue) * 360f;
            byteRotOut.z = ((float)z / byte.MaxValue) * 360f;
            return byteRotOut;
        }

        public static float ByteRotOut(byte rot)
        {
            return ((float)rot / byte.MaxValue) * 360f;
        }

        public static byte ByteRotIn(float rot)
        {
            rot = rot % 360;

            if (rot < 0)
                rot += 360;

            return (byte)((rot / 360) * byte.MaxValue);
        }

        public static Vector3 ByteDirOut(sbyte x, sbyte y, sbyte z)
        {
            byteDirOut.x = ((float)x / sbyte.MaxValue);
            byteDirOut.y = ((float)y / sbyte.MaxValue);
            byteDirOut.z = ((float)z / sbyte.MaxValue);

            return byteDirOut.normalized;
        }

        public static void ByteDirIn(Vector3 value, out sbyte x, out sbyte y, out sbyte z)
        {
            value.Normalize();

            x = (sbyte)(value.x * sbyte.MaxValue);
            y = (sbyte)(value.y * sbyte.MaxValue);
            z = (sbyte)(value.z * sbyte.MaxValue);
        }

        public static Vector3 ShortPosOut(short x, short y, short z, float maxRange)
        {
            shortPosOut.x = ((float)x / short.MaxValue) * maxRange;
            shortPosOut.y = ((float)y / short.MaxValue) * maxRange;
            shortPosOut.z = ((float)z / short.MaxValue) * maxRange;

            return shortPosOut;
        }

        public static void ShortPosIn(Vector3 value, out short x, out short y, out short z, float maxRange)
        {
            x = (short)(Mathf.Clamp(value.x / maxRange, -1f, 1f) * short.MaxValue);
            y = (short)(Mathf.Clamp(value.y / maxRange, -1f, 1f) * short.MaxValue);
            z = (short)(Mathf.Clamp(value.z / maxRange, -1f, 1f) * short.MaxValue);
        }

        public static void ShortDirIn(Vector3 value, out short x, out short y, out short z)
        {
            value = Vector3.Normalize(value);

            x = (short)(value.x * short.MaxValue);
            y = (short)(value.y * short.MaxValue);
            z = (short)(value.z * short.MaxValue);
        }

        public static Vector3 ShortRotOut(short x, short y, short z)
        {
            shortRotOut.x = ((float)x / short.MaxValue) * 360;
            shortRotOut.y = ((float)y / short.MaxValue) * 360;
            shortRotOut.z = ((float)z / short.MaxValue) * 360;

            return shortRotOut;
        }

        public static void ShortRotIn(Vector3 value, out short x, out short y, out short z)
        {
            value.x = value.x % 360;

            if (value.x < 0)
                value.x += 360;

            value.y = value.y % 360;

            if (value.y < 0)
                value.y += 360;

            value.z = value.z % 360;

            if (value.z < 0)
                value.z += 360;

            x = (short)(value.x * short.MaxValue);
            y = (short)(value.y * short.MaxValue);
            z = (short)(value.z * short.MaxValue);
        }

        public static Vector3 ShortDirOut(short x, short y, short z)
        {
            shortDirOut.x = ((float)x / short.MaxValue);
            shortDirOut.y = ((float)y / short.MaxValue);
            shortDirOut.z = ((float)z / short.MaxValue);
            return shortDirOut.normalized;
        }

        public static float ByteValOut(byte value, float maxValue = 1f)
        {
            return ((float)value / byte.MaxValue) * maxValue;
        }

        public static byte ByteValIn(float value, float maxValue = 1f)
        {
            return (byte)(Mathf.Clamp01(value / maxValue) * byte.MaxValue);
        }

        public static float UShortValOut(ushort value, float maxValue = 1f)
        {
            return ((float)value / maxValue) * ushort.MaxValue;
        }

        public static ushort UShortValIn(float value, float maxValue = 1f)
        {
            return (ushort)(Mathf.Clamp01(value / maxValue) * ushort.MaxValue);
        }

        private static Vector3 velocity;
        private static Vector3 predictedPos;
        private static Vector3 byteRotOut;
        private static Vector3 byteRotIn;
        private static Vector3 byteDirOut;
        private static Vector3 shortPosOut;
        private static Vector3 shortRotOut;
        private static Vector3 shortDirOut;

        public static bool IsPositionValid(Vector3 value)
        {
            return !float.IsNaN(value.x) && !float.IsNaN(value.y) && !float.IsNaN(value.z);
        }

        /// <summary>
        /// Searches all assemblies for structs with AppStateType attribute
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllStateTypes()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in GetAllStateTypesInAssembly(assembly))
                    yield return type;
            }
        }

        /// <summary>
        /// Searches a single assembly for structs with AppStateType attribute
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllStateTypesInAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                AppStateTypeAttribute attribute = type.GetCustomAttribute<AppStateTypeAttribute>();
                if (attribute != null)
                    yield return type;
            }
        }

        /// <summary>
        /// Searches for a StateArray type that uses stateType as its generic argument
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetStateArrayType(Type stateType)
        {
            if (stateType == null)
                throw new NullReferenceException("stateType cannot be null");

            Type genericBaseClass = typeof(StateArray<>);
            Type constructed = genericBaseClass.MakeGenericType(new Type[] { stateType });

            if (constructed == null)
                throw new Exception("Couldn't get constructed type");
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (constructed.IsAssignableFrom(type))
                        return constructed;
                }
            }
            return null;
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;

                if (generic == cur)
                    return true;

                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}