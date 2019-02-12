using UnityEngine;
using CustomPlugin;

public class ReceiveArray : Photon.PunBehaviour
{
    //int[] testArray = { 0, 0 };
    Test testItem;

    void Awake()
    {
        int[] testArray = new int[4]
            { 100,2123,1235,1235432 };
        testItem = new Test("hello", "hello", testArray);

        //if (!PhotonNetwork.connected)
        //{
        //    PhotonNetwork.ConnectUsingSettings("v1.0");

        //    RoomOptions options = new RoomOptions();
        //    options.MaxPlayers = 0;
        //    PhotonNetwork.JoinOrCreateRoom("InitServer", options, TypedLobby.Default);
        //}

        PhotonNetwork.OnEventCall += TestReceive;
    }

    private void Update()
    {
        int i = 0;
        if (i == 0)
            return;
    }

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            ShowConnectingGUI();
            return;   //Wait for a connection
        }

        if (PhotonNetwork.room == null) return; //Only display this GUI when inside a room

        //if (GUILayout.Button("Test receive array"))
        //{
        //    Test();
        //}
        if (GUILayout.Button("Test custom class"))
        {
            TestFunction();
        }

        //GUILayout.Label(string.Format("Element in array {0}, {1}", testArray[0], testArray[1]));
        GUILayout.Label(string.Format("First Item ID: {0}, Item Name: {1}, testArray: {2}", testItem.ItemID, testItem.Name, testItem.TestArray[0]));
    }

    public void TestFunction()
    {
        byte evCode = (byte)EvCode.PHOTON_TEST;
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, Test.Serialize(testItem), reliable, null);

        Debug.Log("Still works");
    }

    void TestReceive(byte eventCode, object content, int senderID)
    {
        if (eventCode == (byte)EvCode.PHOTON_TEST)
        {
            //testItem = Test.Deserialize((byte[])content) as Test;
            CPlayer player = CPlayer.Deserialize((byte[])content) as CPlayer;

            Debug.Log(player.AccountID);
        }
    }

    void ShowConnectingGUI()
    {
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Connecting to Photon server.");
        GUILayout.Label("Hint: This demo uses a settings file and logs the server address to the console.");

        GUILayout.EndArea();
    }
}
