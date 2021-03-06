using UnityEngine;
using System.Collections;

public class GameManagerVik : Photon.MonoBehaviour {

    // this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
    // read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
    public string playerPrefabName = "Charprefab";

    void OnJoinedRoom()
    {
        StartGame();
        //DoRaiseEvent();
    }

    IEnumerator OnLeftRoom()
    {
        //Easy way to reset the level: Otherwise we'd manually reset the camera

        //Wait untill Photon is properly disconnected (empty room, and connected back to main server)
        while(PhotonNetwork.room!=null || PhotonNetwork.connected==false)
            yield return 0;

        //Application.LoadLevel(Application.loadedLevel);
        //UnityEngine.SceneManagement.SceneManager.LoadScene("InitServer");
    }

    void StartGame()
    {
        Camera.main.farClipPlane = 1000; //Main menu set this to 0.4 for a nicer BG    

        //prepare instantiation data for the viking: Randomly diable the axe and/or shield
        bool[] enabledRenderers = new bool[2];
        enabledRenderers[0] = Random.Range(0,2)==0;//Axe
        enabledRenderers[1] = Random.Range(0, 2) == 0; ;//Shield
        
        object[] objs = new object[1]; // Put our bool data in an object array, to send
        objs[0] = enabledRenderers;

        // Spawn our local player
        PhotonNetwork.Instantiate(playerPrefabName, transform.position, Quaternion.identity, 0, objs);
    }

    void OnGUI()
    {
        if (PhotonNetwork.room == null) return; //Only display this GUI when inside a room

        if (GUILayout.Button("Leave Room"))
        {
            // add code to save player and pet's position
            Logout();

            UnityEngine.SceneManagement.SceneManager.LoadScene("InitServer");
            Player.GetInstance().ResetPlayer();
            InitReceiveFunc.RemoveEventCalls();
            PhotonNetwork.LeaveRoom();
        }
    }

    public void Logout()
    {
        LogoutEvent();
    }

    void LogoutEvent()
    {
        byte evCode = (byte)EvCode.LOGOUT;
        // logout to save player and pet position
        string[] content = { Player.GetInstance().GetAccountID().ToString(), Player.GetInstance().GetPosition().x.ToString(), Player.GetInstance().GetPosition().y.ToString(), Player.GetInstance().GetPosition().z.ToString(), Player.GetInstance().GetPetPosition().x.ToString(), Player.GetInstance().GetPetPosition().y.ToString(), Player.GetInstance().GetPetPosition().z.ToString(), AudioManager.instance.GetVolume(0).ToString(), AudioManager.instance.GetVolume(1).ToString(), AudioManager.instance.GetVolume(2).ToString() };
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    private void OnApplicationQuit()
    {
        Logout();
        Player.GetInstance().ResetPlayer();
        InitReceiveFunc.RemoveEventCalls();
    }

    void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("OnDisconnectedFromPhoton");
    }    
}
