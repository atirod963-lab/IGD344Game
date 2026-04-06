using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public void OnClick_Continue()
    {
        SaveSystem.Instance.LoadGame();
    }

    public void OnClick_MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}