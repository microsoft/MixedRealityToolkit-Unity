using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class MarkerDetectorLocalizationSettings : ISpatialLocalizationSettings
    {
        public int MarkerID { get; set; }
        public float MarkerSize { get; set; }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(MarkerID);
            writer.Write(MarkerSize);
        }

        public static bool TryDeserialize(BinaryReader reader, out MarkerDetectorLocalizationSettings settings)
        {
            try
            {
                settings = new MarkerDetectorLocalizationSettings
                {
                    MarkerID = reader.ReadInt32(),
                    MarkerSize = reader.ReadSingle(),
                };
                return true;
            }
            catch
            {
                settings = null;
                return false;
            }
        }
    }
}