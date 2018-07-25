namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    public enum DistortionType
    {
        NormalizedLength,   // Use the normalized length of the line plus its distortion strength curve to determine distortion strength
        Uniform,            // Use a single value to determine distortion strength
    }
}