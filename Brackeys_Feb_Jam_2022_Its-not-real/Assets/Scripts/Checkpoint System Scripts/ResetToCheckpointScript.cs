using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetToCheckpointScript : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {
			//Set the player's velocity back to 0.
			collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

			//Move the player back to the last saved checkpoint.
			CheckpointManagerScript.ResetToCheckpoint();

			//Play a hurt sound.
			StartCoroutine(AudioManagerScript.PlaySoundEffect("OOF", Camera.main.gameObject.transform.position));
		}
	}
}
