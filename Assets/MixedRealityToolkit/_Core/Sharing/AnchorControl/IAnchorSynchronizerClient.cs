using Pixie.StateControl;

namespace Pixie.AnchorControl
{
    public interface IAnchorSynchronizerClient
    {
        /// <summary>
        /// Copies any locally retrieved anchors to the user's user anchor states
        /// </summary>
        void UpdateUserAnchorStates(short userID, IAnchorMatrixSource anchorMatrixSource, IAppStateReadWrite appState);
    }
}