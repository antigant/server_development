using System.IO;

namespace CustomPlugin
{
    [System.Serializable]
    public class HashWithSaltResult
    {
        public string Salt { get; }
        public string Digest { get; }

        public HashWithSaltResult(string salt, string digest)
        {
            Salt = salt;
            Digest = digest;
        }

        public static byte[] Serialize(object o)
        {
            HashWithSaltResult hash = (HashWithSaltResult)o;
            if (hash == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(hash.Salt);
                    bw.Write(hash.Digest);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            string salt, digest;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    salt = br.ReadString();
                    digest = br.ReadString();
                }
            }
            return new HashWithSaltResult(salt, digest);
        }
    }
}
