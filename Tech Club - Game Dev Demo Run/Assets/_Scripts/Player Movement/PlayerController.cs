﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] float speed;
    [SerializeField] float jumpPower;
    
    //If the player is standing on a ground
    [SerializeField] bool grounded;

    [SerializeField] Rigidbody2D rb2d; //This references the Rigidbody2D component of the player
    
    [SerializeField] float coyoteTime = 0; //This is a leeway that allows player to jump even after leaving ground by a few seconds. This gives room for player error.


    //This is an input buffer that counts down when the player presses space mid-air. 
    //If the player presses space right before they land, they should jump just as they land.
    [SerializeField] float jumpBuffer = 0; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //FixedUpdate is better at handling physics than Update
    void FixedUpdate ()
    {
        //This is the player's horizontal movement.
        rb2d.AddForce(new Vector2(Input.GetAxisRaw("Horizontal"), 0) * 100 * speed * Time.fixedDeltaTime);        

        
    }

    //Jump
    void Jump ()
    {
        rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);

        rb2d.AddForce(new Vector2(0, 100 * jumpPower));
        coyoteTime = 0;
        jumpBuffer = 0;
    }

    // Update is called once per frame
    // The number of times the update function is called per second is dependant on the framerate
    void Update()
    {
        if (!grounded) //If the player isn't grounded, count down the coyote time.
        {
            coyoteTime -= 100f * Time.deltaTime; //Subtracting it by 100 per second.
            coyoteTime = Mathf.Clamp(coyoteTime, 0, 25f); //This just clamps the value between 0 and 15
        }


        //This just counts it down.
        jumpBuffer -= 100f * Time.deltaTime;
        jumpBuffer = Mathf.Clamp(jumpBuffer, 0, 25f);

        //This is called on the frame the space key is pressed. 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if ((coyoteTime > 0 || grounded))
                Jump();
            else if (rb2d.velocity.y < 0) //If the player can't jump and is falling down, then buffer the input.
                jumpBuffer = 25f;
        }

        if (grounded && jumpBuffer > 0) //This is the jump input buffer.
        {
            Jump();
        }

        //To make the player jump more satisfying:
        //1) When the player is jumping the gravity on the player should be weaker.
        //2) If the player holding the space bar down while jumping, the gravity should be even weaker.
        //3) If the player is falling down, the gravity should be stronger.
        if (rb2d.velocity.y > 0 && Input.GetKey(KeyCode.Space))
            rb2d.gravityScale = 0.5f;
        else if (rb2d.velocity.y > 0)
            rb2d.gravityScale = 1.5f;
        else if (rb2d.velocity.y < 0)
            rb2d.gravityScale = 5f;
    }


    //This sets if the player is grounded or not
    public void SetGrounded (bool _grounded)
    {
        grounded = _grounded;

        if (grounded)
            coyoteTime = 25f; //Players have 0.25 seconds to jump after leaving the ground.
    }
}
