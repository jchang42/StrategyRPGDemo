using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages interactions for ground and obstacle controllers.
public class TerrainController : MonoBehaviour
{
    /* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

    private GroundController groundController;
    public GroundController GroundController
    {
    	get
    	{
    		return groundController;
    	}
    }

    private BattleController battleController;   
    public BattleController BattleController
    {
    	get
    	{
    		return battleController;
    	}
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
        groundController = GameObject.FindGameObjectWithTag("Ground").GetComponent<GroundController>();
        battleController = GameObject.FindGameObjectWithTag("Battle Container").GetComponent<BattleController>();
    }
}
