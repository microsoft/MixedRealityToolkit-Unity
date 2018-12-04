using Pixie.Core;

namespace Pixie.AppSystems.StateObjects
{
    public interface IAppListener<T> where T : struct, IItemState
    {
        /// <summary>
        /// Executed on both client and server. Use for shared behaviors.
        /// </summary>
        void OnUpdateApp(IStateObject<T> stateObj);

        /// <summary>
        /// Executed on server only.
        /// </summary>
        void OnUpdateAppServer(IStateObject<T> stateObj);

        /// <summary>
        /// Executed on client only.
        /// </summary>
        void OnUpdateAppClient(IStateObject<T> stateObj);
    }
}