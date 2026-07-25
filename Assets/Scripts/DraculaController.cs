using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class DraculaController : MonoBehaviour
{
    public bool playerCanMove = true;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private AudioClip landSfx;

    private AudioSource audioSource;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float jumpDuration = 0.4f;
    [SerializeField][Range(1f, 5f)] private float downGravity = 2.5f;
    [SerializeField] private float terminalVelocity = 20f;

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
    private float jumpVelocity;
    private float upGravity;
    private bool isGrounded;
    private bool jumpHeld;
    private GameObject currentPlatform;
    private GameObject launchedPlatform;
    private bool jumpedSinceLastLanding;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        RecomputeJumpParams();
    }

    void OnValidate()
    {
        RecomputeJumpParams();
    }

    private void RecomputeJumpParams()
    {
        jumpDuration = Mathf.Max(jumpDuration, 0.01f);
        jumpHeight = Mathf.Max(jumpHeight, 0.01f);
        jumpVelocity = 2f * jumpHeight / jumpDuration;
        upGravity = 2f * jumpHeight / (jumpDuration * jumpDuration);
    }

    void Update()
    {
        if (playerCanMove)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            jumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

            if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                && isGrounded && remainingJumps > 0)
            {
                launchedPlatform = currentPlatform;
                jumpedSinceLastLanding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            if (jumpSfx != null) audioSource.PlayOneShot(jumpSfx);
            Debug.Log($"Dracula: Jump from {launchedPlatform?.name} (charges: {remainingJumps})");
                OnJump?.Invoke();
            }
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

        if (rb.linearVelocity.y > 0 && !jumpHeld)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

        float gravMultiplier = rb.linearVelocity.y < 0 ? downGravity : 1f;
        rb.linearVelocity -= Vector2.up * upGravity * gravMultiplier * Time.fixedDeltaTime;

        if (rb.linearVelocity.y < -terminalVelocity)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -terminalVelocity);

        if (!wasGrounded && isGrounded)
        {
            if (jumpedSinceLastLanding)
            {
                bool diffPlatform = launchedPlatform != currentPlatform;
                if (diffPlatform)
                {
                    remainingJumps = Mathf.Max(0, remainingJumps - 1);
                    Debug.Log($"Dracula: Landed on {currentPlatform?.name} (was {launchedPlatform?.name}) \u2014 cost 1 jump, {remainingJumps} remaining");
                }
                else
                {
                    Debug.Log($"Dracula: Landed back on {currentPlatform?.name} \u2014 free landing");
                }
                jumpedSinceLastLanding = false;
            }

            if (landSfx != null) audioSource.PlayOneShot(landSfx);
            OnLanded?.Invoke(currentPlatform);
        }
    }

    public void AddJump(int amount)
    {
        remainingJumps = Mathf.Max(0, remainingJumps + amount);
        Debug.Log($"Dracula: Jumps changed by {amount} \u2192 {remainingJumps}");
    }

    public void MultiplyJumps(int factor)
    {
        remainingJumps *= factor;
        Debug.Log($"Dracula: Jumps multiplied by {factor} \u2192 {remainingJumps}");
    }

    public void WarpTo(Vector3 position, GameObject platform)
    {
        //rb.position = position;
        this.transform.position = position;
        currentPlatform = platform;
        launchedPlatform = null;
        jumpedSinceLastLanding = false;
        Debug.Log($"Dracula: Warped to {position}, platform set to {platform?.name}");
    }

    void LateUpdate()
    {
        if (!isGrounded || currentPlatform == null) return;

        Collider2D platformCol = currentPlatform.GetComponent<Collider2D>();
        if (platformCol == null || !platformCol.enabled) return;

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
