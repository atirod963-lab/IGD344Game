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

    [Header("Minigame")]
    public DiceMinigameController diceMinigame; // ตัวแปรสำหรับเรียกใช้มินิเกม

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
        DontDestroyOnLoad(gameObject);
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

        // ไม่แน่ใจว่าคุณใช้สคริปต์ชื่อ ClickToMove2D บน Player ใช่ไหม ถ้าใช่ให้เปิดบรรทัดล่าง
        // ClickToMove2D playerMove = player.GetComponent<ClickToMove2D>();
        // if (playerMove != null) playerMove.enabled = true;

        if (isFleeing)
        {
            Debug.Log("💨 แยกย้าย! (หนีสำเร็จ)");
        }
        else if (playerWon)
        {
            Debug.Log("🏆 ชนะแล้ว!");
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
        if (state == BattleState.PLAYER_TURN) ExecuteRunLogic_PlayerTurn();
        else if (state == BattleState.ENEMY_TURN) StartCoroutine(EnemyTurnRunSequence());
    }

    public void OnItemUsed()
    {
        Debug.Log("ใช้ไอเทมแล้ว!");
        itemSubPanel.SetActive(false);
        if (state == BattleState.PLAYER_TURN) StartCoroutine(TransitionToEnemyTurn());
        else if (state == BattleState.ENEMY_TURN) StartCoroutine(EnemyAttackResolution(false));
    }

    // ================================================================
    // ⚙️ LOGIC
    // ================================================================

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

    IEnumerator EnemyTurnRunSequence()
    {
        state = BattleState.BUSY;
        Debug.Log("😱 เลือกหนีในเทิร์นศัตรู -> ต้องรับดาเมจก่อน!");

        yield return new WaitForSeconds(0.5f);
        Debug.Log("💔 ไม่ป้องกัน (เพื่อจะหนี)!");
        player.TakeDamage(enemy.atk, false);

        if (CheckWinCondition()) yield break;
        yield return new WaitForSeconds(1f);

        float myPower = player.luck + player.spd;
        float enemyPower = enemy.luck + enemy.spd;

        if (myPower > enemyPower)
        {
            Debug.Log("💨 หนีรอดจนได้!");
            EndBattle(true, true);
        }
        else
        {
            Debug.Log("🚫 หนีไม่พ้น! (เจ็บฟรี)");
            StartPlayerTurn();
        }
    }

    // ================================================================
    // ⚔️ วงจรการโจมตี (Player Turn)
    // ================================================================
    IEnumerator PlayerAttackRoutine(bool isFateBetting)
    {
        state = BattleState.BUSY;
        if (enemy == null || CheckWinCondition()) yield break;

        if (isFateBetting)
        {
            Debug.Log("⚡ เดิมพันโชคชะตา!");
            CoinSyncBattleController coinGame = player.GetComponent<CoinSyncBattleController>();
            if (coinGame != null) yield return StartCoroutine(coinGame.CoinBattleRoutine(enemy));
        }
        else
        {
            Debug.Log("⚔️ โจมตีปกติ (เรียกมินิเกม 2 รอบ)");

            // 1. ดึงสคริปต์ 4Dice มาทอยเต๋าเตรียมไว้
            _4DiceBattleController pDiceCtrl = player.GetComponent<_4DiceBattleController>();
            _4DiceBattleController eDiceCtrl = enemy.GetComponent<_4DiceBattleController>();

            if (pDiceCtrl != null && eDiceCtrl != null)
            {
                int[] playerRolls = pDiceCtrl.GetDiceRollsArray();
                int[] enemyRolls = eDiceCtrl.GetDiceRollsArray();

                bool minigameFinished = false;

                // 2. ส่ง Array ตัวเลขให้ UI แสดงมินิเกม
                diceMinigame.StartDoubleMinigame(playerRolls, enemyRolls, () =>
                {
                    minigameFinished = true;
                });

                // 3. รอมินิเกมโชว์หน้าสรุปจนเสร็จ
                yield return new WaitUntil(() => minigameFinished);

                // 4. เอาผลรวม ส่งกลับไปให้ 4Dice ทำการตัดสินดาเมจตามคอนเซ็ปต์คุณ
                int pScore = 0; foreach (int i in playerRolls) pScore += i;
                int eScore = 0; foreach (int i in enemyRolls) eScore += i;

                pDiceCtrl.ExecuteAttackWithPreRolls(enemy, pScore, eScore, false);
            }
            else
            {
                Debug.LogError("หา 4DiceBattleController ไม่เจอที่ตัว Player หรือ Enemy!");
            }
        }

        if (CheckWinCondition()) yield break;
        yield return new WaitForSeconds(1f);
        StartEnemyTurn();
    }

    // ================================================================
    // 🛡️ วงจรการป้องกัน (Enemy Turn)
    // ================================================================
    IEnumerator EnemyAttackResolution(bool isPlayerGuarding)
    {
        state = BattleState.BUSY;
        if (defenceSubPanel) defenceSubPanel.SetActive(false);

        Debug.Log("ศัตรูโจมตี!");
        yield return new WaitForSeconds(0.5f);

        if (isPlayerGuarding)
        {
            Debug.Log("🛡️ คุณป้องกัน! (ประชันลูกเต๋าลดดาเมจ)");

            _4DiceBattleController pDiceCtrl = player.GetComponent<_4DiceBattleController>();
            _4DiceBattleController eDiceCtrl = enemy.GetComponent<_4DiceBattleController>();

            if (pDiceCtrl != null && eDiceCtrl != null)
            {
                // โชว์มินิเกม (ให้ผู้เล่นทอยก่อน แล้วศัตรูทอยตาม เหมือนเดิม)
                int[] playerRolls = pDiceCtrl.GetDiceRollsArray();
                int[] enemyRolls = eDiceCtrl.GetDiceRollsArray();

                bool minigameFinished = false;

                diceMinigame.StartDoubleMinigame(playerRolls, enemyRolls, () =>
                {
                    minigameFinished = true;
                });

                yield return new WaitUntil(() => minigameFinished);

                int pScore = 0; foreach (int i in playerRolls) pScore += i;
                int eScore = 0; foreach (int i in enemyRolls) eScore += i;

                // 🔴 รอบนี้ ศัตรูเป็นคนโจมตี! (ส่ง forcedGuard เป็น true เพราะเรากดยืนยันการป้องกัน)
                eDiceCtrl.ExecuteAttackWithPreRolls(player, eScore, pScore, true);
            }
        }
        else
        {
            Debug.Log("💔 ไม่ป้องกัน โดนเต็มๆ!");
            // ถัาไม่กัน ศัตรูตีสดๆ เลย ไม่ต้องโชว์มินิเกม
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