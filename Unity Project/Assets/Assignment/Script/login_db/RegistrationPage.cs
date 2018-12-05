using UnityEngine;

public class RegistrationPage : Photon.PunBehaviour
{
    string message;

    void Awake()
    {
        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;

        PhotonNetwork.OnEventCall += RegistrationReceive;
    }

    void RegistrationReceive(byte eventCode, object content, int senderID)
    {
        // registration failed
        if ((eventCode == (byte)EvCode.REGISTRATION) && (senderID <= 0))
            message = (string)content;
    }
}
