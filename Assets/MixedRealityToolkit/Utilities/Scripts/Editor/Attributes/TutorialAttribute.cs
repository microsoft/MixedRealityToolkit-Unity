// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.Utilities.Attributes
{
    // Provides a clickable link to a tutorial in the inspector header
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class TutorialAttribute : Attribute
    {

        public string TutorialURL { get; private set; }
        public string Description { get; private set; }

        public TutorialAttribute(string tutorialURL, string description = null)
        {
            TutorialURL = tutorialURL;
            Description = description;
        }
    }
}