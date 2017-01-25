using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerController : MonoBehaviour {

	#region Variables

	//Components
	Rigidbody rb;
	protected Animator animator;
	public GameObject target;

	//jumping variables
	public float gravity = -9.8f;
	bool canJump;
	bool isJumping = false;
	bool isGrounded;
	public float jumpSpeed = 500f;
	bool isFalling;
	bool startFall;
	float fallingVelocity = -1f;

	// Used for continuing momentum while in air
	public float inAirSpeed = 8f;
	float maxVelocity = 2f;
	float minVelocity = -2f;

	//movement variables
	bool canMove = true;
	public float walkSpeed = 1.35f;
	float moveSpeed;
	public float runSpeed = 6f;
	float rotationSpeed = 100f;
	public float mouseRotationSpeed = 300f;
	public float keyboardRotationSpeed = 100f;

	float x;
	float z;
	float dv;
	float dh;
	Vector3 moveVec;
	Vector3 strafeVec;

	Vector3 newVelocity;

	//Weapon and Shield
	private Weapon weapon;
	int rightWeapon = 0;
	int leftWeapon = 0;
	bool isRelax = false;

	//isStrafing/action variables
	bool canAction = true;
	bool isStrafing = false;
	bool isDead = false;
	bool isBlocking = false;
	public float knockbackMultiplier = 1f;
	bool isKnockback;

	#endregion

	#region Initialization

	void Start() {
		//set the animator component
		animator = GetComponentInChildren<Animator>();
		rb = GetComponent<Rigidbody>();
	}

	#endregion

	#region UpdateAndInput

	void Update() {

		//make sure there is animator on character
		if(animator) {
			if(canMove && !isBlocking && !isDead) {
				CameraRelativeMovement();
			} 

			Jumping();

			if(Input.GetButtonDown("Death") && canAction && isGrounded && !isBlocking) {
				if(!isDead) {
					StartCoroutine(_Death());
				} else {
					StartCoroutine(_Revive());
				}
			}

			//if strafing
			if(Input.GetKey(KeyCode.LeftShift) || Input.GetAxisRaw("TargetBlock") > .1 && canAction) {  
				isStrafing = true;
				animator.SetBool("Strafing", true);
				if(Input.GetButtonDown("CastL") && canAction && isGrounded && !isBlocking) {
					CastAttack(1);
				}
				if(Input.GetButtonDown("CastR") && canAction && isGrounded && !isBlocking) {
					CastAttack(2);
				}
			} else {
				isStrafing = false;
				animator.SetBool("Strafing", false);
			}
		} else {
			Debug.Log("ERROR: There is no animator for character.");
		}
	}

	#endregion

	#region Fixed/Late Updates

	void FixedUpdate() {
		CheckForGrounded();
		//apply gravity force
		rb.AddForce(0, gravity, 0, ForceMode.Acceleration);
		AirControl();
		//check if character can move
		if(canMove) {
			moveSpeed = UpdateMovement();  
		}
		//check if falling
		if(rb.velocity.y < fallingVelocity) {
			isFalling = true;
			animator.SetInteger("Jumping", 2);
			canJump = false;
		} else {
			isFalling = false;
		}
	}

	//get velocity of rigid body and pass the value to the animator to control the animations
	void LateUpdate() {
		//Get local velocity of charcter
		float velocityXel = transform.InverseTransformDirection(rb.velocity).x;
		float velocityZel = transform.InverseTransformDirection(rb.velocity).z;

		Debug.Log ("Velocity X: " + velocityXel.ToString ());
		Debug.Log ("Velocity Z: " + velocityZel.ToString ());
		//Update animator with movement values
		animator.SetFloat("Velocity X", runSpeed);
		animator.SetFloat("Velocity Z", runSpeed);
		//if character is alive and can move, set our animator
		if(!isDead && canMove) {
			if(moveSpeed > 0) {
				animator.SetBool("Moving", true);
			} else {
				animator.SetBool("Moving", false);
			}
		}
	}

	#endregion

	#region UpdateMovement

	void CameraRelativeMovement(){
		float inputTurnCharacter = Input.GetAxisRaw("Turn Character");
		float inputMoveCharacter = Input.GetAxisRaw("Move Character");
		float inputStrafeCharacter = Input.GetAxisRaw ("Strafe Character");

		if (Input.GetMouseButton (1)) {
			inputTurnCharacter = Input.GetAxisRaw ("Mouse X");
			rotationSpeed = mouseRotationSpeed;
		} else {
			rotationSpeed = keyboardRotationSpeed;
		}

		transform.Rotate (0, inputTurnCharacter * rotationSpeed * Time.deltaTime, 0);

		//converts control input vectors into camera facing vectors

		//Forward vector relative to the camera along the x-z plane   
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;

		Vector3 right = transform.TransformDirection(Vector3.right);
		right.y = 0;
		right = right.normalized;
		//directional inputs

		moveVec = inputMoveCharacter * forward;
		strafeVec = inputStrafeCharacter * right;
	}

	public static float ClampAngle(float angle, float min, float max) {
		if (angle < -360F) {
			angle += 360F;
		}
		if (angle > 360F) {
			angle -= 360F;
		}
		return Mathf.Clamp(angle, min, max);
	}

	float UpdateMovement() {
		//CameraRelativeMovement();  
		Vector3 motion = moveVec;
		if(isGrounded) {
			//reduce input for diagonal movement
			if(motion.magnitude > 1) {
				motion.Normalize();
			}
			if(canMove) {
				//set speed by walking / running
				if(isStrafing) {
					newVelocity = motion * walkSpeed;
				} else {
					newVelocity = motion * runSpeed;
				}
			}
		} else {
			//if we are falling use momentum
			//newVelocity = rb.velocity;
		}
		//newVelocity.y = rb.velocity.y;
		//rb.velocity = newVelocity;

		float inputMoveCharacter = Input.GetAxisRaw("Move Character");
		float inputStrafeCharacter = Input.GetAxisRaw ("Strafe Character");

		rb.velocity = new Vector3(inputStrafeCharacter, 0.0f, inputMoveCharacter) * runSpeed;

		//rb.MovePosition(transform.position + inputMoveCharacter * transform.forward * runSpeed * Time.deltaTime);
		//return a movement value for the animator
		return moveVec.magnitude;
	}

	#endregion

	#region Jumping

	//checks if character is within a certain distance from the ground, and markes it IsGrounded
	void CheckForGrounded() {
		float distanceToGround;
		float threshold = .45f;
		RaycastHit hit;
		Vector3 offset = new Vector3(0,.4f,0);
		if(Physics.Raycast((transform.position + offset), -Vector3.up, out hit, 100f)) {
			distanceToGround = hit.distance;
			if(distanceToGround < threshold) {
				isGrounded = true;
				canJump = true;
				startFall = false;
				isFalling = false;
				if(!isJumping) {
					animator.SetInteger("Jumping", 0);
				}
			} else {
				isGrounded = false;
			}
		}
	}

	void Jumping() {
		if(isGrounded) {
			if(canJump && Input.GetButtonDown("Jump")) {
				StartCoroutine(_Jump());
			}
		} else {    
			canJump = false;
			if(isFalling) {
				//set the animation back to falling
				animator.SetInteger("Jumping", 2);
				//prevent from going into land animation while in air
				if(!startFall) {
					animator.SetTrigger("JumpTrigger");
					startFall = true;
				}
			}
		}
	}

	IEnumerator _Jump() {
		isJumping = true;
		animator.SetInteger("Jumping", 1);
		animator.SetTrigger("JumpTrigger");
		// Apply the current movement to launch velocity
		//rb.velocity += jumpSpeed * Vector3.up;

		rb.velocity += Vector3.up * jumpSpeed;
		canJump = false;
		yield return new WaitForSeconds(.5f);
		isJumping = false;
	}

	void AirControl() {
		if(!isGrounded) {
			//CameraRelativeMovement();
			Vector3 motion = moveVec;
			motion *= (Mathf.Abs(moveVec.x) == 1 && Mathf.Abs(moveVec.z) == 1) ? 0.7f:1;
			rb.AddForce(motion * inAirSpeed, ForceMode.Acceleration);
			//limit the amount of velocity we can achieve
			float velocityX = 0;
			float velocityZ = 0;
			if(rb.velocity.x > maxVelocity) {
				velocityX = GetComponent<Rigidbody>().velocity.x - maxVelocity;
				if(velocityX < 0)
				{
					velocityX = 0;
				}
				rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
			}
			if(rb.velocity.x < minVelocity) {
				velocityX = rb.velocity.x - minVelocity;
				if(velocityX > 0) {
					velocityX = 0;
				}
				rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
			}
			if(rb.velocity.z > maxVelocity) {
				velocityZ = rb.velocity.z - maxVelocity;
				if(velocityZ < 0) {
					velocityZ = 0;
				}
				rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
			}
			if(rb.velocity.z < minVelocity) {
				velocityZ = rb.velocity.z - minVelocity;
				if(velocityZ > 0) {
					velocityZ = 0;
				}
				rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
			}
		}
	}

	#endregion

	#region MiscMethods

	//0 = No side
	//1 = Left
	//2 = Right
	//3 = Dual
	void Attack(int attackSide) {
		if(canAction) {
			if(weapon == Weapon.UNARMED) {
				int maxAttacks = 3;
				int attackNumber = 0;
				if(attackSide == 1 || attackSide == 3) {
					attackNumber = Random.Range(3, maxAttacks);
				} else if(attackSide == 2) {
					attackNumber = Random.Range(6, maxAttacks + 3);
				}
				if(isGrounded) {
					if(attackSide != 3) {
						animator.SetTrigger("Attack" + (attackNumber).ToString() + "Trigger");
						if(leftWeapon == 12 || leftWeapon == 14 || rightWeapon == 13 || rightWeapon == 15) {
							StartCoroutine(_LockMovementAndAttack(0, .75f));
						} else {
							StartCoroutine(_LockMovementAndAttack(0, .6f));
						}
					} else {
						animator.SetTrigger("AttackDual" + (attackNumber).ToString() + "Trigger");
						StartCoroutine(_LockMovementAndAttack(0, .75f));
					}
				}
			} else {
				if(isGrounded) {
					animator.SetTrigger("Attack" + (6).ToString() + "Trigger");
					StartCoroutine(_LockMovementAndAttack(0, .85f));
				}
			}
		}
	}

	void AttackKick(int kickSide) {
		if(isGrounded) {
			if(kickSide == 1) {
				animator.SetTrigger("AttackKick1Trigger");
			} else {
				animator.SetTrigger("AttackKick2Trigger");
			}
			StartCoroutine(_LockMovementAndAttack(0, .8f));
		}
	}

	//0 = No side
	//1 = Left
	//2 = Right
	//3 = Dual
	void CastAttack(int attackSide) {
		if(weapon == Weapon.UNARMED) {
			int maxAttacks = 3;
			if(attackSide == 1) {
				int attackNumber = Random.Range(0, maxAttacks);
				if(isGrounded) {
					animator.SetTrigger("CastAttack" + (attackNumber + 1).ToString() + "Trigger");
					StartCoroutine(_LockMovementAndAttack(0, .8f));
				}
			}
			if(attackSide == 2) {
				int attackNumber = Random.Range(3, maxAttacks + 3);
				if(isGrounded) {
					animator.SetTrigger("CastAttack" + (attackNumber + 1).ToString() + "Trigger");
					StartCoroutine(_LockMovementAndAttack(0, .8f));
				}
			}
			if(attackSide == 3) {
				int attackNumber = Random.Range(0, maxAttacks);
				if(isGrounded) {
					animator.SetTrigger("CastDualAttack" + (attackNumber + 1).ToString() + "Trigger");
					StartCoroutine(_LockMovementAndAttack(0, 1f));
				}
			}
		} 
	}

	IEnumerator _Knockback(Vector3 knockDirection, int knockBackAmount, int variableAmount) {
		isKnockback = true;
		StartCoroutine(_KnockbackForce(knockDirection, knockBackAmount, variableAmount));
		yield return new WaitForSeconds(.1f);
		isKnockback = false;
	}

	IEnumerator _KnockbackForce(Vector3 knockDirection, int knockBackAmount, int variableAmount) {
		while(isKnockback) {
			rb.AddForce(knockDirection * ((knockBackAmount + Random.Range(-variableAmount, variableAmount)) * (knockbackMultiplier * 10)), ForceMode.Impulse);
			yield return null;
		}
	}

	IEnumerator _Death() {
		animator.SetTrigger("Death1Trigger");
		StartCoroutine(_LockMovementAndAttack(.1f, 1.5f));
		isDead = true;
		animator.SetBool("Moving", false);
		moveVec = new Vector3(0, 0, 0);
		yield return null;
	}

	IEnumerator _Revive() {
		animator.SetTrigger("Revive1Trigger");
		isDead = false;
		yield return null;
	}

	#endregion

	#region _Coroutines

	//method to keep character from moveing while attacking, etc
	public IEnumerator _LockMovementAndAttack(float delayTime, float lockTime) {
		yield return new WaitForSeconds(delayTime);
		canAction = false;
		canMove = false;
		animator.SetBool("Moving", false);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		moveVec = new Vector3(0, 0, 0);
		animator.applyRootMotion = true;
		yield return new WaitForSeconds(lockTime);
		canAction = true;
		canMove = true;
		animator.applyRootMotion = false;
	}

	#endregion

	#region GUI

	void OnGUI() {
		
	}

	#endregion
}
