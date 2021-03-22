using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/*
 * Data structure for pathfinding. 
 * Stores information about ground tiles as double-edged graph in a node list.
 * Gets path from unit position to target tile position via breadth-first search (BFS).
 */
public class GroundGraph
{
	/* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES AND CLASSES
     * -------------------------------------------------------------------------
     */

	public class Node
	{
		public GroundTileController tileController;
		
		public Node northNode;
		public Node eastNode;
		public Node southNode;
		public Node westNode;

		public int moveDepth;
		public bool moveVisited;
		public Color moveColor;
		public Node prevMoveNode;

		public int attackDepth;
		public bool attackVisited;
		public Color attackColor;
		public Node prevAttackNode;

		public static Node Empty = null;

		public Node(GroundTileController tileController) 
		{
			this.tileController = tileController;
			
			northNode = Node.Empty;
			eastNode = Node.Empty;
			southNode = Node.Empty;
			westNode = Node.Empty;

			moveDepth = 0;
			moveVisited = false;
			moveColor = Utils.TileColor.None;
			prevMoveNode = Node.Empty;

			attackDepth = 0;
			attackVisited = false;
			attackColor = Utils.TileColor.None;
			prevAttackNode = Node.Empty;
		}

		public List<Node> GetDirectionalNodes() 
		{
			List<Node> nodes = new List<Node>();
			if (northNode != Node.Empty) {
				nodes.Add(northNode);
			}
			if (eastNode != Node.Empty) {
				nodes.Add(eastNode);
			}
			if (southNode != Node.Empty) {
				nodes.Add(southNode);
			}
			if (westNode != Node.Empty) {
				nodes.Add(westNode);
			}
			return nodes;
		}
	}

	private GroundController controller;
	private Node[] nodeList;

	/* 
     * -------------------------------------------------------------------------
     * MARK: GENERAL SERVICE FUNCTIONS
     * -------------------------------------------------------------------------
     */

	public GroundGraph(GroundController controller)
	{
		this.controller = controller;
		nodeList = new Node[controller.GridSizeX * controller.GridSizeZ];
	}

	// Handles construction of graph. Performed once on initialization of terrain.
	public void ConstructGraph()
	{
		for (int x = 0; x < controller.GridSizeX; x++) {
			for (int z = 0; z < controller.GridSizeZ; z++) {
				GroundTileController tileController = controller.FindGroundTileControllerByXZ(x, z);
				Node node = new Node(tileController);
				nodeList[x * controller.GridSizeZ + z] = node;

				if (z > 0) {
					node.southNode = nodeList[x * controller.GridSizeZ + z - 1];
					nodeList[x * controller.GridSizeZ + z - 1].northNode = node;
				}
				if (x > 0) {
					node.westNode = nodeList[(x - 1) * controller.GridSizeZ + z];
					nodeList[(x - 1) * controller.GridSizeZ + z].eastNode = node;
				}
			}
		}
	}

	/* 
     * -------------------------------------------------------------------------
     * MARK: PARTY MEMBER MOVE SERVICE FUNCTIONS
     * -------------------------------------------------------------------------
     */

	/* 
	 * Handles construction of move map.
	 * Performed each time a unit attempts to move.
	 */
	public bool ConstructMoveMap(Vector3 unitPosition, int movement, Utils.EntityType entityType)
	{
		Assert.IsTrue(movement >= 1);

		bool hasValidMove = false;
		Queue<Node> toVisit = new Queue<Node>();
		Node rootNode = GetNodeByPosition(unitPosition);
		rootNode.moveVisited = true;
		if (IsPartyMember(entityType)) {
			rootNode.moveColor = Utils.TileColor.OccupiedByParty;
		}
		toVisit.Enqueue(rootNode);

		while (toVisit.Count > 0) {
			Node curr = toVisit.Dequeue();
			foreach (Node directionalNode in curr.GetDirectionalNodes()) {
				if (!directionalNode.moveVisited 
					&& IsValidMove(directionalNode) 
					&& curr.moveDepth == movement - 1) {
					MarkMoveNode(curr, directionalNode);
					hasValidMove = true;
				} else if (!directionalNode.moveVisited
					&& (IsValidMove(directionalNode)
						|| IsSameEntityType(directionalNode, entityType))
					&& curr.moveDepth < movement - 1) {
					MarkMoveNode(curr, directionalNode);
					toVisit.Enqueue(directionalNode);
					hasValidMove = true;
				} else if (!directionalNode.moveVisited 
					&& IsPartyMember(entityType)) {
					MarkMoveNode(curr, directionalNode);
				}
			}
		}
		return hasValidMove;
	}

