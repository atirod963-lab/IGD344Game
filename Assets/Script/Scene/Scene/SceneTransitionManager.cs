using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("UI")]
    public Image transitionImage;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ✅ ปิด Transition ตั้งแต่เริ่ม
        InitTransitionUI();
    }

    void InitTransitionUI()
    {
        if (transitionImage == null) return;

        transitionImage.gameObject.SetActive(false);
        transitionImage.raycastTarget = false;

        Color c = transitionImage.color;
        transitionImage.color = new Color(c.r, c.g, c.b, 0f);
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
        // 🛑 เปิด UI เพื่อเริ่ม Transition
        transitionImage.gameObject.SetActive(true);
        transitionImage.raycastTarget = true;

        // ----- เข้า (Fade In) -----
        transitionImage.sprite = enterSprite;
        yield return StartCoroutine(Fade(0f, 1f, duration));

        SceneManager.LoadScene(sceneName);
        yield return null;

        // ----- ออก (Fade Out) -----
        transitionImage.sprite = exitSprite;
        yield return StartCoroutine(Fade(1f, 0f, duration));

        // ✅ ปลดบล็อกเมาส์
        transitionImage.raycastTarget = false;

        // ✅ ปิด UI หลังใช้งาน (แนะนำ)
        transitionImage.gameObject.SetActive(false);
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

        // กันค่าเพี้ยน
        transitionImage.color = new Color(c.r, c.g, c.b, to);
    }
}