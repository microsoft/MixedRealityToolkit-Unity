// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Editor
{
    /// <summary>
    /// Utility class for Unity's Input Manager Mappings.
    /// </summary>
    /// <remarks>
    /// Note, with any luck this will be temporary.  If it is to remain beyond Alpha, then this needs some refactoring to make a proper component.
    /// </remarks>
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
        /// This only exists as the Unity input manager CANNOT map Axis to an id, it has to be through a mapping
        /// </remarks>
        /// <param name="axisMappings">Optional array of Axis Mappings, to configure your own custom set</param>
        public static void CheckUnityInputManagerMappings(InputManagerAxis[] axisMappings)
        {
            AssureInputManagerReference();

            if (axisMappings != null)
            {
                for (var i = 0; i < axisMappings.Length; i++)
                {
                    if (!DoesAxisNameExist(axisMappings[i].Name))
                    {
                        AddAxis(axisMappings[i]);
                    }
                }

                inputManagerAsset.ApplyModifiedProperties();
            }
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
            AssureInputManagerReference();

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
            AssureInputManagerReference();

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
            AssureInputManagerReference();

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
            AssureInputManagerReference();

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
            AssureInputManagerReference();

            AxisNames.Clear();

            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                AxisNames.Add(axesProperty.GetArrayElementAtIndex(i).displayName);
            }
        }

        private static void AssureInputManagerReference()
        {
            if (inputManagerAsset == null)
            {
                // Grabs the actual asset file into a SerializedObject, so we can iterate through it and edit it.
                inputManagerAsset = new SerializedObject(AssetDatabase.LoadAssetAtPath("ProjectSettings/InputManager.asset", typeof(UnityEngine.Object)));
            }
        }

        #endregion
    }
}