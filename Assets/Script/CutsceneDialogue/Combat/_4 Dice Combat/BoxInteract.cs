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

        // 🔥 เพิ่มคำสั่งนี้เข้าไปครับ! (คุณลืมใส่บรรทัดนี้ในรอบที่แล้ว)
        if (SoundManager.Instance != null) SoundManager.Instance.PlayLoopingSFX("4Dice_Roll");

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
        if (animator != null) animator.SetBool("isShaking", false);

        // 🔥 เพิ่มคำสั่งหยุดเสียงตรงนี้
        if (SoundManager.Instance != null) SoundManager.Instance.StopLoopingSFX();

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