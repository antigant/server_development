using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateForRabbit : MonoBehaviour 
{

    // Use this for initialization
    [SerializeField]
    GameObject Patrol;

    [SerializeField]
    GameObject Chase;

    public GameObject Player;

    

    public string currState;

    GameObject[] Walls;

    public Animator anim;

    [SerializeField]
    float DistanceBetweenPlayer;

    [SerializeField]
    float delayTime;

    void Start()
    {
        currState = "Patrol";
        Player = GameObject.FindGameObjectWithTag("Player");
      
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Player == null)
            Player = GameObject.FindGameObjectWithTag("Player");

        //Vector3 extraGravityForce = (Physics.gravity * 2) - Physics.gravity;
        //GetComponent<Rigidbody>().AddForce(extraGravityForce);

        if (Player == null)
        {
            return;
        }

        //foreach (GameObject wall in Walls)
        //{
        //    float WallDist = (transform.position - wall.transform.position).magnitude;
            float PlayerDist = (transform.position - Player.transform.position).magnitude;
            
        //}

        //chase
        if ((Player.transform.position - transform.position).sqrMagnitude < 50.0f && (Player.transform.position - transform.position).sqrMagnitude > DistanceBetweenPlayer)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && currState == "Attack")
            {

            }
            else
            { 
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    currState = "Chase";
                 //   Physics.IgnoreCollision(Player.GetComponent<Collider>(), GetComponent<Collider>());
                    anim.Play("Run");
            }
        }

        // attack
        else if ((GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).sqrMagnitude < DistanceBetweenPlayer)
        {
            transform.LookAt(new Vector3(Player.transform.position.x, transform.position.y, Player.transform.position.z));
            currState = "Attack";
            anim.Play("Attack");
            anim.speed = delayTime;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }

        // patrol
        else
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
           // Physics.IgnoreCollision(Player.GetComponent<Collider>(), GetComponent<Collider>(), false);
            if (currState == "Chase" || currState == "Attack")
            {
                GetComponent<PathManager>().target = GetComponent<PathManager>().FindClosestWaypoint(transform.position).transform.position;
            }

            currState = "Patrol";
            anim.Play("Walk");
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Enemy")
    //    {
    //        Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
    //    }
    //}
}


