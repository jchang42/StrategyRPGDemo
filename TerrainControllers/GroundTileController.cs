using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages interactions for a single ground tile.
public class GroundTileController : MonoBehaviour
{
    /* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

	private Renderer rend;
    public Renderer Rend
    {
        get
        {
            return rend;
        }
    }

    private Color savedColor;
    public Color SavedColor
    {
        get
        {
            return savedColor;
        }

        set
        {
            savedColor = value;
        }
    }

    private GroundController groundController;
    public EntityController occupyingEntity = EntityController.Empty;

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
    	rend = GetComponent<Renderer>();
    	groundController = GameObject.FindGameObjectWithTag("Ground").GetComponent<GroundController>();
        savedColor = rend.material.color;
    }

    /* Handle party member menu selection. 
     * Updates GUI state and initiates unit actions.
     */
    void OnMouseUp()
    {
    	BattleController battleController = groundController.TerrainController.BattleController;
    	switch(battleController.GUIController.GuiState)
  		{
    		case Utils.GUIState.ActionMenu:
    			battleController.SetActiveSelection(EntityController.Empty);
	    		battleController.GUIController.GuiState = Utils.GUIState.PartyTurnDefault;
	    		break;
	    	case Utils.GUIState.MoveMenu:
	    		if (IsValidMoveSpace()) {
					battleController.GUIController.GuiState = Utils.GUIState.PartyTurnDefault;
		    		groundController.TerrainController.BattleController.GUIController.MoveMenuController.MoveUnit(this);
	    		}
	    		break;
            case Utils.GUIState.AttackMenu:
                if (IsValidAttackSpace() ) {
                    battleController.GUIController.GuiState = Utils.GUIState.PartyTurnDefault;
                    groundController.TerrainController.BattleController.GUIController.AttackMenuController.AttackUnit(occupyingEntity);
                }
                break;
	    	default:
	    		break;
    	}
    }

    // Handle menu selection. Updates tile color while mouse is hovering over tile.
    void OnMouseOver()
    {
    	if (IsValidMoveSpace() || IsValidAttackSpace()) {
    		savedColor = rend.material.color;
            rend.material.color = Utils.TileColor.Selectable;
    	}
    }

    // Handle move menu selection. Updates tile color when mouse exits hovering over tile.
    void OnMouseExit()
    {
    	Utils.GUIState guiState = groundController.TerrainController.BattleController.GUIController.GuiState;
    	if (guiState == Utils.GUIState.MoveMenu || guiState  == Utils.GUIState.AttackMenu) {
    		rend.material.color = savedColor;
    	}
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: HELPER FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Checks if tile is in "valid" state in move menu.
    bool IsValidMoveSpace() 
    {
        Utils.GUIState guiState = groundController.TerrainController.BattleController.GUIController.GuiState;
        return Utils.CompareColors(rend.material.color, Utils.TileColor.Valid) && guiState == Utils.GUIState.MoveMenu;
    }

    // Checks if tile is in "valid" state in attack menu.
    bool IsValidAttackSpace() 
    {
        Utils.GUIState guiState = groundController.TerrainController.BattleController.GUIController.GuiState;
        return Utils.CompareColors(rend.material.color, Utils.TileColor.OccupiedByEnemy) && guiState == Utils.GUIState.AttackMenu;
    }
}
