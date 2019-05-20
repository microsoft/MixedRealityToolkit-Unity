#if UNITY_IOS
//
// Spatial Services Client
// This file was auto-generated with sscapigen based on SscApiModelDirect.cs, hash oCDnGdnaFmKPFORSJrQ6mQ==
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.SpatialAnchors
{
    internal enum status
    {
        /// <summary>
        /// Success
        /// </summary>
        OK = 0,
        /// <summary>
        /// Failed
        /// </summary>
        Failed = 1,
        /// <summary>
        /// Cannot access a disposed object.
        /// </summary>
        ObjectDisposed = 2,
        /// <summary>
        /// Out of memory.
        /// </summary>
        OutOfMemory = 12,
        /// <summary>
        /// Invalid argument.
        /// </summary>
        InvalidArgument = 22,
        /// <summary>
        /// The value is out of range.
        /// </summary>
        OutOfRange = 34,
        /// <summary>
        /// Not implemented.
        /// </summary>
        NotImplemented = 38,
        /// <summary>
        /// The key does not exist in the collection.
        /// </summary>
        KeyNotFound = 77,
        /// <summary>
        /// Amount of Metadata exceeded the allowed limit (currently 4k)
        /// </summary>
        MetadataTooLarge = 78,
        /// <summary>
        /// Application did not provide valid credentials and therefore could not authenticate with the Cloud Service
        /// </summary>
        ApplicationNotAuthenticated = 79,
        /// <summary>
        /// Application did not provide any credentials for authorization with the Cloud Service
        /// </summary>
        ApplicationNotAuthorized = 80,
        /// <summary>
        /// Multiple stores (on the same device or different devices) made concurrent changes to the same Spatial Entity and so this particular change was rejected
        /// </summary>
        ConcurrencyViolation = 81,
        /// <summary>
        /// Not enough Neighborhood Spatial Data was available to complete the desired Create operation
        /// </summary>
        NotEnoughSpatialData = 82,
        /// <summary>
        /// No Spatial Location Hint was available (or it was not specific enough) to support rediscovery from the Cloud at a later time
        /// </summary>
        NoSpatialLocationHint = 83,
        /// <summary>
        /// Application cannot connect to the Cloud Service
        /// </summary>
        CannotConnectToServer = 84,
        /// <summary>
        /// Cloud Service returned an unspecified error
        /// </summary>
        ServerError = 85,
        /// <summary>
        /// The Spatial Entity has already been associated with a different Store object, so cannot be used with this current Store object.
        /// </summary>
        AlreadyAssociatedWithADifferentStore = 86,
        /// <summary>
        /// SpatialEntity already exists in a Store but TryCreateAsync was called.
        /// </summary>
        AlreadyExists = 87,
        /// <summary>
        /// A locate operation was requested, but the criteria does not specify anything to look for.
        /// </summary>
        NoLocateCriteriaSpecified = 88,
        /// <summary>
        /// An access token was required but not specified; handle the TokenRequired event on the session to provide one.
        /// </summary>
        NoAccessTokenSpecified = 89,
        /// <summary>
        /// The session was unable to obtain an access token and so the creation could not proceed
        /// </summary>
        UnableToObtainAccessToken = 90,
    }

    internal static class NativeLibraryHelpers
    {
        internal static string[] IntPtrToStringArray(IntPtr result, int result_length)
        {
            byte[] bytes = new byte[result_length];
            System.Runtime.InteropServices.Marshal.Copy(result, bytes, 0, result_length - 1);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(result);
            return System.Text.Encoding.UTF8.GetString(bytes).Split('\0');
        }

        internal static void CheckStatus(IntPtr handle, status value)
        {
            if (value == status.OK)
            {
                return;
            }

            string message;
            string requestCorrelationVector;
            string responseCorrelationVector;

            status code = NativeLibrary.ssc_get_error_details(handle, out message, out requestCorrelationVector, out responseCorrelationVector);

            string fullMessage;
            if (code == status.Failed)
            {
                throw new InvalidOperationException("Unexpected error in exception handling.");
            }
            else if (code != status.OK)
            {
                fullMessage = "Exception thrown and an unexpected error in exception handling.";
            }
            else
            {
                fullMessage = message + ". Request CV: " + requestCorrelationVector + ". Response CV: " + responseCorrelationVector + ".";
            }

            switch (value)
            {
                case status.OK:
                    return;
                case status.Failed:
                    throw new InvalidOperationException(fullMessage);
                case status.ObjectDisposed:
                    throw new ObjectDisposedException(fullMessage);
                case status.OutOfMemory:
                    throw new OutOfMemoryException(fullMessage);
                case status.InvalidArgument:
                    throw new ArgumentException(fullMessage);
                case status.OutOfRange:
                    throw new ArgumentOutOfRangeException("", fullMessage);
                case status.NotImplemented:
                    throw new NotImplementedException(fullMessage);
                case status.KeyNotFound:
                    throw new KeyNotFoundException(fullMessage);
                case status.MetadataTooLarge:
                    throw new CloudSpatialException(CloudSpatialErrorCode.MetadataTooLarge, message, requestCorrelationVector, responseCorrelationVector);
                case status.ApplicationNotAuthenticated:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ApplicationNotAuthenticated, message, requestCorrelationVector, responseCorrelationVector);
                case status.ApplicationNotAuthorized:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ApplicationNotAuthorized, message, requestCorrelationVector, responseCorrelationVector);
                case status.ConcurrencyViolation:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ConcurrencyViolation, message, requestCorrelationVector, responseCorrelationVector);
                case status.NotEnoughSpatialData:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NotEnoughSpatialData, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoSpatialLocationHint:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoSpatialLocationHint, message, requestCorrelationVector, responseCorrelationVector);
                case status.CannotConnectToServer:
                    throw new CloudSpatialException(CloudSpatialErrorCode.CannotConnectToServer, message, requestCorrelationVector, responseCorrelationVector);
                case status.ServerError:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ServerError, message, requestCorrelationVector, responseCorrelationVector);
                case status.AlreadyAssociatedWithADifferentStore:
                    throw new CloudSpatialException(CloudSpatialErrorCode.AlreadyAssociatedWithADifferentStore, message, requestCorrelationVector, responseCorrelationVector);
                case status.AlreadyExists:
                    throw new CloudSpatialException(CloudSpatialErrorCode.AlreadyExists, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoLocateCriteriaSpecified:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoLocateCriteriaSpecified, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoAccessTokenSpecified:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoAccessTokenSpecified, message, requestCorrelationVector, responseCorrelationVector);
                case status.UnableToObtainAccessToken:
                    throw new CloudSpatialException(CloudSpatialErrorCode.UnableToObtainAccessToken, message, requestCorrelationVector, responseCorrelationVector);
            }
        }
    }

    /// <summary>This interface is implemented by classes with events to help track callbacks.</summary>
    internal interface ICookie
    {
        /// <summary>Unique cookie value for callback identification.</summary>
        ulong Cookie { get; set; }
    }

    internal static class CookieTracker<T> where T : class, ICookie
    {
        private static ulong lastCookie;
        private static Dictionary<ulong, System.WeakReference<T>> tracked = new Dictionary<ulong, System.WeakReference<T>>();
        internal static void Add(T instance)
        {
            lock (tracked)
            {
                instance.Cookie = ++lastCookie;
                tracked[instance.Cookie] = new System.WeakReference<T>(instance);
            }
        }
        internal static T Lookup(ulong cookie)
        {
            T result;
            System.WeakReference<T> reference;
            bool found;
            lock (tracked)
            {
                found = tracked.TryGetValue(cookie, out reference);
            }
            if (!found)
            {
                return null;
            }
            found = reference.TryGetTarget(out result);
            if (!found)
            {
                lock (tracked)
                {
                    tracked.Remove(cookie);
                }
            }
            return result;
        }
        internal static void Remove(T instance)
        {
            lock (tracked)
            {
                tracked.Remove(instance.Cookie);
            }
        }
    }

    /// <summary>
    /// Informs the application that a locate operation has completed.
    /// </summary>
    /// <param name="sender">
    /// The session that ran the locate operation.
    /// </param>
    /// <param name="args">
    /// The arguments describing the operation completion.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void LocateAnchorsCompletedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session requires an updated access token or authentication token.
    /// </summary>
    /// <param name="sender">
    /// The session that requires an updated access token or authentication token.
    /// </param>
    /// <param name="args">
    /// The event arguments that require an AccessToken property or an AuthenticationToken property to be set.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void TokenRequiredDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session has located an anchor or discovered that it cannot yet be located.
    /// </summary>
    /// <param name="sender">
    /// The session that fires the event.
    /// </param>
    /// <param name="args">
    /// Information about the located anchor.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void AnchorLocatedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session has been updated with new information.
    /// </summary>
    /// <param name="sender">
    /// The session that has been updated.
    /// </param>
    /// <param name="args">
    /// Information about the current session status.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SessionUpdatedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that an error occurred in a session.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the error.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SessionErrorDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application of a debug log message.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the log.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void OnLogDebugDelegateNative(ulong cookie, IntPtr args);

    internal static class NativeLibrary
    {
        internal const string DllName = "__Internal";
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_get_cancelled(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_get_identifier(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_stop(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_local_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_set_local_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_expiration(IntPtr handle, out long result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_set_expiration(IntPtr handle, long value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_identifier(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_app_properties(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_count(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_clear(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_key(IntPtr handle, Int32 index, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_item(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_set_item(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_remove_key(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string key);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_version_tag(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_identifier(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_status(IntPtr handle, out LocateAnchorStatus result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_domain(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_domain(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_id(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_id(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_key(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_key(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_configuration(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_diagnostics(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_log_level(IntPtr handle, out SessionLogLevel result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_log_level(IntPtr handle, SessionLogLevel value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_log_directory(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_log_directory(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_max_disk_size_in_mb(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_max_disk_size_in_mb(IntPtr handle, Int32 value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_images_enabled(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_images_enabled(IntPtr handle, Boolean value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_create_manifest_async(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string description, [MarshalAs(UnmanagedType.LPStr)] out String result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_submit_manifest_async(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string manifest_path);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_log_level(IntPtr handle, out SessionLogLevel result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_log_level(IntPtr handle, SessionLogLevel value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_session(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session_id(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_middleware_versions(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_middleware_versions(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_sdk_package_type(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_sdk_package_type(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_token_required(IntPtr handle, ulong value, TokenRequiredDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_set_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_set_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_deferral(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_complete(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_anchor_located(IntPtr handle, ulong value, AnchorLocatedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_locate_anchors_completed(IntPtr handle, ulong value, LocateAnchorsCompletedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_session_updated(IntPtr handle, ulong value, SessionUpdatedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_get_status(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_ready_for_create_progress(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_recommended_for_create_progress(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_session_create_hash(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_session_locate_hash(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_user_feedback(IntPtr handle, out SessionUserFeedback result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_error(IntPtr handle, ulong value, SessionErrorDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_get_error_message(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_on_log_debug(IntPtr handle, ulong value, OnLogDebugDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_get_message(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_dispose(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_access_token_with_authentication_token_async(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string authentication_token, [MarshalAs(UnmanagedType.LPStr)] out String result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_access_token_with_account_key_async(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string account_key, [MarshalAs(UnmanagedType.LPStr)] out String result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create_anchor_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create_watcher(IntPtr handle, IntPtr criteria, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_anchor_locate_criteria_get_identifiers_flat", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_identifiers(IntPtr handle, out IntPtr result, out int result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_identifiers(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPStr)] String[] value, int value_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_bypass_cache(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_bypass_cache(IntPtr handle, Boolean value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_near_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_source_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_source_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_distance_in_meters(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_distance_in_meters(IntPtr handle, Single value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_max_result_count(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_max_result_count(IntPtr handle, Int32 value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_near_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_requested_categories(IntPtr handle, out AnchorDataCategory result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_requested_categories(IntPtr handle, AnchorDataCategory value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_strategy(IntPtr handle, out LocateStrategy result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_strategy(IntPtr handle, LocateStrategy value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_anchor_properties_async(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string identifier, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_active_watchers_count(IntPtr handle, out Int32 result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_active_watchers_items(IntPtr handle, [MarshalAs(UnmanagedType.LPArray), In, Out] IntPtr[] result_array, ref Int32 result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_refresh_anchor_properties_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_update_anchor_properties_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_delete_anchor_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_process_frame(IntPtr handle, IntPtr frame);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session_status_async(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_start(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_stop(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_reset(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_get_error_details(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result_message, [MarshalAs(UnmanagedType.LPStr)] out string result_requestCorrelationVector, [MarshalAs(UnmanagedType.LPStr)] out string result_responseCorrelationVector);
    }

    // CODE STARTS HERE

    abstract class BasePrivateDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected abstract int InternalGetCount();
        protected abstract TKey InternalGetKey(int index);
        protected abstract TValue InternalGetItem(TKey key);
        protected abstract void InternalSetItem(TKey key, TValue value);
        protected abstract void InternalRemoveKey(TKey key);

        public TValue this[TKey key] { get { return InternalGetItem(key); } set { InternalSetItem(key, value); } }

        public ICollection<TKey> Keys { get { return Enumerable.Range(0, InternalGetCount()).Select(n => InternalGetKey(n)).ToList().AsReadOnly(); } }

        public ICollection<TValue> Values { get { return Enumerable.Range(0, InternalGetCount()).Select(n => InternalGetKey(n)).Select(k => InternalGetItem(k)).ToList().AsReadOnly(); } }

        public int Count { get { return InternalGetCount(); } }

        public bool IsReadOnly { get { return false; } }

        public void Add(TKey key, TValue value)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                InternalSetItem(key, value);
                return;
            }
            throw new ArgumentException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            while (InternalGetCount() > 0)
            {
                TKey key = InternalGetKey(0);
                InternalRemoveKey(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            try
            {
                TValue value = InternalGetItem(item.Key);
                if (Comparer<TValue>.Default.Compare(value, item.Value) == 0)
                {
                    return true;
                }
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                yield return new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }

        public bool Remove(TKey key)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            InternalRemoveKey(key);
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = InternalGetItem(key);
                return true;
            }
            catch (KeyNotFoundException)
            {
                value = default(TValue);
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                yield return new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }
    }

    abstract class BasePrivateList<T> : IList<T>
    {
        protected abstract int InternalGetCount();
        protected abstract T InternalGetItem(int index);
        protected abstract void InternalSetItem(int index, T value);
        protected abstract void InternalRemoveItem(int index);

        public int Count { get { return InternalGetCount(); } }

        public bool IsReadOnly { get { return false; } }

        public T this[int index] { get { return InternalGetItem(index); } set { InternalSetItem(index, value); } }

        public int IndexOf(T item)
        {
            int count = InternalGetCount();
            for (int i = 0; i < count; i++)
            {
                if (Comparer<T>.Default.Compare(item, InternalGetItem(i)) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            InternalSetItem(index, item);
        }

        public void RemoveAt(int index)
        {
            InternalRemoveItem(index);
        }

        public void Add(T item)
        {
            InternalSetItem(InternalGetCount(), item);
        }

        public void Clear()
        {
            while (InternalGetCount() > 0)
            {
                InternalRemoveItem(0);
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; ++i)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0) return false;
            InternalRemoveItem(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }
    }

    class IDictionary_String_String : BasePrivateDictionary<String, String>
    {
        internal IntPtr handle;
        internal IDictionary_String_String(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_idictionary_string_string_addref(ahandle);
        }
        ~IDictionary_String_String()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_release(this.handle));
            this.handle = IntPtr.Zero;
        }
        protected override int InternalGetCount()
        {
            int result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_count(this.handle, out result));
            return result;
        }
        protected override String InternalGetKey(int index)
        {
            string result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_key(this.handle, index, out result));
            return result;
        }
        protected override String InternalGetItem(String key)
        {
            string result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_item(this.handle, key, out result));
            return result;
        }
        protected override void InternalSetItem(String key, String value)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_set_item(this.handle, key, value));
        }
        protected override void InternalRemoveKey(String key)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_remove_key(this.handle, key));
        }
    }
    public enum SessionLogLevel : int
    {
        /// <summary>
        /// Specifies that logging should not write any messages.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies logs that indicate when the current flow of execution stops due to a failure.
        /// </summary>
        Error = 1,
        /// <summary>
        /// Specifies logs that highlight an abnormal or unexpected event, but do not otherwise cause execution to stop.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Specifies logs that track the general flow.
        /// </summary>
        Information = 3,
        /// <summary>
        /// Specifies logs used for interactive investigation during development.
        /// </summary>
        Debug = 4,
        /// <summary>
        /// Specifies all messages should be logged.
        /// </summary>
        All = 5,
    }

    public enum LocateAnchorStatus : int
    {
        /// <summary>
        /// The anchor was already being tracked.
        /// </summary>
        AlreadyTracked = 0,
        /// <summary>
        /// The anchor was found.
        /// </summary>
        Located = 1,
        /// <summary>
        /// The anchor was not found.
        /// </summary>
        NotLocated = 2,
        /// <summary>
        /// The anchor cannot be found - it was deleted or the identifier queried for was incorrect.
        /// </summary>
        NotLocatedAnchorDoesNotExist = 3,
    }

    public enum SessionUserFeedback : int
    {
        /// <summary>
        /// No specific feedback is available.
        /// </summary>
        None = 0,
        /// <summary>
        /// Device is not moving enough to create a neighborhood of key-frames.
        /// </summary>
        NotEnoughMotion = 1,
        /// <summary>
        /// Device is moving too quickly for stable tracking.
        /// </summary>
        MotionTooQuick = 2,
        /// <summary>
        /// The environment doesn't have enough feature points for stable tracking.
        /// </summary>
        NotEnoughFeatures = 4,
    }

    public enum AnchorDataCategory : int
    {
        /// <summary>
        /// No data is returned.
        /// </summary>
        None = 0,
        /// <summary>
        /// Returns Anchor properties including AppProperties.
        /// </summary>
        Properties = 1,
        /// <summary>
        /// Returns spatial information about an Anchor.
        /// </summary>
        /// <remarks>
        /// Returns a LocalAnchor for any returned CloudSpatialAnchors from the service.
        /// </remarks>
        Spatial = 2,
    }

    public enum LocateStrategy : int
    {
        /// <summary>
        /// Indicates that any method is acceptable.
        /// </summary>
        AnyStrategy = 0,
        /// <summary>
        /// Indicates that anchors will be located primarily by visual information.
        /// </summary>
        VisualInformation = 1,
        /// <summary>
        /// Indicates that anchors will be located primarily by relationship to other anchors.
        /// </summary>
        Relationship = 2,
    }

    public enum CloudSpatialErrorCode : int
    {
        /// <summary>
        /// Amount of Metadata exceeded the allowed limit (currently 4k)
        /// </summary>
        MetadataTooLarge = 0,
        /// <summary>
        /// Application did not provide valid credentials and therefore could not authenticate with the Cloud Service
        /// </summary>
        ApplicationNotAuthenticated = 1,
        /// <summary>
        /// Application did not provide any credentials for authorization with the Cloud Service
        /// </summary>
        ApplicationNotAuthorized = 2,
        /// <summary>
        /// Multiple stores (on the same device or different devices) made concurrent changes to the same Spatial Entity and so this particular change was rejected
        /// </summary>
        ConcurrencyViolation = 3,
        /// <summary>
        /// Not enough Neighborhood Spatial Data was available to complete the desired Create operation
        /// </summary>
        NotEnoughSpatialData = 4,
        /// <summary>
        /// No Spatial Location Hint was available (or it was not specific enough) to support rediscovery from the Cloud at a later time
        /// </summary>
        NoSpatialLocationHint = 5,
        /// <summary>
        /// Application cannot connect to the Cloud Service
        /// </summary>
        CannotConnectToServer = 6,
        /// <summary>
        /// Cloud Service returned an unspecified error
        /// </summary>
        ServerError = 7,
        /// <summary>
        /// The Spatial Entity has already been associated with a different Store object, so cannot be used with this current Store object.
        /// </summary>
        AlreadyAssociatedWithADifferentStore = 8,
        /// <summary>
        /// SpatialEntity already exists in a Store but TryCreateAsync was called.
        /// </summary>
        AlreadyExists = 9,
        /// <summary>
        /// A locate operation was requested, but the criteria does not specify anything to look for.
        /// </summary>
        NoLocateCriteriaSpecified = 10,
        /// <summary>
        /// An access token was required but not specified; handle the TokenRequired event on the session to provide one.
        /// </summary>
        NoAccessTokenSpecified = 11,
        /// <summary>
        /// The session was unable to obtain an access token and so the creation could not proceed
        /// </summary>
        UnableToObtainAccessToken = 12,
    }

    /// <summary>
    /// Informs the application that a locate operation has completed.
    /// </summary>
    /// <param name="sender">
    /// The session that ran the locate operation.
    /// </param>
    /// <param name="args">
    /// The arguments describing the operation completion.
    /// </param>
    public delegate void LocateAnchorsCompletedDelegate(object sender, Microsoft.Azure.SpatialAnchors.LocateAnchorsCompletedEventArgs args);

    /// <summary>
    /// Informs the application that a session requires an updated access token or authentication token.
    /// </summary>
    /// <param name="sender">
    /// The session that requires an updated access token or authentication token.
    /// </param>
    /// <param name="args">
    /// The event arguments that require an AccessToken property or an AuthenticationToken property to be set.
    /// </param>
    public delegate void TokenRequiredDelegate(object sender, Microsoft.Azure.SpatialAnchors.TokenRequiredEventArgs args);

    /// <summary>
    /// Informs the application that a session has located an anchor or discovered that it cannot yet be located.
    /// </summary>
    /// <param name="sender">
    /// The session that fires the event.
    /// </param>
    /// <param name="args">
    /// Information about the located anchor.
    /// </param>
    public delegate void AnchorLocatedDelegate(object sender, Microsoft.Azure.SpatialAnchors.AnchorLocatedEventArgs args);

    /// <summary>
    /// Informs the application that a session has been updated with new information.
    /// </summary>
    /// <param name="sender">
    /// The session that has been updated.
    /// </param>
    /// <param name="args">
    /// Information about the current session status.
    /// </param>
    public delegate void SessionUpdatedDelegate(object sender, Microsoft.Azure.SpatialAnchors.SessionUpdatedEventArgs args);

    /// <summary>
    /// Informs the application that an error occurred in a session.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the error.
    /// </param>
    public delegate void SessionErrorDelegate(object sender, Microsoft.Azure.SpatialAnchors.SessionErrorEventArgs args);

    /// <summary>
    /// Informs the application of a debug log message.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the log.
    /// </param>
    public delegate void OnLogDebugDelegate(object sender, Microsoft.Azure.SpatialAnchors.OnLogDebugEventArgs args);

    /// <summary>
    /// The exception that is thrown when an error occurs processing cloud spatial anchors.
    /// </summary>
    public class CloudSpatialException : Exception
    {
        private CloudSpatialErrorCode code;
        private string requestCorrelationVector;
        private string responseCorrelationVector;

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        public CloudSpatialException()
        {
            this.code = default(CloudSpatialErrorCode);
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code)
        {
            this.code = code;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message) : base(message)
        {
            this.code = code;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        /// <param name='requestCorrelationVector'>Request correlation vector for this exception.</param>
        /// <param name='responseCorrelationVector'>Response correlation vector for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message, string requestCorrelationVector, string responseCorrelationVector) : base(message)
        {
            this.code = code;
            this.requestCorrelationVector = requestCorrelationVector;
            this.responseCorrelationVector = responseCorrelationVector;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        /// <param name='requestCorrelationVector'>Request correlation vector for this exception.</param>
        /// <param name='responseCorrelationVector'>Response correlation vector for this exception.</param>
        /// <param name='innerException'>Exception that caused this exception to be thrown.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message, string requestCorrelationVector, string responseCorrelationVector, Exception inner) : base(message, inner)
        {
            this.code = code;
            this.requestCorrelationVector = requestCorrelationVector;
            this.responseCorrelationVector = responseCorrelationVector;
        }

        /// <summary>
        /// The error code associated with this exception.
        /// </summary>
        public CloudSpatialErrorCode ErrorCode
        {
            get { return this.code; }
        }

        /// <summary>
        /// The request correlation vector associated with this exception.
        /// </summary>
        public string RequestCorrelationVector
        {
            get { return this.requestCorrelationVector; }
        }

        /// <summary>
        /// The response correlation vector associated with this exception.
        /// </summary>
        public string ResponseCorrelationVector
        {
            get { return this.responseCorrelationVector; }
        }

    }

    /// <summary>
    /// Specifies a set of criteria for locating anchors.
    /// </summary>
    /// <remarks>
    /// Within the object, properties are combined with the AND operator. For example, if identifiers and nearAnchor are specified, then the filter will look for anchors that are near the nearAnchor and have an identifier that matches any of those identifiers.
    /// </remarks>
    public partial class AnchorLocateCriteria
    {
        internal IntPtr handle;
        internal AnchorLocateCriteria(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_anchor_locate_criteria_addref(ahandle);
        }
        public AnchorLocateCriteria()
        {
            status resultStatus = (NativeLibrary.ssc_anchor_locate_criteria_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~AnchorLocateCriteria()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Cloud anchor identifiers to locate. If empty, any anchors can be located.
        /// </summary>
        /// <remarks>
        /// Any anchors within this list will match this criteria.
        /// </remarks>
        public string[] Identifiers
        {
            get
            {
                IntPtr result;
                int result_length;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_identifiers(this.handle, out result, out result_length));
                return NativeLibraryHelpers.IntPtrToStringArray(result, result_length);
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_identifiers(this.handle, value, value.Length));
            }
        }

        /// <summary>
        /// Whether locate should bypass the local cache of anchors.
        /// </summary>
        public bool BypassCache
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_bypass_cache(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_bypass_cache(this.handle, value));
            }
        }

        /// <summary>
        /// Filters anchors to locate to be close to a specific anchor.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.NearAnchorCriteria NearAnchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.NearAnchorCriteria result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_near_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.NearAnchorCriteria(result_handle, transfer:true) : null;
                return result_object;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_near_anchor(this.handle, value.handle));
            }
        }

        /// <summary>
        /// Categories of data that are requested.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.AnchorDataCategory RequestedCategories
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.AnchorDataCategory result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_requested_categories(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_requested_categories(this.handle, value));
            }
        }

        /// <summary>
        /// Indicates the strategy by which anchors will be located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.LocateStrategy Strategy
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.LocateStrategy result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_strategy(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_strategy(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Use this type to determine the status of an anchor after a locate operation.
    /// </summary>
    public partial class AnchorLocatedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal AnchorLocatedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_anchor_located_event_args_addref(ahandle);
        }
        ~AnchorLocatedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The cloud spatial anchor that was located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor Anchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// The spatial anchor that was located.
        /// </summary>
        public string Identifier
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Specifies whether the anchor was located, or the reason why it may have failed.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.LocateAnchorStatus Status
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.LocateAnchorStatus result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_status(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that located the anchor.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Use this class to represent an anchor in space that can be persisted in a CloudSpatialAnchorSession.
    /// </summary>
    public partial class CloudSpatialAnchor
    {
        internal IntPtr handle;
        internal CloudSpatialAnchor(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_addref(ahandle);
        }
        public CloudSpatialAnchor()
        {
            status resultStatus = (NativeLibrary.ssc_cloud_spatial_anchor_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~CloudSpatialAnchor()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The anchor in the local mixed reality system.
        /// </summary>
        public IntPtr LocalAnchor
        {
            get
            {
                IntPtr result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_local_anchor(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_set_local_anchor(this.handle, value));
            }
        }

        /// <summary>
        /// The time the anchor will expire.
        /// </summary>
        public System.DateTimeOffset Expiration
        {
            get
            {
                long result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_expiration(this.handle, out result));
                return (result == 0) ? DateTimeOffset.MaxValue : DateTimeOffset.FromUnixTimeMilliseconds(result);
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_set_expiration(this.handle, (value == DateTimeOffset.MaxValue) ? 0 : value.ToUnixTimeMilliseconds()));
            }
        }

        /// <summary>
        /// The persistent identifier of this spatial anchor in the cloud service.
        /// </summary>
        public string Identifier
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A dictionary of application-defined properties that is stored with the anchor.
        /// </summary>
        public System.Collections.Generic.IDictionary<string, string> AppProperties
        {
            get
            {
                IntPtr result_handle;
                IDictionary_String_String result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_app_properties(this.handle, out result_handle));
                result_object = new IDictionary_String_String(result_handle, transfer:true);
                return result_object;
            }
        }

        /// <summary>
        /// An opaque version tag that can be used for concurrency control.
        /// </summary>
        public string VersionTag
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_version_tag(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Use this class to defer completing an operation.
    /// </summary>
    /// <remarks>
    /// This is similar to the Windows.Foundation.Deferral class.
    /// </remarks>
    public partial class CloudSpatialAnchorSessionDeferral
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSessionDeferral(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_addref(ahandle);
        }
        ~CloudSpatialAnchorSessionDeferral()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Mark the deferred operation as complete and perform any associated tasks.
        /// </summary>
        public void Complete()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_complete(this.handle));
        }

    }

    /// <summary>
    /// Use this class to configure session diagnostics that can be collected and submitted to improve system quality.
    /// </summary>
    public partial class CloudSpatialAnchorSessionDiagnostics
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSessionDiagnostics(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_addref(ahandle);
        }
        ~CloudSpatialAnchorSessionDiagnostics()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Level of tracing to log.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionLogLevel LogLevel
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionLogLevel result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_log_level(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_log_level(this.handle, value));
            }
        }

        /// <summary>
        /// Directory into which temporary log files and manifests are saved.
        /// </summary>
        public string LogDirectory
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_log_directory(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_log_directory(this.handle, value));
            }
        }

        /// <summary>
        /// Approximate maximum disk space to be used, in megabytes.
        /// </summary>
        /// <remarks>
        /// When this value is set to zero, no information will be written to disk.
        /// </remarks>
        public int MaxDiskSizeInMB
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_max_disk_size_in_mb(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_max_disk_size_in_mb(this.handle, value));
            }
        }

        /// <summary>
        /// Whether images should be logged.
        /// </summary>
        public bool ImagesEnabled
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_images_enabled(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_images_enabled(this.handle, value));
            }
        }

        /// <summary>
        /// Creates a manifest of log files and submission information to be uploaded.
        /// </summary>
        /// <param name="description">
        /// Description to be added to the diagnostics manifest.
        /// </param>
        public async System.Threading.Tasks.Task<string> CreateManifestAsync(string description)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_create_manifest_async(this.handle, description, out result));
                return result;
            });
        }

        /// <summary>
        /// Submits a diagnostics manifest and cleans up its resources.
        /// </summary>
        /// <param name="manifestPath">
        /// Path to the manifest file to submit.
        /// </param>
        public async System.Threading.Tasks.Task SubmitManifestAsync(string manifestPath)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_submit_manifest_async(this.handle, manifestPath));
            });
        }

    }

    /// <summary>
    /// Use this class to create, locate and manage spatial anchors.
    /// </summary>
    public partial class CloudSpatialAnchorSession : IDisposable, ICookie
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSession(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_addref(ahandle);
            CookieTracker<CloudSpatialAnchorSession>.Add(this);
        }
        /// <summary>
        /// Initializes a new instance with a default configuration.
        /// </summary>
        public CloudSpatialAnchorSession()
        {
            status resultStatus = (NativeLibrary.ssc_cloud_spatial_anchor_session_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
            CookieTracker<CloudSpatialAnchorSession>.Add(this);
            // Custom initialization (iOS/Unity) begins for CloudSpatialAnchorSession.
            // Custom initialization (iOS/Unity) ends for CloudSpatialAnchorSession.
        }

        ~CloudSpatialAnchorSession()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The configuration information for the session.
        /// </summary>
        /// <remarks>
        /// Configuration settings take effect when the session is started.
        /// </remarks>
        public Microsoft.Azure.SpatialAnchors.SessionConfiguration Configuration
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionConfiguration result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_configuration(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.SessionConfiguration(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// The diagnostics settings for the session, which can be used to collect and submit data for troubleshooting and improvements.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics Diagnostics
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_diagnostics(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// Logging level for the session log events.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionLogLevel LogLevel
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionLogLevel result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_log_level(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_log_level(this.handle, value));
            }
        }

        /// <summary>
        /// The tracking session used to help locate anchors.
        /// </summary>
        /// <remarks>
        /// This property is not available on the HoloLens platform.
        /// </remarks>
        public IntPtr Session
        {
            get
            {
                IntPtr result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_session(this.handle, value));
            }
        }

        /// <summary>
        /// The unique identifier for the session.
        /// </summary>
        public string SessionId
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session_id(this.handle, out result));
                return result;
            }
        }

        private ulong cookie;
        ulong ICookie.Cookie { get { return this.cookie; } set { this.cookie = value; } }
        /// <summary>Registered callbacks on this instance.</summary>
        private event TokenRequiredDelegate _TokenRequired;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(TokenRequiredDelegateNative))]
        private static void TokenRequiredStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            TokenRequiredDelegate handler = (instance == null) ? null : instance._TokenRequired;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.TokenRequiredEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static TokenRequiredDelegateNative TokenRequiredStaticHandlerDelegate = TokenRequiredStaticHandler;
        public event TokenRequiredDelegate TokenRequired
        {
            add
            {
                this._TokenRequired += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_token_required(this.handle, this.cookie, TokenRequiredStaticHandlerDelegate));
            }
            remove
            {
                this._TokenRequired -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event AnchorLocatedDelegate _AnchorLocated;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(AnchorLocatedDelegateNative))]
        private static void AnchorLocatedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            AnchorLocatedDelegate handler = (instance == null) ? null : instance._AnchorLocated;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.AnchorLocatedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static AnchorLocatedDelegateNative AnchorLocatedStaticHandlerDelegate = AnchorLocatedStaticHandler;
        public event AnchorLocatedDelegate AnchorLocated
        {
            add
            {
                this._AnchorLocated += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_anchor_located(this.handle, this.cookie, AnchorLocatedStaticHandlerDelegate));
            }
            remove
            {
                this._AnchorLocated -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event LocateAnchorsCompletedDelegate _LocateAnchorsCompleted;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(LocateAnchorsCompletedDelegateNative))]
        private static void LocateAnchorsCompletedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            LocateAnchorsCompletedDelegate handler = (instance == null) ? null : instance._LocateAnchorsCompleted;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.LocateAnchorsCompletedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static LocateAnchorsCompletedDelegateNative LocateAnchorsCompletedStaticHandlerDelegate = LocateAnchorsCompletedStaticHandler;
        public event LocateAnchorsCompletedDelegate LocateAnchorsCompleted
        {
            add
            {
                this._LocateAnchorsCompleted += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_locate_anchors_completed(this.handle, this.cookie, LocateAnchorsCompletedStaticHandlerDelegate));
            }
            remove
            {
                this._LocateAnchorsCompleted -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event SessionUpdatedDelegate _SessionUpdated;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(SessionUpdatedDelegateNative))]
        private static void SessionUpdatedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            SessionUpdatedDelegate handler = (instance == null) ? null : instance._SessionUpdated;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.SessionUpdatedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static SessionUpdatedDelegateNative SessionUpdatedStaticHandlerDelegate = SessionUpdatedStaticHandler;
        public event SessionUpdatedDelegate SessionUpdated
        {
            add
            {
                this._SessionUpdated += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_session_updated(this.handle, this.cookie, SessionUpdatedStaticHandlerDelegate));
            }
            remove
            {
                this._SessionUpdated -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event SessionErrorDelegate _Error;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(SessionErrorDelegateNative))]
        private static void ErrorStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            SessionErrorDelegate handler = (instance == null) ? null : instance._Error;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.SessionErrorEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static SessionErrorDelegateNative ErrorStaticHandlerDelegate = ErrorStaticHandler;
        public event SessionErrorDelegate Error
        {
            add
            {
                this._Error += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_error(this.handle, this.cookie, ErrorStaticHandlerDelegate));
            }
            remove
            {
                this._Error -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event OnLogDebugDelegate _OnLogDebug;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(OnLogDebugDelegateNative))]
        private static void OnLogDebugStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            OnLogDebugDelegate handler = (instance == null) ? null : instance._OnLogDebug;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.OnLogDebugEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static OnLogDebugDelegateNative OnLogDebugStaticHandlerDelegate = OnLogDebugStaticHandler;
        public event OnLogDebugDelegate OnLogDebug
        {
            add
            {
                this._OnLogDebug += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_on_log_debug(this.handle, this.cookie, OnLogDebugStaticHandlerDelegate));
            }
            remove
            {
                this._OnLogDebug -= value;
            }
        }

        /// <summary>
        /// Stops this session and releases all associated resources.
        /// </summary>
        public void Dispose()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_dispose(this.handle));
        }

        /// <summary>
        /// Gets the Azure Spatial Anchors access token from authentication token.
        /// </summary>
        /// <param name="authenticationToken">
        /// Authentication token.
        /// </param>
        public async System.Threading.Tasks.Task<string> GetAccessTokenWithAuthenticationTokenAsync(string authenticationToken)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_access_token_with_authentication_token_async(this.handle, authenticationToken, out result));
                return result;
            });
        }

        /// <summary>
        /// Gets the Azure Spatial Anchors access token from account key.
        /// </summary>
        /// <param name="accountKey">
        /// Account key.
        /// </param>
        public async System.Threading.Tasks.Task<string> GetAccessTokenWithAccountKeyAsync(string accountKey)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_access_token_with_account_key_async(this.handle, accountKey, out result));
                return result;
            });
        }

        /// <summary>
        /// Creates a new persisted spatial anchor from the specified local anchor and string properties.
        /// </summary>
        /// <param name="anchor">
        /// Anchor to be persisted.
        /// </param>
        public async System.Threading.Tasks.Task CreateAnchorAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_create_anchor_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Creates a new object that watches for anchors that meet the specified criteria.
        /// </summary>
        /// <param name="criteria">
        /// Criteria for anchors to watch for.
        /// </param>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher CreateWatcher(Microsoft.Azure.SpatialAnchors.AnchorLocateCriteria criteria)
        {
            IntPtr result_handle;
            Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_create_watcher(this.handle, criteria.handle, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
            return result_object;
        }

        /// <summary>
        /// Gets a cloud spatial anchor for the given identifier, even if it hasn't been located yet.
        /// </summary>
        /// <param name="identifier">
        /// The identifier to look for.
        /// </param>
        public async System.Threading.Tasks.Task<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor> GetAnchorPropertiesAsync(string identifier)
        {
            return await Task.Run(() =>
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_anchor_properties_async(this.handle, identifier, out result_handle));
                result_object = new CloudSpatialAnchor(result_handle, transfer:true);
                return result_object;
            });
        }

        /// <summary>
        /// Gets a list of active watchers.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher> GetActiveWatchers()
        {
            System.Collections.Generic.IReadOnlyList<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher> result;
            IntPtr[] result_array;
            int result_count;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_active_watchers_count(this.handle, out result_count));
            result_array = new IntPtr[result_count];
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_active_watchers_items(this.handle, result_array, ref result_count));
            result = result_array.Take(result_count).Select(handle => new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(handle, transfer:true)).ToList().AsReadOnly();
            return result;
        }

        /// <summary>
        /// Refreshes properties for the specified spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to refresh.
        /// </param>
        public async System.Threading.Tasks.Task RefreshAnchorPropertiesAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_refresh_anchor_properties_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Updates the specified spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to be updated.
        /// </param>
        public async System.Threading.Tasks.Task UpdateAnchorPropertiesAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_update_anchor_properties_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Deletes a persisted spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to be deleted.
        /// </param>
        public async System.Threading.Tasks.Task DeleteAnchorAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_delete_anchor_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Applications must call this method on platforms where per-frame processing is required.
        /// </summary>
        /// <param name="frame">
        /// AR frame to process.
        /// </param>
        /// <remarks>
        /// This method is not available on the HoloLens platform.
        /// </remarks>
        public void ProcessFrame(IntPtr frame)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_process_frame(this.handle, frame));
        }

        /// <summary>
        /// Gets an object describing the status of the session.
        /// </summary>
        public async System.Threading.Tasks.Task<Microsoft.Azure.SpatialAnchors.SessionStatus> GetSessionStatusAsync()
        {
            return await Task.Run(() =>
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionStatus result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session_status_async(this.handle, out result_handle));
                result_object = new SessionStatus(result_handle, transfer:true);
                return result_object;
            });
        }

        /// <summary>
        /// Begins capturing environment data for the session.
        /// </summary>
        public void Start()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_start(this.handle));
        }

        /// <summary>
        /// Stops capturing environment data for the session and cancels any outstanding locate operations. Environment data is maintained.
        /// </summary>
        public void Stop()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_stop(this.handle));
        }

        /// <summary>
        /// Resets environment data that has been captured in this session; applications must call this method when tracking is lost.
        /// </summary>
        /// <remarks>
        /// On any platform, calling the method will clean all internal cached state.
        /// </remarks>
        public void Reset()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_reset(this.handle));
        }

    }

    /// <summary>
    /// Use this class to control an object that watches for spatial anchors.
    /// </summary>
    public partial class CloudSpatialAnchorWatcher
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorWatcher(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_watcher_addref(ahandle);
        }
        ~CloudSpatialAnchorWatcher()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Distinct identifier for the watcher within its session.
        /// </summary>
        public int Identifier
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Stops further activity from this watcher.
        /// </summary>
        public void Stop()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_stop(this.handle));
        }

    }

    /// <summary>
    /// Use this type to determine when a locate operation has completed.
    /// </summary>
    public partial class LocateAnchorsCompletedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal LocateAnchorsCompletedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_locate_anchors_completed_event_args_addref(ahandle);
        }
        ~LocateAnchorsCompletedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Gets a value indicating whether the locate operation was canceled.
        /// </summary>
        /// <remarks>
        /// When this property is true, the watcher was stopped before completing.
        /// </remarks>
        public bool Cancelled
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_get_cancelled(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that completed the locate operation.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Use this class to describe how anchors to be located should be near a source anchor.
    /// </summary>
    public partial class NearAnchorCriteria
    {
        internal IntPtr handle;
        internal NearAnchorCriteria(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_near_anchor_criteria_addref(ahandle);
        }
        public NearAnchorCriteria()
        {
            status resultStatus = (NativeLibrary.ssc_near_anchor_criteria_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~NearAnchorCriteria()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Source anchor around which nearby anchors should be located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor SourceAnchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_source_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor(result_handle, transfer:true) : null;
                return result_object;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_source_anchor(this.handle, value.handle));
            }
        }

        /// <summary>
        /// Maximum distance in meters from the source anchor (defaults to 5).
        /// </summary>
        public float DistanceInMeters
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_distance_in_meters(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_distance_in_meters(this.handle, value));
            }
        }

        /// <summary>
        /// Maximum desired result count (defaults to 20).
        /// </summary>
        public int MaxResultCount
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_max_result_count(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_max_result_count(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires for logging messages.
    /// </summary>
    public partial class OnLogDebugEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal OnLogDebugEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_on_log_debug_event_args_addref(ahandle);
        }
        ~OnLogDebugEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_on_log_debug_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The logging message.
        /// </summary>
        public string Message
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_on_log_debug_event_args_get_message(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Use this class to set up the service configuration for a SpatialAnchorSession.
    /// </summary>
    public partial class SessionConfiguration
    {
        internal IntPtr handle;
        internal SessionConfiguration(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_configuration_addref(ahandle);
        }
        ~SessionConfiguration()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Account domain for the Azure Spatial Anchors service.
        /// </summary>
        /// <remarks>
        /// The default is "mixedreality.azure.com".
        /// </remarks>
        public string AccountDomain
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_domain(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_domain(this.handle, value));
            }
        }

        /// <summary>
        /// Account-level ID for the Azure Spatial Anchors service.
        /// </summary>
        public string AccountId
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_id(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_id(this.handle, value));
            }
        }

        /// <summary>
        /// Authentication token for Azure Active Directory (AAD).
        /// </summary>
        /// <remarks>
        /// If the access token and the account key are missing, the session will obtain an access token based on this value.
        /// </remarks>
        public string AuthenticationToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_authentication_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_authentication_token(this.handle, value));
            }
        }

        /// <summary>
        /// Account-level key for the Azure Spatial Anchors service.
        /// </summary>
        public string AccountKey
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_key(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_key(this.handle, value));
            }
        }

        /// <summary>
        /// Access token for the Azure Spatial Anchors service.
        /// </summary>
        public string AccessToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_access_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_access_token(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires when errors are thrown.
    /// </summary>
    public partial class SessionErrorEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal SessionErrorEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_error_event_args_addref(ahandle);
        }
        ~SessionErrorEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_get_error_message(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that found an error, possibly null.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// This type describes the status of spatial data processing.
    /// </summary>
    public partial class SessionStatus
    {
        internal IntPtr handle;
        internal SessionStatus(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_status_addref(ahandle);
        }
        ~SessionStatus()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The level of data sufficiency for a successful operation.
        /// </summary>
        /// <remarks>
        /// This value will be in the [0;1) range when data is insufficient; 1 when data is sufficient for success and greater than 1 when conditions are better than minimally sufficient.
        /// </remarks>
        public float ReadyForCreateProgress
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_ready_for_create_progress(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The ratio of data available to recommended data to create an anchor.
        /// </summary>
        /// <remarks>
        /// This value will be in the [0;1) range when data is below the recommended threshold; 1 and greater when the recommended amount of data has been gathered for a creation operation.
        /// </remarks>
        public float RecommendedForCreateProgress
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_recommended_for_create_progress(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A hash value that can be used to know when environment data that contributes to a creation operation has changed to included the latest input data.
        /// </summary>
        /// <remarks>
        /// If the hash value does not change after new frames were added to the session, then those frames were not deemed as sufficientlyy different from existing environment data and disgarded. This value may be 0 (and should be ignored) for platforms that don't feed frames individually.
        /// </remarks>
        public int SessionCreateHash
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_session_create_hash(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A hash value that can be used to know when environment data that contributes to a locate operation has changed to included the latest input data.
        /// </summary>
        /// <remarks>
        /// If the hash value does not change after new frames were added to the session, then those frames were not deemed as sufficiency different from existing environment data and disgarded. This value may be 0 (and should be ignored) for platforms that don't feed frames individually.
        /// </remarks>
        public int SessionLocateHash
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_session_locate_hash(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Feedback that can be provided to user about data processing status.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionUserFeedback UserFeedback
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionUserFeedback result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_user_feedback(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires when the session state is updated.
    /// </summary>
    public partial class SessionUpdatedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal SessionUpdatedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_updated_event_args_addref(ahandle);
        }
        ~SessionUpdatedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_updated_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Current session status.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionStatus Status
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionStatus result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_updated_event_args_get_status(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.SessionStatus(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Informs the application that the service requires an updated access token or authentication token.
    /// </summary>
    public partial class TokenRequiredEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal TokenRequiredEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_token_required_event_args_addref(ahandle);
        }
        ~TokenRequiredEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The access token to be used by the operation that requires it.
        /// </summary>
        public string AccessToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_access_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_set_access_token(this.handle, value));
            }
        }

        /// <summary>
        /// The authentication token to be used by the operation that requires it.
        /// </summary>
        public string AuthenticationToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_authentication_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_set_authentication_token(this.handle, value));
            }
        }

        /// <summary>
        /// Returns a deferral object that can be used to provide an updated access token or authentication token from another asynchronous operation.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral GetDeferral()
        {
            IntPtr result_handle;
            Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral result_object;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_deferral(this.handle, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral(result_handle, transfer:true) : null;
            return result_object;
        }

    }

}

