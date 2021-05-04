// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(menuName = "Mixed Reality/Toolkit/Profiles/Mixed Reality Input Action Rules Profile", fileName = "MixedRealityInputActionRulesProfile", order = (int)CreateProfileMenuItemIndices.InputActionRules)]
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
        /// All the Input Action Rules for <see href="https://docs.unity3d.com/ScriptReference/Vector2.html">Vector2</see> based <see cref="MixedRealityInputAction"/>s
        /// </summary>
        public InputActionRuleDualAxis[] InputActionRulesDualAxis => inputActionRulesDualAxis;

        [SerializeField]
        private InputActionRuleVectorAxis[] inputActionRulesVectorAxis = null;

        /// <summary>
        /// All the Input Action Rules for <see href="https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> based <see cref="MixedRealityInputAction"/>s
        /// </summary>
        public InputActionRuleVectorAxis[] InputActionRulesVectorAxis => inputActionRulesVectorAxis;

        [SerializeField]
        private InputActionRuleQuaternionAxis[] inputActionRulesQuaternionAxis = null;

        /// <summary>
        /// All the Input Action Rules for <see href="https://docs.unity3d.com/ScriptReference/Quaternion.html">Quaternion</see> based <see cref="MixedRealityInputAction"/>s
        /// </summary>
        public InputActionRuleQuaternionAxis[] InputActionRulesQuaternionAxis => inputActionRulesQuaternionAxis;

        [SerializeField]
        private InputActionRulePoseAxis[] inputActionRulesPoseAxis = null;

        /// <summary>
        /// All the Input Action Rules for <see cref="Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose"/> based <see cref="MixedRealityInputAction"/>s
        /// </summary>
        public InputActionRulePoseAxis[] InputActionRulesPoseAxis => inputActionRulesPoseAxis;
    }
}