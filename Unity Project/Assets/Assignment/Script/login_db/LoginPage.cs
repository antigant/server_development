﻿using UnityEngine;
using CustomPlugin;

public class LoginPage : Photon.PunBehaviour
{
    string username;
    string password;
    // display the message from server
    string displayMessage;

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

        PhotonNetwork.OnEventCall += LoginReceive;

        textStyle = new GUIStyle();
    }

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            ShowConnectingGUI();
            return;   //Wait for a connection
        }

        //if (PhotonNetwork.room == null)
        //    return; //Only when we're not in a Room

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
        GUILayout.Label(displayMessage, textStyle);

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

        GUILayout.EndArea();
    }

    // send message to server
    void Login()
    {
        byte evCode = (byte)EvCode.LOGIN;
        string[] content = { username, password };
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
        //Debug.Log("username: " + username);
        //Debug.Log("password: " + password);
    }

    // Use this to receive message from server
    void LoginReceive(byte eventCode, object content, int senderID)
    {
        string message = "";
        if ((eventCode == (byte)EvCode.LOGIN) && (senderID <= 0))
            message = (string)content;
        // Successful
        if(message[0] == 'S')
        {
            // set up relevant data for the player
            int accountID = System.Convert.ToInt32(General.GetStringDataFromMessage(message, "AccountID"));
            string playerName = General.GetStringDataFromMessage(message, "PlayerName");
            // Init the player
            Player.GetInstance(accountID).SetPlayerName(playerName);

            // go over to viking scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("VikingScene");
            PhotonNetwork.LeaveRoom();
        }
        // Unsuccesful
        else if(message[0] == 'U')
        {
            // inform the player that the username/password is incorrect
            displayMessage = General.GetStringDataFromMessage(message, "Message");
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
