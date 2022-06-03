// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This interface is used to enable components outside of the Data Binding package to
    /// specify bindings for specific implementations of DataConsumers, and then
    /// call the ConfigureFromBindingProfile method on any object, typically a DataConsumer, that
    /// implements the IDataBindingConfigurator interface, to establish a series of one or
    /// more bindings.
    /// </summary>
    public interface IClassDataBindingProfile
    {
        /// <summary>
        /// The class name of DataConsumer that expects the data binding profiles provided
        /// by the data binding profiles.
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// A list of data binding profiles that can be used to identify GameObjects and then
        /// add the DataConsumers that can manage themable and data bindable UX elements.
        /// </summary>
        DataBindingProfile[] Bindings { get; }
    }
}