namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core
{
    /// <summary>
    /// Manager objects are expected to:
    /// - Set the AppRole
    /// - Call OnSharingStart as soon as object is found
    /// - Call OnStateInitialized as soon as all objects are initialized
    /// - Call OnSharingStop before the app quits
    /// These need to be game objects so they can be scraped from scenes.
    /// </summary>
    public interface ISharingAppObject : IGameObject
    {
        AppRoleEnum AppRole { get; set; }

        void OnSharingStart();
        void OnStateInitialized();
        void OnSharingStop();
    }
}