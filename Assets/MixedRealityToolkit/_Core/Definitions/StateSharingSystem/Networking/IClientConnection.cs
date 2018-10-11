namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Networking
{
    public interface IClientConnection
    {
        bool IsConnected { get; }
        bool ClientSceneReady { get; }
        bool UseLocalHost { get; set; }
        string ServerIP { get; set; }

        void SetClientSceneReady();
        void StartClient();
        void GetStatsOut(out int numMsgs, out int numBufferedMsgs, out int numBytes, out int lastBufferedPerSecond);
        void GetStatsIn(out int numMsgs, out int numBytes);
        void ForceDisconnect();
    }
}
