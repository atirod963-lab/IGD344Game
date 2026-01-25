using UnityEngine;

public class RollDiceTest : MonoBehaviour
{
    public DataLuckPlayer playerStats; // ลาก Object ที่มี DataLuckPlayer มาใส่

    public void RollDice()
    {
        int totalScore = 0;
        int numberOfDice = 4;
        int luckValue = playerStats.baseLuck;

        Debug.Log($"--- เริ่มการทอยลูกเต๋า (Luck: {luckValue}) ---");

        for (int i = 0; i < numberOfDice; i++)
        {
            int result = RollSingleDice(luckValue);
            totalScore += result;
            Debug.Log($"ลูกที่ {i + 1}: ออกแต้ม {result}");
        }

        Debug.Log($"คะแนนรวมทั้งหมด: {totalScore}");
    }

    // ฟังก์ชันทอยลูกเต๋า 1 ลูก โดยใช้ Luck ช่วย
    int RollSingleDice(int luck)
    {
        // ทอยครั้งที่ 1 (พื้นฐาน)
        int roll1 = Random.Range(1, 21);

        // ถ้า Luck สูง เช่น Luck 15 อาจจะมีโอกาส 15% ที่จะทอยใหม่แล้วเลือกค่าที่มากกว่า
        // หรือใช้วิธีง่ายๆ: ถ้าสุ่มได้เลขน้อยกว่า Luck ให้มีโอกาสทอยใหม่
        if (Random.Range(0, 100) < luck)
        {
            int roll2 = Random.Range(1, 21);
            return Mathf.Max(roll1, roll2); // เลือกแต้มที่สูงที่สุดจากการทอยสองครั้ง
        }

        return roll1;
    }
}
