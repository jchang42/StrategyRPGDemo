using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Manages interactions for all enemies.
public class EnemyController : MonoBehaviour
{
    /* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

    private UnitController unitController;
    public UnitController UnitController
    {
    	get
    	{
    		return unitController;
    	}
    }

    private EnemyUnitController[] enemyControllers;

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
    	unitController = GameObject.FindGameObjectWithTag("Units").GetComponent<UnitController>();
        enemyControllers = GameObject.FindGameObjectsWithTag("Enemy")
        	.Select((enemy) => enemy.GetComponent<EnemyUnitController>())
        	.Cast<EnemyUnitController>()
        	.ToArray();
    }

    void Start()
    {
        if (unitController.LoadedUnitData != null) {
            LoadSavedData(unitController.LoadedUnitData.enemies);
        }
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: PERSISTENT DATA FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Load previously saved data for all enemy units, if they exist.
    void LoadSavedData(List<UnitInfo> enemyInfoList)
    {
        if (enemyInfoList == null) {
            return;
        }

        foreach (UnitInfo enemyInfo in enemyInfoList) {
            foreach (EnemyUnitController enemyController in enemyControllers) {
                if (enemyController.Name == enemyInfo.name) {
                    enemyController.LoadSavedData(enemyInfo);
                }
            }
        }
    }

    // Save data for all enemy units.
    public List<UnitInfo> SaveData()
    {
        List<UnitInfo> enemyInfoList = new List<UnitInfo>();
        foreach (EnemyUnitController enemyController in enemyControllers) {
            UnitInfo enemyInfo = enemyController.SaveData();
            enemyInfoList.Add(enemyInfo);
        }
        return enemyInfoList;
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: TURN MANAGEMENT FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Checks if all enemy units are tapped.
    public bool AreEnemiesTapped()
    {
        return enemyControllers.All(enemyController => enemyController.Tapped);
    }

    // Untap all enemy units.
    public void UntapEnemies()
    {
        foreach (EnemyUnitController enemyController in enemyControllers) {
            enemyController.UntapUnit();
        }
    }

    /*
     * Initiates actions for all enemy units.
     * Called at the start of each enemy turn.
     */
    public void StartEnemyActions()
    {
        foreach (EnemyUnitController enemyController in enemyControllers) {
            enemyController.DoAction();
        }
    }

    // Checks if all enemy units are defeated.
    public bool AllEnemiesDefeated()
    {
        return enemyControllers.All(enemyController => enemyController.Health == 0);
    }
}
