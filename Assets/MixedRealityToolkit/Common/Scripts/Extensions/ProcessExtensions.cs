// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR || !UNITY_WSA
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace MixedRealityToolkit.Common.Extensions
{
    public static class ProcessExtensions
    {
        public static async Task<ProcessResult> StartProcessAsync(this Process process, string fileName, string args,
            bool showDebug = false)
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

        public static async Task<ProcessResult> StartProcessAsync(this Process process, ProcessStartInfo startInfo,
            bool showDebug = false)
        {
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            var processResult = new TaskCompletionSource<ProcessResult>();
            var errorCodeResult = new TaskCompletionSource<string[]>();
            var errorList = new List<string>();
            var outputCodeResult = new TaskCompletionSource<string[]>();
            var outputList = new List<string>();

            process.Exited += async (sender, args) =>
            {
                processResult.TrySetResult(new ProcessResult(process.ExitCode, await errorCodeResult.Task,
                    await outputCodeResult.Task));
                process.Close();
                process.Dispose();
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    errorList.Add(args.Data);
                    if (showDebug) Debug.LogError(args.Data);
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
                    if (showDebug) Debug.Log(args.Data);
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

                processResult.TrySetResult(
                    new ProcessResult(process.ExitCode, new[] { "Failed to start process!" }, null));
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return await processResult.Task;
        }
    }
}
#endif
