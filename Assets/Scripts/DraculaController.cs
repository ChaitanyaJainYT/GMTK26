using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class DraculaController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.15f;

    [Header("Jump Charges")]
    [SerializeField] private int remainingJumps = 3;

    public int RemainingJumps => remainingJumps;
    public GameObject CurrentPlatform { get => currentPlatform; set => currentPlatform = value; }
    public bool HasKey { get; set; }

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isGrounded;
    private GameObject currentPlatform;
    private GameObject launchedPlatform;
    private bool jumpedSinceLastLanding;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded && remainingJumps > 0)
        {
            launchedPlatform = currentPlatform;
            jumpedSinceLastLanding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        bool wasGrounded = isGrounded;

        Vector2 checkPos = groundCheckPoint
            ? groundCheckPoint.position
            : (Vector2)transform.position + col.offset + Vector2.down * (col.bounds.extents.y + 0.05f);

        Collider2D hit = Physics2D.OverlapCircle(checkPos, groundCheckRadius, platformLayer);

        isGrounded = hit != null;
        currentPlatform = hit ? hit.gameObject : null;

        if (!wasGrounded && isGrounded && jumpedSinceLastLanding)
        {
            if (launchedPlatform != currentPlatform)
            {
                remainingJumps = Mathf.Max(0, remainingJumps - 1);
            }
            jumpedSinceLastLanding = false;
        }
    }

    public void AddJump(int amount)
    {
        remainingJumps = Mathf.Max(0, remainingJumps + amount);
    }

    public void MultiplyJumps(int factor)
    {
        remainingJumps *= factor;
    }

    public void WarpTo(Vector3 position, GameObject platform)
    {
        rb.position = position;
        currentPlatform = platform;
        launchedPlatform = null;
        jumpedSinceLastLanding = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
