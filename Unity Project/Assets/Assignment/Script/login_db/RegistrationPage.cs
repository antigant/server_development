using UnityEngine;

public class RegistrationPage : Photon.PunBehaviour
{
    string username;
    string password;
    string confirmPassword;
    // display the message from server
    string displayMessage;

    GUIStyle textStyle;

    void Awake()
    {
        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;

        PhotonNetwork.OnEventCall += RegistrationReceive;
        textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.black;
    }

    void OnGUI()
    {

        GUILayout.BeginArea(new Rect((Screen.width - 400) * 0.5f, (Screen.height - 300) * 0.5f, 275, 300));

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

        // Password
        GUILayout.BeginHorizontal();
        GUILayout.Label("Confirm password:", GUILayout.Width(90));
        if (confirmPassword == null)
        {
            confirmPassword = GUILayout.TextField("confirm password");
            confirmPassword = "";
        }
        else
            confirmPassword = GUILayout.PasswordField(confirmPassword, '*', 15);
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.Label("Player Info", textStyle);


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Register"))
        {
            Registration();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    // send message to server
    void Registration()
    {
        byte evCode = (byte)EvCode.REGISTRATION;
        string[] content = { username, password };
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    void RegistrationReceive(byte eventCode, object content, int senderID)
    {
        string message = "";
        if ((eventCode == (byte)EvCode.REGISTRATION) && (senderID <= 0))
            message = (string)content;
    }
}
