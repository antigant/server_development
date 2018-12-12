using UnityEngine;

public class InitReceiveFunc : MonoBehaviour
{
	// Use this for initialization
	void Awake ()
    {
        // don't destory this game object so that event calls can be init once
        PhotonNetwork.OnEventCall += LoginPage.LoginReceive;
        PhotonNetwork.OnEventCall += RegistrationPage.RegistrationReceive;
        PhotonNetwork.OnEventCall += ResetPasswordPage.ResetPasswordReceive;
        PhotonNetwork.OnEventCall += Inventory.UpdateItemReceive;
        PhotonNetwork.OnEventCall += Inventory.InitInventoryReceive;
	}

    public static void RemoveEventCalls()
    {
        PhotonNetwork.OnEventCall -= LoginPage.LoginReceive;
        PhotonNetwork.OnEventCall -= RegistrationPage.RegistrationReceive;
        PhotonNetwork.OnEventCall -= ResetPasswordPage.ResetPasswordReceive;
        PhotonNetwork.OnEventCall -= Inventory.UpdateItemReceive;
        PhotonNetwork.OnEventCall -= Inventory.InitInventoryReceive;
    }
}
