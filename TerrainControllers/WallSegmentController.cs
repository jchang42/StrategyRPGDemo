using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages all interactions for a wall segment.
public class WallSegmentController : EntityController
{
	/* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

    private WallController wallController;

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
    	wallController = transform.parent.gameObject.GetComponent<WallController>();
    }
}
