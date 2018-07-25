namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    /// <summary>
    /// Default options for how to generate points in a line renderer
    /// </summary>
    public enum StepMode
    {
        Interpolated,   // Draw points based on LineStepCount
        FromSource,     // Draw only the points available in the source - use this for hard edges
    }
}