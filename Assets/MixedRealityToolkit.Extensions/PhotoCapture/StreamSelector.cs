// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture
{
    /// <summary>
    /// Class that helps with picking a specific stream type
    /// </summary>
    public class StreamSelector
    {
        /// <summary>
        /// List containing stream descriptions
        /// </summary>
        public List<StreamDescription> StreamDescriptions { get; private set; } = new List<StreamDescription>();

        /// <summary>
        /// Use this function to declare a new stream description
        /// </summary>
        /// <param name="streamDescription"></param>
        public void AddStream(StreamDescription streamDescription)
        {
            if (!StreamDescriptions.Contains(streamDescription))
            {
                StreamDescriptions.Add(streamDescription);
            }
        }

        /// <summary>
        /// Select streams by resolution
        /// </summary>
        /// <param name="compare">The comparison to use</param>
        /// <param name="width">The width to compare with</param>
        /// <param name="height">The height to compare with</param>
        /// <returns></returns>
        public StreamSelector Select(StreamCompare compare, int width, int height)
        {
            StreamSelector selector = new StreamSelector();

            foreach (StreamDescription desc in StreamDescriptions)
            {
                if (compare == StreamCompare.GreaterThan)
                {
                    if (desc.Resolution.Width > width && desc.Resolution.Height > height)
                    {
                        selector.AddStream(desc);
                    }
                }
                else if (compare == StreamCompare.LessThan)
                {
                    if (desc.Resolution.Width < width && desc.Resolution.Height < height)
                    {
                        selector.AddStream(desc);
                    }
                }
                else if (compare == StreamCompare.EqualTo)
                {
                    if (desc.Resolution.Width == width && desc.Resolution.Height == height)
                    {
                        selector.AddStream(desc);
                    }
                }
            }

            return selector;
        }

        /// <summary>
        /// Select streams by framerate
        /// </summary>
        /// <param name="compare">The comparison to use</param>
        /// <param name="framerate">The framerate to compare with</param>
        /// <returns></returns>
        public StreamSelector Select(StreamCompare compare, double framerate)
        {
            StreamSelector selector = new StreamSelector();

            foreach (StreamDescription desc in StreamDescriptions)
            {
                if (compare == StreamCompare.GreaterThan)
                {
                    if (desc.Resolution.Framerate > framerate)
                    {
                        selector.AddStream(desc);
                    }
                }
                else if (compare == StreamCompare.LessThan)
                {
                    if (desc.Resolution.Framerate < framerate)
                    {
                        selector.AddStream(desc);
                    }
                }
                else if (compare == StreamCompare.EqualTo)
                {
                    if (desc.Resolution.Framerate == framerate)
                    {
                        selector.AddStream(desc);
                    }
                }
            }

            return selector;
        }
    }
}
