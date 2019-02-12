using System.IO;

namespace CustomPlugin
{
    [System.Serializable]
    public class CSound
    {
        public float Master { get; set; }
        public float Bgm { get; set; }
        public float Sfx { get; set; }

        public CSound(float master = 0.0f, float bgm = 0.0f, float sfx = 0.0f)
        {
            Master = master;
            Bgm = bgm;
            Sfx = sfx;
        }

        public static byte[] Serialize(object o)
        {
            CSound sound = (CSound)o;
            if (sound == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(sound.Master);
                    bw.Write(sound.Bgm);
                    bw.Write(sound.Sfx);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            float master, bgm, sfx;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    master = br.ReadSingle();
                    bgm = br.ReadSingle();
                    sfx = br.ReadSingle();
                }
            }
            return new CSound(master, bgm, sfx);
        }
    }
}
