using UnityEngine;

public class RollDiceTest : MonoBehaviour
{
    public DataLuckPlayer playerStats; // ลาก Object ที่มีสคริปต์ DataLuckPlayer มาใส่
    public int numberOfDice = 4;

    void Start()
    {
        // 1. ทอย 1 ครั้งตอนกดเริ่มเกม
        Debug.Log("--- [Start Game] เริ่มทอยครั้งแรก ---");
        RollAllDice();
    }

    // 2. ฟังก์ชันตรวจจับการเดินเข้าพื้นที่ (Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("--- [Area Entered] เดินเข้าพื้นที่! เริ่มทอยใหม่ ---");
            RollAllDice();
        }
    }

    // ฟังก์ชันหลักในการทอยลูกเต๋า 4 ลูก
    public void RollAllDice()
    {
        if (playerStats == null)
        {
            Debug.LogError("กรุณาลาก DataLuckPlayer มาใส่ในช่อง Player Stats ก่อนครับ!");
            return;
        }

        int total = 0;
        int luck = playerStats.baseLuck; // ค่าโชค 0 - 100

        Debug.Log($"<color=cyan>กำลังทอยด้วยค่า Luck: {luck}</color>");

        for (int i = 0; i < numberOfDice; i++)
        {
            int finalResult = GetWeightedRoll(luck);
            total += finalResult;
            Debug.Log($"ลูกเต๋าใบที่ {i + 1}: สุ่มได้แต้ม {finalResult}");
        }

        Debug.Log($"<color=yellow>รวมคะแนนทั้งหมด: {total}</color>");
    }

    // ฟังก์ชันคำนวณการสุ่มแบบถ่วงน้ำหนักตามค่าโชค
    private int GetWeightedRoll(int luck)
    {
        // สุ่มเลขดวง 0-100 เพื่อเลือกว่าจะตกกลุ่มไหน
        float chanceRoll = Random.Range(0f, 100f);

        // --- คำนวณขอบเขตโอกาส ---
        // ถ้า Luck 0: โอกาสได้แต้มสูง (16-20) คือ 15%
        // ถ้า Luck 100: โอกาสได้แต้มสูง (16-20) คือ 15 + 50 = 65%
        float highThreshold = 15f + (luck * 0.5f);

        // โอกาสได้แต้มกลาง (6-15) จะอยู่ที่ประมาณ 60% และจะค่อยๆ โดนบีบถ้า Luck สูงมากๆ
        float midThreshold = highThreshold + 60f;

        if (chanceRoll < highThreshold)
        {
            // กลุ่มแต้มสูง: สุ่มจริงในช่วง 16 ถึง 20
            return Random.Range(16, 21);
        }
        else if (chanceRoll < midThreshold)
        {
            // กลุ่มแต้มกลาง: สุ่มจริงในช่วง 6 ถึง 15
            return Random.Range(6, 16);
        }
        else
        {
            // กลุ่มแต้มต่ำ: สุ่มจริงในช่วง 1 ถึง 5
            return Random.Range(1, 6);
        }
    }
}