// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.Utilities.Attributes
{
    // Class used to send members to bottom of drawing queue
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DrawLastAttribute : Attribute { }
}