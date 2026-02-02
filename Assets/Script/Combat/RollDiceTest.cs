using UnityEngine;

public class RollDiceTest : MonoBehaviour
{
    [Header("Settings")]
    public DataLuckPlayer playerStats;
    public int numberOfDice = 4;

    [Header("Enemy Stats")]
    public int enemyHP = 30;
    public int enemyLuck = 5;

    void Start()
    {
        Debug.Log("--- [Start Game] ระบบพร้อมสำหรับการต่อสู้ ---");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ถ้าชนแล้วเลือดศัตรูยังมีอยู่ ให้เข้าสู่การ Battle
            if (enemyHP > 0)
            {
                Battle();
            }
            else
            {
                // ถ้าเลือดหมดอยู่แล้ว (ซึ่งปกติจะถูกทำลายไปแล้ว) ให้ทำลายทิ้ง
                Die();
            }
        }
    }

    public void Battle()
    {
        if (playerStats == null) return;

        int playerTotal = RollDices(playerStats.baseLuck, "ผู้เล่น");
        int enemyTotal = RollDices(enemyLuck, "ศัตรู");

        Debug.Log($"<color=cyan>แต้มสรุป -> ผู้เล่น: {playerTotal} VS ศัตรู: {enemyTotal}</color>");

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

        // เช็คตรงนี้: ถ้าเลือดหมดหลังจบ Battle ให้เรียกฟังก์ชันตายทันที
        if (enemyHP <= 0)
        {
            Die();
        }
    }

    // แยกฟังก์ชันการตายออกมาเพื่อให้เรียกใช้ง่ายและดูสะอาด
    private void Die()
    {
        Debug.Log("<color=red><b>ศัตรูพ่ายแพ้และถูกทำลาย!</b></color>");
        Destroy(gameObject); // แก้จาก Destroy.gameObject; เป็นแบบนี้ครับ
    }

    private int RollDices(int luck, string ownerName)
    {
        int total = 0;
        for (int i = 0; i < numberOfDice; i++)
        {
            total += GetWeightedRoll(luck);
        }
        return total;
    }

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