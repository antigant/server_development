using System.Collections;
using UnityEngine;

// using this script when only the player drop an item
public class Item : Photon.MonoBehaviour
{
    // check if this gameObject is still "alive"
    public int id = -1;

    // time before the item despawn
    float dt = 0.0f;
    readonly float timeAlive = 10.0f;

    private bool appliedInitialUpdate;
    private Vector3 correctPos = Vector3.zero; //We lerp towards this

    void Start()
    {
        PhotonNetwork.OnEventCall += ItemState;

        transform.position = GameObject.FindGameObjectWithTag("item_drop_pos").transform.position;

        GetComponent<Rigidbody>().AddForce(Player.GetInstance().GetForward() * 125.0f, ForceMode.Force);
    }

    void Update()
    {
        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, correctPos, Time.deltaTime * 5);
        }
    }

    public void ItemState(byte eventCode, object content, int senderID)
    {
        if (eventCode != (byte)EvCode.ITEM_STATE || senderID > 0)
            return;
        DestroyItem();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
        }
        else
        {
            //Network player, receive data
            correctPos = (Vector3)stream.ReceiveNext();

            if (!appliedInitialUpdate)
            {
                appliedInitialUpdate = true;
                transform.position = correctPos;
            }
        }
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // use this to init the values of the item drop by the player

        //We know there should be instantiation data..get our bools from this PhotonView!
        object[] objs = photonView.instantiationData; //The instantiate data..
        id = (int)objs[0];
    }

    // only call this when timer runs out
    void DeleteItem()
    {
        Player.GetInstance().GetInventory().UpdateItem("DELETE", id);
        DestroyItem();
    }

    public IEnumerator ProcessDestroyItem()
    {
        yield return new WaitForSeconds(0.2f);
        DestroyItem();
    }

    void DestroyItem()
    {
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= ItemState;
    }
}
