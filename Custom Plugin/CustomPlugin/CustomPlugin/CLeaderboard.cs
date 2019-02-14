using System.IO;

// will only send over to plugin when achievement is done
namespace CustomPlugin
{
    [System.Serializable]
    public class CLeaderboard
    {
        public string[] TopPlayers { get; }

        public CLeaderboard(string[] topPlayers)
        {
            TopPlayers = topPlayers;
        }

        public static byte[] Serialize(object o)
        {
            CLeaderboard ldrb = (CLeaderboard)o;
            if (ldrb == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(ldrb.TopPlayers.LongLength);
                    foreach (var i in ldrb.TopPlayers)
                        bw.Write(i);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            string[] topPlayers;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    var length = br.ReadInt64();
                    topPlayers = new string[length];
                    for (long i = 0; i < length; ++i)
                        topPlayers[i] = br.ReadString();
                }
            }
            return new CLeaderboard(topPlayers);
        }
    }
}
