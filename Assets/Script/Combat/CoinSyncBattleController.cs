using UnityEngine;
using UnityEngine.UI; // 🔥 เพิ่ม Library สำหรับจัดการปุ่มและรูปภาพ
using System.Collections;
using TMPro;

public class CoinSyncBattleController : MonoBehaviour
{
    public string targetTag = "Enemy";
    private BaseUnit myStats;
    private BaseUnit currentTarget; // บันทึกเป้าหมายไว้ใช้ตอนทอยเหรียญ

    [Header("Number Input UI")]
    public GameObject numberInputPanel;
    public TextMeshProUGUI numberDisplayText;
    public TextMeshProUGUI enemyNumberDisplayText;

    [Header("Manual Coin Toss UI")]
    public GameObject coinTossPanel; // ลาก Panel ทอยเหรียญมาใส่
    public TextMeshProUGUI tossCountText; // โชว์จำนวนครั้งที่ทอย
    public TextMeshProUGUI playerChoiceText; // โชว์ตัวเลขที่ Player ทาย
    public TextMeshProUGUI enemyChoiceText; // โชว์ตัวเลขที่ Enemy ทาย

    [Header("Coin Sprites & Buttons")]
    public Button[] coinButtons = new Button[4]; // ลากปุ่มเหรียญทั้ง 4 มาใส่
    public Image[] coinImages = new Image[4]; // ลาก Image ของเหรียญทั้ง 4 มาใส่ (มักจะอยู่บนปุ่ม)
    public Sprite defaultCoinSprite; // ภาพเหรียญด้านข้าง (ก่อนทอย)
    public Sprite sunSprite; // ภาพเหรียญหน้าพระอาทิตย์
    public Sprite starSprite; // ภาพเหรียญหน้าดาว

    private string currentInputString = "";
    private int playerChosenNumber = -1;
    private int enemyChosenNumber = -1;

    // ตัวแปรสำหรับระบบทอยเหรียญ
    private int actualTries = 0;
    private int matchedCoins = 0;
    private bool isFirstCoinTossed = false;
    private bool targetFaceIsSun = false; // บันทึกว่าเหรียญแรกออกหน้าอะไร (true=Sun, false=Star)
    private bool[] isCoinLocked = new bool[4];

    void Start() => myStats = GetComponent<BaseUnit>();

    public IEnumerator CoinBattleRoutine(BaseUnit target)
    {
        currentTarget = target;
        Time.timeScale = 0;
        Debug.Log($"<color=yellow>=== [PAUSED] เริ่มการเดิมพันโดย {myStats.unitName} ===</color>");

        // 1. ============ ช่วงทายตัวเลข ============
        playerChosenNumber = -1;
        currentInputString = "";
        UpdateNumberDisplayText(); // 🔥 เปลี่ยนเป็นชื่อนี้ครับ

        if (enemyNumberDisplayText != null)
        {
            enemyNumberDisplayText.gameObject.SetActive(true);
            enemyNumberDisplayText.text = "Enemy กำลังตัดสินใจ...";
        }

        if (numberInputPanel != null) numberInputPanel.SetActive(true);

        yield return new WaitUntil(() => playerChosenNumber != -1);

        enemyChosenNumber = DecideEnemyNumber(playerChosenNumber);

        if (enemyNumberDisplayText != null)
        {
            enemyNumberDisplayText.text = "<color=red>Enemy เลือก: " + enemyChosenNumber.ToString() + "</color>";
        }

        yield return new WaitForSecondsRealtime(2f); // ให้เวลาอ่านเลขศัตรู

        if (numberInputPanel != null) numberInputPanel.SetActive(false);


        // 2. ============ ช่วงทอยเหรียญ ============
        StartManualCoinToss(); // เปิด UI ทอยเหรียญและรีเซ็ตค่า

        // โค้ดจะหยุดรอตรงนี้ จนกว่าเหรียญที่ 2, 3, 4 จะออกหน้าเดียวกับเหรียญแรกครบทั้งหมด (matchedCoins == 3)
        yield return new WaitUntil(() => matchedCoins == 3);

        // ทอยเสร็จแล้ว รอให้ผู้เล่นดูผลลัพธ์สัก 1.5 วินาที แล้วค่อยปิดพาเนล
        yield return new WaitForSecondsRealtime(1.5f);
        if (coinTossPanel != null) coinTossPanel.SetActive(false);

        Time.timeScale = 1;

        if (myStats == null || target == null || playerChosenNumber == -1) yield break;

        Debug.Log($"ผลทอยทั้งหมด: {actualTries}");
        DetermineWinner(actualTries, playerChosenNumber, enemyChosenNumber, target);
    }

    // ================================================================
    // ⚙️ ระบบ UI และ Logic ทอยเหรียญด้วยมือ
    // ================================================================

    private void StartManualCoinToss()
    {
        actualTries = 0;
        matchedCoins = 0;
        isFirstCoinTossed = false;

        // รีเซ็ตเหรียญทั้ง 4
        for (int i = 0; i < 4; i++)
        {
            isCoinLocked[i] = false;
            if (coinImages[i] != null) coinImages[i].sprite = defaultCoinSprite;
        }

        UpdateTossUI();

        if (coinTossPanel != null) coinTossPanel.SetActive(true);
    }

