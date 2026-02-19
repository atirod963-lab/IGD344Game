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

          
        }
        else
        {
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
        // ----- รูปตอนเข้า -----
        transitionImage.sprite = enterSprite;
        yield return StartCoroutine(Fade(0f, 1f, duration));

        SceneManager.LoadScene(sceneName);
        yield return null;

        // ----- รูปตอนออก -----
        transitionImage.sprite = exitSprite;
        yield return StartCoroutine(Fade(1f, 0f, duration));
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
