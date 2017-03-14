using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerController : MonoBehaviour {

	#region Variables

	//Components
	Rigidbody rb;
	Camera playerCamera;

	//Targeting variables
	GameObject target;
	GameObject[] possibleTargets;
	List<GameObject> previouslySelectedTargets = new List<GameObject>();
	int maxTargetAngle = 65;
	int maxTargetDistance = 45;

	//movement variables
	bool isWalking = false; //Make a toggle keybind for this
	int walkSpeed = 250;
	int runSpeed = 500;
	float rotationSpeed;
	float mouseRotationSpeed = 300f;
	float keyboardRotationSpeed = 100f;

	//jump variables
	float jumpSpeed = 5.5f;
	bool isGrounded = true;
	float groundedThreshold = 0.1f;

	//spellcasting variables
	bool isSpellCasting = false;
	float remainingCastTime = 0.0f;
	float totalCastTime = 2.0f;
	float spellWidth = 150.0f;
	float spellHeight = 30.0f;
	public float spellX = 0.0f;
	public float spellY = 0.0f;


	Vector3 movementDirection;

	#endregion

	#region Initialization

	void Start() {
		//cache the rigidbody component
		rb = GetComponent<Rigidbody>();
		playerCamera = rb.GetComponentInChildren<Camera>();
		spellX = Screen.width / 2 - spellWidth / 2;
		spellY = Screen.height - 50;
	}

	#endregion

	#region Update

	void Update() {

		updateMovement();
		if(Input.GetButtonDown("Jump")) {
			Debug.Log("Velocity: " + rb.velocity.ToString());
			//Prevents multiple jumps if frame rendering lags
			isGrounded = false;

			//Update canJump if Player is on the ground
			checkGrounded();

			//Jump if able
			if(isGrounded) {
				jump();
			}
		}
			
		if(Input.GetButtonDown("TargetEnemy")) {
			selectEnemyTarget();
		}

		if(Input.GetButtonDown("CastSpell")) {
			castSpell(1);
		}

		if(isSpellCasting) {
			updateRemainingSpellcastTime();
		}
			
	}

	#endregion

	#region UpdateMovement

	//To-Do: Add movement with both mouse buttons pressed
	void updateMovement() {
		//Get all movement related inputs
		float inputTurnCharacter = Input.GetAxisRaw("Turn Character");
		float inputMoveCharacter = Input.GetAxisRaw("Move Character");
		float inputStrafeCharacter = Input.GetAxisRaw("Strafe Character");

		//Rotate character using right-click drag
		//Different rotation speeds for keyboard or mouse turning
		if(Input.GetMouseButton(1)) {
			inputTurnCharacter = Input.GetAxisRaw("Mouse X");
			rotationSpeed = mouseRotationSpeed;
		} else {
			rotationSpeed = keyboardRotationSpeed;
		}

		//Rotate character if turn inputs were detected
		if(inputTurnCharacter != 0) {
			transform.Rotate(0, inputTurnCharacter * rotationSpeed * Time.deltaTime, 0);
		}

		//Check if grounded before moving
		checkGrounded();
		if(isGrounded) {
			//Forward vector from the player's POV 
			Vector3 forward = transform.TransformDirection(Vector3.forward);
			forward.y = 0;
			forward = forward.normalized;

			//Right vector from the player's POV
			Vector3 right = transform.TransformDirection(Vector3.right);
			right.y = 0;
			right = right.normalized;

			//Create the current direction vector based on inputs
			movementDirection = inputMoveCharacter * forward + inputStrafeCharacter * right;
			//Preserve the y velocity
			//movementDirection.y = rb.velocity.y;

			//Move the player based on walk/run speed
			if(isWalking) {
				rb.velocity = Vector3.up * rb.velocity.y + movementDirection * walkSpeed * Time.deltaTime;
				//rb.MovePosition(transform.position + movementDirection * walkSpeed * Time.deltaTime);
			} else {
				rb.velocity = Vector3.up * rb.velocity.y + movementDirection * runSpeed * Time.deltaTime;
				//rb.MovePosition(transform.position + movementDirection * runSpeed * Time.deltaTime);
			}
		}
	}

	#endregion

	#region Targeting

	void selectEnemyTarget() {

		//Used for determining if enemies are in the current camera view
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

		//Find enemies that are in front of the player sorted by distance
		//To-Do: Ensure this "Where" clause will not cause performance issues
		possibleTargets = GameObject.FindGameObjectsWithTag("Enemy")
			.Where(x => Vector3.Angle(rb.transform.forward, x.transform.position - rb.transform.position) < maxTargetAngle
				&& Vector3.Distance(rb.transform.position,x.transform.position) < maxTargetDistance
				&& GeometryUtility.TestPlanesAABB(planes, x.GetComponent<Collider>().bounds)
				&& x != target)
			.OrderBy(x => Vector3.Distance(rb.transform.position,x.transform.position)).ToArray();

		//Deselect previous target if it exists
		if(target != null) {
			target.GetComponent<MeshRenderer>().material.color = Color.white;
		}

		for(int i = 0; i < possibleTargets.Length; i++) {
			//If this target has not been selected previously then select it
			//Otherwise if all possible targets have been previously selected 
			//then select the first possibleTarget and clear previouslySelectedTargets
			if(!previouslySelectedTargets.Contains(possibleTargets[i])) {
				target = possibleTargets[i];
				break;
			} else if(i == possibleTargets.Length - 1) {
				previouslySelectedTargets.Clear();
				target = possibleTargets[0];
			}
		}

		//Add target to previouslySelected if other possibleTargets were available
		if(possibleTargets.Length > 0) {
			previouslySelectedTargets.Add(target);
		}

		//Select new target
		target.GetComponent<MeshRenderer>().material.color = Color.red;
	}

	#endregion

	#region Jumping

	//To-Do: Need to figure out why jumping sometimes causes x/z velocity to increase
	void jump() {
		isGrounded = false;
		rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
	}

	void checkGrounded() {
		if((Physics.Raycast(rb.transform.position, Vector3.down, groundedThreshold) && rb.velocity.y < 0)
			|| rb.velocity.y == 0) {
			isGrounded = true;
		}
	}

	#endregion

	#region Spellcasting

	void castSpell(int spellID) {
		isSpellCasting = true;
		remainingCastTime = 2.0f;
	}

	void updateRemainingSpellcastTime() {
		if(remainingCastTime <= 0) {
			remainingCastTime = 0;
			isSpellCasting = false;
		} else {
			if(remainingCastTime - Time.deltaTime < 0) {
				remainingCastTime = 0;
			} else {
				remainingCastTime -= Time.deltaTime;
			}
		}
	}

	#endregion

	#region GUI

	void OnGUI() {
		if(isSpellCasting) {
			GUIStyle defaultStyle = new GUIStyle(GUI.skin.box);
			GUI.Box(new Rect(spellX, spellY, spellWidth, spellHeight), remainingCastTime.ToString(), defaultStyle);
			GUIStyle remainingCastStyle = new GUIStyle(GUI.skin.box);
			remainingCastStyle.border.left = 1;
			remainingCastStyle.border.right = 1;
			GUI.backgroundColor = Color.red;
			GUI.Box(new Rect(spellX, spellY, spellWidth * remainingCastTime / totalCastTime, spellHeight), "", remainingCastStyle);
		}
	}

	#endregion
}
