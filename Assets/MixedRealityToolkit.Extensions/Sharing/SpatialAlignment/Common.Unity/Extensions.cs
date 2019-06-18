// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UQuaternion = UnityEngine.Quaternion;
using UVector3 = UnityEngine.Vector3;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.Common
{
    /// <summary>
    /// Helper Unity side extensions to enable the core abstraction to be Unity agnostic.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a <see cref="Vector3"/> to a <see cref="UVector3"/>.
        /// </summary>
        public static UVector3 AsUnityVector(this Vector3 input)
        {
            return new UVector3(input.X, input.Y, input.Z);
        }

        /// <summary>
        /// Converts a <see cref="UVector3"/> to a <see cref="Vector3"/>.
        /// </summary>
        public static Vector3 AsNumericsVector(this UVector3 input)
        {
            return new Vector3(input.x, input.y, input.z);
        }

        /// <summary>
        /// Converts a <see cref="Quaternion"/> to a <see cref="UQuaternion"/>.
        /// </summary>
        public static UQuaternion AsUnityQuaternion(this Quaternion input)
        {
            return new UQuaternion(input.X, input.Y, input.Z, input.W);
        }

        /// <summary>
        /// Converts a <see cref="UQuaternion"/> to a <see cref="Quaternion"/>.
        /// </summary>
        public static Quaternion AsNumericsQuaternion(this UQuaternion input)
        {
            return new Quaternion(input.x, input.y, input.z, input.w);
        }

        /// <summary>
        /// Converst world space position to coordinate space position.
        /// </summary>
        public static UVector3 WorldToCoordinateSpace(this ISpatialCoordinate coordinate, UVector3 vector)
        {
            return coordinate.WorldToCoordinateSpace(vector.AsNumericsVector()).AsUnityVector();
        }

        /// <summary>
        /// Converst world space rotation to coordinate space rotation.
        /// </summary>
        public static UQuaternion WorldToCoordinateSpace(this ISpatialCoordinate coordinate, UQuaternion quaternion)
        {
            return coordinate.WorldToCoordinateSpace(quaternion.AsNumericsQuaternion()).AsUnityQuaternion();
        }

        /// <summary>
        /// Converst coordinate space position to world space position.
        /// </summary>
        public static UVector3 CoordinateToWorldSpace(this ISpatialCoordinate coordinate, UVector3 vector)
        {
            return coordinate.CoordinateToWorldSpace(vector.AsNumericsVector()).AsUnityVector();
        }

        /// <summary>
        /// Converst coordinate space position to world space position.
        /// </summary>
        public static UQuaternion CoordinateToWorldSpace(this ISpatialCoordinate coordinate, UQuaternion quaternion)
        {
            return coordinate.CoordinateToWorldSpace(quaternion.AsNumericsQuaternion()).AsUnityQuaternion();
        }

        /// <summary>
        /// Attempts to create a new coordinate with this service.
        /// </summary>
        /// <param name="localPosition">Position at which the coordinate should be created.</param>
        /// <param name="localRotation">Orientation the coordinate should be created with.</param>
        /// <returns>The coordinate if the coordinate was succesfully created, otherwise null.</returns>
        public static Task<ISpatialCoordinate> TryCreateCoordinateAsync(this ISpatialCoordinateService spatialCoordinateService, UVector3 vector, UQuaternion quaternion, CancellationToken cancellationToken)
        {
            return spatialCoordinateService.TryCreateCoordinateAsync(vector.AsNumericsVector(), quaternion.AsNumericsQuaternion(), cancellationToken);
        }

        /// <summary>
        /// Gracefully allows a task to continue running without loosing any exceptions thrown or requireing to await it.
        /// </summary>
        /// <param name="task">The task that should be wrapped.</param>
        public static async void FireAndForget(this Task task)
        {
            try
            {
                await task.IgnoreCancellation().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Encountered an exception with a FireAndForget task.");
                UnityEngine.Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Gracefully allows a task to continue running without loosing any exceptions thrown or requireing to await it.
        /// </summary>
        /// <param name="task">The task that should be wrapped.</param>
        public static async void FireAndForget<T>(this Task<T> task)
        {
            try
            {
                await task.IgnoreCancellation().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Encountered an exception with a FireAndForget task.");
                UnityEngine.Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Prevents <see cref="TaskCanceledException"/> or <see cref="OperationCanceledException"/> from trickling up.
        /// </summary>
        /// <param name="task">The task to ignore exceptions for./param>
        /// <returns>A wrapping task for the given task.</returns>
        public static Task IgnoreCancellation(this Task task)
        {
            return task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    // This will rethrow any remaining exceptions, if any.
                    t.Exception.Handle(ex => ex is OperationCanceledException);
                } // else do nothing
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Prevents <see cref="TaskCanceledException"/> or <see cref="OperationCanceledException"/> from trickling up.
        /// </summary>
        /// <typeparam name="T">The result type of the Task.</typeparam>
        /// <param name="task">The task to ignore exceptions for./param>
        /// <param name="defaultCancellationReturn">The default value to return in case the task is cancelled.</param>
        /// <returns>A wrapping task for the given task.</returns>
        public static Task<T> IgnoreCancellation<T>(this Task<T> task, T defaultCancellationReturn = default(T))
        {
            return task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    // This will rethrow any remaining exceptions, if any.
                    t.Exception.Handle(ex => ex is OperationCanceledException);
                    return defaultCancellationReturn;
                }

                return t.IsCanceled ? defaultCancellationReturn : t.Result;
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// A simple helper to enable "awaiting" a <see cref="CancellationToken"/> by creating a task wrapping it.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to await.</param>
        /// <returns>The task that can be awaited.</returns>
        public static Task AsTask(this CancellationToken cancellationToken)
        {
            return Task.Delay(-1, cancellationToken);
        }

        /// <summary>
        /// The task will be awaited until the cancellation token is triggered. (await task unless cancelled).
        /// </summary>
        /// <remarks>This is different from cancelling the task. The use case is to enable a calling method 
        /// bow out of the await that it can't cancel, but doesn't require completion/cancellation in order to cancel it's own execution.</remarks>
        /// <param name="task">The task to await.</param>
        /// <param name="cancellationToken">The cancellation token to stop awaiting.</param>
        /// <returns>The task that can be awaited unless the cancellation token is triggered.</returns>
        public static Task Unless(this Task task, CancellationToken cancellationToken)
        {
            return Task.WhenAny(task, cancellationToken.AsTask());
        }

        /// <summary>
        /// The task will be awaited until the cancellation token is triggered. (await task unless cancelled).
        /// </summary>
        /// <remarks>This is different from cancelling the task. The use case is to enable a calling method 
        /// bow out of the await that it can't cancel, but doesn't require completion/cancellation in order to cancel it's own execution.</remarks>
        /// <param name="task">The task to await.</param>
        /// <param name="cancellationToken">The cancellation token to stop awaiting.</param>
        /// <returns>The task that can be awaited unless the cancellation token is triggered.</returns>
        public async static Task<T> Unless<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            return (await Task.WhenAny(task, cancellationToken.AsTask())) is Task<T> result ? result.Result : default(T);
        }

        /// <summary>
        /// Helper class to enable await on <see cref="SynchronizationContext"/>. 
        /// This is useful if you want to switch execution flow of an async function to a different thread, like Unity game thread for example.
        /// </summary>
        public struct SynchronizationContextAwaiter : INotifyCompletion
        {
            private static readonly SendOrPostCallback _postCallback = state => ((Action)state)();

            private readonly SynchronizationContext _context;
            public SynchronizationContextAwaiter(SynchronizationContext context)
            {
                _context = context;
            }

            public bool IsCompleted => _context == SynchronizationContext.Current;

            public void OnCompleted(Action continuation)
            {
                _context.Post(_postCallback, continuation);
            }

            public void GetResult() { }
        }

        /// <summary>
        /// Required extension method to enable awaiting on <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <param name="context">Context to await (switch execution flow to).</param>
        /// <returns>Awaiter for the "await" keyword to work.</returns>
        public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext context)
        {
            return new SynchronizationContextAwaiter(context);
        }
    }
}
