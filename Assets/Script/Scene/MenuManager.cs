using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // ปุ่มเริ่มเกม
    public void StartGame()
    {
        // ใส่ชื่อ Scene เกมหลักของคุณ
        SceneManager.LoadScene("MainGame");
    }

    // ปุ่มออกเกม
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}