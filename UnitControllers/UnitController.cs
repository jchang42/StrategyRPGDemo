using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages interactions for party, enemy, and ally controllers.
public class UnitController : MonoBehaviour
{
    /* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

    private BattleController battleController;
    public BattleController BattleController
    {
    	get
    	{
    		return battleController;
    	}
    }

    private PartyController partyController;
    public PartyController PartyController
    {
        get
        {
            return partyController;
        }
    }

    private EnemyController enemyController;
    public EnemyController EnemyController
    {
        get
        {
            return enemyController;
        }
    }

    private Utils.TurnState turnState = Utils.TurnState.Party;
    public Utils.TurnState TurnState
    {
        get
        {
            return turnState;
        }

        set
        {
            turnState = value;
        }
    }

    private int turnNumber = 1;
    public int TurnNumber
    {
        get
        {
            return turnNumber;
        }
    }

    private UnitData loadedUnitData;
    public UnitData LoadedUnitData
    {
        get
        {
            return loadedUnitData;
        }
    }

    private string savedBattleDataFilename = "battleData";
    private string savedUnitDataFilename = "unitData";

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
        battleController = GameObject.FindGameObjectWithTag("Battle Container").GetComponent<BattleController>();
        partyController = GameObject.FindGameObjectWithTag("Party").GetComponent<PartyController>();
        enemyController = GameObject.FindGameObjectWithTag("Enemies").GetComponent<EnemyController>();

        loadedUnitData = DataSaver.loadData<UnitData>(savedUnitDataFilename);
    }

    void Update()
    {
        switch (turnState)
        {
            case Utils.TurnState.Party:
                if (enemyController.AllEnemiesDefeated() && battleController.GUIController.GuiState != Utils.GUIState.GameEnd) {
                    Debug.Log("All enemies have been defeated");
                    battleController.GUIController.GuiState = Utils.GUIState.GameEnd;
                }
                if (partyController.IsPartyTapped()) {
                    Debug.Log("Turn Switch: party to enemy");
                    turnState = Utils.TurnState.Enemy;
                }
                break;
            case Utils.TurnState.Enemy:
                /*if (enemyController.AreEnemiesTapped()) {
                    turnState = Utils.TurnState.Party;
                }*/
                Debug.Log("Turn Switch: enemy to party");
                turnState = Utils.TurnState.Party;
                partyController.UntapParty();
                break;
            default:
                break;
        }
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: PERSISTENT DATA FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Save all unit data.
    public void SaveData()
    {
        BattleData battleData = new BattleData();
        battleData.turnState = turnState;
        DataSaver.saveData<BattleData>(battleData,  savedBattleDataFilename);

        UnitData unitData = new UnitData();
        List<UnitInfo> partyMemberInfoList = partyController.SaveData();
        unitData.partyMembers = partyMemberInfoList;
        DataSaver.saveData<UnitData>(unitData, savedUnitDataFilename);
    }
}
