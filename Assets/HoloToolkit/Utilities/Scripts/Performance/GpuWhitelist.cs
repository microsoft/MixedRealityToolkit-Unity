using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System;

namespace HoloToolkit.Unity
{
    public class GpuWhitelist
    {
        //references:
        //https://developer.microsoft.com/en-us/windows/mixed-reality/windows_mixed_reality_minimum_pc_hardware_compatibility_guidelines
        //https://en.wikipedia.org/wiki/List_of_AMD_graphics_processing_units#Radeon_RX_400_Series
        //https://en.wikipedia.org/wiki/List_of_AMD_graphics_processing_units#Radeon_RX_Vega_Series
        //https://en.wikipedia.org/wiki/List_of_Nvidia_graphics_processing_units#GeForce_900_series
        //https://en.wikipedia.org/wiki/List_of_Nvidia_graphics_processing_units#GeForce_10_series
        //https://en.wikipedia.org/wiki/List_of_Nvidia_graphics_processing_units#GeForce_900M_.289xxM.29_series

        public static string[] MainstreamAMD =
        {
    };

        public static string[] MainstreamNVIDIA =
        {
        "940MX",
        "MX150",
    };

        public static string[] LowEndUltraAMD =
        {
        "RX 460",
        "RX 560"
    };

        public static string[] LowEndUltraNVIDIA =
        {
        "GTX 1050 Ti Mobile",
        "GTX 1050 Ti",
        "GTX 1050 Mobile",
        "GTX 1050",
        "GTX 950",
        "GTX 960",
        "GTX 965M",
    };

        public static string[] UltraAMD3 =
        {
        "Pro WX 8100",
        "V7300X",
        "V7350x2",
        "Pro WX 5100",
        "EllesmereM GL Pro",

    };

        public static string[] UltraNVIDIA3 =
        {
        "GTX 970M",
        "GTX 980M",
    };

        public static string[] UltraAMD4 =
        {
        "Polaris 22",
        "VEGA10 DESKTOP XT",
        "VEGA10 GLXT SERVER VF Pro",
        "Vega10 GLXTX Radeon Vega Frontier Edition",
        "VEGA10 GLXL",
        "VEGA10 GLXT Radeon (TM) Pro WX 9100",
        "VEGA10 SSG",
        "VEGA10",
        "Pro WX 7100",
        "Baffin",
        "VEGA10",
        "RX 470",
        "RX 480",
        "RX 570"
    };

        public static string[] UltraNVIDIA4 =
        {
        "GTX 1060 Mobile",
        "GTX 1060",
        "GTX 970M",
        "GTX 980M",
        "QUADRO GP100",
        "QUADRO M5000",
        "QUADRO M6000",
        "QUADRO P4000",
        "QUADRO P5000",
        "QUADRO M5500",
        "QUADRO P6000",
        "QUADRO P5200",
        "TITAN Z",
        "TITAN Xp",
        "TITAN X",
        "GTX TITAN X",
    };

        public static string[] HighEndUltraAMD =
        {
    };

        public static string[] HighEndUltraNVIDIA =
        {
        "GTX 970",
        "GTX 980",
        "GTX 980 Ti",
        "GTX 1070 Mobile",
        "GTX 1070",
        "GTX 1080",
        "GTX 1080 Ti",
    };

        public static string GPUVendorName = "";
        public static string GPUTypeName = "";
        public static string PerformanceLevelString = "";

        public static SystemPerformanceLevel PerformanceLevel = SystemPerformanceLevel.Undefined;

        public static bool IsPerformanceLevelFound
        {
            get
            {
                return PerformanceLevel != SystemPerformanceLevel.Undefined;
            }
        }

        public static SystemPerformanceLevel EnsurePerformanceLevelFound(uint? graphicsDeviceVendorID = null,
            string graphicsDeviceName = null, uint? graphicsMemorySize = null)
        {
            if (!IsPerformanceLevelFound)
            {
                return FindPerformanceLevel(graphicsDeviceVendorID, graphicsDeviceName, graphicsMemorySize);
            }

            return PerformanceLevel;
        }

