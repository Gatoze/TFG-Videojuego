using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias a objetos")]
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;

    [Header("Dash")]
    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float dashPower =40f;
    private float dashTime = 0.2f;
    private float cooldownDash = 1f;

    [Header("Salto y Doble salto")]
    private bool doubleJump;
    private bool air;

    [Header("Horizontal")]
    private float horizontal = 0f;
    private bool isFacingRight = true;

    [Header("Coyote time")]
    private float coyoteTime = 0.3f;
    private float coyoteTimeCounter;

    [Header("bufferTime")]
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Wall Sliding / Wall jump")]
    private bool iisWallSliding;
    private bool isWallJumping;
    private float WallSlidingSpeed = 2f;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);
    
   
    [SerializeField] private Transform WallCheck;
    [SerializeField] private LayerMask WallLayer;
    [SerializeField] private float movespeed = 7f;
    [SerializeField] private float jumpforce = 14f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private TrailRenderer tr;


    private enum Movimiento { idle, running, jumping, falling, wallSliding }


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame
    private void Update()
    {
        if (isDashing)
        {
            return;
        }
        //Movimiento horizontal del personaje
        horizontal = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontal * movespeed, rb.velocity.y);

        //Coyote Time (saltar 0.2 segundos despues de tocar el suelo)
        if (isGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            doubleJump = true;
            air = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        //JumpBufferTime
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        //Salto y doble salto 
        if(isGrounded() && !Input.GetButton("Jump"))
        {
            doubleJump = false;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            jumpBufferCounter = 0f;
            
            if (isGrounded() || isWalled())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpforce);
                Debug.Log(doubleJump);
                doubleJump = true;
                air = true;
                Debug.Log(doubleJump);
            }
            else
            if (Input.GetButtonDown("Jump") && doubleJump || isWalled())
            {
                Debug.Log(doubleJump);
                rb.velocity = new Vector2(rb.velocity.x, jumpforce);
                doubleJump = !doubleJump;
                coyoteTimeCounter = 0f;
            }
        }

        //Para hacerlo reactivo al soltar la tecla de salto

        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // La función GetButtonDown llama directamente a los keyBinds de unity donde las teclas ya están mapeadas y con un nombre que se aplicará 
        // entre comillas.
        //Dash 
        if (Input.GetButtonDown("Fire3") && canDash)
        {
            StartCoroutine(Dash());
        }

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }
        

        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * movespeed, rb.velocity.y);
        }
    }
    private void UpdateAnimationState()
    {
        //animaciones de movimiento
        Movimiento state;
        if (horizontal > 0f)
        {
            state = Movimiento.running;
          //  sprite.flipX = false;
            
        }
        else if (horizontal < 0f)
        {
            state = Movimiento.running;
           // sprite.flipX = true;

        }
        else
        {
            state = Movimiento.idle;
        }

        if(rb.velocity.y > .1f)
        {
            state = Movimiento.jumping;
        }
        else 
        if (rb.velocity.y < -.1f)
        {
            state = Movimiento.falling;
        }
        if (isWalled())
        {
            state = Movimiento.wallSliding;
        }
        
        //Debug.Log(state);
        anim.SetInteger("state", (int)state);
    }

    /*
     * Metodo que se encarga de girar al personaje 
     **/
    private void Flip()
    {
        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 vector = transform.localScale;
            vector.x *= -1f;
            transform.localScale = vector;
        }
    }

    
    //Evitar saltar todo el tiempo
    private bool isGrounded()
    {

        RaycastHit2D raycastHit2 = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, ground);
        
        float extraTest = .01f;
        RaycastHit2D raycastHit = Physics2D.Raycast(coll.bounds.center, Vector2.down, coll.bounds.extents.y + extraTest, ground);
        /*
        Color rayColor;
        if (raycastHit2.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(coll.bounds.center, Vector2.down * (coll.bounds.extents.y + extraTest), rayColor);
        Debug.Log(raycastHit2.collider);
        */
        return raycastHit2.collider != null;
        
    }
    //Desplazamiento rapido lateral 
    private IEnumerator Dash()
    {
        anim.SetInteger("state", 0);
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        if (horizontal > 0f)
        {
            rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        }
        else if(horizontal < 0f)
        {
            rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        }
        tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(cooldownDash);
        canDash = true;
    }
    //Comprueba si esta en una pared 
    private bool isWalled()
    {
        return Physics2D.OverlapCircle(WallCheck.position, 0.2f, WallLayer);
    }

    // Permite deslizarse en una pared 
    private void WallSlide()
    {
        if(isWalled() && !isGrounded() && horizontal != 0f)
        {
            iisWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallSlidingSpeed, float.MaxValue));
        }
        else
        {
            iisWallSliding = false;
        }
    }
    // Permite saltar despues de deslizarse en una pared
    private void WallJump()
    {
        if (iisWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
        Invoke(nameof(StopWallJumping), wallJumpingDuration);
    }
    // hace que no puedas saltar desde la pared.
    private void StopWallJumping()
    {
        isWallJumping = false;
    }
}
