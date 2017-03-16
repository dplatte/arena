using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Ability {

	private Type type;
	private bool interruptible;

	public enum Type {
		Damage = 0,
		Healing = 1
	}
		
}
