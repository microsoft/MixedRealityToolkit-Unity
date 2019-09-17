namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
    public interface ILostTrackingVisual
    {
        /// <summary>
        /// Completely enables or disables the visual. Should probably be linked to the root game object's active value.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Sets all visual components to the layer provided.
        /// </summary>
        /// <param name="layer"></param>
        void SetLayer(int layer);

        /// <summary>
        /// Resets the visual state to default
        /// </summary>
        void ResetVisual();
    }
}