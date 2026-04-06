using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MenuManager : MonoBehaviour
{
    // ปุ่มเริ่มเกม
    public void StartGame()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/gamesave.json"))
        {
            SaveSystem.Instance.LoadGame(); // 👉 โหลดเซฟล่าสุด
        }
        else
        {
            SceneManager.LoadScene("Map1"); // 👉 ยังไม่มีเซฟ
        }
    }
    public void NewGame()
    {
        string path = Application.persistentDataPath + "/gamesave.json";

        if (File.Exists(path))
        {
            File.Delete(path); // 🧹 ลบเซฟเก่า
            Debug.Log("🗑️ ลบเซฟเก่าแล้ว");
        }

        SceneManager.LoadScene("Map1"); // 👉 เริ่มเกมใหม่จริง ๆ
    }


    // ปุ่มออกเกม
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}