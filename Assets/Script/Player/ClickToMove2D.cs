using UnityEngine;

public class ClickToMove2D : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Update()
    {
        HandleMouseInput();
        Move();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1)) // คลิกขวา
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            // ยิง Raycast เพื่อตรวจสอบว่าคลิกโดนอะไร
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider == null)
            {
                // คลิกนอกแมพ → หยุดเดิน
                isMoving = false;
                return;
            }

            if (hit.collider.CompareTag("Ground"))
            {
                targetPosition = mouseWorldPos;
                isMoving = true;
            }
            else
            {
                // คลิกโดนอย่างอื่นที่ไม่ใช่แมพ → หยุด
                isMoving = false;
            }
        }
    }

    void Move()
    {
        if (!isMoving) return;

        Vector3 nextPos = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // เช็คสิ่งกีดขวางระหว่างทาง
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            (nextPos - transform.position),
            Vector2.Distance(transform.position, nextPos)
        );

        if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
        {
            // ชนสิ่งกีดขวาง → หยุด
            isMoving = false;
            return;
        }

        transform.position = nextPos;

        // ถึงจุดหมายแล้ว
        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            isMoving = false;
        }
    }
}
