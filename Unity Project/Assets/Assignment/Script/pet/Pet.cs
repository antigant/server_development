using UnityEngine;

// simple pet script that make it follows the player
public class Pet : Photon.MonoBehaviour
{
    public Transform target;
    float moveSpeed = 2.0f;

    private bool appliedInitialUpdate;
    private Vector3 correctPos = Vector3.zero; //We lerp towards this

    void Start()
    {
        // detach from parent
        gameObject.transform.parent = null;
        gameObject.transform.position = Player.GetInstance().GetPetPosition();    
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            //Network player, receive data
            //controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
            correctPos = (Vector3)stream.ReceiveNext();

            if (!appliedInitialUpdate)
            {
                appliedInitialUpdate = true;
                transform.position = correctPos;
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPos, Time.deltaTime * 5);
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        // saving the pet's position
        Player.GetInstance().SetPetPosition(gameObject.transform.position);
	}
}
