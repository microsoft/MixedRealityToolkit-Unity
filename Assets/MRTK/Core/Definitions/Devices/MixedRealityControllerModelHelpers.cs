// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Provides helpers for setting up the controller model with a visualization script.
    /// </summary>
    public static class MixedRealityControllerModelHelpers
    {
        private static MixedRealityControllerVisualizationProfile visualizationProfile = null;

        /// <summary>
        /// Tries to read the controller visualization profile to apply a visualization script to the passed-in controller model.
        /// </summary>
        /// <remarks>Automatically disables DestroyOnSourceLost to encourage controller model creators to manage their life-cycle themselves.</remarks>
        /// <param name="controllerModel">The GameObject to modify.</param>
        /// <param name="controllerType">The type of controller this model represents.</param>
        /// <param name="handedness">The handedness of this controller.</param>
        /// <returns>True if a visualization script could be loaded and applied.</returns>
        public static bool TryAddVisualizationScript(GameObject controllerModel, Type controllerType, Handedness handedness)
        {
            if (controllerModel != null)
            {
                if (visualizationProfile == null && CoreServices.InputSystem?.InputSystemProfile != null)
                {
                    visualizationProfile = CoreServices.InputSystem.InputSystemProfile.ControllerVisualizationProfile;
                }

                if (visualizationProfile != null)
                {
                    var visualizationType = visualizationProfile.GetControllerVisualizationTypeOverride(controllerType, handedness);
                    if (visualizationType != null)
                    {
                        // Set the platform controller model to not be destroyed when the source is lost. It'll be disabled instead,
                        // and re-enabled when the same controller is re-detected.
                        if (controllerModel.EnsureComponent(visualizationType.Type) is IMixedRealityControllerPoseSynchronizer visualizer)
                        {
                            visualizer.DestroyOnSourceLost = false;
                        }

                        return true;
                    }
                    else
                    {
                        Debug.LogError("Controller visualization type not defined for controller visualization profile");
                    }
                }
                else
                {
                    Debug.LogError("Failed to obtain a controller visualization profile");
                }
            }

            return false;
        }
    }
}
