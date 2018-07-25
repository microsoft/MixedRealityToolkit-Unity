namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    /// <summary>
    /// Default options for getting a rotation along a line
    /// </summary>
    public enum LineRotationType
    {
        None,                           // Don't rotate
        Velocity,                       // Use velocity
        RelativeToOrigin,               // Rotate relative to direction from origin point
    }
}