#elif UNITY_WSA
//
// Spatial Services Client
// This file was auto-generated with sscapigen based on SscApiModelDirect.cs, hash oCDnGdnaFmKPFORSJrQ6mQ==
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.SpatialAnchors
{
    internal enum status
    {
        /// <summary>
        /// Success
        /// </summary>
        OK = 0,
        /// <summary>
        /// Failed
        /// </summary>
        Failed = 1,
        /// <summary>
        /// Cannot access a disposed object.
        /// </summary>
        ObjectDisposed = 2,
        /// <summary>
        /// Out of memory.
        /// </summary>
        OutOfMemory = 12,
        /// <summary>
        /// Invalid argument.
        /// </summary>
        InvalidArgument = 22,
        /// <summary>
        /// The value is out of range.
        /// </summary>
        OutOfRange = 34,
        /// <summary>
        /// Not implemented.
        /// </summary>
        NotImplemented = 38,
        /// <summary>
        /// The key does not exist in the collection.
        /// </summary>
        KeyNotFound = 77,
        /// <summary>
        /// Amount of Metadata exceeded the allowed limit (currently 4k)
        /// </summary>
        MetadataTooLarge = 78,
        /// <summary>
        /// Application did not provide valid credentials and therefore could not authenticate with the Cloud Service
        /// </summary>
        ApplicationNotAuthenticated = 79,
        /// <summary>
        /// Application did not provide any credentials for authorization with the Cloud Service
        /// </summary>
        ApplicationNotAuthorized = 80,
        /// <summary>
        /// Multiple stores (on the same device or different devices) made concurrent changes to the same Spatial Entity and so this particular change was rejected
        /// </summary>
        ConcurrencyViolation = 81,
        /// <summary>
        /// Not enough Neighborhood Spatial Data was available to complete the desired Create operation
        /// </summary>
        NotEnoughSpatialData = 82,
        /// <summary>
        /// No Spatial Location Hint was available (or it was not specific enough) to support rediscovery from the Cloud at a later time
        /// </summary>
        NoSpatialLocationHint = 83,
        /// <summary>
        /// Application cannot connect to the Cloud Service
        /// </summary>
        CannotConnectToServer = 84,
        /// <summary>
        /// Cloud Service returned an unspecified error
        /// </summary>
        ServerError = 85,
        /// <summary>
        /// The Spatial Entity has already been associated with a different Store object, so cannot be used with this current Store object.
        /// </summary>
        AlreadyAssociatedWithADifferentStore = 86,
        /// <summary>
        /// SpatialEntity already exists in a Store but TryCreateAsync was called.
        /// </summary>
        AlreadyExists = 87,
        /// <summary>
        /// A locate operation was requested, but the criteria does not specify anything to look for.
        /// </summary>
        NoLocateCriteriaSpecified = 88,
        /// <summary>
        /// An access token was required but not specified; handle the TokenRequired event on the session to provide one.
        /// </summary>
        NoAccessTokenSpecified = 89,
        /// <summary>
        /// The session was unable to obtain an access token and so the creation could not proceed
        /// </summary>
        UnableToObtainAccessToken = 90,
    }

    internal static class NativeLibraryHelpers
    {
        internal static string[] IntPtrToStringArray(IntPtr result, int result_length)
        {
            byte[] bytes = new byte[result_length];
            System.Runtime.InteropServices.Marshal.Copy(result, bytes, 0, result_length - 1);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(result);
            return System.Text.Encoding.UTF8.GetString(bytes).Split('\0');
        }

        internal static void CheckStatus(IntPtr handle, status value)
        {
            if (value == status.OK)
            {
                return;
            }

            string message;
            string requestCorrelationVector;
            string responseCorrelationVector;

            status code = NativeLibrary.ssc_get_error_details(handle, out message, out requestCorrelationVector, out responseCorrelationVector);

            string fullMessage;
            if (code == status.Failed)
            {
                throw new InvalidOperationException("Unexpected error in exception handling.");
            }
            else if (code != status.OK)
            {
                fullMessage = "Exception thrown and an unexpected error in exception handling.";
            }
            else
            {
                fullMessage = message + ". Request CV: " + requestCorrelationVector + ". Response CV: " + responseCorrelationVector + ".";
            }

            switch (value)
            {
                case status.OK:
                    return;
                case status.Failed:
                    throw new InvalidOperationException(fullMessage);
                case status.ObjectDisposed:
                    throw new ObjectDisposedException(fullMessage);
                case status.OutOfMemory:
                    throw new OutOfMemoryException(fullMessage);
                case status.InvalidArgument:
                    throw new ArgumentException(fullMessage);
                case status.OutOfRange:
                    throw new ArgumentOutOfRangeException("", fullMessage);
                case status.NotImplemented:
                    throw new NotImplementedException(fullMessage);
                case status.KeyNotFound:
                    throw new KeyNotFoundException(fullMessage);
                case status.MetadataTooLarge:
                    throw new CloudSpatialException(CloudSpatialErrorCode.MetadataTooLarge, message, requestCorrelationVector, responseCorrelationVector);
                case status.ApplicationNotAuthenticated:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ApplicationNotAuthenticated, message, requestCorrelationVector, responseCorrelationVector);
                case status.ApplicationNotAuthorized:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ApplicationNotAuthorized, message, requestCorrelationVector, responseCorrelationVector);
                case status.ConcurrencyViolation:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ConcurrencyViolation, message, requestCorrelationVector, responseCorrelationVector);
                case status.NotEnoughSpatialData:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NotEnoughSpatialData, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoSpatialLocationHint:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoSpatialLocationHint, message, requestCorrelationVector, responseCorrelationVector);
                case status.CannotConnectToServer:
                    throw new CloudSpatialException(CloudSpatialErrorCode.CannotConnectToServer, message, requestCorrelationVector, responseCorrelationVector);
                case status.ServerError:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ServerError, message, requestCorrelationVector, responseCorrelationVector);
                case status.AlreadyAssociatedWithADifferentStore:
                    throw new CloudSpatialException(CloudSpatialErrorCode.AlreadyAssociatedWithADifferentStore, message, requestCorrelationVector, responseCorrelationVector);
                case status.AlreadyExists:
                    throw new CloudSpatialException(CloudSpatialErrorCode.AlreadyExists, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoLocateCriteriaSpecified:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoLocateCriteriaSpecified, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoAccessTokenSpecified:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoAccessTokenSpecified, message, requestCorrelationVector, responseCorrelationVector);
                case status.UnableToObtainAccessToken:
                    throw new CloudSpatialException(CloudSpatialErrorCode.UnableToObtainAccessToken, message, requestCorrelationVector, responseCorrelationVector);
            }
        }
    }

    /// <summary>This interface is implemented by classes with events to help track callbacks.</summary>
    internal interface ICookie
    {
        /// <summary>Unique cookie value for callback identification.</summary>
        ulong Cookie { get; set; }
    }

    internal static class CookieTracker<T> where T : class, ICookie
    {
        private static ulong lastCookie;
        private static Dictionary<ulong, System.WeakReference<T>> tracked = new Dictionary<ulong, System.WeakReference<T>>();
        internal static void Add(T instance)
        {
            lock (tracked)
            {
                instance.Cookie = ++lastCookie;
                tracked[instance.Cookie] = new System.WeakReference<T>(instance);
            }
        }
        internal static T Lookup(ulong cookie)
        {
            T result;
            System.WeakReference<T> reference;
            bool found;
            lock (tracked)
            {
                found = tracked.TryGetValue(cookie, out reference);
            }
            if (!found)
            {
                return null;
            }
            found = reference.TryGetTarget(out result);
            if (!found)
            {
                lock (tracked)
                {
                    tracked.Remove(cookie);
                }
            }
            return result;
        }
        internal static void Remove(T instance)
        {
            lock (tracked)
            {
                tracked.Remove(instance.Cookie);
            }
        }
    }

    /// <summary>
    /// Informs the application that a locate operation has completed.
    /// </summary>
    /// <param name="sender">
    /// The session that ran the locate operation.
    /// </param>
    /// <param name="args">
    /// The arguments describing the operation completion.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void LocateAnchorsCompletedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session requires an updated access token or authentication token.
    /// </summary>
    /// <param name="sender">
    /// The session that requires an updated access token or authentication token.
    /// </param>
    /// <param name="args">
    /// The event arguments that require an AccessToken property or an AuthenticationToken property to be set.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void TokenRequiredDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session has located an anchor or discovered that it cannot yet be located.
    /// </summary>
    /// <param name="sender">
    /// The session that fires the event.
    /// </param>
    /// <param name="args">
    /// Information about the located anchor.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void AnchorLocatedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session has been updated with new information.
    /// </summary>
    /// <param name="sender">
    /// The session that has been updated.
    /// </param>
    /// <param name="args">
    /// Information about the current session status.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SessionUpdatedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that an error occurred in a session.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the error.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SessionErrorDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application of a debug log message.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the log.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void OnLogDebugDelegateNative(ulong cookie, IntPtr args);

    internal static class NativeLibrary
    {
        internal const string DllName = "AzureSpatialAnchors";
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_get_cancelled(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_get_identifier(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_stop(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_local_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_set_local_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_expiration(IntPtr handle, out long result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_set_expiration(IntPtr handle, long value);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_get_identifier_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_identifier(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_app_properties(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_count(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_clear(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_idictionary_string_string_get_key_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_key(IntPtr handle, Int32 index, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_idictionary_string_string_get_item_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_item(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string key, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_idictionary_string_string_set_item_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_set_item(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string key, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_idictionary_string_string_remove_key_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_remove_key(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string key);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_get_version_tag_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_version_tag(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_anchor_located_event_args_get_identifier_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_identifier(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_status(IntPtr handle, out LocateAnchorStatus result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_get_account_domain_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_domain(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_set_account_domain_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_domain(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_get_account_id_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_id(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_set_account_id_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_id(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_get_authentication_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_set_authentication_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_get_account_key_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_key(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_set_account_key_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_key(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_get_access_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_set_access_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_configuration(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_diagnostics(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_log_level(IntPtr handle, out SessionLogLevel result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_log_level(IntPtr handle, SessionLogLevel value);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_diagnostics_get_log_directory_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_log_directory(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_diagnostics_set_log_directory_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_log_directory(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_max_disk_size_in_mb(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_max_disk_size_in_mb(IntPtr handle, Int32 value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_images_enabled(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_images_enabled(IntPtr handle, Boolean value);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_diagnostics_create_manifest_async_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_create_manifest_async(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string description, [MarshalAs(UnmanagedType.LPWStr)] out String result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_diagnostics_submit_manifest_async_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_submit_manifest_async(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string manifest_path);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_log_level(IntPtr handle, out SessionLogLevel result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_log_level(IntPtr handle, SessionLogLevel value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_session(IntPtr handle, IntPtr value);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_session_id_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session_id(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_middleware_versions_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_middleware_versions(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_set_middleware_versions_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_middleware_versions(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_sdk_package_type_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_sdk_package_type(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_set_sdk_package_type_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_sdk_package_type(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_token_required(IntPtr handle, ulong value, TokenRequiredDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_token_required_event_args_get_access_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_token_required_event_args_set_access_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_set_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_token_required_event_args_get_authentication_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_token_required_event_args_set_authentication_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_set_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_deferral(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_complete(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_anchor_located(IntPtr handle, ulong value, AnchorLocatedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_locate_anchors_completed(IntPtr handle, ulong value, LocateAnchorsCompletedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_session_updated(IntPtr handle, ulong value, SessionUpdatedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_get_status(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_ready_for_create_progress(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_recommended_for_create_progress(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_session_create_hash(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_session_locate_hash(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_user_feedback(IntPtr handle, out SessionUserFeedback result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_error(IntPtr handle, ulong value, SessionErrorDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_session_error_event_args_get_error_message_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_get_error_message(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_on_log_debug(IntPtr handle, ulong value, OnLogDebugDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_on_log_debug_event_args_get_message_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_get_message(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_dispose(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_access_token_with_authentication_token_async_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_access_token_with_authentication_token_async(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string authentication_token, [MarshalAs(UnmanagedType.LPWStr)] out String result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_access_token_with_account_key_async_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_access_token_with_account_key_async(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string account_key, [MarshalAs(UnmanagedType.LPWStr)] out String result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create_anchor_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create_watcher(IntPtr handle, IntPtr criteria, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_anchor_locate_criteria_get_identifiers_wide_flat", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_identifiers(IntPtr handle, out IntPtr result, out int result_count);
        [DllImport(DllName, EntryPoint="ssc_anchor_locate_criteria_set_identifiers_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_identifiers(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPWStr)] String[] value, int value_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_bypass_cache(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_bypass_cache(IntPtr handle, Boolean value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_near_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_source_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_source_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_distance_in_meters(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_distance_in_meters(IntPtr handle, Single value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_max_result_count(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_max_result_count(IntPtr handle, Int32 value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_near_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_requested_categories(IntPtr handle, out AnchorDataCategory result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_requested_categories(IntPtr handle, AnchorDataCategory value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_strategy(IntPtr handle, out LocateStrategy result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_strategy(IntPtr handle, LocateStrategy value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_create(out IntPtr instance);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_anchor_properties_async_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_anchor_properties_async(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string identifier, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_active_watchers_count(IntPtr handle, out Int32 result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_active_watchers_items(IntPtr handle, [MarshalAs(UnmanagedType.LPArray), In, Out] IntPtr[] result_array, ref Int32 result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_refresh_anchor_properties_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_update_anchor_properties_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_delete_anchor_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session_status_async(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_start(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_stop(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_reset(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_get_error_details_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_get_error_details(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result_message, [MarshalAs(UnmanagedType.LPWStr)] out string result_requestCorrelationVector, [MarshalAs(UnmanagedType.LPWStr)] out string result_responseCorrelationVector);
    }

    // CODE STARTS HERE

    abstract class BasePrivateDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected abstract int InternalGetCount();
        protected abstract TKey InternalGetKey(int index);
        protected abstract TValue InternalGetItem(TKey key);
        protected abstract void InternalSetItem(TKey key, TValue value);
        protected abstract void InternalRemoveKey(TKey key);

        public TValue this[TKey key] { get { return InternalGetItem(key); } set { InternalSetItem(key, value); } }

        public ICollection<TKey> Keys { get { return Enumerable.Range(0, InternalGetCount()).Select(n => InternalGetKey(n)).ToList().AsReadOnly(); } }

        public ICollection<TValue> Values { get { return Enumerable.Range(0, InternalGetCount()).Select(n => InternalGetKey(n)).Select(k => InternalGetItem(k)).ToList().AsReadOnly(); } }

        public int Count { get { return InternalGetCount(); } }

        public bool IsReadOnly { get { return false; } }

        public void Add(TKey key, TValue value)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                InternalSetItem(key, value);
                return;
            }
            throw new ArgumentException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            while (InternalGetCount() > 0)
            {
                TKey key = InternalGetKey(0);
                InternalRemoveKey(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            try
            {
                TValue value = InternalGetItem(item.Key);
                if (Comparer<TValue>.Default.Compare(value, item.Value) == 0)
                {
                    return true;
                }
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                yield return new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }

        public bool Remove(TKey key)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            InternalRemoveKey(key);
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = InternalGetItem(key);
                return true;
            }
            catch (KeyNotFoundException)
            {
                value = default(TValue);
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                yield return new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }
    }

    abstract class BasePrivateList<T> : IList<T>
    {
        protected abstract int InternalGetCount();
        protected abstract T InternalGetItem(int index);
        protected abstract void InternalSetItem(int index, T value);
        protected abstract void InternalRemoveItem(int index);

        public int Count { get { return InternalGetCount(); } }

        public bool IsReadOnly { get { return false; } }

        public T this[int index] { get { return InternalGetItem(index); } set { InternalSetItem(index, value); } }

        public int IndexOf(T item)
        {
            int count = InternalGetCount();
            for (int i = 0; i < count; i++)
            {
                if (Comparer<T>.Default.Compare(item, InternalGetItem(i)) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            InternalSetItem(index, item);
        }

        public void RemoveAt(int index)
        {
            InternalRemoveItem(index);
        }

        public void Add(T item)
        {
            InternalSetItem(InternalGetCount(), item);
        }

        public void Clear()
        {
            while (InternalGetCount() > 0)
            {
                InternalRemoveItem(0);
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; ++i)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0) return false;
            InternalRemoveItem(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }
    }

    class IDictionary_String_String : BasePrivateDictionary<String, String>
    {
        internal IntPtr handle;
        internal IDictionary_String_String(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_idictionary_string_string_addref(ahandle);
        }
        ~IDictionary_String_String()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_release(this.handle));
            this.handle = IntPtr.Zero;
        }
        protected override int InternalGetCount()
        {
            int result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_count(this.handle, out result));
            return result;
        }
        protected override String InternalGetKey(int index)
        {
            string result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_key(this.handle, index, out result));
            return result;
        }
        protected override String InternalGetItem(String key)
        {
            string result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_item(this.handle, key, out result));
            return result;
        }
        protected override void InternalSetItem(String key, String value)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_set_item(this.handle, key, value));
        }
        protected override void InternalRemoveKey(String key)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_remove_key(this.handle, key));
        }
    }
    public enum SessionLogLevel : int
    {
        /// <summary>
        /// Specifies that logging should not write any messages.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies logs that indicate when the current flow of execution stops due to a failure.
        /// </summary>
        Error = 1,
        /// <summary>
        /// Specifies logs that highlight an abnormal or unexpected event, but do not otherwise cause execution to stop.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Specifies logs that track the general flow.
        /// </summary>
        Information = 3,
        /// <summary>
        /// Specifies logs used for interactive investigation during development.
        /// </summary>
        Debug = 4,
        /// <summary>
        /// Specifies all messages should be logged.
        /// </summary>
        All = 5,
    }

    public enum LocateAnchorStatus : int
    {
        /// <summary>
        /// The anchor was already being tracked.
        /// </summary>
        AlreadyTracked = 0,
        /// <summary>
        /// The anchor was found.
        /// </summary>
        Located = 1,
        /// <summary>
        /// The anchor was not found.
        /// </summary>
        NotLocated = 2,
        /// <summary>
        /// The anchor cannot be found - it was deleted or the identifier queried for was incorrect.
        /// </summary>
        NotLocatedAnchorDoesNotExist = 3,
    }

    public enum SessionUserFeedback : int
    {
        /// <summary>
        /// No specific feedback is available.
        /// </summary>
        None = 0,
        /// <summary>
        /// Device is not moving enough to create a neighborhood of key-frames.
        /// </summary>
        NotEnoughMotion = 1,
        /// <summary>
        /// Device is moving too quickly for stable tracking.
        /// </summary>
        MotionTooQuick = 2,
        /// <summary>
        /// The environment doesn't have enough feature points for stable tracking.
        /// </summary>
        NotEnoughFeatures = 4,
    }

    public enum AnchorDataCategory : int
    {
        /// <summary>
        /// No data is returned.
        /// </summary>
        None = 0,
        /// <summary>
        /// Returns Anchor properties including AppProperties.
        /// </summary>
        Properties = 1,
        /// <summary>
        /// Returns spatial information about an Anchor.
        /// </summary>
        /// <remarks>
        /// Returns a LocalAnchor for any returned CloudSpatialAnchors from the service.
        /// </remarks>
        Spatial = 2,
    }

    public enum LocateStrategy : int
    {
        /// <summary>
        /// Indicates that any method is acceptable.
        /// </summary>
        AnyStrategy = 0,
        /// <summary>
        /// Indicates that anchors will be located primarily by visual information.
        /// </summary>
        VisualInformation = 1,
        /// <summary>
        /// Indicates that anchors will be located primarily by relationship to other anchors.
        /// </summary>
        Relationship = 2,
    }

    public enum CloudSpatialErrorCode : int
    {
        /// <summary>
        /// Amount of Metadata exceeded the allowed limit (currently 4k)
        /// </summary>
        MetadataTooLarge = 0,
        /// <summary>
        /// Application did not provide valid credentials and therefore could not authenticate with the Cloud Service
        /// </summary>
        ApplicationNotAuthenticated = 1,
        /// <summary>
        /// Application did not provide any credentials for authorization with the Cloud Service
        /// </summary>
        ApplicationNotAuthorized = 2,
        /// <summary>
        /// Multiple stores (on the same device or different devices) made concurrent changes to the same Spatial Entity and so this particular change was rejected
        /// </summary>
        ConcurrencyViolation = 3,
        /// <summary>
        /// Not enough Neighborhood Spatial Data was available to complete the desired Create operation
        /// </summary>
        NotEnoughSpatialData = 4,
        /// <summary>
        /// No Spatial Location Hint was available (or it was not specific enough) to support rediscovery from the Cloud at a later time
        /// </summary>
        NoSpatialLocationHint = 5,
        /// <summary>
        /// Application cannot connect to the Cloud Service
        /// </summary>
        CannotConnectToServer = 6,
        /// <summary>
        /// Cloud Service returned an unspecified error
        /// </summary>
        ServerError = 7,
        /// <summary>
        /// The Spatial Entity has already been associated with a different Store object, so cannot be used with this current Store object.
        /// </summary>
        AlreadyAssociatedWithADifferentStore = 8,
        /// <summary>
        /// SpatialEntity already exists in a Store but TryCreateAsync was called.
        /// </summary>
        AlreadyExists = 9,
        /// <summary>
        /// A locate operation was requested, but the criteria does not specify anything to look for.
        /// </summary>
        NoLocateCriteriaSpecified = 10,
        /// <summary>
        /// An access token was required but not specified; handle the TokenRequired event on the session to provide one.
        /// </summary>
        NoAccessTokenSpecified = 11,
        /// <summary>
        /// The session was unable to obtain an access token and so the creation could not proceed
        /// </summary>
        UnableToObtainAccessToken = 12,
    }

    /// <summary>
    /// Informs the application that a locate operation has completed.
    /// </summary>
    /// <param name="sender">
    /// The session that ran the locate operation.
    /// </param>
    /// <param name="args">
    /// The arguments describing the operation completion.
    /// </param>
    public delegate void LocateAnchorsCompletedDelegate(object sender, Microsoft.Azure.SpatialAnchors.LocateAnchorsCompletedEventArgs args);

    /// <summary>
    /// Informs the application that a session requires an updated access token or authentication token.
    /// </summary>
    /// <param name="sender">
    /// The session that requires an updated access token or authentication token.
    /// </param>
    /// <param name="args">
    /// The event arguments that require an AccessToken property or an AuthenticationToken property to be set.
    /// </param>
    public delegate void TokenRequiredDelegate(object sender, Microsoft.Azure.SpatialAnchors.TokenRequiredEventArgs args);

    /// <summary>
    /// Informs the application that a session has located an anchor or discovered that it cannot yet be located.
    /// </summary>
    /// <param name="sender">
    /// The session that fires the event.
    /// </param>
    /// <param name="args">
    /// Information about the located anchor.
    /// </param>
    public delegate void AnchorLocatedDelegate(object sender, Microsoft.Azure.SpatialAnchors.AnchorLocatedEventArgs args);

    /// <summary>
    /// Informs the application that a session has been updated with new information.
    /// </summary>
    /// <param name="sender">
    /// The session that has been updated.
    /// </param>
    /// <param name="args">
    /// Information about the current session status.
    /// </param>
    public delegate void SessionUpdatedDelegate(object sender, Microsoft.Azure.SpatialAnchors.SessionUpdatedEventArgs args);

    /// <summary>
    /// Informs the application that an error occurred in a session.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the error.
    /// </param>
    public delegate void SessionErrorDelegate(object sender, Microsoft.Azure.SpatialAnchors.SessionErrorEventArgs args);

    /// <summary>
    /// Informs the application of a debug log message.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the log.
    /// </param>
    public delegate void OnLogDebugDelegate(object sender, Microsoft.Azure.SpatialAnchors.OnLogDebugEventArgs args);

    /// <summary>
    /// The exception that is thrown when an error occurs processing cloud spatial anchors.
    /// </summary>
    public class CloudSpatialException : Exception
    {
        private CloudSpatialErrorCode code;
        private string requestCorrelationVector;
        private string responseCorrelationVector;

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        public CloudSpatialException()
        {
            this.code = default(CloudSpatialErrorCode);
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code)
        {
            this.code = code;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message) : base(message)
        {
            this.code = code;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        /// <param name='requestCorrelationVector'>Request correlation vector for this exception.</param>
        /// <param name='responseCorrelationVector'>Response correlation vector for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message, string requestCorrelationVector, string responseCorrelationVector) : base(message)
        {
            this.code = code;
            this.requestCorrelationVector = requestCorrelationVector;
            this.responseCorrelationVector = responseCorrelationVector;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        /// <param name='requestCorrelationVector'>Request correlation vector for this exception.</param>
        /// <param name='responseCorrelationVector'>Response correlation vector for this exception.</param>
        /// <param name='innerException'>Exception that caused this exception to be thrown.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message, string requestCorrelationVector, string responseCorrelationVector, Exception inner) : base(message, inner)
        {
            this.code = code;
            this.requestCorrelationVector = requestCorrelationVector;
            this.responseCorrelationVector = responseCorrelationVector;
        }

        /// <summary>
        /// The error code associated with this exception.
        /// </summary>
        public CloudSpatialErrorCode ErrorCode
        {
            get { return this.code; }
        }

        /// <summary>
        /// The request correlation vector associated with this exception.
        /// </summary>
        public string RequestCorrelationVector
        {
            get { return this.requestCorrelationVector; }
        }

        /// <summary>
        /// The response correlation vector associated with this exception.
        /// </summary>
        public string ResponseCorrelationVector
        {
            get { return this.responseCorrelationVector; }
        }

    }

    /// <summary>
    /// Specifies a set of criteria for locating anchors.
    /// </summary>
    /// <remarks>
    /// Within the object, properties are combined with the AND operator. For example, if identifiers and nearAnchor are specified, then the filter will look for anchors that are near the nearAnchor and have an identifier that matches any of those identifiers.
    /// </remarks>
    public partial class AnchorLocateCriteria
    {
        internal IntPtr handle;
        internal AnchorLocateCriteria(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_anchor_locate_criteria_addref(ahandle);
        }
        public AnchorLocateCriteria()
        {
            status resultStatus = (NativeLibrary.ssc_anchor_locate_criteria_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~AnchorLocateCriteria()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Cloud anchor identifiers to locate. If empty, any anchors can be located.
        /// </summary>
        /// <remarks>
        /// Any anchors within this list will match this criteria.
        /// </remarks>
        public string[] Identifiers
        {
            get
            {
                IntPtr result;
                int result_length;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_identifiers(this.handle, out result, out result_length));
                return NativeLibraryHelpers.IntPtrToStringArray(result, result_length);
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_identifiers(this.handle, value, value.Length));
            }
        }

        /// <summary>
        /// Whether locate should bypass the local cache of anchors.
        /// </summary>
        public bool BypassCache
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_bypass_cache(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_bypass_cache(this.handle, value));
            }
        }

        /// <summary>
        /// Filters anchors to locate to be close to a specific anchor.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.NearAnchorCriteria NearAnchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.NearAnchorCriteria result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_near_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.NearAnchorCriteria(result_handle, transfer:true) : null;
                return result_object;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_near_anchor(this.handle, value.handle));
            }
        }

        /// <summary>
        /// Categories of data that are requested.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.AnchorDataCategory RequestedCategories
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.AnchorDataCategory result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_requested_categories(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_requested_categories(this.handle, value));
            }
        }

        /// <summary>
        /// Indicates the strategy by which anchors will be located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.LocateStrategy Strategy
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.LocateStrategy result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_strategy(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_strategy(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Use this type to determine the status of an anchor after a locate operation.
    /// </summary>
    public partial class AnchorLocatedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal AnchorLocatedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_anchor_located_event_args_addref(ahandle);
        }
        ~AnchorLocatedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The cloud spatial anchor that was located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor Anchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// The spatial anchor that was located.
        /// </summary>
        public string Identifier
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Specifies whether the anchor was located, or the reason why it may have failed.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.LocateAnchorStatus Status
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.LocateAnchorStatus result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_status(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that located the anchor.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Use this class to represent an anchor in space that can be persisted in a CloudSpatialAnchorSession.
    /// </summary>
    public partial class CloudSpatialAnchor
    {
        internal IntPtr handle;
        internal CloudSpatialAnchor(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_addref(ahandle);
        }
        public CloudSpatialAnchor()
        {
            status resultStatus = (NativeLibrary.ssc_cloud_spatial_anchor_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~CloudSpatialAnchor()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The anchor in the local mixed reality system.
        /// </summary>
        public IntPtr LocalAnchor
        {
            get
            {
                IntPtr result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_local_anchor(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_set_local_anchor(this.handle, value));
            }
        }

        /// <summary>
        /// The time the anchor will expire.
        /// </summary>
        public System.DateTimeOffset Expiration
        {
            get
            {
                long result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_expiration(this.handle, out result));
                return (result == 0) ? DateTimeOffset.MaxValue : DateTimeOffset.FromUnixTimeMilliseconds(result);
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_set_expiration(this.handle, (value == DateTimeOffset.MaxValue) ? 0 : value.ToUnixTimeMilliseconds()));
            }
        }

        /// <summary>
        /// The persistent identifier of this spatial anchor in the cloud service.
        /// </summary>
        public string Identifier
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A dictionary of application-defined properties that is stored with the anchor.
        /// </summary>
        public System.Collections.Generic.IDictionary<string, string> AppProperties
        {
            get
            {
                IntPtr result_handle;
                IDictionary_String_String result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_app_properties(this.handle, out result_handle));
                result_object = new IDictionary_String_String(result_handle, transfer:true);
                return result_object;
            }
        }

        /// <summary>
        /// An opaque version tag that can be used for concurrency control.
        /// </summary>
        public string VersionTag
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_version_tag(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Use this class to defer completing an operation.
    /// </summary>
    /// <remarks>
    /// This is similar to the Windows.Foundation.Deferral class.
    /// </remarks>
    public partial class CloudSpatialAnchorSessionDeferral
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSessionDeferral(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_addref(ahandle);
        }
        ~CloudSpatialAnchorSessionDeferral()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Mark the deferred operation as complete and perform any associated tasks.
        /// </summary>
        public void Complete()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_complete(this.handle));
        }

    }

    /// <summary>
    /// Use this class to configure session diagnostics that can be collected and submitted to improve system quality.
    /// </summary>
    public partial class CloudSpatialAnchorSessionDiagnostics
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSessionDiagnostics(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_addref(ahandle);
        }
        ~CloudSpatialAnchorSessionDiagnostics()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Level of tracing to log.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionLogLevel LogLevel
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionLogLevel result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_log_level(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_log_level(this.handle, value));
            }
        }

        /// <summary>
        /// Directory into which temporary log files and manifests are saved.
        /// </summary>
        public string LogDirectory
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_log_directory(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_log_directory(this.handle, value));
            }
        }

        /// <summary>
        /// Approximate maximum disk space to be used, in megabytes.
        /// </summary>
        /// <remarks>
        /// When this value is set to zero, no information will be written to disk.
        /// </remarks>
        public int MaxDiskSizeInMB
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_max_disk_size_in_mb(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_max_disk_size_in_mb(this.handle, value));
            }
        }

        /// <summary>
        /// Whether images should be logged.
        /// </summary>
        public bool ImagesEnabled
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_images_enabled(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_images_enabled(this.handle, value));
            }
        }

        /// <summary>
        /// Creates a manifest of log files and submission information to be uploaded.
        /// </summary>
        /// <param name="description">
        /// Description to be added to the diagnostics manifest.
        /// </param>
        public async System.Threading.Tasks.Task<string> CreateManifestAsync(string description)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_create_manifest_async(this.handle, description, out result));
                return result;
            });
        }

        /// <summary>
        /// Submits a diagnostics manifest and cleans up its resources.
        /// </summary>
        /// <param name="manifestPath">
        /// Path to the manifest file to submit.
        /// </param>
        public async System.Threading.Tasks.Task SubmitManifestAsync(string manifestPath)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_submit_manifest_async(this.handle, manifestPath));
            });
        }

    }

    /// <summary>
    /// Use this class to create, locate and manage spatial anchors.
    /// </summary>
    public partial class CloudSpatialAnchorSession : IDisposable, ICookie
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSession(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_addref(ahandle);
            CookieTracker<CloudSpatialAnchorSession>.Add(this);
        }
        /// <summary>
        /// Initializes a new instance with a default configuration.
        /// </summary>
        public CloudSpatialAnchorSession()
        {
            status resultStatus = (NativeLibrary.ssc_cloud_spatial_anchor_session_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
            CookieTracker<CloudSpatialAnchorSession>.Add(this);
            // Custom initialization (HoloLens/Unity) begins for CloudSpatialAnchorSession.
            // Custom initialization (HoloLens/Unity) ends for CloudSpatialAnchorSession.
        }

        ~CloudSpatialAnchorSession()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The configuration information for the session.
        /// </summary>
        /// <remarks>
        /// Configuration settings take effect when the session is started.
        /// </remarks>
        public Microsoft.Azure.SpatialAnchors.SessionConfiguration Configuration
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionConfiguration result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_configuration(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.SessionConfiguration(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// The diagnostics settings for the session, which can be used to collect and submit data for troubleshooting and improvements.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics Diagnostics
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_diagnostics(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// Logging level for the session log events.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionLogLevel LogLevel
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionLogLevel result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_log_level(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_log_level(this.handle, value));
            }
        }


        /// <summary>
        /// The unique identifier for the session.
        /// </summary>
        public string SessionId
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session_id(this.handle, out result));
                return result;
            }
        }

        private ulong cookie;
        ulong ICookie.Cookie { get { return this.cookie; } set { this.cookie = value; } }
        /// <summary>Registered callbacks on this instance.</summary>
        private event TokenRequiredDelegate _TokenRequired;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(TokenRequiredDelegateNative))]
        private static void TokenRequiredStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            TokenRequiredDelegate handler = (instance == null) ? null : instance._TokenRequired;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.TokenRequiredEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static TokenRequiredDelegateNative TokenRequiredStaticHandlerDelegate = TokenRequiredStaticHandler;
        public event TokenRequiredDelegate TokenRequired
        {
            add
            {
                this._TokenRequired += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_token_required(this.handle, this.cookie, TokenRequiredStaticHandlerDelegate));
            }
            remove
            {
                this._TokenRequired -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event AnchorLocatedDelegate _AnchorLocated;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(AnchorLocatedDelegateNative))]
        private static void AnchorLocatedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            AnchorLocatedDelegate handler = (instance == null) ? null : instance._AnchorLocated;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.AnchorLocatedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static AnchorLocatedDelegateNative AnchorLocatedStaticHandlerDelegate = AnchorLocatedStaticHandler;
        public event AnchorLocatedDelegate AnchorLocated
        {
            add
            {
                this._AnchorLocated += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_anchor_located(this.handle, this.cookie, AnchorLocatedStaticHandlerDelegate));
            }
            remove
            {
                this._AnchorLocated -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event LocateAnchorsCompletedDelegate _LocateAnchorsCompleted;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(LocateAnchorsCompletedDelegateNative))]
        private static void LocateAnchorsCompletedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            LocateAnchorsCompletedDelegate handler = (instance == null) ? null : instance._LocateAnchorsCompleted;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.LocateAnchorsCompletedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static LocateAnchorsCompletedDelegateNative LocateAnchorsCompletedStaticHandlerDelegate = LocateAnchorsCompletedStaticHandler;
        public event LocateAnchorsCompletedDelegate LocateAnchorsCompleted
        {
            add
            {
                this._LocateAnchorsCompleted += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_locate_anchors_completed(this.handle, this.cookie, LocateAnchorsCompletedStaticHandlerDelegate));
            }
            remove
            {
                this._LocateAnchorsCompleted -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event SessionUpdatedDelegate _SessionUpdated;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(SessionUpdatedDelegateNative))]
        private static void SessionUpdatedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            SessionUpdatedDelegate handler = (instance == null) ? null : instance._SessionUpdated;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.SessionUpdatedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static SessionUpdatedDelegateNative SessionUpdatedStaticHandlerDelegate = SessionUpdatedStaticHandler;
        public event SessionUpdatedDelegate SessionUpdated
        {
            add
            {
                this._SessionUpdated += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_session_updated(this.handle, this.cookie, SessionUpdatedStaticHandlerDelegate));
            }
            remove
            {
                this._SessionUpdated -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event SessionErrorDelegate _Error;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(SessionErrorDelegateNative))]
        private static void ErrorStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            SessionErrorDelegate handler = (instance == null) ? null : instance._Error;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.SessionErrorEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static SessionErrorDelegateNative ErrorStaticHandlerDelegate = ErrorStaticHandler;
        public event SessionErrorDelegate Error
        {
            add
            {
                this._Error += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_error(this.handle, this.cookie, ErrorStaticHandlerDelegate));
            }
            remove
            {
                this._Error -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event OnLogDebugDelegate _OnLogDebug;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(OnLogDebugDelegateNative))]
        private static void OnLogDebugStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            OnLogDebugDelegate handler = (instance == null) ? null : instance._OnLogDebug;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.OnLogDebugEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static OnLogDebugDelegateNative OnLogDebugStaticHandlerDelegate = OnLogDebugStaticHandler;
        public event OnLogDebugDelegate OnLogDebug
        {
            add
            {
                this._OnLogDebug += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_on_log_debug(this.handle, this.cookie, OnLogDebugStaticHandlerDelegate));
            }
            remove
            {
                this._OnLogDebug -= value;
            }
        }

        /// <summary>
        /// Stops this session and releases all associated resources.
        /// </summary>
        public void Dispose()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_dispose(this.handle));
        }

        /// <summary>
        /// Gets the Azure Spatial Anchors access token from authentication token.
        /// </summary>
        /// <param name="authenticationToken">
        /// Authentication token.
        /// </param>
        public async System.Threading.Tasks.Task<string> GetAccessTokenWithAuthenticationTokenAsync(string authenticationToken)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_access_token_with_authentication_token_async(this.handle, authenticationToken, out result));
                return result;
            });
        }

        /// <summary>
        /// Gets the Azure Spatial Anchors access token from account key.
        /// </summary>
        /// <param name="accountKey">
        /// Account key.
        /// </param>
        public async System.Threading.Tasks.Task<string> GetAccessTokenWithAccountKeyAsync(string accountKey)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_access_token_with_account_key_async(this.handle, accountKey, out result));
                return result;
            });
        }

        /// <summary>
        /// Creates a new persisted spatial anchor from the specified local anchor and string properties.
        /// </summary>
        /// <param name="anchor">
        /// Anchor to be persisted.
        /// </param>
        public async System.Threading.Tasks.Task CreateAnchorAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_create_anchor_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Creates a new object that watches for anchors that meet the specified criteria.
        /// </summary>
        /// <param name="criteria">
        /// Criteria for anchors to watch for.
        /// </param>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher CreateWatcher(Microsoft.Azure.SpatialAnchors.AnchorLocateCriteria criteria)
        {
            IntPtr result_handle;
            Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_create_watcher(this.handle, criteria.handle, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
            return result_object;
        }

        /// <summary>
        /// Gets a cloud spatial anchor for the given identifier, even if it hasn't been located yet.
        /// </summary>
        /// <param name="identifier">
        /// The identifier to look for.
        /// </param>
        public async System.Threading.Tasks.Task<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor> GetAnchorPropertiesAsync(string identifier)
        {
            return await Task.Run(() =>
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_anchor_properties_async(this.handle, identifier, out result_handle));
                result_object = new CloudSpatialAnchor(result_handle, transfer:true);
                return result_object;
            });
        }

        /// <summary>
        /// Gets a list of active watchers.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher> GetActiveWatchers()
        {
            System.Collections.Generic.IReadOnlyList<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher> result;
            IntPtr[] result_array;
            int result_count;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_active_watchers_count(this.handle, out result_count));
            result_array = new IntPtr[result_count];
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_active_watchers_items(this.handle, result_array, ref result_count));
            result = result_array.Take(result_count).Select(handle => new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(handle, transfer:true)).ToList().AsReadOnly();
            return result;
        }

        /// <summary>
        /// Refreshes properties for the specified spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to refresh.
        /// </param>
        public async System.Threading.Tasks.Task RefreshAnchorPropertiesAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_refresh_anchor_properties_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Updates the specified spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to be updated.
        /// </param>
        public async System.Threading.Tasks.Task UpdateAnchorPropertiesAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_update_anchor_properties_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Deletes a persisted spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to be deleted.
        /// </param>
        public async System.Threading.Tasks.Task DeleteAnchorAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_delete_anchor_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Gets an object describing the status of the session.
        /// </summary>
        public async System.Threading.Tasks.Task<Microsoft.Azure.SpatialAnchors.SessionStatus> GetSessionStatusAsync()
        {
            return await Task.Run(() =>
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionStatus result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session_status_async(this.handle, out result_handle));
                result_object = new SessionStatus(result_handle, transfer:true);
                return result_object;
            });
        }

        /// <summary>
        /// Begins capturing environment data for the session.
        /// </summary>
        public void Start()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_start(this.handle));
        }

        /// <summary>
        /// Stops capturing environment data for the session and cancels any outstanding locate operations. Environment data is maintained.
        /// </summary>
        public void Stop()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_stop(this.handle));
        }

        /// <summary>
        /// Resets environment data that has been captured in this session; applications must call this method when tracking is lost.
        /// </summary>
        /// <remarks>
        /// On any platform, calling the method will clean all internal cached state.
        /// </remarks>
        public void Reset()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_reset(this.handle));
        }

    }

    /// <summary>
    /// Use this class to control an object that watches for spatial anchors.
    /// </summary>
    public partial class CloudSpatialAnchorWatcher
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorWatcher(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_watcher_addref(ahandle);
        }
        ~CloudSpatialAnchorWatcher()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Distinct identifier for the watcher within its session.
        /// </summary>
        public int Identifier
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Stops further activity from this watcher.
        /// </summary>
        public void Stop()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_stop(this.handle));
        }

    }

    /// <summary>
    /// Use this type to determine when a locate operation has completed.
    /// </summary>
    public partial class LocateAnchorsCompletedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal LocateAnchorsCompletedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_locate_anchors_completed_event_args_addref(ahandle);
        }
        ~LocateAnchorsCompletedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Gets a value indicating whether the locate operation was canceled.
        /// </summary>
        /// <remarks>
        /// When this property is true, the watcher was stopped before completing.
        /// </remarks>
        public bool Cancelled
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_get_cancelled(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that completed the locate operation.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Use this class to describe how anchors to be located should be near a source anchor.
    /// </summary>
    public partial class NearAnchorCriteria
    {
        internal IntPtr handle;
        internal NearAnchorCriteria(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_near_anchor_criteria_addref(ahandle);
        }
        public NearAnchorCriteria()
        {
            status resultStatus = (NativeLibrary.ssc_near_anchor_criteria_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~NearAnchorCriteria()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Source anchor around which nearby anchors should be located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor SourceAnchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_source_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor(result_handle, transfer:true) : null;
                return result_object;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_source_anchor(this.handle, value.handle));
            }
        }

        /// <summary>
        /// Maximum distance in meters from the source anchor (defaults to 5).
        /// </summary>
        public float DistanceInMeters
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_distance_in_meters(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_distance_in_meters(this.handle, value));
            }
        }

        /// <summary>
        /// Maximum desired result count (defaults to 20).
        /// </summary>
        public int MaxResultCount
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_max_result_count(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_max_result_count(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires for logging messages.
    /// </summary>
    public partial class OnLogDebugEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal OnLogDebugEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_on_log_debug_event_args_addref(ahandle);
        }
        ~OnLogDebugEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_on_log_debug_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The logging message.
        /// </summary>
        public string Message
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_on_log_debug_event_args_get_message(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Use this class to set up the service configuration for a SpatialAnchorSession.
    /// </summary>
    public partial class SessionConfiguration
    {
        internal IntPtr handle;
        internal SessionConfiguration(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_configuration_addref(ahandle);
        }
        ~SessionConfiguration()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Account domain for the Azure Spatial Anchors service.
        /// </summary>
        /// <remarks>
        /// The default is "mixedreality.azure.com".
        /// </remarks>
        public string AccountDomain
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_domain(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_domain(this.handle, value));
            }
        }

        /// <summary>
        /// Account-level ID for the Azure Spatial Anchors service.
        /// </summary>
        public string AccountId
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_id(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_id(this.handle, value));
            }
        }

        /// <summary>
        /// Authentication token for Azure Active Directory (AAD).
        /// </summary>
        /// <remarks>
        /// If the access token and the account key are missing, the session will obtain an access token based on this value.
        /// </remarks>
        public string AuthenticationToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_authentication_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_authentication_token(this.handle, value));
            }
        }

        /// <summary>
        /// Account-level key for the Azure Spatial Anchors service.
        /// </summary>
        public string AccountKey
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_key(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_key(this.handle, value));
            }
        }

        /// <summary>
        /// Access token for the Azure Spatial Anchors service.
        /// </summary>
        public string AccessToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_access_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_access_token(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires when errors are thrown.
    /// </summary>
    public partial class SessionErrorEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal SessionErrorEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_error_event_args_addref(ahandle);
        }
        ~SessionErrorEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_get_error_message(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that found an error, possibly null.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// This type describes the status of spatial data processing.
    /// </summary>
    public partial class SessionStatus
    {
        internal IntPtr handle;
        internal SessionStatus(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_status_addref(ahandle);
        }
        ~SessionStatus()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The level of data sufficiency for a successful operation.
        /// </summary>
        /// <remarks>
        /// This value will be in the [0;1) range when data is insufficient; 1 when data is sufficient for success and greater than 1 when conditions are better than minimally sufficient.
        /// </remarks>
        public float ReadyForCreateProgress
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_ready_for_create_progress(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The ratio of data available to recommended data to create an anchor.
        /// </summary>
        /// <remarks>
        /// This value will be in the [0;1) range when data is below the recommended threshold; 1 and greater when the recommended amount of data has been gathered for a creation operation.
        /// </remarks>
        public float RecommendedForCreateProgress
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_recommended_for_create_progress(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A hash value that can be used to know when environment data that contributes to a creation operation has changed to included the latest input data.
        /// </summary>
        /// <remarks>
        /// If the hash value does not change after new frames were added to the session, then those frames were not deemed as sufficientlyy different from existing environment data and disgarded. This value may be 0 (and should be ignored) for platforms that don't feed frames individually.
        /// </remarks>
        public int SessionCreateHash
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_session_create_hash(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A hash value that can be used to know when environment data that contributes to a locate operation has changed to included the latest input data.
        /// </summary>
        /// <remarks>
        /// If the hash value does not change after new frames were added to the session, then those frames were not deemed as sufficiency different from existing environment data and disgarded. This value may be 0 (and should be ignored) for platforms that don't feed frames individually.
        /// </remarks>
        public int SessionLocateHash
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_session_locate_hash(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Feedback that can be provided to user about data processing status.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionUserFeedback UserFeedback
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionUserFeedback result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_user_feedback(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires when the session state is updated.
    /// </summary>
    public partial class SessionUpdatedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal SessionUpdatedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_updated_event_args_addref(ahandle);
        }
        ~SessionUpdatedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_updated_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Current session status.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionStatus Status
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionStatus result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_updated_event_args_get_status(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.SessionStatus(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Informs the application that the service requires an updated access token or authentication token.
    /// </summary>
    public partial class TokenRequiredEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal TokenRequiredEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_token_required_event_args_addref(ahandle);
        }
        ~TokenRequiredEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The access token to be used by the operation that requires it.
        /// </summary>
        public string AccessToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_access_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_set_access_token(this.handle, value));
            }
        }

        /// <summary>
        /// The authentication token to be used by the operation that requires it.
        /// </summary>
        public string AuthenticationToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_authentication_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_set_authentication_token(this.handle, value));
            }
        }

        /// <summary>
        /// Returns a deferral object that can be used to provide an updated access token or authentication token from another asynchronous operation.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral GetDeferral()
        {
            IntPtr result_handle;
            Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral result_object;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_deferral(this.handle, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral(result_handle, transfer:true) : null;
            return result_object;
        }

    }

}

