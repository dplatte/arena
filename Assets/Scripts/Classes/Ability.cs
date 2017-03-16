using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability {

	protected string name;
	protected string description;
	protected int range;
	protected int power;
	protected int cooldown;
	protected int castTime;
	protected School school;
	protected int resourceCost;
	protected int resourceGain;
	protected int duration;
	public SpellType spellType;
	public Area area;
	public Direction direction;
	public List<Flag> flags;

	public Ability() {
	}

	public string Name {
		get { return name; }
		set { name = value; }
	}

	public string Description {
		get { return description; }
		set { description = value; }
	}

	public int Range {
		get { return range; }
		set { range = value; }
	}

	public int Power {
		get { return power; }
		set { power = value; }
	}

	public int Cooldown {
		get { return cooldown; }
		set { cooldown = value; }
	}

	public int CastTime {
		get { return castTime; }
		set { castTime = value; }
	}

	public School School {
		get { return school; }
		set { school = value; }
	}

	public int ResourceCost {
		get { return resourceCost; }
		set { resourceCost = value; }
	}

	public int ResourceGain {
		get { return resourceGain; }
		set { resourceGain = value; }
	}

	public int Duration {
		get { return duration; }
		set { duration = value; }
	}

	public SpellType SpellType {
		get { return spellType; }
		set { spellType = value; }
	}

	public Area Area {
		get { return area; }
		set { area = value; }
	}

	public Direction Direction {
		get { return direction; }
		set { direction = value; }
	}
			
}

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

