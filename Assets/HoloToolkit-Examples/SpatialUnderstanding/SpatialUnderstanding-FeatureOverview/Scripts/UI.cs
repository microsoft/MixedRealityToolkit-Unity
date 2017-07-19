// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace HoloToolkit.Examples.SpatialUnderstandingFeatureOverview
{
    public class UI : LineDrawer
    {
        // Consts
        public const float MenuWidth = 1.5f;
        public const float MenuHeight = 1.0f;
        public const float MenuMinDepth = 2.0f;

        // Enums
        public enum Panels
        {
            Topology,
            Shapes,
            LevelSolver,
            PANEL_COUNT
        }
        [Serializable]
        public class TabPanel
        {
            public Button Button;
            public Image ButtonImage;
            public Image Background;
            public GridLayoutGroup ButtonGrid;
            public List<Button> GridButtons = new List<Button>();
        }

        // Config
        public Canvas ParentCanvas;
        public TabPanel[] ButtonPanels = new TabPanel[(int)Panels.PANEL_COUNT];
        public Button PrefabButton;
        public LayerMask UILayerMask;

        // Properties
        public bool HasPlacedMenu { get; private set; }
        public AnimatedBox MenuAnimatedBox { get; private set; }
        public Panels ActivePanel { get; private set; }

        // Privates
        private DateTime timeLastQuery = DateTime.MinValue;
        private bool placedMenuNeedsBillboard = false;

        // Functions
        private void Start()
        {
            // Turn menu off until we're placed
            ParentCanvas.gameObject.SetActive(false);

            // Events
            SpatialUnderstanding.Instance.ScanStateChanged += OnScanStateChanged;
        }

        protected override void OnDestroy()
        {
            if (SpatialUnderstanding.Instance != null)
            {
                SpatialUnderstanding.Instance.ScanStateChanged -= OnScanStateChanged;
            }

            base.OnDestroy();
        }

        private void OnScanStateChanged()
        {
            // If we are leaving the None state, go ahead and register shapes now
            if ((SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done) &&
                SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                // Make sure we've created our shapes
                ShapeDefinition.Instance.CreateShapes();

                // Make sure our solver is initialized
                LevelSolver.Instance.InitializeSolver();

                // Setup the menu
                StartCoroutine(SetupMenu());
            }
        }

        private IEnumerator SetupMenu()
        {
            // Setup for queries
            SpatialUnderstandingDllTopology.TopologyResult[] resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[1];
            IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);

            // Place on a wall (do it in a thread, as it can take a little while)
            SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placeOnWallDef = 
                SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnWall(new Vector3(MenuWidth * 0.5f, MenuHeight * 0.5f, MenuMinDepth * 0.5f), 0.5f, 3.0f);
            SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult placementResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResult();

            var thread =
#if UNITY_EDITOR || !UNITY_WSA
                new System.Threading.Thread
#else
                System.Threading.Tasks.Task.Run
#endif
            (() => {
                if (SpatialUnderstandingDllObjectPlacement.Solver_PlaceObject(
                    "UIPlacement",
                    SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placeOnWallDef),
                    0,
                    IntPtr.Zero,
                    0,
                    IntPtr.Zero,
                    SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResultPtr()) == 0)
                {
                    placementResult = null;
                }
            });

#if UNITY_EDITOR || !UNITY_WSA
            thread.Start();
#endif

            while
                (
#if UNITY_EDITOR || !UNITY_WSA
                !thread.Join(TimeSpan.Zero)
#else
                !thread.IsCompleted
#endif
                )
            {
                yield return null;
            }
            if (placementResult != null)
            {
                Debug.Log("PlaceMenu - ObjectSolver-OnWall");
                Vector3 posOnWall = placementResult.Position - placementResult.Forward * MenuMinDepth * 0.5f;
                PlaceMenu(posOnWall, -placementResult.Forward);
                yield break;
            }

            // Wait a frame
            yield return null;

            // Fallback, place floor (add a facing, if so)
            int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindLargestPositionsOnFloor(
                resultsTopology.Length, resultsTopologyPtr);
            if (locationCount > 0)
            {
                Debug.Log("PlaceMenu - LargestPositionsOnFloor");
                SpatialUnderstandingDllTopology.TopologyResult menuLocation = resultsTopology[0];
                Vector3 menuPosition = menuLocation.position + Vector3.up * MenuHeight;
                Vector3 menuLookVector = Camera.main.transform.position - menuPosition;
                PlaceMenu(menuPosition, (new Vector3(menuLookVector.x, 0.0f, menuLookVector.z)).normalized, true);
                yield break;
            }

            // Final fallback just in front of the user
            SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());
            SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();
            Vector3 defaultPosition = Camera.main.transform.position + Camera.main.transform.forward * 2.0f;
            PlaceMenu(new Vector3(defaultPosition.x, Math.Max(defaultPosition.y, alignment.FloorYValue + 1.5f), defaultPosition.z), (new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z)).normalized, true);
            Debug.Log("PlaceMenu - InFrontOfUser");
        }

        private void SetActiveTab(Panels panel)
        {
            // Set it
            ActivePanel = panel;
            timeLastQuery = DateTime.MinValue;

            // Colors
            Update_Colors();
        }

        private void Update_Colors()
        {
            const float TimeToFadeAfterQuery = 3.0f;

            // Time since query (fade for a bit after a query)
            float timeSinceQuery = (float)(DateTime.Now - timeLastQuery).TotalSeconds;
            float alphaScale = Mathf.SmoothStep(0.0f, 1.0f, Mathf.Clamp01(timeSinceQuery - TimeToFadeAfterQuery)) * 0.8f + 0.2f;

            // Colors
            Color colorButtonActive = new Color(1.0f, 1.0f, 1.0f, 0.8f * alphaScale);
            Color colorButtonInactive = new Color(1.0f, 1.0f, 1.0f, 0.25f * alphaScale);
            Color colorPanelActive = new Color(1.0f, 1.0f, 1.0f, 0.6f * alphaScale);
            Color colorPanelInactive = new Color(1.0f, 1.0f, 1.0f, 0.15f * alphaScale);

            // Colors on buttons
            for (int i = 0; i < (int)Panels.PANEL_COUNT; ++i)
            {
                bool isEnabled = (i == (int)ActivePanel);

                ButtonPanels[i].ButtonImage.color = isEnabled ? colorButtonActive : colorButtonInactive;
                ButtonPanels[i].Background.enabled = isEnabled;
                ButtonPanels[i].Background.color = isEnabled ? colorPanelActive : colorPanelInactive;
                ButtonPanels[i].ButtonGrid.enabled = isEnabled;

                for (int j = 0; j < ButtonPanels[i].GridButtons.Count; ++j)
                {
                    ButtonPanels[i].GridButtons[j].gameObject.SetActive(isEnabled);
                }
            }
        }

        private void SetupMenus()
        {
            // Topology queries
            ButtonPanels[(int)Panels.Topology].Button.GetComponentInChildren<Text>().text = "Topology Queries";
            ButtonPanels[(int)Panels.Topology].Button.onClick.AddListener(() => { SetActiveTab(Panels.Topology); });
            AddButton("Position on wall", Panels.Topology, () => { SpaceVisualizer.Instance.Query_Topology_FindPositionOnWall(); timeLastQuery = DateTime.MinValue; });
            AddButton("Large positions on wall", Panels.Topology, () => { SpaceVisualizer.Instance.Query_Topology_FindLargePositionsOnWalls(); timeLastQuery = DateTime.MinValue; });
            AddButton("Largest wall", Panels.Topology, () => { SpaceVisualizer.Instance.Query_Topology_FindLargeWall(); timeLastQuery = DateTime.MinValue; });
            AddButton("Positions on floor", Panels.Topology, () => { SpaceVisualizer.Instance.Query_Topology_FindPositionsOnFloor(); timeLastQuery = DateTime.MinValue; });
            AddButton("Large positions on floor", Panels.Topology, () => { SpaceVisualizer.Instance.Query_Topology_FindLargestPositionsOnFloor(); timeLastQuery = DateTime.MinValue; });
            AddButton("Place objects positions", Panels.Topology, () => { SpaceVisualizer.Instance.Query_Topology_FindPositionsPlaceable(); timeLastQuery = DateTime.MinValue; });

            // Shape queries
            ButtonPanels[(int)Panels.Shapes].Button.GetComponentInChildren<Text>().text = "Shape Queries";
            ButtonPanels[(int)Panels.Shapes].Button.onClick.AddListener(() => { SetActiveTab(Panels.Shapes); });
            ReadOnlyCollection<string> customShapes = ShapeDefinition.Instance.CustomShapeDefinitions;
            for (int i = 0; i < customShapes.Count; ++i)
            {
                string shapeName = customShapes[i];
                AddButton(shapeName, Panels.Shapes, () =>
                {
                    SpaceVisualizer.Instance.Query_Shape_FindShapeHalfDims(shapeName);
                    timeLastQuery = DateTime.MinValue;
                });
            }

            // Level solver
            ButtonPanels[(int)Panels.LevelSolver].Button.GetComponentInChildren<Text>().text = "Object Placement";
            ButtonPanels[(int)Panels.LevelSolver].Button.onClick.AddListener(() => { SetActiveTab(Panels.LevelSolver); timeLastQuery = DateTime.MinValue; });
            AddButton("On Floor", Panels.LevelSolver, () => { LevelSolver.Instance.Query_OnFloor(); timeLastQuery = DateTime.MinValue; });
            AddButton("On Wall", Panels.LevelSolver, () => { LevelSolver.Instance.Query_OnWall(); timeLastQuery = DateTime.MinValue; });
            AddButton("On Ceiling", Panels.LevelSolver, () => { LevelSolver.Instance.Query_OnCeiling(); timeLastQuery = DateTime.MinValue; });
            AddButton("On SurfaceEdge", Panels.LevelSolver, () => { LevelSolver.Instance.Query_OnEdge(); timeLastQuery = DateTime.MinValue; });
            AddButton("On FloorAndCeiling", Panels.LevelSolver, () => { LevelSolver.Instance.Query_OnFloorAndCeiling(); timeLastQuery = DateTime.MinValue; });
            AddButton("RandomInAir AwayFromMe", Panels.LevelSolver, () => { LevelSolver.Instance.Query_RandomInAir_AwayFromMe(); timeLastQuery = DateTime.MinValue; });
            AddButton("OnEdge NearCenter", Panels.LevelSolver, () => { LevelSolver.Instance.Query_OnEdge_NearCenter(); timeLastQuery = DateTime.MinValue; });
            AddButton("OnFloor AwayFromMe", Panels.LevelSolver, () => { LevelSolver.Instance.Query_OnFloor_AwayFromMe(); timeLastQuery = DateTime.MinValue; });
            AddButton("OnFloor NearMe", Panels.LevelSolver, () => { LevelSolver.Instance.Query_OnFloor_NearMe(); timeLastQuery = DateTime.MinValue; });

            // Default one of them active
            SetActiveTab(Panels.Topology);
        }

        private void AddButton(string text, Panels panel, UnityEngine.Events.UnityAction action)
        {
            Button button = Instantiate(PrefabButton);
            button.GetComponentInChildren<Text>().text = text;
            button.transform.SetParent(ButtonPanels[(int)panel].ButtonGrid.transform, false);
            button.transform.localScale = Vector3.one;
            button.onClick.AddListener(action);

            ButtonPanels[(int)panel].GridButtons.Add(button);
        }

        private void PlaceMenu(Vector3 position, Vector3 normal, bool needsBillboarding = false)
        {
            // Offset in a bit
            position -= normal * 0.05f;
            Quaternion rotation = Quaternion.LookRotation(normal, Vector3.up);

            // Place it
            transform.position = position;
            transform.rotation = rotation;

            // Setup the menu
            SetupMenus();

            // Enable it
            ParentCanvas.gameObject.SetActive(true);

            // Create up a box
            MenuAnimatedBox = new AnimatedBox(0.0f, position, rotation, new Color(1.0f, 1.0f, 1.0f, 0.25f), new Vector3(MenuWidth * 0.5f, MenuHeight * 0.5f, 0.025f), LineDrawer.DefaultLineWidth * 0.5f);

            // Initial position
            transform.position = MenuAnimatedBox.AnimPosition.Evaluate(MenuAnimatedBox.Time);
            transform.rotation = MenuAnimatedBox.Rotation * Quaternion.AngleAxis(360.0f * MenuAnimatedBox.AnimRotation.Evaluate(MenuAnimatedBox.Time), Vector3.up);

            // Billboarding (note that because of the transition animation we need to place this late)
            placedMenuNeedsBillboard = needsBillboarding;

            // And mark that we've done it
            HasPlacedMenu = true;
        }

        private void Update()
        {
            Update_Colors();

            // Animated box
            if (MenuAnimatedBox != null)
            {
                // We're using the animated box for the animation only
                MenuAnimatedBox.Update(Time.deltaTime);

                // Billboarding
                if (MenuAnimatedBox.IsAnimationComplete &&
                    placedMenuNeedsBillboard)
                {
                    // Rotate to face the user
                    transform.position = MenuAnimatedBox.AnimPosition.Evaluate(MenuAnimatedBox.Time);
                    Vector3 lookDirTarget = Camera.main.transform.position - transform.position;
                    lookDirTarget = (new Vector3(lookDirTarget.x, 0.0f, lookDirTarget.z)).normalized;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-lookDirTarget), Time.deltaTime * 10.0f);
                }
                else
                {
                    // Keep the UI locked to the animated box
                    transform.position = MenuAnimatedBox.AnimPosition.Evaluate(MenuAnimatedBox.Time);
                    transform.rotation = MenuAnimatedBox.Rotation * Quaternion.AngleAxis(360.0f * MenuAnimatedBox.AnimRotation.Evaluate(MenuAnimatedBox.Time), Vector3.up);
                }
            }
        }
    }
}
