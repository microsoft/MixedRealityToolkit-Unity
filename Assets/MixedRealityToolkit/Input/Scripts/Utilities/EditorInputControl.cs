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
        public bool pressed;
        public bool grasped;
        public bool menuPressed;
        public bool selectPressed;
        public DebugInteractionSourcePose sourcePose;
    }

    /// <summary>
    /// Since the InteractionSourcePose is internal to UnityEngine.VR.WSA.Input,
    /// this is a fake InteractionSourcePose structure to keep the test code consistent.
    /// </summary>
    public class DebugInteractionSourcePose
    {
        /// <summary>
        /// In the typical InteractionSourcePose, the hardware determines if
        /// TryGetPosition and TryGetVelocity will return true or not. Here
        /// we manually emulate this state with TryGetFunctionsReturnTrue.
        /// </summary>
        public bool TryGetFunctionsReturnTrue;
        public bool IsPositionAvailable;
        public bool IsRotationAvailable;

        public Vector3 Position;
        public Vector3 Velocity;
        public Quaternion Rotation;
        public Ray? PointerRay;

        public void Awake()
        {
            TryGetFunctionsReturnTrue = false;
            IsPositionAvailable = false;
            IsRotationAvailable = false;
            Position = new Vector3(0, 0, 0);
            Velocity = new Vector3(0, 0, 0);
            Rotation = Quaternion.identity;
        }

        public bool TryGetPosition(out Vector3 position)
        {
            position = Position;
            if (!TryGetFunctionsReturnTrue)
            {
                return false;
            }
            return true;
        }

        public bool TryGetVelocity(out Vector3 velocity)
        {
            velocity = Velocity;
            if (!TryGetFunctionsReturnTrue)
            {
                return false;
            }
            return true;
        }

        public bool TryGetRotation(out Quaternion rotation)
        {
            rotation = Rotation;
            if (!TryGetFunctionsReturnTrue || !IsRotationAvailable)
            {
                return false;
            }
            return true;
        }

        public bool TryGetPointerRay(out Ray pointerRay)
        {
            pointerRay = (Ray)PointerRay;
            if (PointerRay == null)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Class for manually controlling inputs when running in the Unity editor.
    /// </summary>
    public class EditorInputControl : MonoBehaviour
    {
        public float ControllerReturnFactor = 0.25f;  /// [0.0,1.0] the closer this is to one the faster it brings the hand back to center
        public float ControllerTimeBeforeReturn = 0.5f;
        public float MinimumTrackedMovement = 0.001f;

        [Tooltip("Use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        public bool UseUnscaledTime = true;

        public AxisController PrimaryAxisTranslateControl;
        public AxisController SecondaryAxisTranslateControl;
        public AxisController PrimaryAxisRotateControl;
        public AxisController SecondaryAxisRotateControl;
        public AxisController TertiaryAxisRotateControl;
        public ButtonController SelectButtonControl;
        public ButtonController MenuButtonControl;
        public ButtonController GraspControl;

        public DebugInteractionSourceState ControllerSourceState;

        public Color ActiveControllerColor;
        public Color DroppedControllerColor;

        /// <summary>
        /// Will place hand visualizations in the world space, only for debugging.
        /// Place the representative GameObjects in LeftHandVisualizer & RightHandVisualizer.
        /// </summary>
        public bool VisualizeController = true;
        public GameObject ControllerVisualizer;

        public Texture HandUpTexture;
        public Texture HandDownTexture;

        public bool ShowPointingRay;

        public bool ControllerInView { get; private set; }

        public Vector3 InitialPosition;

        private Vector3 LocalPosition;
        private Vector3 LocalRotation;

        private Renderer VisualRenderer;
        private MaterialPropertyBlock VisualPropertyBlock;
        private int mainTexID;

        private float timeBeforeReturn;

        private void Awake()
        {
#if !UNITY_EDITOR
            Destroy(gameObject);
#endif

            mainTexID = Shader.PropertyToID("_MainTex");

            ControllerSourceState.pressed = false;
            ControllerSourceState.grasped = false;
            ControllerSourceState.menuPressed = false;
            ControllerSourceState.selectPressed = false;
            ControllerSourceState.sourcePose = new DebugInteractionSourcePose();
            ControllerSourceState.sourcePose.IsPositionAvailable = false;
            ControllerSourceState.sourcePose.IsRotationAvailable = false;

            LocalPosition = ControllerVisualizer.transform.position;
            InitialPosition = LocalPosition;
            ControllerSourceState.sourcePose.Position = LocalPosition;
            ControllerSourceState.sourcePose.Rotation = ControllerVisualizer.transform.rotation;

            VisualRenderer = ControllerVisualizer.GetComponent<Renderer>();
            VisualPropertyBlock = new MaterialPropertyBlock();
            VisualRenderer.SetPropertyBlock(VisualPropertyBlock);
        }

        private void Update()
        {
            UpdateControllerVisualization();

            var deltaTime = UseUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            float smoothingFactor = deltaTime * 30.0f * ControllerReturnFactor;

            if (timeBeforeReturn > 0.0f)
            {
                timeBeforeReturn = Mathf.Clamp(timeBeforeReturn - deltaTime, 0.0f, ControllerTimeBeforeReturn);
            }

            ControllerSourceState.selectPressed = SelectButtonControl.Pressed();
            ControllerSourceState.pressed = ControllerSourceState.selectPressed;

            if (MenuButtonControl)
            {
                ControllerSourceState.menuPressed = MenuButtonControl.Pressed();
            }

            if (GraspControl)
            {
                ControllerSourceState.grasped = GraspControl.Pressed();
            }

            if (ControllerSourceState.pressed)
            {
                timeBeforeReturn = ControllerTimeBeforeReturn;
            }

            if (timeBeforeReturn <= 0.0f)
            {
                LocalPosition = Vector3.Slerp(LocalPosition, InitialPosition, smoothingFactor);
                if (LocalPosition == InitialPosition)
                {
                    ControllerInView = false;
                }
            }

            Vector3 translate = Vector3.zero;

            if (PrimaryAxisTranslateControl && SecondaryAxisTranslateControl)
            {
                translate = PrimaryAxisTranslateControl.GetDisplacementVector3() +
                    SecondaryAxisTranslateControl.GetDisplacementVector3();

                ControllerSourceState.sourcePose.IsPositionAvailable = true;
            }

            Vector3 rotate = Vector3.zero;

            if (PrimaryAxisRotateControl && SecondaryAxisRotateControl && TertiaryAxisRotateControl)
            {
                rotate = PrimaryAxisRotateControl.GetDisplacementVector3() +
                    SecondaryAxisRotateControl.GetDisplacementVector3() +
                    TertiaryAxisRotateControl.GetDisplacementVector3();

                if ((PrimaryAxisRotateControl.axisType != AxisController.AxisType.None && PrimaryAxisRotateControl.ShouldControl()) ||
                    (SecondaryAxisRotateControl.axisType != AxisController.AxisType.None && SecondaryAxisRotateControl.ShouldControl()) ||
                    (TertiaryAxisRotateControl.axisType != AxisController.AxisType.None && TertiaryAxisRotateControl.ShouldControl()))
                {
                    ControllerSourceState.sourcePose.IsRotationAvailable = true;
                    LocalRotation += rotate;
                }
            }

            // If there is a mouse translate with a modifier key and it is held down, do not reset the controller position.
            bool controllerTranslateActive =
                (PrimaryAxisTranslateControl.axisType == AxisController.AxisType.Mouse && PrimaryAxisTranslateControl.buttonType != ButtonController.ButtonType.None && PrimaryAxisTranslateControl.ShouldControl()) ||
                (SecondaryAxisTranslateControl.axisType == AxisController.AxisType.Mouse && SecondaryAxisTranslateControl.buttonType != ButtonController.ButtonType.None && SecondaryAxisTranslateControl.ShouldControl());

            if (controllerTranslateActive ||
                ControllerSourceState.selectPressed ||
                ControllerSourceState.menuPressed ||
                ControllerSourceState.grasped ||
                ControllerSourceState.sourcePose.IsRotationAvailable)
            {
                timeBeforeReturn = ControllerTimeBeforeReturn;
                ControllerInView = true;
            }

            LocalPosition += translate;
            ControllerSourceState.sourcePose.Position = Camera.main.transform.position + Camera.main.transform.TransformVector(LocalPosition);

            ControllerVisualizer.transform.position = ControllerSourceState.sourcePose.Position;
            ControllerVisualizer.transform.forward = Camera.main.transform.forward;

            ControllerVisualizer.transform.Rotate(LocalRotation);

            ControllerSourceState.sourcePose.Rotation = ControllerVisualizer.transform.rotation;

            VisualPropertyBlock.SetTexture(mainTexID, ControllerSourceState.pressed ? HandDownTexture : HandUpTexture);
            VisualRenderer.SetPropertyBlock(VisualPropertyBlock);

            ControllerSourceState.sourcePose.TryGetFunctionsReturnTrue = ControllerInView;

            if (ControllerInView && ControllerSourceState.sourcePose.IsRotationAvailable && ControllerSourceState.sourcePose.IsPositionAvailable)
            {
                // Draw ray
                Vector3 up = ControllerVisualizer.transform.TransformDirection(Vector3.up);
                ControllerSourceState.sourcePose.PointerRay = new Ray(ControllerVisualizer.transform.position, up);

                Ray newRay;
                if (ControllerSourceState.sourcePose.TryGetPointerRay(out newRay))
                {
                    if (ShowPointingRay && Physics.Raycast(newRay))
                    {
                        // TODO shanama: get pretty ray here, maybe an "active" ray and an "inactive" ray for when buttons are pressed
                        Debug.DrawRay(newRay.origin, newRay.direction, Color.cyan);
                    }
                }
            }
            else
            {
                ControllerSourceState.sourcePose.PointerRay = null;
            }
        }

        private void UpdateControllerVisualization()
        {
             VisualRenderer.material.SetColor("_Color", ControllerInView ? ActiveControllerColor : DroppedControllerColor);

            if (ControllerVisualizer.activeSelf != VisualizeController)
            {
                ControllerVisualizer.SetActive(VisualizeController);
            }
        }
    }

}