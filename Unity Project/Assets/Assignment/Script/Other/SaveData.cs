using UnityEngine;

public class SaveData : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(gameObject);
	}

    void OnApplicationQuit()
    {
        //LogoutScript.Logout();
    }
}
