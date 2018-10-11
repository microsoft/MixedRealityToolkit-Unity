namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core
{
    /// <summary>
    /// A struct with an sbyte key.
    /// Used in IObjectStateArray.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IItemState<T>
    {
        /// <summary>
        /// The key for the item
        /// </summary>
        sbyte Key { get; }

        /// <summary>
        /// This will be used to filter out which items are displayed to users.
        /// </summary>
        sbyte Filter { get; }

        /// <summary>
        /// Compares to other of this type to determine if contents need to be synced
        /// Superior to value type comparisons and doesn't generate garbage
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        bool IsDifferent(T from);

        /// <summary>
        /// Used primarily by IObjectStateArray on the client side.
        /// If the client has a temporary local state, and a newly arrived value from the server conflicts with it, this function is called to produce a 'merged' local value.
        /// In most cases the server value will be preferred and the client value will be discarded.
        /// </summary>
        /// <param name="clientValue"></param>
        /// <param name="serverValue"></param>
        /// <returns></returns>
        T Merge(T clientValue, T serverValue);
    }
}