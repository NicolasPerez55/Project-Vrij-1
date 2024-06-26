using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Components & Prefabs")]
    public Rigidbody2D rb;
    public BoxCollider2D col;
    [SerializeField] private LayerMask whatIsGround;
    public SceneController scene;
    [SerializeField] public Animator animator;
    [SerializeField] Spotting spotting;
    public GameObject customTagPrefab;

    [Space, Header("Movement Modifiers")]
    public float speed;
    private float moveInput;
    public bool facingRight = true;

    // Jump-related variables
    [Space, Header("Jump Modifiers")]
    public float jumpForce;
    public float coyote;
    private float coyoteTime = 0.15f;
    public float jumpBuffer;
    private float jumpBufferTime = 0.15f;
    private float jumpCut = 0.4f;
    public float floorDetectionSize = 0.1f;

    [Space, Header("Meta")]
    [Tooltip("1 = Physical Form, 2 = Graffiti Form")]
    public int characterType = 0; //1 = real player, 2 = graffiti player
    public bool inFrontOfWall = true;
    private void Update()
    {
        if (scene.currentCutscene == 0) CheckJump();
        //Swapping character (shift)
        if ((Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) && scene.swapCooldown <= 0 && inFrontOfWall && scene.canSwap)
        {
            scene.switchCharacter();
        }
        
        //Make tag
        if (Input.GetKeyDown(KeyCode.T) && inFrontOfWall && characterType == 1 && scene.currentCutscene == 0)
        {
            GameObject tag = Instantiate(customTagPrefab, gameObject.transform);
            tag.transform.localScale *= 0.3f;
            tag.transform.parent = null;
            if (scene.inFinalRoom) //trigger ending
            {
                scene.finalCutscene();
            }
        }
    }

    private void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (scene.currentCutscene == 0) AnimatorMovement();
        if (scene.currentCutscene == 0) rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        changeFacingDirection();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spotter") && scene.playerActive == 1)
        {
            spotting.PlayerSpotted();
        }

        switch (collision.gameObject.layer)
        {
            case 11: //background layer
                scene.wallChange(true);
                break;
            case 12: //trigger
                Debug.Log("Trigger entered");
                collision.gameObject.GetComponent<TriggerScript>().activate();
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 11: //background layer
                scene.wallChange(false);
                break;
            default:
                break;
        }
    }

    // Checks if the player can jump and how high they jump
    private void CheckJump()
    {
        // The coyote time and jump buffer window are counting down every frame
        coyote -= Time.deltaTime;
        jumpBuffer -= Time.deltaTime;

        // The coyote time is set to 0.15s when grounded, in that time the player can still jump,
        // even if they are not grounded anymore (e.g. when they run off a platform)
        if (IsGrounded() && Mathf.Abs(rb.velocity.y) < 0.01)
        {
            coyote = coyoteTime;
            animator.SetBool("isJumping", false);
        }

        // When the player presses jump in the air, they get a 0.15s window of being early with jumping
        // to still jump when they land.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBuffer = jumpBufferTime;
        }

        // The player can only jump when both the jump buffer and the coyote values are above zero
        if (jumpBuffer > 0 && coyote > 0)
        {
            Debug.Log("JUMPED");
            animator.SetBool("isJumping", true);
            rb.velocity = Vector2.up * jumpForce;
            coyote = 0;
            jumpBuffer = 0;
        }

        // When releasing the jump key, the player's upward velocity gets cut,
        // so they don't jump as high as a full jump
        if (Input.GetKeyUp(KeyCode.Space))
        {
            coyote = 0;
            jumpBuffer = 0;
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCut);
            }
        }
    }

    // This function uses a BoxCast to determine whether the player is grounded;
    // returns true if grounded, false if not grounded.
    public bool IsGrounded()
    {
        return Physics2D.BoxCast((col.bounds.center), col.bounds.size * floorDetectionSize, 0f, Vector2.down, 1, whatIsGround);
    }

    public void changeFacingDirection()
    {
        if ((!facingRight && moveInput > 0 || facingRight && moveInput < 0) && scene.currentCutscene == 0)
        {
            facingRight = !facingRight;
            Vector2 localscale = transform.localScale;
            localscale.x *= -1f;
            transform.localScale = localscale;
            //customTagPrefab.transform.localScale = new Vector3(customTagPrefab.transform.localScale.x * -1, customTagPrefab.transform.localScale.y, customTagPrefab.transform.localScale.z);
        }
    }

    void AnimatorMovement()
    {
        animator.SetFloat("xVelocity", Mathf.Abs(moveInput));
        animator.SetFloat("yVelocity", rb.velocity.y);
        if (rb.velocity.y < 0 && animator.GetBool("isJumping") == false)
        {
            animator.SetBool("isJumping", true);
        }
    }
}