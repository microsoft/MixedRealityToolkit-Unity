// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public interface ICommandService
    {
        bool RegisterCommandHandler(string command, ICommandHandler handler);
        bool UnregisterCommandHandler(string command, ICommandHandler handler);
    }
}
