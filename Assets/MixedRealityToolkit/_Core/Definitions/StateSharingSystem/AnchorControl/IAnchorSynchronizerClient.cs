using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AnchorControl
{
    public interface IAnchorSynchronizerClient
    {
        /// <summary>
        /// Copies any locally retrieved anchors to the user's user anchor states
        /// </summary>
        void UpdateUserAnchorStates(sbyte userNum, IAnchorMatrixSource anchorMatrixSource, IAppStateReadWrite appState);
    }
}