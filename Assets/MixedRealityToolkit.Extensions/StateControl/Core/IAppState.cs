using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions.StateControl.Core;

namespace Microsoft.MixedReality.Toolkit.Extensions.StateControl
{
    public interface IAppState : IAppStateReadWrite, ISharingAppObject, IMixedRealityExtensionService { }
}