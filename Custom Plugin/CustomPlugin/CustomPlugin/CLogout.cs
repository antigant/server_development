using System.IO;
using UnityEngine;

namespace CustomPlugin
{
    [System.Serializable]
    public class CLogout
    {
        public int AccountID { get; }
        public CVector3 PlayerPosition { get; }
        public CVector3 PetPosition { get; }
        public CSound Sound { get; }

        public CLogout(int accountID, CVector3 playerPosition, CVector3 petPosition, CSound sound)
        {
            AccountID = accountID;
            PlayerPosition = playerPosition;
            PetPosition = petPosition;
            Sound = sound;
        }

        public CLogout(int accountID, Vector3 playerPosition, Vector3 petPosition, CSound sound)
        {
            AccountID = accountID;

            PlayerPosition.x = playerPosition.x;
            PlayerPosition.y = playerPosition.y;
            PlayerPosition.z = playerPosition.z;

            PetPosition.x = petPosition.x;
            PetPosition.y = petPosition.y;
            PetPosition.z = petPosition.z;

            Sound = sound;
        }

        public static byte[] Serialize(object o)
        {
            CLogout logout = (CLogout)o;
            if (logout == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(logout.AccountID);

                    byte[] bytes = CVector3.Serialize(logout.PlayerPosition);
                    bw.Write(bytes.LongLength);
                    bw.Write(bytes);

                    bytes = CVector3.Serialize(logout.PetPosition);
                    bw.Write(bytes.LongLength);
                    bw.Write(bytes);

                    bytes = CSound.Serialize(logout.Sound);
                    bw.Write(bytes.LongLength);
                    bw.Write(bytes);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            int accountID;
            CVector3 player, pet;
            CSound sound;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    accountID = br.ReadInt32();

                    long size = br.ReadInt64();
                    player = CVector3.Deserialize(br.ReadBytes((int)size)) as CVector3;

                    size = br.ReadInt64();
                    pet = CVector3.Deserialize(br.ReadBytes((int)size)) as CVector3;

                    size = br.ReadInt64();
                    sound = CSound.Deserialize(br.ReadBytes((int)size)) as CSound;
                }
            }
            return new CLogout(accountID, player, pet, sound);
        }
    }
}
