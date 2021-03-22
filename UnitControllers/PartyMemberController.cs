using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages interactions for a single party member.
public class PartyMemberController : EntityController
{
    /* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

	private PartyController partyController;

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
    	partyController = GameObject.FindGameObjectWithTag("Party").GetComponent<PartyController>();
    }

    /* Handle party member menu activations. 
     * Updates selected entity and sets GUI state.
     * Resets menu maps as necessary.
     */
    void OnMouseUp()
    {
    	BattleController battleController = partyController.UnitController.BattleController;
    	switch(battleController.GUIController.GuiState)
    	{
    		case Utils.GUIState.PartyTurnDefault:
    		case Utils.GUIState.ActionMenu:
                if (Health == 0) {
                    Debug.Log("This unit is downed");
                    break;
                } else if (Tapped) {
                    Debug.Log("This unit is tapped");
                    break;
                }
                
                battleController.TerrainController.GroundController.groundGraph.ResetMoveMap();
                battleController.TerrainController.GroundController.ResetAllTileColors();
                battleController.TerrainController.GroundController.groundGraph.ResetAttackMap();
                battleController.TerrainController.GroundController.ResetAllTileColors();
    			battleController.SetActiveSelection(this);
                battleController.GUIController.GuiState = Utils.GUIState.ActionMenu;
    			break;
    		case Utils.GUIState.MoveMenu:
    			if (battleController.ActiveSelection == this) {
    				battleController.SetActiveSelection(EntityController.Empty);
	    			battleController.GUIController.GuiState = Utils.GUIState.PartyTurnDefault;
	    		    battleController.TerrainController.GroundController.groundGraph.ResetMoveMap();
                    battleController.TerrainController.GroundController.ResetAllTileColors();
    			}
    			break;
            case Utils.GUIState.AttackMenu:
                if (battleController.ActiveSelection == this) {
                    battleController.SetActiveSelection(EntityController.Empty);
                    battleController.GUIController.GuiState = Utils.GUIState.PartyTurnDefault;
                    battleController.TerrainController.GroundController.groundGraph.ResetAttackMap();
                    battleController.TerrainController.GroundController.ResetAllTileColors();
                }
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

    // Load party member's saved data.
    public void LoadSavedData(UnitInfo partyMemberInfo)
    {
        GroundTileController targetTileController = partyController.UnitController.BattleController
            .TerrainController.GroundController
            .FindGroundTileControllerByXZ(partyMemberInfo.location.x, partyMemberInfo.location.z);
        UpdateOccupyingTileController(targetTileController);
        UpdatePositionFixed(Utils.XZProjVec(partyMemberInfo.location));
        UpdateStats(partyMemberInfo.stats);
    }
}
