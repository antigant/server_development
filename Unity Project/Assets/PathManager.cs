using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathManager : MonoBehaviour
{

    public string TagToChase;
    public float walkSpeed;
    public Stack<Vector3> currentPath;
    public Vector3 target;
    public Vector3 previousTarget;

    public Waypoint previousWaypoint;

    //bool something = false;

    void Start()
    {
        currentPath = new Stack<Vector3>();
        currentPath.Push(FindClosestWaypoint(transform.position).transform.position);
        target = currentPath.Pop();
        previousTarget = Vector3.zero;
        transform.LookAt(target);

    }

    public void GiveMeNextPoint()
    {
        previousWaypoint = FindClosestWaypoint(target);
        GetRandomAdjacent();
        target = currentPath.Pop();
    }

    public void Stop()
    {
        // reset
        currentPath = null;
    }

    public void GetRandomAdjacent()
    {
        int rand = Random.Range(0, previousWaypoint.neighbors.Count);
        currentPath.Push(previousWaypoint.neighbors[rand].transform.position);
    }

    void LateUpdate()
    {

    }

    public Waypoint FindClosestWaypoint(Vector3 target) // this is to find the closest waypoint to the target
    {
        GameObject closest = null;
        float closestDist = Mathf.Infinity; // a representation of positive infinity
        foreach (var waypoint in GameObject.FindGameObjectsWithTag(TagToChase)) // finding objects with a specific tag
        {
            var dist = (waypoint.transform.position - target).magnitude; // magnitude returns the length of the vector
            if (dist < closestDist) // finding the waypoint with closest dist 
            {
                closest = waypoint;
                closestDist = dist;
            }
        }

        if (closest != null)
        {

            return closest.GetComponent<Waypoint>(); // returning the closest waypoint
        }

        return null;
    }
}