#elif UNITY_ANDROID
//
// Spatial Services Client
// This file was auto-generated with sscapigen based on SscApiModelDirect.cs, hash oCDnGdnaFmKPFORSJrQ6mQ==
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.SpatialAnchors
{
    internal enum status
    {
        /// <summary>
        /// Success
        /// </summary>
        OK = 0,
        /// <summary>
        /// Failed
        /// </summary>
        Failed = 1,
        /// <summary>
        /// Cannot access a disposed object.
        /// </summary>
        ObjectDisposed = 2,
        /// <summary>
        /// Out of memory.
        /// </summary>
        OutOfMemory = 12,
        /// <summary>
        /// Invalid argument.
        /// </summary>
        InvalidArgument = 22,
        /// <summary>
        /// The value is out of range.
        /// </summary>
        OutOfRange = 34,
        /// <summary>
        /// Not implemented.
        /// </summary>
        NotImplemented = 38,
        /// <summary>
        /// The key does not exist in the collection.
        /// </summary>
        KeyNotFound = 77,
        /// <summary>
        /// Amount of Metadata exceeded the allowed limit (currently 4k)
        /// </summary>
        MetadataTooLarge = 78,
        /// <summary>
        /// Application did not provide valid credentials and therefore could not authenticate with the Cloud Service
        /// </summary>
        ApplicationNotAuthenticated = 79,
        /// <summary>
        /// Application did not provide any credentials for authorization with the Cloud Service
        /// </summary>
        ApplicationNotAuthorized = 80,
        /// <summary>
        /// Multiple stores (on the same device or different devices) made concurrent changes to the same Spatial Entity and so this particular change was rejected
        /// </summary>
        ConcurrencyViolation = 81,
        /// <summary>
        /// Not enough Neighborhood Spatial Data was available to complete the desired Create operation
        /// </summary>
        NotEnoughSpatialData = 82,
        /// <summary>
        /// No Spatial Location Hint was available (or it was not specific enough) to support rediscovery from the Cloud at a later time
        /// </summary>
        NoSpatialLocationHint = 83,
        /// <summary>
        /// Application cannot connect to the Cloud Service
        /// </summary>
        CannotConnectToServer = 84,
        /// <summary>
        /// Cloud Service returned an unspecified error
        /// </summary>
        ServerError = 85,
        /// <summary>
        /// The Spatial Entity has already been associated with a different Store object, so cannot be used with this current Store object.
        /// </summary>
        AlreadyAssociatedWithADifferentStore = 86,
        /// <summary>
        /// SpatialEntity already exists in a Store but TryCreateAsync was called.
        /// </summary>
        AlreadyExists = 87,
        /// <summary>
        /// A locate operation was requested, but the criteria does not specify anything to look for.
        /// </summary>
        NoLocateCriteriaSpecified = 88,
        /// <summary>
        /// An access token was required but not specified; handle the TokenRequired event on the session to provide one.
        /// </summary>
        NoAccessTokenSpecified = 89,
        /// <summary>
        /// The session was unable to obtain an access token and so the creation could not proceed
        /// </summary>
        UnableToObtainAccessToken = 90,
    }

    internal static class NativeLibraryHelpers
    {
        internal static string[] IntPtrToStringArray(IntPtr result, int result_length)
        {
            byte[] bytes = new byte[result_length];
            System.Runtime.InteropServices.Marshal.Copy(result, bytes, 0, result_length - 1);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(result);
            return System.Text.Encoding.UTF8.GetString(bytes).Split('\0');
        }

        internal static void CheckStatus(IntPtr handle, status value)
        {
            if (value == status.OK)
            {
                return;
            }

            string message;
            string requestCorrelationVector;
            string responseCorrelationVector;

            status code = NativeLibrary.ssc_get_error_details(handle, out message, out requestCorrelationVector, out responseCorrelationVector);

            string fullMessage;
            if (code == status.Failed)
            {
                throw new InvalidOperationException("Unexpected error in exception handling.");
            }
            else if (code != status.OK)
            {
                fullMessage = "Exception thrown and an unexpected error in exception handling.";
            }
            else
            {
                fullMessage = message + ". Request CV: " + requestCorrelationVector + ". Response CV: " + responseCorrelationVector + ".";
            }

            switch (value)
            {
                case status.OK:
                    return;
                case status.Failed:
                    throw new InvalidOperationException(fullMessage);
                case status.ObjectDisposed:
                    throw new ObjectDisposedException(fullMessage);
                case status.OutOfMemory:
                    throw new OutOfMemoryException(fullMessage);
                case status.InvalidArgument:
                    throw new ArgumentException(fullMessage);
                case status.OutOfRange:
                    throw new ArgumentOutOfRangeException("", fullMessage);
                case status.NotImplemented:
                    throw new NotImplementedException(fullMessage);
                case status.KeyNotFound:
                    throw new KeyNotFoundException(fullMessage);
                case status.MetadataTooLarge:
                    throw new CloudSpatialException(CloudSpatialErrorCode.MetadataTooLarge, message, requestCorrelationVector, responseCorrelationVector);
                case status.ApplicationNotAuthenticated:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ApplicationNotAuthenticated, message, requestCorrelationVector, responseCorrelationVector);
                case status.ApplicationNotAuthorized:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ApplicationNotAuthorized, message, requestCorrelationVector, responseCorrelationVector);
                case status.ConcurrencyViolation:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ConcurrencyViolation, message, requestCorrelationVector, responseCorrelationVector);
                case status.NotEnoughSpatialData:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NotEnoughSpatialData, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoSpatialLocationHint:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoSpatialLocationHint, message, requestCorrelationVector, responseCorrelationVector);
                case status.CannotConnectToServer:
                    throw new CloudSpatialException(CloudSpatialErrorCode.CannotConnectToServer, message, requestCorrelationVector, responseCorrelationVector);
                case status.ServerError:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ServerError, message, requestCorrelationVector, responseCorrelationVector);
                case status.AlreadyAssociatedWithADifferentStore:
                    throw new CloudSpatialException(CloudSpatialErrorCode.AlreadyAssociatedWithADifferentStore, message, requestCorrelationVector, responseCorrelationVector);
                case status.AlreadyExists:
                    throw new CloudSpatialException(CloudSpatialErrorCode.AlreadyExists, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoLocateCriteriaSpecified:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoLocateCriteriaSpecified, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoAccessTokenSpecified:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoAccessTokenSpecified, message, requestCorrelationVector, responseCorrelationVector);
                case status.UnableToObtainAccessToken:
                    throw new CloudSpatialException(CloudSpatialErrorCode.UnableToObtainAccessToken, message, requestCorrelationVector, responseCorrelationVector);
            }
        }
    }

    /// <summary>This interface is implemented by classes with events to help track callbacks.</summary>
    internal interface ICookie
    {
        /// <summary>Unique cookie value for callback identification.</summary>
        ulong Cookie { get; set; }
    }

    internal static class CookieTracker<T> where T : class, ICookie
    {
        private static ulong lastCookie;
        private static Dictionary<ulong, System.WeakReference<T>> tracked = new Dictionary<ulong, System.WeakReference<T>>();
        internal static void Add(T instance)
        {
            lock (tracked)
            {
                instance.Cookie = ++lastCookie;
                tracked[instance.Cookie] = new System.WeakReference<T>(instance);
            }
        }
        internal static T Lookup(ulong cookie)
        {
            T result;
            System.WeakReference<T> reference;
            bool found;
            lock (tracked)
            {
                found = tracked.TryGetValue(cookie, out reference);
            }
            if (!found)
            {
                return null;
            }
            found = reference.TryGetTarget(out result);
            if (!found)
            {
                lock (tracked)
                {
                    tracked.Remove(cookie);
                }
            }
            return result;
        }
        internal static void Remove(T instance)
        {
            lock (tracked)
            {
                tracked.Remove(instance.Cookie);
            }
        }
    }

    /// <summary>
    /// Informs the application that a locate operation has completed.
    /// </summary>
    /// <param name="sender">
    /// The session that ran the locate operation.
    /// </param>
    /// <param name="args">
    /// The arguments describing the operation completion.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void LocateAnchorsCompletedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session requires an updated access token or authentication token.
    /// </summary>
    /// <param name="sender">
    /// The session that requires an updated access token or authentication token.
    /// </param>
    /// <param name="args">
    /// The event arguments that require an AccessToken property or an AuthenticationToken property to be set.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void TokenRequiredDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session has located an anchor or discovered that it cannot yet be located.
    /// </summary>
    /// <param name="sender">
    /// The session that fires the event.
    /// </param>
    /// <param name="args">
    /// Information about the located anchor.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void AnchorLocatedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session has been updated with new information.
    /// </summary>
    /// <param name="sender">
    /// The session that has been updated.
    /// </param>
    /// <param name="args">
    /// Information about the current session status.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SessionUpdatedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that an error occurred in a session.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the error.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SessionErrorDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application of a debug log message.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the log.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void OnLogDebugDelegateNative(ulong cookie, IntPtr args);

    internal static class NativeLibrary
    {
        internal const string DllName = "azurespatialanchorsndk";
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_get_cancelled(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_get_identifier(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_stop(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_local_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_set_local_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_expiration(IntPtr handle, out long result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_set_expiration(IntPtr handle, long value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_identifier(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_app_properties(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_count(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_clear(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_key(IntPtr handle, Int32 index, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_item(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_set_item(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_remove_key(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string key);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_version_tag(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_identifier(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_status(IntPtr handle, out LocateAnchorStatus result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_domain(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_domain(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_id(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_id(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_key(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_key(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_configuration(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_diagnostics(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_log_level(IntPtr handle, out SessionLogLevel result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_log_level(IntPtr handle, SessionLogLevel value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_log_directory(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_log_directory(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_max_disk_size_in_mb(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_max_disk_size_in_mb(IntPtr handle, Int32 value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_images_enabled(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_images_enabled(IntPtr handle, Boolean value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_create_manifest_async(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string description, [MarshalAs(UnmanagedType.LPStr)] out String result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_submit_manifest_async(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string manifest_path);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_log_level(IntPtr handle, out SessionLogLevel result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_log_level(IntPtr handle, SessionLogLevel value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_session(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session_id(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_middleware_versions(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_middleware_versions(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_sdk_package_type(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_sdk_package_type(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_token_required(IntPtr handle, ulong value, TokenRequiredDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_set_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_set_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_deferral(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_complete(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_anchor_located(IntPtr handle, ulong value, AnchorLocatedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_locate_anchors_completed(IntPtr handle, ulong value, LocateAnchorsCompletedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_session_updated(IntPtr handle, ulong value, SessionUpdatedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_get_status(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_ready_for_create_progress(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_recommended_for_create_progress(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_session_create_hash(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_session_locate_hash(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_user_feedback(IntPtr handle, out SessionUserFeedback result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_error(IntPtr handle, ulong value, SessionErrorDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_get_error_message(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_on_log_debug(IntPtr handle, ulong value, OnLogDebugDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_get_message(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_dispose(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_access_token_with_authentication_token_async(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string authentication_token, [MarshalAs(UnmanagedType.LPStr)] out String result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_access_token_with_account_key_async(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string account_key, [MarshalAs(UnmanagedType.LPStr)] out String result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create_anchor_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create_watcher(IntPtr handle, IntPtr criteria, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_anchor_locate_criteria_get_identifiers_flat", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_identifiers(IntPtr handle, out IntPtr result, out int result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_identifiers(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPStr)] String[] value, int value_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_bypass_cache(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_bypass_cache(IntPtr handle, Boolean value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_near_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_source_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_source_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_distance_in_meters(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_distance_in_meters(IntPtr handle, Single value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_max_result_count(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_max_result_count(IntPtr handle, Int32 value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_near_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_requested_categories(IntPtr handle, out AnchorDataCategory result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_requested_categories(IntPtr handle, AnchorDataCategory value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_strategy(IntPtr handle, out LocateStrategy result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_strategy(IntPtr handle, LocateStrategy value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_anchor_properties_async(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string identifier, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_active_watchers_count(IntPtr handle, out Int32 result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_active_watchers_items(IntPtr handle, [MarshalAs(UnmanagedType.LPArray), In, Out] IntPtr[] result_array, ref Int32 result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_refresh_anchor_properties_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_update_anchor_properties_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_delete_anchor_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_process_frame(IntPtr handle, IntPtr frame);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session_status_async(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_start(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_stop(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_reset(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_get_error_details(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] out string result_message, [MarshalAs(UnmanagedType.LPStr)] out string result_requestCorrelationVector, [MarshalAs(UnmanagedType.LPStr)] out string result_responseCorrelationVector);
    }

    // CODE STARTS HERE

    abstract class BasePrivateDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected abstract int InternalGetCount();
        protected abstract TKey InternalGetKey(int index);
        protected abstract TValue InternalGetItem(TKey key);
        protected abstract void InternalSetItem(TKey key, TValue value);
        protected abstract void InternalRemoveKey(TKey key);

        public TValue this[TKey key] { get { return InternalGetItem(key); } set { InternalSetItem(key, value); } }

        public ICollection<TKey> Keys { get { return Enumerable.Range(0, InternalGetCount()).Select(n => InternalGetKey(n)).ToList().AsReadOnly(); } }

        public ICollection<TValue> Values { get { return Enumerable.Range(0, InternalGetCount()).Select(n => InternalGetKey(n)).Select(k => InternalGetItem(k)).ToList().AsReadOnly(); } }

        public int Count { get { return InternalGetCount(); } }

        public bool IsReadOnly { get { return false; } }

        public void Add(TKey key, TValue value)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                InternalSetItem(key, value);
                return;
            }
            throw new ArgumentException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            while (InternalGetCount() > 0)
            {
                TKey key = InternalGetKey(0);
                InternalRemoveKey(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            try
            {
                TValue value = InternalGetItem(item.Key);
                if (Comparer<TValue>.Default.Compare(value, item.Value) == 0)
                {
                    return true;
                }
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                yield return new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }

        public bool Remove(TKey key)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            InternalRemoveKey(key);
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = InternalGetItem(key);
                return true;
            }
            catch (KeyNotFoundException)
            {
                value = default(TValue);
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                yield return new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }
    }

    abstract class BasePrivateList<T> : IList<T>
    {
        protected abstract int InternalGetCount();
        protected abstract T InternalGetItem(int index);
        protected abstract void InternalSetItem(int index, T value);
        protected abstract void InternalRemoveItem(int index);

        public int Count { get { return InternalGetCount(); } }

        public bool IsReadOnly { get { return false; } }

        public T this[int index] { get { return InternalGetItem(index); } set { InternalSetItem(index, value); } }

        public int IndexOf(T item)
        {
            int count = InternalGetCount();
            for (int i = 0; i < count; i++)
            {
                if (Comparer<T>.Default.Compare(item, InternalGetItem(i)) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            InternalSetItem(index, item);
        }

        public void RemoveAt(int index)
        {
            InternalRemoveItem(index);
        }

        public void Add(T item)
        {
            InternalSetItem(InternalGetCount(), item);
        }

        public void Clear()
        {
            while (InternalGetCount() > 0)
            {
                InternalRemoveItem(0);
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; ++i)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0) return false;
            InternalRemoveItem(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }
    }

    class IDictionary_String_String : BasePrivateDictionary<String, String>
    {
        internal IntPtr handle;
        internal IDictionary_String_String(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_idictionary_string_string_addref(ahandle);
        }
        ~IDictionary_String_String()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_release(this.handle));
            this.handle = IntPtr.Zero;
        }
        protected override int InternalGetCount()
        {
            int result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_count(this.handle, out result));
            return result;
        }
        protected override String InternalGetKey(int index)
        {
            string result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_key(this.handle, index, out result));
            return result;
        }
        protected override String InternalGetItem(String key)
        {
            string result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_item(this.handle, key, out result));
            return result;
        }
        protected override void InternalSetItem(String key, String value)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_set_item(this.handle, key, value));
        }
        protected override void InternalRemoveKey(String key)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_remove_key(this.handle, key));
        }
    }
    public enum SessionLogLevel : int
    {
        /// <summary>
        /// Specifies that logging should not write any messages.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies logs that indicate when the current flow of execution stops due to a failure.
        /// </summary>
        Error = 1,
        /// <summary>
        /// Specifies logs that highlight an abnormal or unexpected event, but do not otherwise cause execution to stop.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Specifies logs that track the general flow.
        /// </summary>
        Information = 3,
        /// <summary>
        /// Specifies logs used for interactive investigation during development.
        /// </summary>
        Debug = 4,
        /// <summary>
        /// Specifies all messages should be logged.
        /// </summary>
        All = 5,
    }

    public enum LocateAnchorStatus : int
    {
        /// <summary>
        /// The anchor was already being tracked.
        /// </summary>
        AlreadyTracked = 0,
        /// <summary>
        /// The anchor was found.
        /// </summary>
        Located = 1,
        /// <summary>
        /// The anchor was not found.
        /// </summary>
        NotLocated = 2,
        /// <summary>
        /// The anchor cannot be found - it was deleted or the identifier queried for was incorrect.
        /// </summary>
        NotLocatedAnchorDoesNotExist = 3,
    }

    public enum SessionUserFeedback : int
    {
        /// <summary>
        /// No specific feedback is available.
        /// </summary>
        None = 0,
        /// <summary>
        /// Device is not moving enough to create a neighborhood of key-frames.
        /// </summary>
        NotEnoughMotion = 1,
        /// <summary>
        /// Device is moving too quickly for stable tracking.
        /// </summary>
        MotionTooQuick = 2,
        /// <summary>
        /// The environment doesn't have enough feature points for stable tracking.
        /// </summary>
        NotEnoughFeatures = 4,
    }

    public enum AnchorDataCategory : int
    {
        /// <summary>
        /// No data is returned.
        /// </summary>
        None = 0,
        /// <summary>
        /// Returns Anchor properties including AppProperties.
        /// </summary>
        Properties = 1,
        /// <summary>
        /// Returns spatial information about an Anchor.
        /// </summary>
        /// <remarks>
        /// Returns a LocalAnchor for any returned CloudSpatialAnchors from the service.
        /// </remarks>
        Spatial = 2,
    }

    public enum LocateStrategy : int
    {
        /// <summary>
        /// Indicates that any method is acceptable.
        /// </summary>
        AnyStrategy = 0,
        /// <summary>
        /// Indicates that anchors will be located primarily by visual information.
        /// </summary>
        VisualInformation = 1,
        /// <summary>
        /// Indicates that anchors will be located primarily by relationship to other anchors.
        /// </summary>
        Relationship = 2,
    }

    public enum CloudSpatialErrorCode : int
    {
        /// <summary>
        /// Amount of Metadata exceeded the allowed limit (currently 4k)
        /// </summary>
        MetadataTooLarge = 0,
        /// <summary>
        /// Application did not provide valid credentials and therefore could not authenticate with the Cloud Service
        /// </summary>
        ApplicationNotAuthenticated = 1,
        /// <summary>
        /// Application did not provide any credentials for authorization with the Cloud Service
        /// </summary>
        ApplicationNotAuthorized = 2,
        /// <summary>
        /// Multiple stores (on the same device or different devices) made concurrent changes to the same Spatial Entity and so this particular change was rejected
        /// </summary>
        ConcurrencyViolation = 3,
        /// <summary>
        /// Not enough Neighborhood Spatial Data was available to complete the desired Create operation
        /// </summary>
        NotEnoughSpatialData = 4,
        /// <summary>
        /// No Spatial Location Hint was available (or it was not specific enough) to support rediscovery from the Cloud at a later time
        /// </summary>
        NoSpatialLocationHint = 5,
        /// <summary>
        /// Application cannot connect to the Cloud Service
        /// </summary>
        CannotConnectToServer = 6,
        /// <summary>
        /// Cloud Service returned an unspecified error
        /// </summary>
        ServerError = 7,
        /// <summary>
        /// The Spatial Entity has already been associated with a different Store object, so cannot be used with this current Store object.
        /// </summary>
        AlreadyAssociatedWithADifferentStore = 8,
        /// <summary>
        /// SpatialEntity already exists in a Store but TryCreateAsync was called.
        /// </summary>
        AlreadyExists = 9,
        /// <summary>
        /// A locate operation was requested, but the criteria does not specify anything to look for.
        /// </summary>
        NoLocateCriteriaSpecified = 10,
        /// <summary>
        /// An access token was required but not specified; handle the TokenRequired event on the session to provide one.
        /// </summary>
        NoAccessTokenSpecified = 11,
        /// <summary>
        /// The session was unable to obtain an access token and so the creation could not proceed
        /// </summary>
        UnableToObtainAccessToken = 12,
    }

    /// <summary>
    /// Informs the application that a locate operation has completed.
    /// </summary>
    /// <param name="sender">
    /// The session that ran the locate operation.
    /// </param>
    /// <param name="args">
    /// The arguments describing the operation completion.
    /// </param>
    public delegate void LocateAnchorsCompletedDelegate(object sender, Microsoft.Azure.SpatialAnchors.LocateAnchorsCompletedEventArgs args);

    /// <summary>
    /// Informs the application that a session requires an updated access token or authentication token.
    /// </summary>
    /// <param name="sender">
    /// The session that requires an updated access token or authentication token.
    /// </param>
    /// <param name="args">
    /// The event arguments that require an AccessToken property or an AuthenticationToken property to be set.
    /// </param>
    public delegate void TokenRequiredDelegate(object sender, Microsoft.Azure.SpatialAnchors.TokenRequiredEventArgs args);

    /// <summary>
    /// Informs the application that a session has located an anchor or discovered that it cannot yet be located.
    /// </summary>
    /// <param name="sender">
    /// The session that fires the event.
    /// </param>
    /// <param name="args">
    /// Information about the located anchor.
    /// </param>
    public delegate void AnchorLocatedDelegate(object sender, Microsoft.Azure.SpatialAnchors.AnchorLocatedEventArgs args);

    /// <summary>
    /// Informs the application that a session has been updated with new information.
    /// </summary>
    /// <param name="sender">
    /// The session that has been updated.
    /// </param>
    /// <param name="args">
    /// Information about the current session status.
    /// </param>
    public delegate void SessionUpdatedDelegate(object sender, Microsoft.Azure.SpatialAnchors.SessionUpdatedEventArgs args);

    /// <summary>
    /// Informs the application that an error occurred in a session.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the error.
    /// </param>
    public delegate void SessionErrorDelegate(object sender, Microsoft.Azure.SpatialAnchors.SessionErrorEventArgs args);

    /// <summary>
    /// Informs the application of a debug log message.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the log.
    /// </param>
    public delegate void OnLogDebugDelegate(object sender, Microsoft.Azure.SpatialAnchors.OnLogDebugEventArgs args);

    /// <summary>
    /// The exception that is thrown when an error occurs processing cloud spatial anchors.
    /// </summary>
    public class CloudSpatialException : Exception
    {
        private CloudSpatialErrorCode code;
        private string requestCorrelationVector;
        private string responseCorrelationVector;

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        public CloudSpatialException()
        {
            this.code = default(CloudSpatialErrorCode);
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code)
        {
            this.code = code;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message) : base(message)
        {
            this.code = code;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        /// <param name='requestCorrelationVector'>Request correlation vector for this exception.</param>
        /// <param name='responseCorrelationVector'>Response correlation vector for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message, string requestCorrelationVector, string responseCorrelationVector) : base(message)
        {
            this.code = code;
            this.requestCorrelationVector = requestCorrelationVector;
            this.responseCorrelationVector = responseCorrelationVector;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        /// <param name='requestCorrelationVector'>Request correlation vector for this exception.</param>
        /// <param name='responseCorrelationVector'>Response correlation vector for this exception.</param>
        /// <param name='innerException'>Exception that caused this exception to be thrown.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message, string requestCorrelationVector, string responseCorrelationVector, Exception inner) : base(message, inner)
        {
            this.code = code;
            this.requestCorrelationVector = requestCorrelationVector;
            this.responseCorrelationVector = responseCorrelationVector;
        }

        /// <summary>
        /// The error code associated with this exception.
        /// </summary>
        public CloudSpatialErrorCode ErrorCode
        {
            get { return this.code; }
        }

        /// <summary>
        /// The request correlation vector associated with this exception.
        /// </summary>
        public string RequestCorrelationVector
        {
            get { return this.requestCorrelationVector; }
        }

        /// <summary>
        /// The response correlation vector associated with this exception.
        /// </summary>
        public string ResponseCorrelationVector
        {
            get { return this.responseCorrelationVector; }
        }

    }

    /// <summary>
    /// Specifies a set of criteria for locating anchors.
    /// </summary>
    /// <remarks>
    /// Within the object, properties are combined with the AND operator. For example, if identifiers and nearAnchor are specified, then the filter will look for anchors that are near the nearAnchor and have an identifier that matches any of those identifiers.
    /// </remarks>
    public partial class AnchorLocateCriteria
    {
        internal IntPtr handle;
        internal AnchorLocateCriteria(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_anchor_locate_criteria_addref(ahandle);
        }
        public AnchorLocateCriteria()
        {
            status resultStatus = (NativeLibrary.ssc_anchor_locate_criteria_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~AnchorLocateCriteria()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Cloud anchor identifiers to locate. If empty, any anchors can be located.
        /// </summary>
        /// <remarks>
        /// Any anchors within this list will match this criteria.
        /// </remarks>
        public string[] Identifiers
        {
            get
            {
                IntPtr result;
                int result_length;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_identifiers(this.handle, out result, out result_length));
                return NativeLibraryHelpers.IntPtrToStringArray(result, result_length);
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_identifiers(this.handle, value, value.Length));
            }
        }

        /// <summary>
        /// Whether locate should bypass the local cache of anchors.
        /// </summary>
        public bool BypassCache
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_bypass_cache(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_bypass_cache(this.handle, value));
            }
        }

        /// <summary>
        /// Filters anchors to locate to be close to a specific anchor.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.NearAnchorCriteria NearAnchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.NearAnchorCriteria result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_near_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.NearAnchorCriteria(result_handle, transfer:true) : null;
                return result_object;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_near_anchor(this.handle, value.handle));
            }
        }

        /// <summary>
        /// Categories of data that are requested.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.AnchorDataCategory RequestedCategories
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.AnchorDataCategory result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_requested_categories(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_requested_categories(this.handle, value));
            }
        }

        /// <summary>
        /// Indicates the strategy by which anchors will be located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.LocateStrategy Strategy
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.LocateStrategy result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_strategy(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_strategy(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Use this type to determine the status of an anchor after a locate operation.
    /// </summary>
    public partial class AnchorLocatedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal AnchorLocatedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_anchor_located_event_args_addref(ahandle);
        }
        ~AnchorLocatedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The cloud spatial anchor that was located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor Anchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// The spatial anchor that was located.
        /// </summary>
        public string Identifier
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Specifies whether the anchor was located, or the reason why it may have failed.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.LocateAnchorStatus Status
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.LocateAnchorStatus result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_status(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that located the anchor.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Use this class to represent an anchor in space that can be persisted in a CloudSpatialAnchorSession.
    /// </summary>
    public partial class CloudSpatialAnchor
    {
        internal IntPtr handle;
        internal CloudSpatialAnchor(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_addref(ahandle);
        }
        public CloudSpatialAnchor()
        {
            status resultStatus = (NativeLibrary.ssc_cloud_spatial_anchor_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~CloudSpatialAnchor()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The anchor in the local mixed reality system.
        /// </summary>
        public IntPtr LocalAnchor
        {
            get
            {
                IntPtr result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_local_anchor(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_set_local_anchor(this.handle, value));
            }
        }

        /// <summary>
        /// The time the anchor will expire.
        /// </summary>
        public System.DateTimeOffset Expiration
        {
            get
            {
                long result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_expiration(this.handle, out result));
                return (result == 0) ? DateTimeOffset.MaxValue : DateTimeOffset.FromUnixTimeMilliseconds(result);
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_set_expiration(this.handle, (value == DateTimeOffset.MaxValue) ? 0 : value.ToUnixTimeMilliseconds()));
            }
        }

        /// <summary>
        /// The persistent identifier of this spatial anchor in the cloud service.
        /// </summary>
        public string Identifier
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A dictionary of application-defined properties that is stored with the anchor.
        /// </summary>
        public System.Collections.Generic.IDictionary<string, string> AppProperties
        {
            get
            {
                IntPtr result_handle;
                IDictionary_String_String result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_app_properties(this.handle, out result_handle));
                result_object = new IDictionary_String_String(result_handle, transfer:true);
                return result_object;
            }
        }

        /// <summary>
        /// An opaque version tag that can be used for concurrency control.
        /// </summary>
        public string VersionTag
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_version_tag(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Use this class to defer completing an operation.
    /// </summary>
    /// <remarks>
    /// This is similar to the Windows.Foundation.Deferral class.
    /// </remarks>
    public partial class CloudSpatialAnchorSessionDeferral
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSessionDeferral(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_addref(ahandle);
        }
        ~CloudSpatialAnchorSessionDeferral()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Mark the deferred operation as complete and perform any associated tasks.
        /// </summary>
        public void Complete()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_complete(this.handle));
        }

    }

    /// <summary>
    /// Use this class to configure session diagnostics that can be collected and submitted to improve system quality.
    /// </summary>
    public partial class CloudSpatialAnchorSessionDiagnostics
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSessionDiagnostics(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_addref(ahandle);
        }
        ~CloudSpatialAnchorSessionDiagnostics()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Level of tracing to log.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionLogLevel LogLevel
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionLogLevel result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_log_level(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_log_level(this.handle, value));
            }
        }

        /// <summary>
        /// Directory into which temporary log files and manifests are saved.
        /// </summary>
        public string LogDirectory
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_log_directory(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_log_directory(this.handle, value));
            }
        }

        /// <summary>
        /// Approximate maximum disk space to be used, in megabytes.
        /// </summary>
        /// <remarks>
        /// When this value is set to zero, no information will be written to disk.
        /// </remarks>
        public int MaxDiskSizeInMB
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_max_disk_size_in_mb(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_max_disk_size_in_mb(this.handle, value));
            }
        }

        /// <summary>
        /// Whether images should be logged.
        /// </summary>
        public bool ImagesEnabled
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_images_enabled(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_images_enabled(this.handle, value));
            }
        }

        /// <summary>
        /// Creates a manifest of log files and submission information to be uploaded.
        /// </summary>
        /// <param name="description">
        /// Description to be added to the diagnostics manifest.
        /// </param>
        public async System.Threading.Tasks.Task<string> CreateManifestAsync(string description)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_create_manifest_async(this.handle, description, out result));
                return result;
            });
        }

        /// <summary>
        /// Submits a diagnostics manifest and cleans up its resources.
        /// </summary>
        /// <param name="manifestPath">
        /// Path to the manifest file to submit.
        /// </param>
        public async System.Threading.Tasks.Task SubmitManifestAsync(string manifestPath)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_submit_manifest_async(this.handle, manifestPath));
            });
        }

    }

    /// <summary>
    /// Use this class to create, locate and manage spatial anchors.
    /// </summary>
    public partial class CloudSpatialAnchorSession : IDisposable, ICookie
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSession(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_addref(ahandle);
            CookieTracker<CloudSpatialAnchorSession>.Add(this);
        }
        /// <summary>
        /// Initializes a new instance with a default configuration.
        /// </summary>
        public CloudSpatialAnchorSession()
        {
            status resultStatus = (NativeLibrary.ssc_cloud_spatial_anchor_session_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
            CookieTracker<CloudSpatialAnchorSession>.Add(this);
            // Custom initialization (Android/Unity) begins for CloudSpatialAnchorSession.
            // Custom initialization (Android/Unity) ends for CloudSpatialAnchorSession.
        }

        ~CloudSpatialAnchorSession()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The configuration information for the session.
        /// </summary>
        /// <remarks>
        /// Configuration settings take effect when the session is started.
        /// </remarks>
        public Microsoft.Azure.SpatialAnchors.SessionConfiguration Configuration
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionConfiguration result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_configuration(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.SessionConfiguration(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// The diagnostics settings for the session, which can be used to collect and submit data for troubleshooting and improvements.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics Diagnostics
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_diagnostics(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// Logging level for the session log events.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionLogLevel LogLevel
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionLogLevel result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_log_level(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_log_level(this.handle, value));
            }
        }

        /// <summary>
        /// The tracking session used to help locate anchors.
        /// </summary>
        /// <remarks>
        /// This property is not available on the HoloLens platform.
        /// </remarks>
        public IntPtr Session
        {
            get
            {
                IntPtr result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_session(this.handle, value));
            }
        }

        /// <summary>
        /// The unique identifier for the session.
        /// </summary>
        public string SessionId
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session_id(this.handle, out result));
                return result;
            }
        }

        private ulong cookie;
        ulong ICookie.Cookie { get { return this.cookie; } set { this.cookie = value; } }
        /// <summary>Registered callbacks on this instance.</summary>
        private event TokenRequiredDelegate _TokenRequired;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(TokenRequiredDelegateNative))]
        private static void TokenRequiredStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            TokenRequiredDelegate handler = (instance == null) ? null : instance._TokenRequired;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.TokenRequiredEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static TokenRequiredDelegateNative TokenRequiredStaticHandlerDelegate = TokenRequiredStaticHandler;
        public event TokenRequiredDelegate TokenRequired
        {
            add
            {
                this._TokenRequired += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_token_required(this.handle, this.cookie, TokenRequiredStaticHandlerDelegate));
            }
            remove
            {
                this._TokenRequired -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event AnchorLocatedDelegate _AnchorLocated;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(AnchorLocatedDelegateNative))]
        private static void AnchorLocatedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            AnchorLocatedDelegate handler = (instance == null) ? null : instance._AnchorLocated;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.AnchorLocatedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static AnchorLocatedDelegateNative AnchorLocatedStaticHandlerDelegate = AnchorLocatedStaticHandler;
        public event AnchorLocatedDelegate AnchorLocated
        {
            add
            {
                this._AnchorLocated += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_anchor_located(this.handle, this.cookie, AnchorLocatedStaticHandlerDelegate));
            }
            remove
            {
                this._AnchorLocated -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event LocateAnchorsCompletedDelegate _LocateAnchorsCompleted;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(LocateAnchorsCompletedDelegateNative))]
        private static void LocateAnchorsCompletedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            LocateAnchorsCompletedDelegate handler = (instance == null) ? null : instance._LocateAnchorsCompleted;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.LocateAnchorsCompletedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static LocateAnchorsCompletedDelegateNative LocateAnchorsCompletedStaticHandlerDelegate = LocateAnchorsCompletedStaticHandler;
        public event LocateAnchorsCompletedDelegate LocateAnchorsCompleted
        {
            add
            {
                this._LocateAnchorsCompleted += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_locate_anchors_completed(this.handle, this.cookie, LocateAnchorsCompletedStaticHandlerDelegate));
            }
            remove
            {
                this._LocateAnchorsCompleted -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event SessionUpdatedDelegate _SessionUpdated;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(SessionUpdatedDelegateNative))]
        private static void SessionUpdatedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            SessionUpdatedDelegate handler = (instance == null) ? null : instance._SessionUpdated;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.SessionUpdatedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static SessionUpdatedDelegateNative SessionUpdatedStaticHandlerDelegate = SessionUpdatedStaticHandler;
        public event SessionUpdatedDelegate SessionUpdated
        {
            add
            {
                this._SessionUpdated += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_session_updated(this.handle, this.cookie, SessionUpdatedStaticHandlerDelegate));
            }
            remove
            {
                this._SessionUpdated -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event SessionErrorDelegate _Error;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(SessionErrorDelegateNative))]
        private static void ErrorStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            SessionErrorDelegate handler = (instance == null) ? null : instance._Error;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.SessionErrorEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static SessionErrorDelegateNative ErrorStaticHandlerDelegate = ErrorStaticHandler;
        public event SessionErrorDelegate Error
        {
            add
            {
                this._Error += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_error(this.handle, this.cookie, ErrorStaticHandlerDelegate));
            }
            remove
            {
                this._Error -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event OnLogDebugDelegate _OnLogDebug;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(OnLogDebugDelegateNative))]
        private static void OnLogDebugStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            OnLogDebugDelegate handler = (instance == null) ? null : instance._OnLogDebug;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.OnLogDebugEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static OnLogDebugDelegateNative OnLogDebugStaticHandlerDelegate = OnLogDebugStaticHandler;
        public event OnLogDebugDelegate OnLogDebug
        {
            add
            {
                this._OnLogDebug += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_on_log_debug(this.handle, this.cookie, OnLogDebugStaticHandlerDelegate));
            }
            remove
            {
                this._OnLogDebug -= value;
            }
        }

        /// <summary>
        /// Stops this session and releases all associated resources.
        /// </summary>
        public void Dispose()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_dispose(this.handle));
        }

        /// <summary>
        /// Gets the Azure Spatial Anchors access token from authentication token.
        /// </summary>
        /// <param name="authenticationToken">
        /// Authentication token.
        /// </param>
        public async System.Threading.Tasks.Task<string> GetAccessTokenWithAuthenticationTokenAsync(string authenticationToken)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_access_token_with_authentication_token_async(this.handle, authenticationToken, out result));
                return result;
            });
        }

        /// <summary>
        /// Gets the Azure Spatial Anchors access token from account key.
        /// </summary>
        /// <param name="accountKey">
        /// Account key.
        /// </param>
        public async System.Threading.Tasks.Task<string> GetAccessTokenWithAccountKeyAsync(string accountKey)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_access_token_with_account_key_async(this.handle, accountKey, out result));
                return result;
            });
        }

        /// <summary>
        /// Creates a new persisted spatial anchor from the specified local anchor and string properties.
        /// </summary>
        /// <param name="anchor">
        /// Anchor to be persisted.
        /// </param>
        public async System.Threading.Tasks.Task CreateAnchorAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_create_anchor_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Creates a new object that watches for anchors that meet the specified criteria.
        /// </summary>
        /// <param name="criteria">
        /// Criteria for anchors to watch for.
        /// </param>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher CreateWatcher(Microsoft.Azure.SpatialAnchors.AnchorLocateCriteria criteria)
        {
            IntPtr result_handle;
            Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_create_watcher(this.handle, criteria.handle, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
            return result_object;
        }

        /// <summary>
        /// Gets a cloud spatial anchor for the given identifier, even if it hasn't been located yet.
        /// </summary>
        /// <param name="identifier">
        /// The identifier to look for.
        /// </param>
        public async System.Threading.Tasks.Task<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor> GetAnchorPropertiesAsync(string identifier)
        {
            return await Task.Run(() =>
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_anchor_properties_async(this.handle, identifier, out result_handle));
                result_object = new CloudSpatialAnchor(result_handle, transfer:true);
                return result_object;
            });
        }

        /// <summary>
        /// Gets a list of active watchers.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher> GetActiveWatchers()
        {
            System.Collections.Generic.IReadOnlyList<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher> result;
            IntPtr[] result_array;
            int result_count;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_active_watchers_count(this.handle, out result_count));
            result_array = new IntPtr[result_count];
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_active_watchers_items(this.handle, result_array, ref result_count));
            result = result_array.Take(result_count).Select(handle => new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(handle, transfer:true)).ToList().AsReadOnly();
            return result;
        }

        /// <summary>
        /// Refreshes properties for the specified spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to refresh.
        /// </param>
        public async System.Threading.Tasks.Task RefreshAnchorPropertiesAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_refresh_anchor_properties_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Updates the specified spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to be updated.
        /// </param>
        public async System.Threading.Tasks.Task UpdateAnchorPropertiesAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_update_anchor_properties_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Deletes a persisted spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to be deleted.
        /// </param>
        public async System.Threading.Tasks.Task DeleteAnchorAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_delete_anchor_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Applications must call this method on platforms where per-frame processing is required.
        /// </summary>
        /// <param name="frame">
        /// AR frame to process.
        /// </param>
        /// <remarks>
        /// This method is not available on the HoloLens platform.
        /// </remarks>
        public void ProcessFrame(IntPtr frame)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_process_frame(this.handle, frame));
        }

        /// <summary>
        /// Gets an object describing the status of the session.
        /// </summary>
        public async System.Threading.Tasks.Task<Microsoft.Azure.SpatialAnchors.SessionStatus> GetSessionStatusAsync()
        {
            return await Task.Run(() =>
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionStatus result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session_status_async(this.handle, out result_handle));
                result_object = new SessionStatus(result_handle, transfer:true);
                return result_object;
            });
        }

        /// <summary>
        /// Begins capturing environment data for the session.
        /// </summary>
        public void Start()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_start(this.handle));
        }

        /// <summary>
        /// Stops capturing environment data for the session and cancels any outstanding locate operations. Environment data is maintained.
        /// </summary>
        public void Stop()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_stop(this.handle));
        }

        /// <summary>
        /// Resets environment data that has been captured in this session; applications must call this method when tracking is lost.
        /// </summary>
        /// <remarks>
        /// On any platform, calling the method will clean all internal cached state.
        /// </remarks>
        public void Reset()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_reset(this.handle));
        }

    }

    /// <summary>
    /// Use this class to control an object that watches for spatial anchors.
    /// </summary>
    public partial class CloudSpatialAnchorWatcher
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorWatcher(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_watcher_addref(ahandle);
        }
        ~CloudSpatialAnchorWatcher()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Distinct identifier for the watcher within its session.
        /// </summary>
        public int Identifier
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Stops further activity from this watcher.
        /// </summary>
        public void Stop()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_stop(this.handle));
        }

    }

    /// <summary>
    /// Use this type to determine when a locate operation has completed.
    /// </summary>
    public partial class LocateAnchorsCompletedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal LocateAnchorsCompletedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_locate_anchors_completed_event_args_addref(ahandle);
        }
        ~LocateAnchorsCompletedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Gets a value indicating whether the locate operation was canceled.
        /// </summary>
        /// <remarks>
        /// When this property is true, the watcher was stopped before completing.
        /// </remarks>
        public bool Cancelled
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_get_cancelled(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that completed the locate operation.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Use this class to describe how anchors to be located should be near a source anchor.
    /// </summary>
    public partial class NearAnchorCriteria
    {
        internal IntPtr handle;
        internal NearAnchorCriteria(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_near_anchor_criteria_addref(ahandle);
        }
        public NearAnchorCriteria()
        {
            status resultStatus = (NativeLibrary.ssc_near_anchor_criteria_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~NearAnchorCriteria()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Source anchor around which nearby anchors should be located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor SourceAnchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_source_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor(result_handle, transfer:true) : null;
                return result_object;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_source_anchor(this.handle, value.handle));
            }
        }

        /// <summary>
        /// Maximum distance in meters from the source anchor (defaults to 5).
        /// </summary>
        public float DistanceInMeters
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_distance_in_meters(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_distance_in_meters(this.handle, value));
            }
        }

        /// <summary>
        /// Maximum desired result count (defaults to 20).
        /// </summary>
        public int MaxResultCount
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_max_result_count(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_max_result_count(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires for logging messages.
    /// </summary>
    public partial class OnLogDebugEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal OnLogDebugEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_on_log_debug_event_args_addref(ahandle);
        }
        ~OnLogDebugEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_on_log_debug_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The logging message.
        /// </summary>
        public string Message
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_on_log_debug_event_args_get_message(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Use this class to set up the service configuration for a SpatialAnchorSession.
    /// </summary>
    public partial class SessionConfiguration
    {
        internal IntPtr handle;
        internal SessionConfiguration(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_configuration_addref(ahandle);
        }
        ~SessionConfiguration()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Account domain for the Azure Spatial Anchors service.
        /// </summary>
        /// <remarks>
        /// The default is "mixedreality.azure.com".
        /// </remarks>
        public string AccountDomain
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_domain(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_domain(this.handle, value));
            }
        }

        /// <summary>
        /// Account-level ID for the Azure Spatial Anchors service.
        /// </summary>
        public string AccountId
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_id(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_id(this.handle, value));
            }
        }

        /// <summary>
        /// Authentication token for Azure Active Directory (AAD).
        /// </summary>
        /// <remarks>
        /// If the access token and the account key are missing, the session will obtain an access token based on this value.
        /// </remarks>
        public string AuthenticationToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_authentication_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_authentication_token(this.handle, value));
            }
        }

        /// <summary>
        /// Account-level key for the Azure Spatial Anchors service.
        /// </summary>
        public string AccountKey
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_key(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_key(this.handle, value));
            }
        }

        /// <summary>
        /// Access token for the Azure Spatial Anchors service.
        /// </summary>
        public string AccessToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_access_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_access_token(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires when errors are thrown.
    /// </summary>
    public partial class SessionErrorEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal SessionErrorEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_error_event_args_addref(ahandle);
        }
        ~SessionErrorEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_get_error_message(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that found an error, possibly null.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// This type describes the status of spatial data processing.
    /// </summary>
    public partial class SessionStatus
    {
        internal IntPtr handle;
        internal SessionStatus(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_status_addref(ahandle);
        }
        ~SessionStatus()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The level of data sufficiency for a successful operation.
        /// </summary>
        /// <remarks>
        /// This value will be in the [0;1) range when data is insufficient; 1 when data is sufficient for success and greater than 1 when conditions are better than minimally sufficient.
        /// </remarks>
        public float ReadyForCreateProgress
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_ready_for_create_progress(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The ratio of data available to recommended data to create an anchor.
        /// </summary>
        /// <remarks>
        /// This value will be in the [0;1) range when data is below the recommended threshold; 1 and greater when the recommended amount of data has been gathered for a creation operation.
        /// </remarks>
        public float RecommendedForCreateProgress
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_recommended_for_create_progress(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A hash value that can be used to know when environment data that contributes to a creation operation has changed to included the latest input data.
        /// </summary>
        /// <remarks>
        /// If the hash value does not change after new frames were added to the session, then those frames were not deemed as sufficientlyy different from existing environment data and disgarded. This value may be 0 (and should be ignored) for platforms that don't feed frames individually.
        /// </remarks>
        public int SessionCreateHash
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_session_create_hash(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A hash value that can be used to know when environment data that contributes to a locate operation has changed to included the latest input data.
        /// </summary>
        /// <remarks>
        /// If the hash value does not change after new frames were added to the session, then those frames were not deemed as sufficiency different from existing environment data and disgarded. This value may be 0 (and should be ignored) for platforms that don't feed frames individually.
        /// </remarks>
        public int SessionLocateHash
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_session_locate_hash(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Feedback that can be provided to user about data processing status.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionUserFeedback UserFeedback
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionUserFeedback result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_user_feedback(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires when the session state is updated.
    /// </summary>
    public partial class SessionUpdatedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal SessionUpdatedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_updated_event_args_addref(ahandle);
        }
        ~SessionUpdatedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_updated_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Current session status.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionStatus Status
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionStatus result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_updated_event_args_get_status(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.SessionStatus(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Informs the application that the service requires an updated access token or authentication token.
    /// </summary>
    public partial class TokenRequiredEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal TokenRequiredEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_token_required_event_args_addref(ahandle);
        }
        ~TokenRequiredEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The access token to be used by the operation that requires it.
        /// </summary>
        public string AccessToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_access_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_set_access_token(this.handle, value));
            }
        }

        /// <summary>
        /// The authentication token to be used by the operation that requires it.
        /// </summary>
        public string AuthenticationToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_authentication_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_set_authentication_token(this.handle, value));
            }
        }

        /// <summary>
        /// Returns a deferral object that can be used to provide an updated access token or authentication token from another asynchronous operation.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral GetDeferral()
        {
            IntPtr result_handle;
            Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral result_object;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_deferral(this.handle, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral(result_handle, transfer:true) : null;
            return result_object;
        }

    }

}

