namespace HoloToolkit.Unity
{
    public interface IInteractable
    {
        void OnTap();

        void OnGazeEnter();

        void OnGazeExit();
    }
}