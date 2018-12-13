using UnityEngine;

// using this script to destory the gameobject inside the editor
public class DestroyItem : MonoBehaviour
{
    Vector3 startPos;
    Rigidbody rb;

    void Start()
    {
        startPos = Player.GetInstance().GetPosition();
        startPos.y -= 0.6f;
        startPos.z += 0.6f;
        gameObject.transform.position = startPos;

        rb = GetComponent<Rigidbody>();
        rb.AddForce(Player.GetInstance().GetForward() * 100.0f, ForceMode.Force);
    }

    public void DestoryThisItem()
    {
        Destroy(gameObject);
    }
}
