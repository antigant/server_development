﻿using UnityEngine;
using CustomPlugin;

public class LoginPage : Photon.PunBehaviour
{
    string username = "";
    string password = "";
    // display the message from server

    GUIStyle textStyle;

    void Awake()
    {
        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("v1.0");
            PhotonNetwork.JoinLobby();
        }

        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;

        textStyle = new GUIStyle();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            Login();
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

        GUILayout.BeginArea(new Rect((Screen.width - 400) * 0.5f, (Screen.height - 300) * 0.5f, 275, 300));

        textStyle.normal.textColor = Color.black;
        GUILayout.Label("Login Page", textStyle);

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
        textStyle.normal.textColor = Color.red;
        GUILayout.Label(General.Message, textStyle);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Login"))
        {
            Login();
        }
        if (GUILayout.Button("Register"))
        {
            Registration();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Password"))
        {
            ResetPassword();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    // send message to server
    void Login()
    {
        byte evCode = (byte)EvCode.LOGIN;
        CLogin detail = new CLogin(username.ToLower(), password);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, CLogin.Serialize(detail), reliable, null);
    }

    // Use this to receive message from server
    public static void LoginReceive(byte eventCode, object content, int senderID)
    {
        if (eventCode == (byte)EvCode.LOGIN)
        {
            var player = CPlayer.Deserialize((byte[])content) as CPlayer;
            // Successful
            if (player.ReturnMessage[0] == 'S')
            {
                // set up relevant data for the player
                int accountID = player.AccountID;
                string playerName = player.PlayerName;
                // Init the player
                Player.GetInstance(accountID, playerName, force: true);

                // player position
                float pos_x = player.PlayerPosition.x;
                float pos_y = player.PlayerPosition.y;
                float pos_z = player.PlayerPosition.z;
                Vector3 pos = new Vector3(pos_x, pos_y, pos_z);
                Player.GetInstance().SetPosition(pos);

                // pet position
                pos_x = player.PetPosition.x;
                pos_y = player.PetPosition.y;
                pos_z = player.PetPosition.z;
                pos = new Vector3(pos_x, pos_y, pos_z);
                Player.GetInstance().SetPetPosition(pos);

                // audio volume
                float[] volume = { player.SoundSetting.Master, player.SoundSetting.Bgm, player.SoundSetting.Sfx };
                AudioManager.instance.SetVolume(volume);

                // go over to viking scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("VikingScene");
                General.Message = "";
                PhotonNetwork.LeaveRoom();
            }
            // Unsuccesful
            else if (player.ReturnMessage[0] == 'U')
            {
                // inform the player that the username/password is incorrect
                General.Message = General.GetStringDataFromMessage(player.ReturnMessage, "Message");
            }
        }
    }

    // brings the player to the registration screen
    void Registration()
    {
        General.Message = "";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Registration");
    }

    void ResetPassword()
    {
        General.Message = "";
        UnityEngine.SceneManagement.SceneManager.LoadScene("ResetPassword");
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
