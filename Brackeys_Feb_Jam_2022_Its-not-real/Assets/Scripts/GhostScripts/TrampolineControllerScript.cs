using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrampolineControllerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	[Range(0.1f, 1.0f)]
	private float launchDelay = 0.25f;

	[SerializeField]
	private Vector2[] pointsVector2;
	
	[SerializeField]
	private GameObject normalTest = null;
	#endregion

	#region Private Variable Declarations.
	private float timeInTrampoline = 0.0f;
	private Rigidbody2D rigidbodyToLaunch = null;
	private LineRenderer trampolineLineRenderer;

	#endregion

	#region Private Functions.
	// Start is called before the first frame update
	void Start() {
		trampolineLineRenderer = gameObject.GetComponent<LineRenderer>();
		UpdateLinePoints();
	}

	// Update is called once per frame
	void Update() {
		if (timeInTrampoline >= launchDelay) {
			timeInTrampoline = 0.0f;
			LaunchCollision();
		}

		UpdateLinePoints();
		Vector2 normal = CalculateTrampolineNormal();
		Debug.Log(normal);
		normalTest.transform.up = new Vector3(normal.x, normal.y, 0.0f);
	}

	private void LaunchCollision() {

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

		//Retturn it.
		return returnNormal;
	}

	private void UpdateLinePoints()
	{
		trampolineLineRenderer.positionCount = pointsVector2.Length;
		Debug.Log(pointsVector2.Length + " = points array length");
		for (int i = 0; i < pointsVector2.Length; i++)
		{
			trampolineLineRenderer.SetPosition(i, pointsVector2[i]);
		}
	}
	#endregion


	#region Collider Trigger Functions
	private void OnTriggerEnter2D(Collider2D other) {
		//Check the collider is valid.
		Rigidbody2D colliderRigidbody2D = other.GetComponent<Rigidbody2D>();
		if (colliderRigidbody2D) {
			rigidbodyToLaunch = colliderRigidbody2D;
		}
	}

	private void OnTriggerStay2D(Collider2D other) {
		//Check the collider is valid.
		Rigidbody2D colliderRigidbody2D = other.GetComponent<Rigidbody2D>();
		if (colliderRigidbody2D) {
			if (colliderRigidbody2D == rigidbodyToLaunch) {
				//Add to timer.
				timeInTrampoline += Time.deltaTime;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other) {

	}

	#endregion

	#region Public Access Functions (Getters And Setters).

	#endregion
}
