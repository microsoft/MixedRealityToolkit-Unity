namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    /// <summary>
    /// Default options for getting an interpolated point along a line
    /// </summary>
    public enum PointDistributionType
    {
        None,                       // Don't adjust placement
        Auto,                       // Adjust placement automatically (default)
        DistanceSingleValue,        // Place based on distance
        DistanceCurveValue,         // Place based on curve
    }
}