        // Warning! This method is an outline and not guaranteed to work for all GPUs
        public static SystemPerformanceLevel FindPerformanceLevel(uint? graphicsDeviceVendorID = null,
            string graphicsDeviceName = null, uint? graphicsMemorySize = null)
        {
            var GPUVendor = GetGPUVendor(graphicsDeviceVendorID);
            GPUVendorName = GPUVendor.ToString();

            var PerformanceLevel = SystemPerformanceLevel.Undefined;

            var ultraAMD = new List<string>(UltraAMD3.Length + UltraAMD4.Length);
            ultraAMD.AddRange(UltraAMD3);
            ultraAMD.AddRange(UltraAMD4);

            var ultraNVIDIA = new List<string>(UltraNVIDIA3.Length + UltraNVIDIA4.Length);
            ultraNVIDIA.AddRange(UltraNVIDIA3);
            ultraNVIDIA.AddRange(UltraNVIDIA4);

            try
            {
                if (graphicsDeviceName == null)
                {
                    graphicsDeviceName = SystemInfo.graphicsDeviceName.Trim().ToLower();
                }
                else
                {
                    graphicsDeviceName = graphicsDeviceName.Trim().ToLower();
                }

                if (GPUVendor == GPUVendor.Intel)
                {
                    PerformanceLevel = SystemPerformanceLevel.Mainstream;
                }
                else if (GPUVendor == GPUVendor.AMD)
                {
                    foreach (var gpuString in MainstreamAMD)
                    {
                        // Compare string length to always take more specific GPU name
                        if (graphicsDeviceName.Contains(gpuString.ToLower()) && GPUTypeName.Length < gpuString.Length)
                        {
                            GPUTypeName = gpuString;
                            PerformanceLevel = SystemPerformanceLevel.Mainstream;
                        }
                    }

                    foreach (var gpuString in LowEndUltraAMD)
                    {
                        if (graphicsDeviceName.Contains(gpuString.ToLower()) && GPUTypeName.Length < gpuString.Length)
                        {
                            GPUTypeName = gpuString;
                            PerformanceLevel = SystemPerformanceLevel.LowEndUltra;
                        }
                    }

                    foreach (var gpuString in ultraAMD)
                    {
                        if (graphicsDeviceName.Contains(gpuString.ToLower()) && GPUTypeName.Length < gpuString.Length)
                        {
                            GPUTypeName = gpuString;
                            PerformanceLevel = SystemPerformanceLevel.Ultra;
                        }
                    }

                    foreach (var gpuString in HighEndUltraAMD)
                    {
                        if (graphicsDeviceName.Contains(gpuString.ToLower()) && GPUTypeName.Length < gpuString.Length)
                        {
                            GPUTypeName = gpuString;
                            PerformanceLevel = SystemPerformanceLevel.HighEndUltra;
                        }
                    }
                }
                else if (GPUVendor == GPUVendor.NVIDIA)
                {
                    foreach (var gpuString in MainstreamNVIDIA)
                    {
                        if (graphicsDeviceName.Contains(gpuString.ToLower()) && GPUTypeName.Length < gpuString.Length)
                        {
                            GPUTypeName = gpuString;
                            PerformanceLevel = SystemPerformanceLevel.Mainstream;
                        }
                    }

                    foreach (var gpuString in LowEndUltraNVIDIA)
                    {
                        if (graphicsDeviceName.Contains(gpuString.ToLower()) && GPUTypeName.Length < gpuString.Length)
                        {
                            GPUTypeName = gpuString;
                            PerformanceLevel = SystemPerformanceLevel.LowEndUltra;
                        }
                    }

                    foreach (var gpuString in ultraNVIDIA)
                    {
                        if (graphicsDeviceName.Contains(gpuString.ToLower()) && GPUTypeName.Length < gpuString.Length)
                        {
                            GPUTypeName = gpuString;
                            PerformanceLevel = SystemPerformanceLevel.Ultra;
                        }
                    }

                    foreach (var gpuString in HighEndUltraNVIDIA)
                    {
                        if (graphicsDeviceName.Contains(gpuString.ToLower()) && GPUTypeName.Length < gpuString.Length)
                        {
                            GPUTypeName = gpuString;
                            PerformanceLevel = SystemPerformanceLevel.HighEndUltra;
                        }
                    }
                }
            }
            catch (Exception) { }

            // Check for <2GB of memeory
            try
            {
                if (!graphicsMemorySize.HasValue)
                {
                    graphicsMemorySize = (uint)SystemInfo.graphicsMemorySize;
                }
                if (PerformanceLevel != SystemPerformanceLevel.Mainstream)
                {
                    if (graphicsMemorySize <= (2048 + 128))
                    {
                        PerformanceLevel = SystemPerformanceLevel.LowEndUltra;
                    }
                }
            }
            catch (Exception) { }

            PerformanceLevelString = PerformanceLevel.ToString();
            GpuWhitelist.PerformanceLevel = PerformanceLevel;
            return PerformanceLevel;
        }

