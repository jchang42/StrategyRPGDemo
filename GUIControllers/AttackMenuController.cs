using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Manages interactions for attack menu GUI.
public class AttackMenuController : MonoBehaviour
{
    /* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

    private GUIController guiController;

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
    	guiController = GetComponent<GUIController>();
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: MOVE MENU SERVICE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Handle creating attack menu and shows attack map for selected party member.
    public void HandleAttackSelection()
    {
        guiController.BattleController.TerrainController.GroundController.groundGraph.ShowAttackMap();
    }

    /*
     * Handle party member attack after attack menu selection. 
     * Selected party member attacks target unit and updates GUI state.
     */
    public void AttackUnit(EntityController targetUnit)
    {
        EntityController memberController = guiController.BattleController.ActiveSelection;

        Assert.IsTrue(memberController.EntityType == Utils.EntityType.PartyMember);

        GroundController groundController = guiController.BattleController.TerrainController.GroundController;
        groundController.groundGraph.ResetAttackMap();
        groundController.ResetAllTileColors();

    	guiController.GuiState = Utils.GUIState.PartyTurnDefault;
    	
    	memberController.AttackUnit(targetUnit, true);
    }
}
