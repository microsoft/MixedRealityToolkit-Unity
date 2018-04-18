using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Internal
{
    public interface IEventSource : IEqualityComparer
    {
        uint SourceId { get; }

        string SourceName { get; }
    }
}