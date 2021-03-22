using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
	public enum EntityType { None, PartyMember, AllyUnit, EnemyUnit, Obstacle }
	public enum GUIState { 
        PartyTurnDefault, 
        OtherTurnDefault, 
        ActionMenu, 
        MoveMenu, 
        AttackMenu, 
        OptionsMenu,
        GameEnd }
    public enum TurnState { Party, Enemy }
    public enum EnemyPlayStyle { Advance, EventTriggered, Stationed, FixedPursuit, DynamicPursuit }

	public class TileColor
	{
		public static Color None = Color.green;
		public static Color Valid = Color.blue;
		public static Color Selectable = Color.white;
		public static Color OccupiedByParty = Color.yellow;
		public static Color OccupiedByEnemy = Color.red;
	}

	// Compares two colors by R, G, B, and alpha values.
    public static bool CompareColors(Color c1, Color c2)
    {
    	return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
    }

    // Projects Vector3 onto XZ-plane.
    public static Vector2 XZProjVec(Vector3 v)
    {
    	return new Vector2(v.x, v.z);
    }

    public static IEnumerator WaitCoroutine(float secs)
    {
        yield return new WaitForSeconds(secs);
    }
}