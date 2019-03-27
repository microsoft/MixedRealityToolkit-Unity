// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.MarkerDetection
{
    public class Marker
    {
        public int Id { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        private StringBuilder _stringBuilder;

        public Marker(int id, Vector3 position, Quaternion rotation)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
        }

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