#elif UNITY_EDITOR
// Making calls to Azure Spatial Anchors from the Unity editor is not currently supported.
// These stubs are here to prevent the editor from reporting compilation errors.
//
// Spatial Services Client
// This file was auto-generated with sscapigen based on SscApiModelDirect.cs, hash oCDnGdnaFmKPFORSJrQ6mQ==
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.SpatialAnchors
{
    internal enum status
    {
        /// <summary>
        /// Success
        /// </summary>
        OK = 0,
        /// <summary>
        /// Failed
        /// </summary>
        Failed = 1,
        /// <summary>
        /// Cannot access a disposed object.
        /// </summary>
        ObjectDisposed = 2,
        /// <summary>
        /// Out of memory.
        /// </summary>
        OutOfMemory = 12,
        /// <summary>
        /// Invalid argument.
        /// </summary>
        InvalidArgument = 22,
        /// <summary>
        /// The value is out of range.
        /// </summary>
        OutOfRange = 34,
        /// <summary>
        /// Not implemented.
        /// </summary>
        NotImplemented = 38,
        /// <summary>
        /// The key does not exist in the collection.
        /// </summary>
        KeyNotFound = 77,
        /// <summary>
        /// Amount of Metadata exceeded the allowed limit (currently 4k)
        /// </summary>
        MetadataTooLarge = 78,
        /// <summary>
        /// Application did not provide valid credentials and therefore could not authenticate with the Cloud Service
        /// </summary>
        ApplicationNotAuthenticated = 79,
        /// <summary>
        /// Application did not provide any credentials for authorization with the Cloud Service
        /// </summary>
        ApplicationNotAuthorized = 80,
        /// <summary>
        /// Multiple stores (on the same device or different devices) made concurrent changes to the same Spatial Entity and so this particular change was rejected
        /// </summary>
        ConcurrencyViolation = 81,
        /// <summary>
        /// Not enough Neighborhood Spatial Data was available to complete the desired Create operation
        /// </summary>
        NotEnoughSpatialData = 82,
        /// <summary>
        /// No Spatial Location Hint was available (or it was not specific enough) to support rediscovery from the Cloud at a later time
        /// </summary>
        NoSpatialLocationHint = 83,
        /// <summary>
        /// Application cannot connect to the Cloud Service
        /// </summary>
        CannotConnectToServer = 84,
        /// <summary>
        /// Cloud Service returned an unspecified error
        /// </summary>
        ServerError = 85,
        /// <summary>
        /// The Spatial Entity has already been associated with a different Store object, so cannot be used with this current Store object.
        /// </summary>
        AlreadyAssociatedWithADifferentStore = 86,
        /// <summary>
        /// SpatialEntity already exists in a Store but TryCreateAsync was called.
        /// </summary>
        AlreadyExists = 87,
        /// <summary>
        /// A locate operation was requested, but the criteria does not specify anything to look for.
        /// </summary>
        NoLocateCriteriaSpecified = 88,
        /// <summary>
        /// An access token was required but not specified; handle the TokenRequired event on the session to provide one.
        /// </summary>
        NoAccessTokenSpecified = 89,
        /// <summary>
        /// The session was unable to obtain an access token and so the creation could not proceed
        /// </summary>
        UnableToObtainAccessToken = 90,
    }

    internal static class NativeLibraryHelpers
    {
        internal static string[] IntPtrToStringArray(IntPtr result, int result_length)
        {
            byte[] bytes = new byte[result_length];
            System.Runtime.InteropServices.Marshal.Copy(result, bytes, 0, result_length - 1);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(result);
            return System.Text.Encoding.UTF8.GetString(bytes).Split('\0');
        }

        internal static void CheckStatus(IntPtr handle, status value)
        {
            if (value == status.OK)
            {
                return;
            }

            string message;
            string requestCorrelationVector;
            string responseCorrelationVector;

            status code = NativeLibrary.ssc_get_error_details(handle, out message, out requestCorrelationVector, out responseCorrelationVector);

            string fullMessage;
            if (code == status.Failed)
            {
                throw new InvalidOperationException("Unexpected error in exception handling.");
            }
            else if (code != status.OK)
            {
                fullMessage = "Exception thrown and an unexpected error in exception handling.";
            }
            else
            {
                fullMessage = message + ". Request CV: " + requestCorrelationVector + ". Response CV: " + responseCorrelationVector + ".";
            }

            switch (value)
            {
                case status.OK:
                    return;
                case status.Failed:
                    throw new InvalidOperationException(fullMessage);
                case status.ObjectDisposed:
                    throw new ObjectDisposedException(fullMessage);
                case status.OutOfMemory:
                    throw new OutOfMemoryException(fullMessage);
                case status.InvalidArgument:
                    throw new ArgumentException(fullMessage);
                case status.OutOfRange:
                    throw new ArgumentOutOfRangeException("", fullMessage);
                case status.NotImplemented:
                    throw new NotImplementedException(fullMessage);
                case status.KeyNotFound:
                    throw new KeyNotFoundException(fullMessage);
                case status.MetadataTooLarge:
                    throw new CloudSpatialException(CloudSpatialErrorCode.MetadataTooLarge, message, requestCorrelationVector, responseCorrelationVector);
                case status.ApplicationNotAuthenticated:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ApplicationNotAuthenticated, message, requestCorrelationVector, responseCorrelationVector);
                case status.ApplicationNotAuthorized:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ApplicationNotAuthorized, message, requestCorrelationVector, responseCorrelationVector);
                case status.ConcurrencyViolation:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ConcurrencyViolation, message, requestCorrelationVector, responseCorrelationVector);
                case status.NotEnoughSpatialData:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NotEnoughSpatialData, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoSpatialLocationHint:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoSpatialLocationHint, message, requestCorrelationVector, responseCorrelationVector);
                case status.CannotConnectToServer:
                    throw new CloudSpatialException(CloudSpatialErrorCode.CannotConnectToServer, message, requestCorrelationVector, responseCorrelationVector);
                case status.ServerError:
                    throw new CloudSpatialException(CloudSpatialErrorCode.ServerError, message, requestCorrelationVector, responseCorrelationVector);
                case status.AlreadyAssociatedWithADifferentStore:
                    throw new CloudSpatialException(CloudSpatialErrorCode.AlreadyAssociatedWithADifferentStore, message, requestCorrelationVector, responseCorrelationVector);
                case status.AlreadyExists:
                    throw new CloudSpatialException(CloudSpatialErrorCode.AlreadyExists, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoLocateCriteriaSpecified:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoLocateCriteriaSpecified, message, requestCorrelationVector, responseCorrelationVector);
                case status.NoAccessTokenSpecified:
                    throw new CloudSpatialException(CloudSpatialErrorCode.NoAccessTokenSpecified, message, requestCorrelationVector, responseCorrelationVector);
                case status.UnableToObtainAccessToken:
                    throw new CloudSpatialException(CloudSpatialErrorCode.UnableToObtainAccessToken, message, requestCorrelationVector, responseCorrelationVector);
            }
        }
    }

    /// <summary>This interface is implemented by classes with events to help track callbacks.</summary>
    internal interface ICookie
    {
        /// <summary>Unique cookie value for callback identification.</summary>
        ulong Cookie { get; set; }
    }

    internal static class CookieTracker<T> where T : class, ICookie
    {
        private static ulong lastCookie;
        private static Dictionary<ulong, System.WeakReference<T>> tracked = new Dictionary<ulong, System.WeakReference<T>>();
        internal static void Add(T instance)
        {
            lock (tracked)
            {
                instance.Cookie = ++lastCookie;
                tracked[instance.Cookie] = new System.WeakReference<T>(instance);
            }
        }
        internal static T Lookup(ulong cookie)
        {
            T result;
            System.WeakReference<T> reference;
            bool found;
            lock (tracked)
            {
                found = tracked.TryGetValue(cookie, out reference);
            }
            if (!found)
            {
                return null;
            }
            found = reference.TryGetTarget(out result);
            if (!found)
            {
                lock (tracked)
                {
                    tracked.Remove(cookie);
                }
            }
            return result;
        }
        internal static void Remove(T instance)
        {
            lock (tracked)
            {
                tracked.Remove(instance.Cookie);
            }
        }
    }

    /// <summary>
    /// Informs the application that a locate operation has completed.
    /// </summary>
    /// <param name="sender">
    /// The session that ran the locate operation.
    /// </param>
    /// <param name="args">
    /// The arguments describing the operation completion.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void LocateAnchorsCompletedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session requires an updated access token or authentication token.
    /// </summary>
    /// <param name="sender">
    /// The session that requires an updated access token or authentication token.
    /// </param>
    /// <param name="args">
    /// The event arguments that require an AccessToken property or an AuthenticationToken property to be set.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void TokenRequiredDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session has located an anchor or discovered that it cannot yet be located.
    /// </summary>
    /// <param name="sender">
    /// The session that fires the event.
    /// </param>
    /// <param name="args">
    /// Information about the located anchor.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void AnchorLocatedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that a session has been updated with new information.
    /// </summary>
    /// <param name="sender">
    /// The session that has been updated.
    /// </param>
    /// <param name="args">
    /// Information about the current session status.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SessionUpdatedDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application that an error occurred in a session.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the error.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void SessionErrorDelegateNative(ulong cookie, IntPtr args);
    /// <summary>
    /// Informs the application of a debug log message.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the log.
    /// </param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void OnLogDebugDelegateNative(ulong cookie, IntPtr args);

    internal static class NativeLibrary
    {
        internal const string DllName = "__Internal";
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_get_cancelled(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_locate_anchors_completed_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_get_identifier(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_watcher_stop(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_local_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_set_local_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_expiration(IntPtr handle, out long result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_set_expiration(IntPtr handle, long value);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_get_identifier_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_identifier(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_app_properties(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_count(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_clear(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_idictionary_string_string_get_key_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_key(IntPtr handle, Int32 index, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_idictionary_string_string_get_item_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_get_item(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string key, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_idictionary_string_string_set_item_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_set_item(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string key, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_idictionary_string_string_remove_key_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_idictionary_string_string_remove_key(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string key);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_get_version_tag_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_get_version_tag(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_anchor_located_event_args_get_identifier_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_identifier(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_status(IntPtr handle, out LocateAnchorStatus result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_located_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_get_account_domain_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_domain(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_set_account_domain_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_domain(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_get_account_id_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_id(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_set_account_id_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_id(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_get_authentication_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_set_authentication_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_get_account_key_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_account_key(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_set_account_key_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_account_key(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_get_access_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_get_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_session_configuration_set_access_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_configuration_set_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_configuration(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_diagnostics(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_log_level(IntPtr handle, out SessionLogLevel result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_log_level(IntPtr handle, SessionLogLevel value);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_diagnostics_get_log_directory_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_log_directory(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_diagnostics_set_log_directory_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_log_directory(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_max_disk_size_in_mb(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_max_disk_size_in_mb(IntPtr handle, Int32 value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_get_images_enabled(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_set_images_enabled(IntPtr handle, Boolean value);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_diagnostics_create_manifest_async_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_create_manifest_async(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string description, [MarshalAs(UnmanagedType.LPWStr)] out String result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_diagnostics_submit_manifest_async_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_diagnostics_submit_manifest_async(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string manifest_path);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_log_level(IntPtr handle, out SessionLogLevel result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_log_level(IntPtr handle, SessionLogLevel value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_session(IntPtr handle, IntPtr value);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_session_id_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session_id(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_middleware_versions_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_middleware_versions(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_set_middleware_versions_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_middleware_versions(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_sdk_package_type_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_sdk_package_type(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_set_sdk_package_type_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_sdk_package_type(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_token_required(IntPtr handle, ulong value, TokenRequiredDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_token_required_event_args_get_access_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_token_required_event_args_set_access_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_set_access_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, EntryPoint="ssc_token_required_event_args_get_authentication_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, EntryPoint="ssc_token_required_event_args_set_authentication_token_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_set_authentication_token(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_token_required_event_args_get_deferral(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_deferral_complete(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_anchor_located(IntPtr handle, ulong value, AnchorLocatedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_locate_anchors_completed(IntPtr handle, ulong value, LocateAnchorsCompletedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_session_updated(IntPtr handle, ulong value, SessionUpdatedDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_updated_event_args_get_status(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_ready_for_create_progress(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_recommended_for_create_progress(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_session_create_hash(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_session_locate_hash(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_status_get_user_feedback(IntPtr handle, out SessionUserFeedback result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_error(IntPtr handle, ulong value, SessionErrorDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_session_error_event_args_get_error_message_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_get_error_message(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_session_error_event_args_get_watcher(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_set_on_log_debug(IntPtr handle, ulong value, OnLogDebugDelegateNative value_fn);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_on_log_debug_event_args_get_message_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_on_log_debug_event_args_get_message(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_dispose(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_access_token_with_authentication_token_async_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_access_token_with_authentication_token_async(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string authentication_token, [MarshalAs(UnmanagedType.LPWStr)] out String result);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_access_token_with_account_key_async_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_access_token_with_account_key_async(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string account_key, [MarshalAs(UnmanagedType.LPWStr)] out String result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create_anchor_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_create_watcher(IntPtr handle, IntPtr criteria, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_addref(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_anchor_locate_criteria_get_identifiers_wide_flat", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_identifiers(IntPtr handle, out IntPtr result, out int result_count);
        [DllImport(DllName, EntryPoint="ssc_anchor_locate_criteria_set_identifiers_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_identifiers(IntPtr handle, [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPWStr)] String[] value, int value_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_bypass_cache(IntPtr handle, out Boolean result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_bypass_cache(IntPtr handle, Boolean value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_near_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_release(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_addref(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_source_anchor(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_source_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_distance_in_meters(IntPtr handle, out Single result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_distance_in_meters(IntPtr handle, Single value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_get_max_result_count(IntPtr handle, out Int32 result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_set_max_result_count(IntPtr handle, Int32 value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_near_anchor_criteria_create(out IntPtr instance);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_near_anchor(IntPtr handle, IntPtr value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_requested_categories(IntPtr handle, out AnchorDataCategory result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_requested_categories(IntPtr handle, AnchorDataCategory value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_get_strategy(IntPtr handle, out LocateStrategy result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_set_strategy(IntPtr handle, LocateStrategy value);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_anchor_locate_criteria_create(out IntPtr instance);
        [DllImport(DllName, EntryPoint="ssc_cloud_spatial_anchor_session_get_anchor_properties_async_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_anchor_properties_async(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string identifier, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_active_watchers_count(IntPtr handle, out Int32 result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_active_watchers_items(IntPtr handle, [MarshalAs(UnmanagedType.LPArray), In, Out] IntPtr[] result_array, ref Int32 result_count);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_refresh_anchor_properties_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_update_anchor_properties_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_delete_anchor_async(IntPtr handle, IntPtr anchor);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_process_frame(IntPtr handle, IntPtr frame);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_get_session_status_async(IntPtr handle, out IntPtr result);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_start(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_stop(IntPtr handle);
        [DllImport(DllName, CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_cloud_spatial_anchor_session_reset(IntPtr handle);
        [DllImport(DllName, EntryPoint="ssc_get_error_details_wide", CallingConvention=CallingConvention.Cdecl)]
        internal static extern status ssc_get_error_details(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] out string result_message, [MarshalAs(UnmanagedType.LPWStr)] out string result_requestCorrelationVector, [MarshalAs(UnmanagedType.LPWStr)] out string result_responseCorrelationVector);
    }

    // CODE STARTS HERE

    abstract class BasePrivateDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected abstract int InternalGetCount();
        protected abstract TKey InternalGetKey(int index);
        protected abstract TValue InternalGetItem(TKey key);
        protected abstract void InternalSetItem(TKey key, TValue value);
        protected abstract void InternalRemoveKey(TKey key);

        public TValue this[TKey key] { get { return InternalGetItem(key); } set { InternalSetItem(key, value); } }

        public ICollection<TKey> Keys { get { return Enumerable.Range(0, InternalGetCount()).Select(n => InternalGetKey(n)).ToList().AsReadOnly(); } }

        public ICollection<TValue> Values { get { return Enumerable.Range(0, InternalGetCount()).Select(n => InternalGetKey(n)).Select(k => InternalGetItem(k)).ToList().AsReadOnly(); } }

        public int Count { get { return InternalGetCount(); } }

        public bool IsReadOnly { get { return false; } }

        public void Add(TKey key, TValue value)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                InternalSetItem(key, value);
                return;
            }
            throw new ArgumentException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            while (InternalGetCount() > 0)
            {
                TKey key = InternalGetKey(0);
                InternalRemoveKey(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            try
            {
                TValue value = InternalGetItem(item.Key);
                if (Comparer<TValue>.Default.Compare(value, item.Value) == 0)
                {
                    return true;
                }
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                yield return new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }

        public bool Remove(TKey key)
        {
            try
            {
                InternalGetItem(key);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            InternalRemoveKey(key);
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = InternalGetItem(key);
                return true;
            }
            catch (KeyNotFoundException)
            {
                value = default(TValue);
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < InternalGetCount(); ++i)
            {
                TKey key = InternalGetKey(i);
                yield return new KeyValuePair<TKey, TValue>(key, InternalGetItem(key));
            }
        }
    }

    abstract class BasePrivateList<T> : IList<T>
    {
        protected abstract int InternalGetCount();
        protected abstract T InternalGetItem(int index);
        protected abstract void InternalSetItem(int index, T value);
        protected abstract void InternalRemoveItem(int index);

        public int Count { get { return InternalGetCount(); } }

        public bool IsReadOnly { get { return false; } }

        public T this[int index] { get { return InternalGetItem(index); } set { InternalSetItem(index, value); } }

        public int IndexOf(T item)
        {
            int count = InternalGetCount();
            for (int i = 0; i < count; i++)
            {
                if (Comparer<T>.Default.Compare(item, InternalGetItem(i)) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            InternalSetItem(index, item);
        }

        public void RemoveAt(int index)
        {
            InternalRemoveItem(index);
        }

        public void Add(T item)
        {
            InternalSetItem(InternalGetCount(), item);
        }

        public void Clear()
        {
            while (InternalGetCount() > 0)
            {
                InternalRemoveItem(0);
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; ++i)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0) return false;
            InternalRemoveItem(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }
    }

    class IDictionary_String_String : BasePrivateDictionary<String, String>
    {
        internal IntPtr handle;
        internal IDictionary_String_String(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_idictionary_string_string_addref(ahandle);
        }
        ~IDictionary_String_String()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_release(this.handle));
            this.handle = IntPtr.Zero;
        }
        protected override int InternalGetCount()
        {
            int result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_count(this.handle, out result));
            return result;
        }
        protected override String InternalGetKey(int index)
        {
            string result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_key(this.handle, index, out result));
            return result;
        }
        protected override String InternalGetItem(String key)
        {
            string result;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_get_item(this.handle, key, out result));
            return result;
        }
        protected override void InternalSetItem(String key, String value)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_set_item(this.handle, key, value));
        }
        protected override void InternalRemoveKey(String key)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_idictionary_string_string_remove_key(this.handle, key));
        }
    }
    public enum SessionLogLevel : int
    {
        /// <summary>
        /// Specifies that logging should not write any messages.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies logs that indicate when the current flow of execution stops due to a failure.
        /// </summary>
        Error = 1,
        /// <summary>
        /// Specifies logs that highlight an abnormal or unexpected event, but do not otherwise cause execution to stop.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Specifies logs that track the general flow.
        /// </summary>
        Information = 3,
        /// <summary>
        /// Specifies logs used for interactive investigation during development.
        /// </summary>
        Debug = 4,
        /// <summary>
        /// Specifies all messages should be logged.
        /// </summary>
        All = 5,
    }

    public enum LocateAnchorStatus : int
    {
        /// <summary>
        /// The anchor was already being tracked.
        /// </summary>
        AlreadyTracked = 0,
        /// <summary>
        /// The anchor was found.
        /// </summary>
        Located = 1,
        /// <summary>
        /// The anchor was not found.
        /// </summary>
        NotLocated = 2,
        /// <summary>
        /// The anchor cannot be found - it was deleted or the identifier queried for was incorrect.
        /// </summary>
        NotLocatedAnchorDoesNotExist = 3,
    }

    public enum SessionUserFeedback : int
    {
        /// <summary>
        /// No specific feedback is available.
        /// </summary>
        None = 0,
        /// <summary>
        /// Device is not moving enough to create a neighborhood of key-frames.
        /// </summary>
        NotEnoughMotion = 1,
        /// <summary>
        /// Device is moving too quickly for stable tracking.
        /// </summary>
        MotionTooQuick = 2,
        /// <summary>
        /// The environment doesn't have enough feature points for stable tracking.
        /// </summary>
        NotEnoughFeatures = 4,
    }

    public enum AnchorDataCategory : int
    {
        /// <summary>
        /// No data is returned.
        /// </summary>
        None = 0,
        /// <summary>
        /// Returns Anchor properties including AppProperties.
        /// </summary>
        Properties = 1,
        /// <summary>
        /// Returns spatial information about an Anchor.
        /// </summary>
        /// <remarks>
        /// Returns a LocalAnchor for any returned CloudSpatialAnchors from the service.
        /// </remarks>
        Spatial = 2,
    }

    public enum LocateStrategy : int
    {
        /// <summary>
        /// Indicates that any method is acceptable.
        /// </summary>
        AnyStrategy = 0,
        /// <summary>
        /// Indicates that anchors will be located primarily by visual information.
        /// </summary>
        VisualInformation = 1,
        /// <summary>
        /// Indicates that anchors will be located primarily by relationship to other anchors.
        /// </summary>
        Relationship = 2,
    }

    public enum CloudSpatialErrorCode : int
    {
        /// <summary>
        /// Amount of Metadata exceeded the allowed limit (currently 4k)
        /// </summary>
        MetadataTooLarge = 0,
        /// <summary>
        /// Application did not provide valid credentials and therefore could not authenticate with the Cloud Service
        /// </summary>
        ApplicationNotAuthenticated = 1,
        /// <summary>
        /// Application did not provide any credentials for authorization with the Cloud Service
        /// </summary>
        ApplicationNotAuthorized = 2,
        /// <summary>
        /// Multiple stores (on the same device or different devices) made concurrent changes to the same Spatial Entity and so this particular change was rejected
        /// </summary>
        ConcurrencyViolation = 3,
        /// <summary>
        /// Not enough Neighborhood Spatial Data was available to complete the desired Create operation
        /// </summary>
        NotEnoughSpatialData = 4,
        /// <summary>
        /// No Spatial Location Hint was available (or it was not specific enough) to support rediscovery from the Cloud at a later time
        /// </summary>
        NoSpatialLocationHint = 5,
        /// <summary>
        /// Application cannot connect to the Cloud Service
        /// </summary>
        CannotConnectToServer = 6,
        /// <summary>
        /// Cloud Service returned an unspecified error
        /// </summary>
        ServerError = 7,
        /// <summary>
        /// The Spatial Entity has already been associated with a different Store object, so cannot be used with this current Store object.
        /// </summary>
        AlreadyAssociatedWithADifferentStore = 8,
        /// <summary>
        /// SpatialEntity already exists in a Store but TryCreateAsync was called.
        /// </summary>
        AlreadyExists = 9,
        /// <summary>
        /// A locate operation was requested, but the criteria does not specify anything to look for.
        /// </summary>
        NoLocateCriteriaSpecified = 10,
        /// <summary>
        /// An access token was required but not specified; handle the TokenRequired event on the session to provide one.
        /// </summary>
        NoAccessTokenSpecified = 11,
        /// <summary>
        /// The session was unable to obtain an access token and so the creation could not proceed
        /// </summary>
        UnableToObtainAccessToken = 12,
    }

    /// <summary>
    /// Informs the application that a locate operation has completed.
    /// </summary>
    /// <param name="sender">
    /// The session that ran the locate operation.
    /// </param>
    /// <param name="args">
    /// The arguments describing the operation completion.
    /// </param>
    public delegate void LocateAnchorsCompletedDelegate(object sender, Microsoft.Azure.SpatialAnchors.LocateAnchorsCompletedEventArgs args);

    /// <summary>
    /// Informs the application that a session requires an updated access token or authentication token.
    /// </summary>
    /// <param name="sender">
    /// The session that requires an updated access token or authentication token.
    /// </param>
    /// <param name="args">
    /// The event arguments that require an AccessToken property or an AuthenticationToken property to be set.
    /// </param>
    public delegate void TokenRequiredDelegate(object sender, Microsoft.Azure.SpatialAnchors.TokenRequiredEventArgs args);

    /// <summary>
    /// Informs the application that a session has located an anchor or discovered that it cannot yet be located.
    /// </summary>
    /// <param name="sender">
    /// The session that fires the event.
    /// </param>
    /// <param name="args">
    /// Information about the located anchor.
    /// </param>
    public delegate void AnchorLocatedDelegate(object sender, Microsoft.Azure.SpatialAnchors.AnchorLocatedEventArgs args);

    /// <summary>
    /// Informs the application that a session has been updated with new information.
    /// </summary>
    /// <param name="sender">
    /// The session that has been updated.
    /// </param>
    /// <param name="args">
    /// Information about the current session status.
    /// </param>
    public delegate void SessionUpdatedDelegate(object sender, Microsoft.Azure.SpatialAnchors.SessionUpdatedEventArgs args);

    /// <summary>
    /// Informs the application that an error occurred in a session.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the error.
    /// </param>
    public delegate void SessionErrorDelegate(object sender, Microsoft.Azure.SpatialAnchors.SessionErrorEventArgs args);

    /// <summary>
    /// Informs the application of a debug log message.
    /// </summary>
    /// <param name="sender">
    /// The session that fired the event.
    /// </param>
    /// <param name="args">
    /// Information about the log.
    /// </param>
    public delegate void OnLogDebugDelegate(object sender, Microsoft.Azure.SpatialAnchors.OnLogDebugEventArgs args);

    /// <summary>
    /// The exception that is thrown when an error occurs processing cloud spatial anchors.
    /// </summary>
    public class CloudSpatialException : Exception
    {
        private CloudSpatialErrorCode code;
        private string requestCorrelationVector;
        private string responseCorrelationVector;

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        public CloudSpatialException()
        {
            this.code = default(CloudSpatialErrorCode);
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code)
        {
            this.code = code;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message) : base(message)
        {
            this.code = code;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        /// <param name='requestCorrelationVector'>Request correlation vector for this exception.</param>
        /// <param name='responseCorrelationVector'>Response correlation vector for this exception.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message, string requestCorrelationVector, string responseCorrelationVector) : base(message)
        {
            this.code = code;
            this.requestCorrelationVector = requestCorrelationVector;
            this.responseCorrelationVector = responseCorrelationVector;
        }

        /// <summary>Creates a new instance of the <see cref='CloudSpatialException'/> class.</summary>
        /// <param name='code'>Error code for this exception.</param>
        /// <param name='message'>Plain text error message for this exception.</param>
        /// <param name='requestCorrelationVector'>Request correlation vector for this exception.</param>
        /// <param name='responseCorrelationVector'>Response correlation vector for this exception.</param>
        /// <param name='innerException'>Exception that caused this exception to be thrown.</param>
        public CloudSpatialException(CloudSpatialErrorCode code, string message, string requestCorrelationVector, string responseCorrelationVector, Exception inner) : base(message, inner)
        {
            this.code = code;
            this.requestCorrelationVector = requestCorrelationVector;
            this.responseCorrelationVector = responseCorrelationVector;
        }

        /// <summary>
        /// The error code associated with this exception.
        /// </summary>
        public CloudSpatialErrorCode ErrorCode
        {
            get { return this.code; }
        }

        /// <summary>
        /// The request correlation vector associated with this exception.
        /// </summary>
        public string RequestCorrelationVector
        {
            get { return this.requestCorrelationVector; }
        }

        /// <summary>
        /// The response correlation vector associated with this exception.
        /// </summary>
        public string ResponseCorrelationVector
        {
            get { return this.responseCorrelationVector; }
        }

    }

    /// <summary>
    /// Specifies a set of criteria for locating anchors.
    /// </summary>
    /// <remarks>
    /// Within the object, properties are combined with the AND operator. For example, if identifiers and nearAnchor are specified, then the filter will look for anchors that are near the nearAnchor and have an identifier that matches any of those identifiers.
    /// </remarks>
    public partial class AnchorLocateCriteria
    {
        internal IntPtr handle;
        internal AnchorLocateCriteria(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_anchor_locate_criteria_addref(ahandle);
        }
        public AnchorLocateCriteria()
        {
            status resultStatus = (NativeLibrary.ssc_anchor_locate_criteria_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~AnchorLocateCriteria()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Cloud anchor identifiers to locate. If empty, any anchors can be located.
        /// </summary>
        /// <remarks>
        /// Any anchors within this list will match this criteria.
        /// </remarks>
        public string[] Identifiers
        {
            get
            {
                IntPtr result;
                int result_length;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_identifiers(this.handle, out result, out result_length));
                return NativeLibraryHelpers.IntPtrToStringArray(result, result_length);
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_identifiers(this.handle, value, value.Length));
            }
        }

        /// <summary>
        /// Whether locate should bypass the local cache of anchors.
        /// </summary>
        public bool BypassCache
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_bypass_cache(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_bypass_cache(this.handle, value));
            }
        }

        /// <summary>
        /// Filters anchors to locate to be close to a specific anchor.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.NearAnchorCriteria NearAnchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.NearAnchorCriteria result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_near_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.NearAnchorCriteria(result_handle, transfer:true) : null;
                return result_object;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_near_anchor(this.handle, value.handle));
            }
        }

        /// <summary>
        /// Categories of data that are requested.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.AnchorDataCategory RequestedCategories
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.AnchorDataCategory result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_requested_categories(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_requested_categories(this.handle, value));
            }
        }

        /// <summary>
        /// Indicates the strategy by which anchors will be located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.LocateStrategy Strategy
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.LocateStrategy result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_get_strategy(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_locate_criteria_set_strategy(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Use this type to determine the status of an anchor after a locate operation.
    /// </summary>
    public partial class AnchorLocatedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal AnchorLocatedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_anchor_located_event_args_addref(ahandle);
        }
        ~AnchorLocatedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The cloud spatial anchor that was located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor Anchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// The spatial anchor that was located.
        /// </summary>
        public string Identifier
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Specifies whether the anchor was located, or the reason why it may have failed.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.LocateAnchorStatus Status
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.LocateAnchorStatus result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_status(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that located the anchor.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_anchor_located_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Use this class to represent an anchor in space that can be persisted in a CloudSpatialAnchorSession.
    /// </summary>
    public partial class CloudSpatialAnchor
    {
        internal IntPtr handle;
        internal CloudSpatialAnchor(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_addref(ahandle);
        }
        public CloudSpatialAnchor()
        {
            status resultStatus = (NativeLibrary.ssc_cloud_spatial_anchor_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~CloudSpatialAnchor()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The anchor in the local mixed reality system.
        /// </summary>
        public IntPtr LocalAnchor
        {
            get
            {
                IntPtr result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_local_anchor(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_set_local_anchor(this.handle, value));
            }
        }

        /// <summary>
        /// The time the anchor will expire.
        /// </summary>
        public System.DateTimeOffset Expiration
        {
            get
            {
                long result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_expiration(this.handle, out result));
                return (result == 0) ? DateTimeOffset.MaxValue : DateTimeOffset.FromUnixTimeMilliseconds(result);
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_set_expiration(this.handle, (value == DateTimeOffset.MaxValue) ? 0 : value.ToUnixTimeMilliseconds()));
            }
        }

        /// <summary>
        /// The persistent identifier of this spatial anchor in the cloud service.
        /// </summary>
        public string Identifier
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A dictionary of application-defined properties that is stored with the anchor.
        /// </summary>
        public System.Collections.Generic.IDictionary<string, string> AppProperties
        {
            get
            {
                IntPtr result_handle;
                IDictionary_String_String result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_app_properties(this.handle, out result_handle));
                result_object = new IDictionary_String_String(result_handle, transfer:true);
                return result_object;
            }
        }

        /// <summary>
        /// An opaque version tag that can be used for concurrency control.
        /// </summary>
        public string VersionTag
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_get_version_tag(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Use this class to defer completing an operation.
    /// </summary>
    /// <remarks>
    /// This is similar to the Windows.Foundation.Deferral class.
    /// </remarks>
    public partial class CloudSpatialAnchorSessionDeferral
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSessionDeferral(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_addref(ahandle);
        }
        ~CloudSpatialAnchorSessionDeferral()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Mark the deferred operation as complete and perform any associated tasks.
        /// </summary>
        public void Complete()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_deferral_complete(this.handle));
        }

    }

    /// <summary>
    /// Use this class to configure session diagnostics that can be collected and submitted to improve system quality.
    /// </summary>
    public partial class CloudSpatialAnchorSessionDiagnostics
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSessionDiagnostics(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_addref(ahandle);
        }
        ~CloudSpatialAnchorSessionDiagnostics()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Level of tracing to log.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionLogLevel LogLevel
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionLogLevel result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_log_level(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_log_level(this.handle, value));
            }
        }

        /// <summary>
        /// Directory into which temporary log files and manifests are saved.
        /// </summary>
        public string LogDirectory
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_log_directory(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_log_directory(this.handle, value));
            }
        }

        /// <summary>
        /// Approximate maximum disk space to be used, in megabytes.
        /// </summary>
        /// <remarks>
        /// When this value is set to zero, no information will be written to disk.
        /// </remarks>
        public int MaxDiskSizeInMB
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_max_disk_size_in_mb(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_max_disk_size_in_mb(this.handle, value));
            }
        }

        /// <summary>
        /// Whether images should be logged.
        /// </summary>
        public bool ImagesEnabled
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_get_images_enabled(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_set_images_enabled(this.handle, value));
            }
        }

        /// <summary>
        /// Creates a manifest of log files and submission information to be uploaded.
        /// </summary>
        /// <param name="description">
        /// Description to be added to the diagnostics manifest.
        /// </param>
        public async System.Threading.Tasks.Task<string> CreateManifestAsync(string description)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_create_manifest_async(this.handle, description, out result));
                return result;
            });
        }

        /// <summary>
        /// Submits a diagnostics manifest and cleans up its resources.
        /// </summary>
        /// <param name="manifestPath">
        /// Path to the manifest file to submit.
        /// </param>
        public async System.Threading.Tasks.Task SubmitManifestAsync(string manifestPath)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_diagnostics_submit_manifest_async(this.handle, manifestPath));
            });
        }

    }

    /// <summary>
    /// Use this class to create, locate and manage spatial anchors.
    /// </summary>
    public partial class CloudSpatialAnchorSession : IDisposable, ICookie
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorSession(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_session_addref(ahandle);
            CookieTracker<CloudSpatialAnchorSession>.Add(this);
        }
        /// <summary>
        /// Initializes a new instance with a default configuration.
        /// </summary>
        public CloudSpatialAnchorSession()
        {
            status resultStatus = (NativeLibrary.ssc_cloud_spatial_anchor_session_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
            CookieTracker<CloudSpatialAnchorSession>.Add(this);
            // Custom initialization (UnityEditor/Unity) begins for CloudSpatialAnchorSession.
            // Custom initialization (UnityEditor/Unity) ends for CloudSpatialAnchorSession.
        }

        ~CloudSpatialAnchorSession()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The configuration information for the session.
        /// </summary>
        /// <remarks>
        /// Configuration settings take effect when the session is started.
        /// </remarks>
        public Microsoft.Azure.SpatialAnchors.SessionConfiguration Configuration
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionConfiguration result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_configuration(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.SessionConfiguration(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// The diagnostics settings for the session, which can be used to collect and submit data for troubleshooting and improvements.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics Diagnostics
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_diagnostics(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDiagnostics(result_handle, transfer:true) : null;
                return result_object;
            }
        }

        /// <summary>
        /// Logging level for the session log events.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionLogLevel LogLevel
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionLogLevel result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_log_level(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_log_level(this.handle, value));
            }
        }

        /// <summary>
        /// The tracking session used to help locate anchors.
        /// </summary>
        /// <remarks>
        /// This property is not available on the HoloLens platform.
        /// </remarks>
        public IntPtr Session
        {
            get
            {
                IntPtr result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_session(this.handle, value));
            }
        }

        /// <summary>
        /// The unique identifier for the session.
        /// </summary>
        public string SessionId
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session_id(this.handle, out result));
                return result;
            }
        }

        private ulong cookie;
        ulong ICookie.Cookie { get { return this.cookie; } set { this.cookie = value; } }
        /// <summary>Registered callbacks on this instance.</summary>
        private event TokenRequiredDelegate _TokenRequired;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(TokenRequiredDelegateNative))]
        private static void TokenRequiredStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            TokenRequiredDelegate handler = (instance == null) ? null : instance._TokenRequired;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.TokenRequiredEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static TokenRequiredDelegateNative TokenRequiredStaticHandlerDelegate = TokenRequiredStaticHandler;
        public event TokenRequiredDelegate TokenRequired
        {
            add
            {
                this._TokenRequired += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_token_required(this.handle, this.cookie, TokenRequiredStaticHandlerDelegate));
            }
            remove
            {
                this._TokenRequired -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event AnchorLocatedDelegate _AnchorLocated;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(AnchorLocatedDelegateNative))]
        private static void AnchorLocatedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            AnchorLocatedDelegate handler = (instance == null) ? null : instance._AnchorLocated;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.AnchorLocatedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static AnchorLocatedDelegateNative AnchorLocatedStaticHandlerDelegate = AnchorLocatedStaticHandler;
        public event AnchorLocatedDelegate AnchorLocated
        {
            add
            {
                this._AnchorLocated += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_anchor_located(this.handle, this.cookie, AnchorLocatedStaticHandlerDelegate));
            }
            remove
            {
                this._AnchorLocated -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event LocateAnchorsCompletedDelegate _LocateAnchorsCompleted;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(LocateAnchorsCompletedDelegateNative))]
        private static void LocateAnchorsCompletedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            LocateAnchorsCompletedDelegate handler = (instance == null) ? null : instance._LocateAnchorsCompleted;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.LocateAnchorsCompletedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static LocateAnchorsCompletedDelegateNative LocateAnchorsCompletedStaticHandlerDelegate = LocateAnchorsCompletedStaticHandler;
        public event LocateAnchorsCompletedDelegate LocateAnchorsCompleted
        {
            add
            {
                this._LocateAnchorsCompleted += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_locate_anchors_completed(this.handle, this.cookie, LocateAnchorsCompletedStaticHandlerDelegate));
            }
            remove
            {
                this._LocateAnchorsCompleted -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event SessionUpdatedDelegate _SessionUpdated;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(SessionUpdatedDelegateNative))]
        private static void SessionUpdatedStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            SessionUpdatedDelegate handler = (instance == null) ? null : instance._SessionUpdated;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.SessionUpdatedEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static SessionUpdatedDelegateNative SessionUpdatedStaticHandlerDelegate = SessionUpdatedStaticHandler;
        public event SessionUpdatedDelegate SessionUpdated
        {
            add
            {
                this._SessionUpdated += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_session_updated(this.handle, this.cookie, SessionUpdatedStaticHandlerDelegate));
            }
            remove
            {
                this._SessionUpdated -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event SessionErrorDelegate _Error;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(SessionErrorDelegateNative))]
        private static void ErrorStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            SessionErrorDelegate handler = (instance == null) ? null : instance._Error;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.SessionErrorEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static SessionErrorDelegateNative ErrorStaticHandlerDelegate = ErrorStaticHandler;
        public event SessionErrorDelegate Error
        {
            add
            {
                this._Error += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_error(this.handle, this.cookie, ErrorStaticHandlerDelegate));
            }
            remove
            {
                this._Error -= value;
            }
        }

        /// <summary>Registered callbacks on this instance.</summary>
        private event OnLogDebugDelegate _OnLogDebug;
        /// <summary>Static handler.</summary>
        [AOT.MonoPInvokeCallback(typeof(OnLogDebugDelegateNative))]
        private static void OnLogDebugStaticHandler(ulong cookie, IntPtr args)
        {
            var instance = CookieTracker<CloudSpatialAnchorSession>.Lookup(cookie);
            OnLogDebugDelegate handler = (instance == null) ? null : instance._OnLogDebug;
            if (handler != null) handler(instance, new Microsoft.Azure.SpatialAnchors.OnLogDebugEventArgs(args, transfer:false));
        }
        /// <summary>This static delegate instance keeps callbacks alive.</summary>
        private static OnLogDebugDelegateNative OnLogDebugStaticHandlerDelegate = OnLogDebugStaticHandler;
        public event OnLogDebugDelegate OnLogDebug
        {
            add
            {
                this._OnLogDebug += value;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_set_on_log_debug(this.handle, this.cookie, OnLogDebugStaticHandlerDelegate));
            }
            remove
            {
                this._OnLogDebug -= value;
            }
        }

        /// <summary>
        /// Stops this session and releases all associated resources.
        /// </summary>
        public void Dispose()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_dispose(this.handle));
        }

        /// <summary>
        /// Gets the Azure Spatial Anchors access token from authentication token.
        /// </summary>
        /// <param name="authenticationToken">
        /// Authentication token.
        /// </param>
        public async System.Threading.Tasks.Task<string> GetAccessTokenWithAuthenticationTokenAsync(string authenticationToken)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_access_token_with_authentication_token_async(this.handle, authenticationToken, out result));
                return result;
            });
        }

        /// <summary>
        /// Gets the Azure Spatial Anchors access token from account key.
        /// </summary>
        /// <param name="accountKey">
        /// Account key.
        /// </param>
        public async System.Threading.Tasks.Task<string> GetAccessTokenWithAccountKeyAsync(string accountKey)
        {
            return await Task.Run(() =>
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_access_token_with_account_key_async(this.handle, accountKey, out result));
                return result;
            });
        }

        /// <summary>
        /// Creates a new persisted spatial anchor from the specified local anchor and string properties.
        /// </summary>
        /// <param name="anchor">
        /// Anchor to be persisted.
        /// </param>
        public async System.Threading.Tasks.Task CreateAnchorAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_create_anchor_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Creates a new object that watches for anchors that meet the specified criteria.
        /// </summary>
        /// <param name="criteria">
        /// Criteria for anchors to watch for.
        /// </param>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher CreateWatcher(Microsoft.Azure.SpatialAnchors.AnchorLocateCriteria criteria)
        {
            IntPtr result_handle;
            Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_create_watcher(this.handle, criteria.handle, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
            return result_object;
        }

        /// <summary>
        /// Gets a cloud spatial anchor for the given identifier, even if it hasn't been located yet.
        /// </summary>
        /// <param name="identifier">
        /// The identifier to look for.
        /// </param>
        public async System.Threading.Tasks.Task<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor> GetAnchorPropertiesAsync(string identifier)
        {
            return await Task.Run(() =>
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_anchor_properties_async(this.handle, identifier, out result_handle));
                result_object = new CloudSpatialAnchor(result_handle, transfer:true);
                return result_object;
            });
        }

        /// <summary>
        /// Gets a list of active watchers.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher> GetActiveWatchers()
        {
            System.Collections.Generic.IReadOnlyList<Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher> result;
            IntPtr[] result_array;
            int result_count;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_active_watchers_count(this.handle, out result_count));
            result_array = new IntPtr[result_count];
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_active_watchers_items(this.handle, result_array, ref result_count));
            result = result_array.Take(result_count).Select(handle => new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(handle, transfer:true)).ToList().AsReadOnly();
            return result;
        }

        /// <summary>
        /// Refreshes properties for the specified spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to refresh.
        /// </param>
        public async System.Threading.Tasks.Task RefreshAnchorPropertiesAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_refresh_anchor_properties_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Updates the specified spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to be updated.
        /// </param>
        public async System.Threading.Tasks.Task UpdateAnchorPropertiesAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_update_anchor_properties_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Deletes a persisted spatial anchor.
        /// </summary>
        /// <param name="anchor">
        /// The anchor to be deleted.
        /// </param>
        public async System.Threading.Tasks.Task DeleteAnchorAsync(Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor anchor)
        {
            await Task.Run(() =>
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_delete_anchor_async(this.handle, anchor.handle));
            });
        }

        /// <summary>
        /// Applications must call this method on platforms where per-frame processing is required.
        /// </summary>
        /// <param name="frame">
        /// AR frame to process.
        /// </param>
        /// <remarks>
        /// This method is not available on the HoloLens platform.
        /// </remarks>
        public void ProcessFrame(IntPtr frame)
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_process_frame(this.handle, frame));
        }

        /// <summary>
        /// Gets an object describing the status of the session.
        /// </summary>
        public async System.Threading.Tasks.Task<Microsoft.Azure.SpatialAnchors.SessionStatus> GetSessionStatusAsync()
        {
            return await Task.Run(() =>
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionStatus result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_get_session_status_async(this.handle, out result_handle));
                result_object = new SessionStatus(result_handle, transfer:true);
                return result_object;
            });
        }

        /// <summary>
        /// Begins capturing environment data for the session.
        /// </summary>
        public void Start()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_start(this.handle));
        }

        /// <summary>
        /// Stops capturing environment data for the session and cancels any outstanding locate operations. Environment data is maintained.
        /// </summary>
        public void Stop()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_stop(this.handle));
        }

        /// <summary>
        /// Resets environment data that has been captured in this session; applications must call this method when tracking is lost.
        /// </summary>
        /// <remarks>
        /// On any platform, calling the method will clean all internal cached state.
        /// </remarks>
        public void Reset()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_session_reset(this.handle));
        }

    }

    /// <summary>
    /// Use this class to control an object that watches for spatial anchors.
    /// </summary>
    public partial class CloudSpatialAnchorWatcher
    {
        internal IntPtr handle;
        internal CloudSpatialAnchorWatcher(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_cloud_spatial_anchor_watcher_addref(ahandle);
        }
        ~CloudSpatialAnchorWatcher()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Distinct identifier for the watcher within its session.
        /// </summary>
        public int Identifier
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_get_identifier(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Stops further activity from this watcher.
        /// </summary>
        public void Stop()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_cloud_spatial_anchor_watcher_stop(this.handle));
        }

    }

    /// <summary>
    /// Use this type to determine when a locate operation has completed.
    /// </summary>
    public partial class LocateAnchorsCompletedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal LocateAnchorsCompletedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_locate_anchors_completed_event_args_addref(ahandle);
        }
        ~LocateAnchorsCompletedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Gets a value indicating whether the locate operation was canceled.
        /// </summary>
        /// <remarks>
        /// When this property is true, the watcher was stopped before completing.
        /// </remarks>
        public bool Cancelled
        {
            get
            {
                bool result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_get_cancelled(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that completed the locate operation.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_locate_anchors_completed_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Use this class to describe how anchors to be located should be near a source anchor.
    /// </summary>
    public partial class NearAnchorCriteria
    {
        internal IntPtr handle;
        internal NearAnchorCriteria(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_near_anchor_criteria_addref(ahandle);
        }
        public NearAnchorCriteria()
        {
            status resultStatus = (NativeLibrary.ssc_near_anchor_criteria_create(out this.handle));
            NativeLibraryHelpers.CheckStatus(this.handle, resultStatus);
        }

        ~NearAnchorCriteria()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Source anchor around which nearby anchors should be located.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor SourceAnchor
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_source_anchor(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchor(result_handle, transfer:true) : null;
                return result_object;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_source_anchor(this.handle, value.handle));
            }
        }

        /// <summary>
        /// Maximum distance in meters from the source anchor (defaults to 5).
        /// </summary>
        public float DistanceInMeters
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_distance_in_meters(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_distance_in_meters(this.handle, value));
            }
        }

        /// <summary>
        /// Maximum desired result count (defaults to 20).
        /// </summary>
        public int MaxResultCount
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_get_max_result_count(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_near_anchor_criteria_set_max_result_count(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires for logging messages.
    /// </summary>
    public partial class OnLogDebugEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal OnLogDebugEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_on_log_debug_event_args_addref(ahandle);
        }
        ~OnLogDebugEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_on_log_debug_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The logging message.
        /// </summary>
        public string Message
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_on_log_debug_event_args_get_message(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Use this class to set up the service configuration for a SpatialAnchorSession.
    /// </summary>
    public partial class SessionConfiguration
    {
        internal IntPtr handle;
        internal SessionConfiguration(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_configuration_addref(ahandle);
        }
        ~SessionConfiguration()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Account domain for the Azure Spatial Anchors service.
        /// </summary>
        /// <remarks>
        /// The default is "mixedreality.azure.com".
        /// </remarks>
        public string AccountDomain
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_domain(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_domain(this.handle, value));
            }
        }

        /// <summary>
        /// Account-level ID for the Azure Spatial Anchors service.
        /// </summary>
        public string AccountId
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_id(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_id(this.handle, value));
            }
        }

        /// <summary>
        /// Authentication token for Azure Active Directory (AAD).
        /// </summary>
        /// <remarks>
        /// If the access token and the account key are missing, the session will obtain an access token based on this value.
        /// </remarks>
        public string AuthenticationToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_authentication_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_authentication_token(this.handle, value));
            }
        }

        /// <summary>
        /// Account-level key for the Azure Spatial Anchors service.
        /// </summary>
        public string AccountKey
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_account_key(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_account_key(this.handle, value));
            }
        }

        /// <summary>
        /// Access token for the Azure Spatial Anchors service.
        /// </summary>
        public string AccessToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_get_access_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_configuration_set_access_token(this.handle, value));
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires when errors are thrown.
    /// </summary>
    public partial class SessionErrorEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal SessionErrorEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_error_event_args_addref(ahandle);
        }
        ~SessionErrorEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_get_error_message(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The watcher that found an error, possibly null.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher Watcher
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_error_event_args_get_watcher(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorWatcher(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// This type describes the status of spatial data processing.
    /// </summary>
    public partial class SessionStatus
    {
        internal IntPtr handle;
        internal SessionStatus(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_status_addref(ahandle);
        }
        ~SessionStatus()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The level of data sufficiency for a successful operation.
        /// </summary>
        /// <remarks>
        /// This value will be in the [0;1) range when data is insufficient; 1 when data is sufficient for success and greater than 1 when conditions are better than minimally sufficient.
        /// </remarks>
        public float ReadyForCreateProgress
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_ready_for_create_progress(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// The ratio of data available to recommended data to create an anchor.
        /// </summary>
        /// <remarks>
        /// This value will be in the [0;1) range when data is below the recommended threshold; 1 and greater when the recommended amount of data has been gathered for a creation operation.
        /// </remarks>
        public float RecommendedForCreateProgress
        {
            get
            {
                float result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_recommended_for_create_progress(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A hash value that can be used to know when environment data that contributes to a creation operation has changed to included the latest input data.
        /// </summary>
        /// <remarks>
        /// If the hash value does not change after new frames were added to the session, then those frames were not deemed as sufficientlyy different from existing environment data and disgarded. This value may be 0 (and should be ignored) for platforms that don't feed frames individually.
        /// </remarks>
        public int SessionCreateHash
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_session_create_hash(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// A hash value that can be used to know when environment data that contributes to a locate operation has changed to included the latest input data.
        /// </summary>
        /// <remarks>
        /// If the hash value does not change after new frames were added to the session, then those frames were not deemed as sufficiency different from existing environment data and disgarded. This value may be 0 (and should be ignored) for platforms that don't feed frames individually.
        /// </remarks>
        public int SessionLocateHash
        {
            get
            {
                int result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_session_locate_hash(this.handle, out result));
                return result;
            }
        }

        /// <summary>
        /// Feedback that can be provided to user about data processing status.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionUserFeedback UserFeedback
        {
            get
            {
                Microsoft.Azure.SpatialAnchors.SessionUserFeedback result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_status_get_user_feedback(this.handle, out result));
                return result;
            }
        }

    }

    /// <summary>
    /// Provides data for the event that fires when the session state is updated.
    /// </summary>
    public partial class SessionUpdatedEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal SessionUpdatedEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_session_updated_event_args_addref(ahandle);
        }
        ~SessionUpdatedEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_updated_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Current session status.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.SessionStatus Status
        {
            get
            {
                IntPtr result_handle;
                Microsoft.Azure.SpatialAnchors.SessionStatus result_object;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_session_updated_event_args_get_status(this.handle, out result_handle));
                result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.SessionStatus(result_handle, transfer:true) : null;
                return result_object;
            }
        }

    }

    /// <summary>
    /// Informs the application that the service requires an updated access token or authentication token.
    /// </summary>
    public partial class TokenRequiredEventArgs : EventArgs
    {
        internal IntPtr handle;
        internal TokenRequiredEventArgs(IntPtr ahandle, bool transfer)
        {
            this.handle = ahandle;
            if (!transfer) NativeLibrary.ssc_token_required_event_args_addref(ahandle);
        }
        ~TokenRequiredEventArgs()
        {
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_release(this.handle));
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// The access token to be used by the operation that requires it.
        /// </summary>
        public string AccessToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_access_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_set_access_token(this.handle, value));
            }
        }

        /// <summary>
        /// The authentication token to be used by the operation that requires it.
        /// </summary>
        public string AuthenticationToken
        {
            get
            {
                string result;
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_authentication_token(this.handle, out result));
                return result;
            }
            set
            {
                NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_set_authentication_token(this.handle, value));
            }
        }

        /// <summary>
        /// Returns a deferral object that can be used to provide an updated access token or authentication token from another asynchronous operation.
        /// </summary>
        public Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral GetDeferral()
        {
            IntPtr result_handle;
            Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral result_object;
            NativeLibraryHelpers.CheckStatus(this.handle, NativeLibrary.ssc_token_required_event_args_get_deferral(this.handle, out result_handle));
            result_object = (result_handle != IntPtr.Zero) ? new Microsoft.Azure.SpatialAnchors.CloudSpatialAnchorSessionDeferral(result_handle, transfer:true) : null;
            return result_object;
        }

    }

}

#endif
