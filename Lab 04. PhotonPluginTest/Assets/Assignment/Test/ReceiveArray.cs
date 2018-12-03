using UnityEngine;

public class ReceiveArray : Photon.PunBehaviour
{
    int[] testArray = { 0, 0 };

    void Awake()
    {
        //if (!PhotonNetwork.connected)
        //    PhotonNetwork.ConnectUsingSettings("v1.0");

        PhotonNetwork.OnEventCall += TestReceive;
    }

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            ShowConnectingGUI();
            return;   //Wait for a connection
        }

        //if (PhotonNetwork.room == null) return; //Only display this GUI when inside a room

        if (GUILayout.Button("Test receive array"))
        {
            Test();
        }

        GUILayout.Label(string.Format("Element in array {0}, {1}", testArray[0], testArray[1]));
    }

    public void Test()
    {
        byte evCode = (byte)EvCode.PHOTON_TEST;
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, null, reliable, null);
    }

    void TestReceive(byte eventCode, object content, int senderID)
    {
        testArray = (int[])content;
    }

    void ShowConnectingGUI()
    {
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Connecting to Photon server.");
        GUILayout.Label("Hint: This demo uses a settings file and logs the server address to the console.");

        GUILayout.EndArea();
    }
}
