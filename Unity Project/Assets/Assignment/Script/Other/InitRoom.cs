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
        if (dt < 2.0f)
            return;

        // Join or create room to be able to communicate with the server plugin, not sure what will happen if i remove the boolean for checking if player is in room.
        PhotonNetwork.JoinOrCreateRoom("InitServer", new RoomOptions() { MaxPlayers = 10 }, TypedLobby.Default);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }
}
