using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridChangeColor : MonoBehaviour {
    public Color Away;
    public Color Inside;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Vision")
        {
            print("gotem");
            this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_WireColor", Inside);// = Away;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Vision")
        {
            this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_WireColor", Away);
        }
    }
}
