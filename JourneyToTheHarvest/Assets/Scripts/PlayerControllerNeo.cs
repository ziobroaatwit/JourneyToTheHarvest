using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Unity.Mathematics;
using UnityEngine.Rendering;
using UnityEngine.UIElements.Experimental;
/*
 * Author: Aleksander Ziobro
 */


//Player behavior script
//new PlayerController script following tutorial by Pandemonium https://www.youtube.com/playlist?list=PLgOEwFbvGm5o8hayFB6skAfa8Z-mw4dPV
//while also kind of goofing it and cheating the direction so I don't have to change the sprite flipping code that was based on the in-class lecture.
//To Prof Schuster: If this is an issue feel free to email me. I figured it's okay to watch a tutorial since we were using the pre-provided controller code anyway from the start.
public class PlayerControllerNeo : MonoBehaviour
{
    //RigidBody reference
    private Rigidbody2D rb;
    private float input;
    //Allows us to set speed in Unity Editor
    public float speed;
    //Allows us to set the jump "speed" more like jump force in Unity Editor
    public float jumpSpeed;
    //Allows us to set the force of the outward motion of a wall jump
    public float wallJumpOutwardForce;
    //Allows us to set the upward force of the wall jump
    public float wallJumpUpwardForce;
    //Box Collider reference
    //this was a box but im bashing it into being a capsule
    private CapsuleCollider2D boxCollider;
    //Mask required for isGrounded
    public LayerMask groundLayer;
    //Mask required for Wall Jumping
    public LayerMask wallLayer;
    //Cooldown for how soon after a wall jump you can jump again
    public float wallJumpCooldown;
    //Amount of coins the player will have
    private int coins;
    //How much health the player will have
    private int health;
    //The max health they can have
    public int maxHealth;
    //time to accelerate
    public float timeToMaxSpeed;
    private float accelRatePerSec;
    private float forwardVelocity;


    //DASH MECHANIC VARIABLES from PA1
    public float dashSpeed = 500f;
    public float dashCoolDownLimit = 5f;
    private float dashCoolDown = 5f;
    public float dashTimeLimit = .5f;
    private float dashTime = .5f;
    public float defaultSpeed = 150f;
    private bool dashing = false;
    public Animator anim;

