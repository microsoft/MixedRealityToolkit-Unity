//
// Copyright (C) Microsoft. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public static class AudioUtil 
{
    //------------------------------------------------------------------------------------
    // byte - float array fast block copy
    public static void ToByteArray4(this float[] input, byte[] output, int lengthInput)
    {
        Buffer.BlockCopy(input, 0, output, 0, lengthInput * 4);
    }

    public static void ToFloatArray4(this byte[] input, float[] output, int lengthOutput)
    {
        Buffer.BlockCopy(input, 0, output, 0, lengthOutput * 4);
    }

    //------------------------------------------------------------------------------------
    // short - float array conversion
    public static void ToShortArray(this float[] input, short[] output, int lengthInput)
    {
        // output needs to be equal or bigger than input
        for (int i = 0; i < lengthInput; ++i)
        {
            output[i] = (short)Mathf.Clamp((int)(input[i] * 32767.0f), short.MinValue, short.MaxValue);
        }
    }

    public static void ToFloatArray(this short[] input, float[] output, int length)
    {
        // input needs to be equal or bigger than output
        for (int i = 0; i < length; ++i)
        {
            output[i] = input[i] / (float)short.MaxValue;
        }
    }

    public class HiresTimer
    {
        private long startTime;
        private long markerTime;
        private long freq;

        public HiresTimer()
        {
            startTime = 0;
            markerTime = 0;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                Debug.Log("High performance counters not supported");
            }

            Start();
        }

        /// <summary>
        /// record the start time
        /// </summary>
        public void Start()
        {
            QueryPerformanceCounter(out startTime);
        }

        /// <summary>
        /// record the marker time
        /// </summary>
        public void Marker()
        {
            QueryPerformanceCounter(out markerTime);
        }

        /// <summary>
        /// get the elapsed time in milliseconds as a double
        /// </summary>
        public double MilliElapsed
        {
            get
            {
                Marker();
                return 1000.0 * (double)(markerTime - startTime) / (double)freq;
            }
        }

        /// <summary>
        /// get the current time in milliseconds as a double
        /// </summary>
        public double MilliNow
        {
            get
            {
                Marker();
                return 1000.0 * (double)markerTime / (double)freq;
            }
        }

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long performanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long frequency);
    }
}
