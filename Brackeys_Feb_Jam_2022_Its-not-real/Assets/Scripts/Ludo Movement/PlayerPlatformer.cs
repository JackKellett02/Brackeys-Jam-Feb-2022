using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformer : MonoBehaviour {
	private float inputX;
	private bool grounded;
	private bool stretching = false;
	private Transform followLerp;

	private float jumpTimer;
	private float groundedTimer;
	[SerializeField]
	private float noKeySlowdown = 0.7f;

	private bool ghostMode = false;
	Rigidbody2D RB;

	[SerializeField]
	private bool canInput = true;
	[SerializeField]
	private float jumpBufferValue = 0.15f;
	[SerializeField]
	private float speed = 5;
	[SerializeField]
	private float jumpHeight = 5;

	// Start is called before the first frame update
	void Start() {
		RB = GetComponent<Rigidbody2D>();
		Animator = playerAnimationGameObject.GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update() {
		inputX = Input.GetAxisRaw("Horizontal");

		//Handling Jump Input Buffer values
		jumpTimer -= Time.deltaTime;
		if (Input.GetButtonDown("Jump") && canInput) {
			jumpTimer = jumpBufferValue;
		}
		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			canInput = !canInput;
			if (!ghostMode) {
				GhostManagerScript.ActivateGhostMode();
				ghostMode = true;

			} else {
				GhostManagerScript.DeactivateGhostMode();
				ghostMode = false;
			}
		}

		//Handle all animations.
		HandleAnimationTransitions();
	}

	private void FixedUpdate() {
		//Setting player horizontal velocity, only add up to max speed value, but allow player to keep faster velocities
		if (canInput) {
			float maxSpeed = speed * Time.deltaTime;
			if (grounded) {
				RB.velocity -= RB.velocity * new Vector2(0.9f, 0);
			}
			//If Player moves left
			if (inputX < 0) {
				//If they're moving faster than max left, slow back down a little
				//Otherwise, move max left
				if (RB.velocity.x < -maxSpeed) {
					RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, -maxSpeed, 0.02f), RB.velocity.y);
				} else {
					RB.velocity += new Vector2(-maxSpeed - RB.velocity.x, 0);
				}
			}
			//If Player moves right
			else if (inputX > 0) {
				//If they're moving faster than max right, slow back down a little
				//Otherwise, move max right
				if (RB.velocity.x > maxSpeed) {
					RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, maxSpeed, 0.02f), RB.velocity.y);
				} else {
					RB.velocity += new Vector2(maxSpeed - RB.velocity.x, 0);
				}
			}
			//If pressing no keys, slow down the player.
			else {
				if (RB.velocity.x > maxSpeed) {
					RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, maxSpeed, noKeySlowdown), RB.velocity.y);
				} else if (RB.velocity.x < -maxSpeed) {
					RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, -maxSpeed, noKeySlowdown), RB.velocity.y);
				} else {
					//RB.velocity -= RB.velocity * new Vector2(0.9f, 0);
					RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x, 0, 0.2f), RB.velocity.y);
				}

			}

		}

		//Keeps Velocity while in trampoline at zero and lerps towards the back point
		if (stretching) {
			RB.velocity = Vector2.zero;
			transform.position = Vector3.Lerp(followLerp.position, transform.position, 0.95f);
		}


		if (jumpTimer > 0.0f && grounded == true) {
			RB.velocity = new Vector2(RB.velocity.x, jumpHeight);
			jumpTimer = 0.0f;
		}
	}

	public void TrampolineBounce(Vector2 launchVelocity, Transform followPoint, float stretchTime) {
		StartCoroutine(TrampolineStretch(launchVelocity, stretchTime));
		canInput = false;
		stretching = true;
		RB.gravityScale = 0;
		followLerp = followPoint;
	}

	IEnumerator TrampolineStretch(Vector2 launchVelocity, float stretchTime) {
		yield return new WaitForSeconds(stretchTime);
		RB.velocity = launchVelocity;
		canInput = true;
		stretching = false;
		RB.gravityScale = 1.5f;
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (collision.tag == "Room") {
			Camera.main.GetComponent<CameraController>().SwitchRooms(collision);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "Ground") {
			grounded = true;
		}
	}

	private void OnCollisionExit2D(Collision2D collision) {
		if (collision.gameObject.tag == "Ground") {
			grounded = false;
		}
	}

	#region Animation Code.
	//Animation Variables.
	[SerializeField]
	private GameObject playerAnimationGameObject = null;

	private Animator Animator;

	private void HandleAnimationTransitions() {
		//Handle sprite rotation
		if (inputX < 0) {
			//Rotate the sprite left.
			playerAnimationGameObject.transform.right = Vector3.left;
		}

		if (inputX > 0) {
			//Rotate the sprite right.
			playerAnimationGameObject.transform.right = Vector3.right;
		}

		//If no other animator should play then player is idle.
		Animator.SetBool("JumpUp", false);
		Animator.SetBool("JumpDown", false);
		Animator.SetBool("Running", false);
		Animator.SetBool("Stretching", false);
		Animator.SetBool("Idle", true);
		if (inputX <= -1.0f || inputX >= 1.0f && grounded) {
			//Player is running.
			Animator.SetBool("JumpUp", false);
			Animator.SetBool("JumpDown", false);
			Animator.SetBool("Running", true);
			Animator.SetBool("Stretching", false);
			Animator.SetBool("Idle", false);
		}
		if (RB.velocity.y > 0.03f) {
			//Player is moving up.
			Animator.SetBool("JumpUp", true);
			Animator.SetBool("JumpDown", false);
			Animator.SetBool("Running", false);
			Animator.SetBool("Stretching", false);
			Animator.SetBool("Idle", false);
		}
		if (RB.velocity.y < -0.03f) {
			//Player is moving down.
			Animator.SetBool("JumpUp", false);
			Animator.SetBool("JumpDown", true);
			Animator.SetBool("Running", false);
			Animator.SetBool("Stretching", false);
			Animator.SetBool("Idle", false);
		}
		if (stretching) {
			//Player is on the trampoline.
			Animator.SetBool("JumpUp", false);
			Animator.SetBool("JumpDown", false);
			Animator.SetBool("Running", false);
			Animator.SetBool("Stretching", true);
			Animator.SetBool("Idle", false);
		}
	}
	#endregion
}
