using System.IO;

namespace CustomPlugin
{
    [System.Serializable]
    public class CUpdateItem
    {
        public string Message { get; }
        public int AccountID { get; }
        public int ItemID { get; }

        public CUpdateItem(string message, int accountID, int itemID)
        {
            Message = message;
            AccountID = accountID;
            ItemID = itemID;
        }

        public static byte[] Serialize(object o)
        {
            CUpdateItem item = (CUpdateItem)o;
            if (item == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(item.Message);
                    bw.Write(item.AccountID);
                    bw.Write(item.ItemID);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            string message;
            int accountID, itemID;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    message = br.ReadString();
                    accountID = br.ReadInt32();
                    itemID = br.ReadInt32();
                }
            }
            return new CUpdateItem(message, accountID, itemID);
        }
    }
}
