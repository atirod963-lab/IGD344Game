using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
    public DiceMinigameController diceMinigame;

    public DicePokerBattleController dicePokerController;

    [Tooltip("UI มินิเกมเหรียญ — วางบน prefab เดียวกับ BattleManager (DDOL) แล้วลากมาใส่ตรงนี้")]
    public CoinBattleUIRefs coinBattleUIRefs;

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

    private BaseUnit _playerCache;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        state = BattleState.IDLE;
        if (battleUIParent) battleUIParent.SetActive(false);
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        StartCoroutine(RecoverPlayerAfterLoad());
    }

    IEnumerator RecoverPlayerAfterLoad()
    {
        yield return null;
        yield return null;
        yield return new WaitForSeconds(0.1f);

        state = BattleState.IDLE;
        enemy = null;
        player = null; // ล้าง reference เก่า
        _playerCache = null;

        if (battleUIParent) battleUIParent.SetActive(false);
        CloseAllPanels();

        // 🔥 เปลี่ยนจากเดิมที่ FindWithTag ตรงๆ มาใช้ SafePlayer แทน
        player = SafePlayer;
        _playerCache = player; // sync cache

        if (player != null)
        {
            ClickToMove2D playerMove = player.GetComponent<ClickToMove2D>();
            if (playerMove != null)
            {
                playerMove.enabled = true;
                playerMove.StopMovementImmediately();
            }

            CoinSyncBattleController coinUi = player.GetComponent<CoinSyncBattleController>();
            if (coinUi != null) coinUi.TryBindBattleUiFromScene();

            Debug.Log($"✅ [BattleManager] โหลดซีนเสร็จ -> Player: {player.unitName}");
        }
        else
        {
            Debug.LogError("❌ [BattleManager] หาตัวละครไม่เจอ!");
        }
    }

    public void StartBattle(BaseUnit enemyUnit)
    {
        if (state != BattleState.IDLE) return;

        // 🔥 ถ้า player หลุด ให้หาใหม่ก่อน
        if (player == null)
        {
            GameObject pObj = GameObject.FindWithTag("Player");
            if (pObj != null) player = pObj.GetComponent<BaseUnit>();
        }

        if (player == null)
        {
            Debug.LogError("❌ [BattleManager] StartBattle: ไม่พบ Player!");
            return;
        }

        if (player.hp <= 0) return;

        enemy = enemyUnit;

        CoinSyncBattleController enemyCoinUi = enemyUnit != null ? enemyUnit.GetComponent<CoinSyncBattleController>() : null;
        if (enemyCoinUi != null) enemyCoinUi.TryBindBattleUiFromScene();

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

        if (isFleeing) Debug.Log("💨 แยกย้าย! (หนีสำเร็จ)");
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
            SceneManager.LoadScene("gameover");
        }

        CoinSyncBattleController.RewireSharedCoinUiBackToPlayer();
        enemy = null;
    }

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
    public void OnSelect_FateBetting()
    {
        if (attackSubPanel) attackSubPanel.SetActive(false);
        if (playerMainPanel) playerMainPanel.SetActive(false);
        StartCoroutine(PlayerAttackRoutine(true));
    }

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

    private void ExecuteRunLogic_PlayerTurn()
    {
        state = BattleState.BUSY;
        // 🔥 โอกาสหนี ดึง TotalLuck มาใช้
        float myPower = player.TotalLuck + player.spd;
        float enemyPower = enemy.TotalLuck + enemy.spd;

        if (myPower > enemyPower) { Debug.Log("💨 หนีสำเร็จ!"); EndBattle(true, true); }
        else { Debug.Log("🚫 หนีไม่พ้น! (เสียเทิร์น)"); StartCoroutine(TransitionToEnemyTurn()); }
    }

    IEnumerator EnemyTurnRunSequence()
    {
        state = BattleState.BUSY;
        yield return new WaitForSeconds(0.5f);
        // 🔥 ดาเมจตอกกลับตอนหนีพลาด ดึง TotalAtk มาใช้
        player.TakeDamage(enemy.TotalAtk, false);

        if (CheckWinCondition()) yield break;
        yield return new WaitForSeconds(1f);

        float myPower = player.TotalLuck + player.spd;
        float enemyPower = enemy.TotalLuck + enemy.spd;

        if (myPower > enemyPower) { Debug.Log("💨 หนีรอดจนได้!"); EndBattle(true, true); }
        else { Debug.Log("🚫 หนีไม่พ้น! (เจ็บฟรี)"); StartPlayerTurn(); }
    }

    // ================================================================
    // ⚔️ วงจรการโจมตี (Player Turn)
    // ================================================================
    IEnumerator PlayerAttackRoutine(bool isFateBetting)
    {
        state = BattleState.BUSY;
        if (enemy == null || CheckWinCondition()) yield break;

        // ปิดแพนเนลเมนต่อสู้ทั้งหมด — ไม่ให้ Image บน playerMain/enemyMain ทับ UI มินิเกม (Coin / Dice)
        // กรณี OnSelect_FateBetting ไม่ได้ปิด playerMainPanel ทำให้คลิกเลขไม่เข้า WaitUntil ค้าง
        CloseAllPanels();

        if (isFateBetting)
        {
            Debug.Log("⚡ เดิมพันโชคชะตา!");
            CoinSyncBattleController coinGame = player.GetComponent<CoinSyncBattleController>();
            if (coinGame != null) yield return StartCoroutine(coinGame.CoinBattleRoutine(enemy));
        }
        else
        {
            MinigameType weaponMinigame = MinigameType.Standard;
            if (player.equippedWeapon != null)
            {
                weaponMinigame = player.equippedWeapon.attackMinigame;
            }

            if (weaponMinigame == MinigameType.DicePoker && dicePokerController != null)
            {
                Debug.Log("🎲 โจมตีด้วยอาวุธ Dice Poker!");
                dicePokerController.StartDicePokerMinigame(player, enemy, true);
                yield return new WaitWhile(() => dicePokerController.dicePokerPanel.activeSelf);
            }
            else if (weaponMinigame == MinigameType.CoinFate)
            {
                Debug.Log("🪙 โจมตีด้วยอาวุธ Coin Fate!");
                CoinSyncBattleController coinGame = player.GetComponent<CoinSyncBattleController>();
                if (coinGame != null) yield return StartCoroutine(coinGame.CoinBattleRoutine(enemy));
            }
            else
            {
                Debug.Log("⚔️ โจมตีปกติ (เรียกมินิเกม 2 รอบ - 4Dice)");

                _4DiceBattleController pDiceCtrl = player.GetComponent<_4DiceBattleController>();
                _4DiceBattleController eDiceCtrl = enemy.GetComponent<_4DiceBattleController>();

                if (pDiceCtrl != null && eDiceCtrl != null)
                {
                    int[] playerRolls = pDiceCtrl.GetDiceRollsArray();
                    int[] enemyRolls = eDiceCtrl.GetDiceRollsArray();
                    bool minigameFinished = false;

                    diceMinigame.StartDoubleMinigame(playerRolls, enemyRolls, () => { minigameFinished = true; });
                    yield return new WaitUntil(() => minigameFinished);

                    int pScore = 0; foreach (int i in playerRolls) pScore += i;
                    int eScore = 0; foreach (int i in enemyRolls) eScore += i;

                    pDiceCtrl.ExecuteAttackWithPreRolls(enemy, pScore, eScore, false);
                }
                else Debug.LogError("หา 4DiceBattleController ไม่เจอที่ตัว Player หรือ Enemy!");
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
            Debug.Log("🛡️ คุณป้องกัน!");

            MinigameType enemyWeaponType = MinigameType.Standard;
            if (enemy.equippedWeapon != null)
            {
                enemyWeaponType = enemy.equippedWeapon.attackMinigame;
            }

            if (enemyWeaponType == MinigameType.DicePoker && dicePokerController != null)
            {
                Debug.Log("🎲 ศัตรูโจมตีด้วย Dice Poker!");
                dicePokerController.StartDicePokerMinigame(enemy, player, false);
                yield return new WaitWhile(() => dicePokerController.dicePokerPanel.activeSelf);
            }
            else
            {
                _4DiceBattleController pDiceCtrl = player.GetComponent<_4DiceBattleController>();
                _4DiceBattleController eDiceCtrl = enemy.GetComponent<_4DiceBattleController>();

                if (pDiceCtrl != null && eDiceCtrl != null)
                {
                    int[] playerRolls = pDiceCtrl.GetDiceRollsArray();
                    int[] enemyRolls = eDiceCtrl.GetDiceRollsArray();
                    bool minigameFinished = false;

                    diceMinigame.StartDoubleMinigame(playerRolls, enemyRolls, () => { minigameFinished = true; });
                    yield return new WaitUntil(() => minigameFinished);

                    int pScore = 0; foreach (int i in playerRolls) pScore += i;
                    int eScore = 0; foreach (int i in enemyRolls) eScore += i;

                    eDiceCtrl.ExecuteAttackWithPreRolls(player, eScore, pScore, true);
                }
            }
        }
        else
        {
            Debug.Log("💔 ไม่ป้องกัน โดนเต็มๆ!");
            // 🔥 โดนตีสดๆ ดึง TotalAtk มาใช้
            player.TakeDamage(enemy.TotalAtk, false);
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
        if (enemy != null && enemy.hp <= 0) { EndBattle(true); return true; }
        if (player.hp <= 0) { EndBattle(false); return true; }
        return false;
    }

    public BaseUnit SafePlayer
    {
        get
        {
            if (_playerCache == null)
            {
                GameObject pObj = GameObject.FindWithTag("Player");
                if (pObj != null) _playerCache = pObj.GetComponent<BaseUnit>();
            }
            return _playerCache;
        }
    }
}