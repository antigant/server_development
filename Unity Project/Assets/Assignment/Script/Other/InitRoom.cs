using UnityEngine;

public class InitRoom : MonoBehaviour
{
    float dt;

	// Use this for initialization
	void Awake ()
    {
        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // wait 2 seconds before joining or creating room
        dt += Time.deltaTime;
        if (dt < 1.25f)
            return;

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 0;

        int IdNum = Random.Range(1, 9999);
        string userId = "Player" + IdNum.ToString();
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = userId;

        // Join or create room to be able to communicate with the server plugin, not sure what will happen if i remove the boolean for checking if player is in room.
        PhotonNetwork.JoinOrCreateRoom("InitServer", options, TypedLobby.Default);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }
}
