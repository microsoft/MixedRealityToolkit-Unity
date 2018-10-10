using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects
{
    public interface ILayoutImporter : ISharingAppObject
    {
        bool Importing { get; }
        void GatherStateObjects(string layoutSceneName);
        void PushStateObjectsToStateView(IStateView stateView);
        void PushStateObjectsToAppState(IAppStateReadWrite appState);
    }
}