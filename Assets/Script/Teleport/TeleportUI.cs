using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportUI : MonoBehaviour
{
    public static TeleportUI Instance;

    public TeleportButton[] buttons;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ถ้ามีตัวเก่าอยู่แล้ว ให้ลบทิ้ง
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        if (gameObject.activeSelf) return;

        gameObject.SetActive(true);
        Time.timeScale = 0f;

        string currentScene = SceneManager.GetActiveScene().name;
        foreach (var btn in buttons)
        {
            btn.Setup(currentScene);
        }
    }

    public void Close()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}