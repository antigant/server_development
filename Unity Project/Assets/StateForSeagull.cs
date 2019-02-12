using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateForSeagull : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    GameObject Patrol;

    //[SerializeField]
    //GameObject Chase;
    

    bool ThereIsAWall;

    public string currState;

    GameObject[] Walls;

    public Animator anim;

    [SerializeField]
    float DistanceBetweenPlayer;

    [SerializeField]
    float delayTime;



    void Start () {
        currState = "Patrol";
        ThereIsAWall = false;
        //Walls = GameObject.FindGameObjectsWithTag("Wall");
        anim = GetComponentInChildren<Animator>();


    }

    // Update is called once per frame
    void Update() {
        ThereIsAWall = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                //GetComponent<PathManager>().target = GetComponent<PathManager>().FindClosestWaypoint(transform.position).transform.position;

            currState = "Patrol";

	}

}
