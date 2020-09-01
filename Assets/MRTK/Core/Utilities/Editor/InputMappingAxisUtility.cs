// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    /// <summary>
    /// Utility class for Unity's Input Manager mappings.
    /// </summary>
    public static class InputMappingAxisUtility
    {
        #region Configuration elements

        /// <summary>
        /// This is used to keep a local list of axis names, so we don't have to keep iterating through each SerializedProperty.
        /// </summary>
        private static readonly List<string> AxisNames = new List<string>();

        /// <summary>
        /// This is used to keep a single reference to InputManager.asset, refreshed when necessary.
        /// </summary>
        private static SerializedObject inputManagerAsset;

        #endregion Configuration elements

        #region Mappings Functions

        /// <summary>
        /// Simple static function to check Unity InputManager Axis configuration, and apply if needed.
        /// </summary>
        /// <remarks>
        /// This only exists as the Unity input manager CANNOT map Axis to an id; it has to be through a mapping.
        /// </remarks>
        /// <param name="axisMappings">Array of axis mappings, to configure your own custom set.</param>
        /// <param name="updateMappings">If the mappings should be updated to match axisMappings or simply check that they match. Defaults to true.</param>
        /// <returns>True if the mappings needed an update. False if they match axisMappings already.</returns>
        public static bool CheckUnityInputManagerMappings(InputManagerAxis[] axisMappings, bool updateMappings = true)
        {
            EnsureInputManagerReference();

            bool mappingsNeedUpdate = false;

            if (axisMappings != null)
            {
                for (var i = 0; i < axisMappings.Length; i++)
                {
                    if (!DoesAxisNameExist(axisMappings[i].Name))
                    {
                        if (updateMappings)
                        {
                            AddAxis(axisMappings[i]);
                        }
                        mappingsNeedUpdate = true;
                    }
                }

                if (mappingsNeedUpdate && updateMappings)
                {
                    inputManagerAsset.ApplyModifiedProperties();
                }
            }

            return mappingsNeedUpdate;
        }

        /// <summary>
        /// Simple static function to apply Unity InputManager Axis configuration
        /// </summary>
        /// <remarks>
        /// This only exists as the Unity input manager CANNOT map Axis to an id, it has to be through a mapping
        /// </remarks>
        /// <param name="axisMappings">Optional array of Axis Mappings, to configure your own custom set</param>
        public static void RemoveMappings(InputManagerAxis[] axisMappings)
        {
            EnsureInputManagerReference();

            if (axisMappings != null)
            {
                foreach (InputManagerAxis axis in axisMappings)
                {
                    if (DoesAxisNameExist(axis.Name))
                    {
                        RemoveAxis(axis.Name);
                    }
                }
            }

            inputManagerAsset.ApplyModifiedProperties();
        }

        private static void AddAxis(InputManagerAxis axis)
        {
            EnsureInputManagerReference();

            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            // Creates a new axis by incrementing the size of the m_Axes array.
            axesProperty.arraySize++;

            // Get the new axis be querying for the last array element.
            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            // Iterate through all the properties of the new axis.
            while (axisProperty.Next(true))
            {
                switch (axisProperty.name)
                {
                    case "m_Name":
                        axisProperty.stringValue = axis.Name;
                        break;
                    case "descriptiveName":
                        axisProperty.stringValue = axis.DescriptiveName;
                        break;
                    case "descriptiveNegativeName":
                        axisProperty.stringValue = axis.DescriptiveNegativeName;
                        break;
                    case "negativeButton":
                        axisProperty.stringValue = axis.NegativeButton;
                        break;
                    case "positiveButton":
                        axisProperty.stringValue = axis.PositiveButton;
                        break;
                    case "altNegativeButton":
                        axisProperty.stringValue = axis.AltNegativeButton;
                        break;
                    case "altPositiveButton":
                        axisProperty.stringValue = axis.AltPositiveButton;
                        break;
                    case "gravity":
                        axisProperty.floatValue = axis.Gravity;
                        break;
                    case "dead":
                        axisProperty.floatValue = axis.Dead;
                        break;
                    case "sensitivity":
                        axisProperty.floatValue = axis.Sensitivity;
                        break;
                    case "snap":
                        axisProperty.boolValue = axis.Snap;
                        break;
                    case "invert":
                        axisProperty.boolValue = axis.Invert;
                        break;
                    case "type":
                        axisProperty.intValue = (int)axis.Type;
                        break;
                    case "axis":
                        axisProperty.intValue = axis.Axis - 1;
                        break;
                    case "joyNum":
                        axisProperty.intValue = axis.JoyNum;
                        break;
                }
            }
        }

        private static void RemoveAxis(string axis)
        {
            EnsureInputManagerReference();

            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            RefreshLocalAxesList();

            // This loop accounts for multiple axes with the same name.
            while (AxisNames.Contains(axis))
            {
                int index = AxisNames.IndexOf(axis);
                axesProperty.DeleteArrayElementAtIndex(index);
                AxisNames.RemoveAt(index);
            }
        }

        /// <summary>
        /// Checks our local cache of axis names to see if an axis exists. This cache is refreshed if it's empty or if InputManager.asset has been changed.
        /// </summary>
        public static bool DoesAxisNameExist(string axisName)
        {
            EnsureInputManagerReference();

            if (AxisNames.Count == 0 || inputManagerAsset.UpdateIfRequiredOrScript())
            {
                RefreshLocalAxesList();
            }

            return AxisNames.Contains(axisName);
        }

        /// <summary>
        /// Clears our local cache, then refills it by iterating through the m_Axes arrays and storing the display names.
        /// </summary>
        private static void RefreshLocalAxesList()
        {
            EnsureInputManagerReference();

            AxisNames.Clear();

            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                AxisNames.Add(axesProperty.GetArrayElementAtIndex(i).displayName);
            }
        }

        private static void EnsureInputManagerReference()
        {
            if (inputManagerAsset == null)
            {
                // Grabs the actual asset file into a SerializedObject, so we can iterate through it and edit it.
                inputManagerAsset = MixedRealityOptimizeUtils.GetSettingsObject("InputManager");
            }
        }

        #endregion
    }
}