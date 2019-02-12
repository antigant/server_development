using System.IO;

namespace CustomPlugin
{
    [System.Serializable]
    public class CLogin
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public CLogin(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public static byte[] Serialize(object o)
        {
            CLogin detail = (CLogin)o;
            if (detail == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(detail.Username);
                    bw.Write(detail.Password);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            string username, password;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    username = br.ReadString();
                    password = br.ReadString();
                }
            }
            return new CLogin(username, password);
        }
    }
}
