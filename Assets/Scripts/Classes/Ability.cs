using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability : ScriptableObject {

	public enum School {
		Holy = 0,
		Fire = 1,
		Lightning = 2,
		Ice = 3,
		Arcane = 4,
		Nature = 5,
		Shadow = 6,
		Physical = 7
	}

	public enum SpellType {
		Damage = 0,
		Healing = 1,
		Control = 2,
		Buff = 3,
		Debuff = 4
	}

	public enum Area {
		Single = 0,
		AOE = 1,

	}

	public enum Direction {
		Target = 0,
		Forward = 1,
		Point = 2
	}

	public enum Flag {
		None = 0,
		OverTime = 1,
		Slow = 2,
		Snare = 3
	}

	public enum SpellState {
		Available = 0,
		Casting = 1
	}

	public string name;
	public string description;
	public Sprite sprint;
	public AudioClip sound;
	public int range;
	public int minPower;
	public int maxPower;
	public int cooldown;
	public int castTime;
	public School school;
	public int resourceCost;
	public int duration;
	public SpellType spellType;
	public Area area;
	public Direction direction;
	public List<Flag> flags;
	public SpellState state;

	public abstract void Initialize(GameObject obj);
	public abstract void TriggerAbility();
			
}



