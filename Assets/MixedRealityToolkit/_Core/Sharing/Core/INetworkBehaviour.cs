namespace Pixie.Core
{
    #region base interfaces

    /// <summary>
    /// Interface for objects that inherit from NetworkBehavior (or to simulate such an object).
    /// </summary>
    public interface INetworkBehaviour : IGameObject
    {
        bool isLocalPlayer { get; }
    }

    #endregion
}