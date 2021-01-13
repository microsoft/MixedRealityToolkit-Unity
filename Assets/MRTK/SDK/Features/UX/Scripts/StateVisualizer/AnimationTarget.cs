using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    [Serializable]
    public class AnimationTarget
    {
        [SerializeField]
        private GameObject target;

        /// <summary>
        /// 
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
        private List<IStateStyleProperty> stateStyleProperties = new List<IStateStyleProperty>();

        /// <summary>
        /// 
        /// </summary>
        public List<IStateStyleProperty> StateStyleProperties
        {
            get => stateStyleProperties;
            set => stateStyleProperties = value;
        }

        public void CreateStylePropertyInstance(string stylePropertyName, string stateName)
        {
            StateStyleProperty styleProperty;

            // Find matching event configuration by state name
            var stylePropertyTypes = TypeCacheUtility.GetSubClasses<StateStyleProperty>();
            Type stylePropertyType = stylePropertyTypes.Find((type) => type.Name.StartsWith(stylePropertyName));

            if (stylePropertyType != null)
            {
                // If a state has an associated event configuration class, then create an instance with the matching type
                styleProperty = Activator.CreateInstance(stylePropertyType) as StateStyleProperty;
            }
            else
            {
                styleProperty = null;
                Debug.Log("The style property name given does not have a matching configuration type");
            }

            styleProperty.StateName = stateName;
            styleProperty.Target = Target;

            StateStyleProperties.Add(styleProperty);
        }

        public bool IsTargetObjectValid(GameObject target)
        {
            Transform startTransform = target.transform;
            Transform initialTransform = target.transform;

            // If this game object has the State Visualizer attached 
            if (target.GetComponent<StateVisualizer>() != null)
            {
                return true;
            }

            // If the current object is a root and does not have a parent 
            if (startTransform.parent != null)
            {
                // Traverse parents until the State Visualizer is found to determine if the current target is a valid child object
                while (startTransform.parent != initialTransform)
                {
                    if (startTransform.GetComponent<StateVisualizer>() != null)
                    {
                        return true;
                    }

                    startTransform = startTransform.parent;
                }
            }

            return false;
        }

    
        public void SetKeyFrames(string stylePropertyName, AnimationClip animationClip)
        {
            IStateStyleProperty styleProperty = StateStyleProperties.Find((prop) => prop.StylePropertyName == stylePropertyName);

            styleProperty.Target = Target;

            styleProperty.SetKeyFrames(animationClip);

        }


        public void RemoveKeyFrames(string stylePropertyName, AnimationClip animationClip)
        {
            IStateStyleProperty styleProperty = StateStyleProperties.Find((prop) => prop.StylePropertyName == stylePropertyName);

            styleProperty.RemoveKeyFrames(animationClip);
        }
    }
}