    // 🔥 ให้ปุ่มเหรียญทั้ง 4 เรียกใช้ฟังก์ชันนี้ (และส่งค่า 0, 1, 2, 3 เข้ามาตามลำดับเหรียญ)
    public void OnClick_Coin(int coinIndex)
    {
        if (isCoinLocked[coinIndex]) return; // ถ้าเหรียญล็อคแล้ว ให้กดไม่ได้

        if (!isFirstCoinTossed)
        {
            // บังคับว่าต้องทอยเหรียญแรก (Index 0) ก่อนเสมอ
            if (coinIndex != 0) return;

            // ทอยเหรียญแรก (ยังไม่นับจำนวนครั้ง)
            targetFaceIsSun = Random.Range(0, 2) == 0; // สุ่ม 50/50 ว่าเป็นหน้า Sun หรือ Star
            coinImages[0].sprite = targetFaceIsSun ? sunSprite : starSprite;
            isCoinLocked[0] = true;
            isFirstCoinTossed = true;
            UpdateTossUI();
            return;
        }

        // สำหรับเหรียญที่ 2, 3, 4 (Index 1, 2, 3)
        if (coinIndex > 0)
        {
            actualTries++; // เริ่มนับจำนวนครั้งการทอย

            int pLuck = gameObject.CompareTag("Player") ? myStats.luck : currentTarget.luck;
            int eLuck = gameObject.CompareTag("Enemy") ? myStats.luck : currentTarget.luck;

            // คราวนี้นำค่า Luck มาใช้กับการสุ่ม "โอกาสที่จะออกหน้าเดียวกับเหรียญแรก"
            bool isMatch = TryMatchTargetFace(actualTries, playerChosenNumber, pLuck, enemyChosenNumber, eLuck);

            if (isMatch)
            {
                // ออกหน้าเดียวกับเหรียญแรก -> ล็อคเหรียญ
                coinImages[coinIndex].sprite = targetFaceIsSun ? sunSprite : starSprite;
                isCoinLocked[coinIndex] = true;
                matchedCoins++;
            }
            else
            {
                // ออกหน้าตรงข้าม -> ไม่ล็อค กดใหม่ได้
                coinImages[coinIndex].sprite = targetFaceIsSun ? starSprite : sunSprite;
            }

            UpdateTossUI();
        }
    }

    private bool TryMatchTargetFace(int currentTry, int pNum, int pLuck, int eNum, int eLuck)
    {
        // โอกาสตั้งต้นของการทอยเหรียญ 1 เหรียญคือ 50%
        float baseChance = 50f;
        float pInf = (currentTry == pNum) ? (pLuck * 10f) : 0;
        float eInf = (currentTry == eNum) ? (eLuck * 10f) : 0;
        return Random.Range(0f, 100f) < (baseChance + pInf + eInf);
    }

    private void UpdateTossUI()
    {
        if (tossCountText != null) tossCountText.text = $"จำนวนการทอย: {actualTries}";
        if (playerChoiceText != null) playerChoiceText.text = $"Player: {playerChosenNumber}";
        if (enemyChoiceText != null) enemyChoiceText.text = $"Enemy: {enemyChosenNumber}";
    }

    // ================================================================
    // 🖲️ ฟังก์ชันของ UI คีย์บอร์ด (เหมือนเดิม)
    // ================================================================

    public void OnClick_Number(int number)
    {
        currentInputString += number.ToString();
        UpdateNumberDisplayText();
    }

    public void OnClick_Delete()
    {
        if (currentInputString.Length > 0)
        {
            currentInputString = currentInputString.Substring(0, currentInputString.Length - 1);
            UpdateNumberDisplayText();
        }
    }

    public void OnClick_Confirm()
    {
        if (int.TryParse(currentInputString, out int result) && result > 0)
        {
            playerChosenNumber = result;
        }
        else Debug.LogWarning("กรุณาใส่ตัวเลขที่มากกว่า 0");
    }

    private void UpdateNumberDisplayText()
    {
        if (numberDisplayText != null)
        {
            numberDisplayText.text = currentInputString == "" ? "ใส่ตัวเลข..." : currentInputString;
        }
    }

    // ================================================================
    // ⚙️ ลอจิกหาผู้ชนะ (เหมือนเดิม)
    // ================================================================

    private void DetermineWinner(int actual, int pNum, int eNum, BaseUnit target)
    {
        if (myStats == null || target == null) return;
        int pDiff = Mathf.Abs(actual - pNum);
        int eDiff = Mathf.Abs(actual - eNum);
        Debug.Log($"<color=white>--- 🪙 [Coin Fate Battle] ผลออกที่: {actual} ---</color>");

        bool iAmPlayer = gameObject.CompareTag("Player");

        if (pDiff <= eDiff)
        {
            Debug.Log($"<color=green>👑 [WINNER] {myStats.unitName} ชนะการเดิมพัน!</color>");
            if (iAmPlayer)
            {
                FateCoinData enemyCoin = target.currentFate;
                FateInventory myInventory = GetComponent<FateInventory>();
                if (myInventory != null && enemyCoin != null)
                {
                    myInventory.AddCoin(enemyCoin);
                }
                target.TakeDamage(99999f, false);
            }
            else target.TakeDamage(myStats.atk, false);
        }
        else
        {
            Debug.Log($"<color=red>💀 [LOSER] {myStats.unitName} แพ้การเดิมพัน!</color>");
            myStats.TakeDamage(target.atk, true);
        }
    }

    private int DecideEnemyNumber(int playerNum)
    {
        int offset = Random.Range(0, 2) == 0 ? -1 : 1;
        int res = playerNum + offset;
        return res < 1 ? playerNum + 2 : res;
    }

    private void OnDisable() { if (Time.timeScale == 0) Time.timeScale = 1; }
    private void OnDestroy() { if (Time.timeScale == 0) Time.timeScale = 1; }
}