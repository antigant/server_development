using System.IO;

// will only send over to plugin when achievement is done
namespace CustomPlugin
{
    [System.Serializable]
    public class CAchievements
    {
        public int AccountID { get; }
        public int AchievementID { get; }

        public CAchievements(int accountID, int achievementID)
        {
            AccountID = accountID;
            AchievementID = achievementID;
        }

        public static byte[] Serialize(object o)
        {
            CAchievements achievement = (CAchievements)o;
            if (achievement == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(achievement.AccountID);
                    bw.Write(achievement.AchievementID);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            int accountID, achievementID;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    accountID = br.ReadInt32();
                    achievementID = br.ReadInt32();
                }
            }
            return new CAchievements(accountID, achievementID);
        }
    }
}
