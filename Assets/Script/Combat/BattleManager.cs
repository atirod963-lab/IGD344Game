using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // 🔥 1. เพิ่มบรรทัดนี้เข้ามา

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
    // 🧹 RESET SYSTEM (จัดการล้างค่าตอนโหลดเซฟ / เปลี่ยนด่าน)
    // ================================================================

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines(); // 1. หยุดวงจรเก่าทั้งหมดก่อน
        StartCoroutine(RecoverPlayerAfterLoad()); // 2. เริ่มวงจรค้นหาผู้เล่นแบบหน่วงเวลา
    }

    // 🔥 โค้ดใหม่: หน่วงเวลาให้เกมโหลดตัวละครเสร็จก่อนค่อยเปิดสคริปต์เดิน
    IEnumerator RecoverPlayerAfterLoad()
    {
        // 🌟 จุดสำคัญ: รอ 0.1 วินาที ให้ระบบ Save/Load สปอว์นตัวละครลงฉากให้เสร็จสมบูรณ์ 100%
        yield return new WaitForSeconds(0.1f);

        // 1. ล้างค่าระบบต่อสู้
        state = BattleState.IDLE;
        enemy = null;

        if (battleUIParent) battleUIParent.SetActive(false);
        CloseAllPanels();

        // 2. บังคับหา Player ใหม่เสมอ
        GameObject pObj = GameObject.FindWithTag("Player");
        if (pObj != null)
        {
            player = pObj.GetComponent<BaseUnit>();

            ClickToMove2D playerMove = pObj.GetComponent<ClickToMove2D>();
            if (playerMove != null)
            {
                playerMove.enabled = true; // สั่งเปิดสคริปต์เดิน
                playerMove.StopMovementImmediately(); // สั่งล้างสถานะการเดินเก่าที่อาจจะค้างอยู่
            }

            Debug.Log("🔄 [BattleManager] โหลดฉากเสร็จสมบูรณ์ -> เจอตัว Player และเปิดระบบเดินแล้ว!");
        }
        else
        {
            // ถ้าขึ้น Error นี้แปลว่าคุณลืมตั้ง Tag คำว่า Player ให้กับตัวละครครับ
            Debug.LogError("❌ [BattleManager] หาตัวละครไม่เจอ! (เช็คว่าตัวละครมี Tag 'Player' ไหม?)");
        }
    }

    // ================================================================
    // ⚔️ BATTLE FLOW
    // ================================================================

    public void StartBattle(BaseUnit enemyUnit)
    {
        if (state != BattleState.IDLE) return;

        // 🔥 ดักบั๊กชนซ้ำ: ถ้าผู้เล่นตายแล้ว (HP หมด) ห้ามดึงเข้าฉากสู้เด็ดขาด!
        if (player != null && player.hp <= 0)
        {
            Debug.LogWarning("ผู้เล่นตายแล้ว ศัตรูไม่สามารถเริ่มการต่อสู้ซ้ำได้!");
            return;
        }

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

        // 🔥 จุดที่แก้ไข: สั่งเปิดสคริปต์เดินให้กลับมาทำงานอีกครั้ง
        ClickToMove2D playerMove = player.GetComponent<ClickToMove2D>();
        if (playerMove != null)
        {
            playerMove.enabled = true;
        }

        if (isFleeing)
        {
            Debug.Log("💨 แยกย้าย! (หนีสำเร็จ)");
        }
        else if (playerWon)
        {
            Debug.Log("🏆 ชนะแล้ว!");
            // แจก EXP และลบศัตรูทิ้ง
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
        enemy = null;
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