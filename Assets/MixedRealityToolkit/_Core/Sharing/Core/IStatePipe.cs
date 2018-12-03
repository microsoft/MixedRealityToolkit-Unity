using System;
using System.Collections.Generic;

namespace Pixie.Core
{
    public interface IStatePipe
    {
        void SendFlushedStates(Type stateArrayType, List<object> flushedStates);
    }
}