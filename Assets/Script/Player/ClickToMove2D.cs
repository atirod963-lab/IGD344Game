using UnityEngine;

public class ClickToMove2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float holdThreshold = 0.2f;

    private Vector3 targetPosition;
    private bool isMoving;
    private bool isHolding;
    private float mouseHoldTime;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // เพิ่ม: ถ้ากำลังสู้ (เกมหยุด หรือ BattleManager สั่งปิด) ไม่ต้องรับ Input
        // แต่ถ้าใช้วิธีปิด Script ใน EnemyTrigger แล้ว บรรทัดนี้ไม่ต้องมีก็ได้
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

        // 🔥 แก้ตรงนี้: เปลี่ยนจาก Raycast ธรรมดา เป็น RaycastAll (เจาะทะลุทุกอย่าง)
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);

        bool foundGround = false;

        // วนลูปดูว่า ในบรรดาสิ่งที่คลิกโดน มีอันไหนเป็น "Ground" ไหม?
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                targetPosition = mouseWorldPos;
                isMoving = true;
                foundGround = true;
                break; // เจอพื้นแล้ว หยุดหา
            }
        }

        // ถ้าคลิกไม่โดนพื้นเลย (เช่น คลิกออกนอกแมพ) ถึงค่อยหยุด
        if (!foundGround)
        {
            isMoving = false;
        }
    }

    void Move()
    {
        if (!isMoving) return;

        Vector2 nextPos = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPos);

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

    // 🔥 ฟังก์ชันสำหรับให้ EnemyTrigger เรียกใช้ (Unity 6)
    public void StopMovementImmediately()
    {
        isMoving = false;
        isHolding = false;
        targetPosition = rb.position;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Unity 6 ใช้ linearVelocity ถูกต้องแล้วครับ
        }
    }
}