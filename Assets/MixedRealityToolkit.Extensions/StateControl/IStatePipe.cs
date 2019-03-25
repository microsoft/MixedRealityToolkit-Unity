using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.StateControl
{
    public interface IStatePipe
    {
        void SendFlushedStates(Type stateArrayType, List<object> flushedStates);
    }
}