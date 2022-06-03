// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This profile typically will become part of a scriptable object to provide a series of
    /// mappings between UX elements and the data and theme keypaths and data sources that will service them,
    /// for a specific C# class that implements the IDataConsumerConfiguration interface.
    /// 
    /// Note that only the class name is required. The bindings can essentially be anything that
    /// that specific class knows how to consume. The majority of DataConsumer classes can be serviced by this
    /// implementation of a binding profile.
    /// </summary>
    [Serializable]
    public class ClassDataBindingProfile : IClassDataBindingProfile
    {
        [Tooltip("Class name for the DataConsumer that this profile is designed to configure.")]
        [SerializeField]
        private string className;
        public string ClassName => className;

        [Tooltip("List of mappings between UX elements and the data and theme keypaths and data sources that will service them.")]
        [SerializeField]
        private DataBindingProfile[] bindings;
        public DataBindingProfile[] Bindings => bindings;
    }
}

