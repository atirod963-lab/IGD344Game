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

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            targetPosition = mouseWorldPos;
            isMoving = true;
        }
        else
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
}