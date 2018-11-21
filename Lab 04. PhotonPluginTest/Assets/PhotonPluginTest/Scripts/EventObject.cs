using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : Photon.PunBehaviour
{
    string Message;

    void Awake()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
    }

    public void DoRaiseEvent()
    {
        byte evCode = 1;
        //        byte[] content = new byte[] { 3, 6, 9 };
        bool reliable = true;
        PhotonNetwork.RaiseEvent( evCode, null, reliable, null );
    }

    private void OnEvent( byte eventCode, object content, int senderID )
    {
        if((eventCode == 1) && (senderID <= 0))
        {
            //            PhotonPlayer sender = PhotonPlayer.Find(senderid);
            //int counter = (int)content;
            Message = (string)content;
            Debug.Log( string.Format( "Message from Server: {0}", Message ) );
        }
    }
    private void OnGUI()
    {
        GUILayout.Label( string.Format( "I am {0}. Message from Server: {1}", PhotonNetwork.playerName, Message ) );
    }
}