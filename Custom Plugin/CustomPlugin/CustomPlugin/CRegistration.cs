using System.IO;

namespace CustomPlugin
{
    [System.Serializable]
    public class CRegistration
    {
        public string Username { get; }
        public string PlayerName { get; }
        public HashWithSaltResult Hash { get; }

        /// <summary>
        /// Use this for registration of new account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="playerName"></param>
        /// <param name="hash"></param>
        public CRegistration(string username, string playerName, HashWithSaltResult hash)
        {
            Username = username;
            PlayerName = playerName;
            Hash = hash;
        }

        /// <summary>
        /// Use this for resetting of password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="hash"></param>
        public CRegistration(string username, HashWithSaltResult hash)
        {
            Username = username;
            Hash = hash;
        }

        public static byte[] Serialize(object o)
        {
            CRegistration detail = (CRegistration)o;
            if (detail == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(detail.Username);
                    bw.Write(detail.PlayerName);

                    byte[] bytes = HashWithSaltResult.Serialize(detail.Hash);
                    bw.Write(bytes.LongLength);
                    bw.Write(bytes);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            string username, playerName;
            HashWithSaltResult hash = null;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    username = br.ReadString();
                    playerName = br.ReadString();

                    long size = br.ReadInt64();
                    hash = HashWithSaltResult.Deserialize(br.ReadBytes((int)size)) as HashWithSaltResult;
                }
            }
            return new CRegistration(username, playerName, hash);
        }
    }
}
