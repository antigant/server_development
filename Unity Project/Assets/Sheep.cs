using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{

    [SerializeField]
    int invokeTimeMin = 5;
    [SerializeField]
    int invokeTimeMax = 10;
    int invokeTime;
    [SerializeField]
    float movingRange = 10.0f;

    [SerializeField]
    Transform toRotate;

    NavMeshAgent sheep;
    public bool debug;

    Animator anim;
    float timer;


    public enum State
    {
        STATE_Move,
        STATE_Moving,
        STATE_Eating
    };
    State _state;

    void Start()
    {
        anim = GetComponent<Animator>();
        sheep = GetComponent<UnityEngine.AI.NavMeshAgent>();
        invokeTime = Random.Range(invokeTimeMin, invokeTimeMax);
        anim.SetBool("Eat", true);
        _state = State.STATE_Eating; StartCoroutine(AI());
    }

    IEnumerator AI()
    {
        while (true)
        {
            anim.SetFloat("Speed", (sheep.velocity.magnitude / sheep.speed));

            switch (_state)
            {
                case State.STATE_Moving:

                    if (sheep.remainingDistance <= 0)
                    {

                        anim.SetBool("Eat", true);
                        _state = State.STATE_Eating;
                    }
                    break;

                case State.STATE_Eating:

                    timer += Time.deltaTime;

                    if (timer >= 5)
                    {
                        anim.SetBool("Eat", false);
                        timer = 0;
                        _state = State.STATE_Move;
                    }
                    break;

                case State.STATE_Move:

                    //anim.SetBool("Eat", false);            
                    sheep.destination = RandomPoint();
                    _state = State.STATE_Moving;
                    break;

                default:
                    break;
            }
            
            yield return null;
        }
    }

    ///<summary>Returns true if nearest point is found & if point where to move is front of the sheep</summary>
    bool RandomPointFromNavMesh(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {

            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            //Debug.DrawRay(randomPoint, transform.up, Color.red, 5.0f);
            Vector3 isInFront = (randomPoint - transform.position).normalized;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas) && Vector3.Dot(isInFront, transform.forward) > 0)
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    ///<summary>returns random point as a vector3 from nav mesh</summary>
    Vector3 RandomPoint()
    {
        Vector3 point;
        if (RandomPointFromNavMesh(transform.position, movingRange, out point))
        {
            if (debug)
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            }
        }
        return point;
    }
}
