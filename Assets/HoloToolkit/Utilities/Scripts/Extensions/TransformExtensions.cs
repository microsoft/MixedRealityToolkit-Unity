//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using System.Text;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public static class TransformExtensions
    {
        /// <summary>
        /// An extension method that will get you the full path to an object.
        /// </summary>
        /// <param name="transform">The transform you wish a full path to.</param>
        /// <param name="delimiter">The delimiter with which each object is delimited in the string.</param>
        /// <param name="prefix">Prefix with which the full path to the object should start.</param>
        /// <returns>A delimited string that is the full path to the game object in the hierarchy.</returns>
        public static string GetFullPath(this Transform transform, string delimiter = ".", string prefix = "/")
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (transform.parent == null)
            {
                stringBuilder.Append(prefix);
                stringBuilder.Append(transform.name);
            }
            else
            {
                stringBuilder.Append(transform.parent.GetFullPath(delimiter, prefix));
                stringBuilder.Append(delimiter);
                stringBuilder.Append(transform.name);
            }

            return stringBuilder.ToString();
        }
    }
}