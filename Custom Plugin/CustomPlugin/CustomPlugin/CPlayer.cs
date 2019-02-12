using System.IO;

namespace CustomPlugin
{
    [System.Serializable]
    public class CPlayer
    {
        public string ReturnMessage { get; private set; }
        public int AccountID { get; private set; }
        public string PlayerName { get; private set; }
        public float TimeOnline { get; }
        public CVector3 PlayerPosition { get; private set; }
        public CVector3 PetPosition { get; private set; }
        public CSound SoundSetting { get; private set; }

        public CPlayer(string returnMessage, int accountID, string playerName, float timeOnline, CVector3 playerPosition, CVector3 petPosition, CSound soundSetting)
        {
            ReturnMessage = returnMessage;
            AccountID = accountID;
            PlayerName = playerName;
            TimeOnline = timeOnline;
            PlayerPosition = playerPosition;
            PetPosition = petPosition;
            SoundSetting = soundSetting;
        }

        public static byte[] Serialize(object o)
        {
            CPlayer player = (CPlayer)o;
            if (player == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(player.ReturnMessage);
                    bw.Write(player.AccountID);
                    bw.Write(player.PlayerName);
                    bw.Write(player.TimeOnline);

                    // get the number of byte array written into the stream
                    byte[] bytes = CVector3.Serialize(player.PlayerPosition);
                    bw.Write(bytes.LongLength);
                    bw.Write(bytes);

                    bytes = CVector3.Serialize(player.PetPosition);
                    bw.Write(bytes.LongLength);
                    bw.Write(bytes);

                    bytes = CSound.Serialize(player.SoundSetting);
                    bw.Write(bytes.LongLength);
                    bw.Write(bytes);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            float timeOnline;
            string message, playerName;
            int accountID;
            CVector3 playerPos, petPos;
            CSound sound;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    message = br.ReadString();
                    accountID = br.ReadInt32();
                    playerName = br.ReadString();
                    timeOnline = br.ReadSingle();

                    long size = br.ReadInt64();
                    playerPos = CVector3.Deserialize(br.ReadBytes((int)size)) as CVector3;

                    size = br.ReadInt64();
                    petPos = CVector3.Deserialize(br.ReadBytes((int)size)) as CVector3;

                    size = br.ReadInt64();
                    sound = CSound.Deserialize(br.ReadBytes((int)size)) as CSound;
                }
            }
            return new CPlayer(message, accountID, playerName, timeOnline, playerPos, petPos, sound);
        }
    }
}
