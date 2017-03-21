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
        public bool IsGrasped;
        public bool IsMenuPressed;
        public bool IsSelectPressed;
        public DebugInteractionSourceProperties Properties;
        public Ray? PointingRay;
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
        public bool IsPositionAvailable;
        public bool IsOrientationAvailable;

        public Vector3 Position;
        public Vector3 Velocity;
        public Quaternion Orientation;

        public void Awake()
        {
            TryGetFunctionsReturnsTrue = false;
            IsPositionAvailable = false;
            IsOrientationAvailable = false;
            Position = new Vector3(0, 0, 0);
            Velocity = new Vector3(0, 0, 0);
            Orientation = Quaternion.identity;
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

        public bool TryGetOrientation(out Quaternion orientation)
        {
            orientation = Orientation;
            if (!TryGetFunctionsReturnsTrue || !IsOrientationAvailable)
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

            ControllerSourceState.Pressed = false;
            ControllerSourceState.IsGrasped = false;
            ControllerSourceState.IsMenuPressed = false;
            ControllerSourceState.IsSelectPressed = false;
            ControllerSourceState.Properties.Location = new DebugInteractionSourceLocation();
            ControllerSourceState.Properties.Location.IsPositionAvailable = false;
            ControllerSourceState.Properties.Location.IsOrientationAvailable = false;

            LocalPosition = ControllerVisualizer.transform.position;
            InitialPosition = LocalPosition;
            ControllerSourceState.Properties.Location.Position = LocalPosition;
            ControllerSourceState.Properties.Location.Orientation = ControllerVisualizer.transform.rotation;

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

            ControllerSourceState.IsSelectPressed = SelectButtonControl.Pressed();
            ControllerSourceState.Pressed = ControllerSourceState.IsSelectPressed;

            if (MenuButtonControl)
            {
                ControllerSourceState.IsMenuPressed = MenuButtonControl.Pressed();
            }

            if (GraspControl)
            {
                ControllerSourceState.IsGrasped = GraspControl.Pressed();
            }

            if (ControllerSourceState.Pressed)
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

                ControllerSourceState.Properties.Location.IsPositionAvailable = true;
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
                    ControllerSourceState.Properties.Location.IsOrientationAvailable = true;
                    LocalRotation += rotate;
                }
            }

            // If there is a mouse translate with a modifier key and it is held down, do not reset the controller position.
            bool controllerTranslateActive =
                (PrimaryAxisTranslateControl.axisType == AxisController.AxisType.Mouse && PrimaryAxisTranslateControl.buttonType != ButtonController.ButtonType.None && PrimaryAxisTranslateControl.ShouldControl()) ||
                (SecondaryAxisTranslateControl.axisType == AxisController.AxisType.Mouse && SecondaryAxisTranslateControl.buttonType != ButtonController.ButtonType.None && SecondaryAxisTranslateControl.ShouldControl());

            if (controllerTranslateActive ||
                ControllerSourceState.IsSelectPressed ||
                ControllerSourceState.IsMenuPressed ||
                ControllerSourceState.IsGrasped ||
                ControllerSourceState.Properties.Location.IsOrientationAvailable)
            {
                timeBeforeReturn = ControllerTimeBeforeReturn;
                ControllerInView = true;
            }

            LocalPosition += translate;
            ControllerSourceState.Properties.Location.Position = Camera.main.transform.position + Camera.main.transform.TransformVector(LocalPosition);

            ControllerVisualizer.transform.position = ControllerSourceState.Properties.Location.Position;
            ControllerVisualizer.transform.forward = Camera.main.transform.forward;

            ControllerVisualizer.transform.Rotate(LocalRotation);

            ControllerSourceState.Properties.Location.Orientation = ControllerVisualizer.transform.rotation;

            VisualPropertyBlock.SetTexture(mainTexID, ControllerSourceState.Pressed ? HandDownTexture : HandUpTexture);
            VisualRenderer.SetPropertyBlock(VisualPropertyBlock);

            ControllerSourceState.Properties.Location.TryGetFunctionsReturnsTrue = ControllerInView;

            if (ShowPointingRay && ControllerInView && ControllerSourceState.Properties.Location.IsOrientationAvailable && ControllerSourceState.Properties.Location.IsPositionAvailable)
            {
                // Draw ray
                Vector3 up = ControllerVisualizer.transform.TransformDirection(Vector3.up);
                ControllerSourceState.PointingRay = new Ray(ControllerVisualizer.transform.position, up);

                RaycastHit hit;
                if (Physics.Raycast(ControllerVisualizer.transform.position, up, out hit))
                {
                    // todo shanama: get pretty ray here, maybe an "active" ray and an "inactive" ray for when buttons are pressedd
                    Debug.DrawRay(((Ray)ControllerSourceState.PointingRay).origin, up, Color.cyan);
                }
            }
            else
            {
                ControllerSourceState.PointingRay = null;
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