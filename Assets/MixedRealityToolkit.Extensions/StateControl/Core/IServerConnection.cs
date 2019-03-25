namespace MRTK.Networking
{
    public interface IServerConnection
    {
        ConnectionStatusEnum Status { get; }
        bool CanJoinExperience { get; }
        string FeedbackText { get; }
        
        void StartServer();
        void CreateExperience(string experienceName);
        void ForceDisconnect();
    }
}