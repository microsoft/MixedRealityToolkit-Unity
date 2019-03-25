using System.Collections.Generic;

namespace MRTK.Networking
{
    public enum ConnectionStatusEnum
    {
        NotConnected,
        ConnectingToService,
        Connected,
        JoiningExperience,
        ExperienceJoined,
    }

    public interface IClientConnection
    {
        IEnumerable<string> AvailableExperiences { get; }

        ConnectionStatusEnum Status { get; }
        bool CanJoinExperience { get; }
        string FeedbackText { get; }

        void StartClient();
        void JoinExperience(string experienceName);
        void ForceDisconnect();
    }
}
