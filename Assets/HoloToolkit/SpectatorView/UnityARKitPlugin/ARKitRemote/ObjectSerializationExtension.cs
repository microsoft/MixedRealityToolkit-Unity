using UnityEngine;
using System.Collections;
#if PLATFORM_IOS
using System.Runtime.Serialization.Formatters.Binary;
#endif
using System.IO;

namespace ARKit.Utils
{
    //Extension class to provide serialize / deserialize methods to object.
    //src: http://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
    //NOTE: You need add [Serializable] attribute in your class to enable serialization
    public static class ObjectSerializationExtension
    {

        public static byte[] SerializeToByteArray(this object obj)
        {
            #if PLATFORM_IOS
            if (obj == null)
            {
                return null;
            }
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
            #else
            return new byte[0];
            #endif
        }

        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            #if PLATFORM_IOS
            if (byteArray == null)
            {
                return null;
            }
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
            #else
            return null;
            #endif
        }
    }
}