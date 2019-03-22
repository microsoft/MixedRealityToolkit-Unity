using System.Collections.Generic;

namespace MRTK.Core
{
    public interface IStateArray<T> : IStateArrayBase, IEnumerable<T> where T : struct, IItemState, IItemStateComparer<T>
    {
        void AddState(T state);

        T this[short key]
        {
            get;
            set;
        }
    }
}