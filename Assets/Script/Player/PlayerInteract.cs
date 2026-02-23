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
            Vector2 mousePos =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit == null) return;

            // 🔹 ต้องเป็น NPC (หรือวัตถุที่ interact ได้)
            IInteract interact = hit.GetComponent<IInteract>();
            if (interact == null) return;

            // 🔹 เช็คระยะ
            float distance =
                Vector2.Distance(transform.position, hit.transform.position);

            if (distance > interactDistance) return;

            // ✅ เรียก interact
            interact.Interact();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
