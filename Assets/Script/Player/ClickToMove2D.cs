using UnityEngine;

public class ClickToMove2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float holdThreshold = 0.2f;

    private Vector3 targetPosition;
    private bool isMoving;
    private bool wasMoving; // 🔥 เพิ่มตัวแปรนี้มาเช็คว่า "เฟรมที่แล้วเดินอยู่หรือเปล่า"
    private bool isHolding;
    private float mouseHoldTime;

    private Rigidbody2D rb;

    private Animator animator;
    private Vector2 moveinput;
    public static ClickToMove2D Instance { get; private set; }

    void Awake()
    {
        Instance = this; // เพิ่มแค่นี้
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        HandleMouseInput();
    }

    void FixedUpdate()
    {
        Move();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            mouseHoldTime = 0f;
            isHolding = false;
        }

        if (Input.GetMouseButton(1))
        {
            mouseHoldTime += Time.deltaTime;

            if (mouseHoldTime >= holdThreshold)
            {
                isHolding = true;
                SetTargetToMouse();
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (!isHolding)
                SetTargetToMouse();

            isHolding = false;
        }
    }

    void SetTargetToMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);

        bool foundGround = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                targetPosition = mouseWorldPos;
                isMoving = true;
                foundGround = true;
                break;
            }
        }

        if (!foundGround)
        {
            isMoving = false;
        }
    }

    void Move()
    {
        // 🔥 ระบบเช็คสถานะเพื่อเล่นเสียง/หยุดเสียงเดิน
        if (isMoving && !wasMoving)
        {
            // ถ้าเฟรมนี้เดิน แต่เฟรมที่แล้วไม่ได้เดิน (แปลว่า "เพิ่งเริ่มเดิน")
            if (SoundManager.Instance != null) SoundManager.Instance.PlayLoopingSFX("Player_Walk");
        }
        else if (!isMoving && wasMoving)
        {
            // ถ้าเฟรมนี้หยุด แต่เฟรมที่แล้วเดิน (แปลว่า "เพิ่งหยุดเดิน")
            if (SoundManager.Instance != null) SoundManager.Instance.StopLoopingSFX();
        }

        // อัปเดตสถานะของเฟรมนี้ไปเก็บไว้ใช้เช็คในเฟรมหน้า
        wasMoving = isMoving;

        if (!isMoving)
        {
            animator.SetBool("IsWalking", false);
            return;
        }

        Vector2 currentPos = rb.position;
        Vector2 nextPos = Vector2.MoveTowards(
            currentPos,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        moveinput = (targetPosition - (Vector3)currentPos).normalized;

        rb.MovePosition(nextPos);

        animator.SetBool("IsWalking", true);
        animator.SetFloat("InputX", moveinput.x);
        animator.SetFloat("InputY", moveinput.y);

        if (!isHolding && Vector2.Distance(rb.position, targetPosition) < 0.05f)
        {
            isMoving = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            isMoving = false;
        }
    }

    public void StopMovementImmediately()
    {
        isMoving = false;
        isHolding = false;
        targetPosition = rb.position;

        // 🔥 สำคัญ: ต้องสั่งหยุดเสียงด้วยเผื่อตอนโดนบังคับให้หยุดเดิน (เช่น เข้าฉากต่อสู้)
        if (SoundManager.Instance != null) SoundManager.Instance.StopLoopingSFX();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}