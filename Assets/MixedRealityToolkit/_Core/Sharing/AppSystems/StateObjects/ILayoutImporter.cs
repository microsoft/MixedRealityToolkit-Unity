using Pixie.Core;
using Pixie.StateControl;

namespace Pixie.AppSystems.StateObjects
{
    public interface ILayoutImporter : ISharingAppObject
    {
        bool Importing { get; }
        void GatherStateObjects(string layoutSceneName);
        void PushStateObjectsToStateView(IStateView stateView);
        void PushStateObjectsToAppState(IAppStateReadWrite appState);
    }
}