namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl
{
    public interface IAppStateClient : IAppStateReadWrite
    {
        /// <summary>
        /// Assign a state pipe that this app state can use to send states to server.
        /// </summary>
        void AssignStatePipeInput(IStatePipeInput statePipeInput);
    }
}