using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("UI")]
    public Image transitionImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ⭐ สำคัญ: พา Canvas ข้ามฉากไปด้วย
            if (transitionImage != null && transitionImage.canvas != null)
            {
                DontDestroyOnLoad(transitionImage.canvas.gameObject);
            }

            // ปิดการบังเมาส์ไว้ก่อนตอนเริ่มเกม (กันเหนียว)
            if (transitionImage != null)
            {
                transitionImage.raycastTarget = false;
            }
        }
        else
        {
            // 🔥 แก้บั๊กซ่อนเร้น: ถ้า Manager เป็นตัวซ้ำ ต้องทำลาย Canvas ของมันทิ้งตามไปด้วย!
            // ไม่งั้น Canvas จะลอยค้างอยู่ในฉากและกางกำแพงล่องหนทับของจริง
            if (transitionImage != null && transitionImage.canvas != null)
            {
                Destroy(transitionImage.canvas.gameObject);
            }
            Destroy(gameObject);
        }
    }

    public void LoadScene(
        string sceneName,
        float duration,
        Sprite enterSprite,
        Sprite exitSprite
    )
    {
        StartCoroutine(LoadSceneRoutine(sceneName, duration, enterSprite, exitSprite));
    }

    IEnumerator LoadSceneRoutine(
        string sceneName,
        float duration,
        Sprite enterSprite,
        Sprite exitSprite
    )
    {
        // 🛑 1. ก่อนเริ่มเฟดจอดำ: เปิดให้ Image กลับมาบล็อกเมาส์ (กันผู้เล่นกดมั่วตอนโหลดฉาก)
        transitionImage.gameObject.SetActive(true);
        transitionImage.raycastTarget = true;

        // ----- รูปตอนเข้า -----
        transitionImage.sprite = enterSprite;
        yield return StartCoroutine(Fade(0f, 1f, duration));

        SceneManager.LoadScene(sceneName);
        yield return null;

        // ----- รูปตอนออก -----
        transitionImage.sprite = exitSprite;
        yield return StartCoroutine(Fade(1f, 0f, duration));

        // ✅ 2. เฟดจอใสเสร็จแล้ว: ปิดบล็อกเมาส์! (ให้เมาส์คลิกทะลุไปหาผัก/NPC ได้)
        transitionImage.raycastTarget = false;

        // (ตัวเลือกเสริม: จะสั่งปิด GameObject ไปเลยก็ได้เพื่อประหยัดทรัพยากรการเรนเดอร์ UI)
        // transitionImage.gameObject.SetActive(false); 
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        Color c = transitionImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            transitionImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
    }
}