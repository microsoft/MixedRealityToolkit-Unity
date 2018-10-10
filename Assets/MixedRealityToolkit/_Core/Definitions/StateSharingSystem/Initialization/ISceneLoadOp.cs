namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization
{
    public interface ISceneLoadOp
    {
        bool ReadyToActivate { get; }
        bool Activated { get; }
        void Activate();
        void Dispose();
    }
}