using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level Table", menuName = "FateSystem/Level Table")]
public class LevelProgressionData : ScriptableObject
{
    [Header("Configuration")]
    public int maxLevel = 20;

    [Header("EXP Required to Next Level")]
    // Index 0 = EXP จาก Lv.0 -> Lv.1 (ถ้าเริ่มเวล 1 ก็ข้ามช่อง 0 ไป)
    // Index 1 = EXP จาก Lv.1 -> Lv.2 (ใส่ 10)
    // Index 5 = EXP จาก Lv.5 -> Lv.6 (ใส่ 15)
    public List<int> expRequirements;

    // ฟังก์ชันช่วยดึงค่า EXP ที่ต้องใช้
    public int GetExpForLevel(int level)
    {
        if (level >= expRequirements.Count) return int.MaxValue; // ตันแล้ว
        if (level < 0) return 0;
        return expRequirements[level];
    }
}