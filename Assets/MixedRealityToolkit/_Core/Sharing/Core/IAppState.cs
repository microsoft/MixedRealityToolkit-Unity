using Microsoft.MixedReality.Toolkit;
using MRTK.Core;

namespace MRTK.StateControl
{
    public interface IAppState : IAppStateReadWrite, ISharingAppObject, IMixedRealityExtensionService { }
}