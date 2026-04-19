using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 2f;
    float lastClickTime;
    public float clickCooldown = 0.2f;
    bool ignoreClickThisFrame = false;

    void Update()
    {
        if (ignoreClickThisFrame)
        {
            ignoreClickThisFrame = false;
            return;
        }

        if (DialogueManger.Instance != null &&
    (DialogueManger.Instance.IsDialogueActive() || DialogueManger.Instance.JustClosedDialogue()))
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);

            foreach (Collider2D hit in hits)
            {
                IInteract interact = hit.GetComponent<IInteract>();

                if (interact != null)
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);

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

    public void IgnoreClickThisFrame()
    {
        ignoreClickThisFrame = true;
    }
}
