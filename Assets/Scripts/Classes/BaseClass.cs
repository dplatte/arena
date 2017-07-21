using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseClass : MonoBehaviour {

	protected string name;
	protected string description;
	protected int maxHealth;
	protected int curHealth;
	protected int maxPrimaryResource;
	protected int curPrimaryResource;
	protected int maxSecondaryResource;
	protected int curSecondaryResource;
	protected Resource primaryResource;
	protected Resource secondaryResource;
	public GameObject[] abilities;

	public virtual void Cast() {

	}

	public enum Resource {
		Mana = 0,
		Energy = 1,
		Rage = 2
	}
}

