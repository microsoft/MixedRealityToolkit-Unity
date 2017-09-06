using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public class GamePadEventData : InputEventData
    {
        public string GamePadName { get; private set; }

        public GamePadEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource source, uint sourceId, string gamePadName)
        {
            BaseInitialize(source, sourceId);
            GamePadName = gamePadName;
        }
    }
}
