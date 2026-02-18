using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Base", menuName = "FateSystem/Monster Base")]
public class MonsterBaseData : ScriptableObject
{
    public string monsterName;

    [Header("Rewards")]
    public int expDrop = 10;

    [Header("Base Stats (ที่ Level 0)")]
    public int baseHp;
    public int baseAtk; // Strength
    public int baseDef;
    public int baseSpd;
    public int baseLuck;
}