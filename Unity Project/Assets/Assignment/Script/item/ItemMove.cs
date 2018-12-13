using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMove : MonoBehaviour
{
    float yPos;
    float originalYPos;
    int multiplier = 1;

    readonly float rotateSpeed = 1.0f;
    readonly float moveSpeed = 0.25f;
    readonly float maxDist = 0.25f;

    // Use this for initialization
    void Start ()
    {
        originalYPos = transform.localPosition.y;
        yPos = originalYPos;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // should have the same code to rotate and "float" for all clients
        if (yPos - originalYPos >= maxDist || yPos - originalYPos <= -maxDist)
            multiplier *= -1;
        yPos += moveSpeed * Time.deltaTime * multiplier;

        transform.Rotate(0.0f, rotateSpeed, 0.0f);
        transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
    }
}
