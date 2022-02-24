using System;
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
	[Range(0.01f, 1.0f)]
	private float stretchTime = 0.25f;

	[SerializeField]
	[Range(1.0f, 30.0f)]
	private float stretchLength = 10.0f;

	[SerializeField]
	private Transform centerPointTransform = null;
	#endregion

	#region Private Variable Declarations.
	private LineRenderer trampolineLineRenderer;
	private EdgeCollider2D trampolineCollider;

	private Vector2 trampolineNormal = Vector2.zero;
	private Vector2[] pointsVector2;
	private Vector2[] baseTrampolinePoints;
	private Vector3 baseMidpointValue;
	private int midpointint;
	private Transform originalCenterPoint;

	private bool trampolineCooldown = false;
	private bool trampolineAnimating = false;
	private float animationTimer = 0.0f;
	private Vector2 m_launchNormal = Vector3.zero;
	#endregion

	#region Private Functions.
	// Start is called before the first frame update
	void Awake() {
		//Get the collider and line renderer.
		trampolineCollider = gameObject.GetComponent<EdgeCollider2D>();
		trampolineLineRenderer = gameObject.GetComponent<LineRenderer>();
		trampolineCooldown = false;
	}

	// Update is called once per frame
	void Update() {
		if (trampolineAnimating) {
			if (animationTimer <= stretchTime) {
				//Add to timer.
				animationTimer += Time.deltaTime;

				//Animate the trampoline.
				AnimateTrampoline();
				UpdateLinePoints();
			} else {
				trampolineAnimating = false;
				trampolineCooldown = true;
				StartCoroutine("TrampolineCooldownTimer");
				animationTimer = 0.0f;
				Debug.Log("Trampoline Animation Over");
			}
		} else {
			centerPointTransform = originalCenterPoint;
			//for (int i = 0; i < baseTrampolinePoints.Length; i++) {
			//	pointsVector2[i] = new Vector2(baseTrampolinePoints[i].x, baseTrampolinePoints[i].y);
			//}
			UpdateLinePoints();
		}
	}

	private void UpdateLinePoints() {
		//Add the points to the collider and line renderer.
		List<Vector2> tempList = new List<Vector2>();
		for (int i = 0; i < pointsVector2.Length; i++) {
			tempList.Add(pointsVector2[i]);
		}
		trampolineCollider.SetPoints(tempList);
		trampolineLineRenderer.positionCount = pointsVector2.Length;
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
		collision.gameObject.GetComponent<PlayerPlatformer>().TrampolineBounce(launchVelocity * launchSpeed, centerPointTransform, stretchTime);

		//Animate the trampoline.
		trampolineAnimating = true;
		m_launchNormal = launchNormal;

		//Set center points transform.up to the launch normal.
		//centerPointTransform.up = new Vector3(launchNormal.x, launchNormal.y, 0.0f);
	}

	private void AnimateTrampoline() {
		if (animationTimer < (stretchTime / 2.0f)) {
			//Move center point in the opposite direction of the launch normal.
			centerPointTransform.position += ((-(new Vector3(m_launchNormal.x, m_launchNormal.y, 0.0f))) * stretchLength * Time.deltaTime);
		} else {
			//Move center point in the direction of the launch normal.
			centerPointTransform.position += ((new Vector3(m_launchNormal.x, m_launchNormal.y, 0.0f)) * stretchLength * Time.deltaTime);
		}

		//Bezier Calculation.
		Vector2 point1 = pointsVector2[0];
		Vector2 point2 = pointsVector2[pointsVector2.Length - 1];
		Vector2 midPoint = new Vector2(centerPointTransform.position.x, centerPointTransform.position.y);

		//Calculate Points
		for (int i = 1; i < pointsVector2.Length - 1; i++) { //Interpolate between the start and end point for the current item in the array.
			float percentageAlongArray = ((float)(i + 1)) / ((float)(pointsVector2.Length));

			//Bezier Calculation.
			Vector2 ab = Vector2.Lerp(point1, midPoint, percentageAlongArray);
			Vector2 bc = Vector2.Lerp(midPoint, point2, percentageAlongArray);

			//Update point in array.
			pointsVector2[i] = Vector2.Lerp(ab, bc, percentageAlongArray);
		}

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

	private IEnumerator TrampolineCooldownTimer()
	{
		yield return new WaitForSeconds(stretchTime);
		trampolineCooldown = false;
	}
	#endregion

	#region Collider Trigger Functions
	private void OnTriggerEnter2D(Collider2D other) {
		if (!trampolineCooldown) {
			//Check the collider is valid.
			Rigidbody2D colliderRigidbody2D = other.GetComponent<Rigidbody2D>();
			if (colliderRigidbody2D) {
				LaunchCollision(other);
			}
		}
	}

	private void OnTriggerStay2D(Collider2D other) {

	}

	private void OnTriggerExit2D(Collider2D other) {

	}

	#endregion

	#region Public Access Functions (Getters And Setters).

	public void SetTrampolinePoints(Vector2 a_StartPoint, Vector2 a_EndPoint, int numberOfPoints) {
		//Add the points to the trampoline.
		if ((numberOfPoints % 2) == 0) {
			//Make sure number of points is odd.
			numberOfPoints++;
		}

		//Set up the points array.
		pointsVector2 = new Vector2[numberOfPoints];
		for (int i = 0; i < numberOfPoints; i++) {
			//If it's the start or end of the array just set it to the start/endpoint.
			if (i == 0) {
				pointsVector2[i] = a_StartPoint;
			} else if (i == (numberOfPoints - 1)) {
				pointsVector2[i] = a_EndPoint;
			} else {//Interpolate between the start and end point for the current item in the array.
				float percentageAlongArray = ((float)(i + 1)) / ((float)(numberOfPoints));
				Vector2 pointInSpace = new Vector2();
				pointInSpace.x = Mathf.Lerp(a_StartPoint.x, a_EndPoint.x, percentageAlongArray);
				pointInSpace.y = Mathf.Lerp(a_StartPoint.y, a_EndPoint.y, percentageAlongArray);

				//Add it to the array.
				pointsVector2[i] = pointInSpace;
			}
		}

		//Save the base points.
		baseTrampolinePoints = new Vector2[pointsVector2.Length];

		for (int i = 0; i < pointsVector2.Length; i++) {
			baseTrampolinePoints[i] = new Vector2(pointsVector2[i].x, pointsVector2[i].y);
		}

		//Set base midpoint
		float midpoint = ((numberOfPoints / 2) - 0.5f) + 1.0f;
		int int_midpoint = (int)midpoint;
		midpointint = int_midpoint;
		baseMidpointValue = new Vector3(pointsVector2[midpointint].x, pointsVector2[midpointint].y, 0.0f);
		centerPointTransform.position = baseMidpointValue;
		originalCenterPoint = centerPointTransform;

		//Update the line.
		UpdateLinePoints();

		//Calculate Normal.
		trampolineNormal = CalculateTrampolineNormal();
	}
	#endregion
}