    private void Awake()
    {
        accelRatePerSec = speed / timeToMaxSpeed;
        forwardVelocity = 0f;
        //Gets the RigidBody component of the Player
        rb = GetComponent<Rigidbody2D>();
        //Gets the BoxCollider2D component of the player
        boxCollider = GetComponent<CapsuleCollider2D>();
        health = maxHealth;
        coins = 0;
    }
    private void Start()
    {
        GameManager.instance().updateCoinText(getCoins());
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Get Horizontal Inputs
        input = UnityEngine.Input.GetAxisRaw("Horizontal");
        //Debug Print for Wall Raycast.
        //print(isWalled());
        //Debug Print for Coin Counting
        //print(coins);
        //DASH CODE FROM PA1
        // dash
        //if dash is on cool down, lower the cooldown and update the GUI.
        if (dashCoolDown > 0)
        {
            dashCoolDown -= Time.deltaTime;
            GameManager.instance().updateDashText((int)dashCoolDown);
        }
        //if shift is pressed and the cooldown is less than or equal to 0
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) && dashCoolDown <= 0 && isGrounded() && !isWalled())
        {

            //change animation state to dashing
            //anim.SetBool("isDashing", true);
            //change speed to dash speed
            speed = dashSpeed;
            //set dash time to the set limit
            dashTime = dashTimeLimit;
            //set dashing flag to true to tick the timer down
            dashing = true;
        }
        //if dashTime is still > 0  and the dashing flag is up, increment the dash time down.
        //if the dash time is zero and the player is in dashing state then do the else
        if (dashing && dashTime > 0)
        {
            dashTime -= Time.deltaTime;
        }
        if (dashing && dashTime <= 0)
        {
            //dashing is now false
            dashing = false;
            //the animation ends
            //anim.SetBool("isDashing", false);
            //speed is now default.
            speed = defaultSpeed;
            //the cooldown is reset?s
            dashCoolDown = dashCoolDownLimit;
            GameManager.instance().updateDashText((int)dashCoolDown);
        }

    }
    void FixedUpdate()
    {
        if(input>0||input<0)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
        //print(health);
        //Velocity change from player input.
        if(input>0||input<0)
        {
            forwardVelocity += accelRatePerSec * Time.deltaTime;
            forwardVelocity = Mathf.Min(forwardVelocity, speed);
        }
        else
        {
            forwardVelocity -= accelRatePerSec * Time.deltaTime;
            forwardVelocity = Mathf.Max(forwardVelocity, 0);
        }
        rb.velocity = new Vector2(input * forwardVelocity * Time.deltaTime, rb.velocity.y);
        //Sprite Flipping from in Class in the Volcano Island assignment.
        if (input > 0) //right?
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (input < 0) //left?
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        //Wall jumping mechanics detailed below
        //if the cooldown is greater than 0.2f
        if (wallJumpCooldown > 0.2f)
        {
            //if you are both touching a wall and are not on the ground
            if (isWalled() == true && !isGrounded())
            {
                //don't let the player fall
                //perhaps if you let this be a really small number it would act like popular platformer N++, where you slowly....drag along the wall
                rb.gravityScale = 0;
                //stop the player
                rb.velocity = Vector2.zero;
            }
            else
                rb.gravityScale = 2.5f;
            //if somehow either of the two booleans are not met, return gravity back to normal (I should have made this a variable.)
        }
        else
            wallJumpCooldown += Time.deltaTime;
        //While nothing is going on, count up on the jump cooldown.
        //if space is pressed, jump.
        //needed to specify UnityEngine as my home PC keeps complaining if I just write Input
        if (UnityEngine.Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
    }
    
    //This is where the jump command truly lives.
    private void Jump()
    {   
        //if on the ground, then do the jump, else if on a wall, then reset the jump cooldown to 0, and perform a wall jump.
        if(isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * Time.deltaTime);
        }
        else if(isWalled() == true && !isGrounded())
        {
            wallJumpCooldown = 0f;
            //Sign returns the sign of a number.
            rb.velocity = new Vector2(-Mathf.Sign(this.transform.right.x)*wallJumpOutwardForce, wallJumpUpwardForce);
        }
        
    }
    //maybe ill need this for enemies later...or the coins, we will see.
    //update 03/14/2023: I didn't need this for anything lol. But I did need this *exact* method for the Enemy script.
    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "COIN")
        {
            coins += 1;
            GameManager.instance().updateCoinText((int)getCoins());
        }
        if(collision.tag == "VICTORY")
        {
            GameManager.instance().victoryCanvasSwitch(true);
        }
    }
    public int getCoins()
    {
        return coins;
    }
    //raycast to check if touching the ground
    private bool isGrounded()
    {
        //These BoxCasts from Pandemonium's tutorial are highly useful. A lot more flexible to the problem of edges than just rays. 
        //Seems like it saves a lot of time instead of doing a lot of math calculations for slopes etc.
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f,groundLayer);
        return hit.collider!=null;
    }
    //Raycast to check if touching a wall. It works okay with my strange platform objects I made.
    private bool isWalled()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f,this.transform.right, 0.1f,wallLayer);
        Color hitColor = Color.red;
        if (hit.collider != null)
        {
            hitColor = Color.green;
        }
        //debug raycasts from class.
        //Debug.DrawRay(boxCollider.bounds.center,this.transform.right, hitColor, 0.05f);
        return hit.collider != null;
    }
    //reused takeDamage method from PA1. The enemy sends damage to the player, the player then deducts its own health based on what they enemy claims it did. (sounds like how cheating in FPS games happens lol).
    //since our game is one hit and you die, no need to display this to the user, they'll know when they lost.
    public void takeDamage(int value)
    {
        if ((health -= value) >= 100)
        {
            health = 100;
        }
        else
        {
            health -= value;
        }
        //GameManager.instance().updateHeathText(health);
        if (health <= -0)
        {
            //gameover condition
            //play a sound/music
            //play a particle system
            //show the game over screen
            //use setactive to "destroy" the player
            //enable the game over canvas.
            GameManager.instance().gameOverCanvasSwitch(true);
            this.gameObject.SetActive(false);

        }
    }
}
