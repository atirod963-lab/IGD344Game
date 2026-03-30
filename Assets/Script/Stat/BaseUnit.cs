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
    [Range(0, 25)] public int currentLevel = 1;
    public int currentExp = 0;

    public int requiredExp => levelTable != null ? levelTable.GetExpForLevel(currentLevel) : 999;

    [Header("Current Stats (Base)")]
    public float maxHp;
    public float hp;
    public float atk;
    public float def;
    public float spd;
    public int luck;

    [Header("Equipment")]
    public WeaponData equippedWeapon;

    // 🔥 สร้างสเตตัสรวม (ตัวละคร + อาวุธ) เอาไว้ให้ระบบต่อสู้ดึงไปใช้
    public float TotalAtk => equippedWeapon != null ? atk + equippedWeapon.bonusAtk : atk;
    public int TotalLuck => equippedWeapon != null ? luck + equippedWeapon.bonusLuck : luck;

    [Header("Debug / Cheats")]
    [Range(1, 25)] public int targetLevelCheat = 10;
    public bool testLvCheatNow = false;

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
        if (testLvCheatNow)
        {
            testLvCheatNow = false;
            DebugSetLevel();
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
            int hpGain = StatCalculator.GetStatGain(currentLevel, currentFate.hp);
            int atkGain = StatCalculator.GetStatGain(currentLevel, currentFate.atk);
            int defGain = StatCalculator.GetStatGain(currentLevel, currentFate.def);
            int spdGain = StatCalculator.GetStatGain(currentLevel, currentFate.spd);
            int luckGain = StatCalculator.GetStatGain(currentLevel, currentFate.luck);

            maxHp += hpGain;
            atk += atkGain;
            def += defGain;
            spd += spdGain;
            luck += luckGain;

            Debug.Log($"🎉 LEVEL UP! ({currentLevel}) Stats Gained -> HP+{hpGain}, ATK+{atkGain}...");
        }
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
        if (gameObject.CompareTag("Player"))
        {
            SaveSystem.Instance.LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DebugSetLevel()
    {
        if (targetLevelCheat <= currentLevel)
        {
            Debug.LogWarning("⚠️ เลเวลเป้าหมายต้องมากกว่าเลเวลปัจจุบัน!");
            return;
        }
        Debug.Log($"🚀 กำลังวาร์ปจาก Lv.{currentLevel} ไปยัง Lv.{targetLevelCheat}...");
        while (currentLevel < targetLevelCheat)
        {
            currentExp = requiredExp;
            LevelUp();
        }
    }
}