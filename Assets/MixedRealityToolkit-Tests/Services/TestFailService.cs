// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Services;

namespace Microsoft.MixedReality.Toolkit.Tests.Services
{
    internal class TestFailService : BaseExtensionService, IFailService
    {
        public TestFailService(string name, uint priority) : base(name, priority) { }
    }
}