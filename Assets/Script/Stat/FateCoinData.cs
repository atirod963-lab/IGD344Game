using UnityEngine;

// 1. สร้างกล่องข้อมูลย่อย (Struct) เพื่อให้ตั้งค่าได้ละเอียดขึ้น
[System.Serializable]
public struct StatGrowthInfo
{
    public bool isFate;         // เป็นสีเขียวไหม?
    [Range(0, 100)]
    public int sealLevel;       // จะเริ่ม "ตัวแดง" (ตัน) ที่เลเวลเท่าไหร่? (ถ้าไม่ตันให้ใส่ 100)
}

[CreateAssetMenu(fileName = "New Fate Coin", menuName = "FateSystem/Fate Coin")]
public class FateCoinData : ScriptableObject
{
    public string coinName;

    [Header("Growth Configuration")]
    public StatGrowthInfo hp;
    public StatGrowthInfo atk;
    public StatGrowthInfo def;
    public StatGrowthInfo spd;
    public StatGrowthInfo luck;
}