// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public enum ButtonChoices
    {
        Left,
        Right,
        Middle,
        Control,
        Shift,
        Alt,
        Focused,
        None
    }

    /// <summary>
    /// Since the InteractionSourceState is internal to UnityEngine.VR.WSA.Input,
    /// this is a fake SourceState structure to keep the test code consistent.
    /// </summary>
    public struct DebugInteractionSourceState
    {
        public bool Pressed;
        public DebugInteractionSourceProperties Properties;
    }

    /// <summary>
    /// Since the InteractionSourceProperties is internal to UnityEngine.VR.WSA.Input,
    /// this is a fake SourceProperties structure to keep the test code consistent.
    /// </summary>
    public struct DebugInteractionSourceProperties
    {
        public DebugInteractionSourceLocation Location;
    }

    /// <summary>
    /// Since the InteractionSourceLocation is internal to UnityEngine.VR.WSA.Input,
    /// this is a fake SourceLocation structure to keep the test code consistent.
    /// </summary>
    public class DebugInteractionSourceLocation
    {
        /// <summary>
        /// In the typical InteractionSourceLocation, the hardware determines if
        /// TryGetPosition and TryGetVelocity will return true or not. Here
        /// we manually emulate this state with TryGetFunctionsReturnsTrue.
        /// </summary>
        public bool TryGetFunctionsReturnsTrue;

        public Vector3 Position;
        public Vector3 Velocity;

        public void Awake()
        {
            TryGetFunctionsReturnsTrue = false;
            Position = new Vector3(0, 0, 0);
            Velocity = new Vector3(0, 0, 0);
        }

        public bool TryGetPosition(out Vector3 position)
        {
            position = Position;
            if (!TryGetFunctionsReturnsTrue)
            {
                return false;
            }
            return true;
        }

        public bool TryGetVelocity(out Vector3 velocity)
        {
            velocity = Velocity;
            if (!TryGetFunctionsReturnsTrue)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Class for manually controlling the hand(s) when not running on HoloLens (in editor).
    /// </summary>
    public class ManualHandControl : MonoBehaviour
    {
        public float HandReturnFactor = 0.25f;  /// [0.0,1.0] the closer this is to one the faster it brings the hand back to center
        public float HandTimeBeforeReturn = 0.5f;
        public float MinimumTrackedMovement = 0.001f;

        [Tooltip("Use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        public bool UseUnscaledTime = true;

        public AxisController LeftHandPrimaryAxisControl;
        public AxisController LeftHandSecondaryAxisControl;
        public ButtonController LeftFingerUpButtonControl;
        public ButtonController LeftFingerDownButtonControl;

        public AxisController RightHandPrimaryAxisControl;
        public AxisController RightHandSecondaryAxisControl;
        public ButtonController RightFingerUpButtonControl;
        public ButtonController RightFingerDownButtonControl;

        public DebugInteractionSourceState LeftHandSourceState;
        public DebugInteractionSourceState RightHandSourceState;

        public Color ActiveHandColor;
        public Color DroppedHandColor;
        /// <summary>
        /// Will place hand visualizations in the world space, only for debugging.
        /// Place the representative GameObjects in LeftHandVisualizer & RightHandVisualizer.
        /// </summary>
        public bool VisualizeHands = true;
        public GameObject LeftHandVisualizer;
        public GameObject RightHandVisualizer;

        public Texture HandUpTexture;
        public Texture HandDownTexture;

        public bool LeftHandInView;
        public bool RightHandInView;

        private Vector3 leftHandInitialPosition;
        private Vector3 rightHandInitialPosition;

        private Vector3 leftHandLocalPosition;
        private Vector3 rightHandLocalPosition;

        private Renderer leftHandVisualRenderer;
        private Renderer rightHandVisualRenderer;
        private MaterialPropertyBlock leftHandVisualPropertyBlock;
        private MaterialPropertyBlock rightHandVisualPropertyBlock;
        private int mainTexID;
        private bool appHasFocus = true;

        private float timeBeforeReturn;

        private void Awake()
        {
            mainTexID = Shader.PropertyToID("_MainTex");

            LeftHandSourceState.Pressed = false;
            LeftHandSourceState.Properties.Location = new DebugInteractionSourceLocation();
            leftHandLocalPosition = LeftHandVisualizer.transform.position;
            leftHandInitialPosition = leftHandLocalPosition;
            LeftHandSourceState.Properties.Location.Position = leftHandLocalPosition;
            leftHandVisualRenderer = LeftHandVisualizer.GetComponent<Renderer>();
            leftHandVisualPropertyBlock = new MaterialPropertyBlock();
            leftHandVisualRenderer.SetPropertyBlock(leftHandVisualPropertyBlock);

            RightHandSourceState.Pressed = false;
            RightHandSourceState.Properties.Location = new DebugInteractionSourceLocation();
            rightHandLocalPosition = RightHandVisualizer.transform.position;
            rightHandInitialPosition = rightHandLocalPosition;
            RightHandSourceState.Properties.Location.Position = rightHandLocalPosition;
            rightHandVisualRenderer = RightHandVisualizer.GetComponent<Renderer>();
            rightHandVisualPropertyBlock = new MaterialPropertyBlock();
            rightHandVisualRenderer.SetPropertyBlock(rightHandVisualPropertyBlock);

#if !UNITY_EDITOR
            VisualizeHands = false;
            UpdateHandVisualization();
            Destroy(this);
#endif
        }

        private void Update()
        {
            UpdateHandVisualization();

            float deltaTime = UseUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            float smoothingFactor = deltaTime * 30.0f * HandReturnFactor;
            if (timeBeforeReturn > 0.0f)
            {
                timeBeforeReturn = Mathf.Clamp(timeBeforeReturn - deltaTime, 0.0f, HandTimeBeforeReturn);
            }

            LeftHandSourceState.Pressed = LeftFingerDownButtonControl.Pressed();
            RightHandSourceState.Pressed = RightFingerDownButtonControl.Pressed();

            if (LeftHandSourceState.Pressed || RightHandSourceState.Pressed)
            {
                timeBeforeReturn = HandTimeBeforeReturn;
            }

            if (timeBeforeReturn <= 0.0f)
            {
                leftHandLocalPosition = Vector3.Slerp(leftHandLocalPosition, leftHandInitialPosition, smoothingFactor);
                if (leftHandLocalPosition == leftHandInitialPosition)
                {
                    LeftHandInView = false;
                }
                rightHandLocalPosition = Vector3.Slerp(rightHandLocalPosition, rightHandInitialPosition, smoothingFactor);
                if (rightHandLocalPosition == rightHandInitialPosition)
                {
                    RightHandInView = false;
                }
            }

            if (appHasFocus)
            {
                Vector3 translate1 = LeftHandPrimaryAxisControl.GetDisplacementVector3();
                Vector3 translate2 = LeftHandSecondaryAxisControl.GetDisplacementVector3();
                Vector3 translate = translate1 + translate2;

                // If there is a mouse translate with a modifier key and it is held down, do not reset the hand position.
                bool handTranslateActive =
                    (LeftHandPrimaryAxisControl.axisType == AxisController.AxisType.Mouse && LeftHandPrimaryAxisControl.buttonType != ButtonController.ButtonType.None && LeftHandPrimaryAxisControl.ShouldControl()) ||
                    (LeftHandSecondaryAxisControl.axisType == AxisController.AxisType.Mouse && LeftHandSecondaryAxisControl.buttonType != ButtonController.ButtonType.None && LeftHandSecondaryAxisControl.ShouldControl());

                if (handTranslateActive || LeftHandSourceState.Pressed)
                {
                    timeBeforeReturn = HandTimeBeforeReturn;
                    LeftHandInView = true;
                }

                leftHandLocalPosition += translate;
                LeftHandSourceState.Properties.Location.Position = Camera.main.transform.position + Camera.main.transform.TransformVector(leftHandLocalPosition);

                LeftHandVisualizer.transform.position = LeftHandSourceState.Properties.Location.Position;
                LeftHandVisualizer.transform.forward = Camera.main.transform.forward;

                leftHandVisualPropertyBlock.SetTexture(mainTexID, LeftHandSourceState.Pressed ? HandDownTexture : HandUpTexture);
                leftHandVisualRenderer.SetPropertyBlock(leftHandVisualPropertyBlock);

                LeftHandSourceState.Properties.Location.TryGetFunctionsReturnsTrue = LeftHandInView;
            }
            else
            {
                LeftHandSourceState.Properties.Location.TryGetFunctionsReturnsTrue = false;
            }

            if (appHasFocus)
            {
                Vector3 translate1 = RightHandPrimaryAxisControl.GetDisplacementVector3();
                Vector3 translate2 = RightHandSecondaryAxisControl.GetDisplacementVector3();
                Vector3 translate = translate1 + translate2;


                bool handTranslateActive =
                    (RightHandPrimaryAxisControl.axisType == AxisController.AxisType.Mouse && RightHandPrimaryAxisControl.buttonType != ButtonController.ButtonType.None && RightHandPrimaryAxisControl.ShouldControl()) ||
                    (RightHandSecondaryAxisControl.axisType == AxisController.AxisType.Mouse && RightHandSecondaryAxisControl.buttonType != ButtonController.ButtonType.None && RightHandSecondaryAxisControl.ShouldControl());

                // If there is a mouse translate with a modifier key and it is held down, do not reset the hand position.
                if (handTranslateActive || RightHandSourceState.Pressed)
                {
                    timeBeforeReturn = HandTimeBeforeReturn;
                    RightHandInView = true;
                }

                rightHandLocalPosition += translate;
                RightHandSourceState.Properties.Location.Position = Camera.main.transform.position + Camera.main.transform.TransformVector(rightHandLocalPosition);

                RightHandVisualizer.transform.position = RightHandSourceState.Properties.Location.Position;
                RightHandVisualizer.transform.forward = Camera.main.transform.forward;
                rightHandVisualPropertyBlock.SetTexture(mainTexID, RightHandSourceState.Pressed ? HandDownTexture : HandUpTexture);
                rightHandVisualRenderer.SetPropertyBlock(rightHandVisualPropertyBlock);

                RightHandSourceState.Properties.Location.TryGetFunctionsReturnsTrue = RightHandInView;
            }
            else
            {
                RightHandSourceState.Properties.Location.TryGetFunctionsReturnsTrue = false;
            }
        }

        private void UpdateHandVisualization()
        {
            leftHandVisualPropertyBlock.SetColor("_Color", LeftHandInView ? ActiveHandColor : DroppedHandColor);
            rightHandVisualPropertyBlock.SetColor("_Color", RightHandInView ? ActiveHandColor : DroppedHandColor);

            if (LeftHandVisualizer.activeSelf != VisualizeHands)
            {
                LeftHandVisualizer.SetActive(VisualizeHands);
            }
            if (RightHandVisualizer.activeSelf != VisualizeHands)
            {
                RightHandVisualizer.SetActive(VisualizeHands);
            }
        }
    }

}

