using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineManagerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	[Range(1, 20)]
	private int trampolinePoolSize = 5;

	[SerializeField]
	private GameObject trampolinePrefab = null;
	#endregion

	#region Private Variable Declarations.

	private static Queue<GameObject> trampolinePool;


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

	}

	#endregion

	#region Public Access Functions (Getters and Setters).

	public static void SpawnTrampoline(Vector2 a_StartPoint, Vector2 a_EndPoint, int numberOfPoints) {
		//Get the next trampoline in the queue and make sure it is deactivated.
		GameObject newTrampoline = trampolinePool.Dequeue();
		newTrampoline.SetActive(false);

		//Get the trampoline controller.
		TrampolineControllerScript trampController = newTrampoline.GetComponent<TrampolineControllerScript>();

		//Reactivate the trampoline.
		newTrampoline.SetActive(true);

		//Update the trampolines points.
		trampController.SetTrampolinePoints(a_StartPoint, a_EndPoint, numberOfPoints);

		//Add it back to the queue.
		trampolinePool.Enqueue(newTrampoline);
	}

	#endregion
}
