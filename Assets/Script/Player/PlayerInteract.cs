using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 2f;

    void Update()
    {
        // ถ้ากำลังคุยอยู่ → ห้ามคลิกอะไร
        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsDialogueActive())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit == null) return;

            // 🔹 เช็ค Tag NPC ก่อน
            if (!hit.CompareTag("NPC")) return;

            IInteract interact = hit.GetComponent<IInteract>();
            if (interact == null) return;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance > interactDistance) return;

            interact.Interact();
        }
    }
}