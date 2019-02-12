using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : Photon.MonoBehaviour
{
    public GameObject board;
    public Text[] names = new Text[3];

    static Text[] ldrName;

    private void Awake()
    {
        LeaderboardFunc();
    }

    // Use this for initialization
    void Start ()
    {
        board.SetActive(false);
        ldrName = names;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Toggle();
    }

    void Toggle()
    {
        if (Input.GetKeyDown(KeyCode.U))
            board.SetActive(!board.GetActive());
    }

    void LeaderboardFunc()
    {
        byte evCode = (byte)EvCode.LEADERBOARD;
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, null, reliable, null);
    }

    public static void LeaderboardReceive(byte eventCode, object content, int senderID)
    {
        if (eventCode == (byte)EvCode.LEADERBOARD)
        {
            string[] topPlayers = (string[])content;
            for (int i = 0; i < topPlayers.Length; ++i)
                ldrName[i].text = topPlayers[i];
        }
    }
}
