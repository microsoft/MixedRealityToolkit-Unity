﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Dispatch commands along with reference data bound to this consumer.
    /// </summary>
    /// <remarks>
    /// This command dispatcher is useful for enabling UX elements, such as action buttons, to
    /// invoke functionality that is specific to this instance of an data bound entity. This
    /// is particularly useful in combination with DataConsumerCollection where each item
    /// is bound to different data and yet each item may wish to invoke functionality specific to
    /// itself via some form of unique identifier.
    ///
    /// As an example, given a list of contacts that has been populated via data binding, each entry may have
    /// an "Edit" and a "Delete" button for that contact.  With this component, it's possible to invoke a
    /// command from the "Edit" and "Delete" button prefabs that is automatically bound to the correct "contactID"
    /// field so that the command receiver is able to invoke the appropriate functionality for the specific
    /// contact associated with the contact ID.</remarks>
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Command Dispatcher")]
    public class DataConsumerCommandDispatcher : DataConsumerGOBase
    {
        [Tooltip("Local key path for the data to provide with a command, typically a unique identifier field.")]
        [SerializeField]
        protected string dataReferenceKeyPath;

        protected object _dataObject = null;
        protected bool _noDataObjectFound = false;

        /// </inheritdoc/>
        protected override void AttachDataConsumer()
        {
            if (!string.IsNullOrWhiteSpace(dataReferenceKeyPath))
            {
                AddKeyPathListener(dataReferenceKeyPath);
            }
        }

        /// <summary>
        /// Send a command through the Data Controller with no optional parameters
        /// </summary>
        /// <remarks>
        /// This is provided for easy integration into UX elements that can easily bind to simple methods to
        /// indicate user intent.  This will then be bound with data that has been bound, usually a unique identifier.
        ///
        /// Note that the parameters are not included and do not specify a default value of null
        /// because this prevents the method signature from meeting the requirements of simple event/command
        /// method searches using reflection.
        /// </remarks>
        /// <param name="command">The command to send.</param>
        public virtual void SendCommand(string command)
        {
            SendCommand(command, null);
        }

        /// <summary>
        /// Send a command through the Data Controller with optional parameters
        /// </summary>
        /// <remarks>
        /// If optional parameters are needed, this method allows for these to be passed in.
        /// </remarks>
        /// <param name="command">The command to send.</param>
        public virtual void SendCommand(string command, Dictionary<string, object> optionalParameters)
        {
            if (DataController != null)
            {
                if (_dataObject == null && !_noDataObjectFound)
                {
                    IDataSource dataSource = GetBestDataSource("data");

                    if (dataSource != null)
                    {
                        // One time fetching of data if not yet fetched. Any subsequent data updates are provided via ProcessDataChanged.
                        string resolvedKeyPath = dataSource.ResolveKeyPath(ResolvedKeyPathPrefix, dataReferenceKeyPath);
                        _dataObject = dataSource.GetValue(resolvedKeyPath);
                    }

                    if (_dataObject == null)
                    {
                        _noDataObjectFound = true;      // make sure we don't keep trying to initialize
                    }
                }

                if (_dataObject != null)
                {
                    DataController.ProcessCommand(command, _dataObject, optionalParameters);
                }
            }
        }

        /// </inheritdoc/>
        protected override void DetachDataConsumer()
        {
            _dataObject = null;
            _noDataObjectFound = false;
        }

        /// </inheritdoc/>
        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object value, DataChangeType dataChangeType)
        {
            if (localKeyPath == dataReferenceKeyPath)
            {
                _dataObject = value;
            }
        }
    }
}
