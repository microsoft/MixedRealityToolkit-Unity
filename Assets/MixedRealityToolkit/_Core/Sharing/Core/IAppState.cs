using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using MRTK.Core;

namespace MRTK.StateControl
{
    public interface IAppState : IAppStateReadWrite, ISharingAppObject, IMixedRealityExtensionService { }
}