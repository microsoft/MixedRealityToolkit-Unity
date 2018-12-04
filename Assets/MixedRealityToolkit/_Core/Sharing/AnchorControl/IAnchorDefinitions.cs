using System.Collections.Generic;

namespace Pixie.AnchorControl
{
    public interface IAnchorDefinitions
    {
        void FetchDefinitions();
        bool Ready { get; }
        IEnumerable<AnchorDefinition> Definitions { get; }
    }
}