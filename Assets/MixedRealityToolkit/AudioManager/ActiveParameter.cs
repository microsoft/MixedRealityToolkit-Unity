// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// The runtime instance of an AudioParameter on an ActiveEvent
    /// </summary>
    public class ActiveParameter : IDisposable
    {
        /// <summary>
        /// The value of the parameter
        /// </summary>
        private float currentValue = 0;
        /// <summary>
        /// The result of the parameter's curve on the currentValue
        /// </summary>
        private float currentResult = 0;
        /// <summary>
        /// Has the ActiveParameter been set to a value independent from the main AudioParameter
        /// </summary>
        private bool isDirty = false;
        /// <summary>
        /// Whether the object is being disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Constructor: Create a new ActiveParameter from the EventParameter
        /// </summary>
        /// <param name="root">The EventParameter to apply to this event</param>
        public ActiveParameter(EventParameter root)
        {
            this.rootParameter = root;
        }

        /// <summary>
        /// The EventParameter being used
        /// </summary>
        public EventParameter rootParameter { get; private set; }
        /// <summary>
        /// The value of the root parameter, unless the ActiveParameter has been independently set
        /// </summary>
        public float CurrentValue
        {
            get
            {
                if (this.isDirty)
                {
                    return this.currentValue;
                }
                else
                {
                    return this.rootParameter.parameter.CurrentValue;
                }
            }
            set
            {
                this.currentValue = value;
                this.currentResult = this.rootParameter.ProcessParameter(this.currentValue);
                this.isDirty = true;
            }
        }

        /// <summary>
        /// The result of the current value applied to the response curve
        /// </summary>
        public float CurrentResult
        {
            get {
                if (this.isDirty)
                {
                    return this.currentResult;
                }
                else
                {
                    return this.rootParameter.CurrentResult;
                }
            }
        }

        /// <summary>
        /// Clear the modified value and use the global parameter's value
        /// </summary>
        public void Reset()
        {
            this.currentValue = this.rootParameter.parameter.CurrentValue;
            this.currentResult = this.rootParameter.CurrentResult;
            this.isDirty = false;
        }

        /// <summary>
        /// Clear the object from memory
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implement the IDisposable interface
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //No managed resources to dispose of here?
                }

                this.disposed = true;
            }
        }
    }
}