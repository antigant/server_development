using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItem : Photon.MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        Vector3 startPos = Player.GetInstance().GetPosition();
        startPos.y -= 0.6f;
        startPos.z += 0.6f;
        transform.position = startPos;

        GetComponent<Rigidbody>().AddForce(Player.GetInstance().GetForward() * 200.0f, ForceMode.Force);
    }

    public void DestroyThisItem()
    {
        Destroy(gameObject);
    }
}
