using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineManagerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	[Range(1, 20)]
	private int trampolinePoolSize = 5;

	[SerializeField]
	[Range(2, 20)]
	private int trampolineResolution = 5;

	[SerializeField]
	private GameObject trampolinePrefab = null;
	#endregion

	#region Private Variable Declarations.

	private static Queue<GameObject> trampolinePool;

	private Vector3 worldPos1 = Vector3.zero;
	private Vector3 worldPos2 = Vector3.zero;
	#endregion

	#region Private Functions.
	// Start is called before the first frame update
	void Start() {
		if (trampolinePrefab.GetComponent<TrampolineControllerScript>()) {
			//If the trampoline prefab is valid as it has the trampoline controller.
			//instantiate trampolines into the pool.
			trampolinePool = new Queue<GameObject>();
			for (int i = 0; i < trampolinePoolSize; i++) {
				//Spawn and parent the trampoline to the spawner.
				GameObject newTrampoline = Instantiate(trampolinePrefab, this.gameObject.transform);

				//Deactivate it.
				newTrampoline.SetActive(false);

				//Add it to the pool.
				trampolinePool.Enqueue(newTrampoline);
			}
		}
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			Debug.Log("Mouse Down.");
			//Reset the world positions for the new trampoline.
			worldPos1 = Vector3.zero;
			worldPos2 = Vector3.zero;

			//Get the first position when the user presses the mouse down.
			worldPos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		} else if (Input.GetMouseButtonUp(0)) {
			Debug.Log("Mouse Up.");
			//Get the second position for the trampoline.
			worldPos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			//Convert the points to vector2 and pass them to the spawn trampoline function.
			Vector2 v2Point1 = new Vector2(worldPos1.x, worldPos1.y);
			Vector2 v2Point2 = new Vector2(worldPos2.x, worldPos2.y);
			SpawnTrampoline(v2Point1, v2Point2, trampolineResolution);
		}
	}
	#endregion

	#region Public Access Functions (Getters and Setters).

	public static void SpawnTrampoline(Vector2 a_StartPoint, Vector2 a_EndPoint, int numberOfPoints) {
		//Get the next trampoline in the queue and make sure it is deactivated.
		GameObject newTrampoline = trampolinePool.Dequeue();
		newTrampoline.SetActive(false);

		//Get the trampoline controller.
		TrampolineControllerScript trampController = newTrampoline.GetComponent<TrampolineControllerScript>();

		//Update the trampolines points.
		trampController.SetTrampolinePoints(a_StartPoint, a_EndPoint, numberOfPoints);

		//Reactivate the trampoline.
		newTrampoline.SetActive(true);

		//Add it back to the queue.
		trampolinePool.Enqueue(newTrampoline);
	}

	#endregion
}
