using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D col;
    [SerializeField] private LayerMask whatIsGround;

    public SceneController scene;

    public float speed;
    private float moveInput;

    // Jump-related variables
    public float jumpForce;
    public float coyote;
    private float coyoteTime = 0.15f;
    public float jumpBuffer;
    private float jumpBufferTime = 0.15f;
    private float jumpCut = 0.4f;

    public int characterType = 0; //1 = real player, 2 = graffiti player
    //public int currentGraffitiType = 1; //1 = platform, 2 = warp
    private void Update()
    {
        CheckJump();
        //Swapping character (shift)
        if ((Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) && scene.swapCooldown <= 0)
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
        //Pause/Unpause game (Esc / Backspace)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            scene.pauseGame();
        }
    }

    private void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    //Boosts the player up a bit, stalling them briefly in the air. Was used when placing a platform in the air in my original demo, keeping it in for now -Nico
    /*public void BoostUp()
    {
        rb.velocity = Vector2.up * 2;
    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
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
        return Physics2D.BoxCast((col.bounds.center), col.bounds.size * 0.1f, 0f, Vector2.down, 1, whatIsGround);
    }
}