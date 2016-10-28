//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class InputHandleCallbackFX : MonoBehaviour, IInputHandler
    {
        [SerializeField]
        private ParticleSystem particles = null;

        private void Start()
        {
            InputManager.Instance.PushFallbackInputHandler(gameObject);
        }

        public void OnInputUp(InputEventData eventData)
        {
        }

        public void OnInputDown(InputEventData eventData)
        {
        }

        public void OnInputClicked(InputEventData eventData)
        {
            particles.transform.position = GazeManager.Instance.HitPosition;
            particles.Emit(60);
        }
    }
}