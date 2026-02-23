using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 2f;

    void Update()
    {
        // 🔒 ถ้ากำลังคุย → ห้าม interact
        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsDialogueActive())
            return;

        // 🖱️ กันคลิกโดน UI
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // เปลี่ยนมาใช้ OverlapPointAll เพื่อกวาดหาทุก Collider ตรงที่เมาส์คลิก
            Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);

            foreach (Collider2D hit in hits)
            {
                IInteract interact = hit.GetComponent<IInteract>();

                // ถ้าเจอ Object ที่มีระบบ Interact
                if (interact != null)
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);

                    // ถ้าระยะถึง ก็เก็บเลย แล้วหยุดการค้นหา
                    if (distance <= interactDistance)
                    {
                        interact.Interact();
                        return;
                    }
                }
            }

        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
