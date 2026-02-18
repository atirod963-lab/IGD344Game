using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public enum BattleState { IDLE, START, PLAYER_TURN, ENEMY_TURN, BUSY, ENDED }

    [Header("State")]
    public BattleState state;

    [Header("Units")]
    public BaseUnit player;
    public BaseUnit enemy;

    [Header("UI Panels (Parents)")]
    public GameObject battleUIParent;

    [Header("UI Panels (Main Menus)")]
    public GameObject playerMainPanel;
    public GameObject enemyMainPanel;

    [Header("UI Panels (Sub Menus)")]
    public GameObject attackSubPanel;
    public GameObject defenceSubPanel;
    public GameObject itemSubPanel;
    public GameObject runConfirmPanel;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        state = BattleState.IDLE;
        if (battleUIParent) battleUIParent.SetActive(false);
    }

    // ================================================================
    // ⚔️ BATTLE FLOW
    // ================================================================

    public void StartBattle(BaseUnit enemyUnit)
    {
        if (state != BattleState.IDLE) return;
        enemy = enemyUnit;
        if (battleUIParent) battleUIParent.SetActive(true);
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        CloseAllPanels();
        Debug.Log($"--- สู้กับ {enemy.unitName} ---");
        yield return new WaitForSeconds(1f);
        if (player.spd >= enemy.spd) StartPlayerTurn();
        else StartEnemyTurn();
    }

    public void EndBattle(bool playerWon, bool isFleeing = false)
    {
        if (state == BattleState.ENDED) return;

        state = BattleState.ENDED;
        if (battleUIParent) battleUIParent.SetActive(false);
        state = BattleState.IDLE;

        ClickToMove2D playerMove = player.GetComponent<ClickToMove2D>();
        if (playerMove != null) playerMove.enabled = true;

        if (isFleeing)
        {
            Debug.Log("💨 แยกย้าย! (หนีสำเร็จ)");
        }
        else if (playerWon)
        {
            Debug.Log("🏆 ชนะแล้ว!");
            // 🔥 แก้ตรงนี้: ดึง EXP จากมอนสเตอร์ตัวนั้นๆ
            if (enemy != null && enemy.baseStats != null)
            {
                int expReward = enemy.baseStats.expDrop;
                player.GainExp(expReward);
                Destroy(enemy.gameObject);
            }
        }
        else
        {
            Debug.Log("💀 แพ้แล้ว...");
        }
    }

    // ================================================================
    // 🕹️ UI SYSTEM
    // ================================================================

    void CloseAllPanels()
    {
        if (playerMainPanel) playerMainPanel.SetActive(false);
        if (enemyMainPanel) enemyMainPanel.SetActive(false);
        if (attackSubPanel) attackSubPanel.SetActive(false);
        if (defenceSubPanel) defenceSubPanel.SetActive(false);
        if (itemSubPanel) itemSubPanel.SetActive(false);
        if (runConfirmPanel) runConfirmPanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        CloseAllPanels();
        if (state == BattleState.PLAYER_TURN) playerMainPanel.SetActive(true);
        else if (state == BattleState.ENEMY_TURN) enemyMainPanel.SetActive(true);
    }

    // ================================================================
    // 🔵 PLAYER TURN UI
    // ================================================================

    public void StartPlayerTurn()
    {
        if (state == BattleState.ENDED) return;
        state = BattleState.PLAYER_TURN;
        Debug.Log("🔵 เทิร์นของคุณ");
        CloseAllPanels();
        if (playerMainPanel) playerMainPanel.SetActive(true);
    }

    public void OnClick_PlayerAttackMenu() { playerMainPanel.SetActive(false); attackSubPanel.SetActive(true); }
    public void OnClick_PlayerItemMenu() { playerMainPanel.SetActive(false); itemSubPanel.SetActive(true); }
    public void OnClick_PlayerRunConfirm() { playerMainPanel.SetActive(false); runConfirmPanel.SetActive(true); }

    public void OnSelect_NormalAttack() { attackSubPanel.SetActive(false); StartCoroutine(PlayerAttackRoutine(false)); }
    public void OnSelect_FateBetting() { attackSubPanel.SetActive(false); StartCoroutine(PlayerAttackRoutine(true)); }

    // ================================================================
    // 🔴 ENEMY TURN UI
    // ================================================================

    public void StartEnemyTurn()
    {
        if (state == BattleState.ENDED) return;
        state = BattleState.ENEMY_TURN;
        Debug.Log("🔴 เทิร์นศัตรู: เตรียมรับมือ!");
        CloseAllPanels();
        if (enemyMainPanel) enemyMainPanel.SetActive(true);
    }

    public void OnClick_DefenceMenu() { enemyMainPanel.SetActive(false); defenceSubPanel.SetActive(true); }
    public void OnClick_EnemyItemMenu() { enemyMainPanel.SetActive(false); itemSubPanel.SetActive(true); }
    public void OnClick_EnemyRunConfirm() { enemyMainPanel.SetActive(false); runConfirmPanel.SetActive(true); }

    public void OnSelect_NormalGuard() { defenceSubPanel.SetActive(false); StartCoroutine(EnemyAttackResolution(true)); }
    public void OnSelect_RefuseFate() { defenceSubPanel.SetActive(false); StartCoroutine(EnemyAttackResolution(false)); }

    // ================================================================
    // 🏃 COMMON ACTIONS (RUN / ITEM)
    // ================================================================

    public void OnSelect_RunYes()
    {
        runConfirmPanel.SetActive(false);

        // เช็คว่ากดหนีในเทิร์นใคร?
        if (state == BattleState.PLAYER_TURN)
        {
            // ถ้าเทิร์นเรา -> คำนวณหนีเลย
            ExecuteRunLogic_PlayerTurn();
        }
        else if (state == BattleState.ENEMY_TURN)
        {
            // ถ้าเทิร์นศัตรู -> ต้องโดนตีก่อน แล้วค่อยหนี
            StartCoroutine(EnemyTurnRunSequence());
        }
    }

    public void OnItemUsed()
    {
        Debug.Log("ใช้ไอเทมแล้ว!");
        itemSubPanel.SetActive(false);
        if (state == BattleState.PLAYER_TURN) StartCoroutine(TransitionToEnemyTurn());
        else if (state == BattleState.ENEMY_TURN)
        {
            // ถ้าใช้ไอเทมในเทิร์นศัตรู ก็ต้องโดนตีก่อนเหมือนกัน (ใช้ Logic เดียวกับตอนไม่กัน)
            StartCoroutine(EnemyAttackResolution(false));
        }
    }

    // ================================================================
    // ⚙️ LOGIC
    // ================================================================

    // --- 1. หนีในเทิร์นผู้เล่น (หนีได้เลย ถ้าพลาดเสียเทิร์น) ---
    private void ExecuteRunLogic_PlayerTurn()
    {
        state = BattleState.BUSY;
        float myPower = player.luck + player.spd;
        float enemyPower = enemy.luck + enemy.spd;

        if (myPower > enemyPower)
        {
            Debug.Log("💨 หนีสำเร็จ!");
            EndBattle(true, true);
        }
        else
        {
            Debug.Log("🚫 หนีไม่พ้น! (เสียเทิร์น)");
            StartCoroutine(TransitionToEnemyTurn());
        }
    }

    // --- 2. 🔥 หนีในเทิร์นศัตรู (โดนตีก่อน -> ค่อยคำนวณหนี) ---
    IEnumerator EnemyTurnRunSequence()
    {
        state = BattleState.BUSY;

        Debug.Log("😱 เลือกหนีในเทิร์นศัตรู -> ต้องรับดาเมจก่อน!");

        // STEP 1: โดนศัตรูโจมตี (เหมือนไม่ป้องกัน)
        yield return new WaitForSeconds(0.5f);
        Debug.Log("💔 ไม่ป้องกัน (เพื่อจะหนี)!");
        player.TakeDamage(enemy.atk, false);

        // STEP 2: เช็คว่าตายไหม?
        if (CheckWinCondition()) yield break; // ถ้าตายก็จบเลย ไม่ได้หนี

        yield return new WaitForSeconds(1f);

        // STEP 3: ถ้าไม่ตาย ค่อยคำนวณหนี
        float myPower = player.luck + player.spd;
        float enemyPower = enemy.luck + enemy.spd;

        Debug.Log($"🩸 เจ็บตัวแล้ว... พยายามหนีต่อ ({myPower} vs {enemyPower})");

        if (myPower > enemyPower)
        {
            Debug.Log("💨 หนีรอดจนได้!");
            EndBattle(true, true);
        }
        else
        {
            Debug.Log("🚫 หนีไม่พ้น! (เจ็บฟรี)");
            // หนีพลาด + โดนตีไปแล้ว -> กลับมาเป็นตาเรา
            StartPlayerTurn();
        }
    }

    // ... (Logic โจมตีอื่นๆ คงเดิม) ...

    IEnumerator PlayerAttackRoutine(bool isFateBetting)
    {
        state = BattleState.BUSY;
        if (enemy == null) yield break;
        if (CheckWinCondition()) yield break;

        if (isFateBetting)
        {
            Debug.Log("⚡ เดิมพันโชคชะตา!");
            CoinSyncBattleController coinGame = player.GetComponent<CoinSyncBattleController>();
            if (coinGame != null) yield return StartCoroutine(coinGame.CoinBattleRoutine(enemy));
        }
        else
        {
            Debug.Log("⚔️ โจมตีปกติ!");
            _4DiceBattleController diceGame = player.GetComponent<_4DiceBattleController>();
            if (diceGame != null) diceGame.ExecuteAttack(enemy);
            else Debug.LogError("ไม่พบสคริปต์ _4DiceBattleController");
        }

        if (CheckWinCondition()) yield break;
        yield return new WaitForSeconds(1f);
        StartEnemyTurn();
    }

    IEnumerator EnemyAttackResolution(bool isPlayerGuarding)
    {
        state = BattleState.BUSY;
        if (defenceSubPanel) defenceSubPanel.SetActive(false);

        Debug.Log("ศัตรูโจมตี!");
        yield return new WaitForSeconds(0.5f);

        if (isPlayerGuarding)
        {
            Debug.Log("🛡️ คุณป้องกัน!");
            _4DiceBattleController enemyDice = enemy.GetComponent<_4DiceBattleController>();
            if (enemyDice != null) enemyDice.ExecuteAttack(player, true);
            else player.TakeDamage(enemy.atk, true);
        }
        else
        {
            Debug.Log("💔 ไม่ป้องกัน!");
            player.TakeDamage(enemy.atk, false);
        }

        if (CheckWinCondition()) yield break;
        yield return new WaitForSeconds(1f);
        StartPlayerTurn();
    }

    IEnumerator TransitionToEnemyTurn()
    {
        if (CheckWinCondition()) yield break;
        yield return new WaitForSeconds(1f);
        StartEnemyTurn();
    }

    bool CheckWinCondition()
    {
        if (state == BattleState.ENDED) return true;

        if (enemy != null && enemy.hp <= 0)
        {
            EndBattle(true);
            return true;
        }

        if (player.hp <= 0)
        {
            EndBattle(false);
            return true;
        }

        return false;
    }
}