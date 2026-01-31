using UnityEngine;
using System.Collections;

public class CoinSyncBattleController : MonoBehaviour
{
    public string targetTag = "Enemy";
    private BaseUnit myStats;
    private string currentInputString = "";

    void Start() => myStats = GetComponent<BaseUnit>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ถ้าไม่มีตัวตน หรือสคริปต์ปิดอยู่ หรือโลกหยุดเวลาอยู่แล้ว ไม่ต้องทำงาน
        if (myStats == null || !enabled || Time.timeScale == 0) return;

        if (other.CompareTag(targetTag))
        {
            BaseUnit targetStats = other.GetComponent<BaseUnit>();
            if (targetStats != null && targetStats.hp > 0)
                StartCoroutine(CoinBattleRoutine(targetStats));
        }
    }

    IEnumerator CoinBattleRoutine(BaseUnit target)
    {
        // --- เริ่มหยุดเวลา ---
        Time.timeScale = 0;
        Debug.Log($"<color=yellow>=== [PAUSED] เริ่มการเดิมพันโดย {myStats.unitName} ===</color>");

        int playerChosenNumber = -1;
        currentInputString = "";

        // วนลูปรับค่า
        while (playerChosenNumber == -1)
        {
            // [หัวใจหลัก] เช็คว่าตัวผู้ใส่สคริปต์ (myStats) หรือเป้าหมาย (target) ตายหรือยัง
            if (myStats == null || myStats.hp <= 0 || target == null || target.hp <= 0)
            {
                Debug.LogWarning("ผู้เล่นหรือมอนสเตอร์ตายระหว่างรอพิมพ์เลข! คืนค่าเวลาปกติ");
                break;
            }

            // รับค่าตัวเลข
            for (int i = 0; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
                {
                    currentInputString += i.ToString();
                    Debug.Log($"<color=white>ตัวเลข: {currentInputString}</color>");
                }
            }

            if (Input.GetKeyDown(KeyCode.Backspace) && currentInputString.Length > 0)
            {
                currentInputString = currentInputString.Substring(0, currentInputString.Length - 1);
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (int.TryParse(currentInputString, out int result) && result > 0)
                    playerChosenNumber = result;
            }
            yield return null;
        }

        // --- คืนค่าเวลาเสมอเมื่อจบลูป ---
        Time.timeScale = 1;

        // เช็คอีกครั้งว่าต้องทำต่อไหม (ถ้าหลุดลูปมาเพราะมีคนตาย ไม่ต้องรันต่อ)
        if (myStats == null || target == null || playerChosenNumber == -1) yield break;

        // --- กระบวนการทอยเหรียญ ---
        int enemyChosenNumber = DecideEnemyNumber(playerChosenNumber);
        int actualTries = 0;
        bool isSynchronized = false;

        // ดึงค่า Luck
        int pLuck = gameObject.CompareTag("Player") ? myStats.luck : target.luck;
        int eLuck = gameObject.CompareTag("Enemy") ? myStats.luck : target.luck;

        while (!isSynchronized)
        {
            actualTries++;
            isSynchronized = TossFourCoinsManaged(actualTries, playerChosenNumber, pLuck, enemyChosenNumber, eLuck);
            if (actualTries > 500) break;
        }

        Debug.Log($"<color=white>ผลทอย: {actualTries} (P: {playerChosenNumber} | E: {enemyChosenNumber})</color>");
        DetermineWinner(actualTries, playerChosenNumber, enemyChosenNumber, target);
    }

    private void DetermineWinner(int actual, int pNum, int eNum, BaseUnit target)
    {
        if (myStats == null || target == null) return;

        int pDiff = Mathf.Abs(actual - pNum);
        int eDiff = Mathf.Abs(actual - eNum);

        if (pDiff <= eDiff)
        {
            if (gameObject.CompareTag("Player")) target.TakeDamage(myStats.atk, false);
            else target.TakeDamage(myStats.atk, true);
        }
        else
        {
            if (gameObject.CompareTag("Player")) target.TakeDamage(myStats.atk, true);
            else target.TakeDamage(myStats.atk, false);
        }
    }

    private int DecideEnemyNumber(int playerNum)
    {
        int offset = Random.Range(0, 2) == 0 ? -1 : 1;
        int result = playerNum + offset;
        return (result < 1) ? playerNum + 2 : result;
    }

    private bool TossFourCoinsManaged(int currentTry, int pNum, int pLuck, int eNum, int eLuck)
    {
        float baseChance = 12.5f;
        float luckDiff = pLuck - eLuck;

        float pInfluence = (currentTry == pNum) ? (pLuck * 10f) : 0;
        float eInfluence = (currentTry == eNum) ? (eLuck * 10f) : 0;

        if (currentTry == eNum && luckDiff > 10) eInfluence = 0;
        if (currentTry == pNum && luckDiff < -10) pInfluence = 0;

        return Random.Range(0f, 100f) < (baseChance + pInfluence + eInfluence);
    }

    // --- ส่วนเสริมป้องกันเวลาค้าง ---
    // ถ้าสคริปต์นี้โดนลบ หรือ Object นี้โดนทำลายกะทันหัน
    private void OnDisable() { if (Time.timeScale == 0) Time.timeScale = 1; }
    private void OnDestroy() { if (Time.timeScale == 0) Time.timeScale = 1; }
}