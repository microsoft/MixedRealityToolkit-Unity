using Pixie.Core;
using Pixie.DeviceControl.Users;

namespace Pixie.AppSystems.Managers
{
    public interface IAppManager
    {
        AppRoleEnum AppRole { get; }
        DeviceTypeEnum DeviceType { get; }
        IUserProfile Profile { get; }
        string ExperienceName { get; }

        void StartApp(IUserProfile profile);
        void StopApp();
        void ResetApp();
    }
}