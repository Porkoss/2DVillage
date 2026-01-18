using UnityEditor;
using UnityEngine;

public class Loot : MonoBehaviour
{
    // need to add this on object that have gravitation in order to make them stop when "hitting the ground" with isometric 2D camera
    Rigidbody2D rb;

    Vector3 startPosition;
    private Collider2D newCollider2D;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        newCollider2D = GetComponent<Collider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < startPosition.y) 
        {
            StopGravity();
        }
    }

    public void StopGravity()
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        newCollider2D.enabled = true;
    }

    public virtual void  Collected()
    {
        Destroy(gameObject);
    }


}
