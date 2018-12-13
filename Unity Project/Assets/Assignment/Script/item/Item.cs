using System.Collections;
using UnityEngine;

// using this script when only the player drop an item
public class Item : Photon.MonoBehaviour
{
    int id = -1;
    int multiplier = 1;
    float yPos;
    float originalYPos;
    float dt = 0.0f;
    // check if this gameObject is still "alive"
    bool alive = true;
    bool updated = false;

    // time before the item despawn
    readonly float timeAlive = 10.0f;
    readonly float rotateSpeed = 1.0f;
    readonly float moveSpeed = 0.25f;
    readonly float maxDist = 0.25f;

    Rigidbody rb;

    void Start()
    {
        //Vector3 startPos = Player.GetInstance().GetPosition();
        //startPos.y -= 0.6f;
        //startPos.z += 0.6f;
        //transform.position = startPos;

        originalYPos = transform.position.y;
        yPos = originalYPos;

        rb = GetComponent<Rigidbody>();
        //rb.AddForce(Player.GetInstance().GetForward() * 100.0f, ForceMode.Force);
    }

    void Update()
    {
        // if this is mine, i calculate the time left before this item despawn and send it to delete in the db
        //if (photonView.isMine)
        //{
        //    dt += Time.deltaTime;
        //    if (dt < timeAlive)
        //        return;

        //    alive = false;
        //}

        // should have the same code to rotate and "float" for all clients
        if (yPos - originalYPos >= maxDist || yPos - originalYPos <= -maxDist)
            multiplier *= -1;
        yPos += moveSpeed * Time.deltaTime * multiplier;

        transform.Rotate(0.0f, rotateSpeed, 0.0f);
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

        // set the state of this item
        gameObject.SetActive(alive);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(alive);
        }
        else
        {
            //Network player, receive data
            alive = (bool)stream.ReceiveNext();
        }
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // use this to init the values of the item drop by the player

        //We know there should be instantiation data..get our bools from this PhotonView!
        object[] objs = photonView.instantiationData; //The instantiate data..
        id = (int)objs[0];
    }

    // when someone picks up the item
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag != "Player")
            return;

        alive = false;
        Player.GetInstance().GetInventory().UpdateItem("UPDATE", id, Player.GetInstance().GetAccountID());
    }

    // only call this when timer runs out
    void DeleteItem()
    {
        Player.GetInstance().GetInventory().UpdateItem("DELETE", id);
    }
}
