using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private string saveFilePath;

    void Awake()
    {
        // ทำให้มี SaveSystem แค่ตัวเดียว และไม่โดนทำลายตอนย้ายฉาก
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // กำหนดที่อยู่ไฟล์เซฟในเครื่องคอม/มือถือ
        saveFilePath = Application.persistentDataPath + "/gamesave.json";
    }

    // 🔴 เรียกใช้เมื่อเดินไปแตะจุดเซฟ
    public void SaveGame(BaseUnit player, FateInventory inventory)
    {
        GameSaveData data = new GameSaveData();

        // บันทึกฉากและตำแหน่ง
        data.sceneName = SceneManager.GetActiveScene().name;
        data.px = player.transform.position.x;
        data.py = player.transform.position.y;
        data.pz = player.transform.position.z;

        // บันทึกสเตตัส
        data.level = player.currentLevel;
        data.exp = player.currentExp;
        data.maxHp = player.maxHp;
        data.atk = player.atk;
        data.def = player.def;
        data.spd = player.spd;
        data.luck = player.luck;

        // บันทึกชื่อเหรียญ (ถ้ามีกระเป๋า)
        if (inventory != null)
        {
            foreach (var coin in inventory.ownedCoins)
            {
                data.ownedCoinNames.Add(coin.coinName);
            }
            if (inventory.CurrentEquipped != null)
            {
                data.equippedCoinName = inventory.CurrentEquipped.coinName;
            }
        }

        // แปลงข้อมูลเป็น JSON แล้วเขียนลงไฟล์
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"💾 <color=green>เซฟเกมสำเร็จ!</color> บันทึกไว้ที่: {saveFilePath}");
    }

    // 🟢 เรียกใช้ตอนกด Continue หน้าเมนู หรือ ตอนตาย
    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("❌ ไม่พบไฟล์เซฟ!");
            return;
        }

        // อ่านไฟล์ JSON กลับมาเป็นข้อมูล
        string json = File.ReadAllText(saveFilePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        Debug.Log("🔄 กำลังโหลดเกม...");

        // โหลดฉากที่เซฟไว้
        SceneManager.LoadScene(data.sceneName);

        // รอให้ฉากโหลดเสร็จก่อนค่อยยัดค่าใส่ตัวละคร
        StartCoroutine(ApplySaveData(data));
    }

    private System.Collections.IEnumerator ApplySaveData(GameSaveData data)
    {
        // รอ 1 เฟรมให้ฉากโหลดเสร็จสมบูรณ์
        yield return null;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            // จับวางตำแหน่งเดิม
            playerObj.transform.position = new Vector3(data.px, data.py, data.pz);

            // คืนค่าสเตตัส
            BaseUnit player = playerObj.GetComponent<BaseUnit>();
            if (player != null)
            {
                player.currentLevel = data.level;
                player.currentExp = data.exp;
                player.maxHp = data.maxHp;
                player.hp = data.maxHp; // เกิดใหม่เลือดเต็ม
                player.atk = data.atk;
                player.def = data.def;
                player.spd = data.spd;
                player.luck = data.luck;

                Debug.Log("✨ คืนชีพที่จุดเซฟเรียบร้อย!");
            }
        }
    }
}