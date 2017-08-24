//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

namespace HoloToolkit.Unity
{
    public class CopyDirectoryInfo
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Filter { get; set; }
        public bool Recursive { get; set; }

        public CopyDirectoryInfo()
        {
            Source = null;
            Destination = null;
            Filter = "*";
            Recursive = false;
        }
    }
}