	/* 
	 * Handles deconstruction of move map.
	 * Must be used after calling ConstructMoveMap().
	 */
	public void ResetMoveMap()
	{
		foreach (Node node in nodeList) {
			UnmarkMoveNode(node);
		}
	}

	/*
	 * Handles setting tile colors using current move map.
	 * Must be used after calling ConstructMoveMap().
	 */
	public void ShowMoveMap()
	{
		foreach (Node node in nodeList) {
			SetMoveNodeColor(node);
		}
	}

	// Handles getting a path from current move map. Returns path as a stack.
	public Stack<Vector3> GetMoveMapPath(Vector3 targetPosition)
	{
		Stack<Vector3> path = new Stack<Vector3>();
		Node targetNode = GetNodeByPosition(targetPosition);
		while (targetNode.moveDepth > 0) {
			path.Push(targetNode.tileController.transform.position);
			targetNode = targetNode.prevMoveNode;
		}
		return path;
	}

	/* 
     * -------------------------------------------------------------------------
     * MARK: PARTY MEMBER ATTACK SERVICE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    /* 
	 * Handles construction of attack map.
	 * Performed each time a unit attempts to attack.
	 */
	public bool ConstructAttackMap(Vector3 unitPosition, int minRange, int maxRange, Utils.EntityType entityType)
	{
		Assert.IsTrue(minRange >= 1 && minRange <= maxRange);

		bool hasValidAttack = false;
		Queue<Node> toVisit = new Queue<Node>();
		Node rootNode = GetNodeByPosition(unitPosition);
		rootNode.attackVisited = true;
		toVisit.Enqueue(rootNode);

		while (toVisit.Count > 0) {
			Node curr = toVisit.Dequeue();
			foreach (Node directionalNode in curr.GetDirectionalNodes()) {
				if (!directionalNode.attackVisited
					&& curr.attackDepth < minRange - 1) {
					MarkAttackNode(curr, directionalNode);
					if (IsNotBlocked(directionalNode)) {
						toVisit.Enqueue(directionalNode);
					}
				} else if (!directionalNode.attackVisited
					&& curr.attackDepth >= minRange - 1
					&& curr.attackDepth < maxRange -  1) {
					MarkAttackNode(curr, directionalNode);
					if (directionalNode.tileController.occupyingEntity == EntityController.Empty) {
						toVisit.Enqueue(directionalNode);
					} else {
						switch (directionalNode.tileController.occupyingEntity.EntityType) {
							case Utils.EntityType.PartyMember:
								toVisit.Enqueue(directionalNode);
								break;
							case Utils.EntityType.EnemyUnit:
								toVisit.Enqueue(directionalNode);
								hasValidAttack = true;
								break;
							case Utils.EntityType.Obstacle:
							default:
								break;
						}
					}
				} else if (!directionalNode.attackVisited
					&& curr.attackDepth == maxRange - 1) {
					MarkAttackNode(curr, directionalNode);
					if (directionalNode.tileController.occupyingEntity != EntityController.Empty
						&& IsEnemyUnit(directionalNode.tileController.occupyingEntity.EntityType)) {
						hasValidAttack = true;
					}
				}
			}
		}
		return hasValidAttack;
	}

	/* 
	 * Handles deconstruction of attack map.
	 * Must be used after calling ConstructAttackMap().
	 */
	public void ResetAttackMap()
	{
		foreach (Node node in nodeList) {
			UnmarkAttackNode(node);
		}
	}

	/*
	 * Handles setting tile colors using current attack map.
	 * Must be used after calling ConstructAttackMap().
	 */
	public void ShowAttackMap()
	{
		foreach (Node node in nodeList) {
			SetAttackNodeColor(node);
		}
	}

