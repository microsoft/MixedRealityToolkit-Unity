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
        public bool ShouldPersistCoordinate { get; set; }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(MarkerID);
            writer.Write(MarkerSize);
            writer.Write(ShouldPersistCoordinate);
        }

        public static bool TryDeserialize(BinaryReader reader, out MarkerDetectorLocalizationSettings settings)
        {
            try
            {
                settings = new MarkerDetectorLocalizationSettings
                {
                    MarkerID = reader.ReadInt32(),
                    MarkerSize = reader.ReadSingle(),
                    ShouldPersistCoordinate = reader.ReadBoolean()
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