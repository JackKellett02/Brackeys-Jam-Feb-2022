using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndPointScript : MonoBehaviour {
	[SerializeField]
	[Range(10.0f, 1000.0f)]
	private float upSpeed = 100.0f;

	[SerializeField]
	[Range(1.0f, 25.0f)]
	private float delayTillUITint = 5.0f;

	[SerializeField]
	[Range(1.0f, 100.0f)]
	private float alphaIncreaseMultiplier = 10.0f;

	[SerializeField]
	[Range(0.5f, 2.0f)]
	private float timeTillBlack = 1.0f;

	[SerializeField]
	private Image endingTint = null;

	[SerializeField]
	private Image endText = null;

	private bool canAnimateTint = false;
	private bool canShowText = false;
	private bool timerTillMenu = false;

	private float timer = 0.0f;

	private Rigidbody2D playerRB = null;

	private void Update() {
		if (canAnimateTint) {
			playerRB.velocity = Vector2.zero;
			Color colourToAdd = Color.white;
			colourToAdd.a = alphaIncreaseMultiplier * Time.deltaTime;
			endingTint.color += colourToAdd;
			timer += Time.deltaTime;

			if (timer >= timeTillBlack) {
				canShowText = true;
				timer = 0.0f;
			}
		}

		if (canShowText) {
			playerRB.velocity = Vector2.zero;
			canAnimateTint = false;

			//Fade to black and show text.
			endText.gameObject.SetActive(true);
			timer += Time.deltaTime;
			float percentage = timer / timeTillBlack;

			endingTint.color = Color.Lerp(Color.white, Color.black, percentage);

			if (endingTint.color == Color.black)
			{
				timer = 0.0f;
				canShowText = false;

				//Start timer to go back to menu.
				timerTillMenu = true;
				StartCoroutine("TimerTillMenuReturn");

				//Play a hurt sound.
				StartCoroutine(AudioManagerScript.PlaySoundEffect("OOF", Camera.main.gameObject.transform.position));
			}
		}

		if (timerTillMenu)
		{
			playerRB.velocity = Vector2.zero;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {
			//Add a massive amount of upwards velocity to the player.
			collision.gameObject.GetComponent<Rigidbody2D>().velocity += (Vector2.up * upSpeed);
			playerRB = collision.gameObject.GetComponent<Rigidbody2D>();

			//Start a swoosh sound.
			StartCoroutine(AudioManagerScript.PlaySoundEffect("ArrowSwoosh", collision.gameObject.transform.position));

			//Start timer till starting tint.
			StartCoroutine("TimerTillTintAnimation");

			endingTint.rectTransform.localScale = Vector3.one * 3;
		}
	}

	private IEnumerator TimerTillTintAnimation() {
		yield return new WaitForSeconds(delayTillUITint);
		canAnimateTint = true;
	}

	private IEnumerator TimerTillMenuReturn()
	{
		yield return new WaitForSeconds(timeTillBlack);
		SceneManager.LoadScene(0);
	}
}
