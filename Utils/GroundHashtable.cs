using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/*
 * Data structure for efficiently looking up GroundTileControllers using position.
 * Uses spatial hashing to group neighboring GroundTileControllers by position.
 */
public class GroundHashtable
{
	/* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

    private GroundController controller;
    private Dictionary<int, List<GroundTileController>> controllersTable;
    private int hashOffset = 5;

    /* 
     * -------------------------------------------------------------------------
     * MARK: SERVICE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    public GroundHashtable(GroundController groundController)
    {
    	controller = groundController;
    	controllersTable = new Dictionary<int, List<GroundTileController>>();
    	ConstructHashtable(groundController.TileControllers);
    }

    // Gets GroundTileController corresponding to (x, z) coordinates.
    public GroundTileController GetTileControllerByPosition(float x, float z)
    {
    	int regionLength = controller.spatialHashingRegionLength;
    	int xMod = ((int) x) % regionLength;
		int zMod = ((int) z) % regionLength;
		int controllerHash = xMod * hashOffset + zMod;
		
		Assert.IsTrue(controllersTable.ContainsKey(controllerHash));

		foreach (GroundTileController tileController in controllersTable[controllerHash]) {
			if (tileController.transform.position.x == x 
				&& tileController.transform.position.z == z) {
				return tileController;
			}
		}
		return default(GroundTileController);
    }

	/* 
	 * -------------------------------------------------------------------------
	 * MARK: HELPER FUNCTIONS
	 * -------------------------------------------------------------------------
	 */

	/* 
	 * Construct hashtable from provided array.
	 * Uses region length provided by GroundController and self-defined hashOffset.
	 */
	void ConstructHashtable(GroundTileController[] tileControllers)
	{
		int regionLength = controller.spatialHashingRegionLength;
		foreach (GroundTileController tileController in tileControllers) {
			int x = ((int) tileController.transform.position.x) % regionLength;
			int z = ((int) tileController.transform.position.z) % regionLength;
			int controllerHash = x * hashOffset + z;
			if (controllersTable.ContainsKey(controllerHash)) {
				controllersTable[controllerHash].Add(tileController);
			} else {
				List<GroundTileController> controllersList = new List<GroundTileController>();
				controllersList.Add(tileController);
				controllersTable.Add(controllerHash, controllersList);
			}
		}
	}
}
