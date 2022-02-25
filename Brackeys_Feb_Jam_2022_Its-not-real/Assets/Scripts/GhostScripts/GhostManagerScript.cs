using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManagerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	private GameObject ghostModeUiTint = null;

	[SerializeField]
	[Range(0.0f, 1.0f)]
	private float ghostModeTimeScale = 0.0f;

	[SerializeField]
	[Range(2, 20)]
	private int trampolineResolution = 5;

	[SerializeField]
	private LineRenderer lineRenderer = null;
	#endregion

	#region Private Variable Declarations.
	private static bool ghostModeActive;
	private static float static_timeScale;


	private Vector3 worldPos1 = Vector3.zero;
	private Vector3 worldPos2 = Vector3.zero;
	#endregion

	#region Private Functions.
	// Start is called before the first frame update
	void Start() {
		lineRenderer.enabled = false;
		ghostModeActive = false;
		static_timeScale = ghostModeTimeScale;
	}

	// Update is called once per frame
	void Update() {
		//Check if the game is in "Ghost Mode"
		if (ghostModeActive) {
			//Activate the tint.
			if (ghostModeUiTint) {
				ghostModeUiTint.SetActive(true);
			}

			//Let the player draw trampolines.
			if (Input.GetMouseButtonDown(0)) {
				//Reset the world positions for the new trampoline.
				worldPos1 = Vector3.zero;
				worldPos2 = Vector3.zero;

				//Get the first position when the user presses the mouse down.
				worldPos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				//Acivate the dotted line.
				lineRenderer.enabled = true;
			} else if (Input.GetMouseButton(0)) {
				//If the player is holding down the left mouse button.
				//Draw the dotted line.
				Vector2 v2Point1 = new Vector2(worldPos1.x, worldPos1.y);
				worldPos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector2 v2Point2 = new Vector2(worldPos2.x, worldPos2.y);
				UpdateDottedLineMaterial(v2Point1, v2Point2);

			} else if (Input.GetMouseButtonUp(0)) {
				//Deactivate Dotted Line.
				lineRenderer.enabled = false;
				
				//Get the second position for the trampoline.
				worldPos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				//Convert the points to vector2 and pass them to the spawn trampoline function.
				Vector2 v2Point1 = new Vector2(worldPos1.x, worldPos1.y);
				Vector2 v2Point2 = new Vector2(worldPos2.x, worldPos2.y);
				TrampolineManagerScript.SpawnTrampoline(v2Point1, v2Point2, trampolineResolution);
			}
		} else {
			//Deactivate the tint.
			if (ghostModeUiTint) {
				ghostModeUiTint.SetActive(false);
			}
		}
	}
	#endregion

	#region Dotted Line Renderer Functions.
	private void UpdateDottedLineMaterial(Vector2 position1, Vector2 position2) {
		//Set up material.
		float width = lineRenderer.startWidth;
		lineRenderer.material.mainTextureScale = new Vector2(1.0f / width, 1.0f);

		//Update positions.
		lineRenderer.SetPosition(0, new Vector3(position1.x, position1.y, 0.0f));
		lineRenderer.SetPosition(1, new Vector3(position2.x, position2.y, 0.0f));
	}
	#endregion

	#region Public Access Functions (Getters and Setters).
	/// <summary>
	/// Freezes time and let's the ghost be controlled.
	/// </summary>
	public static void ActivateGhostMode() {
		ghostModeActive = true;
		Time.timeScale = static_timeScale;
		Time.fixedDeltaTime = Time.timeScale * 0.02f;
	}

	/// <summary>
	/// Unfreezes time and stops the ghost from being controlled.
	/// </summary>
	public static void DeactivateGhostMode() {
		ghostModeActive = false;
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
	}
	#endregion
}
