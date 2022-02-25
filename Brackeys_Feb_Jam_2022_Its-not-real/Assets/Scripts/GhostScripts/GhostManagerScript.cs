using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManagerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	private GameObject ghostModeUiTint = null;

	[SerializeField]
	private GameObject ghostGameObject = null;

	[SerializeField]
	[Range(0.0f, 1.0f)]
	private float ghostModeTimeScale = 0.0f;

	[SerializeField]
	[Range(0.0f, 10.0f)]
	private float minGhostDistance = 3.0f;

	[SerializeField]
	[Range(2, 20)]
	private int trampolineResolution = 5;

	[SerializeField]
	private LineRenderer lineRenderer = null;
	#endregion

	#region Private Variable Declarations.
	private static bool ghostModeActive;
	private static float static_timeScale;

	private GameObject playerGameObject = null;
	private Vector3 worldPos1 = Vector3.zero;
	private Vector3 worldPos2 = Vector3.zero;
	#endregion

	#region Private Functions.
	// Start is called before the first frame update
	void Start() {
		lineRenderer.enabled = false;
		ghostModeActive = false;
		static_timeScale = ghostModeTimeScale;
		playerGameObject = GameObject.FindGameObjectsWithTag("Player")[0];
	}

	// Update is called once per frame
	void Update() {
		//Check if the game is in "Ghost Mode"
		if (ghostModeActive) {
			//Activate the tint.
			if (ghostModeUiTint) {
				ghostModeUiTint.SetActive(true);
			}

			//Activate the ghost and make it follow the mouse.
			if (ghostGameObject) {
				ghostGameObject.SetActive(true);

				//Calculate new position for ghost.
				Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				newPos = new Vector3(newPos.x, newPos.y, 0.0f);
				Vector3 playerToMouseDir = newPos - (new Vector3(playerGameObject.transform.position.x, playerGameObject.transform.position.y, 0.0f));
				if (playerToMouseDir.magnitude <= minGhostDistance) {
					playerToMouseDir = playerToMouseDir.normalized;
					newPos = new Vector3(playerGameObject.transform.position.x + playerToMouseDir.x * minGhostDistance, playerGameObject.transform.position.y + playerToMouseDir.y * minGhostDistance, 0.0f);
				}

				//Apply new pos.
				ghostGameObject.transform.position = new Vector3(newPos.x, newPos.y, 0.0f);

				//Animate Ghost.
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

			//Deactivate the ghost.
			if (ghostGameObject) {
				ghostGameObject.SetActive(false);
			}
		}
	}

	private void GhostAnimation(bool start)
	{
		if (start)
		{

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
