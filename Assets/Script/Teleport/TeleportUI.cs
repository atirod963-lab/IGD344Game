using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportUI : MonoBehaviour
{
    public static TeleportUI Instance;

    public TeleportButton[] buttons;

    private void Awake()
    {

        Instance = this;
      
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