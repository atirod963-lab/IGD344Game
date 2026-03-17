using UnityEngine;

// Enum ยังคงใช้เหมือนเดิม แต่อยู่ในไฟล์นี้แทน
public enum MinigameType
{
    Standard,
    CoinFate,
    DicePoker
}

// โค้ดนี้จะทำให้คุณคลิกขวา Create -> Game Data -> Weapon ใน Unity ได้เลย
[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float bonusAtk; // พลังโจมตีบวกเพิ่มจากอาวุธ
    public MinigameType attackMinigame; // อาวุธนี้ใช้มินิเกมอะไร
    public Sprite weaponIcon; // รูปไอคอนอาวุธ
}