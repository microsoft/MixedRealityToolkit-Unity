// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Services;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.CustomExtensionServices
{
    /// <summary>
    /// The implementation of your <see cref="IDemoCustomExtensionService"/>
    /// </summary>
    public class DemoCustomExtensionService : BaseExtensionService, IDemoCustomExtensionService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// TODO: use profile data in constructor.
        public DemoCustomExtensionService(string name, uint priority/*, BaseMixedRealityExtensionServiceProfile profile*/) : base(name, priority/*, profile*/)
        {
            // In the constructor, you should set any configuration data from your profile here.
            MyCustomData = string.Empty;
        }

        /// <inheritdoc />
        public string MyCustomData { get; }

        /// <inheritdoc />
        public void MyCustomMethod() { }
    }
}