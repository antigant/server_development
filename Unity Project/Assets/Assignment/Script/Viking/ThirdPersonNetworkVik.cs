using UnityEngine;
using System.Collections;


public class ThirdPersonNetworkVik : Photon.MonoBehaviour
{
    ThirdPersonCameraNET cameraScript;
    ThirdPersonControllerNET controllerScript;
    private bool appliedInitialUpdate;

    void Awake()
    {
        cameraScript = GetComponent<ThirdPersonCameraNET>();
        controllerScript = GetComponent<ThirdPersonControllerNET>();

    }
    void Start()
    {
        //TODO: Bugfix to allow .isMine and .owner from AWAKE!
        if (photonView.isMine)
        {
            //MINE: local player, simply enable the local scripts
            cameraScript.enabled = true;
            controllerScript.enabled = true;
            Camera.main.transform.parent = transform;
            Camera.main.transform.localPosition = new Vector3(0, 2, -10);
            Camera.main.transform.localEulerAngles = new Vector3(10, 0, 0);

        }
        else
        {           
            cameraScript.enabled = false;
            controllerScript.enabled = true;

        }
        controllerScript.SetIsRemotePlayer(!photonView.isMine);

        //gameObject.name = gameObject.name + photonView.viewID;
        gameObject.transform.position = Player.GetInstance().GetPosition();
        gameObject.name = Player.GetInstance().GetPlayerName();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            // stream.SendNext((int)controllerScript._characterState);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(GetComponent<Rigidbody>().velocity);

        }
        else
        {
            //Network player, receive data
            //controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            GetComponent<Rigidbody>().velocity = (Vector3)stream.ReceiveNext();

            if (!appliedInitialUpdate)
            {
                appliedInitialUpdate = true;
                transform.position = correctPlayerPos;
                transform.rotation = correctPlayerRot;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
        }
        else
        {
            Player.GetInstance().SetPosition(transform.position);
            Player.GetInstance().SetForward(transform.forward);
        }
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //We know there should be instantiation data..get our bools from this PhotonView!
        object[] objs = photonView.instantiationData; //The instantiate data..
        bool[] mybools = (bool[])objs[0];   //Our bools!

        //disable the axe and shield meshrenderers based on the instantiate data
        MeshRenderer[] rens = GetComponentsInChildren<MeshRenderer>();
        rens[0].enabled = mybools[0];//Axe
        rens[1].enabled = mybools[1];//Shield

    }

    // should even work here when another client pick up, but im just gonna cheat this part
    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.isMine || collision.transform.tag != "item")
            return;

        Item item = collision.transform.GetComponent<Item>();

        Player.GetInstance().GetInventory().UpdateItem("U2PDATE", item.id, Player.GetInstance().GetAccountID());
        Player.GetInstance().AddItem(item.id);

        AudioManager.instance.PlaySFX(0);
        Debug.Log("Collision with item_id:" + item.id);
    }
}