using UnityEngine;
using TMPro;

// using this to set the player name and sync the rotation
public class NamePanel : Photon.MonoBehaviour
{
    TextMeshPro playerName;
    private bool appliedInitialUpdate;
    // position, rotation of the panel
    Transform correctPanelTransform;

    static Camera ownCamera;

    // Use this for initialization
    void Start ()
    {
        playerName = GetComponent<TextMeshPro>();
        correctPanelTransform = gameObject.transform;

        // init player's name
        playerName.text = Player.GetInstance().GetPlayerName();
        ownCamera = Camera.main;
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //stream.SendNext(transform.position);
            stream.SendNext(Player.GetInstance().GetPlayerName());
        }
        else
        {
            //Vector3 pos = (Vector3)stream.ReceiveNext();
            //correctPanelTransform.position = pos;

            //Network player, receive data
            if (!appliedInitialUpdate)
            {
                string name = (string)stream.ReceiveNext();
                playerName.text = name;
                appliedInitialUpdate = true;
                //gameObject.transform.position = correctPanelTransform.position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            // update the name panel of other players to look at your camera
            correctPanelTransform.LookAt(ownCamera.transform);
            correctPanelTransform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else
            gameObject.transform.localScale = Vector3.one;
    }
}
