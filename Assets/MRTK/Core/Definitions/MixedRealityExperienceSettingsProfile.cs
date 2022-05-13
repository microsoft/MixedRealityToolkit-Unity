// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.EditModeTests")]
[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.PlayModeTests")]
namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Experience settings profile for the Mixed Reality Toolkit.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Toolkit Experience Settings Profile", fileName = "MixedRealityToolkitExperienceSettingsProfile", order = (int)CreateProfileMenuItemIndices.Configuration)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/configuration/mixed-reality-configuration-guide")]
    public class MixedRealityExperienceSettingsProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The scale of the Mixed Reality experience.")]
        private ExperienceScale targetExperienceScale = ExperienceScale.Room;

        /// <summary>
        /// The desired the scale of the experience.
        /// </summary>
        public ExperienceScale TargetExperienceScale
        {
            get { return targetExperienceScale; }
            set { targetExperienceScale = value; }
        }

        [SerializeField]
        [Tooltip("The amount to offset the MixedRealitySceneContent for the target experience. 1 unit = 1 meter")]
        private float contentOffset = 0.0f;

        /// <summary>
        /// The height above the floor for the Mixed Reality Experience. 1 unit = 1 meter
        /// </summary>
        public float ContentOffset
        {
            get { return contentOffset; }
            set { contentOffset = value; }
        }
    }
}
