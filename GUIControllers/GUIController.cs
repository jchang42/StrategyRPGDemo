using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages interactions for GUI.
public class GUIController : MonoBehaviour
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

    private MoveMenuController moveMenuController;
    public MoveMenuController MoveMenuController
    {
    	get
    	{
    		return moveMenuController;
    	}
    }

    private AttackMenuController attackMenuController;
    public AttackMenuController AttackMenuController
    {
        get
        {
            return attackMenuController;
        }
    }

    private Utils.GUIState guiState = Utils.GUIState.PartyTurnDefault;
    public Utils.GUIState GuiState
    {
        get
        {
            return guiState;
        }

        set
        {
            guiState = value;
        }
    }

    private bool validMoveMenu = true;
    public bool ValidMoveMenu
    {
        set
        {
            validMoveMenu = value;
        }
    }

    private bool validAttackMenu = true;
    public bool ValidAttackMenu
    {
        set
        {
            validAttackMenu = value;
        }
    }

    private int menuXOffset = 10;
    private int menuYOffset = 10;
    private int menuWidth = 120;
    private int menuHeight = 90;

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
        battleController = GetComponent<BattleController>();
        moveMenuController = GetComponent<MoveMenuController>();
        attackMenuController = GetComponent<AttackMenuController>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            guiState = Utils.GUIState.OptionsMenu;
        }
    }

    // Handle mounting GUI components based on GUI state.
    void OnGUI() 
    {
    	switch(guiState)
    	{
    		case Utils.GUIState.ActionMenu:
    			ActionMenuGUI();
    			break;
    		case Utils.GUIState.MoveMenu:
    			MoveMenuGUI();
    			break;
            case Utils.GUIState.AttackMenu:
                AttackMenuGUI();
                break;
            case Utils.GUIState.OptionsMenu:
                OptionsMenuGUI();
                break;
            case Utils.GUIState.GameEnd:
                GameEndGUI();
                break;
    		default:
    			break;
    	}
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: MENU SERVICE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Handle action menu GUI.
    void ActionMenuGUI()
    {
    	GUI.Box(makeRect(), menuLabelWithUnitName("Action Menu"));

    	if (validMoveMenu && GUI.Button(new Rect(30, 40, 80, 20), "Move")) {
    		guiState = Utils.GUIState.MoveMenu;
    	}

        if (validAttackMenu && GUI.Button(new Rect(30, 70, 80, 20), "Attack")) {
            guiState = Utils.GUIState.AttackMenu;
        }
    }

    // Handle move menu GUI.
    void MoveMenuGUI()
    {
    	GUI.Box(makeRect(), menuLabelWithUnitName("Move Menu"));
    	moveMenuController.HandleMoveSelection();
    }

    // Handle attack menu GUI.
    void AttackMenuGUI()
    {
        GUI.Box(makeRect(), menuLabelWithUnitName("Attack Menu"));
        attackMenuController.HandleAttackSelection();
    }

    // Handle options menu GUI.
    void OptionsMenuGUI()
    {
        GUI.Box(makeRect(), "Options Menu");

        if (GUI.Button(new Rect(30, 40, 80, 20), "Save")) {
            battleController.UnitController.SaveData();
            guiState = Utils.GUIState.PartyTurnDefault;
        }

        // TESTING ONLY
        if (GUI.Button(new Rect(30, 70, 80, 20), "Delete")) {
            DataSaver.deleteData("battleData");
            DataSaver.deleteData("unitData");
            guiState = Utils.GUIState.PartyTurnDefault;
        }
    }

    // Handle game end GUI.
    void GameEndGUI() 
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "Level Cleared");
        GUI.Box(new Rect(Screen.width / 3, Screen.height / 2 - 20, Screen.width / 3, 20), "You defeated all of the enemies!");
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: HELPER FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Returns specialized GUI name.
    string menuLabelWithUnitName(string menuLabel)
    {
    	return menuLabel + ": " + battleController.ActiveSelection.Name;
    }

    // Returns Rect object with default menu dimensions.
    Rect makeRect() {
        return new Rect(menuXOffset, menuYOffset, menuWidth, menuHeight);
    }
}
