using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Serialized data class with lists for all types of battle units.
[Serializable]
public class UnitData
{
    public List<UnitInfo> partyMembers;
    public List<UnitInfo> enemies;
    public List<UnitInfo> otherUnits;
}

// Serialized data class for an individual battle unit.
[Serializable]
public class UnitInfo
{
	public string name;

	// Unit battle location
	public Vector3 location;

	// Unit battle stats
	public UnitStats stats;

	// Unit inventory
	public UnitInventory inventory;

	public UnitInfo()
	{
		stats = new UnitStats();
		inventory = new UnitInventory();
	}
}

// Serialized data class for an individual battle unit's stats.
[Serializable]
public class UnitStats
{
	public int movement;
	public int minRange;
	public int maxRange;
	public int health;
	public int mana;
	public int physicalAttack;
	public int magicalAttack;
	public int physicalDefense;
	public int magicalDefense;
}

// Serialized data class for an individual battle unit's inventory.
[Serializable]
public class UnitInventory
{
	public List<string> armor;
	public List<string> baubles;
	public List<string> items;
}