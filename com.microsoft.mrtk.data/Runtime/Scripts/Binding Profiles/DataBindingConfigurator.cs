// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Automatically configures and adds DataConsumer components needed to manage all data bound and/or themable
    /// elements in a prefab
    /// </summary>
    [Serializable]
    public class DataBindingConfigurator : IDataBindingConfigurator
    {

        public void ConfigureBinding(GameObject gameObject, UXBindingProfileTemplate bindingProfile)
        {
            ConfigureTextBinding(gameObject, bindingProfile);
            ConfigureClassBindings(gameObject, bindingProfile);
        }

        protected void ConfigureTextBinding(GameObject gameObject, UXBindingProfileTemplate bindingProfile)
        {
            // TODO: This could be optimized slightly by determining if there are actually any TMPros that need binding.
            DataConsumerText dataConsumerText = gameObject.AddComponent(typeof(DataConsumerText)) as DataConsumerText;
            dataConsumerText.DataSourceTypes = bindingProfile.DataSourceTypes;
        }

        protected void ConfigureClassBindings(GameObject gameObject, UXBindingProfileTemplate bindingProfile)
        {
            foreach (ClassDataBindingProfile classBinding in bindingProfile.ClassBindings)
            {
                try
                {
                    Type classType = Type.GetType(classBinding.ClassName);
                    Component dataBindableComponent = gameObject.AddComponent(classType);
                    IDataBindable dataBindableConsumer = dataBindableComponent as IDataBindable;
                    if (!dataBindableConsumer.ConfigureFromBindingProfile(bindingProfile.DataSourceTypes, classBinding.Bindings))
                    {
                        UnityEngine.Object.Destroy(dataBindableComponent);
                    }
                }
                catch
                {
                    Debug.LogError("Attempting to bind to class " + classBinding.ClassName + " which could not be found.");
                }
            }
        }
    }
}