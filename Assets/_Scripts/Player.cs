using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PlayerAnimationState
{
    IDLE,
    RUN,
    JUMP,
    CROUCH,
    FALL
}

public class Player : MonoBehaviour
{
    public Joystick joystick;
    public GameObject spawnPoint;
    public Vector2 threshold;
    public float speed;
    public float jumpForce;
    bool jumping;
    bool crouching;
    bool grounded;
    Rigidbody2D rb;
    PlayerAnimationState animState;

    // Start is called before the first frame update
    void Start()
    {
        jumping = false;
        grounded = false;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Move()
    {
        if (grounded)
        {
            // horizonatal movement
            if (!jumping && !crouching)
            {
                // horizontal joystick movement
                if (joystick.Horizontal > threshold.x)
                {
                    // run right
                    rb.AddForce(Vector2.right * speed * Time.deltaTime);
                    GetComponent<SpriteRenderer>().flipX = false;
                    animState = PlayerAnimationState.RUN;
                }
                else if (joystick.Horizontal < -threshold.x)
                {
                    // run left
                    rb.AddForce(Vector2.left * speed *Time.deltaTime);
                    GetComponent<SpriteRenderer>().flipX = true;
                    animState = PlayerAnimationState.RUN;
                }
                else
                {
                    animState = PlayerAnimationState.IDLE;
                }
            }

            // jumping
            if (joystick.Vertical > threshold.y && !jumping)
            {
                rb.AddForce(Vector2.up * jumpForce);
                animState = PlayerAnimationState.JUMP;
                jumping = true;
            }
            else
            {
                jumping = false;
            }
            // crouching
            if (joystick.Vertical < -threshold.y && !crouching)
            {
                animState = PlayerAnimationState.CROUCH;
                crouching = true;
            }
            else
            {
                crouching = false;
            }

        }
        else
        {
            animState = PlayerAnimationState.JUMP;
        }
        GetComponent<Animator>().SetInteger("state", (int)animState);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "platform")
        {
            grounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "platform")
        {
            grounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //respawn
        if(other.tag == "death")
        {
            transform.position = spawnPoint.transform.position;
        }
    }
}
