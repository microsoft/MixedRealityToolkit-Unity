using Pixie.Core;

namespace Pixie.DeviceControl
{
    public interface IDeviceInput : ISharingAppObject
    {
        void GatherDeviceInput(IUserObject userObject);
    }
}