// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity
{
    [Serializable]
    public class AppDetails
    {
        public string Name;
        public string PackageFamilyName;
        public string PackageFullName;
        public int PackageOrigin;
        public string PackageRelativeId;
        public string Publisher;
    }
}