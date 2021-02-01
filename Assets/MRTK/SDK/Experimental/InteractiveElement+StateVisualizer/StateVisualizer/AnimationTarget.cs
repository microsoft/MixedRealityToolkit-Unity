// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer
{
    /// <summary>
    /// Definition class for an animation target, utilized in the State Visualizer component.
    /// </summary>
    [Serializable]
    public class AnimationTarget
    {
        [SerializeField]
        [Tooltip("The target game object for animations.")]
        private GameObject target;

        /// <summary>
        /// The target game object for animations.
        /// </summary>
        public GameObject Target
        {
            get => target;
            set
            {
                if (IsTargetObjectValid(value))
                {
                    target = value;
                }
                else
                {
                    target = null;
                    Debug.LogError("The Target property can only be a child of this game object, the target was set back to null");
                }
            }
        }

        [SerializeReference]
        [Tooltip("List of animatable properties for the target game object.  Scale and material color are examples of animatable properties.")]
        private List<IStateAnimatableProperty> stateAnimatableProperties = new List<IStateAnimatableProperty>();

        /// <summary>
        /// List of animatable properties for the target game object.  Scale and material color are examples of animatable properties.
        /// </summary>
        public List<IStateAnimatableProperty> StateAnimatableProperties
        {
            get => stateAnimatableProperties;
            internal set => stateAnimatableProperties = value;
        }

        /// <summary>
        /// Set the keyframes on an AnimationClip.
        /// </summary>
        public void SetKeyFrames(AnimationClip animationClip)
        {
            foreach (var animatableProperty in StateAnimatableProperties)
            {
                animatableProperty.Target = Target;
                animatableProperty.SetKeyFrames(animationClip);
            }
        }

        /// <summary>
        /// Remove keyframes for a given animatable property. 
        /// </summary>
        public void RemoveKeyFrames(string animatablePropertyName, AnimationClip animationClip)
        {
            IStateAnimatableProperty animatableProperty = GetAnimatableProperty(animatablePropertyName);

            if (animatableProperty != null)
            {
                animatableProperty.RemoveKeyFrames(animationClip);
            }
        }

        private IStateAnimatableProperty GetAnimatableProperty(string animatablePropertyName)
        {
            return StateAnimatableProperties.Find((prop) => prop.AnimatablePropertyName == animatablePropertyName);
        }

        private bool IsTargetObjectValid(GameObject target)
        {
            return target.transform.FindAncestorComponent<StateVisualizer>(true);
        }

        internal StateAnimatableProperty CreateAnimatablePropertyInstance(string animatablePropertyTypeName, string stateName)
        {
            StateAnimatableProperty animatableProperty;

            // Find matching event configuration by state name
            var animatablePropertyTypes = TypeCacheUtility.GetSubClasses<StateAnimatableProperty>();
            Type animatablePropertyType = animatablePropertyTypes.Find((type) => type.Name.StartsWith(animatablePropertyTypeName));

            if (animatablePropertyType != null)
            {

                if (CanAddAnimatableProperty(animatablePropertyTypeName))
                {
                    // If a state has an associated event configuration class, then create an instance with the matching type
                    animatableProperty = Activator.CreateInstance(animatablePropertyType) as StateAnimatableProperty;

                }
                else
                {
                    animatableProperty = null;
                    Debug.LogError($"Only one {animatablePropertyTypeName} animatable property can be present for this target object.");
                }

                if (animatableProperty != null)
                {
                    animatableProperty.StateName = stateName;
                    animatableProperty.Target = Target;

                    // Generate unique id for shader properties because multiple shader properties can be 
                    // animated on a single target object
                    if (animatablePropertyTypeName.Contains("Shader"))
                    {
                        int shaderPropertyID = GenerateIDShaderProperty();

                        animatableProperty.AnimatablePropertyName = animatablePropertyTypeName + "_" + shaderPropertyID;
                    }

                    StateAnimatableProperties.Add(animatableProperty);
                }
            }
            else
            {
                animatableProperty = null;
                Debug.Log("The animatableProperty property name given does not have a matching configuration type");
            }

            return animatableProperty;
        }

        public StateAnimatableProperty AddNewAnimatableProperty(AnimatableProperty animatablePropertyTypeName, string stateName)
        {
            return CreateAnimatablePropertyInstance(animatablePropertyTypeName.ToString(), stateName);
        }

        private bool CanAddAnimatableProperty(string animatablePropertyTypeName)
        {
            // Multiple animatable shader properties can be present on one target object
            if (animatablePropertyTypeName.Contains("Shader"))
            {
                return true;
            }

            // Ensure that there is only one Scale and Position Offset property per target game object
            foreach (var animatableProp in StateAnimatableProperties)
            {
                if (animatableProp.AnimatablePropertyName == animatablePropertyTypeName)
                {
                    return false;
                }
            }

            return true;
        }

        private int GenerateIDShaderProperty()
        {
            int i = 0;
            foreach (var animatableProp in StateAnimatableProperties)
            {
                if (animatableProp is ShaderStateAnimatableProperty)
                {
                    i++;
                }
            }

            return i;
        }
    }
}
