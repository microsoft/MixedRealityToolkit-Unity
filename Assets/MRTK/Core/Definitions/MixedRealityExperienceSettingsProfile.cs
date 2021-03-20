using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.EditModeTests")]
[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.PlayModeTests")]
namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Configuration profile settings for the Mixed Reality Toolkit.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Toolkit Configuration Profile", fileName = "MixedRealityToolkitConfigurationProfile", order = (int)CreateProfileMenuItemIndices.Configuration)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/configuration/mixed-reality-configuration-guide")]
    public class MixedRealityExperienceSettingsProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The scale of the Mixed Reality experience.")]
        private ExperiencePlatform targetExperiencePlatform = ExperiencePlatform.AR;

        /// <summary>
        /// The desired the scale of the experience.
        /// </summary>
        public ExperiencePlatform TargetExperiencePlatform
        {
            get { return targetExperiencePlatform; }
            set { targetExperiencePlatform = value; }
        }


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
        [Tooltip("The height above the floor for the Mixed Reality Experience. 1 unit = 1 meter")]
        private float floorHeight = 1.0f;

        /// <summary>
        /// The height above the floor for the Mixed Reality Experience. 1 unit = 1 meter
        /// </summary>
        public float FloorHeight
        {
            get { return floorHeight; }
            set { floorHeight = value; }
        }
    }


    /// <summary>
    /// The ExperienceScale identifies the environment for which the experience is designed.
    /// </summary>
    [System.Serializable]
    public enum ExperiencePlatform
    {
        /// <summary>
        /// An experience which (idk yet)
        /// </summary>
        AR,
        /// <summary>
        /// An experience which (idk yet but for VR)
        /// </summary>
        VR
    }
}
