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
    [SerializeField] Animator animator;
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
        CheckJump();
        //Swapping character (shift)
        if ((Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) && scene.swapCooldown <= 0 && inFrontOfWall)
        {
            scene.switchCharacter();
        }
        //Swapping graffiti type (control) (CURRENTLY UNUSED)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            //if (currentGraffitiType == 1) currentGraffitiType = 2;
            //else if (currentGraffitiType == 2) currentGraffitiType = 1;
            //scene.updateSelectionUI(currentGraffitiType);
        }
        //Make Graffiti (E) (CURRENTLY UNUSED)
        if (Input.GetKeyDown(KeyCode.E))
        {
            //scene.makeGraffiti(currentGraffitiType);
        }
        if (Input.GetKeyDown(KeyCode.T) && inFrontOfWall && characterType == 1)
        {
            GameObject tag = Instantiate(customTagPrefab, gameObject.transform);
            tag.transform.localScale *= 0.3f;
            tag.transform.parent = null;
        }
        //Pause/Unpause game (Esc / Backspace)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            scene.pauseGame();
        }
    }

    private void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        AnimatorMovement();
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
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
        if (!facingRight && moveInput > 0 || facingRight && moveInput < 0)
        {
            facingRight = !facingRight;
            Vector2 localscale = transform.localScale;
            localscale.x *= -1f;
            transform.localScale = localscale;
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