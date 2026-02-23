using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    [Header("Identity")]
    public string unitName;

    [Header("Fate System Data")]
    public MonsterBaseData baseStats;
    public FateCoinData currentFate;
    public LevelProgressionData levelTable;

    [Header("Progression")]
    // 🔥 แก้ไข Range เป็น 0 - 25 ตามที่ขอครับ
    [Range(0, 25)] public int currentLevel = 1;
    public int currentExp = 0;

    // ดึงค่า Exp ที่ต้องใช้จากตาราง (ถ้าไม่มีตารางใช้ 999)
    public int requiredExp => levelTable != null ? levelTable.GetExpForLevel(currentLevel) : 999;

    [Header("Current Stats")]
    public float maxHp;
    public float hp;
    public float atk;
    public float def;
    public float spd;
    public int luck;

    // --- 👇 ส่วน Cheat วาร์ปเลเวล (ติ๊กถูกเพื่อใช้งาน) 👇 ---
    [Header("Debug / Cheats")]
    [Range(1, 25)] public int targetLevelCheat = 10; // เป้าหมายที่จะวาร์ปไป
    public bool testLvCheatNow = false;  // 🔥 ติ๊กถูกตรงนี้เพื่อวาร์ป!

    protected virtual void Start()
    {
        if (maxHp <= 0 && baseStats != null)
        {
            InitializeStats();
        }
        hp = maxHp;
    }

    protected virtual void Update()
    {
        // เช็คว่ามีการกดปุ่มโกงไหม
        if (testLvCheatNow)
        {
            testLvCheatNow = false; // รีเซ็ตปุ่ม
            DebugSetLevel();     // รันคำสั่ง
        }
    }

    public void InitializeStats()
    {
        if (baseStats == null) return;

        maxHp = baseStats.baseHp;
        atk = baseStats.baseAtk;
        def = baseStats.baseDef;
        spd = baseStats.baseSpd;
        luck = baseStats.baseLuck;

        Debug.Log($"🌱 {unitName} เริ่มต้นใหม่ด้วยสเตตัสพื้นฐาน");
    }

    public void GainExp(int amount)
    {
        if (levelTable == null) return;
        currentExp += amount;

        while (currentExp >= requiredExp && currentLevel < levelTable.maxLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentExp -= requiredExp;
        currentLevel++;

        if (currentFate != null)
        {
            // คำนวณค่าที่จะได้เพิ่ม (Gain)
            int hpGain = StatCalculator.GetStatGain(currentLevel, currentFate.hp);
            int atkGain = StatCalculator.GetStatGain(currentLevel, currentFate.atk);
            int defGain = StatCalculator.GetStatGain(currentLevel, currentFate.def);
            int spdGain = StatCalculator.GetStatGain(currentLevel, currentFate.spd);
            int luckGain = StatCalculator.GetStatGain(currentLevel, currentFate.luck);

            // บวกทบเข้าไปในสเตตัสถาวร
            maxHp += hpGain;
            atk += atkGain;
            def += defGain;
            spd += spdGain;
            luck += luckGain;

            Debug.Log($"🎉 LEVEL UP! ({currentLevel}) Stats Gained -> HP+{hpGain}, ATK+{atkGain}...");
        }

        // รีเลือดเต็ม
        hp = maxHp;
    }

    public void EquipFateCoin(FateCoinData newCoin)
    {
        if (newCoin != null)
        {
            currentFate = newCoin;
            Debug.Log($"💎 {unitName} เปลี่ยนเหรียญเป็น: {newCoin.coinName}");
        }
    }

    public virtual void TakeDamage(float rawDamage, bool isDefending)
    {
        float finalDamage = rawDamage;
        if (isDefending) finalDamage = Mathf.Max(1f, rawDamage - def);

        hp -= finalDamage;
        if (hp <= 0) { hp = 0; Die(); }
    }

    protected virtual void Die()
    {
        Debug.Log($"{unitName} Died");

        // ถ้าคนที่ตายคือ Player ให้โหลดเซฟกลับมา
        if (gameObject.CompareTag("Player"))
        {
            SaveSystem.Instance.LoadGame();
        }
        else
        {
            Destroy(gameObject); // ถ้าศัตรูตายก็ลบทิ้งปกติ
        }
    }
    // --- ฟังก์ชันสูตรโกง ---
    public void DebugSetLevel()
    {
        if (targetLevelCheat <= currentLevel)
        {
            Debug.LogWarning("⚠️ เลเวลเป้าหมายต้องมากกว่าเลเวลปัจจุบัน!");
            return;
        }

        Debug.Log($"🚀 กำลังวาร์ปจาก Lv.{currentLevel} ไปยัง Lv.{targetLevelCheat}...");

        // วนลูปอัพเลเวลจนกว่าจะถึงเป้า
        while (currentLevel < targetLevelCheat)
        {
            currentExp = requiredExp; // เติม EXP เต็มหลอด
            LevelUp();                // สั่งอัพเลเวล
        }
    }
}