using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManagerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).

	#endregion

	#region Private Variable Declarations.
	private static GameObject playerGameObject = null;

	private static Vector3 checkPointPos = Vector3.zero;
	#endregion

	#region Private Functions.
	// Start is called before the first frame update
	void Start() {
		playerGameObject = GameObject.FindGameObjectsWithTag("Player")[0];
		checkPointPos = playerGameObject.transform.position;
	}

	// Update is called once per frame
	void Update() {

	}
	#endregion

	#region Public Access Functions (Getters and Setters).

	public static void SetSavedCheckpoint(Vector3 a_position) {
		checkPointPos = a_position;
	}

	public static void ResetToCheckpoint() {
		playerGameObject.transform.position = checkPointPos;
	}
	#endregion
}
