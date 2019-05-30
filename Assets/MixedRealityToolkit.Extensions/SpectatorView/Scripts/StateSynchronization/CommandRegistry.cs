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
        private Dictionary<string, CommandHandler> commandHandlers = new Dictionary<string, CommandHandler>();

        public event ConnectedEventHandler Connected;
        public event DisconnectedEventHandler Disconnected;

        protected void NotifyConnected(SocketEndpoint socketEndpoint)
        {
            Connected?.Invoke(socketEndpoint);
        }

        protected void NotifyDisconnected(SocketEndpoint socketEndpoint)
        {
            Disconnected?.Invoke(socketEndpoint);
        }

        protected void NotifyCommand(SocketEndpoint socketEndpoint, string command, BinaryReader message, int remainingDataSize)
        {
            lock (lockObject)
            {
                CommandHandler commandHandlerList = null;
                if (commandHandlers.TryGetValue(command, out commandHandlerList))
                {
                    commandHandlerList(socketEndpoint, command, message, remainingDataSize);
                }
            }
        }

        public void RegisterCommandHandler(string command, CommandHandler handler)
        {
            lock (lockObject)
            {
                CommandHandler commandList;
                if (!commandHandlers.TryGetValue(command, out commandList))
                {
                    commandHandlers[command] = handler;
                }
                else
                {
                    commandHandlers[command] = commandList + handler;
                }
            }
        }

        public void UnregisterCommandHandler(string command, CommandHandler handler)
        {
            lock (lockObject)
            {
                CommandHandler commandList;

                if (commandHandlers.TryGetValue(command, out commandList))
                {
                    commandList -= handler;
                    if (commandList == null)
                    {
                        commandHandlers.Remove(command);
                    }
                    else
                    {
                        commandHandlers[command] = commandList;
                    }
                }
            }
        }
    }
}
