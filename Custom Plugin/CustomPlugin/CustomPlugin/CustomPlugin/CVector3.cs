using System.IO;

// using this just to store values
namespace CustomPlugin
{
    [System.Serializable]
    public class CVector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public CVector3(float x = 0.0f, float y = 0.0f, float z = 0.0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static byte[] Serialize(object o)
        {
            CVector3 vec = (CVector3)o;
            if (vec == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(vec.x);
                    bw.Write(vec.y);
                    bw.Write(vec.z);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            float x, y, z;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    x = br.ReadSingle();
                    y = br.ReadSingle();
                    z = br.ReadSingle();
                }
            }
            return new CVector3(x, y, z);
        }
    }
}