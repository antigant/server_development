using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject map;

	// Use this for initialization
	void Start ()
    {
        map.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        ToggleMap();
    }

    void ToggleMap()
    {
        if(Input.GetKeyDown(KeyCode.M))
            map.SetActive(!map.GetActive());
    }
}
