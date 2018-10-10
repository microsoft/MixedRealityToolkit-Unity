using System.Collections.Generic;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core
{
    /// <summary>
    /// A wrapper for UNet HLAPI SyncListStruct for IItemState<T> type structs.
    /// Caches changed entries until Flush is called to prevent spamming clients with data.
    /// Treats internal snyc list array as a dictionary.
    /// </summary>
    public interface IStateArray<T> : IStateArrayBase, IEnumerable<T> where T : struct, IItemState<T>
    {
        /// <summary>
        /// Access an item state
        /// </summary>
        T this[sbyte key] { get; set; }

        /// <summary>
        /// Initializes with states list. IsEmpty will return true until this has been called.
        /// </summary>
        void Create(SyncListStruct<T> states);
    }
}
