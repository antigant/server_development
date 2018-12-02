using UnityEngine;

public class LoginPage : Photon.PunBehaviour
{
    string username;
    string password;
    // message from server
    string message;

    void Awake()
    {
        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)

        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;

        PhotonNetwork.OnEventCall += LoginReceive;
    }

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            ShowConnectingGUI();
            return;   //Wait for a connection
        }

        if (PhotonNetwork.room != null)
            return; //Only when we're not in a Room

        GUILayout.BeginArea(new Rect((Screen.width - 400) * 0.5f, (Screen.height - 300) * 0.5f, 275, 300));

        GUILayout.Label("Login Page");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Username:", GUILayout.Width(90));
        username = GUILayout.TextField(username);
        GUILayout.EndHorizontal();

        // Password
        GUILayout.BeginHorizontal();
        GUILayout.Label("Password:", GUILayout.Width(90));
        if (password == null)
        {
            password = GUILayout.TextField("password");
            password = "";
        }
        else
            password = GUILayout.PasswordField(password, '*', 15);
        GUILayout.EndHorizontal();

        GUILayout.Space(15);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Login"))
        {
            Login();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    // send message to server
    void Login()
    {
        byte evCode = (byte)EvCode.LOGIN;
        string contentMessage = "Username=" + username + ", Password=" + password;
        byte[] content = System.Text.Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        //PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    // Use this to receive message from server
    void LoginReceive(byte eventCode, object content, int senderID)
    {
        // only receiving message from server if login password/username is incorrect/don't exist in db
        if ((eventCode == (byte)EvCode.LOGIN) && (senderID <= 0))
            message = (string)content;
        // Successful
        if(message[0] == 'S')
        {
            // set up relevant data for the player
        }
        // Unsuccesful
        else if(message[0] == 'U')
        {
            // inform the player that the username/password is incorrect
        }
    }

    // brings the player to the registration screen
    void Registration()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Registration");
    }

    void ShowConnectingGUI()
    {
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Connecting to Photon server.");
        GUILayout.Label("Hint: This demo uses a settings file and logs the server address to the console.");

        GUILayout.EndArea();
    }
}
