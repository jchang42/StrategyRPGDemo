using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Manages interactions for all wall segments in this wall container.
public class WallController : MonoBehaviour
{
    /* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

    private WallsController wallsController;
    public WallsController WallsController
    {
    	get
    	{
    		return wallsController;
    	}
    }

    private WallSegmentController[] wallSegmentControllers;
    public WallSegmentController[] WallSegmentController
    {
    	get
    	{
    		return wallSegmentControllers;
    	}
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
        wallsController = GameObject.FindGameObjectWithTag("Walls").GetComponent<WallsController>();
        List<Transform> childTransforms = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++) {
        	childTransforms.Add(transform.GetChild(i));
        }
        wallSegmentControllers = childTransforms.ToArray()
        	.Select((trans) => trans.gameObject.GetComponent<WallSegmentController>())
        	.Cast<WallSegmentController>()
        	.ToArray();
    }
}
