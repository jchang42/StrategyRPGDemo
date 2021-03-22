using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Manages interactions for all ground tiles.
public class GroundController : MonoBehaviour
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

    private Renderer[] tileRenderers;
    public Renderer[] TileRenderers
    {
        get
        {
            return tileRenderers;
        }
    }

    private GroundTileController[] tileControllers;
    public GroundTileController[] TileControllers
    {
        get
        {
            return tileControllers;
        }
    }

    private int gridSizeX;
    public int GridSizeX
    {
        get
        {
            return gridSizeX;
        }
    }

    private int gridSizeZ;
    public int GridSizeZ
    {
        get
        {
            return gridSizeZ;
        }
    }

    public GroundHashtable groundHashtable;
    public int spatialHashingRegionLength = 3;
    public GroundGraph groundGraph;

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
        terrainController = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainController>();
        GameObject[] groundTiles = GameObject.FindGameObjectsWithTag("Ground Tile");
        tileRenderers = groundTiles.Select((tile) => tile.GetComponent<Renderer>())
            .Cast<Renderer>()
            .ToArray();
        tileControllers = groundTiles.Select((tile) => tile.GetComponent<GroundTileController>())
            .Cast<GroundTileController>()
            .ToArray();

        foreach (GameObject tile in groundTiles) {
            if (tile.transform.position.x + 1 > gridSizeX) {
                gridSizeX = (int) tile.transform.position.x + 1;
            }
            if (tile.transform.position.z + 1 > gridSizeZ) {
                gridSizeZ = (int) tile.transform.position.z + 1;
            }
        }

        groundHashtable = new GroundHashtable(this);
        groundGraph = new GroundGraph(this);
        groundGraph.ConstructGraph();
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: GROUND SERVICE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Handle return from menu selection. Updates all tile colors.
    public void ResetAllTileColors() {
        for (int i = 0; i < tileControllers.Length; i++) {
            tileControllers[i].SavedColor = Utils.TileColor.None;
            tileRenderers[i].material.color = Utils.TileColor.None;
        }
    }

    // Search for a GroundTileController by (x, z) coordinates.
    public GroundTileController FindGroundTileControllerByXZ(float x, float z)
    {
        return groundHashtable.GetTileControllerByPosition(x, z);
    }
}
