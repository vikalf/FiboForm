using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FiboForm.Common
{
    public static class Helpers
    {
        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return default;
            BinaryFormatter bf = new BinaryFormatter();
            using MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using MemoryStream ms = new MemoryStream(data);
            object obj = bf.Deserialize(ms);
            return (T)obj;
        }

    }
}
