// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.



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
