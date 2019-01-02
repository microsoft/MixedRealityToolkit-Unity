namespace Pixie.Core
{
    /// <summary>
    /// Manager objects are expected to:
    /// - Set the AppRole
    /// - Call OnAppInitialize as soon as object is found
    /// - Call OnAppStart as soon as all objects are initialized
    /// - Call OnAppShutDown before the app quits
    /// These need to be game objects so they can be scraped from scenes.
    /// </summary>
    public interface ISharingAppObject : IGameObject
    {
        AppRoleEnum AppRole { get; set; }
        DeviceTypeEnum DeviceType { get; set; }

        void OnAppInitialize();
        void OnAppConnect();
        void OnAppSynchronize();
        void OnAppShutDown();
    }
}