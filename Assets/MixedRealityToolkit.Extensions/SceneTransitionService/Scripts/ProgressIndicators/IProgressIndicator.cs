// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    public interface IProgressIndicator
    {
        /// <summary>
        /// Used to determine whether it's appropriate to use this indicator.
        /// </summary>
        ProgressIndicatorState State { get; }

        /// <summary>
        /// The message to display during loading.
        /// </summary>
        string Message { set; }

        /// <summary>
        /// Loading progress value from 0-1
        /// </summary>
        float Progress { set; }

        /// <summary>
        /// Opens the progress indicator before loading begins. Method is async to allow for animation to begin before loading.
        /// </summary>
        /// <returns></returns>
        Task OpenAsync();

        /// <summary>
        /// Closes the progress indicator after loading is finished. Method is async to allow for animation to complete.
        /// </summary>
        /// <returns></returns>
        Task CloseAsync();

    }
}