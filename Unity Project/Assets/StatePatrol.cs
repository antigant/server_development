using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePatrol : MonoBehaviour
{

    public GameObject[] goArray { get; private set; }
    PathManager pathmanager;

    private void Start()
    {
        pathmanager = transform.parent.GetComponent<PathManager>();
    }

 
    // Update is called once per frame
    void LateUpdate()
    {
        if (transform.parent.GetComponent<StateForSeagull>().currState == "Patrol")
        {
            Vector3 temp = pathmanager.target - transform.parent.position;
            if (temp.sqrMagnitude < 5 * 5)
            {
                pathmanager.GiveMeNextPoint();
            }
            else
            {
                transform.parent.GetComponent<Rigidbody>().velocity = transform.forward * pathmanager.walkSpeed * Time.deltaTime * 50;
            }
            transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, Quaternion.LookRotation(temp), Time.deltaTime * 100);
        }
    }
}
