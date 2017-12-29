using System;
using UnityEngine;
namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// TO DO
    /// </summary>
    public class TwoHandManipulatable : MonoBehaviour, IInputHandler, ISourceStateHandler
    {
        public event Action StartedManipulating;
        public event Action StoppedManipulating;

        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        public Transform HostTransform;

        public void OnInputDown(InputEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnInputUp(InputEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}