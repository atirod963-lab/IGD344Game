using UnityEngine;
using System.Collections;

public class CoinSyncBattleController : MonoBehaviour
{
    public string targetTag = "Enemy";
    private BaseUnit myStats;
    private string currentInputString = "";

    void Start() => myStats = GetComponent<BaseUnit>();

    // 🔥 เปลี่ยนเป็น public เพื่อให้ BattleManager เรียกใช้ได้
    public IEnumerator CoinBattleRoutine(BaseUnit target)
    {
        Time.timeScale = 0;
        Debug.Log($"<color=yellow>=== [PAUSED] เริ่มการเดิมพันโดย {myStats.unitName} ===</color>");

        int playerChosenNumber = -1;
        currentInputString = "";

        while (playerChosenNumber == -1)
        {
            if (myStats == null || myStats.hp <= 0 || target == null || target.hp <= 0)
            {
                Debug.LogWarning("จบการต่อสู้กลางคัน");
                break;
            }

            for (int i = 0; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
                    currentInputString += i.ToString();
            }

            if (Input.GetKeyDown(KeyCode.Backspace) && currentInputString.Length > 0)
                currentInputString = currentInputString.Substring(0, currentInputString.Length - 1);

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                if (int.TryParse(currentInputString, out int result) && result > 0)
                    playerChosenNumber = result;

            yield return null;
        }

        Time.timeScale = 1;

        if (myStats == null || target == null || playerChosenNumber == -1) yield break;

        int enemyChosenNumber = DecideEnemyNumber(playerChosenNumber);
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

    // ก๊อปปี้ฟังก์ชันนี้ไปทับ DetermineWinner ตัวเดิมครับ
    private void DetermineWinner(int actual, int pNum, int eNum, BaseUnit target)
    {
        if (myStats == null || target == null) return;

        int pDiff = Mathf.Abs(actual - pNum); // ความห่างของเจ้าของ (เรา)
        int eDiff = Mathf.Abs(actual - eNum); // ความห่างของเป้าหมาย (ศัตรู)

        Debug.Log($"<color=white>--- 🪙 [Coin Fate Battle] ผลออกที่: {actual} ---</color>");
        Debug.Log($"ทายผล: {myStats.unitName} ทาย {pNum} (ห่าง {pDiff}) vs {target.unitName} ทาย {eNum} (ห่าง {eDiff})");

        bool iAmPlayer = gameObject.CompareTag("Player");

        // ถ้าค่าความห่างเรา น้อยกว่าหรือเท่ากับ เขา (เราแม่นกว่า = เราชนะ)
        if (pDiff <= eDiff)
        {
            Debug.Log($"<color=green>👑 [WINNER] {myStats.unitName} ชนะการเดิมพัน!</color>");

            if (iAmPlayer)
            {
                // เราเป็นคนตี -> มอนกันไม่ได้
                target.TakeDamage(myStats.atk, false);
            }
            else
            {
                // มอนเป็นคนตี -> เรากันได้ (เพราะเราชนะเดิมพัน)
                target.TakeDamage(myStats.atk, true);
            }
        }
        else
        {
            Debug.Log($"<color=red>💀 [LOSER] {myStats.unitName} แพ้การเดิมพัน!</color>");

            if (iAmPlayer)
            {
                // เราเป็นคนตี -> มอนกันได้
                target.TakeDamage(myStats.atk, true);
            }
            else
            {
                // มอนเป็นคนตี -> เรากันไม่ได้ (โดนเต็ม)
                target.TakeDamage(myStats.atk, false);
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