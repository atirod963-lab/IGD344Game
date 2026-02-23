using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    // ตำแหน่ง และ ฉาก
    public string sceneName;
    public float px, py, pz;

    // ข้อมูลตัวละคร
    public int level;
    public int exp;
    public float maxHp;
    public float atk;
    public float def;
    public float spd;
    public int luck;

    // *หมายเหตุ: ของสวมใส่ที่เป็น ScriptableObject เราจะเซฟเป็น "ชื่อ" แทนครับ
    public List<string> ownedCoinNames = new List<string>();
    public string equippedCoinName;
}