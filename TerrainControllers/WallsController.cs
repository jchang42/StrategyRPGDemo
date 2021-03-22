using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Manages all interactions for all wall containers.
public class WallsController : MonoBehaviour
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

    private WallController[] wallControllers;
    public WallController[] WallControllers
    {
    	get
    	{
    		return wallControllers;
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
        wallControllers = GameObject.FindGameObjectsWithTag("Wall")
        	.Select((wall) => wall.GetComponent<WallController>())
            .Cast<WallController>()
            .ToArray();
    }
}
