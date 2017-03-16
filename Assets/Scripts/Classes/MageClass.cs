using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MageClass : BaseClass {

	// Use this for initialization
	void Start () {
		name = "Mage";
		description = "A powerful spellcaster.";
		maxHealth = 100;
		curHealth = 100;
		maxPrimaryResource = 100;
		curPrimaryResource = 100;
		primaryResource = Resource.Mana;

		abilities.Add (new Ability {
			Name = "Frostbolt", 
			Description = "Launch a missile of ice at an enemy.",
			Range = 40, 
			Power = 10, 
			Cooldown = 0, 
			CastTime = 2, 
			School = School.Ice, 
			ResourceCost = 10, 
			ResourceGain = 0, 
			Duration = 0, 
			SpellType = SpellType.Damage, 
			Area = Area.Single, 
			Direction = Direction.Target
		});
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

