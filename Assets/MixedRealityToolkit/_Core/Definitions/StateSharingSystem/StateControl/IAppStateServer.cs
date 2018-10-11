namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl
{
    /// <summary>
    /// Generates states for required types.
    /// </summary>
    public interface IAppStateServer : IAppStateReadWrite
    {
        /// <summary>
        /// Adds a source for client-side states
        /// </summary>
        void AssignStateOutputSource(IStatePipeOutputSource statePipeOutputSource);
    }
}