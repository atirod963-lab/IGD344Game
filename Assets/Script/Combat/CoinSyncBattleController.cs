using UnityEngine;
using System.Collections;
using TMPro; // 🔥 Library สำหรับจัดการ TextMeshPro

public class CoinSyncBattleController : MonoBehaviour
{
    public string targetTag = "Enemy";
    private BaseUnit myStats;

    [Header("UI Settings")]
    public GameObject numberInputPanel; // ลาก Panel ที่รวมปุ่มกดตัวเลขมาใส่
    public TextMeshProUGUI numberDisplayText; // ลาก Text ที่จะใช้โชว์ตัวเลขที่กดมาใส่

    // 🔥 1. เพิ่มตัวแปรมารับ Text ที่จะใช้แสดงเลขของศัตรู
    public TextMeshProUGUI enemyNumberDisplayText;

    private string currentInputString = "";
    private int playerChosenNumber = -1;

    void Start() => myStats = GetComponent<BaseUnit>();

    public IEnumerator CoinBattleRoutine(BaseUnit target)
    {
        Time.timeScale = 0;
        Debug.Log($"<color=yellow>=== [PAUSED] เริ่มการเดิมพันโดย {myStats.unitName} ===</color>");

        // รีเซ็ตค่าก่อนเริ่ม
        playerChosenNumber = -1;
        currentInputString = "";
        UpdateDisplayText();

        // 🔥 รีเซ็ตข้อความของศัตรูตอนเปิดพาเนล
        if (enemyNumberDisplayText != null)
        {
            enemyNumberDisplayText.gameObject.SetActive(true);
            enemyNumberDisplayText.text = "Enemy กำลังตัดสินใจ..."; // ให้ดูมีอะไรขยับระหว่างรอเราพิมพ์
        }

        // เปิดหน้าต่าง UI ทายเลข
        if (numberInputPanel != null) numberInputPanel.SetActive(true);

        // โค้ดจะหยุดรออยู่ตรงนี้ จนกว่าตัวแปร playerChosenNumber จะถูกเปลี่ยนค่าโดยปุ่ม "ตกลง"
        yield return new WaitUntil(() => playerChosenNumber != -1);

        // 🔥 1. พอเรากดยืนยัน ให้คำนวณเลขของศัตรูทันที
        int enemyChosenNumber = DecideEnemyNumber(playerChosenNumber);

        // 🔥 2. อัปเดต Text ในพาเนลให้โชว์เลขของศัตรู
        if (enemyNumberDisplayText != null)
        {
            enemyNumberDisplayText.text = "<color=red>Enemy เลือก: " + enemyChosenNumber.ToString() + "</color>";
        }

        // 🔥 3. หน่วงเวลา 2 วินาที เพื่อให้ผู้เล่นอ่านเลขศัตรูทัน (ใช้ Realtime เพราะ TimeScale = 0 อยู่)
        yield return new WaitForSecondsRealtime(2f);

        // 🔥 4. ปิดหน้าต่าง UI เมื่อดูเลขเสร็จ
        if (numberInputPanel != null) numberInputPanel.SetActive(false);

        Time.timeScale = 1;

        if (myStats == null || target == null || playerChosenNumber == -1) yield break;

        // เข้าสู่ระบบทอยเหรียญตามปกติ
        int actualTries = 0;
        bool isSynchronized = false;

        int pLuck = gameObject.CompareTag("Player") ? myStats.luck : target.luck;
        int eLuck = gameObject.CompareTag("Enemy") ? myStats.luck : target.luck;

        while (!isSynchronized)
        {
            actualTries++;
            isSynchronized = TossFourCoinsManaged(actualTries, playerChosenNumber, pLuck, enemyChosenNumber, eLuck);
            if (actualTries > 500) break;
        }

        Debug.Log($"ผลทอย: {actualTries}");
        DetermineWinner(actualTries, playerChosenNumber, enemyChosenNumber, target);
    }

    // ================================================================
    // 🖲️ ฟังก์ชันสำหรับให้ UI Button เรียกใช้งาน
    // ================================================================

    public void OnClick_Number(int number)
    {
        currentInputString += number.ToString();
        UpdateDisplayText();
    }

    public void OnClick_Delete()
    {
        if (currentInputString.Length > 0)
        {
            currentInputString = currentInputString.Substring(0, currentInputString.Length - 1);
            UpdateDisplayText();
        }
    }

    public void OnClick_Confirm()
    {
        if (int.TryParse(currentInputString, out int result) && result > 0)
        {
            playerChosenNumber = result;
        }
        else
        {
            Debug.LogWarning("กรุณาใส่ตัวเลขที่มากกว่า 0");
        }
    }

    private void UpdateDisplayText()
    {
        if (numberDisplayText != null)
        {
            numberDisplayText.text = currentInputString == "" ? "ใส่ตัวเลข..." : currentInputString;
        }
    }

    // ================================================================
    // ⚙️ ลอจิกการต่อสู้ (เหมือนเดิม)
    // ================================================================

    private void DetermineWinner(int actual, int pNum, int eNum, BaseUnit target)
    {
        if (myStats == null || target == null) return;

        int pDiff = Mathf.Abs(actual - pNum);
        int eDiff = Mathf.Abs(actual - eNum);

        Debug.Log($"<color=white>--- 🪙 [Coin Fate Battle] ผลออกที่: {actual} ---</color>");
        Debug.Log($"ทายผล: {myStats.unitName} ทาย {pNum} (ห่าง {pDiff}) vs {target.unitName} ทาย {eNum} (ห่าง {eDiff})");

        bool iAmPlayer = gameObject.CompareTag("Player");

        if (pDiff <= eDiff)
        {
            Debug.Log($"<color=green>👑 [WINNER] {myStats.unitName} ชนะการเดิมพัน!</color>");

            if (iAmPlayer)
            {
                FateCoinData enemyCoin = target.currentFate;
                FateInventory myInventory = GetComponent<FateInventory>();

                if (myInventory != null && enemyCoin != null)
                {
                    myInventory.AddCoin(enemyCoin);
                    Debug.Log($"💰 <color=yellow>FATE EXTRACTED!</color> ยึดเหรียญ {enemyCoin.coinName} สำเร็จ!");
                }

                target.TakeDamage(99999f, false);
            }
            else
            {
                target.TakeDamage(myStats.atk, false);
            }
        }
        else
        {
            Debug.Log($"<color=red>💀 [LOSER] {myStats.unitName} แพ้การเดิมพัน!</color>");

            if (iAmPlayer)
            {
                myStats.TakeDamage(target.atk, true);
            }
            else
            {
                myStats.TakeDamage(target.atk, true);
            }
        }
    }

    private int DecideEnemyNumber(int playerNum)
    {
        int offset = Random.Range(0, 2) == 0 ? -1 : 1;
        int res = playerNum + offset;
        return res < 1 ? playerNum + 2 : res;
    }

    private bool TossFourCoinsManaged(int currentTry, int pNum, int pLuck, int eNum, int eLuck)
    {
        float baseChance = 12.5f;
        float pInf = (currentTry == pNum) ? (pLuck * 10f) : 0;
        float eInf = (currentTry == eNum) ? (eLuck * 10f) : 0;
        return Random.Range(0f, 100f) < (baseChance + pInf + eInf);
    }

    private void OnDisable() { if (Time.timeScale == 0) Time.timeScale = 1; }
    private void OnDestroy() { if (Time.timeScale == 0) Time.timeScale = 1; }
}