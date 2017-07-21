using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DigitalRuby.PyroParticles;

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
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("CastSpell")) {
			Cast();
		}
	}


	public override void Cast() {
		Vector3 pos;
		float yRot = transform.rotation.eulerAngles.y;
		Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
		Vector3 forward = transform.forward;
		Vector3 right = transform.right;
		Vector3 up = transform.up;
		pos = transform.position + forward + right + up;
		Quaternion rotation = Quaternion.identity;
		GameObject spellPrefab = GameObject.Instantiate(abilities[0]);
		FireBaseScript currentPrefabScript = spellPrefab.GetComponent<FireBaseScript>();

		FireProjectileScript projectileScript = spellPrefab.GetComponentInChildren<FireProjectileScript>();
		if (projectileScript != null)
		{
			// make sure we don't collide with other friendly layers
			projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("FriendlyLayer"));
		}

		spellPrefab.transform.position = pos;
		spellPrefab.transform.rotation = rotation;
	}
}

