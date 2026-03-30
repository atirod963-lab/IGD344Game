using UnityEngine;

public enum MinigameType
{
    Standard,
    CoinFate,
    DicePoker
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float bonusAtk; // พลังโจมตีบวกเพิ่มจากอาวุธ
    public int bonusLuck;  // 🔥 เพิ่มโบนัสโชคจากอาวุธ
    public MinigameType attackMinigame; // อาวุธนี้ใช้มินิเกมอะไร
    public Sprite weaponIcon; // รูปไอคอนอาวุธ
}