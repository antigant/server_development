using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Waypoint : MonoBehaviour {

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    public List<Waypoint> neighbors;

    public Waypoint Previous
    {
        get;
        set;
    }

    public float Distance
    {
        get;
        set;
    }

    //private void OnDrawGizmos()
    //{
    //    // gizmos are used to give visual debugging or setup aids in the scene view
    //    if (neighbors == null)
    //        return; // if nothing is in the list

    //    Gizmos.color = new Color(0f, 0f, 0f);
    //    foreach(var neighbor in neighbors)
    //    {
    //        if(neighbor != null)
    //        {
    //            // draw the line from 1 neighbor to another
    //            Gizmos.DrawLine(transform.position, neighbor.transform.position);
    //        }
    //    }
    //}
}
