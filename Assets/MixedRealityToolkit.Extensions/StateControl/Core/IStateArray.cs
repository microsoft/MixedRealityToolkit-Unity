using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.StateControl.Core
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