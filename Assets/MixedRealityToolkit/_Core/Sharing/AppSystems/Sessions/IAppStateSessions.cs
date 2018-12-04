namespace Pixie.AppSystems.Sessions
{
    public interface IAppStateSessions
    {
        /// <summary>
        /// Generates session states.
        /// This must be called before generating users or adding layout scene states.
        /// </summary>
        void GenerateSessionStates(int numConcurrentSessions);
    }
}
