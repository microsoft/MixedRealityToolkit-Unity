using System.Collections;

namespace Pixie.Initialization
{
    public interface ISceneLoadOp : IEnumerator
    {
        bool ReadyToActivate { get; }
        bool Finished { get; }
        void Activate();
    }
}