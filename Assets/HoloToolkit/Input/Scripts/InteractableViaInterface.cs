using UnityEngine;

namespace HoloToolkit.Unity
{
    public class InteractableViaInterface : MonoBehaviour, IInteractable
    {
        public void OnTap()
        {
        }

        public void OnGazeEnter()
        {
        }

        public void OnGazeExit()
        {
        }
    }
}