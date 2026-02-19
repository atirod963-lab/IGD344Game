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

    private void DetermineWinner(int actual, int pNum, int eNum, BaseUnit target)
    {
        if (myStats == null || target == null) return;

        int pDiff = Mathf.Abs(actual - pNum);
        int eDiff = Mathf.Abs(actual - eNum);

        Debug.Log($"<color=white>--- 🪙 [Coin Fate Battle] ผลออกที่: {actual} ---</color>");
        Debug.Log($"ทายผล: {myStats.unitName} ทาย {pNum} (ห่าง {pDiff}) vs {target.unitName} ทาย {eNum} (ห่าง {eDiff})");

        bool iAmPlayer = gameObject.CompareTag("Player");

        // --- กรณี: ทายถูกแม่นกว่า (ชนะ) ---
        if (pDiff <= eDiff)
        {
            Debug.Log($"<color=green>👑 [WINNER] {myStats.unitName} ชนะการเดิมพัน!</color>");

            if (iAmPlayer)
            {
                // 1. ยึด Fate Coin
                FateCoinData enemyCoin = target.currentFate;
                FateInventory myInventory = GetComponent<FateInventory>();

                if (myInventory != null && enemyCoin != null)
                {
                    myInventory.AddCoin(enemyCoin);
                    Debug.Log($"💰 <color=yellow>FATE EXTRACTED!</color> ยึดเหรียญ {enemyCoin.coinName} สำเร็จ!");
                }

                // 2. ตัดสินชีวิต (ตามคอนเซปต์ Fate: รับทุกอย่าง = ฆ่า)
                bool chooseToKill = true; // (อนาคตผูกกับ UI ปุ่มกด)

                if (chooseToKill)
                {
                    Debug.Log("💀 ผู้เล่นเลือก: สังหาร (Fate Execution)");

                    // 🔥 แก้ไขจุดที่ 1: ส่ง false ไปที่ช่อง isDefending เสมอ
                    // เพื่อบอกว่า "ห้ามป้องกัน" (Ignore Defense / True Damage)
                    // ต่อให้ศัตรูกด Defend มา ก็จะโดน 99999 เต็มๆ
                    target.TakeDamage(99999f, false);
                }
                else
                {
                    Debug.Log("✋ ผู้เล่นเลือก: ปล่อยไป (ไม่ทำดาเมจ)");
                    // ไม่เรียก TakeDamage เลย ศัตรูจะไม่โดนดาเมจใดๆ
                }
            }
            else
            {
                // ถ้าศัตรูชนะเรา: AI ตัดสินใจฆ่าเรา
                // เราโดนเต็มๆ (กันไม่ได้เหมือนกัน)
                target.TakeDamage(myStats.atk, false);
            }
        }
        // --- กรณี: ทายผิด (แพ้) ---
        else
        {
            Debug.Log($"<color=red>💀 [LOSER] {myStats.unitName} แพ้การเดิมพัน!</color>");

            if (iAmPlayer)
            {
                // 🔥 แก้ไขจุดที่ 2: ถ้าเราแพ้ "เรา" ต้องโดนดาเมจครับ (ไม่ใช่ target)
                // และเราโดนสวนกลับ (Counter) ถือว่าเราไม่ได้ Defend
                myStats.TakeDamage(target.atk, false);
            }
            else
            {
                // ศัตรูแพ้ -> ศัตรูโดนเราสวน
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