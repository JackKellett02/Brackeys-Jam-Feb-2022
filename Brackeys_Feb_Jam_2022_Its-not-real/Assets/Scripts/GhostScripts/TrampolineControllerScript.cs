using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrampolineControllerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	private float launchSpeed = 10.0f;

	[SerializeField]
	[Range(0.0f, 1.0f)]
	private float oldVelocityInfluence = 0.0f;

	[SerializeField]
	private Vector2[] pointsVector2;

	[SerializeField]
	private GameObject normalTest = null;
	#endregion

	#region Private Variable Declarations.
	private LineRenderer trampolineLineRenderer;
	private EdgeCollider2D trampolineCollider;

	private Vector2 trampolineNormal = Vector2.zero;

	#endregion

	#region Private Functions.
	// Start is called before the first frame update
	void Start() {
		//Get the collider and line renderer.
		trampolineCollider = gameObject.GetComponent<EdgeCollider2D>();
		trampolineLineRenderer = gameObject.GetComponent<LineRenderer>();

		//Update the line.
		UpdateLinePoints();
		//Calculate The Normal Vector for the trampoline.
		trampolineNormal = CalculateTrampolineNormal();
	}

	// Update is called once per frame
	void Update() {
		//Update the line.
		UpdateLinePoints();

		//Calculate The Normal Vector for the trampoline.
		trampolineNormal = CalculateTrampolineNormal();
		normalTest.transform.up = new Vector3(trampolineNormal.x, trampolineNormal.y, 0.0f);
	}

	private Vector2 CalculateTrampolineNormal() {
		//Declare points to calculate Normal.
		Vector2 point1 = pointsVector2[0];
		Vector2 point2 = pointsVector2[pointsVector2.Length - 1];

		//Initialise return variable.
		Vector2 returnNormal = Vector2.zero;

		//Check the points are valid.
		if (point1 != point2) {
			//Calculate normal.
			float normalGradient = ((-1) / ((point2.y - point1.y) / (point2.x - point1.x)));
			Vector2 pointOnNormal1 = new Vector2(0, (point1.y - (normalGradient * point1.x)));
			Vector2 pointOnNormal2 = new Vector2(1, normalGradient + (point1.y - (normalGradient * point1.x)));
			returnNormal = pointOnNormal2 - pointOnNormal1;
			returnNormal = returnNormal.normalized;
		}

		//Return it.
		return returnNormal;
	}

	private void UpdateLinePoints() {
		//Add the points to the collider and line renderer.
		trampolineCollider.points = pointsVector2;
		trampolineLineRenderer.positionCount = pointsVector2.Length;
		Debug.Log(pointsVector2.Length + " = points array length");
		for (int i = 0; i < pointsVector2.Length; i++) {
			trampolineLineRenderer.SetPosition(i, pointsVector2[i]);
		}
	}

	private void LaunchCollision(Collider2D collision) {
		//Get collision info needed to launch the object that has touched the trampoline.
		Rigidbody2D collisionRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
		Vector2 colliderVelocity = collisionRigidbody.velocity;

		//Check what direction the collider is travelling and compare that
		//to the normal of the trampoline to make sure the collider is launched
		//in the correct direction.
		Vector2 launchNormal = trampolineNormal;
		float dotProduct = Vector2.Dot(colliderVelocity, launchNormal);

		//If the dot product is positive that means the normal is in the same direction as the
		//velocity so we need to flip it around for the player to be launched by the trampoline.
		if (dotProduct > 0.0f) {
			launchNormal = -launchNormal;
		}

		//Once we have the direction, launch the collider in that direction.
		Vector2 launchVelocity = launchNormal * launchSpeed + colliderVelocity * oldVelocityInfluence;
		launchVelocity = launchVelocity.normalized;
		collisionRigidbody.velocity = launchVelocity * launchSpeed;
	}
	#endregion


	#region Collider Trigger Functions
	private void OnTriggerEnter2D(Collider2D other) {
		//Check the collider is valid.
		Rigidbody2D colliderRigidbody2D = other.GetComponent<Rigidbody2D>();
		if (colliderRigidbody2D) {
			LaunchCollision(other);
		}
	}

	private void OnTriggerStay2D(Collider2D other) {

	}

	private void OnTriggerExit2D(Collider2D other) {

	}

	#endregion

	#region Public Access Functions (Getters And Setters).

	#endregion
}
