// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.



namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// A reference to the InspectorField and cached info
    /// </summary>
    [System.Serializable]
    public struct InspectorFieldData
    {
        public InspectorField Attributes;
        public object Value;
        public string Name;
    }
}
