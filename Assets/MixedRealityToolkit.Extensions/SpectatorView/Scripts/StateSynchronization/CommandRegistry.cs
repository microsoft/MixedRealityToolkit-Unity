// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public abstract class CommandRegistry<TService> : Singleton<TService>,
        ICommandRegistry where TService : Singleton<TService>
    {
        private readonly object lockObject = new object();
        private Dictionary<string, HashSet<ICommandHandler>> commandHandlers = new Dictionary<string, HashSet<ICommandHandler>>();

        protected void NotifyConnected(SocketEndpoint socketEndpoint)
        {
            foreach (ICommandHandler commandHandler in GetCommandHandlerListForEnumeration())
            {
                commandHandler.OnConnected(socketEndpoint);
            }
        }

        protected void NotifyDisconnected(SocketEndpoint socketEndpoint)
        {
            foreach (ICommandHandler commandHandler in GetCommandHandlerListForEnumeration())
            {
                commandHandler.OnDisconnected(socketEndpoint);
            }
        }

        protected void NotifyCommand(SocketEndpoint socketEndpoint, string command, BinaryReader message, int remainingDataSize)
        {
            List<ICommandHandler> commandHandlerList = null;
            lock (lockObject)
            {
                HashSet<ICommandHandler> commandHandlerSet;
                if (commandHandlers.TryGetValue(command, out commandHandlerSet))
                {
                    commandHandlerList = new List<ICommandHandler>(commandHandlerSet);
                }
            }

            if (commandHandlerList != null)
            {
                foreach (ICommandHandler commandHandler in commandHandlerList)
                {
                    commandHandler.HandleCommand(socketEndpoint, command, message, remainingDataSize);
                }
            }
        }

        private HashSet<ICommandHandler> GetCommandHandlerListForEnumeration()
        {
            lock (lockObject)
            {
                return new HashSet<ICommandHandler>(commandHandlers.Values.SelectMany(s => s));
            }
        }

        public void RegisterCommandHandler(string command, ICommandHandler handler)
        {
            lock (lockObject)
            {
                HashSet<ICommandHandler> set;
                if (!commandHandlers.TryGetValue(command, out set))
                {
                    commandHandlers[command] = set = new HashSet<ICommandHandler>();
                }

                set.Add(handler);
            }
        }

        public void UnregisterCommandHandler(string command, ICommandHandler handler)
        {
            lock (lockObject)
            {
                HashSet<ICommandHandler> commandHandlerSet;

                if (commandHandlers.TryGetValue(command, out commandHandlerSet))
                {
                    commandHandlerSet.Remove(handler);
                }
            }
        }
    }
}
