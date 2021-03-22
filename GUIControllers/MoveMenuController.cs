using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Manages interactions for move menu GUI.
public class MoveMenuController : MonoBehaviour
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

    // Handle creating move menu and shows move map for selected party member.
    public void HandleMoveSelection()
    {
        EntityController memberController = guiController.BattleController.ActiveSelection;

        Assert.IsTrue(memberController.EntityType == Utils.EntityType.PartyMember);

        GroundGraph groundGraph = guiController.BattleController.TerrainController.GroundController.groundGraph;
        groundGraph.ShowMoveMap();
    }

    /*
     * Handle party member movement after move menu selection. 
     * Moves selected party member to target position and updates GUI state.
     */
    public void MoveUnit(GroundTileController targetTileController)
    {
        EntityController memberController = guiController.BattleController.ActiveSelection;

        Assert.IsTrue(memberController.EntityType == Utils.EntityType.PartyMember);

        GroundController groundController = guiController.BattleController.TerrainController.GroundController;
        Stack<Vector3> movementPath = groundController.groundGraph.GetMoveMapPath(targetTileController.transform.position);
        groundController.groundGraph.ResetMoveMap();
        groundController.ResetAllTileColors();

    	guiController.GuiState = Utils.GUIState.PartyTurnDefault;

        memberController.MoveUnit(targetTileController, movementPath);
    }
}
