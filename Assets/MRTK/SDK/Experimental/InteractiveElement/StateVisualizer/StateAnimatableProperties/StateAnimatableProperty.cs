// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer
{
    /// <summary>
    /// The base class for state animatable properties. Based on the values defined in the animatable property, keyframes for a target game object are set in the animation clip linked to 
    /// the animatable properties. 
    /// </summary>
    [Serializable]
    public class StateAnimatableProperty: IStateAnimatableProperty
    {
        [SerializeField, HideInInspector]
        [Tooltip("The name of state animatable property.")]
        private string animatablePropertyName;

        /// <summary>
        /// The name of state animatable property.
        /// </summary>
        public string AnimatablePropertyName
        {
            get => animatablePropertyName;
            set => animatablePropertyName = value;
        }

        [SerializeField, HideInInspector]
        [Tooltip("The name of the interaction state associated with this state animatable property.")]
        private string stateName;

        /// <summary>
        /// The name of the interaction state associated with this state animatable property.
        /// </summary>
        public string StateName
        {
            get => stateName;
            set => stateName = value;
        }

        [SerializeField, HideInInspector]
        [Tooltip("The target game object to animate.")]
        private GameObject target;

        /// <summary>
        /// The target game object to animate.  
        /// </summary>
        public GameObject Target
        {
            get => target;
            set => target = value;
        }

        [SerializeField]
        [Tooltip("The duration of the animation in seconds.")]
        private float animationDuration = 0.5f;

        /// <summary>
        /// The duration of the animation in seconds.
        /// </summary>
        public float AnimationDuration
        {
            get => animationDuration;
            set => animationDuration = value;
        }

        /// <summary>
        /// Sets the keyframes in an animation clip based on the values of the animatable properties. 
        /// </summary>
        /// <param name="animationClip">The animation clip to add keyframes to</param>
        public virtual void SetKeyFrames(AnimationClip animationClip) { }

        /// <summary>
        /// Removes the keyframes in an animation clip. 
        /// </summary>
        /// <param name="animationClip">The animation clip for keyframe removal</param>
        public virtual void RemoveKeyFrames(AnimationClip animationClip) { }

        // Find the path of the given target game object in its hierarchy
        protected string GetTargetPath(GameObject target)
        {
            List<string> objectPath = new List<string>();

            Transform startTransform = target.transform;
            Transform initialTransform = target.transform;

            // If the current object is a root and does not have a parent 
            if (startTransform.parent != null)
            {
                while (startTransform.parent != initialTransform)
                {
                    if (startTransform.GetComponent<StateVisualizer>() != null)
                    {
                        // Exit when we reach the root
                        break;
                    }

                    objectPath.Add(startTransform.name);

                    startTransform = startTransform.parent;
                }
            }

            string path = "";

            for (int i = objectPath.Count - 1; i >= 0; i--)
            {
                path += objectPath[i];

                if (i != 0)
                {
                    path += "/";
                }
            }

            return path;
        }
    }
}
