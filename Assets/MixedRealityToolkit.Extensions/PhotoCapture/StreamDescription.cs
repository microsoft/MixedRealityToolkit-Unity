// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture
{
    /// <summary>
    /// The type of camera
    /// </summary>
    public enum CameraType
    {
        Invalid,
        Color,
        Depth,
        Infrared,
    }

    /// <summary>
    /// Struct containing information related to a camera's resolution and framerate.
    /// </summary>
    [Serializable]
    public struct CameraResolution : IEquatable<CameraResolution>
    {
        /// <summary>
        /// Width in pixels of this resolution
        /// </summary>
        public uint Width;

        /// <summary>
        /// Height in pixels of this resolution
        /// </summary>
        public uint Height;

        /// <summary>
        /// Framerate of this quality setting in frames per second. This will only be non-zero for 
        /// Video and PhotoLowLatency modes
        /// </summary>
        public double Framerate;

        public override bool Equals(object obj)
        {
            if (!(obj is CameraResolution))
            {
                return false;
            }

            return this == (CameraResolution)obj;
        }

        public override int GetHashCode()
        {
            return (Width * Height * Framerate).GetHashCode();
        }

        public bool Equals(CameraResolution other)
        {
            return this == other;
        }

        public static bool operator ==(CameraResolution lhs, CameraResolution rhs)
        {
            return lhs.Width == rhs.Width && lhs.Height == rhs.Height && lhs.Framerate == rhs.Framerate;
        }

        public static bool operator !=(CameraResolution lhs, CameraResolution rhs)
        {
            return !(lhs == rhs);
        }
    }

    /// <summary>
    /// Class containing data related to a stream description
    /// </summary>
    [Serializable]
    public class StreamDescription : IEquatable<StreamDescription>
    {
        /// <summary>
        /// Camera source
        /// </summary>
        public string SourceName = String.Empty;

        /// <summary>
        /// Camera id
        /// </summary>
        public string SourceId = String.Empty;

        /// <summary>
        /// Resolution of the camera
        /// </summary>
        public CameraResolution Resolution = new CameraResolution();

        /// <summary>
        /// Camera type
        /// </summary>
        public CameraType CameraType = CameraType.Color;

        public override bool Equals(object obj)
        {
            StreamDescription other = obj as StreamDescription;
            if (other == null)
            {
                return false;
            }

            return this == other;
        }

        public override int GetHashCode()
        {
            return SourceName.GetHashCode();
        }

        public bool Equals(StreamDescription other)
        {
            return this == other;
        }

        public static bool operator ==(StreamDescription lhs, StreamDescription rhs)
        {
            return lhs.SourceId == rhs.SourceId && lhs.SourceName == rhs.SourceName && lhs.Resolution == rhs.Resolution;
        }

        public static bool operator !=(StreamDescription lhs, StreamDescription rhs)
        {
            return !(lhs == rhs);
        }
    }
}
