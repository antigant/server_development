using UnityEngine;
using System.Collections;

public class MainMenuVik : MonoBehaviour
{
    float dt;
    public GameObject canvas;

    void Awake()
    {
        //PhotonNetwork.logLevel = NetworkLogLevel.Full;

        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)
        }

        //Load name from PlayerPrefs
        PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest" + Random.Range(1, 9999));

        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;
    }

    void Update()
    {
        dt += Time.deltaTime;
        if (dt < 1.75f)
            return;

        string playerName = Player.GetInstance().GetPlayerName();

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 0;
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = playerName;
        PhotonNetwork.playerName = playerName;

        PhotonNetwork.JoinOrCreateRoom("GameRoom", options, TypedLobby.Default);
        gameObject.SetActive(false);
        canvas.SetActive(true);

        // play this audio
        AudioManager.instance.PlayBGM(1, 0);
    }

    //private string roomName = "myRoom";
    //private Vector2 scrollPos = Vector2.zero;
    //string playerPassword;

    //void OnGUI()
    //{
    //    if (!PhotonNetwork.connected)
    //    {
    //        ShowConnectingGUI();
    //        return;   //Wait for a connection
    //    }


    //    if (PhotonNetwork.room != null)
    //        return; //Only when we're not in a Room

    //    if (GUILayout.Button("Logout"))
    //    {
    //        //PhotonNetwork.LeaveRoom();
    //        // save player and pet position

    //    }

    //    GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

    //    GUILayout.Label("Main Menu");

    //    ////Player name
    //    //GUILayout.BeginHorizontal();
    //    //GUILayout.Label("Player name:", GUILayout.Width(150));
    //    //PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
    //    //if (GUI.changed)//Save name
    //    //    PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
    //    //GUILayout.EndHorizontal();

    //    //// Password
    //    //GUILayout.BeginHorizontal();
    //    //GUILayout.Label("Password:", GUILayout.Width(150));

    //    //if (playerPassword == null)
    //    //{
    //    //    playerPassword = GUILayout.TextField("password");
    //    //    playerPassword = "";
    //    //}
    //    //else
    //    //    playerPassword = GUILayout.PasswordField(playerPassword, '*', 15);

    //    //if (GUI.changed)//Save password
    //    //    PlayerPrefs.SetString("playerPassword", playerPassword);
    //    //GUILayout.EndHorizontal();

    //    //GUILayout.Space(15);

    //    //Join room by title
    //    GUILayout.BeginHorizontal();
    //    GUILayout.Label("JOIN ROOM:", GUILayout.Width(150));
    //    roomName = GUILayout.TextField(roomName);
    //    if (GUILayout.Button("GO"))
    //    {
    //        PhotonNetwork.JoinRoom(roomName);
    //    }
    //    GUILayout.EndHorizontal();

    //    //Create a room (fails if exist!)
    //    GUILayout.BeginHorizontal();
    //    GUILayout.Label("CREATE ROOM:", GUILayout.Width(150));
    //    roomName = GUILayout.TextField(roomName);
    //    if (GUILayout.Button("GO"))
    //    {
    //        // using null as TypedLobby parameter will also use the default lobby
    //        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 10 }, TypedLobby.Default);
    //    }
    //    GUILayout.EndHorizontal();

    //    //Join random room
    //    GUILayout.BeginHorizontal();
    //    GUILayout.Label("JOIN RANDOM ROOM:", GUILayout.Width(150));
    //    if (PhotonNetwork.GetRoomList().Length == 0)
    //    {
    //        GUILayout.Label("..no games available...");
    //    }
    //    else
    //    {
    //        if (GUILayout.Button("GO"))
    //        {
    //            PhotonNetwork.JoinRandomRoom();
    //        }
    //    }
    //    GUILayout.EndHorizontal();

    //    GUILayout.Space(30);
    //    GUILayout.Label("ROOM LISTING:");
    //    if (PhotonNetwork.GetRoomList().Length == 0)
    //    {
    //        GUILayout.Label("..no games available..");
    //    }
    //    else
    //    {
    //        //Room listing: simply call GetRoomList: no need to fetch/poll whatever!
    //        scrollPos = GUILayout.BeginScrollView(scrollPos);
    //        foreach (RoomInfo game in PhotonNetwork.GetRoomList())
    //        {
    //            GUILayout.BeginHorizontal();
    //            GUILayout.Label(game.Name + " " + game.PlayerCount + "/" + game.MaxPlayers);
    //            if (GUILayout.Button("JOIN"))
    //            {
    //                PhotonNetwork.JoinRoom(game.Name);
    //            }
    //            GUILayout.EndHorizontal();
    //        }
    //        GUILayout.EndScrollView();
    //    }

    //    GUILayout.EndArea();
    //}

    void ShowConnectingGUI()
    {
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Connecting to Photon server.");
        GUILayout.Label("Hint: This demo uses a settings file and logs the server address to the console.");

        GUILayout.EndArea();
    }

    public void OnConnectedToMaster()
    {
        // this method gets called by PUN, if "Auto Join Lobby" is off.
        // this demo needs to join the lobby, to show available rooms!

        PhotonNetwork.JoinLobby();  // this joins the "default" lobby
    }
}