        public static GPUVendor GetGPUVendor(uint? graphicsDeviceVendorID = null)
        {
            try
            {
                if (!graphicsDeviceVendorID.HasValue)
                {
                    graphicsDeviceVendorID = (uint)SystemInfo.graphicsDeviceVendorID;
                }

                if (graphicsDeviceVendorID == 32902)
                {
                    return GPUVendor.Intel;
                }

                if (graphicsDeviceVendorID == 4318)
                {
                    return GPUVendor.NVIDIA;
                }

                if (graphicsDeviceVendorID == 4098)
                {
                    return GPUVendor.AMD;
                }

                var graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor.Trim().ToLower();

                if (graphicsDeviceVendor.Equals("intel"))
                {
                    return GPUVendor.Intel;
                }

                if (graphicsDeviceVendor.Equals("nvidia"))
                {
                    return GPUVendor.NVIDIA;
                }

                if (graphicsDeviceVendor.Equals("ati"))
                {
                    return GPUVendor.AMD;
                }
            }
            catch (Exception) { }

            return GPUVendor.unknown;
        }

        /// <summary>
        /// This function is meant to be used to keep track of what 
        /// machine specs give your app trouble. You will likely want to
        /// output other info such as the FPS and/or the bucket you are on
        /// </summary>
        public static void LogSystemInfo(int? bucketNumber = null)
        {
            EnsurePerformanceLevelFound();

            var systemInfo = new Dictionary<string, object>
        {
            { "graphicsDeviceName", SystemInfo.graphicsDeviceName },
            { "graphicsDeviceID", SystemInfo.graphicsDeviceID.ToString() },
            { "graphicsDeviceType", SystemInfo.graphicsDeviceType.ToString() },
            { "graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor },
            { "graphicsDeviceVendorID", SystemInfo.graphicsDeviceVendorID.ToString() },
            { "graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion.ToString() },
            { "graphicsMemorySize", SystemInfo.graphicsMemorySize.ToString() },
            { "graphicsMultiThreaded", SystemInfo.graphicsMultiThreaded.ToString() },
            { "graphicsShaderLevel", SystemInfo.graphicsShaderLevel.ToString() },
            { "graphicsUVStartsAtTop", SystemInfo.graphicsUVStartsAtTop.ToString() },
            { "processorCount", SystemInfo.processorCount.ToString() },
            { "processorFrequency", SystemInfo.processorFrequency.ToString() },
            { "processorType", SystemInfo.processorType.ToString() },
            { "batteryStatus", SystemInfo.batteryStatus.ToString() },
            { "batteryLevel", SystemInfo.batteryLevel.ToString() },
            { "copyTextureSupport", SystemInfo.copyTextureSupport.ToString() },
            { "deviceModel", SystemInfo.deviceModel.ToString() },
            { "deviceName", SystemInfo.deviceName.ToString() },
            { "deviceType", SystemInfo.deviceType.ToString() },
            { "deviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier.ToString() },
            { "supportsRawShadowDepthSampling", SystemInfo.supportsRawShadowDepthSampling.ToString() },
            { "supportsShadows", SystemInfo.supportsShadows.ToString() },
            { "GPUVendorName", GPUVendorName },
            { "GPUTypeName", GPUTypeName },
            { "PerformanceLevelString", PerformanceLevelString },
            { "BucketNumber", bucketNumber.HasValue ? bucketNumber.Value.ToString() : "none specified" }
        };

            Analytics.CustomEvent("SystemInfo", systemInfo);

            string outString = " ======================= \r\n    SystemInfo:  \r\n";
            foreach (var key in systemInfo.Keys)
            {
                if (key != null && systemInfo[key] != null && systemInfo[key].ToString() != null)
                {
                    outString += key + " : " + systemInfo[key].ToString() + "\r\n";
                }
            }
            outString += " ======================= \r\n";

            Debug.Log(outString);
        }

        public enum GPUVendor
        {
            Intel,
            NVIDIA,
            AMD,
            unknown
        }

        public enum SystemPerformanceLevel
        {
            Mainstream,
            LowEndUltra,
            Ultra,
            HighEndUltra,
            Undefined
        }
    }
}
