using UnityEngine;
using System.Collections;

public class CoinSyncBattleController : MonoBehaviour
{
    public string targetTag = "Enemy";
    private BaseUnit myStats;
    private string currentInputString = "";

    void Start() => myStats = GetComponent<BaseUnit>();

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

                // 2. ตัดสินชีวิต (ชนะ = ฆ่าตายแน่นอน ไม่สน Def)
                bool chooseToKill = true; 

                if (chooseToKill)
                {
                    Debug.Log("💀 ผู้เล่นเลือก: สังหาร (Fate Execution)");
                    // ชนะ: เจาะเกราะ 100% (True Damage)
                    target.TakeDamage(99999f, false);
                }
                else
                {
                    Debug.Log("✋ ผู้เล่นเลือก: ปล่อยไป (ไม่ทำดาเมจ)");
                }
            }
            else
            {
                // ศัตรูชนะเรา: เราตาย (True Damage)
                target.TakeDamage(myStats.atk, false);
            }
        }
        // --- กรณี: ทายผิด (แพ้เดิมพัน) ---
        else
        {
            Debug.Log($"<color=red>💀 [LOSER] {myStats.unitName} แพ้การเดิมพัน!</color>");

            if (iAmPlayer)
            {
                // 🔥 แก้ไขตรงนี้ครับ: เปลี่ยน false -> true
                // เพื่อบอกว่า "ให้ใช้ค่า Def มาคำนวณลดดาเมจด้วย"
                // สูตรจะเป็น: Damage = target.atk - myStats.def
                myStats.TakeDamage(target.atk, true);
            }
            else
            {
                // ถ้าศัตรูเป็นคนท้าแล้วแพ้ -> ศัตรูโดนเราตี (และให้ศัตรูใช้ Def กันได้)
                // หมายเหตุ: โค้ดเดิมเขียน target.TakeDamage(...) ซึ่ง target คือ Player 
                // ถ้าศัตรู (myStats) แพ้ คนที่ต้องเจ็บคือ myStats ครับ
                // แต่ถ้า logic คุณคือ Player ตีสวนกลับ ก็ต้องเขียนแบบนี้:
                
                // ให้ศัตรู (myStats) รับดาเมจจากเรา (target.atk) โดยให้กันได้ (true)
                // (ผมแก้ Logic ตรงนี้ให้ด้วยเผื่อคุณจะใช้ Enemy เป็นคนท้าในอนาคต)
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