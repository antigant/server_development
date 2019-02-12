using UnityEngine;
using System.IO;
using ExitGames.Client.Photon;

namespace CustomPlugin
{
    [System.Serializable]
    public class Test
    {
        //public static readonly byte[] memItemInfo = new byte[General.INTEGER_INT * 2];

        public string ItemID { get; private set; }
        public string Name { get; private set; }
        public int[] TestArray { get; private set; }

        // game object of the item (looks etc), can attach relevant scripts to it too
        public GameObject Obj { get; set; }

        public Test(string id,string name, int[] testArray)
        {
            ItemID = id;
            Name = name;
            TestArray = testArray;
        }

        // Single
        public static byte[] Serialize(object o)
        {
            Test test = (Test)o;
            if (test == null) return null;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(test.ItemID);
                    bw.Write(test.Name);

                    bw.Write(test.TestArray.LongLength);
                    foreach (var i in test.TestArray)
                        bw.Write(i);

                    return ms.ToArray();
                }
            }
        }

        public static object Deserialize(byte[] b)
        {
            string id = ""; string name = ""; int[] testArray;
            using (var ms = new MemoryStream(b))
            {
                using (var br = new BinaryReader(ms))
                {
                    id = br.ReadString();
                    name = br.ReadString();

                    var length = br.ReadInt64();
                    testArray = new int[length];
                    for (long i = 0; i < length; ++i)
                        testArray[i] = br.ReadInt32();
                }
            }
            
            return new Test(id, name, testArray);
        }

        //// array of custom test
        //public static void SerializeA(object[] o)
        //{
        //    Test[] test = (Test[])o;
        //    if (test == null) return/* null*/;

        //    using (var ms = new MemoryStream())
        //    {
        //        using (var bw = new BinaryWriter(ms))
        //        {
        //            bw.Write(test.LongLength);
        //            foreach(Test testItem in test)
        //            {
        //                bw.Write(testItem.ItemID);
        //                bw.Write(testItem.Name);
        //            }

        //            //return ms.ToArray();
        //        }
        //    }
        //}

        //public static object[] DeserializeA(byte[] b)
        //{
        //    string id = ""; string name = "";
        //    using (var ms = new MemoryStream(b))
        //    {
        //        using (var br = new BinaryReader(ms))
        //        {
        //            var record = br.ReadInt64();
        //            Test[] test = new Test[record];

        //            for(long i = 0; i < record; ++i)
        //            {
        //                id = br.ReadString();
        //                name = br.ReadString();

        //                var temp = new Test(id, name);
        //                test[i] = temp;
        //            }

        //            return test;
        //        }
        //    }
        //}
    }
}
