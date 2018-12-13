﻿using UnityEngine;

// using this script when only the player drop an item
public class Item : Photon.MonoBehaviour
{
    public DestroyItem destroyItem;

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

    void Start()
    {
        originalYPos = transform.localPosition.y;
        yPos = originalYPos;
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
        transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);

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
        alive = false;
        Player.GetInstance().GetInventory().UpdateItem("UPDATE", id, Player.GetInstance().GetAccountID());
    }

    // only call this when timer runs out
    void DeleteItem()
    {
        Player.GetInstance().GetInventory().UpdateItem("DELETE", id);
        // remove the collder from the scene
        destroyItem.DestoryThisItem();
    }
}
