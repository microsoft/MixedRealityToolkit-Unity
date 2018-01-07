namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Base class for input sources that don't inherit from MonoBehaviour.
    /// </summary>
    public class GenericInputSource : IInputSource
    {
        public GenericInputSource(uint sourceId, string name)
        {
            SourceId = sourceId;
            Name = name;
        }

        public uint SourceId { get; private set; }

        public string Name { get; private set; }

        public SupportedInputInfo SupportedInputInfo { get; private set; }

        public virtual SupportedInputInfo GetSupportedInputInfo()
        {
            return SupportedInputInfo;
        }

        public bool SupportsInputInfo(SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo() & inputInfo) == inputInfo;
        }
    }
}