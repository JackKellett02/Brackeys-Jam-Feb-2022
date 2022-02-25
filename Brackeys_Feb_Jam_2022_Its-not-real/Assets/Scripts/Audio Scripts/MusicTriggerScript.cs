using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicTriggerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	private AudioClip musicClip = null;

	[SerializeField]
	private bool loopMusic = true;
	#endregion

	#region Private Variable Declarations.

	private bool canPlayAgain = true;
	private static string currentClipName = string.Empty;
	#endregion

	#region Private Functions.

	private void Start()
	{
		currentClipName = string.Empty;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player" && currentClipName != musicClip.name)
		{
			StopCoroutine(MusicTriggerCooldown());
			canPlayAgain = true;
		}

		if (collision.gameObject.tag == "Player" && canPlayAgain && musicClip != null)
		{
			currentClipName = musicClip.name;
			AudioManagerScript.PlayMusicClip(musicClip.name, loopMusic);
			canPlayAgain = false;

			if (!loopMusic)
			{
				StartCoroutine(MusicTriggerCooldown());
			}
		}
	}

	private IEnumerator MusicTriggerCooldown()
	{
		yield return new WaitForSeconds(musicClip.length);
		canPlayAgain = true;
	}
	#endregion
}
