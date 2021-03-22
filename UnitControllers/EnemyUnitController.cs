using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages all interactions for a single enemy unit.
public class EnemyUnitController : EntityController
{
    /* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

	private EnemyController enemyController;

    public Utils.EnemyPlayStyle playStyle = Utils.EnemyPlayStyle.Stationed;
    private Vector3 aggroOrigin;
    private EntityController targetUnit;
    public EntityController TargetUnit
    {
        get
        {
            return targetUnit;
        }

        set
        {
            targetUnit = value;
        }
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
    	enemyController = GameObject.FindGameObjectWithTag("Enemies").GetComponent<EnemyController>();
        aggroOrigin = transform.position;
        targetUnit = EntityController.Empty;
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: PERSISTENT DATA FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Load enemy unit's saved data.
    public void LoadSavedData(UnitInfo enemyInfo)
    {
        GroundTileController targetTileController = enemyController.UnitController.BattleController
            .TerrainController.GroundController
            .FindGroundTileControllerByXZ(enemyInfo.location.x, enemyInfo.location.z);
        UpdateOccupyingTileController(targetTileController);
        UpdatePositionFixed(Utils.XZProjVec(enemyInfo.location));
        UpdateStats(enemyInfo.stats);
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: MOVEMENT/ATTACK FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Enemy takes movement and attack actions depending on presets.
    public void DoAction()
    {
        switch(playStyle)
        {
            case Utils.EnemyPlayStyle.Advance:
                break;
            case Utils.EnemyPlayStyle.EventTriggered:
                // TODO
                break;
            case Utils.EnemyPlayStyle.Stationed:
                break;
            case Utils.EnemyPlayStyle.FixedPursuit:
                break;
            case Utils.EnemyPlayStyle.DynamicPursuit:
                break;
            default:
                break;
        }
    }
}
