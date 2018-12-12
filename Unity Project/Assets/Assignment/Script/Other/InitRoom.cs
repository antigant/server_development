using UnityEngine;

public class InitRoom : MonoBehaviour
{
    float dt;

    // Use this for initialization
    void Awake()
    {
        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)
        }
    }

    void OnGUI()
    {
        if (GUILayout.Button("Quit Game"))
        {
            QuitGame();
        }

        if (!PhotonNetwork.connected)
        {
            ShowConnectingGUI();
            return;   //Wait for a connection
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.connected)
            return;

        // wait 2 seconds before joining or creating room
        dt += Time.deltaTime;
        if (dt < 1.25f)
            return;

        LoginPage();
    }

    void LoginPage()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 0;

        int IdNum = Random.Range(1, 9999);
        string userId = "Player" + IdNum.ToString();
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = userId;

        // Join or create room to be able to communicate with the server plugin, not sure what will happen if i remove the booleanforchecking  if player is in room.
        PhotonNetwork.JoinOrCreateRoom("InitServer", options, TypedLobby.Default);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }

    void ShowConnectingGUI()
    {
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Connecting to Photon server.");
        GUILayout.Label("Hint: This demo uses a settings file and logs the server address to the console.");

        GUILayout.EndArea();
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
