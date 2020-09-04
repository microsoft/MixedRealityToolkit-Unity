// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Used to define an entire InputManagerAxis, with each variable defined by the same term the Inspector shows.
    /// </summary>
    public class InputManagerAxis
    {
        public string Name = string.Empty;
        public string DescriptiveName = string.Empty;
        public string DescriptiveNegativeName = string.Empty;
        public string NegativeButton = string.Empty;
        public string PositiveButton = string.Empty;
        public string AltNegativeButton = string.Empty;
        public string AltPositiveButton = string.Empty;
        public float Gravity = 0.0f;
        public float Dead = 0.0f;
        public float Sensitivity = 0.0f;
        public bool Snap = false;
        public bool Invert = false;
        public InputManagerAxisType Type = default(InputManagerAxisType);
        public int Axis = 0;
        public int JoyNum = 0;
    }
}
