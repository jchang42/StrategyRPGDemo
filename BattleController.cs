using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Master class for managing battle scene. 
 * Connects terrain, unit, and GUI controllers.
 */
public class BattleController : MonoBehaviour
{
    /* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

	private TerrainController terrainController;
	public TerrainController TerrainController
	{
		get
		{
			return terrainController;
		}
	}

    private UnitController unitController;
    public UnitController UnitController
    {
    	get
    	{
    		return unitController;
    	}
    }

    private GUIController guiController;
    public GUIController GUIController
    {
    	get
    	{
    		return guiController;
    	}
    }

    private EntityController activeSelection = EntityController.Empty;
    public EntityController ActiveSelection
    {
        get
        {
            return activeSelection;
        }
    }
    
    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
    	terrainController = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainController>();
        unitController = GameObject.FindGameObjectWithTag("Units").GetComponent<UnitController>();
        guiController = GetComponent<GUIController>();
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: UNIT SELECTION FUNCTIONS
     * -------------------------------------------------------------------------
     */

    /*
     * Handles switching active unit selections.
     * Constructs menu maps and updates valid menus.
     */
    public void SetActiveSelection(EntityController nextActiveSelection)
    {
        activeSelection = nextActiveSelection;

        if (activeSelection != EntityController.Empty
            && activeSelection.EntityType == Utils.EntityType.PartyMember) {
            GroundGraph groundGraph = terrainController.GroundController.groundGraph;
            bool hasValidMove = groundGraph.ConstructMoveMap(activeSelection.transform.position, activeSelection.Movement, activeSelection.EntityType);
            guiController.ValidMoveMenu = !activeSelection.Moved && hasValidMove;
            
            bool hasValidAttack = groundGraph.ConstructAttackMap(activeSelection.transform.position, activeSelection.MinRange, activeSelection.MaxRange, activeSelection.EntityType);
            guiController.ValidAttackMenu = !activeSelection.Tapped && hasValidAttack;
            
            if (!hasValidMove && !hasValidAttack) {
                activeSelection.Tapped = true;
            }
        }
    }
}
