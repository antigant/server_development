using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Changeofvision : MonoBehaviour {

    public GameObject visualGridSize;
    //public GameObject Camera;

    public bool zoomin;
    public float timer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer = -1.0f * Time.deltaTime;
        //Key Input
		if(Input.GetKeyDown(KeyCode.P) && zoomin == false && timer < 0)
        {
            zoomin = true;
            timer = 1;
        }
        if(Input.GetKeyDown(KeyCode.P) && zoomin == true && timer < 0)
        {
            zoomin = false;
            timer = 1;
        }

        //Update
        if(zoomin)
        {
            visualGridSize.gameObject.transform.localScale = new Vector3(20, 20, 20);
            Camera.main.farClipPlane = 400.0f;
        }
        else if (!zoomin)
        {
            visualGridSize.gameObject.transform.localScale = new Vector3(3, 5, 3);
            Camera.main.farClipPlane = 30.0f;
        }

    }
}