	/* 
     * -------------------------------------------------------------------------
     * MARK: ENEMY MOVEMENT/ATTACK SERVICE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    /*
     * Check if enemy unit is aggro (has a valid attack).
     * Checks radial area of max size maxRange from origin.
     * Returns all possible targets' associated GroundTileController.
     */
    public List<GroundTileController> IsEnemyAggro(Vector3 origin, int maxRange)
    {
    	Assert.IsTrue(maxRange > 0);
		Queue<Node> toVisit = new Queue<Node>();
    	Node rootNode = GetNodeByPosition(origin);
		rootNode.attackVisited = true;
		toVisit.Enqueue(rootNode);

		while (toVisit.Count > 0) {
			Node curr = toVisit.Dequeue();
			foreach (Node directionalNode in curr.GetDirectionalNodes()) {
				if (!directionalNode.attackVisited
					&& curr.attackDepth < maxRange - 1) {

				}
			}
		}

    	return new List<GroundTileController>(); // TODO
    }

	/* 
	 * -------------------------------------------------------------------------
	 * MARK: HELPER FUNCTIONS
	 * -------------------------------------------------------------------------
	 */

	// Search node list by position.
	Node GetNodeByPosition(Vector3 position)
	{
		int x = (int) position.x;
		int z = (int) position.z;
		return nodeList[x * controller.GridSizeZ + z];
	}

	// Checks if tile is unoccupied.
	bool IsValidMove(Node node)
	{
		return node.tileController.occupyingEntity == EntityController.Empty;
	}

	// Checks if tile is not occupied by an obstacle.
	bool IsNotBlocked(Node node)
	{
		return node.tileController.occupyingEntity != EntityController.Empty
			&& node.tileController.occupyingEntity.EntityType != Utils.EntityType.Obstacle;
	}

	// Checks if tile is occupied by party member. 
	bool IsSameEntityType(Node node, Utils.EntityType entityType)
	{
		return node.tileController.occupyingEntity != EntityController.Empty
			&& node.tileController.occupyingEntity.EntityType == entityType;
	}

	// Check if entity type is party member.
	bool IsPartyMember(Utils.EntityType entityType)
	{
		return entityType == Utils.EntityType.PartyMember;
	}

	// Check if entity type is enemy unit.
	bool IsEnemyUnit(Utils.EntityType entityType)
	{
		return entityType == Utils.EntityType.EnemyUnit;
	}

	// "Marks" a move node by setting node attributes.
	void MarkMoveNode(Node prev, Node curr)
	{
		curr.moveDepth = prev.moveDepth + 1;
		curr.moveVisited = true;
		curr.prevMoveNode = prev;
		Color tileColor;
		if (curr.tileController.occupyingEntity == EntityController.Empty) {
			tileColor = Utils.TileColor.Valid;
		} else {
			switch(curr.tileController.occupyingEntity.EntityType)
			{
				case Utils.EntityType.PartyMember:
					tileColor = Utils.TileColor.OccupiedByParty;
					break;
				case Utils.EntityType.EnemyUnit:
					tileColor = Utils.TileColor.OccupiedByEnemy;
					break;
				default:
					tileColor = Utils.TileColor.None;
					break;
			}
		}
		curr.moveColor = tileColor;
	}

	// "Marks" an attack node by setting node attributes.
	void MarkAttackNode(Node prev, Node curr)
	{
		curr.attackDepth = prev.attackDepth + 1;
		curr.attackVisited = true;
		curr.prevAttackNode = prev;
		if (curr.tileController.occupyingEntity != EntityController.Empty
				&& IsEnemyUnit(curr.tileController.occupyingEntity.EntityType)) {
			curr.attackColor = Utils.TileColor.OccupiedByEnemy;
		} else {
			curr.attackColor = Utils.TileColor.None;
		}
	}

	// "Unmarks" a move node by resetting node attributes.
	void UnmarkMoveNode(Node node)
	{
		node.moveDepth = 0;
		node.moveVisited = false;
		node.prevMoveNode = Node.Empty;
		node.moveColor = Utils.TileColor.None;
	}

	// "Unmarks" an attack node by resetting node attributes.
	void UnmarkAttackNode(Node node)
	{
		node.attackDepth = 0;
		node.attackVisited = false;
		node.prevAttackNode = Node.Empty;
		node.attackColor = Utils.TileColor.None;
	}

	// Colors tiles based on move map.
	void SetMoveNodeColor(Node node)
	{
		node.tileController.SavedColor = node.tileController.Rend.material.color;
		node.tileController.Rend.material.color = node.moveColor;
	}

	// Colors tiles based on attack map.
	void SetAttackNodeColor(Node node)
	{
		node.tileController.SavedColor = node.tileController.Rend.material.color;
		node.tileController.Rend.material.color = node.attackColor;
	}
}