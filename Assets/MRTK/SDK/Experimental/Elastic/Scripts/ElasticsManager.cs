// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    /// <summary>
    /// ElasticsManager can be used to add elastics simulation to supporting components.
    /// Call Initialize on manipulation start.
    /// Call ApplyHostTransform to apply elastics calculation to target transform.
    /// Elastics will continue simulating once manipulation ends through it's update function - 
    /// to block the elastics auto update set EnableElasticsUpdate to false. 
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Elastics/ElasticSystem.html")]
    [AddComponentMenu("Scripts/MRTK/SDK/Experimental/Elastics Manager")]
    public class ElasticsManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the ScriptableObject which holds the elastic system configuration for translation manipulation.")]
        private ElasticConfiguration translationElasticConfigurationObject = null;

        /// <summary>
        /// Reference to the ScriptableObject which holds the elastic system configuration for translation manipulation.
        /// </summary>
        public ElasticConfiguration TranslationElasticConfigurationObject
        {
            get => translationElasticConfigurationObject;
            set => translationElasticConfigurationObject = value;
        }

        [SerializeField]
        [Tooltip("Reference to the ScriptableObject which holds the elastic system configuration for rotation manipulation.")]
        private ElasticConfiguration rotationElasticConfigurationObject = null;

        /// <summary>
        /// Reference to the ScriptableObject which holds the elastic system configuration for rotation manipulation.
        /// </summary>
        public ElasticConfiguration RotationElasticConfigurationObject
        {
            get => rotationElasticConfigurationObject;
            set => rotationElasticConfigurationObject = value;
        }

        [SerializeField]
        [Tooltip("Reference to the ScriptableObject which holds the elastic system configuration for scale manipulation.")]
        private ElasticConfiguration scaleElasticConfigurationObject = null;

        /// <summary>
        /// Reference to the ScriptableObject which holds the elastic system configuration for scale manipulation.
        /// </summary>
        public ElasticConfiguration ScaleElasticConfigurationObject
        {
            get => scaleElasticConfigurationObject;
            set => scaleElasticConfigurationObject = value;
        }

        [SerializeField]
        [Tooltip("Extent of the translation elastic.")]
        private VolumeElasticExtent translationElasticExtent;

        /// <summary>
        /// Extent of the translation elastic.
        /// </summary>
        public VolumeElasticExtent TranslationElasticExtent
        {
            get => translationElasticExtent;
            set => translationElasticExtent = value;
        }

        [SerializeField]
        [Tooltip("Extent of the rotation elastic.")]
        private QuaternionElasticExtent rotationElasticExtent;

        /// <summary>
        /// Extent of the rotation elastic.
        /// </summary>
        public QuaternionElasticExtent RotationElasticExtent
        {
            get => rotationElasticExtent;
            set => rotationElasticExtent = value;
        }

        [SerializeField]
        [Tooltip("Extent of the scale elastic.")]
        private VolumeElasticExtent scaleElasticExtent;

        /// <summary>
        /// Extent of the scale elastic.
        /// </summary>
        public VolumeElasticExtent ScaleElasticExtent
        {
            get => scaleElasticExtent;
            set => scaleElasticExtent = value;
        }

        [SerializeField]
        [Tooltip("Indication of which manipulation types use elastic feedback.")]
        private TransformFlags elasticTypes = 0; // Default to none enabled.

        /// <summary>
        /// Indication of which manipulation types use elastic feedback.
        /// </summary>
        public TransformFlags ElasticTypes
        {
            get => elasticTypes;
            set => elasticTypes = value;
        }

        /// <summary>
        /// Enables elastics simulation in the update method.
        /// </summary>
        public bool EnableElasticsUpdate
        {
            get;
            set;
        }

        #region private properties

        // Magnitude of the velocity at which the elastic systems will
        // cease being simulated (if enabled) and the object will stop updating/moving.
        private const float elasticVelocityThreshold = 0.001f;

        private IElasticSystem<Vector3> translationElastic;
        private IElasticSystem<Quaternion> rotationElastic;
        private IElasticSystem<Vector3> scaleElastic;

        private Transform hostTransform = null;
        private TransformFlags elasticTypesSimulating = 0;
        #endregion

        /// <summary>
        /// Applies elastics calculation to the passed targetTransform and applies to the host transform.
        /// </summary>
        /// <param name="targetTransform">Precalculated target transform that's influenced by elastics</param>
        /// <param name="transformsToApply">Indicates which types of transforms are going to be applied. Default is Move, Rotate and Scale.</param>
        /// <returns>Modified transform types.</returns>
        public TransformFlags ApplyTargetTransform(MixedRealityTransform targetTransform, TransformFlags transformsToApply = TransformFlags.Move|TransformFlags.Rotate|TransformFlags.Scale)
        {
            Debug.Assert(hostTransform != null, "Can't apply target before calling Initialize with a valid transform reference.");
            if (hostTransform != null)
            {
                TransformFlags enabledTransformTypes = transformsToApply & elasticTypes;
                if (enabledTransformTypes.HasFlag(TransformFlags.Move))
                {
                    hostTransform.position = translationElastic.ComputeIteration(targetTransform.Position, Time.deltaTime);
                }

                if (enabledTransformTypes.HasFlag(TransformFlags.Rotate))
                {
                    hostTransform.rotation = rotationElastic.ComputeIteration(targetTransform.Rotation, Time.deltaTime);
                }

                if (enabledTransformTypes.HasFlag(TransformFlags.Scale))
                {
                    hostTransform.localScale = scaleElastic.ComputeIteration(targetTransform.Scale, Time.deltaTime);
                }

                elasticTypesSimulating = enabledTransformTypes;
                return elasticTypes;
            }
            else 
            {
                return 0;
            }
        }

        /// <summary>
        /// Initialize elastics system with the given host transform.
        /// Caches a reference to the host transform to be able to keep updating elastics after manipulation.
        /// </summary>
        /// <param name="elasticsTransform">host transform the elastics are applied to.</param>
        public void InitializeElastics(Transform elasticsTransform)
        {
            hostTransform = elasticsTransform;
            if (elasticTypes.HasFlag(TransformFlags.Move))
            {
                translationElastic = new VolumeElasticSystem(hostTransform.position,
                                                             translationElastic?.GetCurrentVelocity() ?? Vector3.zero,
                                                             translationElasticExtent,
                                                             translationElasticConfigurationObject.ElasticProperties);
            }
            if (elasticTypes.HasFlag(TransformFlags.Rotate))
            {
                rotationElastic = new QuaternionElasticSystem(hostTransform.rotation,
                                                              rotationElastic?.GetCurrentVelocity() ?? Quaternion.identity,
                                                              rotationElasticExtent,
                                                              rotationElasticConfigurationObject.ElasticProperties);
            }
            if (elasticTypes.HasFlag(TransformFlags.Scale))
            {
                scaleElastic = new VolumeElasticSystem(hostTransform.localScale,
                                                       scaleElastic?.GetCurrentVelocity() ?? Vector3.zero,
                                                       scaleElasticExtent,
                                                       scaleElasticConfigurationObject.ElasticProperties);
            }
        }

        #region MonoBehaviour Functions
        private void Update()
        {
            // If the user is not actively interacting with the object,
            // we let the elastic systems continue simulating, to allow
            // the object to naturally come to rest.
            if (EnableElasticsUpdate && hostTransform != null && elasticTypesSimulating != 0)
            {
                TransformFlags currentlySimulatedStates = 0;
                float squaredVelocityThreshold = elasticVelocityThreshold * elasticVelocityThreshold;
                if (ShouldUpdateElastics(TransformFlags.Move, translationElastic) && translationElastic.GetCurrentVelocity().sqrMagnitude > squaredVelocityThreshold)
                {
                    hostTransform.position = translationElastic.ComputeIteration(hostTransform.position, Time.deltaTime);
                    currentlySimulatedStates |= TransformFlags.Move;
                }
                if (ShouldUpdateElastics(TransformFlags.Rotate, rotationElastic) && rotationElastic.GetCurrentVelocity().eulerAngles.sqrMagnitude > squaredVelocityThreshold)
                {
                    hostTransform.rotation = rotationElastic.ComputeIteration(hostTransform.rotation, Time.deltaTime);
                    currentlySimulatedStates |= TransformFlags.Rotate;
                }
                if (ShouldUpdateElastics(TransformFlags.Scale, scaleElastic) && scaleElastic.GetCurrentVelocity().sqrMagnitude > squaredVelocityThreshold)
                {
                    hostTransform.localScale = scaleElastic.ComputeIteration(hostTransform.localScale, Time.deltaTime);
                    currentlySimulatedStates |= TransformFlags.Scale;
                }
                elasticTypesSimulating = currentlySimulatedStates;
            }
        }

        private bool ShouldUpdateElastics<T>(TransformFlags elasticType, IElasticSystem<T> elasticSystem)
        {
            return (elasticTypes.HasFlag(elasticType) && 
                elasticTypesSimulating.HasFlag(elasticType) && 
                elasticSystem != null);
        }

        #endregion MonoBehaviour Functions
    }
}