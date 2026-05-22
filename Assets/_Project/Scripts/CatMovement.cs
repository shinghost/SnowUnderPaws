using UnityEngine;

public class CatMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float jumpForce = 8f;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        isRunning = Input.GetKey(KeyCode.LeftShift);
        isCrawling = Input.GetKey(KeyCode.LeftControl);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrawling)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (moveInput > 0)
            spriteRenderer.flipX = true;
        else if (moveInput < 0)
            spriteRenderer.flipX = false;

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
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
            currentSpeed = walkSpeed * 0.5f;

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }
}