// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;
using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A PropertyAttribute for showing a warning box that the tagged implementation is experimental.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ExperimentalAttribute : PropertyAttribute
    {
        /// <summary>
        /// The text to display in the warning box.
        /// </summary>
        public string Text;

        private const string defaultText = "<b><color=yellow>This is an experimental feature.</color></b>\n" +
                                           "Parts of the MRTK appear to have a lot of value even if the details " +
                                           "haven’t fully been fleshed out. For these types of features, we want " +
                                           "the community to see them and get value out of them early. Because " +
                                           "they are early in the cycle, we label them as experimental to indicate " +
                                           "that they are still evolving, and subject to change over time.";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="experimentalText">The experimental text to display in the warning box.</param>
        public ExperimentalAttribute(string experimentalText = defaultText)
        {
            Text = experimentalText;
        }
    }
}
