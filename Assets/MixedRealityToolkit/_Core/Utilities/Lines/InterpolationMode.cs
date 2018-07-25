namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    /// <summary>
    /// Default options for how to distribute interpolated points in a line renderer
    /// </summary>
    public enum InterpolationMode
    {
        FromNumSteps,   // Specify the number of interpolation steps manually
        FromLength,     // Create steps based on total length of line + manually specified length
        FromCurve       // Create steps based on total length of line + animation curve
    }
}