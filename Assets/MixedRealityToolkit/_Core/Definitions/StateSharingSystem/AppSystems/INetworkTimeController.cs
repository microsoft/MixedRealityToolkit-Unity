namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems
{
    public interface INetworkTimeController
    {
        /// <summary>
        /// Whether synchronized time has begin.
        /// </summary>
        bool Started { get; }

        /// <summary>
        /// The time our local network time object should seek to match.
        /// </summary>
        float TargetTime { get; }
    }
}