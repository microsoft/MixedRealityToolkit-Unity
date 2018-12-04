namespace Pixie.AnchorControl
{
    public interface ISpatialServiceManager
    {
        bool IsReadyForCreate { get; }
        bool IsReadyForLocate { get; }
        bool IsRecommendedForCreate { get; }
        bool IsRecommendedForLocate { get; }
        SpatialSessionState SessionState { get; }

        void StartSession();
        void StopSession();
    }
}