using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StateChase : MonoBehaviour {

    public GameObject[] goArray { get; private set; }


    // Use this for initialization
    void Start()
    {

    }

 

    // Update is called once per frame
    void LateUpdate()
    {
        if (transform.parent.GetComponent<StateForRabbit>().currState == "Chase"
            /*&& GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().isGrounded*/)
        {

            transform.parent.GetComponent<PathManager>().target = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.parent.transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
            transform.parent.transform.LookAt(transform.parent.GetComponent<PathManager>().target);

         
            transform.parent.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x * transform.parent.GetComponent<PathManager>().walkSpeed * Time.deltaTime * 50,
                    transform.parent.GetComponent<Rigidbody>().velocity.y,
                    transform.forward.z * transform.parent.GetComponent<PathManager>().walkSpeed * Time.deltaTime * 50);
           
        }
    }
}
