using Pixie.StateControl;

namespace Pixie.AnchorControl
{
    public interface IAnchorSynchronizerServer
    {
        AnchorSyncStateEnum State { get; }

        /// <summary>
        /// Generates shared anchor states, user states and alignment states for all users
        /// </summary>
        /// <param name="definitions"></param>
        /// <param name="appState"></param>
        void CreateAnchorStates(IAnchorDefinitions definitions, IAppStateReadWrite appState);

        /// <summary>
        /// Uses shared anchor states and user anchor states to generate alignment states
        /// </summary>
        /// <param name="anchorMatrixes"></param>
        /// <param name="appState"></param>
        void UpdateAlignmentStates(IAppStateReadWrite appState);
    }
}