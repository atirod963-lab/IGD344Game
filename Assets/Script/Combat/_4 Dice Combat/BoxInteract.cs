using UnityEngine;
using UnityEngine.EventSystems;

public class BoxInteract : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Animator animator; // 🌟 เพิ่มตัวควบคุมแอนิเมชัน

    [HideInInspector] public bool hasFinishedShaking = false;
    private bool isDragging = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        animator = GetComponent<Animator>(); // ดึง Animator ที่แปะอยู่บนกล่องมาใช้
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (hasFinishedShaking) return;

        isDragging = true;

        // 🌟 สั่งให้ Animator เข้าสู่โหมด "สั่น"
        if (animator != null) animator.SetBool("isShaking", true);

        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            rectTransform.position = eventData.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;

        // 🌟 สั่งให้ Animator กลับสู่โหมด "อยู่นิ่งๆ"
        if (animator != null) animator.SetBool("isShaking", false);

        transform.localScale = Vector3.one;
        hasFinishedShaking = true;
    }

    public void ResetBox()
    {
        hasFinishedShaking = false;
        isDragging = false;
        if (animator != null) animator.SetBool("isShaking", false);
    }
}