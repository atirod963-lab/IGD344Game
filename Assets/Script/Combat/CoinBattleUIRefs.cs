using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// สะท้อนฟิลด์ public ทั้งหมดของ <see cref="CoinSyncBattleController"/> ที่ผูกกับ UI/สเปรไรต์ —
/// ตั้งค่าบน prefab Battle (DDOL) แล้วลงทะเบียนที่ <see cref="BattleManager.coinBattleUIRefs"/>.
/// ตอนโหลดซีน / เริ่มสู้ <see cref="CoinSyncBattleController.TryBindBattleUiFromScene"/> จะคัดลอกจากที่นี่ไปที่ Player และ Enemy
/// </summary>
public class CoinBattleUIRefs : MonoBehaviour
{
    [Header("Gameplay (CoinSyncBattleController.targetTag)")]
    [Tooltip("แท็กเป้าหมาย — ว่าง = ไม่ทับค่าบนตัวละคร")]
    public string targetTag = "Enemy";

    [Header("Number Input UI")]
    public GameObject numberInputPanel;
    public TextMeshProUGUI numberDisplayText;
    public TextMeshProUGUI enemyNumberDisplayText;

    [Header("Manual Coin Toss UI")]
    public GameObject coinTossPanel;
    public TextMeshProUGUI tossCountText;
    public TextMeshProUGUI playerChoiceText;
    public TextMeshProUGUI enemyChoiceText;

    [Header("Coin Sprites & Buttons")]
    public Button[] coinButtons = new Button[4];
    public Image[] coinImages = new Image[4];
    public Sprite defaultCoinSprite;
    public Sprite sunSprite;
    public Sprite starSprite;

    [Header("Number pad (รันไทม์ — ไม่มีฟิลด์บน CoinSync; ใช้ลง listener หลังโหลดซีน)")]
    public Button[] digitButtons = new Button[10];
    public Button deleteButton;
    public Button confirmButton;

    /// <summary>
    /// คัดลอกทุกอย่างที่สอดคล้องกับ <see cref="CoinSyncBattleController"/> จาก refs นี้ไปที่ target
    /// </summary>
    public void ApplyTo(CoinSyncBattleController target)
    {
        if (target == null) return;

        if (!string.IsNullOrEmpty(targetTag))
            target.targetTag = targetTag;

        target.numberInputPanel = numberInputPanel;
        target.numberDisplayText = numberDisplayText;
        target.enemyNumberDisplayText = enemyNumberDisplayText;

        target.coinTossPanel = coinTossPanel;
        target.tossCountText = tossCountText;
        target.playerChoiceText = playerChoiceText;
        target.enemyChoiceText = enemyChoiceText;

        if (coinButtons != null && coinButtons.Length >= 4)
            target.coinButtons = coinButtons;
        if (coinImages != null && coinImages.Length >= 4)
            target.coinImages = coinImages;

        target.defaultCoinSprite = defaultCoinSprite;
        target.sunSprite = sunSprite;
        target.starSprite = starSprite;
    }
}
