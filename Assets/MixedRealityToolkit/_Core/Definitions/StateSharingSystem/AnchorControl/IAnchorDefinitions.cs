using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AnchorControl
{
    public interface IAnchorDefinitions
    {
        void FetchDefinitions();
        bool Ready { get; }
        IEnumerable<AnchorDefinition> Definitions { get; }
    }
}