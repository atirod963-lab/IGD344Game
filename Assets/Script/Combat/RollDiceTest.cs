using UnityEngine;

public class RollDiceTest : MonoBehaviour
{
    [Header("Settings")]
    public DataLuckPlayer playerStats; // สถิติของผู้เล่น
    public int numberOfDice = 4;

    [Header("Enemy Stats")]
    public int enemyHP = 30;
    public int enemyLuck = 5; // โชคของศัตรู

    void Start()
    {
        Debug.Log("--- [Start Game] ระบบพร้อมสำหรับการต่อสู้ ---");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyHP > 0)
            {
                Battle();
            }
            else
            {
                Debug.Log("ศัตรูตัวนี้ตายแล้ว!");
            }
        }
    }

    public void Battle()
    {
        if (playerStats == null) return;

        // 1. ผู้เล่นทอย (ใช้ค่า Luck จากสคริปต์)
        int playerTotal = RollDices(playerStats.baseLuck, "ผู้เล่น");

        // 2. ศัตรูทอย (ใช้ค่า Luck ที่ตั้งไว้ 5)
        int enemyTotal = RollDices(enemyLuck, "ศัตรู");

        Debug.Log($"<color=cyan>แต้มสรุป -> ผู้เล่น: {playerTotal} VS ศัตรู: {enemyTotal}</color>");

        // 3. เปรียบเทียบผล
        if (playerTotal >= enemyTotal)
        {
            enemyHP -= 5;
            Debug.Log($"<color=green>ชนะ! ตีเข้า 5 หน่วย (เลือดศัตรูเหลือ: {enemyHP})</color>");
        }
        else
        {
            enemyHP -= 3;
            Debug.Log($"<color=orange>แพ้! ตีเข้าแค่ 3 หน่วย (เลือดศัตรูเหลือ: {enemyHP})</color>");
        }

        if (enemyHP <= 0)
        {
            Debug.Log("<color=red><b>ศัตรูพ่ายแพ้!</b></color>");
        }
    }

    // ฟังก์ชันช่วยทอยลูกเต๋าตามจำนวนและค่าโชค
    private int RollDices(int luck, string ownerName)
    {
        int total = 0;
        for (int i = 0; i < numberOfDice; i++)
        {
            total += GetWeightedRoll(luck);
        }
        return total;
    }

    // ระบบสุ่มแบบถ่วงน้ำหนัก (เหมือนเดิม)
    private int GetWeightedRoll(int luck)
    {
        float chanceRoll = Random.Range(0f, 100f);
        float highThreshold = 15f + (luck * 0.5f);
        float midThreshold = highThreshold + 60f;

        if (chanceRoll < highThreshold)
            return Random.Range(16, 21);
        else if (chanceRoll < midThreshold)
            return Random.Range(6, 16);
        else
            return Random.Range(1, 6);
    }
}