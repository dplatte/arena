using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummyController : MonoBehaviour {

	private int baseHealth = 100;
	private int curHealth = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int BaseHealth {
		get { return baseHealth; }
		set { baseHealth = value; }
	}

	public int CurHealth {
		get { return curHealth; }
		set { curHealth = value; }
	}

	public void receiveDamage(int damage) {
		
		if (curHealth - damage < 0) {
			curHealth = 0;
		} else {
			curHealth -= damage;
		}
	}

	void OnGUI() {

	}
}
