// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR || !UNITY_WSA
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Debug = UnityEngine.Debug;

namespace Microsoft.MixedReality.Toolkit.Core.Extensions
{
    /// <summary>
    /// Process Extension class.
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>
        /// Starts a process asynchronously.
        /// </summary>
        /// <param name="process">This Process.</param>
        /// <param name="fileName">The process executable to run.</param>
        /// <param name="args">The Process arguments.</param>
        /// <param name="showDebug">Should output debug code to Editor Console?</param>
        /// <returns><see cref="ProcessResult"/></returns>
        public static async Task<ProcessResult> StartProcessAsync(this Process process, string fileName, string args, bool showDebug = false)
        {
            return await StartProcessAsync(process, new ProcessStartInfo
            {
                FileName = fileName,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = args
            }, showDebug);
        }

        /// <summary>
        /// Starts a process asynchronously.<para/>
        /// <remarks>The provided Process Start Info must not use shell execution, and should redirect the standard output and errors.</remarks>
        /// </summary>
        /// <param name="process">This Process.</param>
        /// <param name="startInfo">The Process start info.</param>
        /// <param name="showDebug">Should output debug code to Editor Console?</param>
        /// <returns><see cref="ProcessResult"/></returns>
        public static async Task<ProcessResult> StartProcessAsync(this Process process, ProcessStartInfo startInfo, bool showDebug = false)
        {
            Debug.Assert(!startInfo.UseShellExecute, "Process Start Info must not use shell execution.");
            Debug.Assert(startInfo.RedirectStandardOutput, "Process Start Info must redirect standard output.");
            Debug.Assert(startInfo.RedirectStandardError, "Process Start Info must redirect standard errors.");

            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            var processResult = new TaskCompletionSource<ProcessResult>();
            var errorCodeResult = new TaskCompletionSource<string[]>();
            var errorList = new List<string>();
            var outputCodeResult = new TaskCompletionSource<string[]>();
            var outputList = new List<string>();

            process.Exited += async (sender, args) =>
            {
                processResult.TrySetResult(new ProcessResult(process.ExitCode, await errorCodeResult.Task, await outputCodeResult.Task));
                process.Close();
                process.Dispose();
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    errorList.Add(args.Data);
                    if (showDebug) { Debug.LogError(args.Data); }
                }
                else
                {
                    errorCodeResult.TrySetResult(errorList.ToArray());
                }
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    outputList.Add(args.Data);
                    if (showDebug) { Debug.Log(args.Data); }
                }
                else
                {
                    outputCodeResult.TrySetResult(outputList.ToArray());
                }
            };

            if (!process.Start())
            {
                if (showDebug)
                {
                    Debug.LogError("Failed to start process!");
                }

                processResult.TrySetResult(new ProcessResult(process.ExitCode, new[] { "Failed to start process!" }, null));
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return await processResult.Task;
        }
    }
}
#endif
