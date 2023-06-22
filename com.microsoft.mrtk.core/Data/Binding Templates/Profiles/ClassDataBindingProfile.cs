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
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    [Serializable]
    public class ClassDataBindingProfile : IClassDataBindingProfile
    {
        [Tooltip("Class name for the DataConsumer that this profile is designed to configure.")]
        [SerializeField, Experimental]
        private string className;
        public string ClassName => className;

        [Tooltip("List of mappings between UX elements and the data and theme keypaths and data sources that will service them.")]
        [SerializeField]
        private DataBindingProfile[] bindings;
        public DataBindingProfile[] Bindings => bindings;
    }
}

