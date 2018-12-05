using UnityEngine;
using ExitGames.Client.Photon;

namespace CustomPlugin
{
    [System.Serializable]
    public class Item
    {
        public static readonly byte[] memItemInfo = new byte[General.INTEGER_INT * 2];

        // fields that will be serialized
        [SerializeField] protected int itemID; // id of the item the player have in the db (this id will be used to search for the item  with the stats and all
        [SerializeField] protected int itemNameID; // id of the item name

        // game object of the item (looks etc), can attach relevant scripts to it too
        public GameObject itemObj { get; set; }

        public int ItemID { get { return itemID; } }
        public int ItemNameID { get { return itemNameID; } }

        public Item(int id, int nameID)
        {
            itemID = id;
            itemNameID = nameID;
        }


        public static short SerializeItem(StreamBuffer outStream, object customObject)
        {
            Item item = (Item)customObject;

            lock(memItemInfo)
            {
                int index = 0; // byte stream starting index
                byte[] bytes = memItemInfo;

                // Serialize each value in item class
                Protocol.Serialize(item.ItemID, bytes, ref index);
                Protocol.Serialize(item.ItemNameID, bytes, ref index);

                outStream.Write(bytes, 0, memItemInfo.Length);
            }

            return (short)memItemInfo.Length;
        }
    }
}
