using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour {
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			//Set the new checkpoint to the current player position.
			CheckpointManagerScript.SetSavedCheckpoint(collision.gameObject.transform.position);
		}
	}
}
