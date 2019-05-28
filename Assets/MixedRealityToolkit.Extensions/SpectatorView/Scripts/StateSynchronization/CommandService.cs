// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class CommandService : Singleton<CommandService>,
        ICommandService
    {
        private readonly object lockObject = new object();
        private Dictionary<string, List<ICommandHandler>> commandHandlers = new Dictionary<string, List<ICommandHandler>>();
        private List<ICommandHandler> allHandlers = new List<ICommandHandler>();
        private Dictionary<string, IReadOnlyList<ICommandHandler>> commandHandlersCache = new Dictionary<string, IReadOnlyList<ICommandHandler>>();

        public IReadOnlyDictionary<string, IReadOnlyList<ICommandHandler>> CommandHandlerDictionary
        {
            get
            {
                return commandHandlersCache;
            }
        }

        public IReadOnlyList<ICommandHandler> CommandHandlers
        {
            get
            {
                lock (lockObject)
                {
                    return new List<ICommandHandler>(allHandlers);
                }
            }
        }

        public bool RegisterCommandHandler(string command, ICommandHandler handler)
        {
            lock (lockObject)
            {
                if (!commandHandlers.ContainsKey(command))
                {
                    commandHandlers[command] = new List<ICommandHandler>();
                }

                if (commandHandlers[command].Contains(handler) ||
                    allHandlers.Contains(handler))
                {
                    return false;
                }

                commandHandlers[command].Add(handler);
                allHandlers.Add(handler);
                UpdateCommandHandlerDictionaryCache();
            }

            return true;
        }

        public bool UnregisterCommandHandler(string command, ICommandHandler handler)
        {
            lock (lockObject)
            {
                if (!commandHandlers.ContainsKey(command) ||
                    !commandHandlers[command].Contains(handler) ||
                    !allHandlers.Contains(handler))
                {
                    return false;
                }

                commandHandlers[command].Remove(handler);
                allHandlers.Remove(handler);
                UpdateCommandHandlerDictionaryCache();
            }

            return true;
        }

        private void UpdateCommandHandlerDictionaryCache()
        {
            var dictionary = new Dictionary<string, IReadOnlyList<ICommandHandler>>();
            foreach (var handlerPair in commandHandlers)
            {
                dictionary.Add(handlerPair.Key, new List<ICommandHandler>(handlerPair.Value));
            }

            commandHandlersCache = dictionary;
        }
    }
}
