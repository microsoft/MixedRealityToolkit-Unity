// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection
{
    /// <summary>
    /// Helper class containing data related to detected markers
    /// </summary>
    [Serializable]
    public class Marker
    {
        /// <summary>
        /// Id of the detected marker
        /// </summary>
        public int Id;

        /// <summary>
        /// Position of the detected marker relative to the application origin
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Rotation of the marker relative to the application origin.
        /// X-Axis is reported horizontal with the marker.
        /// Y-Axis positive direction is upward along the marker.
        /// Z-Axis positive direction is outward from the marker compared to into the marker.
        /// </summary>
        public Quaternion Rotation;

        /// <summary>
        /// Helper field for generating ToString content
        /// </summary>
        private StringBuilder _stringBuilder;

        /// <summary>
        /// Contructor for the Marker class
        /// </summary>
        /// <param name="id">Detected marker id</param>
        /// <param name="position">Position of the detected marker relative to the application origin</param>
        /// <param name="rotation">Rotation of the detected marker relative to the application origin</param>
        public Marker(int id, Vector3 position, Quaternion rotation)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (_stringBuilder == null)
                _stringBuilder = new StringBuilder();
            else
                _stringBuilder.Clear();

            _stringBuilder.Append(Id);
            _stringBuilder.Append(": ");
            _stringBuilder.Append(Position.x);
            _stringBuilder.Append(", ");
            _stringBuilder.Append(Position.y);
            _stringBuilder.Append(", ");
            _stringBuilder.Append(Position.z);
            _stringBuilder.Append(" ");
            _stringBuilder.Append(Rotation.x);
            _stringBuilder.Append(", ");
            _stringBuilder.Append(Rotation.y);
            _stringBuilder.Append(", ");
            _stringBuilder.Append(Rotation.z);
            _stringBuilder.Append(", ");
            _stringBuilder.Append(Rotation.w);
            return _stringBuilder.ToString();
        }
    }
}
