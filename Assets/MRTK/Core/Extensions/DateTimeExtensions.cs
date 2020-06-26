// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// <see cref="System.DateTime"/> Extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets string literal for relative time from now since the DateTime provided. String output is in most appropriate "x time units ago"
        /// Example: If DateTime provided is 30 seconds before now, then result will be "30 seconds ago"
        /// </summary>
        /// <param name="time">DateTime in UTC to compare against DateTime.UtcNow</param>
        /// <returns>Encoded string.</returns>
        public static string GetRelativeTime(this DateTime time)
        {
            var delta = new TimeSpan(DateTime.UtcNow.Ticks - time.Ticks);

            if (Math.Abs(delta.TotalDays) > 1.0)
            {
                return (int)Math.Abs(delta.TotalDays) + " days ago";
            }
            else if (Math.Abs(delta.TotalHours) > 1.0)
            {
                return (int)Math.Abs(delta.TotalHours) + " hours ago";
            }
            else if (Math.Abs(delta.TotalMinutes) > 1.0)
            {
                return (int)Math.Abs(delta.TotalMinutes) + " minutes ago";
            }
            else
            {
                return (int)Math.Abs(delta.TotalSeconds) + " seconds ago";
            }
        }
    }
}