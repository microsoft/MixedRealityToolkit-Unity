// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An interface for binding a class according to a DataBindingProfile.
    /// This interface is implemented by any data consumer that also supports
    /// binding and theming and wishes to be bindable from a common profile with
    /// other data consumers.
    /// </summary>
    public interface IDataBindable
    {
        /// <summary>
        /// Configure binding for this object according to the
        /// specified binding profiles.
        /// </summary>
        /// <param name="dataSourceTypes">Specific data source types to inject into the data consumer.</param>
        /// <param name="bindingProfiles">The array of binding profiles to process.</param>
        /// <returns>Returns whether any binding was found and processed.</returns>
        bool ConfigureFromBindingProfile(string[] dataSourceTypes, DataBindingProfile[] bindingProfile);
    }
}
