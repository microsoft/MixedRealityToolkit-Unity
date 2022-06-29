// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// Defines a control scheme for use with the MRTK3 input simulator.
    /// </summary>
    [CreateAssetMenu(fileName = "SimulatorControlScheme.asset", menuName = "MRTK/Input/Simulator Control Scheme")]
    public class SimulatorControlScheme : ScriptableObject
    {
        [SerializeField]
        [Tooltip("A description of the control scheme")]
        private string description = string.Empty;

        /// <summary>
        /// A description of the control scheme.
        /// </summary>
        public string Description
        {
            get => description;
            set => description = value;
        }

    }
}
