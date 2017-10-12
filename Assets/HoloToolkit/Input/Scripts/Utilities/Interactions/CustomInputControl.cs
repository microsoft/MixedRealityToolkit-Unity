// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Class for manually controlling inputs when running in the Unity editor.
    /// </summary>
    public class CustomInputControl : MonoBehaviour
    {
        public float ControllerReturnFactor = 0.25f;  /// [0.0,1.0] the closer this is to one the faster it brings the hand back to center
        public float ControllerTimeBeforeReturn = 0.5f;

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

        private Vector3 localPosition;
        private Vector3 localRotation;

        private Renderer visualRenderer;
        private MaterialPropertyBlock visualPropertyBlock;
        private int mainTexId;

        private float timeBeforeReturn;

        private void Awake()
        {
            if (!Application.isEditor)
            {
                Destroy(gameObject);
            }

            mainTexId = Shader.PropertyToID("_MainTex");

            ControllerSourceState.Pressed = false;
            ControllerSourceState.Grasped = false;
            ControllerSourceState.MenuPressed = false;
            ControllerSourceState.SelectPressed = false;
            ControllerSourceState.SourcePose = new DebugInteractionSourcePose
            {
                IsPositionAvailable = false,
                IsRotationAvailable = false
            };

            localPosition = ControllerVisualizer.transform.position;
            InitialPosition = localPosition;
            ControllerSourceState.SourcePose.Position = localPosition;
            ControllerSourceState.SourcePose.Rotation = ControllerVisualizer.transform.rotation;

            visualRenderer = ControllerVisualizer.GetComponent<Renderer>();
            visualPropertyBlock = new MaterialPropertyBlock();
            visualRenderer.SetPropertyBlock(visualPropertyBlock);
        }

        private void Update()
        {
            UpdateControllerVisualization();

            float deltaTime = UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            float smoothingFactor = deltaTime * 30.0f * ControllerReturnFactor;

            if (timeBeforeReturn > 0.0f)
            {
                timeBeforeReturn = Mathf.Clamp(timeBeforeReturn - deltaTime, 0.0f, ControllerTimeBeforeReturn);
            }

            ControllerSourceState.SelectPressed = SelectButtonControl.Pressed();
            ControllerSourceState.Pressed = ControllerSourceState.SelectPressed;

            if (MenuButtonControl)
            {
                ControllerSourceState.MenuPressed = MenuButtonControl.Pressed();
            }

            if (GraspControl)
            {
                ControllerSourceState.Grasped = GraspControl.Pressed();
            }

            if (ControllerSourceState.Pressed)
            {
                timeBeforeReturn = ControllerTimeBeforeReturn;
            }

            if (timeBeforeReturn <= 0.0f)
            {
                localPosition = Vector3.Slerp(localPosition, InitialPosition, smoothingFactor);
                if (localPosition == InitialPosition)
                {
                    ControllerInView = false;
                }
            }

            Vector3 translate = Vector3.zero;

            if (PrimaryAxisTranslateControl && SecondaryAxisTranslateControl)
            {
                translate = PrimaryAxisTranslateControl.GetDisplacementVector3() +
                    SecondaryAxisTranslateControl.GetDisplacementVector3();

                ControllerSourceState.SourcePose.IsPositionAvailable = true;
            }

            if (PrimaryAxisRotateControl && SecondaryAxisRotateControl && TertiaryAxisRotateControl)
            {
                Vector3 rotate = PrimaryAxisRotateControl.GetDisplacementVector3() +
                                 SecondaryAxisRotateControl.GetDisplacementVector3() +
                                 TertiaryAxisRotateControl.GetDisplacementVector3();

                if ((PrimaryAxisRotateControl.axisType != AxisController.AxisType.None && PrimaryAxisRotateControl.ShouldControl()) ||
                    (SecondaryAxisRotateControl.axisType != AxisController.AxisType.None && SecondaryAxisRotateControl.ShouldControl()) ||
                    (TertiaryAxisRotateControl.axisType != AxisController.AxisType.None && TertiaryAxisRotateControl.ShouldControl()))
                {
                    ControllerSourceState.SourcePose.IsRotationAvailable = true;
                    localRotation += rotate;
                }
            }

            // If there is a mouse translate with a modifier key and it is held down, do not reset the controller position.
            bool controllerTranslateActive =
                (PrimaryAxisTranslateControl.axisType == AxisController.AxisType.Mouse &&
                 PrimaryAxisTranslateControl.buttonType != ButtonController.ButtonType.None &&
                 PrimaryAxisTranslateControl.ShouldControl()) ||
                (SecondaryAxisTranslateControl.axisType == AxisController.AxisType.Mouse &&
                 SecondaryAxisTranslateControl.buttonType != ButtonController.ButtonType.None &&
                 SecondaryAxisTranslateControl.ShouldControl());

            if (controllerTranslateActive ||
                ControllerSourceState.SelectPressed ||
                ControllerSourceState.MenuPressed ||
                ControllerSourceState.Grasped ||
                ControllerSourceState.SourcePose.IsRotationAvailable)
            {
                timeBeforeReturn = ControllerTimeBeforeReturn;
                ControllerInView = true;
            }

            localPosition += translate;
            ControllerSourceState.SourcePose.Position = CameraCache.Main.transform.position + CameraCache.Main.transform.TransformVector(localPosition);

            ControllerVisualizer.transform.position = ControllerSourceState.SourcePose.Position;
            ControllerVisualizer.transform.forward = CameraCache.Main.transform.forward;

            ControllerVisualizer.transform.Rotate(localRotation);

            ControllerSourceState.SourcePose.Rotation = ControllerVisualizer.transform.rotation;

            visualPropertyBlock.SetTexture(mainTexId, ControllerSourceState.Pressed ? HandDownTexture : HandUpTexture);
            visualRenderer.SetPropertyBlock(visualPropertyBlock);

            ControllerSourceState.SourcePose.TryGetFunctionsReturnTrue = ControllerInView;

            if (ControllerInView && ControllerSourceState.SourcePose.IsRotationAvailable && ControllerSourceState.SourcePose.IsPositionAvailable)
            {
                // Draw ray
                Vector3 up = ControllerVisualizer.transform.TransformDirection(Vector3.up);
                ControllerSourceState.SourcePose.PointerRay = new Ray(ControllerVisualizer.transform.position, up);

                Ray newRay;
                if (ControllerSourceState.SourcePose.TryGetPointerRay(out newRay))
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
                ControllerSourceState.SourcePose.PointerRay = null;
            }
        }

        private void UpdateControllerVisualization()
        {
            visualRenderer.material.SetColor("_Color", ControllerInView ? ActiveControllerColor : DroppedControllerColor);

            if (ControllerVisualizer.activeSelf != VisualizeController)
            {
                ControllerVisualizer.SetActive(VisualizeController);
            }
        }
    }
}
