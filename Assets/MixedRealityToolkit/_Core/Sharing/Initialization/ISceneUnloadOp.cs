using System.Collections;

namespace Pixie.Initialization
{
    public interface ISceneUnloadOp : IEnumerator
    {
        bool Finished { get; }
    }
}