using UnityEngine;
using System.IO;
//using ExitGames.Client.Photon;

namespace CustomPlugin
{
    [System.Serializable]
    public class Item
    {
        //public static readonly byte[] memItemInfo = new byte[General.INTEGER_INT * 2];

        public int ItemID { get; private set; }
        public string Name { get; private set; }

        // game object of the item (looks etc), can attach relevant scripts to it too
        public GameObject Obj { get; set; }

        public Item(int id, string name)
        {
            ItemID = id;
            Name = name;
        }

        public static byte[] Serialize(object o)
        {
            Item item = (Item)o;
            if (item == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(item.ItemID);
                    bw.Write(item.Name);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            int id = 0; string name = "";
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    id = br.ReadInt32();
                    name = br.ReadString();
                }
            }
            return new Item(id, name);
        }

        //public static void RegisterServer()
        //{
        //    Protocol.TryRegisterType(typeof(Item), (byte)'A', SerializeItem, DeserializeItem);
        //}

        //public static void RegisterClient()
        //{
        //    PhotonPeer.RegisterType(typeof(Item), (byte)'A', SerializeItem, DeserializeItem);
        //}

        //public static short Serialize(StreamBuffer outStream, object customObject)
        //{
        //    Item item = (Item)customObject;

        //    lock(memItemInfo)
        //    {
        //        int index = 0; // byte stream starting index
        //        byte[] bytes = memItemInfo;

        //        // Serialize each value in item class
        //        Protocol.Serialize(item.ItemID, bytes, ref index);
        //        Protocol.Serialize(item.ItemNameID, bytes, ref index);

        //        outStream.Write(bytes, 0, memItemInfo.Length);
        //    }

        //    return (short)memItemInfo.Length;
        //}

        //public static object Deserialize(StreamBuffer inStream, short length)
        //{
        //    // Temp holders for each member in Item
        //    int itemID = 0;
        //    int itemNameID = 0;

        //    lock(memItemInfo)
        //    {
        //        int index = 0; // byte stream strating index

        //        // Deserialize each value in the same order when serializing
        //        Protocol.Deserialize(out itemID, memItemInfo, ref index);
        //        Protocol.Deserialize(out itemNameID, memItemInfo, ref index);
        //    }

        //    // Return new instance of Item with the relevant data
        //    return new Item(itemID, itemNameID);
        //}
    }
}
