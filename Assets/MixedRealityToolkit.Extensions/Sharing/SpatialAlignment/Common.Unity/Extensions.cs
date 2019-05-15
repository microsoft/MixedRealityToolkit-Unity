// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UQuaternion = UnityEngine.Quaternion;
using UVector3 = UnityEngine.Vector3;

namespace Microsoft.MixedReality.SpatialAlignment.Common
{
    /// <summary>
    /// Helper Unity side extensions to enable the core abstraction to be Unity agnostic.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a <see cref="Vector3"/> to a <see cref="UVector3"/>.
        /// </summary>
        public static UVector3 AsUnityVector(this Vector3 input) => new UVector3(input.X, input.Y, input.Z);

        /// <summary>
        /// Converts a <see cref="UVector3"/> to a <see cref="Vector3"/>.
        /// </summary>
        public static Vector3 AsNumericsVector(this UVector3 input) => new Vector3(input.x, input.y, input.z);

        /// <summary>
        /// Converts a <see cref="Quaternion"/> to a <see cref="UQuaternion"/>.
        /// </summary>
        public static UQuaternion AsUnityQuaternion(this Quaternion input) => new UQuaternion(input.X, input.Y, input.Z, input.W);

        /// <summary>
        /// Converts a <see cref="UQuaternion"/> to a <see cref="Quaternion"/>.
        /// </summary>
        public static Quaternion AsNumericsQuaternion(this UQuaternion input) => new Quaternion(input.x, input.y, input.z, input.w);

        /// <summary>
        /// Converst world space position to coordinate space position.
        /// </summary>
        public static UVector3 WorldToCoordinateSpace(this ISpatialCoordinate coordinate, UVector3 vector) => coordinate.WorldToCoordinateSpace(vector.AsNumericsVector()).AsUnityVector();

        /// <summary>
        /// Converst world space rotation to coordinate space rotation.
        /// </summary>
        public static UQuaternion WorldToCoordinateSpace(this ISpatialCoordinate coordinate, UQuaternion quaternion) => coordinate.WorldToCoordinateSpace(quaternion.AsNumericsQuaternion()).AsUnityQuaternion();

        /// <summary>
        /// Converst coordinate space position to world space position.
        /// </summary>
        public static UVector3 CoordinateToWorldSpace(this ISpatialCoordinate coordinate, UVector3 vector) => coordinate.CoordinateToWorldSpace(vector.AsNumericsVector()).AsUnityVector();

        /// <summary>
        /// Converst coordinate space position to world space position.
        /// </summary>
        public static UQuaternion CoordinateToWorldSpace(this ISpatialCoordinate coordinate, UQuaternion quaternion) => coordinate.CoordinateToWorldSpace(quaternion.AsNumericsQuaternion()).AsUnityQuaternion();

        /// <summary>
        /// Attempts to create a new coordinate with this service.
        /// </summary>
        /// <param name="localPosition">Position at which the coordinate should be created.</param>
        /// <param name="localRotation">Orientation the coordinate should be created with.</param>
        /// <returns>The coordinate if the coordinate was succesfully created, otherwise null.</returns>
        public static Task<ISpatialCoordinate> TryCreateCoordinateAsync(this ISpatialCoordinateService spatialCoordinateService, UVector3 vector, UQuaternion quaternion, CancellationToken cancellationToken)
            => spatialCoordinateService.TryCreateCoordinateAsync(vector.AsNumericsVector(), quaternion.AsNumericsQuaternion(), cancellationToken);

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
    }
}
