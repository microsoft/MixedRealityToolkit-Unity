using System;
using System.Collections.Generic;

namespace MRTK.Core
{
    public interface IStatePipe
    {
        void SendFlushedStates(Type stateArrayType, List<object> flushedStates);
    }
}