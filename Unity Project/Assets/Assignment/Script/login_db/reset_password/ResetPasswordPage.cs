using UnityEngine;
using CustomPlugin;

public class ResetPasswordPage : MonoBehaviour
{
    string username = "";
    string password = "";
    string confirmPassword = "";
    string prevPassword = "";

    static bool resetPasswordComplete;
    float dt = 0.0f;

    GUIStyle textStyle;

    void Awake()
    {
        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;

        textStyle = new GUIStyle();
        resetPasswordComplete = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            ResetPassword();

        if (!resetPasswordComplete)
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

        // Previous password
        GUILayout.BeginHorizontal();
        GUILayout.Label("Previous Password:", GUILayout.Width(90));
        if (prevPassword == null)
        {
            prevPassword = GUILayout.TextField("password");
            prevPassword = "";
        }
        else
            prevPassword = GUILayout.PasswordField(prevPassword, '*', 15);
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

        GUILayout.Space(15);
        textStyle.normal.textColor = Color.red;
        GUILayout.Label(General.Message, textStyle);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Password"))
        {
            ResetPassword();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    void ResetPassword()
    {
        byte evCode = (byte)EvCode.RESET_PASSWORD;
        //string[] content = { username.ToLower(), password, prevPassword };

        EncryptPassword pw = new EncryptPassword();
        HashWithSaltResult hash = pw.HashWithSalt(password, 64, System.Security.Cryptography.SHA256.Create());

        CRegistration reset = new CRegistration(username.ToLower(), prevPassword, hash);
        bool reliable = true;

        bool ready = false;

        if (username != "" && password != "" && confirmPassword != "" && prevPassword != "")
        {
            if (password == confirmPassword)
                ready = true;
            else
                General.Message = "Password does not match";
        }
        else
            General.Message = "Please fill in every field";

        // send over to the server plugin to register if password matches and every textfield is filled
        if (ready)
            PhotonNetwork.RaiseEvent(evCode, CRegistration.Serialize(reset), reliable, null);
    }

    public static void ResetPasswordReceive(byte eventCode, object content, int senderID)
    {
        if (eventCode != (byte)EvCode.RESET_PASSWORD || senderID > 0)
            return;

        string message = "";
        message = (string)content;
        if (message[0] == 'S')
            resetPasswordComplete = true;

        General.Message = General.GetStringDataFromMessage(message, "Message");
    }

    // goes back to login page
    void LoginPage()
    {
        General.Message = "";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }
}
