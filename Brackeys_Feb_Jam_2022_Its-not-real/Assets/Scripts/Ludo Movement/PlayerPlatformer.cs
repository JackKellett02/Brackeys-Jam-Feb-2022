using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformer : MonoBehaviour
{
    float inputX;
    bool grounded;
    float jumpTimer;
    float groundedTimer;
    float slowDownSpeed = 1.0f;
    Rigidbody2D RB;

    [SerializeField]
    float jumpBufferValue = 0.15f;
    [SerializeField]
    float speed = 5;
    [SerializeField]
    float jumpHeight = 5;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        //Handling Jump Input Buffer values
        jumpTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = jumpBufferValue;
        }
        
    }

    private void FixedUpdate()
    {
        //Setting player horizontal velocity, only add up to max speed value, but allow player to keep faster velocities
        float maxSpeed = speed * Time.deltaTime;
        if (grounded)
        {
            RB.velocity -= RB.velocity * new Vector2(0.9f, 0);
        }
        //If Player moves left
        if (inputX < 0)
        {
            //If they're moving faster than max left, slow back down a little
            //Otherwise, move max left
            if(RB.velocity.x < -maxSpeed)
            {
                RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, -maxSpeed, 0.02f), RB.velocity.y);
            }
            else
            {
                RB.velocity += new Vector2(-maxSpeed - RB.velocity.x, 0);
            }
        }
        //If Player moves right
        else if (inputX > 0)
        {
            //If they're moving faster than max right, slow back down a little
            //Otherwise, move max right
            if (RB.velocity.x > maxSpeed)
            {
                RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, maxSpeed, 0.02f), RB.velocity.y);
            }
            else
            {
                RB.velocity += new Vector2(maxSpeed - RB.velocity.x, 0);
            }
        }
        else
        {
            if (RB.velocity.x > maxSpeed)
            {
                RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, maxSpeed, 0.7f), RB.velocity.y);
            }
            else if (RB.velocity.x < -maxSpeed)
            {
                RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, -maxSpeed, 0.7f), RB.velocity.y);
            }
            else
            {
                //RB.velocity -= RB.velocity * new Vector2(0.9f, 0);
                RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, 0, 0.2f), RB.velocity.y);
            }
            
        }



        
        if (jumpTimer > 0.0f && grounded == true)
        {
            RB.velocity = new Vector2(RB.velocity.x, jumpHeight);
            jumpTimer = 0.0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Room")
        {
            Camera.main.GetComponent<CameraController>().SwitchRooms(collision);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }
}
