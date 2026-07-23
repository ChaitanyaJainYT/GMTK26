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

    public event System.Action<GameObject> OnLanded;
    public event System.Action OnJump;

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
            Debug.Log($"Dracula: Jump from {launchedPlatform?.name} (charges: {remainingJumps})");
            OnJump?.Invoke();
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

        if (!wasGrounded && isGrounded)
        {
            if (jumpedSinceLastLanding)
            {
                bool diffPlatform = launchedPlatform != currentPlatform;
                if (diffPlatform)
                {
                    remainingJumps = Mathf.Max(0, remainingJumps - 1);
                    Debug.Log($"Dracula: Landed on {currentPlatform?.name} (was {launchedPlatform?.name}) — cost 1 jump, {remainingJumps} remaining");
                }
                else
                {
                    Debug.Log($"Dracula: Landed back on {currentPlatform?.name} — free landing");
                }
                jumpedSinceLastLanding = false;
            }

            OnLanded?.Invoke(currentPlatform);
        }
    }

    public void AddJump(int amount)
    {
        remainingJumps = Mathf.Max(0, remainingJumps + amount);
        Debug.Log($"Dracula: Jumps changed by {amount} → {remainingJumps}");
    }

    public void MultiplyJumps(int factor)
    {
        remainingJumps *= factor;
        Debug.Log($"Dracula: Jumps multiplied by {factor} → {remainingJumps}");
    }

    public void WarpTo(Vector3 position, GameObject platform)
    {
        rb.position = position;
        currentPlatform = platform;
        launchedPlatform = null;
        jumpedSinceLastLanding = false;
        Debug.Log($"Dracula: Warped to {position}, platform set to {platform?.name}");
    }

    void LateUpdate()
    {
        if (!isGrounded || currentPlatform == null) return;

        Collider2D platformCol = currentPlatform.GetComponent<Collider2D>();
        if (platformCol == null) return;

        Bounds platformBounds = platformCol.bounds;
        float playerHalf = col.bounds.extents.x;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, platformBounds.min.x + playerHalf, platformBounds.max.x - playerHalf);
        transform.position = pos;
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
