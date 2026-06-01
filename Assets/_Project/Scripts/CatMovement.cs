using UnityEngine;

public class CatMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float crawlSpeed = 1.5f;
    public float jumpForce = 8f;

    public float turnDuration = 0.35f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float moveInput;
    private bool isGrounded;
    private bool isRunning;
    private bool isCrawling;

    private bool facingRight = true;
    private bool isTurning = false;
    private bool pendingFacingRight;
    private float turnTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        isRunning = Input.GetKey(KeyCode.LeftShift);
        isCrawling = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);

        if (!isTurning && moveInput != 0)
        {
            bool wantsRight = moveInput > 0;

            if (wantsRight != facingRight)
            {
                StartTurn(wantsRight);
                moveInput = 0f;
            }
        }


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrawling)
        {
            animator.ResetTrigger("JumpTrigger");
            animator.SetTrigger("JumpTrigger");

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (isTurning)
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0f)
            {
                FinishTurn();
            }
        }

        animator.SetFloat("Speed", isTurning ? 0f : Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsCrawling", isCrawling);
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        float currentSpeed = walkSpeed;

        if (isRunning)
            currentSpeed = runSpeed;

        if (isCrawling)
            currentSpeed = crawlSpeed;

        if (isTurning)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    private void StartTurn(bool wantsRight)
    {
        isTurning = true;
        pendingFacingRight = wantsRight;
        turnTimer = turnDuration;

        spriteRenderer.flipX = facingRight;
        animator.SetFloat("Speed", 0f);


        if (wantsRight)
        {
            animator.ResetTrigger("TurnRightTrigger");
            animator.SetTrigger("TurnRightTrigger");
        }
        else
        {
            animator.ResetTrigger("TurnLeftTrigger");
            animator.SetTrigger("TurnLeftTrigger");

        }

        Debug.Log("Turn wantsRight = " + wantsRight);
    }

    private void FinishTurn()
    {
        facingRight = pendingFacingRight;

        spriteRenderer.flipX = !facingRight;

        isTurning = false;
    }

}