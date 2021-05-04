// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public class MixedRealitySubsystemManagementWindow : EditorWindow
    {
        private static MixedRealitySubsystemManagementWindow subsystemManagementWindow;

        private SerializedObject configProfile;
        private MixedRealityToolkitConfigurationProfileInspector parentInspector;

        private struct SubsystemEntry
        {
            public SubsystemEntry(SubsystemProfile type, string description)
            {
                this.type = type;
                this.description = description;
            }

            public SubsystemProfile type;
            public string description;
        };

        private List<SubsystemEntry> availableSubsystems = new List<SubsystemEntry>()
        {
            new SubsystemEntry(SubsystemProfile.Camera, "MRTK Camera Subsystem"),
            new SubsystemEntry(SubsystemProfile.Input, "MRTK Input Subsystem"),
            new SubsystemEntry(SubsystemProfile.Boundary, "MRTK Boundary Subsystem"),
            new SubsystemEntry(SubsystemProfile.Teleport, "MRTK Teleport Subsystem"),
            new SubsystemEntry(SubsystemProfile.SpatialAwareness, "MRTK Spatial Awareness Subsystem"),
            new SubsystemEntry(SubsystemProfile.Diagnostics, "MRTK Diagnostics Subsystem"),
            new SubsystemEntry(SubsystemProfile.SceneSystem, "MRTK Scene System Subsystem"),
            new SubsystemEntry(SubsystemProfile.Extensions, "MRTK Extensions Subsystem")
        };

        public static void OpenWindow(SerializedObject configProfile, MixedRealityToolkitConfigurationProfileInspector parentInspector)
        {
            if (subsystemManagementWindow != null)
            {
                subsystemManagementWindow.Close();
            }

            subsystemManagementWindow = GetWindow<MixedRealitySubsystemManagementWindow>(true, "Add Service", true);
            subsystemManagementWindow.Initialize(configProfile, parentInspector);
            subsystemManagementWindow.Show(true);
        }

        private void Initialize(SerializedObject configProfile, MixedRealityToolkitConfigurationProfileInspector parentInspector)
        {
            this.configProfile = configProfile;
            this.parentInspector = parentInspector;
            // Find all the serialized properties for sub-profiles
        }

        private void OnGUI()
        {
            if (subsystemManagementWindow == null || configProfile == null)
            {
                Close();
                return;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            string[] subsystemNames = availableSubsystems.Select(x => ObjectNames.NicifyVariableName(x.type.ToString())).ToArray();

            int prefsSelectedSubsystemTab = SessionState.GetInt("SelectdSubsystemTab", 0);
            int selectedSubsystem = GUILayout.SelectionGrid(prefsSelectedSubsystemTab, subsystemNames, 1, GUILayout.MaxWidth(125));
            if (selectedSubsystem != prefsSelectedSubsystemTab)
            {
                SessionState.SetInt("SelectdSubsystemTab", selectedSubsystem);
            }
            EditorGUILayout.EndVertical();

            SubsystemEntry selectedEntry = availableSubsystems[selectedSubsystem];

            EditorGUILayout.HelpBox(selectedEntry.description, MessageType.Info);
            EditorGUILayout.EndHorizontal();

            if(parentInspector.GetSubsystemStatus(selectedEntry.type))
            {
                if (GUILayout.Button(string.Format("Remove {0} subsystem", subsystemNames[selectedSubsystem])))
                {
                    parentInspector.RemoveSubsystem(selectedEntry.type);
                }
            }
            else
            {
                if (GUILayout.Button(string.Format("Add {0} subsystem", subsystemNames[selectedSubsystem])))
                {
                    parentInspector.AddSubsystem(selectedEntry.type);
                }
            }

            if (GUILayout.Button("Close"))
            {
                subsystemManagementWindow.Close();
            }


            Repaint();
        }
    }
}