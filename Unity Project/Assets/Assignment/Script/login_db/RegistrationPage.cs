using UnityEngine;
using CustomPlugin;

public class RegistrationPage : Photon.PunBehaviour
{
    string username = "";
    string password = "";
    string confirmPassword = "";
    string playerName = "";
    // display the message from server

    GUIStyle textStyle;

    static bool registerComplete;
    float dt = 0.0f;

    void Awake()
    {
        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;

        textStyle = new GUIStyle();
        registerComplete = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            Registration();

        if (!registerComplete)
            return;

        dt += Time.deltaTime;
        if (dt < 2.0f)
            return;

        LoginPage();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Back"))
        {
            LoginPage();
        }

        GUILayout.BeginArea(new Rect((Screen.width - 400) * 0.5f, (Screen.height - 300) * 0.5f, 300, 300));

        textStyle.normal.textColor = Color.black;
        GUILayout.Label("Registration Page", textStyle);

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

        // Confirm password
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

        // Player info
        GUILayout.BeginHorizontal();
        GUILayout.Label("Character Name:", GUILayout.Width(90));
        playerName = GUILayout.TextField(playerName);
        GUILayout.EndHorizontal();

        GUILayout.Space(15);
        textStyle.normal.textColor = Color.red;
        GUILayout.Label(General.Message, textStyle);

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
        string[] content = { username.ToLower(), password, playerName };
        bool reliable = true;

        bool ready = false;

        if (username != "" && password != "" && confirmPassword != "" && playerName != "")
        {
            if (password == confirmPassword)
            {
                int maxChar = 15;
                if (playerName.Length <= maxChar)
                {
                    if(username.Length <= maxChar)
                        ready = true;
                    else
                        General.Message = "Username has to be less than " + maxChar + " characters";
                }
                else
                    General.Message = "Character name has to be less than " + maxChar + " characters";
            }
            else
                General.Message = "Password does not match";
        }
        else
            General.Message = "Please fill in every field";

        // send over to the server plugin to register if password matches and every textfield is filled
        if (ready)
            PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    public static void RegistrationReceive(byte eventCode, object content, int senderID)
    {
        if (eventCode != (byte)EvCode.REGISTRATION || senderID > 0)
            return;

        string message = "";
        message = (string)content;
        if(message[0] == 'S')
            registerComplete = true;

        General.Message = General.GetStringDataFromMessage(message, "Message");
    }

    // goes back to login page
    void LoginPage()
    {
        General.Message = "";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }
}
