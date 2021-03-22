using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Base class for storing and updating info for a single battle entity.
public class EntityController : MonoBehaviour
{
	/* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

    // Unit properties
    public string Name;
   	public Utils.EntityType EntityType;
   	public GroundTileController OccupyingTileController;

    private bool moved = false;
    public bool Moved
    {
        get
        {
            return moved;
        }
    }

    private bool tapped = false;
    public bool Tapped
    {
        get
        {
            return tapped;
        }

        set
        {
            tapped = value;
        }
    }
    
    // Unit stats
    public int Movement;
    public int MinRange;
    public int MaxRange;
	public int Health;
	public int Mana;
	public int PhysicalAttack;
	public int MagicalAttack;
	public int PhysicalDefense;
	public int MagicalDefense;

	// Unit inventory
	public List<string> armor;
	public List<string> baubles;
	public List<string> items;

    public float smoothingSpeed = 1f;
    public float movementErrorThreshold = 0.02f;
    public static EntityController Empty = null;

	/* 
     * -------------------------------------------------------------------------
     * MARK: PERSISTENT DATA FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Load unit's saved data.
    public void UpdateStats(UnitStats loadedStats)
	{
		Movement = loadedStats.movement;
        MinRange = loadedStats.minRange;
        MaxRange = loadedStats.maxRange;
		Health = loadedStats.health;
		Mana = loadedStats.mana;
		PhysicalAttack = loadedStats.physicalAttack;
		MagicalAttack = loadedStats.magicalAttack;
		PhysicalDefense = loadedStats.physicalDefense;
		MagicalDefense = loadedStats.magicalDefense;
	}

	// Save unit's data.
    public UnitInfo SaveData()
    {
        UnitInfo unitInfo = new UnitInfo();
        unitInfo.name = Name;
        unitInfo.location = transform.position;
        unitInfo.stats.movement = Movement;
        unitInfo.stats.health = Health;
        unitInfo.stats.minRange = MinRange;
        unitInfo.stats.maxRange = MaxRange;
        unitInfo.stats.mana = Mana;
        unitInfo.stats.physicalAttack = PhysicalAttack;
        unitInfo.stats.magicalAttack = MagicalAttack;
        unitInfo.stats.physicalDefense = PhysicalDefense;
        unitInfo.stats.magicalDefense = MagicalDefense;
        return unitInfo;
    }

   	/* 
     * -------------------------------------------------------------------------
     * MARK: UNIT MOVEMENT FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Updates unit's occupying tile.
	public void UpdateOccupyingTileController(GroundTileController targetTileController)
	{
		OccupyingTileController.occupyingEntity = EntityController.Empty;
		targetTileController.occupyingEntity = this;
		OccupyingTileController = targetTileController;
	}

	// Moves unit to the target tile following the specified movement path.
    public void MoveUnit(GroundTileController targetTileController, Stack<Vector3> movementPath)
    {
        UpdateOccupyingTileController(targetTileController);
        moved = true;

    	StartCoroutine(MoveCoroutine(movementPath));
    }

	// Coroutine for MoveUnit().
    IEnumerator MoveCoroutine(Stack<Vector3> path)
    {
        Vector3 targetPosition = new Vector3();
        while (path.Count > 0) {
            targetPosition = path.Pop();
            while (!ReachedTarget(targetPosition, transform.position)) {
                UpdatePositionSmooth(targetPosition);
                yield return null;
            }
            UpdatePositionFixed(Utils.XZProjVec(targetPosition));
        }
    	UpdatePositionFixed(Utils.XZProjVec(targetPosition));
    }

    /*
     * Updates unit's position.
     * Unit moves constant speed and displacement is clamped if it will overshoot target position.
     */
    void UpdatePositionSmooth(Vector3 targetPosition)
    {
    	Vector2 directionVec = Utils.XZProjVec(targetPosition) - Utils.XZProjVec(transform.position);
    	Vector2 displacementVec;
    	if (directionVec.magnitude <= smoothingSpeed * Time.deltaTime) {
    		displacementVec = Utils.XZProjVec(targetPosition);
    	} else {
    		displacementVec = Utils.XZProjVec(transform.position) + smoothingSpeed * Time.deltaTime * directionVec.normalized;
    	}
    	UpdatePositionFixed(displacementVec);
    }

	/*
	 * Updates unit's position.
	 * Sets unit's position to a fixed location.
	 */
	public void UpdatePositionFixed(Vector2 target)
	{
		transform.position = new Vector3(target.x, transform.position.y, target.y);
	}

    // Checks if distance between unit and tile positions are within error margin.
    bool ReachedTarget(Vector3 tilePos, Vector3 unitPos)
    {
        return Vector2.Distance(Utils.XZProjVec(tilePos), Utils.XZProjVec(unitPos)) <= movementErrorThreshold;
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: UNIT ATTACK FUNCTIONS
     * -------------------------------------------------------------------------
     */

    /*
     * Attacks target unit.
     * If it is the first strike, targetUnit will counterattack.
     */
    public void AttackUnit(EntityController targetUnit, bool isFirstStrike)
    {
        tapped = true;

        int xDist = Math.Abs((int) (transform.position.x - targetUnit.transform.position.x));
        int zDist = Math.Abs((int) (transform.position.z - targetUnit.transform.position.z));
        if (xDist + zDist >= MinRange && xDist + zDist <= MaxRange) {
            int damage = Math.Min(PhysicalAttack, targetUnit.Health);
            targetUnit.Health -= damage;
            Debug.Log(Name + " attacked " + targetUnit.Name + " for " + damage);
        }

        if (targetUnit.Health == 0) {
            switch (targetUnit.EntityType) {
                case Utils.EntityType.EnemyUnit:
                    Debug.Log("Enemy unit " + targetUnit.Name + " is defeated.");
                    targetUnit.OccupyingTileController.occupyingEntity = EntityController.Empty;
                    Destroy(targetUnit.gameObject);
                    break;
                case Utils.EntityType.PartyMember:
                    Debug.Log(targetUnit.Name + " is down!");
                    break;
                default:
                    break;
            }
        } else if (isFirstStrike) {
            targetUnit.AttackUnit(this, false);
        }
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: TURN MANAGEMENT FUNCTIONS
     * -------------------------------------------------------------------------
     */

    /*
     * Restores unit's ability to move and attack.
     * Called at the beginning of each turn.
     */
    public void UntapUnit()
    {
        moved = false;
        tapped = false;
    }
}
