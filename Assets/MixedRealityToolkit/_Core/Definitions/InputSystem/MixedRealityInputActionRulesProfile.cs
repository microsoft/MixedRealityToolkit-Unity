// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Input Action Rules Profile", fileName = "MixedRealityInputActionRulesProfile", order = (int)CreateProfileMenuItemIndices.InputActionRules)]
    public class MixedRealityInputActionRulesProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private InputActionRuleDigital[] inputActionRulesDigital = null;

        /// <summary>
        /// All the Input Action Rules for <see cref="bool"/> based <see cref="MixedRealityInputAction"/>s
        /// </summary>
        public InputActionRuleDigital[] InputActionRulesDigital => inputActionRulesDigital;

        [SerializeField]
        private InputActionRuleSingleAxis[] inputActionRulesSingleAxis = null;

        /// <summary>
        /// All the Input Action Rules for <see cref="float"/> based <see cref="MixedRealityInputAction"/>s
        /// </summary>
        public InputActionRuleSingleAxis[] InputActionRulesSingleAxis => inputActionRulesSingleAxis;

        [SerializeField]
        private InputActionRuleDualAxis[] inputActionRulesDualAxis = null;

        /// <summary>
        /// All the Input Action Rules for <see cref="Vector2"/> based <see cref="MixedRealityInputAction"/>s
        /// </summary>
        public InputActionRuleDualAxis[] InputActionRulesDualAxis => inputActionRulesDualAxis;

        [SerializeField]
        private InputActionRuleVectorAxis[] inputActionRulesVectorAxis = null;

        /// <summary>
        /// All the Input Action Rules for <see cref="Vector3"/> based <see cref="MixedRealityInputAction"/>s
        /// </summary>
        public InputActionRuleVectorAxis[] InputActionRulesVectorAxis => inputActionRulesVectorAxis;

        [SerializeField]
        private InputActionRuleQuaternionAxis[] inputActionRulesQuaternionAxis = null;

        /// <summary>
        /// All the Input Action Rules for <see cref="Quaternion"/> based <see cref="MixedRealityInputAction"/>s
        /// </summary>
        public InputActionRuleQuaternionAxis[] InputActionRulesQuaternionAxis => inputActionRulesQuaternionAxis;

        [SerializeField]
        private InputActionRulePoseAxis[] inputActionRulesPoseAxis = null;

        /// <summary>
        /// All the Input Action Rules for <see cref="MixedRealityPose"/> based <see cref="MixedRealityInputAction"/>s
        /// </summary>
        public InputActionRulePoseAxis[] InputActionRulesPoseAxis => inputActionRulesPoseAxis;
    }
}