using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opossum : MonoBehaviour
{
    public Vector2 speed;
    public Transform lookAheadP;
    public Transform lookInFrontP;
    Rigidbody2D rb;
    public LayerMask ground;
    public LayerMask wall;
    //Vector2 direction;
    bool isGroundAhead;
    bool isOnWall;
    bool onRamp;
    enum RampDirection
    {
        UP,
        DOWN,
        NONE
    }
    RampDirection rampdir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rampdir = RampDirection.NONE;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LookInFront();
        LookAhead();
        Move();
    }
    
    private void Move()
    {
        if (isGroundAhead)
        {
            rb.AddForce(Vector2.left * speed.x * Time.deltaTime * transform.localScale.x);

            if(onRamp)
            {
                if (rampdir == RampDirection.UP)
                {
                    rb.AddForce(Vector2.up * speed.y * Time.deltaTime);
                }
                else if (rampdir == RampDirection.DOWN)
                {
                    rb.AddForce(Vector2.down * speed.y * Time.deltaTime);
                }
                StartCoroutine(Rotate());
            }
            else
            {
                rb.SetRotation(0);
                
            }
            
            rb.velocity *= 0.90f;
        }
        else
        {
            FlipX();
            //StartCoroutine(Normalize());
        }
    }

    void LookAhead()
    {
        var groundhit = Physics2D.Linecast(transform.position, lookAheadP.position, ground);
        if (groundhit)
        {
            if(groundhit.collider.CompareTag("ramp"))
            {
                onRamp = true;
            }
            if (groundhit.collider.CompareTag("platform"))
            {
                onRamp = false;    
            }
            isGroundAhead = true;
        }
        else
        {
            rampdir = RampDirection.DOWN;
            isGroundAhead = false;
        }

        Debug.DrawLine(transform.position, lookAheadP.position, Color.green);
    }
    void LookInFront()
    {
        var wallhit = Physics2D.Linecast(transform.position, lookInFrontP.position, wall);
        if (wallhit)
        {

            if (!wallhit.collider.CompareTag("ramp"))
            {
                if (!onRamp && transform.rotation.z == 0)
                {
                    FlipX();
                }
                rampdir = RampDirection.DOWN;
            }
            else
            {
                rampdir = RampDirection.UP;
            }
        }
        Debug.DrawLine(transform.position, lookInFrontP.position, Color.red);
    }

    void FlipX()
    {
        transform.localScale = MultiplyVec3x(transform.localScale, -1);
    }

    IEnumerator Rotate()
    {
       yield return new WaitForSeconds(0.05f);
       rb.SetRotation(-26);
    }

    IEnumerator Normalize()
    {
        yield return new WaitForSeconds(0.05f);
        FlipX();
    }

    Vector3 MultiplyVec3x(Vector3 v3, float m)
    {
        float x = v3.x * m;
        return new Vector3(x, v3.y, v3.z);
    }

}
